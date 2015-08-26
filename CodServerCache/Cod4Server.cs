// CodServerCache
// Copyright 2015 Tim Potze
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CodServerCache
{
    /// <summary>
    ///     Represents a Call of Duty 4: Modern Warfare server entry.
    /// </summary>
    public struct Cod4Server
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Cod4Server" /> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="map">The map.</param>
        /// <param name="mod">The mod.</param>
        /// <param name="gameMode">The game mode.</param>
        /// <param name="playersOnline">The players online.</param>
        /// <param name="maxPlayers">The maximum players.</param>
        public Cod4Server(string name, string ip, string map, string mod, string gameMode, int playersOnline,
            int maxPlayers)
        {
            Name = name;
            Ip = ip;
            Map = map;
            Mod = mod;
            GameMode = gameMode;
            PlayersOnline = playersOnline;
            MaxPlayers = maxPlayers;
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the ip.
        /// </summary>
        public string Ip { get; }

        /// <summary>
        ///     Gets the map.
        /// </summary>
        public string Map { get; }

        /// <summary>
        ///     Gets the mod.
        /// </summary>
        public string Mod { get; }

        /// <summary>
        ///     Gets the game mode.
        /// </summary>
        public string GameMode { get; }

        /// <summary>
        ///     Gets the players online.
        /// </summary>
        public int PlayersOnline { get; }

        /// <summary>
        ///     Gets the maximum players.
        /// </summary>
        public int MaxPlayers { get; }

        #region Overrides of ValueType

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return $"{Name}({Ip}) playing {GameMode}({Mod}) on {Map} with {PlayersOnline}/{MaxPlayers}";
        }

        #endregion
    }
}