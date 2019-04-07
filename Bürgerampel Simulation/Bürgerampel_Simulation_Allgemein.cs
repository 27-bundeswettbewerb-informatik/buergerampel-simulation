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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Bürgerampel_Simulation_Allgemein : Microsoft.Xna.Framework.Game
    {
        //Die Klassen GraphicsDeviceManager, SpriteBatch, SpriteFont und PrimitiveBatch vom XNA Framework werden geladen
        GraphicsDeviceManager graphics; //Allgemeine Einstellungen wie Fenstergröße, Hintergrundfarbe, etc.
        SpriteBatch spriteBatch; //Die Klasse zum Ausgeben von Sprites und Strings
        SpriteFont spriteFontgamefont; //Die benutze Schriftart für den spriteBatch

        MouseState Vorheriger_Maus_Status = Mouse.GetState(); //Die Position und Status(Buttons) der Maus
        Texture2D Maus_Texture; //Die Texture für die Maus

        Ampel StraßenAmpel; //Die Instanzierung der Straßenampel
        Ampel FußgängerAmpel; //Die Instanzierung der FußgängerAmpel

        Button StraßenbedarfButton; //Es gibt einen Button zum Anmelden von Straßenbedarf
        Button FußgängerbedarfButton; //Es gibt einen Button zum Anmelden von Fußgängerbedarf
        
        double ÜbertrageneZeit; //Die Zeit die ständig aktualisiert wird

        bool StraßenBedarfAblaufWirdDurchgeführt = false; //Der Boolean der feststellt ob der Straßenbedarf-Ablauf durchgeführt wird
        bool FußgängerBedarfAblaufWirdDurchgeführt = false; //Der Boolean der feststellt ob der Fußgängerbedarf-Ablauf durchgeführt wird

        public Bürgerampel_Simulation_Allgemein()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Fenstergröße wird festgelegt
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            //Das Fenster wird im Vollbildmodus gestartet 
            graphics.ToggleFullScreen();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFontgamefont = Content.Load<SpriteFont>("Sprites\\gamefont");

            Maus_Texture = Content.Load<Texture2D>("Sprites\\Mauszeiger"); //Die Maus erhält ihre Texture

            StraßenbedarfButton = new Button(); //Der StraßenbedarfButton wird instanziert
            StraßenbedarfButton.Position = new Vector2(20, 20); //Der StraßenbedarfButton bekommt eine Position
            StraßenbedarfButton.Sprite = Content.Load<Texture2D>("Sprites\\Button Straßenbedarf"); //Der StraßenbedarfButton bekommt ein Bild
            StraßenbedarfButton.Viereckgröße = new Rectangle(20, 20, StraßenbedarfButton.Sprite.Width, StraßenbedarfButton.Sprite.Height); 
            //Der StraßenbedarfButton wird eine Viereckgröße zugewiesen, 
            //die sich aus der Position und der Länge under der Breite seines Bildes zusammen setzt

            FußgängerbedarfButton = new Button(); //Der FußgängerbedarfButton wird instanziert
            FußgängerbedarfButton.Position = new Vector2(20, 30 + StraßenbedarfButton.Sprite.Height); //Der FußgängerbedarfButton bekommt eine Position
            FußgängerbedarfButton.Sprite = Content.Load<Texture2D>("Sprites\\Button Fußgängerbedarf"); //Der FußgängerbedarfButton bekommt ein Bild
            FußgängerbedarfButton.Viereckgröße = new Rectangle(20, 20 + StraßenbedarfButton.Sprite.Height, FußgängerbedarfButton.Sprite.Width, FußgängerbedarfButton.Sprite.Height);
            //Der FußgängerbedarfButton wird eine Viereckgröße zugewiesen, 
            //die sich aus der Position und der Länge under der Breite seines Bildes zusammen setzt

            StraßenAmpel = new Ampel(); //Die StraßenAmpel wird instanziert
            StraßenAmpel.Grün = Content.Load<Texture2D>("Sprites\\Ampel Grün"); //Sie erhält ihr Bild für Grün
            StraßenAmpel.Rot = Content.Load<Texture2D>("Sprites\\Ampel Rot"); //und für Rot
            StraßenAmpel.Position = new Vector2(500,25); //Sie bekommt eine Position

            FußgängerAmpel = new Ampel(); //Die FußgängerAmpel wird instanziert
            FußgängerAmpel.Grün = Content.Load<Texture2D>("Sprites\\Bürgerampel Grün"); //Sie erhält ihr Bild für Grün
            FußgängerAmpel.Rot = Content.Load<Texture2D>("Sprites\\Bürgerampel Rot");   //und für Rot
            FußgängerAmpel.Position = new Vector2(StraßenAmpel.Position.X + StraßenAmpel.Grün.Width, StraßenAmpel.Position.Y); 
            //Sie bekommt eine Position, die sich der Position der StraßenAmpel und derren Breite zusammen setzt
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState Momentaner_Maus_Status = Mouse.GetState(); //Der momentane Maus-Status wird aktualisiert

            //Wenn die Maus in der vorherigen 80stel Sekunde die linkte Maustaste nicht gedrückt hatte und in dieser 80stel Sekunde doch, werden im folgenden die jeweiligen Funktionen der Buttons aufgerufen
            if ((Momentaner_Maus_Status.LeftButton == ButtonState.Pressed) && (Vorheriger_Maus_Status.LeftButton == ButtonState.Released))
            {//Im Folgenden wird bei jedem Button abgefragt ob die Maus mit einer Viereckgröße der Buttons einen Punkt gemeinsam hat
                if (StraßenbedarfButton.Viereckgröße.Contains(new Point((int)Momentaner_Maus_Status.X, (int)Momentaner_Maus_Status.Y)))
                {//Wenn der StraßenbedarfButton gedrückt wird
                    if (!StraßenAmpel.Aktiviert)
                    {//und die StraßenAmpel nicht aktiviert ist,
                        StraßenBedarfAblaufWirdDurchgeführt = true;//wird festgestellt das der Ablauf nun gestartet wurde
                        FußgängerBedarfAblaufWirdDurchgeführt = false;//und der andere Ablauf ggf. abgebrochen wird
                        FußgängerAmpel.Aktiviert = false;//Die andere Ampel wird deaktiviert
                        ÜbertrageneZeit = gameTime.TotalRealTime.TotalSeconds;//Die ÜbertrageneZeit wird mit der momentanen Programmzeit festgelegt
                    }
                }
                else
                    if (FußgängerbedarfButton.Viereckgröße.Contains(new Point((int)Momentaner_Maus_Status.X, (int)Momentaner_Maus_Status.Y)))
                    {//Wenn der FußgängerbedarfButton gedrückt wird
                        if (!FußgängerAmpel.Aktiviert)
                        {//und die FußgängerAmpel nicht aktiviert ist,
                            FußgängerBedarfAblaufWirdDurchgeführt = true;//wird festgestellt das der Ablauf nun gestartet wurde
                            StraßenBedarfAblaufWirdDurchgeführt = false;//und der andere Ablauf ggf. abgebrochen wird
                            StraßenAmpel.Aktiviert = false;//Die andere Ampel wird deaktiviert
                            ÜbertrageneZeit = gameTime.TotalRealTime.TotalSeconds;//Die ÜbertrageneZeit wird mit der momentanen Programmzeit festgelegt
                        }
                    }
            }
            if (FußgängerBedarfAblaufWirdDurchgeführt)
            {//Wenn der Fußgänger Bedarf angemeldet wurde
                Fußgänger_Bedarf_Ablauf(gameTime); //Wird der Fußgänger_Bedarf_Ablauf zur Programmzeit aktualisiert bis er beendet ist
            }
            
            
            
                if (StraßenBedarfAblaufWirdDurchgeführt)
                {//Wenn der Straßen Bedarf angemeldet wurde
                    Straßen_Bedarf_Ablauf(gameTime); //Wird der Straßen_Bedarf_Ablauf zur Programmzeit aktualisiert bis er beendet ist
                }

                Vorheriger_Maus_Status = Momentaner_Maus_Status;//Der vorheriger Maus-Status wird am Ende der 80stel Sekunde aktualisiert
                //Er ist der vorherige Momentaner_Maus_Status, welcher jedoch am Anfang der nächsten 80stel Sekunde aktualisiert wird
            base.Update(gameTime);
        }

        private void Straßen_Bedarf_Ablauf(GameTime gameTime)
        {
            if ((int)(gameTime.TotalRealTime.TotalSeconds - ÜbertrageneZeit) == 5)
            {//Wenn die Programmzeit - die Übertragene Zeit 5 erreicht, also 5 Sekunden seid dem Anmelden vergangen sind,
                StraßenBedarfAblaufWirdDurchgeführt = false; //wird festgelegt, dass der Ablauf vorbei ist
                StraßenAmpel.Aktiviert = true; //und die Straßenampel aktiviert wird
            }
        }

        private void Fußgänger_Bedarf_Ablauf(GameTime gameTime)
        {
            if ((int)(gameTime.TotalRealTime.TotalSeconds - ÜbertrageneZeit) == 3)
            {//Wenn die Programmzeit - die Übertragene Zeit 3 erreicht, also 3 Sekunden seid dem Anmelden vergangen sind,
                FußgängerBedarfAblaufWirdDurchgeführt = false; //wird festgelegt, dass der Ablauf vorbei ist
                FußgängerAmpel.Aktiviert = true; //und die Straßenampel aktiviert wird
                }            
        }

        protected override void Draw(GameTime gameTime)
        {
            MouseState Momentaner_Maus_Status = Mouse.GetState(); //Der momentane Maus-Status wird aktualisiert
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);//Der Hintergrund wird gefüllt
            spriteBatch.Begin();
            //Die beiden Bedarf Buttons werden ganz normal gezeichnet
            spriteBatch.Draw(StraßenbedarfButton.Sprite, StraßenbedarfButton.Position, Color.White);
            spriteBatch.Draw(FußgängerbedarfButton.Sprite, FußgängerbedarfButton.Position, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteBlendMode.Additive);
            //Wenn der Mauszeiger mit der Viereckgröße eines Buttons einen Punkt gemeinsam hat, dann wird dieser mit sich selbst vermischt (leuchtet)
            if (StraßenbedarfButton.Viereckgröße.Contains(new Point(Momentaner_Maus_Status.X, Momentaner_Maus_Status.Y)))             
                spriteBatch.Draw(StraßenbedarfButton.Sprite, StraßenbedarfButton.Position, Color.CornflowerBlue);
            if (FußgängerbedarfButton.Viereckgröße.Contains(new Point(Momentaner_Maus_Status.X, Momentaner_Maus_Status.Y)))        
                spriteBatch.Draw(FußgängerbedarfButton.Sprite, FußgängerbedarfButton.Position, Color.CornflowerBlue);        
            spriteBatch.End();

            spriteBatch.Begin();
            if(StraßenAmpel.Aktiviert)//Wenn die Straßenampel aktiviert ist leuchtet sie Grün
            spriteBatch.Draw(StraßenAmpel.Grün, StraßenAmpel.Position, Color.White);
        if (!StraßenAmpel.Aktiviert)//Wenn die Straßenampel nicht aktiviert ist leuchtet sie Rot
            spriteBatch.Draw(StraßenAmpel.Rot, StraßenAmpel.Position, Color.White);
        if (FußgängerAmpel.Aktiviert)//Wenn die FußgängerAmpel aktiviert ist leuchtet sie Grün
            spriteBatch.Draw(FußgängerAmpel.Grün, FußgängerAmpel.Position, Color.White);
        if (!FußgängerAmpel.Aktiviert)//Wenn die FußgängerAmpel nicht aktiviert ist leuchtet sie Rot
            spriteBatch.Draw(FußgängerAmpel.Rot, FußgängerAmpel.Position, Color.White);


        spriteBatch.Draw(Maus_Texture, new Vector2(Momentaner_Maus_Status.X, Momentaner_Maus_Status.Y), Color.White); //Die Maus wird gezeichnet
            
            if(StraßenBedarfAblaufWirdDurchgeführt)//Wenn der Straßenbedarf Ablauf im Gange ist wird dieses vermittelt
        spriteBatch.DrawString(spriteFontgamefont, "Es wurde Strassenbedarf \nangemeldet.", new Vector2(10, 200), Color.White);
    if (FußgängerBedarfAblaufWirdDurchgeführt)//Wenn der Gängerbedarf Ablauf im Gange ist wird dieses vermittelt
        spriteBatch.DrawString(spriteFontgamefont, "Es wurde Fussgaengerbedarf \nangemeldet.", new Vector2(10, 200), Color.White);

            //Ein Hinweis auf die zugeklebte Gelbe Leuchte der Ampel wird geschrieben
            spriteBatch.DrawString(spriteFontgamefont, "In der Aufgabenstellung wird betont, \ndass die gelbe Leuchte fuer den Versuch \nunbetrachtet bleibt, daher ist sie mit einem Aufkleber \nmit der Aufschrift 'defekt' versehen.", new Vector2(10, 350), Color.White);
            
            //Die Programmzeit wird zur Kontrolle ausgegeben
            spriteBatch.DrawString(spriteFontgamefont, "Reale Zeit seid dem Programmstart: " + gameTime.TotalRealTime, new Vector2(10, 550), Color.White);

            //Die Übertragene Zeit wird zur Kontrolle ausgegeben
                spriteBatch.DrawString(spriteFontgamefont, "Zum Zeitpunkt des angemeldeten Bedarfes \nnotierte Zeit: " + ÜbertrageneZeit, new Vector2(10, 600), Color.White);
                //Die Gleichung und die Differenz der Beiden Zahlen wird zur Kontrolle ausgegeben
            spriteBatch.DrawString(spriteFontgamefont, "" + (int)gameTime.TotalRealTime.TotalSeconds+ " - "+ (int)ÜbertrageneZeit + " = " + (int)(gameTime.TotalRealTime.TotalSeconds - ÜbertrageneZeit), new Vector2(10, 680), Color.White);
            spriteBatch.End();

            Vorheriger_Maus_Status = Momentaner_Maus_Status;//Der vorheriger Maus-Status wird am Ende der 80stel Sekunde aktualisiert
            //Er ist der vorherige Momentaner_Maus_Status, welcher jedoch am Anfang der nächsten 80stel Sekunde aktualisiert wird
            base.Draw(gameTime);
        }
    }
}
