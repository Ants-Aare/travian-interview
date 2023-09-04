namespace _AppleShooter
{
    public static class MagicNumbers
    {
        public const int TILE = 0;
        public const int APPLE = -1;
        //AppleProjectiles are a different kind of entity for now
        public const int SHOOTAPPLE = -100;

        public const int ENEMY = -200;

        public static CardinalDirection GetShootAppleDirection(int value)
        {
            return (CardinalDirection)(value + SHOOTAPPLE);
        }

        public static int ToShootAppleIndex(CardinalDirection direction)
        {
            return (int)direction + SHOOTAPPLE;
        }

    }
}