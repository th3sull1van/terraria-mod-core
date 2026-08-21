using System;
using System.Collections.Generic;
using Terraria;

namespace OreCascade
{
    public struct TilePos : IEquatable<TilePos>
    {
        public readonly int X;
        public readonly int Y;

        public TilePos(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(TilePos other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is TilePos other && Equals(other);
        public override int GetHashCode() => (X * 397) ^ Y;
        public override string ToString() => $"({X}, {Y})";
    }

    /// <summary>
    /// Performs graph traversal across the tile matrix to discover connected ore veins using BFS.
    /// </summary>
    public static class VeinFinder
    {
        private static readonly int[] CardinalDx = { 0, 0, -1, 1 };
        private static readonly int[] CardinalDy = { -1, 1, 0, 0 };

        private static readonly int[] FullDx = { 0, 0, -1, 1, -1, 1, -1, 1 };
        private static readonly int[] FullDy = { -1, 1, 0, 0, -1, -1, 1, 1 };

        public static List<TilePos> FindVein(int startX, int startY, ushort initialOreType, CascadeConfig config)
        {
            int maxBlocks = config?.MaxBlocksPerActivation ?? 100;
            bool allowDiagonal = config != null && config.AllowDiagonalConnections;

            int[] dx = allowDiagonal ? FullDx : CardinalDx;
            int[] dy = allowDiagonal ? FullDy : CardinalDy;
            int neighborCount = dx.Length;

            var result = new List<TilePos>();
            var visited = new HashSet<TilePos>();
            var queue = new Queue<TilePos>();

            var startPos = new TilePos(startX, startY);
            visited.Add(startPos);
            queue.Enqueue(startPos);

            int maxX = Main.maxTilesX;
            int maxY = Main.maxTilesY;

            while (queue.Count > 0 && result.Count < maxBlocks)
            {
                TilePos current = queue.Dequeue();

                for (int i = 0; i < neighborCount; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];

                    if (nx < 0 || ny < 0 || nx >= maxX || ny >= maxY)
                    {
                        continue;
                    }

                    var neighborPos = new TilePos(nx, ny);
                    if (visited.Contains(neighborPos))
                    {
                        continue;
                    }

                    visited.Add(neighborPos);

                    Tile neighborTile = Main.tile[nx, ny];
                    if (neighborTile == null || !neighborTile.active())
                    {
                        continue;
                    }

                    if (OreClassifier.IsMatching(initialOreType, neighborTile.type, config))
                    {
                        result.Add(neighborPos);
                        queue.Enqueue(neighborPos);

                        if (result.Count >= maxBlocks)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
