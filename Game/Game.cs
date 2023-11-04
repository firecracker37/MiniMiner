using Game.States;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Game
    {
        private readonly RenderWindow _window;
        private readonly GameStateMachine _stateMachine;
        private readonly Clock _clock;
        private float _timeSinceLastUpdate = 0f;
        private const float TimePerFrame = 1f / 60f;

        public Game()
        {
            _window = new RenderWindow(new VideoMode(1600, 900), "SFML.Net Game");
            _window.Closed += (_, _) => _window.Close();
            _stateMachine = new GameStateMachine();
            _clock = new Clock();

            // Pass the state machine to states so they can request transitions
            _stateMachine.ChangeState(new MainMenuState(_stateMachine, _window));
        }

        public void Run()
        {
            while (_window.IsOpen)
            {
                float deltaTime = _clock.Restart().AsSeconds();
                _timeSinceLastUpdate += deltaTime;

                while (_timeSinceLastUpdate > TimePerFrame)
                {
                    _timeSinceLastUpdate -= TimePerFrame;

                    // Delegate event processing to the state machine
                    ProcessEvents();

                    // Delegate update to the state machine
                    _stateMachine.Update(TimePerFrame);
                }

                // Delegate rendering to the state machine
                Render();
            }
        }

        private void ProcessEvents()
        {
            _window.DispatchEvents(); // This line is necessary to process window events

            _stateMachine.HandleInput(_window);
        }

        private void Render()
        {
            _window.Clear(Color.Black);

            // Delegate the rendering to the current state
            _stateMachine.Draw(_window);

            _window.Display();
        }
    }

}
