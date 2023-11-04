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

        public void GenerateMap(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Get the noise value for the current position
                    float noiseValue = _noise.GetNoise(x, y); // The 0.1f is a scale factor for the noise

                    // Determine the tile type based on the noise value
                    TileType tileType = GetTileTypeFromNoise(noiseValue);

                    // Output for debugging
                    //Console.WriteLine($"Position: ({x},{y}) Noise Value: {noiseValue} Tile Type: {tileType}");

                    // Create a new entity for the tile
                    int entityId = _entityManager.CreateEntity();
                    _entityManager.AddComponent(entityId, new PositionComponent { X = x, Y = y });
                    _entityManager.AddComponent(entityId, new TileComponent { Type = tileType });
                    _entityManager.AddComponent(entityId, new RenderComponent { Color = GetColorForTileType(tileType) });
                }
            }
        }

        public void GenerateMapPreview(int width, int height)
        {
            Stopwatch stopwatch = Stopwatch.StartNew(); // Start the stopwatch

            // Create a RenderTexture
            RenderTexture renderTexture = new RenderTexture((uint)width, (uint)height);

            // Clear with a background color
            renderTexture.Clear(Color.Black);

            long initialMemory = GC.GetTotalMemory(false); // Get initial memory usage

            // Reuse the same RectangleShape for drawing
            RectangleShape rectangle = new RectangleShape(new Vector2f(1, 1));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float noiseValue = _noise.GetNoise(x, y);
                    TileType tileType = GetTileTypeFromNoise(noiseValue);
                    rectangle.FillColor = GetColorForTileType(tileType);
                    rectangle.Position = new Vector2f(x, y);

                    renderTexture.Draw(rectangle);
                }
            }

            // Finish drawing
            renderTexture.Display();

            long finalMemory = GC.GetTotalMemory(false); // Get final memory usage
            long memoryUsed = finalMemory - initialMemory; // Calculate memory used for the operation

            stopwatch.Stop(); // Stop the stopwatch
            TimeSpan timeTaken = stopwatch.Elapsed; // Get the elapsed time

            // Output the results
            Console.WriteLine($"Generated {width * height} tiles in {timeTaken.TotalMilliseconds} milliseconds.");
            Console.WriteLine($"Memory used for map generation: {memoryUsed / 1024.0 / 1024.0} MB.");

            // Create an entity for the map preview
            int mapPreviewEntity = _entityManager.CreateEntity();

            // Add the MapPreviewComponent to the entity with the generated texture
            _entityManager.AddComponent(mapPreviewEntity, new MapPreviewComponent
            {
                PreviewTexture = renderTexture.Texture
            });
            _mapPreviewRenderTexture = renderTexture;
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
