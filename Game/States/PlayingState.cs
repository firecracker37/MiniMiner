using Game.Components;
using Game.Systems;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.States
{
    public class PlayingState : IGameState
    {
        private GameStateMachine _stateMachine;
        private RenderWindow _window;
        private EntityManager _entityManager;
        private MapGenerationSystem _mapGenerationSystem;
        private RenderSystem _renderSystem;
        private CameraZoomSystem _cameraZoomSystem;
        private CameraMovementSystem _cameraMovementSystem;
        private ChunkManagementSystem _chunkManagementSystem;

        // Constructor that takes a GameStateMachine
        public PlayingState(GameStateMachine stateMachine, RenderWindow window)
        {
            _stateMachine = stateMachine;
            _window = window;
            _entityManager = new EntityManager();
            _mapGenerationSystem = new MapGenerationSystem(_entityManager);
            _renderSystem = new RenderSystem(_entityManager);
            _cameraZoomSystem = new CameraZoomSystem(_entityManager, _window);
            _cameraMovementSystem = new CameraMovementSystem(_entityManager, _window);
            _chunkManagementSystem = new ChunkManagementSystem(_entityManager, _mapGenerationSystem);
        }

        public void Enter()
        {
            Console.WriteLine("Constructing PlayingState Started...");
            // Define the initial grid size
            const int initialGridRadius = 2; // This will generate a 3x3 grid

            // Generate the initial grid of chunks
            for (int x = -initialGridRadius; x <= initialGridRadius; x++)
            {
                for (int y = -initialGridRadius; y <= initialGridRadius; y++)
                {
                    var chunkEntityId = _mapGenerationSystem.GenerateChunk(x, y, 64);
                    _entityManager.AddComponent(chunkEntityId, new GeneratedComponent());
                }
            }

            // Calculate chunk dimensions in pixels
            int chunkPixelSize = 64 * 16;

            // Create a camera entity with initial position set to the center of the chunk
            int cameraEntityId = _entityManager.CreateEntity();
            _entityManager.AddComponent(cameraEntityId, new CameraComponent
            {
                Position = new Vector2f(chunkPixelSize / 2f, chunkPixelSize / 2f), // Center of the chunk
                ViewSize = new Vector2f(_window.Size.X, _window.Size.Y), 
                ZoomLevel = 1.0f, // Default zoom level
                MinZoom = 0.25f,   // Minimum zoom level
                MaxZoom = 4.0f,    // Maximum zoom level
                PanningSpeed = 300f, // Speed at which the camera pans, adjust as needed
                Bounds = new FloatRect(0, 0, _window.Size.X * 20, _window.Size.Y *20) // Adding some extra space around the chunk
            });

            _entityManager.AddComponent(cameraEntityId, new MainCameraComponent()); // Mark this as the main camera

            SetupEventHandlers(_window);
            Console.WriteLine("Completed PlayingState Construction...");
        }

        public void Update(float deltaTime)
        {
            _cameraMovementSystem.Update(deltaTime);

            // Get the main camera entity
            var cameraEntity = _entityManager.GetAllEntitiesWithComponent<CameraComponent>()
                                             .FirstOrDefault(entity => _entityManager.HasComponent<MainCameraComponent>(entity));

            if (cameraEntity != default)
            {
                // Get the camera component
                CameraComponent cameraComponent = _entityManager.GetComponent<CameraComponent>(cameraEntity);

                // Pass the camera position to the chunk management system
                _chunkManagementSystem.Update(cameraComponent.Position);
            }
        }

        public void HandleInput(RenderWindow window)
        {
            
        }

        public void Draw(RenderWindow window)
        {
            _renderSystem.Draw(window);
        }

        public void Exit()
        {
            // Clean up entities and systems
            _entityManager.ClearAllEntities();
        }

        private void SetupEventHandlers(RenderWindow window)
        {
            window.Closed += (sender, e) => window.Close(); // Handle the Closed event

            window.MouseWheelScrolled += (sender, e) =>
            {
                if (e.Wheel == Mouse.Wheel.VerticalWheel)
                {
                    _cameraZoomSystem.AdjustZoom(e.Delta);
                }
            };
        }
    }
}
