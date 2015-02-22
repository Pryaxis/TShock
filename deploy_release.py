import requests
import json 
import sys 
import os

create_release_url = 'https://api.github.com/repos/NyxStudios/TShock/releases'
release_name = 'tshock_release.zip'

branch = os.environ["GIT_BRANCH"]
tag_name = os.environ["bamboo_tag_name"]
name = os.environ["bamboo_release_name"]

#because we can't find any other secure way to get a token into this script run from bamboo :'(
with open('/home/bamboo/scripts/token.py') as f:
    token = f.read().rsplit('=', 1)[1].strip()

body = 'This is the newest release for TShock.  Please see the release thread for more information @ http://tshock.co/xf'

data = {'tag_name':tag_name, 'target_commitish':branch, 'name':name, 'body':body, 'draft':True, 'prerelease':False}
create_headers = {'Content-Type': 'application/json', 'Authorization': 'token ' + token}
json_data = json.dumps(data)

r = requests.post(create_release_url, data=json_data, headers=create_headers)
json_response = json.loads(r.text)

release_id = json_response['id']
upload_url = json_response['upload_url'].rsplit('{')[0]
upload_url = upload_url + '?name=' + release_name

upload_headers = {'Authorization': 'token ' + token, 'Content-Type':'application/zip', 'Content-Length':str(os.path.getsize(release_name))}
r = requests.post(upload_url, data=open(release_name, 'rb'), headers = upload_headers, verify=False)

