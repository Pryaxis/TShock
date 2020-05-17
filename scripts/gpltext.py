''' TShock, a server mod for Terraria
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

import re
import os
import glob

extensions = {'.cs', '.py'}
path = "./"
pattern = "/\*\s?\n?TShock, a server mod for Terraria(\n|.)*\*/"
pypattern = "'''\s?\n?TShock, a server mod for Terraria(\n|.)*'''"
year = "2019"
filename = "./README.md"
text = "/*\n\
TShock, a server mod for Terraria\n\
Copyright (C) 2011-2019 Pryaxis & TShock Contributors\n\
\n\
This program is free software: you can redistribute it and/or modify\n\
it under the terms of the GNU General Public License as published by\n\
the Free Software Foundation, either version 3 of the License, or\n\
(at your option) any later version.\n\
\n\
This program is distributed in the hope that it will be useful,\n\
but WITHOUT ANY WARRANTY; without even the implied warranty of\n\
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n\
GNU General Public License for more details.\n\
\n\
You should have received a copy of the GNU General Public License\n\
along with this program.  If not, see <http://www.gnu.org/licenses/>.\n\
*/\n\
\n\
"
pytext = re.sub(r"\*/", "'''", text)
pytext = re.sub(r"/\*", "'''", pytext)

def changeText(filename):
	content = ''

	with open(filename, 'r') as f:
		content = f.read()

	if filename.endswith('.py'):
		if re.search(pypattern, content):
			content = re.sub(r"Copyright \(C\) 2011-[\d]{4}", "Copyright (C) 2011-%s" % year, content)
		else:
			content = pytext + content
	else:
		if re.search(pattern, content):
			content = re.sub(r"Copyright \(C\) 2011-[\d]{4}", "Copyright (C) 2011-%s" % year, content)
		else:
			content = text + content

	with open(filename, 'w') as f:
		f.write(content)

def getFiles(path):
	list = os.listdir(path)

	for f in list:
		#print (f)
		if os.path.isdir(f):
			getFiles(path + f + '/')
		else:
			for ext in extensions:
				if f.endswith(ext):
					if f.endswith('.Designer.cs'):
						break
					print (path + f)
					changeText(path + f)
					break

getFiles(path)