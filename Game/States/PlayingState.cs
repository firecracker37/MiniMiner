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
        private ChunkRenderSystem _chunkRenderSystem;
        private CameraZoomSystem _cameraZoomSystem;
        private CameraMovementSystem _cameraMovementSystem;

        // Constructor that takes a GameStateMachine
        public PlayingState(GameStateMachine stateMachine, RenderWindow window)
        {
            _stateMachine = stateMachine;
            _window = window;
            _entityManager = new EntityManager();
            _mapGenerationSystem = new MapGenerationSystem(_entityManager);
            _chunkRenderSystem = new ChunkRenderSystem(_entityManager);
            _cameraZoomSystem = new CameraZoomSystem(_entityManager, _window);
            _cameraMovementSystem = new CameraMovementSystem(_entityManager, _window);
        }

        public void Enter()
        {
            // Generate a chunk at the specified position and size
            int chunkEntityId = _mapGenerationSystem.GenerateChunk(0, 0, 64); // Generate a chunk at (0,0) with size 64x64
            _entityManager.AddComponent(chunkEntityId, new GeneratedComponent());

            // Calculate chunk dimensions in pixels
            int chunkPixelSize = 64 * 16;

            // Create a camera entity with initial position set to the center of the chunk
            int cameraEntityId = _entityManager.CreateEntity();
            _entityManager.AddComponent(cameraEntityId, new CameraComponent
            {
                Position = new Vector2f(chunkPixelSize / 2f, chunkPixelSize / 2f), // Center of the chunk
                ViewSize = new Vector2f(_window.Size.X, _window.Size.Y), // Assuming a window size or desired view size of 800x600
                ZoomLevel = 1.0f, // Default zoom level
                MinZoom = 0.25f,   // Minimum zoom level
                MaxZoom = 4.0f,    // Maximum zoom level
                PanningSpeed = 300f, // Speed at which the camera pans, adjust as needed
                Bounds = new FloatRect(0, 0, _window.Size.X * 2, _window.Size.Y *2) // Adding some extra space around the chunk
            });

            _entityManager.AddComponent(cameraEntityId, new MainCameraComponent()); // Mark this as the main camera

            SetupEventHandlers(_window);
        }

        public void Update(float deltaTime)
        {
            _cameraMovementSystem.Update(deltaTime);
        }

        public void HandleInput(RenderWindow window)
        {
            
        }

        public void Draw(RenderWindow window)
        {
            _chunkRenderSystem.Draw(window);
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
