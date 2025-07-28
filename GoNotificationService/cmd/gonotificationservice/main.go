package main

import (
	"context"
	"log"
	"net/http"
	"os"

	"github.com/go-redis/redis/v8"
	"github.com/joho/godotenv" // Thêm import này

	app "gonotificationservice/internal/application"
	delivery "gonotificationservice/internal/delivery/http"
	infraRedis "gonotificationservice/internal/infrastructure/redis"
	infraSmtp "gonotificationservice/internal/infrastructure/smtp"
)

func main() {
	// Tải biến môi trường từ tệp .env
	err := godotenv.Load()
	if err != nil {
		log.Printf("Không tìm thấy tệp .env hoặc lỗi khi tải: %v. Tiếp tục với biến môi trường hệ thống.", err)
	}

	ctx := context.Background()

	// Cấu hình Redis
	redisAddr := os.Getenv("REDIS_ADDR")
	if redisAddr == "" {
		redisAddr = "localhost:6379" // Địa chỉ Redis mặc định
	}

	redisPassword := os.Getenv("REDIS_PASSWORD") // Đọc mật khẩu từ biến môi trường

	rdb := redis.NewClient(&redis.Options{
		Addr:     redisAddr,
		Password: redisPassword, // Sử dụng mật khẩu từ biến môi trường
		DB:       0,  // Sử dụng DB mặc định
	})

	// Ping Redis để kiểm tra kết nối
	_, err = rdb.Ping(ctx).Result()
	if err != nil {
		log.Fatalf("Không thể kết nối đến Redis: %v", err)
	}
	log.Printf("Đã kết nối đến Redis tại %s", redisAddr)

	// Khởi tạo SMTP Sender
	smtpSender, err := infraSmtp.NewSmtpSender()
	if err != nil {
		log.Fatalf("Không thể khởi tạo SMTP Sender: %v", err)
	}

	// Khởi tạo các thành phần theo kiến trúc sạch
	redisSender := infraRedis.NewRedisSender(rdb)
	notificationService := app.NewNotificationService(redisSender, smtpSender)
	notificationHandler := delivery.NewNotificationHandler(notificationService)

	// Cấu hình router HTTP
	http.HandleFunc("/send", notificationHandler.SendNotification)
	http.HandleFunc("/send-email", notificationHandler.SendEmail) // Thêm endpoint gửi email

	// Cấu hình cổng lắng nghe
	port := os.Getenv("PORT")
	if port == "" {
		port = "8080" // Cổng mặc định
	}

	log.Printf("Dịch vụ thông báo Go đang lắng nghe trên :%s", port)
	log.Fatal(http.ListenAndServe(":"+port, nil))
}
