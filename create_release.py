import os
import shutil
import subprocess
import urllib2
import zipfile

cur_wd = os.getcwd()
release_dir = os.path.join(cur_wd, "releases")

terraria_bin_name = "TerrariaServer.exe"
sql_bins_names = ["Mono.Data.Sqlite.dll", "MySql.Data.dll", "MySql.Web.dll"]
sqlite_dep = "sqlite3.dll"
json_bin_name = "Newtonsoft.Json.dll"
http_bin_name = "HttpServer.dll"
tshock_bin_name = "TShockAPI.dll"
tshock_symbols = "TShockAPI.dll.mdb"

terraria_bin = os.path.join(cur_wd, "TerrariaServerBins", terraria_bin_name)
sql_dep = os.path.join(cur_wd, "SqlBins")
http_bin = os.path.join(cur_wd, "HttpBins", http_bin_name)
json_bin = os.path.join(cur_wd, "TShockAPI", json_bin_name)
release_bin = os.path.join(cur_wd, "TShockAPI", "bin", "Release", tshock_bin_name)
debug_folder = os.path.join(cur_wd, "TShockAPI", "bin", "Debug")


def create_release_folder():
  os.mkdir(release_dir)

def copy_dependencies():
  shutil.copy(terraria_bin, release_dir)
  shutil.copy(http_bin, release_dir)
  shutil.copy(json_bin, release_dir)
  shutil.copy(os.path.join(sql_dep, sqlite_dep), release_dir)
  for f in sql_bins_names:
    shutil.copy(os.path.join(sql_dep, f), release_dir)
  
def copy_debug_files():
  shutil.copy(os.path.join(debug_folder, tshock_bin_name), release_dir)
  shutil.copy(os.path.join(debug_folder, tshock_symbols), release_dir)

def copy_release_files():
  shutil.copy(release_bin, release_dir)
  shutil.copy(release_bin, release_dir)

def create_base_zip(name):
  os.chdir(release_dir)
  zip = zipfile.ZipFile(name, "w")
  zip.write(terraria_bin_name)
  zip.write(sqlite_dep)
  zip.write(http_bin_name, os.path.join("ServerPlugins", http_bin_name))
  zip.write(json_bin_name, os.path.join("ServerPlugins", json_bin_name))
  for f in sql_bins_names:
    zip.write(f, os.path.join("ServerPlugins", f))
  return zip

def package_release():
  copy_release_files()
  zip = create_base_zip("tshock_release.zip")
  zip.write(tshock_bin_name, os.path.join("ServerPlugins", tshock_bin_name))
  zip.close()
  os.remove(tshock_bin_name)
  os.chdir(cur_wd)

def package_debug():
  copy_debug_files()
  zip = create_base_zip("tshock_debug.zip")
  zip.write(tshock_bin_name, os.path.join("ServerPlugins", tshock_bin_name))
  zip.write(tshock_symbols, os.path.join("ServerPlugins", tshock_symbols))
  zip.close()
  os.remove(tshock_bin_name)
  os.remove(tshock_symbols)
  os.chdir(cur_wd)

def delete_files():
  os.chdir(release_dir)
  os.remove(terraria_bin_name)
  for f in sql_bins_names:
    os.remove(f)
  os.remove(sqlite_dep)
  os.remove(json_bin_name)
  os.remove(http_bin_name)
  os.chdir(cur_wd)

def update_terraria_exe():
  url = urllib2.urlopen('http://direct.tshock.co:8085/browse/TERRA-TSAPI/latestSuccessful/artifact/JOB1/Server-Bin/TerrariaServer.exe')
  localFile = open('TerrariaServer.exe', 'w')
  localFile.write(url.read())
  localFile.close()
  shutil.copy(terraria_bin_name, terraria_bin)
  os.remove(terraria_bin_name)

def build_software():
  subprocess.call(['/usr/local/bin/xbuild', './TShockAPI/TShockAPI.csproj', '/p:Configuration=Release'])
  subprocess.call(['/usr/local/bin/xbuild', './TShockAPI/TShockAPI.csproj', '/p:Configuration=Debug'])
  
if __name__ == '__main__':
  create_release_folder()
  update_terraria_exe()
  copy_dependencies()
  build_software()
  package_release()
  package_debug()
  delete_files()
