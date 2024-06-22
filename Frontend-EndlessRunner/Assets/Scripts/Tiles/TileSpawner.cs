/*
 * File: TileSpawner.cs
 * Purpose: Controls behaviour of which tiles to spawn
 *          Removes previous tiles from scene so there is no overlap when new tiles spawn
 */

using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner {
    public class TileSpawner : MonoBehaviour
    {
        [Header("Tile objects")]
        [SerializeField] private GameObject straightTile;
        [SerializeField] private List<GameObject> straightTiles;
        [SerializeField] private List<GameObject> turnTiles;
        [SerializeField] private List<GameObject> obstacles;

        [Header("Tile Spawn settings")]
        [SerializeField] private int tileStartCount = 10; //Number of tiles ahead of the player at the start of a new game
        [SerializeField] private int minTilesAhead = 3; //Min length of path
        [SerializeField] public int maxTilesAhead = 10; //Max length of path

        [Header("Tile data for rotating player")]
        private Vector3 currentTileLocation = Vector3.zero;
        private Vector3 currentTileDirection = Vector3.forward;
        private GameObject prevTile;

        [Header("Instantiated Tiles")]
        private List<GameObject> currentTiles;
        private List<GameObject> currentObstacles;

        [SerializeField] private PlayerController playerController;
        public float obstacleProbability = 0.3f;

        /// <summary>
        /// Instantiates tile lists and creates a safe runway area to start the game
        /// </summary>
        void Start()
        {
            //Instantiate lists to store tiles and obstacles
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();

            Random.InitState(System.DateTime.Now.Millisecond); //Ensures level random generation is always completely random

            //Spawn tiles in the starting area
            for (int i = 0; i < tileStartCount; i++)
            {
                SpawnTile(SelectRandomInList(straightTiles).GetComponent<Tile>());
            }

            //Spawn a random turn at the end of the start area
            SpawnTile(SelectRandomInList(turnTiles).GetComponent<Tile>());
        }

        /// <summary>
        /// Update the obstacleProbability based on the player's score  <br />
        /// Shortens the max length of paths the longer you play        <br />
        /// Makes the game harder the longer you play
        /// </summary>
        void Update()
        {
            //Updates the game difficulty depending on score
            if (playerController.score > 750)
            {
                obstacleProbability = 0.8f;
                maxTilesAhead = 5;
            }
            else if (playerController.score > 500)
            {
                obstacleProbability = 0.6f;
                maxTilesAhead = 6;
            }
            else if (playerController.score > 300)
            {
                obstacleProbability = 0.45f;
                maxTilesAhead = 8;
            }
            else
            {
                obstacleProbability = 0.3f;
            }
        }

        /// <summary>
        /// Spawns a new tile with correct rotation <br />
        /// Spawns obstacle on tile if flag is set
        /// </summary>
        /// <param name="tile">Tile prefab to spawn</param>
        /// <param name="spawnObstacle">Whether or not tile needs an obstacle</param>
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

        /// <summary>
        /// Removes tiles already passed from game scene to help performance
        /// </summary>
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

        /// <summary>
        /// Returns a random index in a list
        /// </summary>
        /// <param name="list">List of tiles</param>
        /// <returns>Index of spawnable tile</returns>
        private GameObject SelectRandomInList(List <GameObject> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Used to generate tiles after turning in a direction
        /// </summary>
        /// <param name="direction"></param>
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
                SpawnTile(SelectRandomInList(straightTiles).GetComponent<Tile>(), (i == 0) ? false : true); //Make sure obstacles dont spawn immediately after turns
            }

            SpawnTile(SelectRandomInList(turnTiles).GetComponent<Tile>());
        }

        /// <summary>
        /// Spawns obstacles randomly based on spawn probability
        /// </summary>
        private void SpawnObstacle()
        {
            if (Random.value > obstacleProbability) return; //Must be above prob threshold to spawn obstacle

            //Spawn a random obstacle with correct rotation
            GameObject obstaclePrefab = SelectRandomInList(obstacles);
            Quaternion newObstacleRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObstacleRotation);
            currentObstacles.Add(obstacle);
        }
    }
}
