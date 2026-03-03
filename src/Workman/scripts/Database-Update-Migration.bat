@echo off

:: 提示用户输入迁移名称
set /p mn=Please enter the migration name: 

:: 执行命令
echo Executing: dotnet ef database update %mn%
::  --output-dir "_migrations"
dotnet ef database update %mn% --project "../"

:: 检查执行结果
if %errorlevel% neq 0 (
    echo Command execution failed!
) else (
    echo Command execution successful!
)

pause