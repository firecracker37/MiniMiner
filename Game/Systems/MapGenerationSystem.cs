using Game.Components;
using Game.Utilities;
using SFML.Graphics;
using SFML.System;
using System.Diagnostics;

namespace Game.Systems
{
    public class MapGenerationSystem
    {
        private EntityManager _entityManager;
        private FastNoiseLite _noise;
        private RenderTexture _mapPreviewRenderTexture;

        public MapGenerationSystem(EntityManager entityManager)
        {
            _entityManager = entityManager;
            _noise = new FastNoiseLite();
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            _noise.SetFrequency(0.008f); // Smaller value will "zoom out" the noise
            _noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            _noise.SetFractalOctaves(5); // More octaves for more detail
            _noise.SetFractalLacunarity(2.0f); // Controls the frequency multiplier between successive octaves
            _noise.SetFractalGain(0.5f); // Controls the amplitude multiplier between successive octaves
        }

        public int GenerateChunk(int chunkX, int chunkY, int chunkSize)
        {
            // Create a new entity for the chunk
            int chunkEntityId = _entityManager.CreateEntity();

            // Initialize the ChunkComponent
            ChunkComponent chunkComponent = new ChunkComponent(chunkSize, chunkX, chunkY);
            _entityManager.AddComponent(chunkEntityId, chunkComponent);

            // Calculate world position offset based on the chunk position and size
            int worldXOffset = chunkX * chunkSize;
            int worldYOffset = chunkY * chunkSize;

            // Generate tiles within this chunk
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    // Get the noise value for the current world position
                    float noiseValue = _noise.GetNoise(worldXOffset + x, worldYOffset + y);

                    // Determine the tile type based on the noise value
                    TileType tileType = GetTileTypeFromNoise(noiseValue);

                    // Create a new entity for the tile
                    int tileEntityId = _entityManager.CreateEntity();
                    _entityManager.AddComponent(tileEntityId, new PositionComponent { X = worldXOffset + x, Y = worldYOffset + y });
                    _entityManager.AddComponent(tileEntityId, new TileComponent { Type = tileType });
                    _entityManager.AddComponent(tileEntityId, new RenderComponent { Color = GetColorForTileType(tileType) });

                    // Store the tile entity ID in the chunk component
                    chunkComponent.SetTileEntity(x, y, tileEntityId);
                }
            }

            // Update the chunk component with all the tile entity IDs
            _entityManager.UpdateComponent(chunkEntityId, chunkComponent);
            Console.WriteLine($"Created new chunk with ID: {chunkEntityId} at ({chunkComponent.ChunkX},{chunkComponent.ChunkY})");
            return chunkEntityId;
        }

        private TileType GetTileTypeFromNoise(float noiseValue)
        {
            // Convert the noise value to a tile type
            // Example: if noiseValue is between certain ranges, return a corresponding TileType
            // You can adjust the thresholds to your preference for how you want the terrain to look
            if (noiseValue < -0.6)
            {
                return TileType.Water;
            }
            else if (noiseValue < 0.4)
            {
                return TileType.Grass;
            }
            else
            {
                return TileType.Mountain;
            }
        }

        private Color GetColorForTileType(TileType tileType)
        {
            // Return a color based on the tile type
            switch (tileType)
            {
                case TileType.Water:
                    return new Color(0, 0, 205); // Blue
                case TileType.Grass:
                    return new Color(34, 139, 34); // Green
                case TileType.Mountain:
                    return new Color(112, 128, 144); // Gray
                default:
                    return Color.White;
            }
        }
    }
}
