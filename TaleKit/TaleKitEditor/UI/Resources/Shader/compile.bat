@echo off

for /r %%i in (*.fx) do (
	echo Compiling %%~ni.fx ...
	fxc /nologo /T ps_2_0 /E main /Fo %%~pi%%~ni.ps %%i
)
