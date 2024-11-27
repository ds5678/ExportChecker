using AsmResolver.PE;
using System;

namespace ExportChecker
{
	internal class Program
	{
		static void Main(string[] args)
		{
			if(args.Length != 1)
				Console.WriteLine("This program takes exactly one argument: the path to the pe assembly");

			try
			{
				Run(args[0]);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			Console.ReadLine();
		}

		private static void Run(string path)
		{
			PEImage peImage = PEImage.FromFile(path);
			if(peImage.Exports == null)
			{
				Console.WriteLine("File has no export directory");
			}
			else if (peImage.Exports.Entries.Count == 0)
			{
				Console.WriteLine("File has 0 export methods");
			}
			else
			{
				Console.WriteLine($"File has {peImage.Exports.Entries.Count} export methods");
				Console.WriteLine();
				foreach (var symbol in peImage.Exports.Entries)
				{
					Console.WriteLine($"Ordinal: {symbol.Ordinal}");
					if (symbol.IsByName)
						Console.WriteLine($"\tName: {symbol.Name}");
					Console.WriteLine($"\tAddress: {symbol.Address.Rva.ToString("X8")}");
				}
				Console.WriteLine();
				Console.WriteLine($"File had {peImage.Exports.Entries.Count} export methods");
			}
		}
	}
}
