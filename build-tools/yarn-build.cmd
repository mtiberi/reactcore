setlocal
set PATH=%~dp0.;%PATH%
set NODE_TLS_REJECT_UNAUTHORIZED=0
node "%~dp0buildClient.js" "%1" "%2"
