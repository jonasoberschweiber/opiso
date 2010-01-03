using System;

using SdlDotNet.Graphics;

namespace Opiso.Client {
    public class MapDrawer {
        public delegate void DrawObjectsDelegate(Surface surface, int actionLayerIndex, ScreenSection section);

        public void Draw(Surface surface, ScreenSection section) {
            for (int i = 0; i < Map.Layers.Count; i++) {
                var layer = Map.Layers[i];
                if (layer.Type == LayerType.Collision) {
                    continue;
                } else if (layer.Type == LayerType.Object) {
                    DrawObjectLayer(surface, i, section);
                } else {
                    DrawNormalLayer(surface, i, section);
                }
            }
        }

        private void DrawObjectLayer(Surface surface, int layerIndex, ScreenSection section) {
            // Draw the layer line by line, each time asking our delegate to draw its
            // objects beforehand.
            int actionLayerIndex = Map.GetActionLayerIndex(layerIndex);
            var newSection = new ScreenSection();
            newSection.TileLeft = section.TileLeft;
            newSection.TileWidth = section.TileWidth;
            newSection.TileSize = section.TileSize;
            newSection.XOffset = section.XOffset;
            newSection.YOffset = section.YOffset;
            newSection.TileHeight = 1;
            for (int i = section.TileTop; i < section.TileTop + section.TileHeight; i++) {
                newSection.TileTop = i;
                DrawObjects(surface, actionLayerIndex, newSection);
                DrawNormalLayer(surface, layerIndex, newSection);
            }
        }

        private void DrawNormalLayer(Surface surface, int layerIndex, ScreenSection section) {
            // Draw each line from left to right.
            var layer = Map.Layers[layerIndex];
            for (var y = section.TileTop; y < section.TileTop + section.TileHeight; y++) {
                for (var x = section.TileLeft; x < section.TileLeft + section.TileWidth; x++) {
                    var tileIndex = layer.GetTileIndex(x, y);
                    var tile = Map.GetSurfaceForTileId(tileIndex);
                    var position = section.GetTilePosition(x, y);
                    surface.Blit(tile, position);
                }
            }
        }

        public Map Map { get; set; }
        public DrawObjectsDelegate DrawObjects { get; set; }
    }
}