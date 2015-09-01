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
using System.IO;
using System.Linq;

namespace CodServerCache.Debugging
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cache = Cod4ServerCache.DetectCache();

            Console.WriteLine($"0x{BitConverter.ToString(cache.Header.Take(0x31).ToArray()).Replace("-", "")}");
            
            using (var file = File.CreateText("C:/Users/Tim/Desktop/out.txt"))
            {
                file.WriteLine(
                    $"  {string.Concat(Enumerable.Range(0, 0x31).Select(b => BitConverter.ToString(new[] {(byte) b})))}");

                foreach (var server in cache.PublicServers)
                {
                    file.Write($"0x{BitConverter.ToString(server.Data.Take(0x31).ToArray()).Replace("-", "")}");
                    file.WriteLine(server.Name + " " +server.Ip);
                }
            }
            Console.WriteLine("OK");
            Console.ReadLine();
        }
    }
}