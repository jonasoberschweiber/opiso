using System.Drawing;

namespace Opiso.Client {
    public struct ScreenSection {
        public int TileLeft { get; set; }
        public int TileTop { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }
        public int TileSize { get; set; }
        
        public Point GetTilePosition(int x, int y) {
            return new Point(x * TileSize - XOffset, y * TileSize - YOffset);
        }
    }
}