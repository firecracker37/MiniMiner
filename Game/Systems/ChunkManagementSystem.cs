using Game.Components;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems
{
    public class ChunkManagementSystem
    {
        private EntityManager _entityManager;
        private MapGenerationSystem _mapGenerationSystem;
        private Dictionary<string, int> _activeChunks;
        private Queue<int> _chunksToUnload;

        public ChunkManagementSystem(EntityManager entityManager, MapGenerationSystem mapGenerationSystem)
        {
            _entityManager = entityManager;
            _mapGenerationSystem = mapGenerationSystem;
            _activeChunks = new Dictionary<string, int>();
            _chunksToUnload = new Queue<int>();
        }

        public void Update(Vector2f cameraPosition)
        {
            LoadChunksAroundCamera(cameraPosition);
            UnloadDistantChunks(cameraPosition);
        }

        private void LoadChunksAroundCamera(Vector2f cameraPosition)
        {
            const int loadRadius = 2; // Load chunks within a 5x5 grid centered on the camera
            Vector2i cameraChunkPosition = GetChunkCoordinatesFromPosition(cameraPosition);

            for (int x = cameraChunkPosition.X - loadRadius; x <= cameraChunkPosition.X + loadRadius; x++)
            {
                for (int y = cameraChunkPosition.Y - loadRadius; y <= cameraChunkPosition.Y + loadRadius; y++)
                {
                    string chunkId = CalculateChunkId(x, y);
                    if (!_activeChunks.ContainsKey(chunkId))
                    {
                        int chunkEntityId = _mapGenerationSystem.GenerateChunk(x, y, 64);
                        _entityManager.AddComponent(chunkEntityId, new GeneratedComponent());
                        _activeChunks.Add(chunkId, chunkEntityId); // Add the chunk ID as the key and entity ID as the value.
                    }
                }
            }
        }

        private void UnloadDistantChunks(Vector2f cameraPosition)
        {
            // Determine which chunks need to be unloaded
            // Mark them for unloading and add to _chunksToUnload
            // You can perform the actual unloading at a suitable time to avoid frame spikes
        }

        private void PerformUnloading()
        {
            while (_chunksToUnload.Count > 0)
            {
                int chunkEntityId = _chunksToUnload.Dequeue();
                // Perform cleanup of the chunk and its entities
                _entityManager.RemoveComponent<GeneratedComponent>(chunkEntityId);

                // Find the key in the dictionary associated with this chunkEntityId
                string keyToRemove = _activeChunks.FirstOrDefault(pair => pair.Value == chunkEntityId).Key;
                if (keyToRemove != null)
                {
                    _activeChunks.Remove(keyToRemove);
                }

                // Further cleanup as necessary...
                // For example, removing all tile entities associated with this chunk
            }
        }

        private Vector2i GetChunkCoordinatesFromPosition(Vector2f position)
        {
            int chunkSizeInWorldUnits = 64 * 16; // if each tile is 16x16 pixels and each chunk is 64x64 tiles
            int chunkX = (int)position.X / chunkSizeInWorldUnits;
            int chunkY = (int)position.Y / chunkSizeInWorldUnits;
            return new Vector2i(chunkX, chunkY);
        }

        private string CalculateChunkId(int x, int y)
        {
            return $"{x}_{y}";
        }
    }
}
