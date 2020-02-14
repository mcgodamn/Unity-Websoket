package main

import (
	"log"
)

func main() {
	i := 1
	defer func() {
		i += 1
		log.Println(i)
	}()
	if i == 1 {
		return
	}
}
