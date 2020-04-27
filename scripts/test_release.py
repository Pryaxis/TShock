'''
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
'''

import subprocess
import shutil
import os.path
import zipfile

def generate_release():
    zip = zipfile.ZipFile("tshock_release.zip", "r")
    zip.extractall()

def generate_configs():
    subprocess.call(['/usr/local/bin/mono', 'TerrariaServer.exe', '-dump'])
    if not os.path.isfile('ConfigDescriptions.txt') or not os.path.isfile('PermissionsDescriptions.txt') or not os.path.isfile('ServerSideConfigDescriptions.txt') or not os.path.isfile('RestDescriptions.txt'):
        raise CalledProcessError(1)

if __name__ == '__main__':
  generate_release()
  generate_configs()