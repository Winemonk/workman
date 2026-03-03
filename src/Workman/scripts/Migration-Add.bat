@echo off
:: 提示用户输入迁移名称
set /p mn=Please enter the migration name: 

:: 检查用户是否输入内容
if "%mn%"=="" (
    echo The migration name cannot be empty!
    pause
    exit /b
)

:: 执行命令
echo Executing: dotnet ef migrations add %mn%
::  --output-dir "_migrations"
dotnet ef migrations add %mn% --project "../" --output-dir "Infrastructure/Migrations"

:: 检查执行结果
if %errorlevel% neq 0 (
    echo Command execution failed!
) else (
    echo Command execution successful!
)

pause