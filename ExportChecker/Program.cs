using AsmResolver.PE;
using ELFSharp.ELF;
using ELFSharp.ELF.Sections;
using ELFSharp.MachO;
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
		else if (TryLoadMacho(path, out MachO? machoImage))
		{
			PrintExportsForMacho(machoImage);
		}
		else
		{
			Console.WriteLine($"Failed to load binary from path: {path}");
		}
	}

	private static void PrintExportsForPE(PEImage image)
	{
		if (image.Exports == null)
		{
			Console.WriteLine("File has no export directory");
		}
		else if (image.Exports.Entries.Count == 0)
		{
			Console.WriteLine("File has 0 export methods");
		}
		else
		{
			Console.WriteLine($"File has {image.Exports.Entries.Count} export methods");
			Console.WriteLine();
			foreach (var symbol in image.Exports.Entries)
			{
				Console.WriteLine($"Ordinal: {symbol.Ordinal}");
				if (symbol.IsByName)
					Console.WriteLine($"\tName: {symbol.Name}");
				Console.WriteLine($"\tAddress: {symbol.Address.Rva.ToString("X8")}");
			}
			Console.WriteLine();
			Console.WriteLine($"File had {image.Exports.Entries.Count} export methods");
		}
	}

	private static void PrintExportsForElf(IELF image)
	{
		var functions = ((ISymbolTable)image.GetSection(".symtab")).Entries.Where(x => x.Type == SymbolType.Function);
		foreach(var f in functions)
		{
			Console.WriteLine(f.Name);
		}
	}

	private static void PrintExportsForMacho(MachO image)
	{
		var exports = image
			.GetCommandsOfType<SymbolTable>()
			.SelectMany(t => t.Symbols)
			.Select(s => s.Name)
			.Where(n => !string.IsNullOrEmpty(n))
			.Distinct()
			.Order();

		foreach (var export in exports)
		{
			Console.WriteLine(export);
		}
	}

	private static bool TryLoadPEImage(string path, [NotNullWhen(true)] out PEImage? image)
	{
		try
		{
			image = PEImage.FromFile(path);
			return true;
		}
		catch
		{
			image = null;
			return false;
		}
	}

	private static bool TryLoadElf(string path, [NotNullWhen(true)] out IELF? image)
	{
		try
		{
			image = ELFReader.Load(path);
			return true;
		}
		catch
		{
			image = null;
			return false;
		}
	}

	private static bool TryLoadMacho(string path, [NotNullWhen(true)] out MachO? image)
	{
		try
		{
			image = MachOReader.Load(path);
			return true;
		}
		catch
		{
			image = null;
			return false;
		}
	}
}
