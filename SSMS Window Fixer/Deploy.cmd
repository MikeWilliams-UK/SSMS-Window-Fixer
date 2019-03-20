@echo off

cd /d "%~dp0"

taskkill /im "SSMS Window Fixer.exe" /f

copy "SSMS Window Fixer.exe" "C:\Tools\SSMS Window Fixer"
copy "SSMS Window Fixer.exe.config" "C:\Tools\SSMS Window Fixer"
copy "SSMS Window Fixer.pdb" "C:\Tools\SSMS Window Fixer"

start "SSMS Window Fixer.exe" "C:\Tools\SSMS Window Fixer\SSMS Window Fixer.exe"

pause