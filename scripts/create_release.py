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
mysql_bin_name = "MySql.Data.dll"
sqlite_dep_name = "sqlite3.dll"
sqlite_bin_name = "Mono.Data.Sqlite.dll"
json_bin_name = "Newtonsoft.Json.dll"
http_bin_name = "HttpServer.dll"
tshock_bin_name = "TShockAPI.dll"
tshock_symbols = "TShockAPI.dll.mdb"
bcrypt_bin_name = "BCrypt.Net.dll"

terraria_release_bin = os.path.join(cur_wd, "TerrariaServerAPI", "bin", "Release", terraria_bin_name)
terraria_debug_bin = os.path.join(cur_wd, "TerrariaServerAPI", "bin", "Debug", terraria_bin_name)
mysql_bin = os.path.join(cur_wd, "packages", "MySql.Data.6.9.8", "lib", "net45", mysql_bin_name)
sqlite_dep = os.path.join(cur_wd, "prebuilts", sqlite_dep_name)
sqlite_bin = os.path.join(cur_wd, "prebuilts", sqlite_bin_name)
http_bin = os.path.join(cur_wd, "prebuilts", http_bin_name)
json_bin = os.path.join(cur_wd, "packages", "Newtonsoft.Json.8.0.3", "lib", "net45", json_bin_name)
bcrypt_bin = os.path.join(cur_wd, "packages", "BCrypt.Net.0.1.0", "lib", "net35", bcrypt_bin_name)
release_bin = os.path.join(cur_wd, "TShockAPI", "bin", "Release", tshock_bin_name)
debug_folder = os.path.join(cur_wd, "TShockAPI", "bin", "Debug")


def create_release_folder():
  os.mkdir(release_dir)

def copy_dependencies():
  shutil.copy(http_bin, release_dir)
  shutil.copy(json_bin, release_dir)
  shutil.copy(bcrypt_bin, release_dir)
  shutil.copy(sqlite_dep, release_dir)
  shutil.copy(mysql_bin, release_dir)
  shutil.copy(sqlite_bin, release_dir)
  
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
  zip.write(sqlite_dep_name)
  zip.write(http_bin_name, os.path.join("ServerPlugins", http_bin_name))
  zip.write(json_bin_name, json_bin_name)
  zip.write(bcrypt_bin_name, os.path.join("ServerPlugins", bcrypt_bin_name))
  zip.write(mysql_bin_name, os.path.join("ServerPlugins", mysql_bin_name))
  zip.write(sqlite_bin_name, os.path.join("ServerPlugins", sqlite_bin_name))
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
  os.remove(mysql_bin_name)
  os.remove(sqlite_bin_name)
  os.remove(sqlite_dep)
  os.remove(json_bin_name)
  os.remove(bcrypt_bin_name)
  os.remove(http_bin_name)
  os.chdir(cur_wd)

def update_terraria_source():
  subprocess.check_call(['/usr/bin/git', 'submodule', 'init'])
  subprocess.check_call(['/usr/bin/git', 'submodule', 'update'])
  subprocess.check_call(['/usr/bin/mono', '/opt/nuget/nuget.exe', 'restore'])

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
