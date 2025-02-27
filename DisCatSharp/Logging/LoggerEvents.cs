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

using Microsoft.Extensions.Logging;

namespace DisCatSharp;

/// <summary>
/// Contains well-defined event IDs used by core of DisCatSharp.
/// </summary>
public static class LoggerEvents
{
	/// <summary>
	/// Miscellaneous events, that do not fit in any other category.
	/// </summary>
	public static EventId Misc { get; } = new(100, "DisCatSharp");

	/// <summary>
	/// Events pertaining to startup tasks.
	/// </summary>
	public static EventId Startup { get; } = new(101, nameof(Startup));

	/// <summary>
	/// Events typically emitted whenever WebSocket connections fail or are terminated.
	/// </summary>
	public static EventId ConnectionFailure { get; } = new(102, nameof(ConnectionFailure));

	/// <summary>
	/// Events pertaining to Discord-issued session state updates.
	/// </summary>
	public static EventId SessionUpdate { get; } = new(103, nameof(SessionUpdate));

	/// <summary>
	/// Events emitted when exceptions are thrown in handlers attached to async events.
	/// </summary>
	public static EventId EventHandlerException { get; } = new(104, nameof(EventHandlerException));

	/// <summary>
	/// Events emitted for various high-level WebSocket receive events.
	/// </summary>
	public static EventId WebSocketReceive { get; } = new(105, nameof(WebSocketReceive));

	/// <summary>
	/// Events emitted for various low-level WebSocket receive events.
	/// </summary>
	public static EventId WebSocketReceiveRaw { get; } = new(106, nameof(WebSocketReceiveRaw));

	/// <summary>
	/// Events emitted for various low-level WebSocket send events.
	/// </summary>
	public static EventId WebSocketSendRaw { get; } = new(107, nameof(WebSocketSendRaw));

	/// <summary>
	/// Events emitted for various WebSocket payload processing failures, typically when deserialization or decoding fails.
	/// </summary>
	public static EventId WebSocketReceiveFailure { get; } = new(108, nameof(WebSocketReceiveFailure));

	/// <summary>
	/// Events pertaining to connection lifecycle, specifically, heartbeats.
	/// </summary>
	public static EventId Heartbeat { get; } = new(109, nameof(Heartbeat));

	/// <summary>
	/// Events pertaining to various heartbeat failures, typically fatal.
	/// </summary>
	public static EventId HeartbeatFailure { get; } = new(110, nameof(HeartbeatFailure));

	/// <summary>
	/// Events pertaining to clean connection closes.
	/// </summary>
	public static EventId ConnectionClose { get; } = new(111, nameof(ConnectionClose));

	/// <summary>
	/// Events emitted when REST processing fails for any reason.
	/// </summary>
	public static EventId RestError { get; } = new(112, nameof(RestError));

	/// <summary>
	/// Events pertaining to the <see cref="DiscordShardedClient"/> shard startup.
	/// </summary>
	public static EventId ShardStartup { get; } = new(113, nameof(ShardStartup));

	/// <summary>
	/// Events pertaining to ratelimit exhaustion.
	/// </summary>
	public static EventId RatelimitHit { get; } = new(114, nameof(RatelimitHit));

	/// <summary>
	/// Events pertaining to ratelimit diagnostics. Typically contain raw bucket info.
	/// </summary>
	public static EventId RatelimitDiag { get; } = new(115, nameof(RatelimitDiag));

	/// <summary>
	/// Events emitted when a ratelimit is exhausted and a request is preemptively blocked.
	/// </summary>
	public static EventId RatelimitPreemptive { get; } = new(116, nameof(RatelimitPreemptive));

	/// <summary>
	/// Events pertaining to audit log processing.
	/// </summary>
	public static EventId AuditLog { get; } = new(117, nameof(AuditLog));

	/// <summary>
	/// Events containing raw (but decompressed) payloads, received from Discord Gateway.
	/// </summary>
	public static EventId GatewayWsRx { get; } = new(118, "Gateway ↓");

	/// <summary>
	/// Events containing raw payloads, as they're being sent to Discord Gateway.
	/// </summary>
	public static EventId GatewayWsTx { get; } = new(119, "Gateway ↑");

	/// <summary>
	/// Events pertaining to Gateway Intents. Typically diagnostic information.
	/// </summary>
	public static EventId Intents { get; } = new(120, nameof(Intents));

	/// <summary>
	/// Events pertaining to autosharded client shard shutdown, clean or otherwise.
	/// </summary>
	public static EventId ShardShutdown { get; } = new(121, nameof(ShardShutdown));

	/// <summary>
	/// Events pertaining to the <see cref="DiscordShardedClient"/>'s shards not initializing correctly.
	/// </summary>
	public static EventId ShardClientError { get; } = new(122, nameof(ShardClientError));

	/// <summary>
	/// Events containing raw payloads, as they're received from Discord's REST API.
	/// </summary>
	public static EventId RestRx { get; } = new(123, "REST ↓");

	/// <summary>
	/// Events containing raw payloads, as they're sent to Discord's REST API.
	/// </summary>
	public static EventId RestTx { get; } = new(124, "REST ↑");

	/// <summary>
	/// Event is rest cleaner.
	/// </summary>
	public static EventId RestCleaner { get; } = new(125, nameof(RestCleaner));

	/// <summary>
	/// Event is rest hash mover.
	/// </summary>
	public static EventId RestHashMover { get; } = new(126, nameof(RestHashMover));

	/// <summary>
	/// Events pertaining to Discord API requests from the <see cref="DiscordShardedClient"/>.
	/// </summary>
	public static EventId ShardRest { get; } = new(127, nameof(ShardRest));
}
