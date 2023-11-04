using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components
{
    public struct ChunkComponent
    {
        public int[] TileEntities; // Array to store entity IDs of tiles in the chunk
        public int ChunkSize; // The size of the chunk (e.g., 32x32 tiles)
        public int ChunkX; // The chunk's X position in the world
        public int ChunkY; // The chunk's Y position in the world

        public ChunkComponent(int chunkSize, int chunkX, int chunkY)
        {
            ChunkSize = chunkSize;
            ChunkX = chunkX;
            ChunkY = chunkY;
            TileEntities = new int[chunkSize * chunkSize]; // Assuming a square chunk
        }

        // Helper method to access tile entity IDs in a 2D fashion
        public int GetTileEntity(int x, int y)
        {
            return TileEntities[y * ChunkSize + x];
        }

        public void SetTileEntity(int x, int y, int entityId)
        {
            TileEntities[y * ChunkSize + x] = entityId;
        }
    }

}
