using System;
using System.Collections.Generics;

using SdlDotNet.Graphics;

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
        private Surface[] tiles = null;
        private int actionLayerCount = -1;

        public Map() {
            Layers = new List<MapLayer>();
        }        

        /// <summary>
        /// Loads all tilesets used by this map into memory and assigns
        /// the correct tile ids to the tiles in them.
        /// </summary>
        public void LoadTilesets() {
            throw new NotImplementedException("Yeah.");
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
            if (null == tiles) {
                throw new InvalidOperationException("Tilesets not loaded.");
            }
            if (tileId < 0 || tileId > short.MAX) {
                throw new ArgumentException("Invalid tile id.");
            }
            return tiles[tileId];
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