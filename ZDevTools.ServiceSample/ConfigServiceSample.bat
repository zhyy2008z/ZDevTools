echo 将样例服务dll复制到服务控制台制定目录下，服务控制台可以识别本样例服务并加载样例服务
if not exist ..\ZDevTools.ServiceConsole\bin\Debug\services\ md ..\ZDevTools.ServiceConsole\bin\Debug\services\
copy bin\Debug\ServiceSample.dll ..\ZDevTools.ServiceConsole\bin\Debug\services\
pause