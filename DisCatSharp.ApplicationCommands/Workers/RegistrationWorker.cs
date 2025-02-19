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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DisCatSharp.Common;
using DisCatSharp.Entities;
using DisCatSharp.Exceptions;

using Microsoft.Extensions.Logging;

namespace DisCatSharp.ApplicationCommands;

/// <summary>
/// Represents a <see cref="RegistrationWorker"/>.
/// </summary>
internal class RegistrationWorker
{
	/// <summary>
	/// Registers the global commands.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="commands">The command list.</param>
	/// <returns>A list of registered commands.</returns>
	internal static async Task<List<DiscordApplicationCommand>> RegisterGlobalCommandsAsync(DiscordClient client, List<DiscordApplicationCommand> commands)
	{
		var (changedCommands, unchangedCommands) = BuildGlobalOverwriteList(client, commands);
		var globalCommandsCreateList = BuildGlobalCreateList(client, commands);
		var globalCommandsDeleteList = BuildGlobalDeleteList(client, commands);

		if (globalCommandsCreateList.NotEmptyAndNotNull() && unchangedCommands.NotEmptyAndNotNull() && changedCommands.NotEmptyAndNotNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GLOBAL] Creating, re-using and overwriting application commands.");

			foreach (var cmd in globalCommandsCreateList)
			{
				var discordBackendCommand = await client.CreateGlobalApplicationCommandAsync(cmd);
				commands.Add(discordBackendCommand);
			}

			foreach (var cmd in changedCommands)
			{
				var command = cmd.Value;
				var discordBackendCommand = await client.EditGlobalApplicationCommandAsync(cmd.Key, action =>
				{
					action.Name = command.Name;
					action.NameLocalizations = command.NameLocalizations;
					action.Description = command.Description;
					action.DescriptionLocalizations = command.DescriptionLocalizations;
					if(command.Options != null && command.Options.Any())
						action.Options = Entities.Optional.Some(command.Options);
					if (command.DefaultMemberPermissions.HasValue)
						action.DefaultMemberPermissions = command.DefaultMemberPermissions.Value;
					if (command.DmPermission.HasValue)
						action.DmPermission = command.DmPermission.Value;
					//action.IsNsfw = command.IsNsfw;
				});

				commands.Add(discordBackendCommand);
			}

			commands.AddRange(unchangedCommands);
		}
		else if (globalCommandsCreateList.NotEmptyAndNotNull() && (unchangedCommands.NotEmptyAndNotNull() || changedCommands.NotEmptyAndNotNull()))
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GLOBAL] Creating, re-using and overwriting application commands.");

			foreach (var cmd in globalCommandsCreateList)
			{
				var discordBackendCommand = await client.CreateGlobalApplicationCommandAsync(cmd);
				commands.Add(discordBackendCommand);
			}

			if (changedCommands.NotEmptyAndNotNull())
			{
				foreach (var cmd in changedCommands)
				{
					var command = cmd.Value;
					var discordBackendCommand = await client.EditGlobalApplicationCommandAsync(cmd.Key, action =>
					{
						action.Name = command.Name;
						action.NameLocalizations = command.NameLocalizations;
						action.Description = command.Description;
						action.DescriptionLocalizations = command.DescriptionLocalizations;
						if(command.Options != null && command.Options.Any())
							action.Options = Entities.Optional.Some(command.Options);
						if (command.DefaultMemberPermissions.HasValue)
							action.DefaultMemberPermissions = command.DefaultMemberPermissions.Value;
						if (command.DmPermission.HasValue)
							action.DmPermission = command.DmPermission.Value;
						//action.IsNsfw = command.IsNsfw;
					});

					commands.Add(discordBackendCommand);
				}
			}

			if (unchangedCommands.NotEmptyAndNotNull())
				commands.AddRange(unchangedCommands);
		}
		else if (globalCommandsCreateList.EmptyOrNull() && unchangedCommands.NotEmptyAndNotNull() && changedCommands.NotEmptyAndNotNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GLOBAL] Editing & re-using application commands.");

			foreach (var cmd in changedCommands)
			{
				var command = cmd.Value;
				var discordBackendCommand = await client.EditGlobalApplicationCommandAsync(cmd.Key, action =>
				{
					action.Name = command.Name;
					action.NameLocalizations = command.NameLocalizations;
					action.Description = command.Description;
					action.DescriptionLocalizations = command.DescriptionLocalizations;
					if(command.Options != null && command.Options.Any())
						action.Options = Entities.Optional.Some(command.Options);
					if (command.DefaultMemberPermissions.HasValue)
						action.DefaultMemberPermissions = command.DefaultMemberPermissions.Value;
					if (command.DmPermission.HasValue)
						action.DmPermission = command.DmPermission.Value;
					//action.IsNsfw = command.IsNsfw;
				});

				commands.Add(discordBackendCommand);
			}

			commands.AddRange(unchangedCommands);
		}
		else if (globalCommandsCreateList.EmptyOrNull() && changedCommands.NotEmptyAndNotNull() && unchangedCommands.EmptyOrNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GLOBAL] Overwriting all application commands.");

			List<DiscordApplicationCommand> overwriteList = new();
			foreach (var overwrite in changedCommands)
			{
				var cmd = overwrite.Value;
				cmd.Id = overwrite.Key;
				overwriteList.Add(cmd);
			}
			var discordBackendCommands = await client.BulkOverwriteGlobalApplicationCommandsAsync(overwriteList);
			commands.AddRange(discordBackendCommands);
		}
		else if (globalCommandsCreateList.NotEmptyAndNotNull() && changedCommands.EmptyOrNull() && unchangedCommands.EmptyOrNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GLOBAL] Creating all application commands.");

			var cmds = await client.BulkOverwriteGlobalApplicationCommandsAsync(globalCommandsCreateList);
			commands.AddRange(cmds);
		}
		else if (globalCommandsCreateList.EmptyOrNull() && changedCommands.EmptyOrNull() && unchangedCommands.NotEmptyAndNotNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GLOBAL] Re-using all application commands.");

			commands.AddRange(unchangedCommands);
		}

		if (globalCommandsDeleteList.NotEmptyAndNotNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GLOBAL] Deleting missing application commands.");

			foreach (var cmdId in globalCommandsDeleteList)
			{
				try
				{
					await client.DeleteGlobalApplicationCommandAsync(cmdId);
				}
				catch (NotFoundException)
				{
					client.Logger.LogError($"Could not delete global command {cmdId}. Please clean up manually");
				}
			}
		}

		return commands.NotEmptyAndNotNull() ? commands : null;
	}

	/// <summary>
	/// Registers the guild commands.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="guildId">The target guild id.</param>
	/// <param name="commands">The command list.</param>
	/// <returns>A list of registered commands.</returns>
	internal static async Task<List<DiscordApplicationCommand>> RegisterGuildCommandsAsync(DiscordClient client, ulong guildId, List<DiscordApplicationCommand> commands)
	{
		var (changedCommands, unchangedCommands) = BuildGuildOverwriteList(client, guildId, commands);
		var guildCommandsCreateList = BuildGuildCreateList(client, guildId, commands);
		var guildCommandsDeleteList = BuildGuildDeleteList(client, guildId, commands);

		if (guildCommandsCreateList.NotEmptyAndNotNull() && unchangedCommands.NotEmptyAndNotNull() && changedCommands.NotEmptyAndNotNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GUILD] Creating, re-using and overwriting application commands. Guild ID: {guildId}");

			foreach (var cmd in guildCommandsCreateList)
			{
				var discordBackendCommand = await client.CreateGuildApplicationCommandAsync(guildId, cmd);
				commands.Add(discordBackendCommand);
			}

			foreach (var cmd in changedCommands)
			{
				var command = cmd.Value;
				var discordBackendCommand = await client.EditGuildApplicationCommandAsync(guildId, cmd.Key, action =>
				{
					action.Name = command.Name;
					action.NameLocalizations = command.NameLocalizations;
					action.Description = command.Description;
					action.DescriptionLocalizations = command.DescriptionLocalizations;
					if(command.Options != null && command.Options.Any())
						action.Options = Entities.Optional.Some(command.Options);
					if (command.DefaultMemberPermissions.HasValue)
						action.DefaultMemberPermissions = command.DefaultMemberPermissions.Value;
					if (command.DmPermission.HasValue)
						action.DmPermission = command.DmPermission.Value;
					//action.IsNsfw = command.IsNsfw;
				});

				commands.Add(discordBackendCommand);
			}

			commands.AddRange(unchangedCommands);
		}
		else if (guildCommandsCreateList.NotEmptyAndNotNull() && (unchangedCommands.NotEmptyAndNotNull() || changedCommands.NotEmptyAndNotNull()))
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GUILD] Creating, re-using and overwriting application commands. Guild ID: {guildId}");

			foreach (var cmd in guildCommandsCreateList)
			{
				var discordBackendCommand = await client.CreateGuildApplicationCommandAsync(guildId, cmd);
				commands.Add(discordBackendCommand);
			}

			if (changedCommands.NotEmptyAndNotNull())
			{
				foreach (var cmd in changedCommands)
				{
					var command = cmd.Value;
					var discordBackendCommand = await client.EditGuildApplicationCommandAsync(guildId, cmd.Key, action =>
					{
						action.Name = command.Name;
						action.NameLocalizations = command.NameLocalizations;
						action.Description = command.Description;
						action.DescriptionLocalizations = command.DescriptionLocalizations;
						if(command.Options != null && command.Options.Any())
							action.Options = Entities.Optional.Some(command.Options);
						if (command.DefaultMemberPermissions.HasValue)
							action.DefaultMemberPermissions = command.DefaultMemberPermissions.Value;
						if (command.DmPermission.HasValue)
							action.DmPermission = command.DmPermission.Value;
						//action.IsNsfw = command.IsNsfw;
					});

					commands.Add(discordBackendCommand);
				}
			}

			if (unchangedCommands.NotEmptyAndNotNull())
				commands.AddRange(unchangedCommands);
		}
		else if (guildCommandsCreateList.EmptyOrNull() && unchangedCommands.NotEmptyAndNotNull() && changedCommands.NotEmptyAndNotNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GUILD] Editing & re-using application commands. Guild ID: {guildId}");

			foreach (var cmd in changedCommands)
			{
				var command = cmd.Value;
				var discordBackendCommand = await client.EditGuildApplicationCommandAsync(guildId, cmd.Key, action =>
				{
					action.Name = command.Name;
					action.NameLocalizations = command.NameLocalizations;
					action.Description = command.Description;
					action.DescriptionLocalizations = command.DescriptionLocalizations;
					if(command.Options != null && command.Options.Any())
						action.Options = Entities.Optional.Some(command.Options);
					if (command.DefaultMemberPermissions.HasValue)
						action.DefaultMemberPermissions = command.DefaultMemberPermissions.Value;
					if (command.DmPermission.HasValue)
						action.DmPermission = command.DmPermission.Value;
					//action.IsNsfw = command.IsNsfw;
				});

				commands.Add(discordBackendCommand);
			}

			commands.AddRange(unchangedCommands);
		}
		else if (guildCommandsCreateList.EmptyOrNull() && changedCommands.NotEmptyAndNotNull() && unchangedCommands.EmptyOrNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GUILD] Overwriting all application commands. Guild ID: {guildId}");

			List<DiscordApplicationCommand> overwriteList = new();
			foreach (var overwrite in changedCommands)
			{
				var cmd = overwrite.Value;
				cmd.Id = overwrite.Key;
				overwriteList.Add(cmd);
			}
			var discordBackendCommands = await client.BulkOverwriteGuildApplicationCommandsAsync(guildId, overwriteList);
			commands.AddRange(discordBackendCommands);
		}
		else if (guildCommandsCreateList.NotEmptyAndNotNull() && changedCommands.EmptyOrNull() && unchangedCommands.EmptyOrNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GUILD] Creating all application commands. Guild ID: {guildId}");

			var cmds = await client.BulkOverwriteGuildApplicationCommandsAsync(guildId, guildCommandsCreateList);
			commands.AddRange(cmds);
		}
		else if (guildCommandsCreateList.EmptyOrNull() && changedCommands.EmptyOrNull() && unchangedCommands.NotEmptyAndNotNull())
		{
			client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GUILD] Re-using all application commands Guild ID: {guildId}.");

			commands.AddRange(unchangedCommands);
		}

		if (guildCommandsDeleteList.NotEmptyAndNotNull())
		{
			foreach (var cmdId in guildCommandsDeleteList)
			{
				client.Logger.Log(ApplicationCommandsExtension.ApplicationCommandsLogLevel, $"[AC GUILD] Deleting missing application commands. Guild ID: {guildId}");
				try
				{
					await client.DeleteGuildApplicationCommandAsync(guildId, cmdId);
				}
				catch (NotFoundException)
				{
					client.Logger.LogError($"Could not delete guild command {cmdId} in guild {guildId}. Please clean up manually");
				}
			}
		}

		return commands.NotEmptyAndNotNull() ? commands : null;
	}

	/// <summary>
	/// Builds a list of guild command ids to be deleted on discords backend.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="guildId">The guild id these commands belong to.</param>
	/// <param name="updateList">The command list.</param>
	/// <returns>A list of command ids.</returns>
	private static List<ulong> BuildGuildDeleteList(DiscordClient client, ulong guildId, List<DiscordApplicationCommand> updateList)
	{
		List<DiscordApplicationCommand> discord;

		if (ApplicationCommandsExtension.GuildDiscordCommands == null || !ApplicationCommandsExtension.GuildDiscordCommands.Any()
			|| !ApplicationCommandsExtension.GuildDiscordCommands.GetFirstValueByKey(guildId, out discord)
		)
			return null;

		List<ulong> invalidCommandIds = new();

		if (discord == null)
			return null;

		if (updateList == null)
		{
			foreach (var cmd in discord)
			{
				invalidCommandIds.Add(cmd.Id);
			}
		}
		else
		{
			foreach (var cmd in discord)
			{
				if (!updateList.Any(ul => ul.Name == cmd.Name))
					invalidCommandIds.Add(cmd.Id);
			}
		}

		return invalidCommandIds;
	}

	/// <summary>
	/// Builds a list of guild commands to be created on discords backend.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="guildId">The guild id these commands belong to.</param>
	/// <param name="updateList">The command list.</param>
	/// <returns></returns>
	private static List<DiscordApplicationCommand> BuildGuildCreateList(DiscordClient client, ulong guildId, List<DiscordApplicationCommand> updateList)
	{
		List<DiscordApplicationCommand> discord;

		if (ApplicationCommandsExtension.GuildDiscordCommands == null || !ApplicationCommandsExtension.GuildDiscordCommands.Any()
			|| updateList == null || !ApplicationCommandsExtension.GuildDiscordCommands.GetFirstValueByKey(guildId, out discord)
		)
			return updateList;

		List<DiscordApplicationCommand> newCommands = new();

		if (discord == null)
			return updateList;

		foreach (var cmd in updateList)
		{
			if (discord.All(d => d.Name != cmd.Name))
			{
				newCommands.Add(cmd);
			}
		}

		return newCommands;
	}

	/// <summary>
	/// Builds a list of guild commands to be overwritten on discords backend.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="guildId">The guild id these commands belong to.</param>
	/// <param name="updateList">The command list.</param>
	/// <returns>A dictionary of command id and command.</returns>
	private static (
			Dictionary<ulong, DiscordApplicationCommand> changedCommands,
			List<DiscordApplicationCommand> unchangedCommands
		) BuildGuildOverwriteList(DiscordClient client, ulong guildId, List<DiscordApplicationCommand> updateList)
	{
		List<DiscordApplicationCommand> discord;

		if (ApplicationCommandsExtension.GuildDiscordCommands == null || !ApplicationCommandsExtension.GuildDiscordCommands.Any()
			|| ApplicationCommandsExtension.GuildDiscordCommands.All(l => l.Key != guildId) || updateList == null
			|| !ApplicationCommandsExtension.GuildDiscordCommands.GetFirstValueByKey(guildId, out discord)
		)
			return (null, null);

		Dictionary<ulong, DiscordApplicationCommand> updateCommands = new();
		List<DiscordApplicationCommand> unchangedCommands = new();

		if (discord == null)
			return (null, null);

		foreach (var cmd in updateList)
		{
			if (discord.GetFirstValueWhere(d => d.Name == cmd.Name, out var command))
			{
				if (command.IsEqualTo(cmd, client))
				{
					if (ApplicationCommandsExtension.DebugEnabled)
						client.Logger.LogDebug($"[AC] Command {cmd.Name} unchanged");
					cmd.Id = command.Id;
					cmd.ApplicationId = command.ApplicationId;
					cmd.Version = command.Version;
					unchangedCommands.Add(cmd);
				}
				else
				{
					if (ApplicationCommandsExtension.DebugEnabled)
						client.Logger.LogDebug($"[AC] Command {cmd.Name} changed");
					updateCommands.Add(command.Id, cmd);
				}
			}
		}

		return (updateCommands, unchangedCommands);
	}

	/// <summary>
	/// Builds a list of global command ids to be deleted on discords backend.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="updateList">The command list.</param>
	/// <returns>A list of command ids.</returns>
	private static List<ulong> BuildGlobalDeleteList(DiscordClient client, List<DiscordApplicationCommand> updateList = null)
	{
		if (ApplicationCommandsExtension.GlobalDiscordCommands == null || !ApplicationCommandsExtension.GlobalDiscordCommands.Any()
			|| ApplicationCommandsExtension.GlobalDiscordCommands == null
		)
			return null;

		var discord = ApplicationCommandsExtension.GlobalDiscordCommands;

		List<ulong> invalidCommandIds = new();

		if (discord == null)
			return null;

		if (updateList == null)
		{
			foreach (var cmd in discord)
			{
				invalidCommandIds.Add(cmd.Id);
			}
		}
		else
		{
			foreach (var cmd in discord)
			{
				if (updateList.All(ul => ul.Name != cmd.Name))
					invalidCommandIds.Add(cmd.Id);
			}
		}

		return invalidCommandIds;
	}

	/// <summary>
	/// Builds a list of global commands to be created on discords backend.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="updateList">The command list.</param>
	/// <returns>A list of commands.</returns>
	private static List<DiscordApplicationCommand> BuildGlobalCreateList(DiscordClient client, List<DiscordApplicationCommand> updateList)
	{
		if (ApplicationCommandsExtension.GlobalDiscordCommands == null || !ApplicationCommandsExtension.GlobalDiscordCommands.Any() || updateList == null)
			return updateList;

		var discord = ApplicationCommandsExtension.GlobalDiscordCommands;

		List<DiscordApplicationCommand> newCommands = new();

		if (discord == null)
			return updateList;

		foreach (var cmd in updateList)
		{
			if (discord.All(d => d.Name != cmd.Name))
			{
				newCommands.Add(cmd);
			}
		}

		return newCommands;
	}

	/// <summary>
	/// Builds a list of global commands to be overwritten on discords backend.
	/// </summary>
	/// <param name="client">The discord client.</param>
	/// <param name="updateList">The command list.</param>
	/// <returns>A dictionary of command ids and commands.</returns>
	private static (
			Dictionary<ulong, DiscordApplicationCommand> changedCommands,
			List<DiscordApplicationCommand> unchangedCommands
		) BuildGlobalOverwriteList(DiscordClient client, List<DiscordApplicationCommand> updateList)
	{
		if (ApplicationCommandsExtension.GlobalDiscordCommands == null || !ApplicationCommandsExtension.GlobalDiscordCommands.Any()
			|| updateList == null || ApplicationCommandsExtension.GlobalDiscordCommands == null
		)
			return (null, null);

		var discord = ApplicationCommandsExtension.GlobalDiscordCommands;

		if (discord == null)
			return (null, null);

		Dictionary<ulong, DiscordApplicationCommand> updateCommands = new();
		List<DiscordApplicationCommand> unchangedCommands = new();
		foreach (var cmd in updateList)
		{
			if (discord.GetFirstValueWhere(d => d.Name == cmd.Name, out var command))
			{
				if (command.IsEqualTo(cmd, client))
				{
					if (ApplicationCommandsExtension.DebugEnabled)
						client.Logger.LogDebug($"[AC] Command {cmd.Name} unchanged");
					cmd.Id = command.Id;
					cmd.ApplicationId = command.ApplicationId;
					cmd.Version = command.Version;
					unchangedCommands.Add(cmd);
				}
				else
				{
					if (ApplicationCommandsExtension.DebugEnabled)
						client.Logger.LogDebug($"[AC] Command {cmd.Name} changed");
					updateCommands.Add(command.Id, cmd);
				}
			}
		}

		return (updateCommands, unchangedCommands);
	}
}
