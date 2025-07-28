package application

import (
	"context"

	"gonotificationservice/internal/core/domain"
	"gonotificationservice/internal/core/ports"
)

type notificationService struct {
	notificationSender ports.NotificationSender
	emailSender        ports.EmailSender
}

func NewNotificationService(notificationSender ports.NotificationSender, emailSender ports.EmailSender) ports.NotificationService {
	return &notificationService{
		notificationSender: notificationSender,
		emailSender:        emailSender,
	}
}

func (s *notificationService) SendNotification(ctx context.Context, payload domain.NotificationPayload) error {
	// Ở đây bạn có thể thêm bất kỳ logic nghiệp vụ nào trước khi gửi
	// Ví dụ: kiểm tra payload, log, v.v.
	return s.notificationSender.Send(ctx, payload.Channel, payload.Message)
}

func (s *notificationService) SendEmail(ctx context.Context, payload domain.EmailPayload) error {
	// Ở đây bạn có thể thêm bất kỳ logic nghiệp vụ nào trước khi gửi email
	return s.emailSender.SendEmail(ctx, payload.To, payload.Subject, payload.Body)
}
