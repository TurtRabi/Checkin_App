package smtp

import (
	"context"
	"fmt"
	"log"
	"net/smtp"
	"os"

	"gonotificationservice/internal/core/ports"
)

type smtpSender struct {
	host     string
	port     string
	username string
	password string
	from     string
	senderName string // Thêm trường này
}

func NewSmtpSender() (ports.EmailSender, error) {
	host := os.Getenv("SMTP_HOST")
	port := os.Getenv("SMTP_PORT")
	username := os.Getenv("SMTP_USERNAME")
	password := os.Getenv("SMTP_PASSWORD")
	from := os.Getenv("SMTP_FROM")
	senderName := os.Getenv("SMTP_SENDER_NAME") // Đọc biến môi trường mới

	if host == "" || port == "" || username == "" || password == "" || from == "" || senderName == "" {
		return nil, fmt.Errorf("Thiếu cấu hình SMTP: SMTP_HOST, SMTP_PORT, SMTP_USERNAME, SMTP_PASSWORD, SMTP_FROM, SMTP_SENDER_NAME phải được đặt")
	}

	return &smtpSender{
		host:     host,
		port:     port,
		username: username,
		password: password,
		from:     from,
		senderName: senderName, // Gán giá trị
	},	nil
}

func (s *smtpSender) SendEmail(ctx context.Context, to, subject, body string) error {
	addr := fmt.Sprintf("%s:%s", s.host, s.port)
	auth := smtp.PlainAuth("", s.username, s.password, s.host)

	msg := []byte("To: " + to + "\r\n" +
		"From: " + s.senderName + " <" + s.from + ">\r\n" + // Sử dụng senderName ở đây
		"Subject: " + subject + "\r\n" +
		"Content-Type: text/plain; charset=UTF-8\r\n" +
		"\r\n" + body)

	err := smtp.SendMail(addr, auth, s.from, []string{to}, msg)
	if err != nil {
		log.Printf("Lỗi gửi email đến %s: %v", to, err)
		return err
	}
	log.Printf("Email đã gửi thành công đến %s với chủ đề: %s", to, subject)
	return nil
}
