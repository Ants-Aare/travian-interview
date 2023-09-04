using System.Collections.Generic;
using System.Security.Cryptography;
using AAA.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using static _AppleShooter.MagicNumbers;

namespace _AppleShooter
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private GameObject snakePrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject applePrefab;
        [SerializeField] private GameObject gridCell;
        private readonly List<GameObject> _objects = new();

        public void UpdateGrid(int[,] grid)
        {
            //TODO: object pool
            foreach (var obj in _objects)
            {
                Destroy(obj);
            }

            var width = grid.GetLength(0);
            var height = grid.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var position = new Vector2Int(x,y);
                    switch (grid[x, y])
                    {
                        case > 0:
                            CreateSnake(grid[x, y], position);
                            break;
                        case APPLE:
                            CreateGridCell(applePrefab, position);
                            CreateGridCell(gridCell, position);
                            break;
                        case ENEMY:
                            CreateGridCell(enemyPrefab, position);
                            CreateGridCell(gridCell, position);
                            break;
                        case <= SHOOTAPPLE + 3  and >= SHOOTAPPLE:
                            CreateGridCell(applePrefab, position);
                            CreateGridCell(gridCell, position);
                            break;
                        default:
                            CreateGridCell(gridCell, position);
                            break;
                    }
                }
            }
        }
        
        private void CreateGridCell(GameObject prefab, Vector2Int position)
        {
            var instance = GameObject.Instantiate(prefab, position.ToVector3XZ(), Quaternion.identity);
            _objects.Add(instance);
        }

        private void CreateSnake(int i, Vector2Int position)
        {
            var instance = Instantiate(snakePrefab, position.ToVector3XZ(), Quaternion.identity);
            
            //TODO: figure out head and tail etc based on int
            _objects.Add(instance);
        }
    }
}