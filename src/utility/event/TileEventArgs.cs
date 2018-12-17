namespace BesmashContent {
    using System;
    using Microsoft.Xna.Framework;

    public class TileEventArgs : EventArgs {
        public TileMap Map {get; private set;}
        public Point Coords {get; private set;}
        public Movable Movable {get; private set;}

        public TileEventArgs(Movable movable, TileMap map, Point coords) {
            Map = map;
            Coords = coords;
            Movable = movable;
        }
    }

    public delegate void TileSteppedHandler(Tile sender, TileEventArgs args);
    public delegate void TileTriggeredHandler(Tile sender, TileEventArgs args);
}