@echo off

:: 执行命令
echo Executing: dotnet ef migrations remove
dotnet ef migrations remove --project "../"

:: 检查执行结果
if %errorlevel% neq 0 (
    echo Command execution failed!
) else (
    echo Command execution successful!
)

pause