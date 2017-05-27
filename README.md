# WebPrint
a simple website to share your printers to others via web

Windows 网页共享打印机


## 使用方法

### I. 开启IIS 和 ASP
如果已经开启可跳过。
Windwos 系统包含IIS功能,但默认关闭，需要手动打开。

可以参考<http://jingyan.baidu.com/article/48206aeaaacd51216ad6b318.html>

具体步骤  
>
  1. 搜索 `Windows 功能`,或者打开控制面板
  2. 点击 `启用或者关闭Windows功能` 
  3. 勾选`Internet Information Services`(Internet信息服务) 和 子目录下 `ASP.NET 4.x`
  4. 点击确定,等待开启
>

### II. 下载代码

 1. 下载[代码](https://github.com/NewFuture/WebPrint/archive/master.zip) 
 2. 解压**替换**掉`C:\inetpub\wwwroot`下内容,(确保README.md同级文件在此根目录下)
 3. 浏览器打开 localhost 可以看到上传界面

### III. 配置

1. 修改临时文件权限`file`，添加`IIS_IUSRS`的写入权限
2. 设置打印密码,修改[Web.config](https://github.com/NewFuture/WebPrint/blob/master/Web.config#L5)设置打印密码
3. 设置系统默认打印机即配置完成


## 兼容支持

文件格式:

* [x] PDF
* [x] doc
* [x] docx
* [x] rtf
* [x] png
* [x] jpg
* [x] tiff
* [x] txt

上传设置：
* [x] 份数设置
* [x] 页码设置(pdf)
* [x] 多文件上传

## LICENSE

Apache2.0 LICENSE 并保留或添加源码链接`https://github.com/NewFuture/WebPrint`