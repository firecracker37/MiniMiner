using Game.Components;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems
{
    public class ChunkRenderSystem
    {
        private EntityManager _entityManager;

        public ChunkRenderSystem(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public void Draw(RenderWindow window)
        {
            // Get the size of the window
            Vector2u windowSize = window.Size;

            // Get all entities that are chunks and have been generated
            var generatedChunkEntities = _entityManager.GetAllEntitiesWithComponent<ChunkComponent>()
                                                       .Where(entity => _entityManager.HasComponent<GeneratedComponent>(entity));

            foreach (var chunkEntity in generatedChunkEntities)
            {
                ChunkComponent chunk = _entityManager.GetComponent<ChunkComponent>(chunkEntity);
                // Pass the window size to the DrawChunk method
                DrawChunk(window, chunk, windowSize);
            }
        }

        private void DrawChunk(RenderWindow window, ChunkComponent chunk, Vector2u windowSize)
        {
            int chunkSize = chunk.ChunkSize;
            Vector2f tileSize = new Vector2f(16, 16); // Assuming each tile is 16x16 pixels

            // Calculate the position offset to center the chunk in the window
            float offsetX = (windowSize.X - (chunkSize * tileSize.X)) / 2;
            float offsetY = (windowSize.Y - (chunkSize * tileSize.Y)) / 2;
            Vector2f centerOffset = new Vector2f(offsetX, offsetY);

            // Create a single RectangleShape to represent the tile, which we will reuse
            RectangleShape tileShape = new RectangleShape(tileSize);

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    int tileEntityId = chunk.GetTileEntity(x, y);

                    // If the tileEntityId is not 0 (or some invalid id), draw the tile
                    if (tileEntityId != 0)
                    {
                        RenderComponent renderComponent = _entityManager.GetComponent<RenderComponent>(tileEntityId);

                        // Set the fill color for the current tile
                        tileShape.FillColor = renderComponent.Color;

                        // Calculate the position of the tile within the chunk
                        Vector2f tilePosition = new Vector2f(x * tileSize.X, y * tileSize.Y);

                        // Apply the center offset to the position
                        tilePosition += centerOffset;

                        // Set the position of the shape to the current tile
                        tileShape.Position = tilePosition;

                        // Draw the rectangle to the window
                        window.Draw(tileShape);
                    }
                }
            }
        }

    }
}
