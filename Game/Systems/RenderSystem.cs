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
    public class RenderSystem
    {
        private EntityManager _entityManager;

        public RenderSystem(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public void Draw(RenderWindow window)
        {
            // Assume we have a method to get the active camera's component
            CameraComponent camera = GetActiveCamera();

            // Calculate the view bounds based on the camera's position and zoom level
            FloatRect viewBounds = CalculateViewBounds(camera.Position, camera.ViewSize, camera.ZoomLevel);

            // Query the EntityManager for all active chunk entities
            var activeChunkEntities = _entityManager.GetAllEntitiesWithComponent<ChunkComponent>()
                                                   .Where(entity => _entityManager.HasComponent<GeneratedComponent>(entity));

            foreach (var chunkEntity in activeChunkEntities)
            {
                ChunkComponent chunk = _entityManager.GetComponent<ChunkComponent>(chunkEntity);

                // Calculate the world position of the chunk
                Vector2f chunkWorldPosition = CalculateChunkWorldPosition(chunk, camera.ZoomLevel);

                // Check if the chunk is within the view bounds
                if (IsChunkInView(chunkWorldPosition, chunk.ChunkSize, viewBounds))
                {
                    // Render the chunk
                    RenderChunk(window, chunk, chunkWorldPosition, camera.ZoomLevel);
                }
            }
        }

        private CameraComponent GetActiveCamera()
        {
            // Retrieve the active camera component (assuming there's only one camera entity)
            var cameraEntity = _entityManager.GetAllEntitiesWithComponent<MainCameraComponent>().FirstOrDefault();
            return _entityManager.GetComponent<CameraComponent>(cameraEntity);
        }
        
        FloatRect CalculateViewBounds(Vector2f cameraPosition, Vector2f viewSize, float zoomLevel)
        {
            // Calculate the size of the view in the world based on the zoom level
            float width = viewSize.X / zoomLevel;
            float height = viewSize.Y / zoomLevel;

            // Calculate the top-left position based on the camera position being the center of the view
            float left = cameraPosition.X - width / 2;
            float top = cameraPosition.Y - height / 2;

            return new FloatRect(left, top, width, height);
        }

        Vector2f CalculateChunkWorldPosition(ChunkComponent chunk, float zoomLevel)
        {
            // Assuming chunk.X and chunk.Y represent the chunk's coordinates in the grid
            // And assuming each tile is 16x16 pixels and each chunk is chunkSize tiles wide and tall
            int pixelSize = 16; // Tile size in pixels
            int chunkSizeInPixels = chunk.ChunkSize * pixelSize;

            // Calculate the world position of the top-left corner of the chunk
            float worldX = chunk.ChunkX * chunkSizeInPixels;
            float worldY = chunk.ChunkY * chunkSizeInPixels;

            // Adjust position based on zoom level if necessary
            // This adjustment assumes you want to scale the position with the zoom
            // You may or may not need this depending on how you handle zooming in your game
            worldX /= zoomLevel;
            worldY /= zoomLevel;

            return new Vector2f(worldX, worldY);
        }

        bool IsChunkInView(Vector2f chunkWorldPosition, int chunkSize, FloatRect viewBounds)
        {
            // Calculate the chunk bounds
            float chunkPixelSize = chunkSize * 16; // Assuming each tile is 16x16 pixels
            FloatRect chunkBounds = new FloatRect(chunkWorldPosition.X, chunkWorldPosition.Y, chunkPixelSize, chunkPixelSize);

            // Check if the chunk bounds intersect with the view bounds
            // The Intersects method returns true if the two rectangles overlap
            return viewBounds.Intersects(chunkBounds);
        }

        private void RenderChunk(RenderWindow window, ChunkComponent chunk, Vector2f chunkWorldPosition, float zoomLevel)
        {
            int chunkSize = chunk.ChunkSize;
            float tilePixelSize = 16; // Size of each tile in pixels at zoom level 1.0

            // Adjust the tile size based on the zoom level
            Vector2f tileSize = new Vector2f(tilePixelSize * zoomLevel, tilePixelSize * zoomLevel);

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

                        // Calculate the position of the tile within the chunk, adjusted for zoom
                        Vector2f tilePosition = new Vector2f(
                            (x * tilePixelSize + chunkWorldPosition.X) * zoomLevel,
                            (y * tilePixelSize + chunkWorldPosition.Y) * zoomLevel
                        );

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
