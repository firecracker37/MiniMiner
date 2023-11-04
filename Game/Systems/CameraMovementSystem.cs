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
    public class CameraMovementSystem
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private EntityManager _entityManager;
        private RenderWindow _window;
        
        public CameraMovementSystem(EntityManager entityManager, RenderWindow window)
        {
            _entityManager = entityManager;
            _window = window;
        }

        public void Update(float deltaTime)
        {
            // Get the camera entity and its component
            var cameraEntities = _entityManager.GetAllEntitiesWithComponent<CameraComponent>();

            foreach (var cameraEntity in cameraEntities)
            {
                if (_entityManager.HasComponent<MainCameraComponent>(cameraEntity))
                {
                    CameraComponent cameraComponent = _entityManager.GetComponent<CameraComponent>(cameraEntity);

                    // Calculate movement based on input
                    if (Keyboard.IsKeyPressed(Keyboard.Key.W)) MoveCamera(Direction.Up, deltaTime, cameraEntity);
                    if (Keyboard.IsKeyPressed(Keyboard.Key.S)) MoveCamera(Direction.Down, deltaTime, cameraEntity);
                    if (Keyboard.IsKeyPressed(Keyboard.Key.A)) MoveCamera(Direction.Left, deltaTime, cameraEntity);
                    if (Keyboard.IsKeyPressed(Keyboard.Key.D)) MoveCamera(Direction.Right, deltaTime, cameraEntity);

                    break; // Since we found and updated the main camera, we can break out of the loop
                }
            }
        }

        // MoveCamera now also takes the cameraEntity ID to update the specific camera's component
        public void MoveCamera(Direction direction, float deltaTime, int cameraEntity)
        {
            CameraComponent cameraComponent = _entityManager.GetComponent<CameraComponent>(cameraEntity);

            Vector2f movement = new Vector2f(0, 0);
            switch (direction)
            {
                case Direction.Up:
                    movement.Y -= cameraComponent.PanningSpeed * deltaTime;
                    break;
                case Direction.Down:
                    movement.Y += cameraComponent.PanningSpeed * deltaTime;
                    break;
                case Direction.Left:
                    movement.X -= cameraComponent.PanningSpeed * deltaTime;
                    break;
                case Direction.Right:
                    movement.X += cameraComponent.PanningSpeed * deltaTime;
                    break;
            }

            cameraComponent.Position += movement;

            // Ensure the camera stays within bounds
            // You would need to define bounds based on your world size
            cameraComponent.Position.X = Math.Clamp(cameraComponent.Position.X, cameraComponent.Bounds.Left, cameraComponent.Bounds.Left + cameraComponent.Bounds.Width);
            cameraComponent.Position.Y = Math.Clamp(cameraComponent.Position.Y, cameraComponent.Bounds.Top, cameraComponent.Bounds.Top + cameraComponent.Bounds.Height);

            // Update the camera component
            _entityManager.UpdateComponent(cameraEntity, cameraComponent);

            // Adjust the view position based on the camera's new position
            var view = _window.GetView();
            view.Center = cameraComponent.Position;
            _window.SetView(view); // Apply the new view
        }
    }
}
