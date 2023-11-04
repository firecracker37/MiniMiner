using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.States
{
    public interface IGameState
    {
        void Enter();
        void Update(float deltaTime);
        void HandleInput(RenderWindow window);
        void Draw(RenderWindow window);
        void Exit();
    }
}
