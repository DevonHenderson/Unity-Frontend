/*
 * File: Tile.cs
 * Purpose: Stores data about the Tile
 */

using UnityEngine;

namespace EndlessRunner {
    public enum TileType {
        STRAIGHT,
        LEFT,
        RIGHT,
        SIDEWAYS
    }

    /// <summary>
    /// Defines the attributes of a tile.
    /// </summary>
    public class Tile : MonoBehaviour
    {
        public TileType type;
        public Transform pivot;
    }
}