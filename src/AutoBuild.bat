@echo off

REM VS�Զ�����ű� By ArBing Xie

echo 2���Ӻ�Ĭ�Ͽ�ʼ��̨���룺
ping 127.0.0.1 -n 2 1>nul 2>nul

set VSPath=D:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE

path=%path%;%VSPath%

echo ���뿪ʼ

date /t >> build.log
time /t >> build.log

for %%f in (*.sln) do (
   echo ����%%f��...
   devenv %%f /rebuild "Debug">> build.log
)

echo ����ɹ�

pause
exit 0