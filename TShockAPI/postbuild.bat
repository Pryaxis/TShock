set pluginspath=C:\Program Files (x86)\Steam\steamapps\common\terraria\serverplugins\
IF NOT EXIST "%pluginspath%" GOTO SkipCopy
attrib -r "%pluginspath%TShockAPI.dll"
attrib -r "%pluginspath%TShockAPI.pdb"
copy "TShockAPI.dll" "%pluginspath%"
copy "TShockAPI.pdb" "%pluginspath%"
echo Files copied
GOTO end
:SkipCopy
echo Skipped copying files
:end