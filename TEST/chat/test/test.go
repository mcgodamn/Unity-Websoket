package main

import (
	"fmt"
)

func main() {
	x, y := 1, 2
	c := [...]*int{&x, &y} //保存的元素是指向int型的指针
	a := [...]int{99: 1}
	var p *[100]int = &a //指向数组的指针，取的是a的地址
	//加*代表这是一个指针
	fmt.Println(p) //打印的结果和a的结果是一样的，只不过前面多了一个取地址的符号 &，这就是指向数组的指针
	fmt.Println(c) //指针数组，存的是x和y的内存地址

}
