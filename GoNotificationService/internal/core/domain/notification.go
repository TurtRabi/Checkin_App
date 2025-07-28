package domain

type NotificationPayload struct {
	Channel string `json:"channel"`
	Message string `json:"message"`
}
