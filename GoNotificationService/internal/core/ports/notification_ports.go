package ports

import (
	"context"

	"gonotificationservice/internal/core/domain"
)

type NotificationSender interface {
	Send(ctx context.Context, channel, message string) error
}

type EmailSender interface {
	SendEmail(ctx context.Context, to, subject, body string) error
}

type NotificationService interface {
	SendNotification(ctx context.Context, payload domain.NotificationPayload) error
	SendEmail(ctx context.Context, payload domain.EmailPayload) error
}
