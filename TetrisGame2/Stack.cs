using System.Drawing;
namespace TetrisGame2
{
	public class Stack
	{
		/*******************************************************************************************************************/
		//public static
		public const int WIDTH = 10;
		public const int VALID_HEIGHT = 18;
		public const int OVER_HEIGHT = VALID_HEIGHT + (Shape.MAX_BLOCK_UP_OFFSET + 3);


		/*******************************************************************************************************************/
		//private static



		/*******************************************************************************************************************/
		//public
		public int HighestBlockY { get => highestBlockY; }

		public Stack()
		{
			data = new Brush[Stack.WIDTH, Stack.OVER_HEIGHT];
			Reset();
		}

		public void Reset()
		{
			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < OVER_HEIGHT; y++)
				{
					data[x, y] = null;
				}
			}

			highestBlockY = 0;
		}
		public void BreakRow(int keyY)
		{
			// 1줄씩 아래로 당긴다.
			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = keyY; y < highestBlockY; y++)
				{
					data[x, y] = data[x, y + 1];
				}
				data[x, highestBlockY] = null;
			}
			highestBlockY--;
		}
		

		public void PushShape(Shape shape,Brush brushOfShape)
		{
			for (int blockNum = 0; blockNum < Shape.BLOCK_COUNT; blockNum++)
			{
				var locX = shape.GetBlockLocX(blockNum);
				var locY = shape.GetBlockLocY(blockNum);

				data[locX, locY] = brushOfShape;

				if (locY > highestBlockY)
				{
					highestBlockY = locY;
				}
			}
		}

		public bool IsFull() { return highestBlockY >= VALID_HEIGHT; }
		public bool IsFullLow(int y)
		{
			for (int x = 0; x < Stack.WIDTH; x++)
			{
				if (data[x, y] == null)
				{
					return false;
				}
			}
			return true;
		}
		public int FindFullRow(int beginY, int lastY)
		{
			int y;
			for (y = beginY; y <= lastY; y++)
			{
				if (IsFullLow(y))
				{
					return y;
				}
			}
			return -1;
		}

		public Brush GetData(int x, int y) { return data[x, y]; }


		/*******************************************************************************************************************/
		//private
		private int highestBlockY;
		private Brush[,] data;       // 블럭이 없으면 null, 있으면 Brush 저장
	}
}
