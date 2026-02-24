/*
TShock, a server mod for Terraria
Copyright (C) 2011-2025 Pryaxis & TShock Contributors

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
*/

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TShockAPI.Configuration;

namespace TShockAPI;

/// <summary>
/// Provides the storage for a whitelist.
/// </summary>
public sealed class Whitelist
{
	private readonly FileInfo _file;
	private readonly Lock _fileLock = new();

	private readonly HashSet<IPAddress> _whitelistAddresses = [];
	private readonly HashSet<IPNetwork> _whitelistNetworks = [];

	/// <summary>
	/// Defines if the whitelist is enabled or not.
	/// </summary>
	/// <remarks>Shorthand to the current <see cref="TShockSettings.EnableWhitelist" /> setting.</remarks>
	private bool Enabled => TShock.Config.Settings.EnableWhitelist;

	internal const string DefaultWhitelistContent = /*lang=conf*/
		"""
		# Localhost
		127.0.0.1

		# Uncomment to allow IPs within private ranges
		# 10.0.0.0/8
		# 172.16.0.0/12
		# 192.168.0.0/16
		""";

	internal const char CommentPrefix = '#';

	/// <summary>
	/// Initializes a new instance of the <see cref="Whitelist"/> class.
	/// Creates the whitelist file if it does not exist on disk.
	/// </summary>
	public Whitelist(string path)
	{
		_file = new(path);

		if (!_file.Exists)
		{
			throw new FileNotFoundException("The whitelist file does not exist", _file.FullName);
		}

		ReadWhitelistFromFile();
	}

	/// <summary>
	/// Tells if a user is on the whitelist
	/// </summary>
	/// <param name="host">string ip of the user</param>
	/// <returns>true/false</returns>
	public bool IsWhitelisted(string host)
	{
		if (!Enabled)
		{
			return true;
		}

		if (!IPAddress.TryParse(host, out IPAddress? ip))
		{
			throw new ArgumentException($"The provided host '{host}' is not a valid IP address.", nameof(host));
		}

		// HACK: Terraria doesn't support IPv6 yet, so we can't check for it.
		// Remove once TShock supports IPv6.
		if (ip.AddressFamily is AddressFamily.InterNetworkV6)
		{
			TShock.Log.Warn(GetString($"IPv6 address '{ip}' is not supported by Terraria. Skipping check."));
			TShock.Log.Warn(GetString("If you somehow managed to get this message, please report it to the TShock team :"));
			TShock.Log.Warn(GetString("https://github.com/Pryaxis/TShock/issues"));

			return false;
		}

		// First check if the IP address is directly whitelisted
		return _whitelistAddresses.Contains(ip)
			// Otherwise is it contained within a whitelisted network?
			|| _whitelistNetworks.Any(n => n.Contains(ip));
	}

	/// <summary>
	/// Reloads the whitelist from the file.
	/// </summary>
	public void ReloadFromFile()
	{
		lock (_fileLock)
		{
			_whitelistAddresses.Clear();
			_whitelistNetworks.Clear();
			ReadWhitelistFromFile();
		}
	}

	private void ReadWhitelistFromFile()
	{
		using StreamReader sr = _file.OpenText();

		int i = 0;
		while (!sr.EndOfStream)
		{
			ReadWhitelistLine(sr.ReadLine(), i);
			i++;
		}
	}

	private void ReadWhitelistLine(scoped ReadOnlySpan<char> content, int line)
	{
		// Ignore blank line or comment
		if (content is [] or [CommentPrefix, ..] || content.IsWhiteSpace())
		{
			return;
		}

		// Try parse first IP range, which uses CIDR sep as discriminator.
		if (IPNetwork.TryParse(content, out IPNetwork range))
		{
			_whitelistNetworks.Add(range);
		}
		else if (IPAddress.TryParse(content, out IPAddress? ip))
		{
			_whitelistAddresses.Add(ip);
		}
		else
		{
			// If we reach here, the line is not a valid IP address or network.
			// We could throw this, but for now we just log and ignore it.
			TShock.Log.Warn(GetString($"Invalid whitelist entry at line {line}: \"{content.ToString()}\", skipped"));
		}
	}

	/// <summary>
	/// Adds an IP address or network to the whitelist.
	/// </summary>
	/// <param name="ip">The IP address or network to add.</param>
	/// <returns>true if the address or network was added successfully; otherwise, false.</returns>
	public bool AddToWhitelist(scoped ReadOnlySpan<char> ip)
	{
		if (IPNetwork.TryParse(ip, out IPNetwork range))
		{
			return AddToWhitelist(range);
		}

		if (IPAddress.TryParse(ip, out IPAddress? address))
		{
			return AddToWhitelist(address);
		}

		return false;
	}

	/// <summary>
	/// Removes an IP address or network from the whitelist.
	/// </summary>
	/// <param name="ip">The IP address or network to remove.</param>
	/// <returns>>true if the address or network was removed successfully; otherwise, false.</returns>
	public bool RemoveFromWhitelist(scoped ReadOnlySpan<char> ip)
	{
		if (IPNetwork.TryParse(ip, out IPNetwork range))
		{
			return RemoveFromWhitelist(range);
		}

		if (IPAddress.TryParse(ip, out IPAddress? address))
		{
			return RemoveFromWhitelist(address);
		}

		return false;
	}


	private bool AddToWhitelist(IPAddress ip)
		=> _whitelistAddresses.Add(ip)
			& AddLine(ip.ToString());

	private bool AddToWhitelist(IPNetwork network)
		=> _whitelistNetworks.Add(network)
			& AddLine(network.ToString());

	private bool RemoveFromWhitelist(IPAddress ip)
		=> _whitelistAddresses.Remove(ip)
			& RemoveLine(ip.ToString());

	private bool RemoveFromWhitelist(IPNetwork network)
		=> _whitelistNetworks.Remove(network)
			& RemoveLine(network.ToString());

	private bool AddLine(scoped ReadOnlySpan<char> content)
	{
		lock (_fileLock)
		{
			// Case: File does not end with a newline, add one
			bool needsNewLine;

			using (FileStream fs = _file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				fs.Seek(-1, SeekOrigin.End);
				needsNewLine = fs.Length > 0 && fs.ReadByte() is not '\n';
			}

			using StreamWriter sw = _file.AppendText();

			if (needsNewLine)
			{
				sw.WriteLine();
			}

			sw.WriteLine(content);
			return true;
		}
	}


	private bool RemoveLine(scoped ReadOnlySpan<char> content)
	{
		if (content is [])
		{
			throw new ArgumentException("Content cannot be empty.", nameof(content));
		}

		lock (_fileLock)
		{
			string tempFile = Path.GetTempFileName();

			using StreamReader sr = _file.OpenText();
			using StreamWriter sw = new(tempFile);

			bool removed = false;

			while (!sr.EndOfStream)
			{
				scoped ReadOnlySpan<char> line = sr.ReadLine();

				// If the line does not match the content, write it to the temp file
				if (line != content)
				{
					sw.WriteLine(line);
				}
				else
				{
					removed = true;
				}
			}

			// If we removed a line, we need to overwrite the original file
			if (removed)
			{
				sw.Flush();

				_file.Delete();
				File.Move(tempFile, _file.FullName);
			}
			else
			{
				File.Delete(tempFile);
			}

			return removed;
		}
	}
}
