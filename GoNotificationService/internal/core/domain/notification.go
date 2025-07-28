package domain

type NotificationPayload struct {
	Channel string `json:"channel"`
	Message string `json:"message"`
}

type EmailPayload struct {
	To      string `json:"to"`
	Subject string `json:"subject"`
	Body    string `json:"body"`
}
