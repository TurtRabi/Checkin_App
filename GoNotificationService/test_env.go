package main

import (
	"fmt"
	"os"
)

func main() {
	redisAddr := os.Getenv("REDIS_ADDR")
	redisPassword := os.Getenv("REDIS_PASSWORD")

	fmt.Printf("REDIS_ADDR from Go: '%s'\n", redisAddr)
	fmt.Printf("REDIS_PASSWORD from Go: '%s'\n", redisPassword)

	if redisAddr == "" {
		fmt.Println("REDIS_ADDR is empty in Go program.")
	}
}
