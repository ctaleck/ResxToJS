using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace ResxToJs
{
	public class Options 
	{
		[Option('i', "inputFolder", Required = true, HelpText = "Input folder that contains .resx files.")]
		public string InputFolder { get; set; }

		[Option('o', "outputFolder", Required = true, HelpText = "Output folder for the generated .json files.")]
		public string OutputFolder { get; set; }

		[Option("jsFileName", Required = false, HelpText = "Name of the JavaScript file. By default, uses the same name as the .resx file.")]
		public string JavaScriptFileName { get; set; }

		[Option("jsObjectName", Required = false, HelpText = "The Object name for the generated JavaScript file")]
		public string JsResourceObjectName { get; set; }

		[Option("prettyPrint", Required = false, DefaultValue = false, HelpText = "Pretty print the generated JSON")]
		public bool PrettyPrint { get; set; }

		[Option("checkDuplicates", Required = false, DefaultValue = false, HelpText = "Check for duplicate values in the resx files before generation")]
		public bool CheckDuplicatesInResx { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}
