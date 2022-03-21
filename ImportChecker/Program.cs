using AsmResolver.PE;
using AsmResolver.PE.Imports;
using System;

namespace ImportChecker
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
			IPEImage peImage = PEImage.FromFile(path);
			if(peImage.Imports == null)
			{
				Console.WriteLine("File has no import directory");
				return;
			}
			Console.WriteLine($"File has {peImage.Imports.Count} imported modules");
			Console.WriteLine();
			for (int i = 0; i < peImage.Imports.Count; i++)
			{
				Console.WriteLine($"Module {i + 1}");
				IImportedModule module = peImage.Imports[i];
				Console.WriteLine($"\tName: {module.Name}");
				Console.WriteLine($"\tForwarderChain: {module.ForwarderChain}");
				Console.WriteLine($"\tTimeDateStamp: {module.TimeDateStamp}");
				Console.WriteLine($"\tSymbols: {module.Symbols.Count}");
				for(int j = 0; j < module.Symbols.Count; j++)
				{
					ImportedSymbol symbol = module.Symbols[j];
					Console.WriteLine($"\t\tOrdinal: {symbol.Ordinal}");
					Console.WriteLine($"\t\tName: {symbol.Name}");
					if(symbol.Ordinal != symbol.Hint)
						Console.WriteLine($"\t\tHint: {symbol.Hint}");
					Console.WriteLine($"\t\tAddress: {symbol.AddressTableEntry?.Rva.ToString("X8")}");
					Console.WriteLine();
				}
			}
			Console.WriteLine();
			Console.WriteLine($"File has {peImage.Imports.Count} imported modules");
		}
	}
}
