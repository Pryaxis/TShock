import subprocess
import shutil
import os.path
import zipfile

def generate_release():
    zip = zipfile.ZipFile("tshock_release", "r")
    zip.extractall()

def generate_configs():
    subprocess.call(['/usr/bin/mono', 'TerrariaServer.exe', '-dump'])
    if (!os.path.isfile('ConfigDescriptions.txt') || !os.path.isfile('PermissionsDescriptions.txt') || !os.path.isfile('ServerSideConfigDescriptions.txt')):
        raise CalledProcessError(1)

if __name__ == '__main__':
  generate_release()
  generate_configs()