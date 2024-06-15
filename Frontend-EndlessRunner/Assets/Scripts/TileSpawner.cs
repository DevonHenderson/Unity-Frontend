using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner {
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject sraightTile;
        [SerializeField] private List<GameObject> turnTiles;
        [SerializeField] private List<GameObject> obstacles;

        [SerializeField] private int tileStartCount = 10; //Number of tiles ahead of the player at the start of a new game
        [SerializeField] private int minTilesAhead = 3;
        [SerializeField] private int maxTilesAhead = 15;

        private Vector3 currentTileLocation = Vector3.zero;
        private Vector3 currentTileDirection = Vector3.forward;
        private GameObject prevTile;

        private List<GameObject> currentTiles;
        private List<GameObject> currentObstacles;

        void Start()
        {
            //Instantiate lists to store tiles and obstacles
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond); //Ensures level random generation is always completely random

            //Spawn tiles in the starting area
            for (int i = 0; i < tileStartCount; i++)
            {
                SpawnTile(sraightTile.GetComponent<Tile>());
            }

            SpawnTile(SelectRandomInList(turnTiles).GetComponent<Tile>());
        }

        void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            //Make sure tile has correct rotation to other tiles
            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);
            currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection); //Set next tile spawn to be at end of current tile bounds
        }

        private GameObject SelectRandomInList(List <GameObject> list)
        {
            return list[Random.Range(0, list.Count)];
        }
    }
}
