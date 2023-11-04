using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components
{
    public struct CameraComponent
    {
        public Vector2f Position; // Center position of the camera in the world
        public Vector2f ViewSize; // Size of the camera's view in the world
        public float ZoomLevel; // Current zoom level of the camera
        public float MinZoom; // Minimum zoom level
        public float MaxZoom; // Maximum zoom level
        public float PanningSpeed; // Speed at which the camera pans
        public FloatRect Bounds;
    }
}
