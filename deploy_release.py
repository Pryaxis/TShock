import requests
import json 
import sys 
import os

branch = sys.argv[1]
tag_name = sys.argv[2]
name = sys.argv[3]
token = os.environ['GITHUB_TSHOCK_OAUTH']
body = 'This is the newest release for TShock.  Please see the release thread for more information @ http://tshock.co/xf'

data = {'tag_name':tag_name, 'target_commitish':branch, 'name':name, 'body':body, 'draft':False, 'prerelease':False}
headers = {'Content-Type': 'application/json', 'Authorization': 'token ' + token}

print json.dumps(data)
req = requests.post('https://api.github.com/repos/NyxStudios/TShock/releases', data = json.dumps(data), headers = headers)

print req.text