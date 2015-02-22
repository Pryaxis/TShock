import requests
import json 
import sys 
import os

branch = os.environ["GIT_BRANCH"]
tag_name = os.environ["bamboo_tag_name"]
name = os.environ["bamboo_release_name"]

#because we can't find any other secure way to get a token into this script run from bamboo :'(
with open('/home/bamboo/scripts/token.py') as f:
    token = f.read().rsplit('=', 1)[1].strip()

body = 'This is the newest release for TShock.  Please see the release thread for more information @ http://tshock.co/xf'

data = {'tag_name':tag_name, 'target_commitish':branch, 'name':name, 'body':body, 'draft':False, 'prerelease':False}
headers = {'Content-Type': 'application/json', 'Authorization': 'token ' + token}

print json.dumps(data)
req = requests.post('https://api.github.com/repos/NyxStudios/TShock/releases', data = json.dumps(data), headers = headers)

print req.text