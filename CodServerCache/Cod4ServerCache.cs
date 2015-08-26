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
    public class Cod4ServerCache
    {
        public const int ServerSize = 0x9c;//156

        private const int HeaderSize = 0x10;
        private const int FileSize = 0x2fe990;//3 139 984 (- 16 == 20 128)
        private const int FavoritesOffset = 0x2f9b90;//3 120 016 (- 16 == 20 000)
        private const int ServerCount = (FavoritesOffset - HeaderSize) / ServerSize;
        private const int FavoriteServerCount = (FileSize - FavoritesOffset) / ServerSize;

        // fav sec 19968 (== 128 x struct)

        private readonly byte[] _contents;

        public Cod4ServerCache(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path)) throw new ArgumentException("path not found", nameof(path));
            var contents = File.ReadAllBytes(path);
            if (contents.Length != FileSize) throw new ArgumentException("invalid file size", nameof(path));

            Path = path;
            _contents = contents;
        }

        public string Path { get; }

        public IEnumerable<Cod4ServerData> Servers => Split(HeaderSize, ServerCount).Select(ToServer).Distinct();
        public IEnumerable<Cod4ServerData> FavoriteServers => Split(FavoritesOffset, FavoriteServerCount).Select(ToServer).Distinct();

        private IEnumerable<byte[]> Split(int startIndex, int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (IsEmptyRecord(_contents, startIndex + ServerSize * i))
                    continue;

                yield return _contents.Skip(startIndex).Skip(ServerSize * i).Take(ServerSize).ToArray();
            }
        } 

        private static Cod4ServerData ToServer(byte[] data)
        {
            return new Cod4ServerData(GetString(data, 0x31, 0x20),
                $"{string.Join(".", data.Skip(0x04).Take(0x04))}:{BitConverter.ToUInt16(data.Skip(0x08).Take(0x02).Reverse().ToArray(), 0)}",
                GetString(data, 0x51, 0x20), GetString(data, 0x71, 0x18), GetString(data, 0x89, 0x13), data[0x19],
                data[0x1a]);
        }

        public static Cod4ServerCache DetectCache()
        {
            string path =
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Activision\\Call of Duty 4", "InstallPath", null)?
                    .ToString() ??
                Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Activision\\Call of Duty 4", "InstallPath",
                    null)?.ToString();

            if (path == null) return null;
            path = System.IO.Path.Combine(path, "servercache.dat");

            return !File.Exists(path) ? null : new Cod4ServerCache(path);
        }
        
        private static bool IsEmptyRecord(byte[] data, int index)
        {
            return data.Length < index + ServerSize ||
                   Enumerable.Range(0, ServerSize)
                       .All(i => data[i + index] == (i == 27 ? 0x01 : (i == 42 || i == 43 ? 0xff : 0x00)));
        }

        private static string GetString(IEnumerable<byte> bytes, int index, int length)
        {
            var result = string.Empty;
            foreach(var b in bytes.Skip(index).Take(length))
            {
                if (b == 0x00)
                    return result;
                result += (char) b;
            }

            return result;
        }
    }
}