using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using SdlDotNet.Graphics;

using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace Opiso.Client {

    // A map has two kinds of layers: map layers and action layers.
    // Map layers are "physical" layers, meaning the layers that one edits in the map editor.
    // Action layers are "virtual" layers partitioning the map layers into multiple
    // vertical zones. This could for example be used to place a bridge with a tunnel under it 
    // on a map: Using action layers both the tunnel and the top of the bridge would be walkable
    // on different levels (and thus with working collision detection).
    // 
    // There are two special kinds of map layers: object layers and collision layers. Every 
    // action layer must contain exactly one collision layer (in fact action layers "boundaries"
    // are defined by their collision layers) and at most one object layer. 
    // Object layers contain the tiles that should be on the same level as the creatures
    // on screen. They are automatically drawn in front or behind of the creatures.
    // Collision layers contain special tiles that represent collision information, but are
    // not drawn on screen. The possible tiles are:
    // - walkable
    // - non-walkable
    // - non-walkable but shootable
    // - action layer transition up
    // - action layer transition down
    // - half non-walkable horizontal
    // - half non-walkable horizontal shootable
    // - half non-walkable vertical
    // - half non-walkable vertical shootable
    // - stairs
    // - stairs with action layer transition

    public class Map {
        private SurfaceCollection[] tilesets = null;
        private List<string> tilesetNames = null;
        private int actionLayerCount = -1;
        private int totalTiles = -1;

        public Map() {
            Layers = new List<MapLayer>();
        }

        public void LoadFrom(string jsonFile) {
            var map = (JsonObject)JsonConvert.Import(new StreamReader(jsonFile));
            int width = ((JsonNumber)map["width"]).ToInt32();
            int height = ((JsonNumber)map["height"]).ToInt32();
            tilesetNames = new List<string>();
            foreach (string tileset in (JsonArray)map["tilesets"]) {
                tilesetNames.Add(tileset);
            }
            foreach (JsonObject layer in (JsonArray)map["layers"]) {
                var ml = new MapLayer(width, height);
                ml.Index = ((JsonNumber)layer["no"]).ToInt32();
                string stype = (string)layer["type"];
                switch (stype) {
                case "normal": 
                    ml.Type = LayerType.Normal; 
                    break;
                case "object":
                    ml.Type = LayerType.Object;
                    break;
                case "collision":
                    ml.Type = LayerType.Collision;
                    break;
                }
                var tiles = (JsonArray)layer["tiles"];
                for (int i = 0; i < tiles.Count; i++) {
                    ml.SetTileIndex(i, ((JsonNumber)tiles[i]).ToInt16());
                }
                Layers.Add(ml);
            }
            Layers.Sort((x, y) => x.Index > y.Index ? 1 : (x.Index < y.Index ? -1 : 0));
        }

        /// <summary>
        /// Loads all tilesets used by this map into memory and assigns
        /// the correct tile ids to the tiles in them.
        /// </summary>
        public void LoadTilesets() {
            // TODO: It'd probably be a good idea to cache the 
            // tilesets globally.
            tilesets = new SurfaceCollection[tilesetNames.Count];
            totalTiles = 0;
            for (int i = 0; i < tilesets.Length; i++) {
                var coll = new SurfaceCollection();
                coll.Add(Path.Combine("tiles", tilesetNames[i]),
                         new Size(32, 32));
                tilesets[i] = coll;                
                totalTiles += coll.Count;
            }
        }

        /// <summary>
        /// Unloads all tilesets used by this map from memory.
        /// </summary>
        public void UnloadTilesets() {
            throw new NotImplementedException("Yeah.");
        }

        /// <summary>
        /// Returns the surface for the tile with the given id.
        /// Requires the tilesets to be loaded into memory.
        /// </summary>
        /// <param name="tileId">The id of the tile to return.</param>
        /// <returns>The surface.</returns>
        public Surface GetSurfaceForTileId(short tileId) {
            if (null == tilesets) {
                throw new InvalidOperationException("Tilesets not loaded.");
            }
            if (tileId < 0 || tileId >= totalTiles) {
                throw new IndexOutOfRangeException("Invalid tile id.");
            }
            int tiles = 0;
            foreach (var tileset in tilesets) {
                if (tileId <= tiles + tileset.Count) {
                    return tileset[tileId - tiles];
                }
                tiles += tileset.Count;
            }
            return null;
        }

        /// <summary>
        /// Returns the index of the action layer that the map layer
        /// with the given index is on.
        /// <summary>
        /// <param name="layerIndex">The layer's index.</param>
        /// <returns>The index of the action layer.</returns>
        public int GetActionLayerIndex(int layerIndex) {
            if (layerIndex < 0 || layerIndex >= Layers.Count) {
                throw new IndexOutOfRangeException();
            }
            // The index of the action layer is the number of collision 
            // layers up to the current layer (excluding the current layer).
            int collisionLayers = 0;
            for (int i = 0; i < layerIndex; i++) {
                if (Layers[i].Type == LayerType.Collision) {
                    collisionLayers++;
                }
            }
            return collisionLayers;
        }

        public List<MapLayer> Layers { get; private set; }
        
        public int ActionLayerCount {
            get {
                if (actionLayerCount == -1) {
                    actionLayerCount = Layers.FindAll(l => l.Type == LayerType.Collision).Count;
                }
                return actionLayerCount;
            }
        }
    }
}