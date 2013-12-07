@echo off

REM VS自动编译脚本 By ArBing Xie

echo 2秒钟后默认开始后台编译：
ping 127.0.0.1 -n 2 1>nul 2>nul

set VSPath=D:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE

path=%path%;%VSPath%

echo 编译开始

date /t >> build.log
time /t >> build.log

for %%f in (*.sln) do (
   echo 编译%%f中...
   devenv %%f /rebuild "Debug">> build.log
)

echo 编译成功

pause
exit 0