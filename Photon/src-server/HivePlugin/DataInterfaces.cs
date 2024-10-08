﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataInterfaces.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Photon.Hive.Plugin
{
    using System.Collections;

    /// <summary>
    /// Op JoinGame modes.
    /// </summary>
    public class JoinModeConstants
    {
        /// <summary>Regular join. The room must exist.</summary>
        public const byte JoinOnly = 0;

        /// <summary>Join or create the room if it's not existing.</summary>
        public const byte CreateIfNotExists = 1;

        /// <summary>The room might be removed from server memory and should be loaded (if possible) from a web-service.</summary>
        public const byte RejoinOrJoin = 2;

        /// <summary>Only re-join will be allowed. If the user is not yet in the room, the Op JoinGame will fail.</summary>
        public const byte RejoinOnly = 3;
        
        /// <summary></summary>
        public const byte MAX_VALUE = 4;
    }

    public static class RoomOptionFlags
    {
        /// <summary>
        /// Enables UserId check when joining a room: if enabled, actors should have unique UserIDs.
        /// Allows rejoin using UserId.
        /// </summary>
        public const int CheckUserOnJoin = 0x01;
        /// <summary>
        /// Enables automatic cached events clean up of actors who leave the room.
        /// </summary>
        public const int DeleteCacheOnLeave = 0x02;
        /// <summary>
        /// Disables broadcast of Join and Leave events.
        /// </summary>
        public const int SuppressRoomEvents = 0x04;
        /// <summary>
        /// Enables sending UserId of actors in Join event and Join operation response.
        /// Can be ignored if SuppressPlayerInfo is used or 'incomplete' if SuppressRoomEvents is used.
        /// </summary>
        public const int PublishUserId = 0x08;
        /// <summary>
        /// Enables automatic deletion of properties pairs with null values.
        /// It also skips adding new properties with null values.
        /// </summary>
        public const int DeleteNullProps = 0x10;
        /// <summary>
        /// Enables true broadcast of PropertiesChanged event by sending it also to the actor who called SetProperties and triggered the change.
        /// </summary>
        public const int BroadcastPropsChangeToAll = 0x20;
        /// <summary>
        /// we suppress getting any info about joined users during joining process
        /// still it is possible to get this info using GetProperties operation
        /// this flag overrides value of SuppressRoomEvents flag
        /// </summary>
        public const int SuppressPlayerInfo = 0x40;
    }

    /// <summary>
    /// Base interface of Op LeaveRoom request containing the parameters as sent by client.
    /// </summary>
    public interface ILeaveGameRequest : IOperationRequest
    {
        /// <summary>
        /// Indicates if the actor can rejoin the room later and should be marked as inactive.
        /// </summary>
        bool IsCommingBack { get; set; }
    }

#if PLUGINS_0_9
    [Obsolete("Use ILeaveGameRequest instead")]
    public interface ILeaveRequest : ILeaveGameRequest
    {
    }
#endif

    /// <summary>
    /// Base interface of Op RaiseEvent request containing the parameters as sent by client.
    /// </summary>
    public interface IRaiseEventRequest : IOperationRequest
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the actors which should receive the event.
        ///   If set to null or an empty array the event will be sent
        ///   to all actors in the room.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        int[] Actors { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating how to use the <see cref = "EventCache" />.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        ///   Ignored if the event is sent to individual actors (submitted <see cref = "Actors" /> or <see cref = "Photon.Hive.Plugin.Operations.MasterClient" />).
        /// </remarks>
        byte Cache { get; set; }

        /// <summary>
        ///   Gets or sets the hashtable containing the data to send.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        object Data { get; set; }

        /// <summary>
        ///   Gets or sets a byte containing the Code to send.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        byte EvCode { get; set; }

        /// <summary>
        ///   Gets or sets the game id.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        /// </remarks>
        string GameId { get; set; }

        /// <summary>
        /// Target interest group if this event.
        /// </summary>
        byte Group { get; set; }

        /// <summary>
        /// Gets or sets HttpForward webflag indicating whether GameEvent webhook should be sent.
        /// </summary>
        bool HttpForward { get; set; }

        /// <summary>
        ///   Gets or sets the <see cref = "Photon.Hive.Operations.ReceiverGroup" /> for the event.
        /// </summary>
        /// <remarks>
        ///   Optional request parameter.
        ///   Ignored if <see cref = "Actors" /> are set.
        /// </remarks>
        byte ReceiverGroup { get; set; }

        /// <summary>
        /// Cache slice index to be used when caching option requires it.
        /// </summary>
        int? CacheSliceIndex { get; set; }

        #endregion
    }

    /// <summary>
    /// Base interface of Op Join request containing the parameters as sent by client.
    /// </summary>
    public interface IJoinGameRequest : IOperationRequest
    {
        #region Properties

        /// <summary>
        /// Actor number, which will be used for rejoin
        /// </summary>
        int ActorNr { get; set; }

        /// <summary>
        /// Gets or sets custom actor properties.
        /// </summary>
        Hashtable ActorProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the actor properties
        /// should be included in the <see cref="JoinEvent"/> event which 
        /// will be sent to all clients currently in the room.
        /// </summary>
        bool BroadcastActorProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cached events are automatically deleted for 
        /// actors which are leaving a room.
        /// </summary>
        bool DeleteCacheOnLeave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how long the room instance will be kept alive 
        /// in the room cache after all peers have left the room.
        /// </summary>
        /// <remarks>
        /// This property will only be applied for the room creator.
        /// </remarks>
        int EmptyRoomLiveTime { get; set; }

        /// <summary>
        /// Gets or sets the name of the game (room).
        /// </summary>
        string GameId { get; set; }

        /// <summary>
        /// Gets or sets custom game properties.
        /// </summary>
        /// <remarks>
        /// Game properties will only be applied for the game creator.
        /// </remarks>
        Hashtable GameProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if common room events (Join, Leave) will be suppressed.
        /// </summary>
        /// <remarks>
        /// This property will only be applied for the game creator.
        /// </remarks>
        bool SuppressRoomEvents { get; set; }

        /// <summary>
        /// True if JoinMode is CreateIfNotExists.
        /// </summary>
        bool CreateIfNotExists { get; }

        /// <summary>
        /// Name of the lobby to which this room belongs.
        /// </summary>
        string LobbyName { get; }

        /// <summary>
        /// Type of the lobby to which this room belongs.
        /// </summary>
        byte LobbyType { get; }

        /// <summary>
        /// Op Join mode. For possible values, <see cref="Photon.Hive.Plugin.JoinModeConstants"/>.
        /// </summary>
        byte JoinMode { get; }

        /// <summary>
        /// Different aspects of room behaviour in one place
        /// </summary>
        int RoomFlags { get; set; }

        /// <summary>
        /// Gets or sets how long in milliseconds an actor who leaves the room can remain inactive in the room's actors list (in order to be able to rejoin later).
        /// int.MaxValue or negative values should be considered as infinite time.
        /// </summary>
        int PlayerTTL { get; set; }
        #endregion
    }

    [Obsolete("User IJoinGameRequest instead")]
    public interface IJoinRequest : IJoinGameRequest
    { }

    /// <summary>
    /// Base interface of close request as sent by server.
    /// </summary>
    public interface ICloseRequest : IOperationRequest
    {
        #region Properties
        /// <summary>
        /// TTL (Time To Live) of empty room (when all actors left) before it gets removed from Photon servers.
        /// </summary>
        int EmptyRoomTTL { get; set; }

        #endregion
    }

    /// <summary>
    /// Base interface of Op SetProperties request containing the parameters as sent by client.
    /// </summary>
    public interface ISetPropertiesRequest : IOperationRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets ActorNumber.
        /// </summary>
        int ActorNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Broadcast.
        /// </summary>
        bool Broadcast { get; set; }

        /// <summary>
        /// Gets or sets HttpForward webflag indicating if GameProperties webhook should be sent.
        /// </summary>
        bool HttpForward { get; set; }

        /// <summary>
        /// Gets or sets Properties.
        /// </summary>
        Hashtable Properties { get; set; }

        /// <summary>
        /// Gets or sets Properties.
        /// </summary>
        Hashtable ExpectedValues { get; set; }

        #endregion
    }
}