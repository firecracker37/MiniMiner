using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.States
{
    public class GameStateMachine
    {
        private IGameState _currentState;

        public void ChangeState(IGameState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void Update(float deltaTime)
        {
            _currentState?.Update(deltaTime);
        }

        public void HandleInput(RenderWindow window)
        {
            _currentState?.HandleInput(window);
        }

        public void Draw(RenderWindow window)
        {
            _currentState?.Draw(window);
        }
    }

}
