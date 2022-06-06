// This file is part of the DisCatSharp project, based off DSharpPlus.
//
// Copyright (c) 2021-2022 AITSYS
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using DisCatSharp.Enums;
using DisCatSharp.Exceptions;
using DisCatSharp.Net;
using DisCatSharp.Net.Abstractions;

using Newtonsoft.Json;

namespace DisCatSharp.Entities;

/// <summary>
/// Represents a Discord user.
/// </summary>
public class AdvancedDiscordUser : DiscordUser, IEquatable<AdvancedDiscordUser>
{
	internal DiscordApiClient ApiClient { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="AdvancedDiscordUser"/> class.
	/// </summary>
	/// <param name="user">The user object.</param>
	internal AdvancedDiscordUser(DiscordUser user)
	{
		this.Discord = user.Discord;

		this.ApiClient = user.Discord.ApiClient;

		this.Id = user.Id;
	}

	/// <summary>
	/// Builds the <see cref="AdvancedDiscordUser"/> object.
	/// </summary>
	/// <returns>The build object.</returns>
	internal async Task<AdvancedDiscordUser> PopulateAsync()
	{




		return this;
	}

	/// <summary>
	/// A list of bans.
	/// </summary>
	public List<SmallBan> Bans { get; internal set; } = new();

	/// <summary>
	/// A list of audit logs caused by the user.
	/// </summary>
	public Dictionary<ulong, DiscordAuditLogEntry> AuditLogEntriesAsExecutor { get; internal set; } = new();

	/// <summary>
	/// A list of audit logs where the user was the target.
	/// </summary>
	public Dictionary<ulong, DiscordAuditLogEntry> AuditLogEntriesAsTarget { get; internal set; } = new();

	/// <summary>
	/// A list of guild avatars.
	/// </summary>
	public Dictionary<ulong, string> GuildAvatars { get; internal set; } = new();

	/// <summary>
	/// A list of guild nicknames.
	/// </summary>
	public Dictionary<ulong, string> GuildNicknames { get; internal set; } = new();

	/// <summary>
	/// A list of guilds the user is in.
	/// </summary>
	public Dictionary<ulong, string> Guilds { get; internal set; } = new();

	public Dictionary<ulong, GuildNicknameHistory> NicknameHistory { get; set; }
	public List<UsernameHistory> UsernameHistory { get; set; }
	public List<DiscrimHistory> DiscrimHistory { get; set; }

	/// <summary>
	/// Checks whether this <see cref="AdvancedDiscordUser"/> is equal to another object.
	/// </summary>
	/// <param name="obj">Object to compare to.</param>
	/// <returns>Whether the object is equal to this <see cref="AdvancedDiscordUser"/>.</returns>
	public override bool Equals(object obj) => this.Equals(obj as AdvancedDiscordUser);

	/// <summary>
	/// Checks whether this <see cref="AdvancedDiscordUser"/> is equal to another <see cref="AdvancedDiscordUser"/>.
	/// </summary>
	/// <param name="e"><see cref="AdvancedDiscordUser"/> to compare to.</param>
	/// <returns>Whether the <see cref="AdvancedDiscordUser"/> is equal to this <see cref="AdvancedDiscordUser"/>.</returns>
	public bool Equals(AdvancedDiscordUser e) => e is not null && (ReferenceEquals(this, e) || this.Id == e.Id);

	/// <summary>
	/// Gets the hash code for this <see cref="AdvancedDiscordUser"/>.
	/// </summary>
	/// <returns>The hash code for this <see cref="AdvancedDiscordUser"/>.</returns>
	public override int GetHashCode() => this.Id.GetHashCode();

	/// <summary>
	/// Gets whether the two <see cref="DiscordUser"/> objects are equal.
	/// </summary>
	/// <param name="e1">First user to compare.</param>
	/// <param name="e2">Second user to compare.</param>
	/// <returns>Whether the two users are equal.</returns>
	public static bool operator ==(AdvancedDiscordUser e1, AdvancedDiscordUser e2)
	{
		var o1 = e1 as object;
		var o2 = e2 as object;

		return (o1 != null || o2 == null) && (o1 == null || o2 != null) && ((o1 == null && o2 == null) || e1.Id == e2.Id);
	}

	/// <summary>
	/// Gets whether the two <see cref="AdvancedDiscordUser"/> objects are not equal.
	/// </summary>
	/// <param name="e1">First user to compare.</param>
	/// <param name="e2">Second user to compare.</param>
	/// <returns>Whether the two users are not equal.</returns>
	public static bool operator !=(AdvancedDiscordUser e1, AdvancedDiscordUser e2)
		=> !(e1 == e2);
}

public abstract class HistoryObject
{
	internal HistoryObject(HistoryObjectType type)
	{
		this.Type = type;
	}

	public HistoryObjectType Type { get; internal set; }
}

public abstract class HistoryEntry
{
	internal HistoryEntry(DateTimeOffset datetime)
	{
		this.HistoryPoint = datetime;
	}

	public DateTimeOffset HistoryPoint { get; internal set; }
}

public class DiscrimHistory : HistoryObject
{
	internal DiscrimHistory()
		: base(HistoryObjectType.Discriminator)
	{

	}

	public List<DiscrimHistoryEntry> History { get; internal set; } = new();

	internal void AddEntry(DiscrimHistoryEntry entry)
		=> this.History.Add(entry);

	internal void Clear()
		=> this.History.Clear();
}

public class UsernameHistory : HistoryObject
{
	internal UsernameHistory()
		: base(HistoryObjectType.Username)
	{

	}

	public List<UsernameHistoryEntry> History { get; internal set; } = new();

	internal void AddEntry(UsernameHistoryEntry entry)
		=> this.History.Add(entry);

	internal void Clear()
		=> this.History.Clear();
}

public class GuildNicknameHistory : HistoryObject
{
	internal GuildNicknameHistory()
		: base(HistoryObjectType.GuildNicknames)
	{

	}

	public List<NicknameHistoryEntry> History { get; internal set; } = new();

	internal void AddEntry(NicknameHistoryEntry entry)
		=> this.History.Add(entry);

	internal void Clear()
		=> this.History.Clear();
}

public class DiscrimHistoryEntry : HistoryEntry
{
	public DiscrimHistoryEntry(DateTimeOffset datetime, int discriminator)
		: base(datetime)
	{
		this.Discriminator = discriminator;
	}

	public int Discriminator { get; internal set; }
}

public class NicknameHistoryEntry : HistoryEntry
{
	public NicknameHistoryEntry(DateTimeOffset datetime, string nickname, ulong guildId)
		: base(datetime)
	{
		this.Nickname = nickname;
		this.GuildId = guildId;
	}

	public string Nickname { get; internal set; }

	public ulong GuildId { get; internal set; }
}

public class UsernameHistoryEntry : HistoryEntry
{
	public UsernameHistoryEntry(DateTimeOffset datetime, string username)
		: base(datetime)
	{
		this.Username = username;
	}

	public string Username { get; internal set; }
}

/// <summary>
/// Represents a minimized ban entry without the user object, but with the guild id.
/// </summary>
public class SmallBan : SnowflakeObject
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SmallBan"/> class.
	/// </summary>
	/// <param name="userId">The user id.</param>
	/// <param name="guildId">The guild id the user is banned on.</param>
	/// <param name="banId">The ban id.</param>
	/// <param name="reason">The reason why the user was banned.</param>
	public SmallBan(ulong userId, ulong guildId, ulong banId, string reason = null)
	{
		this.GuildId = guildId;
		this.Id = banId;
		this.Reason = reason;
		this.UserId = userId;
	}

	/// <summary>
	/// The user id for internal usage.
	/// </summary>
	internal ulong UserId { get; set; }

	/// <summary>
	/// The guild id.
	/// </summary>
	public ulong GuildId { get; internal set; }

	/// <summary>
	/// The ban reason.
	/// </summary>
	public string Reason { get; internal set; }

	/// <summary>
	/// Removes the ban.
	/// </summary>
	/// <param name="reason">Optional reason.</param>
	/// <returns>Whether the ban could be removed.</returns>
	public async Task<bool> RemoveAsync(string reason = null)
	{
		try
		{
			await this.Discord.ApiClient.RemoveGuildBanAsync(this.GuildId, this.UserId, reason);
			return true;
		}
		catch (UnauthorizedException)
		{
			return false;
		}
		catch (NotFoundException)
		{
			return false;
		}
	}
}
