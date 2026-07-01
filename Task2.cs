using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task2
{
	public class Program
	{

		public static void DrawDiamondOnScreen(int diagonal)
        {
            if (diagonal <= 0 || diagonal % 2 == 0)
            {
                Console.WriteLine("Диагональ должна быть положительным нечётным целым числом");
                return;
            }

            int mid = diagonal / 2;

            for (int row = 0; row < diagonal; row++)
            {
                for (int col = 0; col < diagonal; col++)
                {
                    int rowDist = Math.Abs(row - mid);
                    int colDist = Math.Abs(col - mid);

                    bool isOnBorder = (rowDist + colDist == mid);

                    bool isInsideDiamond = (rowDist + colDist <= mid);

                    if (isOnBorder && isInsideDiamond)
                    {
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }

		public static void Main(string[] args)
		{
			DrawDiamondOnScreen(-9);
		}
	}
}