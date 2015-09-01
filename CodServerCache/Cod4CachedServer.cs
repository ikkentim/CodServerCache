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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CodServerCache
{
    /// <summary>
    ///     Represents a Call of Duty 4: Modern Warfare server entry.
    /// </summary>
    public struct Cod4CachedServer
    {
        public const int Size = 0x9c; 

        private readonly byte[] _data;
        private readonly int _index;

        public IEnumerable<byte> Data => _data.Skip(_index).Take(Size); 
        /// <summary>
        /// Initializes a new instance of the <see cref="Cod4CachedServer"/> struct.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        public Cod4CachedServer(byte[] data, int index)
        {
            _data = data;
            _index = index;
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name => GetString(0x31, 0x20);

        /// <summary>
        ///     Gets the ip.
        /// </summary>
        public IPAddress Ip => new IPAddress(_data.Skip(_index + 0x04).Take(0x04).ToArray());

        /// <summary>
        ///     Gets the port.
        /// </summary>
        public int Port => (_data[_index + 0x09] << 8) + _data[_index + 0x08];

        /// <summary>
        ///     Gets the ping.
        /// </summary>
        public int Ping => (_data[_index + 0x2B] << 8) + _data[_index + 0x2A];

        /// <summary>
        ///     Gets the map.
        /// </summary>
        public string Map => GetString(0x51, 0x20);

        /// <summary>
        ///     Gets the mod.
        /// </summary>
        public string Mod => GetString(0x71, 0x18);

        /// <summary>
        ///     Gets the game mode.
        /// </summary>
        public string GameMode => GetString(0x89, 0x13);

        /// <summary>
        ///     Gets the players online.
        /// </summary>
        public byte PlayersOnline => _data[_index + 0x19];

        /// <summary>
        ///     Gets the maximum players.
        /// </summary>
        public byte MaxPlayers => _data[_index + 0x1a];
        
        /// <summary>
        ///     Gets a value indicating whether this instance is an empty record.
        /// </summary>
        public bool IsPinged => Ping != 0xffff;// _data[_index + 0x1b] == 0x01;

        private string GetString(int index, int length)
        {
            var r = string.Empty;
            for (var i = 0; i < length && _data[_index + index + i] != 0x00; i++)
                r += (char) _data[_index + index + i];
            return r;
        }

        #region Overrides of ValueType

        /// <summary>
        ///     Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> containing a fully qualified type name.
        /// </returns>
        public override string ToString()
        {
            return $"{Name}({Ip}:{Port}) playing {GameMode}({Mod}) on {Map} with {PlayersOnline}/{MaxPlayers}";
        }

        #endregion
    }
}