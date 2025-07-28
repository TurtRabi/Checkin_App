package http

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"time"

	"gonotificationservice/internal/core/domain"
	"gonotificationservice/internal/core/ports"
)

// NotificationHandler xử lý các yêu cầu HTTP liên quan đến thông báo
type NotificationHandler struct {
	notificationService ports.NotificationService
}

func NewNotificationHandler(service ports.NotificationService) *NotificationHandler {
	return &NotificationHandler{
		notificationService: service,
	}
}

// SendNotification xử lý các yêu cầu gửi thông báo qua HTTP POST
func (h *NotificationHandler) SendNotification(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Chỉ cho phép phương thức POST", http.StatusMethodNotAllowed)
		return
	}

	var payload domain.NotificationPayload
	err := json.NewDecoder(r.Body).Decode(&payload)
	if err != nil {
		http.Error(w, fmt.Sprintf("Payload yêu cầu không hợp lệ: %v", err), http.StatusBadRequest)
		return
	}

	if payload.Channel == "" || payload.Message == "" {
		http.Error(w, "Kênh (Channel) và Tin nhắn (Message) không được để trống", http.StatusBadRequest)
		return
	}

	// Tạo một context với timeout cho hoạt động gửi thông báo
	ctx, cancel := context.WithTimeout(r.Context(), 5*time.Second)
	defer cancel()

	err = h.notificationService.SendNotification(ctx, payload)
	if err != nil {
		log.Printf("Lỗi khi gửi thông báo: %v", err)
		http.Error(w, fmt.Sprintf("Không thể gửi thông báo: %v", err), http.StatusInternalServerError)
		return
	}

	w.WriteHeader(http.StatusOK)
	json.NewEncoder(w).Encode(map[string]string{"status": "success", "message": "Thông báo đã được gửi"})
}
