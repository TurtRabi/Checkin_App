package main

import (
	"fmt"
	"log"
	"os"

	"github.com/joho/godotenv" // Thêm import này
)

func main() {
	// Tải biến môi trường từ tệp .env
	err := godotenv.Load()
	if err != nil {
		log.Printf("Không tìm thấy tệp .env hoặc lỗi khi tải: %v. Tiếp tục với biến môi trường hệ thống.", err)
	}

	redisAddr := os.Getenv("REDIS_ADDR")
	redisPassword := os.Getenv("REDIS_PASSWORD")
	smtpHost := os.Getenv("SMTP_HOST")
	smtpPort := os.Getenv("SMTP_PORT")
	smtpUsername := os.Getenv("SMTP_USERNAME")
	smtpPassword := os.Getenv("SMTP_PASSWORD")
	smtpFrom := os.Getenv("SMTP_FROM")
	smtpSenderName := os.Getenv("SMTP_SENDER_NAME") // Thêm dòng này

	fmt.Printf("REDIS_ADDR from Go: '%s'\n", redisAddr)
	fmt.Printf("REDIS_PASSWORD from Go: '%s'\n", redisPassword)
	fmt.Printf("SMTP_HOST from Go: '%s'\n", smtpHost)
	fmt.Printf("SMTP_PORT from Go: '%s'\n", smtpPort)
	fmt.Printf("SMTP_USERNAME from Go: '%s'\n", smtpUsername)
	fmt.Printf("SMTP_PASSWORD from Go: '%s'\n", smtpPassword)
	fmt.Printf("SMTP_FROM from Go: '%s'\n", smtpFrom)
	fmt.Printf("SMTP_SENDER_NAME from Go: '%s'\n", smtpSenderName) // Thêm dòng này

	if redisAddr == "" {
		fmt.Println("REDIS_ADDR is empty in Go program.")
	}
}