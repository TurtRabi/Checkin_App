package main

import (
	"context"
	"log"
	"net/http"
	"os"

	"github.com/go-redis/redis/v8"

	app "gonotificationservice/internal/application"
	delivery "gonotificationservice/internal/delivery/http"
	infra "gonotificationservice/internal/infrastructure/redis"
)

func main() {
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
	_, err := rdb.Ping(ctx).Result()
	if err != nil {
		log.Fatalf("Không thể kết nối đến Redis: %v", err)
	}
	log.Printf("Đã kết nối đến Redis tại %s", redisAddr)

	// Khởi tạo các thành phần theo kiến trúc sạch
	redisSender := infra.NewRedisSender(rdb)
	notificationService := app.NewNotificationService(redisSender)
	notificationHandler := delivery.NewNotificationHandler(notificationService)

	// Cấu hình router HTTP
	http.HandleFunc("/send", notificationHandler.SendNotification)

	// Cấu hình cổng lắng nghe
	port := os.Getenv("PORT")
	if port == "" {
		port = "8080" // Cổng mặc định
	}

	log.Printf("Dịch vụ thông báo Go đang lắng nghe trên :%s", port)
	log.Fatal(http.ListenAndServe(":"+port, nil))
}
