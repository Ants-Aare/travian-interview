using System.Collections;
using AAA.Extensions;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static _AppleShooter.MagicNumbers;

namespace _AppleShooter
{
    //TODO: Input queue for better game feel
    public class GridController : MonoBehaviour
    {
        [SerializeField] Vector2Int gridSize;
        [SerializeField] int snakeStartLength = 3;
        [SerializeField] private int appleConsumeLength;
        [SerializeField] private float snakeSpeed = 1;
        [SerializeField] private float shootAppleSpeed = 0.5f;
        [SerializeField] private float enemySpawnSpeed = 5;
        [SerializeField] private GridView gridView;
        [SerializeField] private UnityEvent onGameEnded;
        [SerializeField] private bool isGameEnd = false;

        private Vector2Int _snakeHeadPosition;
        private int[,] _grid;
        private int _snakeLength;
        private Vector2Int _currentMoveDirection = Vector2Int.up;
        private bool _requestedShoot = false;

        private float _snakeMoveTimer;

        private WaitForSeconds _enemyWaitFor;
        private Coroutine _enemyCoroutine;
        private float _shootAppleTimer;

        void Start()
        {
            _grid = new int[gridSize.x, gridSize.y];
            CreateSnake(gridSize / 2, snakeStartLength);
            SpawnAppleAtRandomPosition();
            // _enemyCoroutine = StartCoroutine(StartEnemySpawning());
        }

        private void SpawnAppleAtRandomPosition()
        {
            var position = GetRandomFreePosition();

            _grid[position.x, position.y] = APPLE;
        }

        private void SpawnEnemyAtRandomPosition()
        {
            var position = GetRandomFreePosition();

            _grid[position.x, position.y] = ENEMY;
        }

        void FixedUpdate()
        {
            if (isGameEnd)
                return;

            GetInput();

            _snakeMoveTimer += Time.fixedDeltaTime;
            _shootAppleTimer += Time.fixedDeltaTime;

            if (_shootAppleTimer >= shootAppleSpeed)
            {
                _shootAppleTimer = 0;
                UpdateShootApples();
            }

            if (_snakeMoveTimer >= snakeSpeed)
            {
                _snakeMoveTimer = 0;

                UpdateSnakeTileHealth();
                if (!TryMoveSnake(_currentMoveDirection))
                    EndGame();
            }

            if (_requestedShoot)
            {
                CreateShootApple(_currentMoveDirection);
            }

            gridView.UpdateGrid(_grid);
        }

        private void UpdateShootApples()
        {
            var width = _grid.GetLength(0);
            var height = _grid.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    // if it's a shootapple index including offsets
                    var index = _grid[x, y];
                    if (index <= SHOOTAPPLE + 3 && index >= SHOOTAPPLE)
                    {
                        _grid[x, y] = TILE;
                        var position = new Vector2Int(x, y);
                        var moveDirection = GetShootAppleDirection(index).ToVector2Int();

                        var targetPosition = position + moveDirection;

                        if (_grid[targetPosition.x, targetPosition.y] == ENEMY)
                        {
                            return;
                        }

                        if (targetPosition.x < 0
                            || targetPosition.y < 0
                            || targetPosition.x >= _grid.GetLength(0)
                            || targetPosition.y >= _grid.GetLength(1))
                        {
                            return;
                        }

                        _grid[targetPosition.x, targetPosition.y] = index;
                    }
                }
            }
        }

        private void CreateShootApple(Vector2Int currentMoveDirection)
        {
            var applePosition = _snakeHeadPosition + currentMoveDirection;
            _grid[applePosition.x, applePosition.y] = ToShootAppleIndex(currentMoveDirection.ToCardinalDirection());
        }

        private void EndGame()
        {
            onGameEnded?.Invoke();
            Debug.Log("Game Ended");
            isGameEnd = true;
            if (_enemyCoroutine != null)
                StopCoroutine(_enemyCoroutine);
        }


        private void GetInput()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                UpdateMoveDirection(new Vector2Int(0, 1));
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                UpdateMoveDirection(new Vector2Int(-1, 0));
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                UpdateMoveDirection(new Vector2Int(0, -1));
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                UpdateMoveDirection(new Vector2Int(1, 0));

            if (Input.GetKeyDown(KeyCode.Space))
                _requestedShoot = true;
        }

        private void UpdateMoveDirection(Vector2Int newDirection)
        {
            _currentMoveDirection = newDirection;
        }

        private IEnumerator StartEnemySpawning()
        {
            _enemyWaitFor ??= new WaitForSeconds(enemySpawnSpeed);
            while (true)
            {
                yield return _enemyWaitFor;
                SpawnEnemyAtRandomPosition();
            }
        }

        private void CreateSnake(Vector2Int startPosition, int startSnakeLenght)
        {
            _snakeHeadPosition = startPosition;
            _snakeLength = startSnakeLenght;

            for (var i = 0; i < startSnakeLenght; i++)
            {
                _grid[startPosition.x, startPosition.y - i] = startSnakeLenght - i;
            }
        }

        // please only set to cardinal directions
        public bool TryMoveSnake(Vector2Int direction)
        {
            var targetPosition = _snakeHeadPosition + direction;

            if (targetPosition.x < 0 || targetPosition.y < 0)
            {
                return false;
            }

            if (targetPosition.x >= _grid.GetLength(0) || targetPosition.y >= _grid.GetLength(1))
            {
                return false;
            }

            var index = _grid[targetPosition.x, targetPosition.y];
            switch (index)
            {
                case > 0:
                    return false;
                case ENEMY:
                    return false;
                case APPLE:
                    _grid[targetPosition.x, targetPosition.y] = _snakeLength;
                    _snakeHeadPosition = targetPosition;
                    CollectApple();
                    return true;
                default:
                    _grid[targetPosition.x, targetPosition.y] = _snakeLength;
                    _snakeHeadPosition = targetPosition;
                    return true;
            }
        }

        private void CollectApple()
        {
            _snakeLength += appleConsumeLength;

            SpawnAppleAtRandomPosition();
            UpdateSnakeTileHealth(appleConsumeLength);
            //TODO: there may be a bug with the tail only showing up after old tail is regenerated
            //TODO: so add apple consumelength to all existing snake tiles or something like that 
        }

        private void UpdateSnakeTileHealth(int damage = -1)
        {
            var width = _grid.GetLength(0);
            var height = _grid.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (_grid[x, y] > 0)
                        _grid[x, y] += damage;
                }
            }
        }

        public Vector2Int GetRandomFreePosition()
        {
            for (var i = 0; i < 10000; i++)
            {
                var randomPosition = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.x));

                if (_grid[randomPosition.x, randomPosition.y] == 0)
                    return randomPosition;
            }

            Debug.LogError("Could not find free position");
            return Vector2Int.zero;
        }
    }
}