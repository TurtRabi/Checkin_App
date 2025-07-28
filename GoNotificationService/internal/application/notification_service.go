package application

import (
	"context"

	"gonotificationservice/internal/core/domain"
	"gonotificationservice/internal/core/ports"
)

type notificationService struct {
	sender ports.NotificationSender
}

func NewNotificationService(sender ports.NotificationSender) ports.NotificationService {
	return &notificationService{
		sender: sender,
	}
}

func (s *notificationService) SendNotification(ctx context.Context, payload domain.NotificationPayload) error {
	// Ở đây bạn có thể thêm bất kỳ logic nghiệp vụ nào trước khi gửi
	// Ví dụ: kiểm tra payload, log, v.v.
	return s.sender.Send(ctx, payload.Channel, payload.Message)
}
