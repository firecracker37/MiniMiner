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
        private EntityManager entityManager;

        public RenderSystem(EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        public void Draw(RenderWindow window)
        {
            // First, draw the map preview if it exists
            var mapPreviewEntities = entityManager.GetAllEntitiesWithComponent<MapPreviewComponent>();
            foreach (var mapPreviewEntity in mapPreviewEntities)
            {
                MapPreviewComponent preview = entityManager.GetComponent<MapPreviewComponent>(mapPreviewEntity);
                Sprite previewSprite = new Sprite(preview.PreviewTexture);
                window.Draw(previewSprite);
            }

            // Next, draw individual tiles (this will not be necessary if we are drawing a map preview)
            if (mapPreviewEntities.Count == 0) // Only draw tiles if no map preview is available
            {
                foreach (var entity in entityManager.GetAllEntities())
                {
                    if (entityManager.HasComponent<PositionComponent>(entity) &&
                        entityManager.HasComponent<RenderComponent>(entity))
                    {
                        PositionComponent position = entityManager.GetComponent<PositionComponent>(entity);
                        RenderComponent render = entityManager.GetComponent<RenderComponent>(entity);

                        // Draw the tile (as a rectangle for now)
                        RectangleShape tileShape = new RectangleShape(new Vector2f(1, 1))
                        {
                            FillColor = render.Color,
                            Position = new Vector2f(position.X * 1, position.Y * 1) // Assuming each tile is 32x32 pixels
                        };

                        window.Draw(tileShape);
                    }
                }
            }
        }
    }
}
