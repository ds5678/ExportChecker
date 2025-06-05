using AsmResolver.PE;
using ELFSharp.ELF;
using ELFSharp.ELF.Sections;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ExportChecker;

internal class Program
{
	static void Main(string[] args)
	{
		if (args.Length != 1)
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
		if (TryLoadPEImage(path, out PEImage? peImage))
		{
			PrintExportsForPE(peImage);
		}
		else if (TryLoadElf(path, out IELF? elfImage))
		{
			PrintExportsForElf(elfImage);
		}
		else
		{
			Console.WriteLine($"Failed to load binary from path: {path}");
		}
	}

	private static void PrintExportsForPE(PEImage peImage)
	{
		if (peImage.Exports == null)
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

	private static void PrintExportsForElf(IELF elfImage)
	{
		var functions = ((ISymbolTable)elfImage.GetSection(".symtab")).Entries.Where(x => x.Type == SymbolType.Function);
		foreach(var f in functions)
		{
			Console.WriteLine(f.Name);
		}
	}

	private static bool TryLoadPEImage(string path, [NotNullWhen(true)] out PEImage? peImage)
	{
		try
		{
			peImage = PEImage.FromFile(path);
			return true;
		}
		catch
		{
			peImage = null;
			return false;
		}
	}

	private static bool TryLoadElf(string path, [NotNullWhen(true)] out IELF? elfImage)
	{
		try
		{
			elfImage = ELFReader.Load(path);
			return true;
		}
		catch
		{
			elfImage = null;
			return false;
		}
	}
}
