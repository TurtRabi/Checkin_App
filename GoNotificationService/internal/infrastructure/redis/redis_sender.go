package redis

import (
	"context"
	"log"

	"github.com/go-redis/redis/v8"
	"gonotificationservice/internal/core/ports"
)

type redisSender struct {
	rdb *redis.Client
}

func NewRedisSender(rdb *redis.Client) ports.NotificationSender {
	return &redisSender{
		rdb: rdb,
	}
}

func (s *redisSender) Send(ctx context.Context, channel, message string) error {
	err := s.rdb.Publish(ctx, channel, message).Err()
	if err != nil {
		log.Printf("Không thể publish tin nhắn đến kênh Redis %s: %v", channel, err)
		return err
	}
	log.Printf("Thông báo đã gửi đến kênh '%s': '%s'", channel, message)
	return nil
}
