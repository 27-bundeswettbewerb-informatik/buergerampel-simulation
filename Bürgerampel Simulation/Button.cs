using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Bürgerampel_Simulation
{
    class Button
    {
        // Alle Buttons haben die folgenden Eigenschaften:
        public Texture2D Sprite; // Das auf dem Bildschirm zu malende Bild
        public Vector2 Position; // Die Position des Buttons auf dem Bildschirm
        public Rectangle Viereckgröße;
    }
}

