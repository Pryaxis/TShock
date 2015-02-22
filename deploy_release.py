import urllib2
import json 
import sys 
import os

req_url = 'https://api.github.com/repos/NyxStudios/TShock/releases'
branch = os.environ["GIT_BRANCH"]
tag_name = os.environ["bamboo_tag_name"]
name = os.environ["bamboo_release_name"]

#because we can't find any other secure way to get a token into this script run from bamboo :'(
with open('/home/bamboo/scripts/token.py') as f:
    token = f.read().rsplit('=', 1)[1].strip()

body = 'This is the newest release for TShock.  Please see the release thread for more information @ http://tshock.co/xf'

data = {'tag_name':tag_name, 'target_commitish':branch, 'name':name, 'body':body, 'draft':False, 'prerelease':False}

headers = {'Content-Type': 'application/json', 'Authorization': 'token ' + token}

json_data = json.dumps(data)
req = urllib2.Request(req_url, json_data, headers)
urllib2.urlopen(req)