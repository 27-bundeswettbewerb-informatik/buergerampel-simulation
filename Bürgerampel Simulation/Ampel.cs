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
    class Ampel
    {
        public Texture2D Grün; //Jede Ampel hat eine Texture für Grün
        public Texture2D Rot; //Jede Ampel hat eine Texture für Rot
        public Vector2 Position; //Jede Ampel hat eine Position
        public bool Aktiviert = false; //Der Boolean Aktiviert entscheidet ob sie Grün ist
    }
}
