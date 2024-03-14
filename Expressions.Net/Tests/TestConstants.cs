using System;

namespace Expressions.Net.Tests
{
	public static class TestConstants

	{
		public static object Variables { get; } = new
		{
			id = 1337,
			text = "hello world",
			success = true,
			traits = new string[] {
				"test",
				"awesome"
			},
			item = new
			{
				id = 420,
				name = "yolo max",
				success = true
			},
			strArrayVar = new string[] {
				"myValue1",
				"myValue2"
			},
			objArrayVar = new object[] {
				new { id = 123, text = "Hello world" }
			},
			boolArrayVar = new bool[] {
				true, false
			},
			numArrayVar = new double[] {
				1d, 2d, 3.5d
			},
			dateTimeArrayVar = new DateTime[]
			{
				new DateTime(2023, 1, 1),
				new DateTime(2023, 1, 2),
				new DateTime(2023, 1, 3)
			},
			objVar = new
			{
				id = 1337,
				name = "Hello world",
				success = true
			},
			dateVar = new DateTime(2023, 1, 1)
		};
	}
}