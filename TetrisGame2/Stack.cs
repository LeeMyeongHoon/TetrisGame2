namespace TetrisGame2
{
	public class Stack
	{
		// public static
		public const int WIDTH = 10 * Shape.BLOCK_SIZE;
		public const int VALID_HEIGHT = 20 * Shape.BLOCK_SIZE;
		public const int OVER_HEIGHT = VALID_HEIGHT + (Shape.MAX_BLOCK_UP_OFFSET + 3) * Shape.BLOCK_SIZE;
	}
}
