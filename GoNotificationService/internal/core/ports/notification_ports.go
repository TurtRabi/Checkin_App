package ports

import (
	"context"

	"gonotificationservice/internal/core/domain"
)

type NotificationSender interface {
	Send(ctx context.Context, channel, message string) error
}

type NotificationService interface {
	SendNotification(ctx context.Context, payload domain.NotificationPayload) error
}
