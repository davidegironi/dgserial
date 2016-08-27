@echo off

rem Commit and push to master
rem Copyright (c) Davide Gironi, 2015

rem load config
if not exist config.git-commitandpushtomaster.bat exit
call config.git-commitandpushtomaster.bat

setlocal 

cd %GITMAINFOLDER%
git commit -am "updated by script"
git push

endlocal

exit
