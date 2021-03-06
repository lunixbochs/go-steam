using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SteamLanguageParser;

namespace GoSteamLanguageGenerator
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length < 2) {
				Console.WriteLine("Must have at least two parameters: SteamKit root path and output path!");
				return;
			}

			string steamKitPath = Path.GetFullPath(args[0]);
			string languagePath = Path.Combine(steamKitPath, "Resources", "SteamLanguage");
			string outputPath = Path.GetFullPath(args[1]);

			Environment.CurrentDirectory = languagePath;

			var codeGen = new GoGen(args.Length > 2 && args[2] == "debug");

			Queue<Token> tokenList = LanguageParser.TokenizeString(File.ReadAllText("steammsg.steamd"));

			Node root = TokenAnalyzer.Analyze(tokenList);

			Node rootEnumNode = new Node();
			Node rootMessageNode = new Node();

			rootEnumNode.childNodes.AddRange(root.childNodes.Where(n => n is EnumNode));
			rootMessageNode.childNodes.AddRange(root.childNodes.Where(n => n is ClassNode));

			StringBuilder enumBuilder = new StringBuilder();
			StringBuilder messageBuilder = new StringBuilder();

			codeGen.EmitEnums(rootEnumNode, enumBuilder);
			codeGen.EmitClasses(rootMessageNode, messageBuilder);

			string outputEnumFile = Path.Combine(outputPath, "steam_language_enums.go");
			string outputMessageFile = Path.Combine(outputPath, "steam_language_internal.go");

			File.WriteAllText(Path.Combine(steamKitPath, outputEnumFile), enumBuilder.ToString());
			File.WriteAllText(Path.Combine(steamKitPath, outputMessageFile), messageBuilder.ToString());
		}
	}
}
