@echo off

SET "dir_projeto_multicte=..\MultiCTe"
SET "arquivo_admin_dll=.\AdminMultisoftware\bin\Release\AdminMultisoftware.dll"

FOR /f "delims=" %%a IN (
 'dir /b /s /a-d "%dir_projeto_multicte%\AdminMultisoftware.dll" '
 ) DO (
 COPY %arquivo_admin_dll% %%a
)

GOTO :EOF
