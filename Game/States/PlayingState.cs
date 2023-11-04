using Game.Components;
using Game.Systems;
using SFML.Graphics;
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
        private uint _windowWidth;
        private EntityManager _entityManager;
        private MapGenerationSystem _mapGenerationSystem;
        private ChunkRenderSystem _chunkRenderSystem;
        private RenderSystem _renderSystem;

        // Constructor that takes a GameStateMachine
        public PlayingState(GameStateMachine stateMachine, uint windowWidth)
        {
            _stateMachine = stateMachine;
            _windowWidth = windowWidth;
            _entityManager = new EntityManager();
            _mapGenerationSystem = new MapGenerationSystem(_entityManager);
            _renderSystem = new RenderSystem(_entityManager);
            _chunkRenderSystem = new ChunkRenderSystem(_entityManager);
        }

        public void Enter()
        {
            // Generate a chunk at the specified position and size
            int chunkEntityId = _mapGenerationSystem.GenerateChunk(0, 0, 64); // Generate a chunk at (0,0) with size 64x64

            _entityManager.AddComponent(chunkEntityId, new GeneratedComponent());
        }

        public void Update(float deltaTime)
        {
            // Here you would update your systems, like a MovementSystem or CollisionSystem
        }

        public void HandleInput(RenderWindow window)
        {
            // Handle input for the game, which might affect your entities or trigger system actions
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
    }
}
