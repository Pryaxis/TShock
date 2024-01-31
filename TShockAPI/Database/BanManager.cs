/*
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
*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Entities;
using Entity = Terraria.Entity;

namespace TShockAPI.Database
{
	/// <summary>
	/// Class that manages bans.
	/// </summary>
	public static class BanManager
	{
		/// <summary>
		/// Returns the number of bans that already exist
		/// </summary>
		/// <returns>Total number of bans</returns>
		public static async Task<long> CountBans()
		{
			return await DB.CountAsync<Ban>();
		}

		/// <summary>
		/// Event invoked after a ban is added
		/// </summary>
		public static event EventHandler<BanEventArgs> OnBanAdd;

		/// <summary>
		/// Event invoked after a user is unbanned
		/// </summary>
		public static event EventHandler<BanEventArgs> OnBanRemove;


		internal static async Task<bool> IsPlayerBanned(TSPlayer player)
		{
			// Attempt to find a ban by account name if the player is logged in,
			// otherwise, find by IP address or UUID.
			var banFilter = player.IsLoggedIn
				? Builders<Ban>.Filter.Eq(b => b.AccountName, player.Account.Name)
				: Builders<Ban>.Filter.Or(
					Builders<Ban>.Filter.Eq(b => b.IpAddress, player.IP),
					Builders<Ban>.Filter.Eq(b => b.Uuid, player.UUID));

			// Execute the database query to find a matching ban.
			var ban = await DB.Find<Ban>().Match(banFilter).ExecuteFirstAsync();

			// If no ban is found, the player is not banned.
			if (ban == null) return false;

			// Disconnect the player with an appropriate message based on the ban expiration.
			var disconnectMessage = GetBanDisconnectMessage(ban);
			player.Disconnect(disconnectMessage);
			return true;
		}

		private static string GetBanDisconnectMessage(Ban ban)
		{
			var baseMessage = $"#{ban.BanId} - You are banned: {ban.Reason}";

			// If the ban is permanent.
			if (ban.ExpirationDateTime == DateTime.MaxValue)
			{
				return GetParticularString("{0} is ban number, {1} is ban reason", baseMessage);
			}

			// If the ban has an expiration date.
			var timeRemaining = ban.ExpirationDateTime - DateTime.UtcNow;
			var prettyExpiration =
				ban.GetPrettyExpirationString(); // Assuming this method exists and formats the expiration nicely.
			return GetParticularString("{0} is ban number, {1} is ban reason, {2} is a timestamp",
				$"{baseMessage} ({prettyExpiration} remaining)");
		}

		/// <summary>
		/// Retrieves a single ban from a ban's ID
		/// </summary>
		/// <param name="id">The ban identifier</param>
		/// <returns>The requested ban</returns>
		public static async Task<Ban?> GetBanById(int id)
		{
			return await DB.Find<Ban>()
				.Match(x => x.BanId == id)
				.ExecuteFirstAsync();
		}

		/// <summary>
		/// Retrieves a list of bans from the database, sorted by their addition date from newest to oldest
		/// </summary>
		public static async Task<IEnumerable<Ban>> RetrieveAllBans() => await RetrieveAllBansSorted(BanSortMethod.DateBanned, true);

		/// <summary>
		/// Retrieves an enumerable of Bans from the database, sorted using the provided sort method
		/// </summary>
		/// <param name="sortMethod">The method to sort the bans.</param>
		/// <param name="descending">Whether the sort should be in descending order.</param>
		/// <returns>A sorted enumerable of Ban objects.</returns>
		public static async Task<IEnumerable<Ban>> RetrieveAllBansSorted(BanSortMethod sortMethod, bool descending = true)
		{
			var sortDefinition = descending
				? Builders<Ban>.Sort.Descending(GetSortField(sortMethod))
				: Builders<Ban>.Sort.Ascending(GetSortField(sortMethod));

			var banList = await DB.Find<Ban>().Sort(x=>sortDefinition).ExecuteAsync();
			return banList;
		}

		private static string GetSortField(BanSortMethod sortMethod)
		{
			return sortMethod switch
			{
				BanSortMethod.TicketNumber => nameof(BanSortMethod.TicketNumber),
				BanSortMethod.DateBanned => nameof(BanSortMethod.DateBanned),
				BanSortMethod.EndDate => nameof(BanSortMethod.EndDate),
				_ => throw new ArgumentOutOfRangeException(nameof(sortMethod), $"Not expected sort method value: {sortMethod}"),
			};
		}

		public static Ban CreateBan(BanType type, string value, string reason, UserAccount banningUser, DateTime start,
			DateTime? endDate = null)
		{
			Ban ban = new()
			{
				Reason = reason,
				BanningUser = banningUser.Name,
				BanDateTime = start,
				ExpirationDateTime = endDate ?? DateTime.MaxValue
			};

			switch (type)
			{
				case BanType.Uuid:
				{
					ban.Uuid = value;
					break;
				}
				case BanType.AccountName:
				{
					ban.AccountName = value;
					break;
				}
				case BanType.IpAddress:
				{
					ban.IpAddress = value;
					break;
				}
				default: throw new Exception("Invalid ban type!");
			}

			TShock.Log.Info("A new ban has been created for: ");
			return ban;
		}

		public static bool UnbanPlayer(string accountName)
		{
			
		}


		/// <summary>
		/// Removes all bans from the database
		/// </summary>
		public static async Task ClearBans() => await DB.DeleteAsync<Ban>(Builders<Ban>.Filter.Empty);

	}

	/// <summary>
	/// Enum containing sort options for ban retrieval
	/// </summary>
	public enum BanSortMethod
	{
		DateBanned,
		EndDate,
		TicketNumber
	}

	/// <summary>
	/// Result of an attempt to add a ban
	/// </summary>
	public class AddBanResult
	{
		/// <summary>
		/// Message generated from the attempt
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Ban object generated from the attempt, or null if the attempt failed
		/// </summary>
		public Ban Ban { get; set; }
	}

	/// <summary>
	/// Event args used for completed bans
	/// </summary>
	public class BanEventArgs : EventArgs
	{
		/// <summary>
		/// Complete ban object
		/// </summary>
		public Ban Ban { get; set; }

		/// <summary>
		/// Player ban is being applied to
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// Whether or not the operation should be considered to be valid
		/// </summary>
		public bool Valid { get; set; } = true;
	}

	/// <summary>
	/// Event args used for ban data prior to a ban being formalized
	/// </summary>
	public class BanPreAddEventArgs : EventArgs
	{
		/// <summary>
		/// An identifiable piece of information to ban
		/// </summary>
		public string Identifier { get; set; }

		/// <summary>
		/// Gets or sets the ban reason.
		/// </summary>
		/// <value>The ban reason.</value>
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the name of the user who added this ban entry.
		/// </summary>
		/// <value>The banning user.</value>
		public string BanningUser { get; set; }

		/// <summary>
		/// DateTime from which the ban will take effect
		/// </summary>
		public DateTime BanDateTime { get; set; }

		/// <summary>
		/// DateTime at which the ban will end
		/// </summary>
		public DateTime ExpirationDateTime { get; set; }

		/// <summary>
		/// Whether or not the operation should be considered to be valid
		/// </summary>
		public bool Valid { get; set; } = true;

		/// <summary>
		/// Optional message to explain why the event was invalidated, if it was
		/// </summary>
		public string Message { get; set; }
	}

	/// <summary>
	/// Model class that represents a ban entry in the database.
	/// </summary>
	public class Ban : MongoDB.Entities.Entity
	{
		/// <summary>
		/// A unique ID assigned to this ban
		/// </summary>
		public int BanId { get; set; }

		/// <summary>
		/// A possible IP address we are banning
		/// </summary>
		public string? IpAddress { get; set; }

		/// <summary>
		/// A possible UUID we are banning
		/// </summary>
		public string? Uuid { get; set; }

		/// <summary>
		/// A possible account name we are banning
		/// </summary>
		public string? AccountName { get; set; }

		/// <summary>
		/// Gets or sets the ban reason.
		/// </summary>
		/// <value>The ban reason.</value>
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the name of the user who added this ban entry.
		/// </summary>
		/// <value>The banning user.</value>
		public string BanningUser { get; set; }

		/// <summary>
		/// DateTime from which the ban will take effect
		/// </summary>
		public DateTime BanDateTime { get; set; }

		/// <summary>
		/// DateTime at which the ban will end
		/// </summary>
		public DateTime ExpirationDateTime { get; set; }

		/// <summary>
		/// Returns whether or not the ban is still in effect
		/// </summary>
		[Ignore] public bool Valid => ExpirationDateTime > BanDateTime;

		/// <summary>
		/// Returns a string in the format dd:mm:hh:ss indicating the time until the ban expires.
		/// If the ban is not set to expire (ExpirationDateTime == DateTime.MaxValue), returns the string 'Never'
		/// </summary>
		/// <returns></returns>
		public string GetPrettyExpirationString()
		{
			if (ExpirationDateTime == DateTime.MaxValue)
			{
				return "Never";
			}

			TimeSpan
				ts = (ExpirationDateTime - DateTime.UtcNow)
					.Duration(); // Use duration to avoid pesky negatives for expired bans
			return $"{ts.Days:00}:{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
		}

		/// <summary>
		/// Returns a string in the format dd:mm:hh:ss indicating the time elapsed since the ban was added.
		/// </summary>
		/// <returns></returns>
		public string GetPrettyTimeSinceBanString()
		{
			TimeSpan ts = (DateTime.UtcNow - BanDateTime).Duration();
			return $"{ts.Days:00}:{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
		}

	}

	public enum BanType
	{
		Uuid,
		AccountName,
		IpAddress
	}
}
