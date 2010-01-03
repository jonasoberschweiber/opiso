using System;

namespace Opiso.Client {
    public enum LayerType {
        Normal,
        Object,
        Collision
    }

    public class MapLayer {
        private short[] tileIndices;

        public MapLayer(int width, int height) {
            tileIndices = new short[width * height];
            Width = width;
            Height = height;
        }

        public short GetTileIndex(int x, int y) {
            return tileIndices[x + y * Height];
        }

        public void SetTileIndex(int x, int y, short tileId) {
            tileIndices[x + y * Height] = tileId;
        }
        
        public void SetTileIndex(int idx, short tileId) {
            tileIndices[idx] = tileId;
        }

        public int Index { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public LayerType Type { get; set; }
    }
}