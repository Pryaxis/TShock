@echo off
:main
echo Generating Pandoc docs
cd src
for %%F in (*.md) do pandoc %%F > %%F.html
pause