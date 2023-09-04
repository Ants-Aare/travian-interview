using UnityEngine;

namespace _AppleShooter
{
    public static class CardinalDirectionExtensions
    {
        public static CardinalDirection ToCardinalDirection(this Vector2Int value)
        {
            if (value == Vector2Int.up) return CardinalDirection.Up;
            if (value == Vector2Int.down) return CardinalDirection.Down;

            if (value == Vector2Int.right) return CardinalDirection.Right;
            if (value == Vector2Int.left) return CardinalDirection.Left;

            return CardinalDirection.Up;
        }

        public static Quaternion ToQuaternion(this CardinalDirection direction)
            => Quaternion.Euler(direction.ToEuler());
        public static Quaternion ToQuaternionZ(this CardinalDirection direction)
            => Quaternion.Euler(direction.ToEulerZ());

        public static Vector3 ToEuler(this CardinalDirection direction)
            => new(0, (int)direction * 90f, 0);
        public static Vector3 ToEulerZ(this CardinalDirection direction)
            => new(0, 0, (int)direction * 90f);

        public static CardinalDirection Rotate(this CardinalDirection direction, int iterations)
        {
            var index = (int)direction;
            index = (index + iterations) % 4;
            return (CardinalDirection)index;
        }
        public static Vector3Int Rotate(this Vector3Int value, CardinalDirection direction)
            => direction switch
            {
                CardinalDirection.Up => value,
                CardinalDirection.Right => new Vector3Int(value.z, value.y, -value.x),
                CardinalDirection.Down => new Vector3Int(-value.x, value.y, -value.z),
                CardinalDirection.Left => new Vector3Int(-value.z, value.y, value.x),
                _ => value
            };
        public static Vector3 Rotate(this Vector3 value, CardinalDirection direction)
            => direction switch
            {
                CardinalDirection.Up => value,
                CardinalDirection.Right => new Vector3(value.z, value.y, -value.x),
                CardinalDirection.Down => new Vector3(-value.x, value.y, -value.z),
                CardinalDirection.Left => new Vector3(-value.z, value.y, value.x),
                _ => value
            };

        public static CardinalDirection Flip(this CardinalDirection direction) => direction.Rotate(2);

        public static CardinalDirection FlipX(this CardinalDirection direction)
            => direction is CardinalDirection.Left or CardinalDirection.Right
                ? direction.Rotate(2)
                : direction;
        public static CardinalDirection FlipY(this CardinalDirection direction)
            => direction is CardinalDirection.Left or CardinalDirection.Right
                ? direction.Rotate(2)
                : direction;

        public static Vector2Int ToVector2Int(this CardinalDirection value)
        {
            switch (value)
            {
                case CardinalDirection.Up: return Vector2Int.up;
                case CardinalDirection.Right: return Vector2Int.right;
                case CardinalDirection.Down: return Vector2Int.down;
                case CardinalDirection.Left: return Vector2Int.left;
                default: return Vector2Int.up;
            }
        }
    }
}
