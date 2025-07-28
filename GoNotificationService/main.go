package main

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"
	"time"

	"github.com/go-redis/redis/v8"
)

// NotificationPayload đại diện cho cấu trúc của yêu cầu JSON đến
type NotificationPayload struct {
	Channel string `json:"channel"`
	Message string `json:"message"`
}

var ctx = context.Background()
var rdb *redis.Client

func init() {
	// Khởi tạo Redis client
	// Có thể cấu hình địa chỉ Redis qua biến môi trường REDIS_ADDR
	redisAddr := os.Getenv("REDIS_ADDR")
	if redisAddr == "" {
		redisAddr = "localhost:6379" // Địa chỉ Redis mặc định
	}

	rdb = redis.NewClient(&redis.Options{
		Addr:     redisAddr,
		Password: "", // Không có mật khẩu
		DB:       0,  // Sử dụng DB mặc định
	})

	// Ping Redis để kiểm tra kết nối
	_, err := rdb.Ping(ctx).Result()
	if err != nil {
		log.Fatalf("Không thể kết nối đến Redis: %v", err)
	}
	log.Printf("Đã kết nối đến Redis tại %s", redisAddr)
}

// sendNotificationHandler xử lý các yêu cầu gửi thông báo
func sendNotificationHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Chỉ cho phép phương thức POST", http.StatusMethodNotAllowed)
		return
	}

	var payload NotificationPayload
	err := json.NewDecoder(r.Body).Decode(&payload)
	if err != nil {
		http.Error(w, fmt.Sprintf("Payload yêu cầu không hợp lệ: %v", err), http.StatusBadRequest)
		return
	}

	if payload.Channel == "" || payload.Message == "" {
		http.Error(w, "Kênh (Channel) và Tin nhắn (Message) không được để trống", http.StatusBadRequest)
		return
	}

	// Tạo một context với timeout cho hoạt động Redis
	redisCtx, cancel := context.WithTimeout(ctx, 5*time.Second)
	defer cancel()

	err = rdb.Publish(redisCtx, payload.Channel, payload.Message).Err()
	if err != nil {
		log.Printf("Không thể publish tin nhắn đến kênh Redis %s: %v", payload.Channel, err)
		http.Error(w, fmt.Sprintf("Không thể gửi thông báo: %v", err), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	json.NewEncoder(w).Encode(map[string]string{"status": "success", "message": "Thông báo đã được gửi"})
	log.Printf("Thông báo đã gửi đến kênh '%s': '%s'", payload.Channel, payload.Message)
}

func main() {
	http.HandleFunc("/send", sendNotificationHandler)

	// Có thể cấu hình cổng qua biến môi trường PORT
	port := os.Getenv("PORT")
	if port == "" {
		port = "8080" // Cổng mặc định
	}

	log.Printf("Dịch vụ thông báo Go đang lắng nghe trên :%s", port)
	log.Fatal(http.ListenAndServe(":"+port, nil))
}
