using System;

using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Opiso.Client {
    public class Game {
        private Map m;
        private Surface screen;

        public Game() {
            screen = Video.SetVideoMode(400, 300);
            Events.Fps = 30;
            Events.Tick += new EventHandler<TickEventArgs>(Tick);
            Events.Quit += new EventHandler<QuitEventArgs>(Quit);
        }

        public static void Main(string[] args) {
            Game g = new Game();
            g.Go();
        }

        public void Go() {
            m = new Map();
            m.LoadFrom("map1.json");
            m.LoadTilesets();
            Events.Run();
        }

        private void Quit(object sender, QuitEventArgs e) {
            Events.QuitApplication();
        }

        private void Tick(object sender, TickEventArgs e) {            
            var drawer = new MapDrawer();
            drawer.Map = m;
            drawer.DrawObjects = (s, i, sec)  => { 
                if (sec.TileTop == 2) {
                    s.Blit(m.GetSurfaceForTileId(306), sec.GetTilePosition(2, 2));
                }
            };
            var section = new ScreenSection();
            section.TileLeft = 0;
            section.TileTop = 0;
            section.TileWidth = 5;
            section.TileHeight = 5;
            section.XOffset = 0;
            section.YOffset = 0;
            section.TileSize = 32;
            drawer.Draw(screen, section);
            screen.Update();
        }
    }
}