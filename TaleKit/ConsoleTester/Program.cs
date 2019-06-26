using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKit;

namespace ConsoleTester {
	public class Program {
		private static GLoopEngine loopEngine;

		static void Main(string[] args) {
			//loopEngine = new GLoopEngine();

			//Console.WriteLine("A");

			dynamic value = 0f;
			Console.WriteLine($"{value is int}");
			Console.WriteLine($"{value is float}");
			Console.WriteLine($"{value is double}");
			Console.WriteLine($"{value is string}");
			Console.ReadLine();
		}
	}
}
