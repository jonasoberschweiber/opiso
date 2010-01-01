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
        }

        public short GetTileIndex(int x, int y) {
            return tileIndices[x + y * Height];
        }

        public int Index { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public LayerType Type { get; set; }
    }
}