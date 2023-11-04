using Game.Components;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems
{
    public class CameraZoomSystem
    {
        private EntityManager _entityManager;
        private RenderWindow _window;

        // Define zoom sensitivity (how much each scroll affects the zoom)
        private const float ZoomSensitivity = 0.1f;

        public CameraZoomSystem(EntityManager entityManager, RenderWindow window)
        {
            _entityManager = entityManager;
            _window = window;
        }

        public void AdjustZoom(float delta)
        {
            var cameraEntities = _entityManager.GetAllEntitiesWithComponent<CameraComponent>();

            foreach (var cameraEntity in cameraEntities)
            {
                if (_entityManager.HasComponent<MainCameraComponent>(cameraEntity))
                {
                    // Get the main camera
                    CameraComponent cameraComponent = _entityManager.GetComponent<CameraComponent>(cameraEntity);

                    // Adjust the zoom level
                    cameraComponent.ZoomLevel += delta * ZoomSensitivity;
                    cameraComponent.ZoomLevel = Math.Clamp(cameraComponent.ZoomLevel, cameraComponent.MinZoom, cameraComponent.MaxZoom);

                    // Update the camera component
                    _entityManager.UpdateComponent(cameraEntity, cameraComponent);

                    // Adjust the view scale based on the zoom level
                    var view = _window.GetView();
                    view.Size = new Vector2f(_window.Size.X / cameraComponent.ZoomLevel, _window.Size.Y / cameraComponent.ZoomLevel);
                    _window.SetView(view); // Apply the new view

                    break; // Since we found and updated the main camera, we can break out of the loop
                }
            }
        }
    }

}
