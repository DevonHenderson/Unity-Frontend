using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EndlessRunner {
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject straightTile;
        [SerializeField] private List<GameObject> turnTiles;
        [SerializeField] private List<GameObject> obstacles;

        [SerializeField] private int tileStartCount = 10; //Number of tiles ahead of the player at the start of a new game
        [SerializeField] private int minTilesAhead = 3;
        [SerializeField] private int maxTilesAhead = 10;

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
                SpawnTile(straightTile.GetComponent<Tile>());
            }

            SpawnTile(SelectRandomInList(turnTiles).GetComponent<Tile>());
        }

        void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            //Make sure tile has correct rotation to other tiles
            Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);

            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            if (spawnObstacle)
            {
                SpawnObstacle();
            }

            //Make sure tiles in new direction line up with path
            if (tile.type == TileType.STRAIGHT)
            {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection); //Set next tile spawn to be at end of current tile bounds
            }
        }

        //Removes tiles already passed from game scene to help performance
        void DeleteTiles()
        {
            // Using 1 so turnTile doesnt get deleted
            while (currentTiles.Count != 1) //Delete previous path
            {
                GameObject tile = currentTiles[0];
                currentTiles.RemoveAt(0);
                Destroy(tile);
            }

            while (currentObstacles.Count != 0) //Delete previous obstacles
            {
                GameObject obstacle = currentObstacles[0];
                currentObstacles.RemoveAt(0);
                Destroy(obstacle);
            }
        }

        private GameObject SelectRandomInList(List <GameObject> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        //Used to generate tiles after turning in a direction
        public void AddNewDirection(Vector3 direction)
        {
            currentTileDirection = direction;
            DeleteTiles();

            Vector3 tileScale;

            //Calculates any new spawning offset for specific turn tile types
            if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
            {
                tileScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size / 2 + (Vector3.one * straightTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }
            else //Left and right tiles
            {
                tileScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * straightTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }

            currentTileLocation += tileScale;

            //Number of straight tiles before another turn
            int currentPathLength = Random.Range(minTilesAhead, maxTilesAhead);

            for (int i = 0; i < currentPathLength; i++)
            {
                SpawnTile(straightTile.GetComponent<Tile>(), (i == 0) ? false : true); //Make sure obstacles dont spawn immediately after turns
            }

            SpawnTile(SelectRandomInList(turnTiles).GetComponent<Tile>());
        }

        private void SpawnObstacle()
        {
            if (Random.value > 0.4f) return; //Only have a 40% chance to spawn an obstacle

            //Spawn a random obstacle with correct rotation
            GameObject obstaclePrefab = SelectRandomInList(obstacles);
            Quaternion newObstacleRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObstacleRotation);
            currentObstacles.Add(obstacle);
        }
    }
}
