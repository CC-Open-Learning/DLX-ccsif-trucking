using UnityEngine;

namespace VARLab.CCSIF
{
    public class RoadSequenceTiles : MonoBehaviour
    {
        [SerializeField] private GameObject activeTile, previousTile, nextTile;

        private const float TileLength = 57.6072f;
        private const int AmountOfTiles = 3;

        private Vector3 startingTilePosition;

        private void Start()
        {
            if (activeTile == null || previousTile == null || nextTile == null)
            {
                Debug.LogWarning("Missing road sequence tile reference from RoadSequenceTiles.cs");
            }

            startingTilePosition = activeTile.transform.position;
        }

        /// <summary>
        /// Moves the previous road tile in the sequence to ahead of the the next tile.
        /// Invoked from timelines where road sequences take place
        /// </summary>
        public void MoveTilesUp()
        {
            previousTile.transform.position += new Vector3(0, 0, (TileLength * AmountOfTiles));

            // Rotate the tile references setting the previous as the next and shuffling the rest down.
            (nextTile, activeTile, previousTile) = (previousTile, nextTile, activeTile);
        }

        public void ResetTiles() 
        {
            activeTile.transform.position = startingTilePosition;
            previousTile.transform.position = startingTilePosition;
            nextTile.transform.position = startingTilePosition;
        }
    }
}
