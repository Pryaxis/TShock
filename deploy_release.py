import requests
import json 
import sys 
import os
import subprocess
import base64
import urllib

create_release_url = 'https://api.github.com/repos/NyxStudios/TShock/releases'
config_doc_get_url = 'https://tshock.atlassian.net/wiki/rest/api/content/%s?expand=body.storage,version,ancestors'
config_doc_put_url = 'https://tshock.atlassian.net/wiki/rest/api/content/%s'

config_desc_page = "3047451"
ssc_desc_page = "39845891"
permissions_desc_page = "3047433"
rest_desc_page = "40632322"

def get_confluence_page(id):
    confluence_header = {"Content-Type":"application/json"}
    r = requests.get(config_doc_get_url % id, auth=(os.environ["bamboo_confluence_username"], os.environ["bamboo_confluence_password"]), headers=confluence_header)
    page = json.loads(r.text)
    return page

def put_confluence_page(page):
    confluence_header = {"Content-Type":"application/json"}
    page['version']['number'] = page['version']['number'] + 1    
    r = requests.put(config_doc_put_url % page['id'], auth=(os.environ["bamboo_confluence_username"], os.environ["bamboo_confluence_password"]), headers=confluence_header, data=json.dumps(page))
    page = json.loads(r.text)
    return page

def update_confluence_page(id, content):
    page = get_confluence_page(id)
    page['body']['storage']['value'] = content
    page['body']['storage']['representation'] = 'storage'
    put_confluence_page(page)
    
def read_and_update_config_on_confluence(id, file):
    #Read the Config
    config = ""
    with open(file, "r") as f:
        line = f.readline()
        while (line is not ""):
            if (config is not ""):
                config = config + '<br />'
            config = config + line
            line = f.readline()
    #update confluence page
    update_confluence_page(id, config)
    
#Load variables from ENV, which are put there by the bamboo build.
branch = os.environ["GIT_BRANCH"]
tag_name = os.environ["bamboo_tag_name"]
name = os.environ["bamboo_release_name"]
body = os.environ["bamboo_release_body"]

#build release file name using the tag, stripping the 'v' off the front ie 'v.1.2.3' => '.1.2.3' resulting in a file called 'tshock.1.2.3.zip'
release_name = 'tshock_' + tag_name[1:] + '.zip'

#because we can't find any other secure way to get a token into this script run from bamboo :'(
with open('/home/bamboo/scripts/token.py') as f:
  # token = f.read().rsplit('=', 1)[1].strip()

#invoke the mv command on the artifact from bamboo to the new name above
subprocess.call('mv tshock_release.zip ' + release_name, shell=True)

#construct the payload for the post request to github to create the release.
data = {'tag_name':tag_name, 'target_commitish':branch, 'name':name, 'body':body, 'draft':False, 'prerelease':False}
#headers for the post request with our oauth token, allowing us write access
create_headers = {'Content-Type': 'application/json', 'Authorization': 'token ' + token}
#payload is a json string, not a strong typed json object
json_data = json.dumps(data)

#make the post request, creating a release
r = requests.post(create_release_url, data=json_data, headers=create_headers)
#parse the response into an object
json_response = json.loads(r.text)

#extract the relevant information from the object needed to attach a binary to the release created previously
release_id = json_response['id']
upload_url = json_response['upload_url'].rsplit('{')[0]

#construct the post url using the release name, as that is required by the api
upload_url = upload_url + '?name=' + release_name

#headers for the post request, need to specify that our file is a zip, and how large it is
upload_headers = {'Authorization': 'token ' + token, 'Content-Type':'application/zip', 'Content-Length':str(os.path.getsize(release_name))}

#upload the binary, resulting in a complete binary
r = requests.post(upload_url, data=open(release_name, 'rb'), headers = upload_headers, verify=False)


read_and_update_config_on_confluence(config_desc_page, "ConfigDescriptions.txt")
read_and_update_config_on_confluence(ssc_desc_page, "ServerSideConfigDescriptions.txt")
read_and_update_config_on_confluence(permissions_desc_page, "PermissionsDescriptions.txt")
read_and_update_config_on_confluence(rest_desc_page, "RestDescriptions.txt")