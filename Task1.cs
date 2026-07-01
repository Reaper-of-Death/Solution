using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Task1
{
	public class Program
	{

		public static string CalculateCompoundInterestByYear(double initialDeposit, int years, double interestRate)
		{

			if (initialDeposit <= 0) 
			    throw new ArgumentException("Депозит должен быть положительным", nameof(initialDeposit));
			if (years <= 0) 
			    throw new ArgumentException("Количество лет должно быть положительным", nameof(years));
			if (interestRate < 0) 
			    throw new ArgumentException("Процентная ставка не может быть отрицательной", nameof(interestRate));

			var result = new StringBuilder();
			var total = initialDeposit;
							            
			for (var i = 0; i < years; i++)
			{
				total += total / 100 * interestRate;
				result.AppendLine($"Год {i + 1}: {total:F2} руб.");
			}

			return result.ToString();
		}

		public static void Main(string[] args)
		{
			Console.WriteLine(CalculateCompoundInterestByYear(1000, 3, 5));
		}
	}
}