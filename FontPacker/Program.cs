using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Common;
using Newtonsoft.Json;

namespace FontPacker
{
	internal class Program
	{
		
		public static void Main(string[] args)
		{
			Console.InputEncoding = Encoding.Unicode;
			Console.OutputEncoding = Encoding.Unicode;

			if (args.Length < 2)
			{
				var programName = Assembly.GetExecutingAssembly().GetName().Name;
				Console.WriteLine($"Invalid args. Example usage: {programName} <path-to-font-folder> <pack-name>");
				return;
			}

			var path = args[0];
			var packName = args[1];

			if(!ValidatePath(path))
			{
				return;
			}

			var charsMap = MapCharsToFiles(path);
			var fontPack = CreateFontPack(packName, charsMap);

			SavePack(fontPack);

			Console.WriteLine($"Saved font pack to: {fontPack.Name}{FileExtension.FontPackExtension}");
		}

		private static bool ValidatePath(string path)
		{
			if (!Directory.Exists(path))
			{
				Console.WriteLine("Given path doesn't exist");
				return false;
			}

			// get the file attributes for file or directory
			var attr = File.GetAttributes(path);

			//detect whether its a directory or file
			if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
			{
				Console.WriteLine("Given path is not directory");
				return false;
			}

			return true;
		}

		private static Dictionary<char, string> MapCharsToFiles(string path)
		{
			Console.WriteLine("Map each file to character:");
			var files = Directory.GetFiles(path);
			var charsMap = new Dictionary<char, string>();
			foreach (var file in files.Where(x => Path.GetExtension(x) == FileExtension.CharExtension))
			{
				char? character = null;
				while (character == null)
				{
					Console.Write($" - {Path.GetFileName(file)}: ");
					var line = Console.ReadLine();

					if (line == null || line.Length != 1 || charsMap.ContainsKey(line[0])) continue;

					character = line[0];
					charsMap.Add(character.Value, file);
				}
			}

			return charsMap;
		}

		private static FontPackModel CreateFontPack(string name, Dictionary<char, string> charsFileMap)
		{
			var fallbackCharacterFile = $"fallback{FileExtension.CharExtension}";
			FontModel fallbackCharacter;

			using (var sr = new StreamReader(fallbackCharacterFile))
			{
				var json = sr.ReadToEnd();
				fallbackCharacter = JsonConvert.DeserializeObject<FontModel>(json);
			}

			var fontPack = new FontPackModel
			{
				Name = name,
				FallbackCharacter = fallbackCharacter,
				CharMap = new Dictionary<char, FontModel>()
			};

			foreach (var (chr, path) in charsFileMap.Select(x => (x.Key, x.Value)))
			{
				var json = File.ReadAllText(path);

				var character = JsonConvert.DeserializeObject<FontModel>(json);

				fontPack.CharMap.Add(chr, character);
			}

			fontPack.Name = name;
			return fontPack;
		}

		private static void SavePack(FontPackModel pack)
		{
			var json = JsonConvert.SerializeObject(pack);
			using var sw = new StreamWriter($"{pack.Name}{FileExtension.FontPackExtension}");
			sw.WriteLine(json);
		}
	}
}
