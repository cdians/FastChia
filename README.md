## FastChia是什么?
一款使用官方Chia内核的P盘工具，解决使用钱包或矿池P盘工具设置繁琐、不能任务自动接续的痛点，实现一键开P，智能分配磁盘，充分压榨CPU、硬盘性能，提高P盘效率。



[![WoQkuQ.png](https://z3.ax1x.com/2021/07/28/WoQkuQ.png)](https://imgtu.com/i/WoQkuQ)
## FastChia有哪些功能？

* 支持官方NFT新协议
* 多内核选择：madMAx43v3r多线程极速内核，官方内核
* 一键`删除P盘任务`功能
    *  自动停止对应P盘进程
    *  自动清理缓存文件
* 充分压榨CPU性能，加速P盘时间
* `自动接续`支持，方便每个P盘进程完成后自动开始下一次任务
* 强大的`容量计算`功能，合理安排压榨每个缓存盘
* 已上线`群控管理`,方便矿场集中下发、开始P盘任务
* 可视化`任务进度`,集中管理，再也不用为了怎么安排显示命令行窗口烦恼了
* 多临时盘选择，有优先级的负载均衡，绘图过程中可以自由修改
* 再也没有官方钱包P盘闪退，导致不知道任务进行到哪步的困恼了

## 使用说明
* 先填写`矿工公钥`，`矿池公钥`，`合约地址`，填入合约地址则不适用矿池公钥，也可以使用读取公钥写入(需要安装官方钱包) ，没有安装过官方钱包,首次使用需要点击Chia配置-初始化！！！不然点击绘图会没反应！！！
* 选择合适的`农田大小`，`写入速度`(最终盘写入文件平均速度，每p开始运行的时间间隔用这个算的，如果你想任务启动间隔短一些可以加大数值) ，其他不太懂的就保持默认，最好也保持默认！
* 选择`临时目录`，`最终目录`，这里有优先级，谁在前面优先使用谁 
* 自动接续 手动为p盘结束后需要手动再次开启 自动为每个p盘线程执行完成后再次继续执行直到对应最终盘p满
* 点击开始绘图即可

### 其他：
* 点击`删除`按钮后，有一定延迟，需要计算对应缓存文件，会自动关闭进程和删除对应缓存文件。
* 自动接续中需要停止，切换为手动即可。
* `循环次数`是单个任务 内循环次数，保持默认就行，除非你有特殊需求，自动接续是软件的外循环，一般情况下开自动接续就可以！！

### 注意：
* 使用FastChia请`抛掉`之前使用官方或者其他软件的使用习惯，现在只要你正确填写`公钥`，设置`临时盘`，`最终盘`以及正确的最终盘`写入速度`，其他保持默认设置并开启自动接续，FastChia会自动计算并合理安排任务间隔及进度，其他不需要你干涉，静待它把所有最终盘P满！
不需要你询问什么配置，能P多少个的问题

## 特别注意
最近出于学习技术的好奇心，使用反编译等手段，观察软件行为，发现有的软件会获取机器配置信息、系统版本、有的会不定时上传敏感信息到云端，更甚者扫描用户目录，望各位谨慎使用保护好自己的隐私！！！
最后郑重声明！！FastChia仅仅是一款单纯的P图软件，现在将来都是免费使用！！！

## 致改名换壳收费党
FastChia面向open source编程，希望你不要面向钱包编程，好自为之！

## 有问题反馈
在使用中有任何问题，欢迎反馈给我，可以用以下联系方式跟我交流

* 邮件(admin@elevenstyle.com)
* qq群: FastChia交流群(129903872)

## 捐助开发者
在兴趣的驱动下,写一个`免费`的东西，有欣喜，也还有汗水，希望你喜欢我的作品
## 感激
感谢以下作者,排名不分先后

* [Mars](mars@elevenstyle.com/)
* [Cdian](admin@elevenstyle.com/)

## 关于作者

```javascript
var coder = {
  nickName  : "cdian",
  site : "http://elevenstyle.com"
}
```
