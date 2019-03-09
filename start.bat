@echo off
echo Starting server!
:server
python serve.py > logging.txt 2>&1
echo Server was killed! Restarting it now!
goto server
