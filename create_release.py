# Hey there, this is used to compile TShock on the build server.
# Don't change it. Thanks!

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

terraria_release_bin = os.path.join(cur_wd, "TerrariaServerAPI", "bin", "Release", terraria_bin_name)
terraria_debug_bin = os.path.join(cur_wd, "TerrariaServerAPI", "bin", "Debug", terraria_bin_name)
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
  shutil.copy(terraria_debug_bin, release_dir)
  shutil.copy(os.path.join(debug_folder, tshock_bin_name), release_dir)
  shutil.copy(os.path.join(debug_folder, tshock_symbols), release_dir)

def copy_release_files():
  shutil.copy(terraria_release_bin, release_dir)
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
  os.remove(terraria_bin_name)
  os.chdir(cur_wd)

def package_debug():
  copy_debug_files()
  zip = create_base_zip("tshock_debug.zip")
  zip.write(tshock_bin_name, os.path.join("ServerPlugins", tshock_bin_name))
  zip.write(tshock_symbols, os.path.join("ServerPlugins", tshock_symbols))
  zip.close()
  os.remove(tshock_bin_name)
  os.remove(tshock_symbols)
  os.remove(terraria_bin_name)
  os.chdir(cur_wd)

def delete_files():
  os.chdir(release_dir)
  for f in sql_bins_names:
    os.remove(f)
  os.remove(sqlite_dep)
  os.remove(json_bin_name)
  os.remove(http_bin_name)
  os.chdir(cur_wd)

def update_terraria_source():
  subprocess.check_call('git submodule init')
  subprocess.check_call('git submodule update')

def build_software():
  release_proc = subprocess.Popen(['/usr/local/bin/xbuild', './TShockAPI/TShockAPI.csproj', '/p:Configuration=Release'])
  debug_proc = subprocess.Popen(['/usr/local/bin/xbuild', './TShockAPI/TShockAPI.csproj', '/p:Configuration=Debug'])
  release_proc.wait()
  debug_proc.wait()
  if (release_proc.returncode != 0):
    raise CalledProcessError(release_proc.returncode)
  if (debug_proc.returncode != 0):
    raise CalledProcessError(debug_proc.returncode)
    
if __name__ == '__main__':
  create_release_folder()
  update_terraria_source()
  copy_dependencies()
  build_software()
  package_release()
  package_debug()
  delete_files()
