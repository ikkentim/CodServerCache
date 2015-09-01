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
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace CodServerCache
{
    /// <summary>
    ///     Represents a Call of Duty 4: Modern Warfare servercache.dat file.
    /// </summary>
    public class Cod4ServerCache
    {
        private const int FileSize = 0x2fe990;
        private const int PublicServerOffset = 0x10;
        private const int PublicServerCount = (FavoriteServerOffset - PublicServerOffset) / Cod4CachedServer.Size;
        private const int FavoriteServerOffset = 0x2f9b90;
        private const int FavoriteServerCount = (FileSize - FavoriteServerOffset) / Cod4CachedServer.Size;

        private readonly byte[] _data;

        public IEnumerable<byte> Header => _data.Take(PublicServerOffset);
         
        /// <summary>
        ///     Initializes a new instance of the <see cref="Cod4ServerCache" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException">Thrown if path is null</exception>
        /// <exception cref="ArgumentException">Thrown if path not found or invalid file size</exception>
        public Cod4ServerCache(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new ArgumentException("path not found", nameof(path));
            var contents = File.ReadAllBytes(path);
            if (contents.Length != FileSize) throw new ArgumentException("invalid file size", nameof(path));

            Path = path;
            _data = contents;
        }

        /// <summary>
        ///     Gets the path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     Gets the public servers.
        /// </summary>
        public IEnumerable<Cod4CachedServer> PublicServers
            =>
                Enumerable.Range(0, PublicServerCount)
                    .Select(index => new Cod4CachedServer(_data, PublicServerOffset + index*Cod4CachedServer.Size))
                    .Where(server => server.IsPinged);

        /// <summary>
        ///     Gets the favorite servers.
        /// </summary>
        public IEnumerable<Cod4CachedServer> FavoriteServers
            =>
                Enumerable.Range(0, Math.Min(FavoriteServerCount, (int)_data[0x08]))
                    .Select(index => new Cod4CachedServer(_data, FavoriteServerOffset + index * Cod4CachedServer.Size))
                    .Where(server => server.IsPinged);

        /// <summary>
        ///     Detects the cache file of the installed copy of Call of Duty 4: Modern Warfare.
        /// </summary>
        /// <returns>An instance of <see cref="Cod4ServerCache" /> or null if the game or servercache could not be found.</returns>
        public static Cod4ServerCache DetectCache()
        {
            var path =
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Activision\\Call of Duty 4", "InstallPath", null)?
                    .ToString() ??
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Activision\\Call of Duty 4", "InstallPath",
                    null)?.ToString();

            if (path == null) return null;
            path = System.IO.Path.Combine(path, "servercache.dat");

            return !File.Exists(path) ? null : new Cod4ServerCache(path);
        }
    }
}