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
    public class MainMenuState : IGameState
    {
        private GameStateMachine _stateMachine;
        private Font _titleFont;
        private Text _titleText;
        private Font _actionFont;
        private Text _actionText;
        private uint _windowWidth;

        // Constructor that takes a GameStateMachine
        public MainMenuState(GameStateMachine stateMachine, uint windowWidth)
        {
            _stateMachine = stateMachine;
            _windowWidth = windowWidth; // Store the window width
        }

        public void Enter() 
        {
            // Load a font
            _titleFont = new Font("Assets/Fonts/bungee-shade.ttf");
            _actionFont = new Font("Assets/Fonts/press-start-2p.ttf");

            // Create a Text object
            _titleText = new Text("Mini Miner", _titleFont)
            {
                CharacterSize = 48,
                FillColor = Color.White,
            };

            _actionText = new Text("Click To Start", _actionFont)
            {
                CharacterSize = 36,
                FillColor = Color.White,
            };

            // Center the text horizontally
            FloatRect textRect = _titleText.GetLocalBounds();
            _titleText.Origin = new Vector2f(textRect.Left + textRect.Width / 2.0f, textRect.Top + textRect.Height / 2.0f);
            _titleText.Position = new Vector2f(_windowWidth / 2.0f, 100); // Assuming you want to place it at 100 pixels from the top

            FloatRect actionRect = _actionText.GetLocalBounds();
            _actionText.Origin = new Vector2f(actionRect.Left + actionRect.Width / 2.0f, actionRect.Top + actionRect.Height / 2.0f);
            _actionText.Position = new Vector2f(_windowWidth / 2.0f, 250);
        }
        public void Update(float deltaTime) 
        {
            
        }
        public void HandleInput(RenderWindow window) 
        {
            // Handle mouse input
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                // Get the mouse position relative to the window
                Vector2i mousePosition = Mouse.GetPosition(window); // 'window' needs to be passed in from the Game class

                // Check if the mouse click is within the bounds of the action text
                FloatRect actionRect = _actionText.GetGlobalBounds();
                if (actionRect.Contains(mousePosition.X, mousePosition.Y))
                {
                    // Change to the PlayingState
                    _stateMachine.ChangeState(new PlayingState(_stateMachine, _windowWidth));
                }
            }
        }
        public void Draw(RenderWindow window) 
        {
            window.Draw(_titleText);
            window.Draw(_actionText);
        }
        public void Exit() { /* Clean up menu */ }
    }
}
