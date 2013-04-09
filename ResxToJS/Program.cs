using System;

namespace ResxToJs
{
	using ResxToJs.Implementation;
	using ResxToJs.Interfaces;

	class Program
	{
		static void Main(string[] args)
		{
			log4net.Config.BasicConfigurator.Configure();

			var options = new Options();
			var parseSuccess = CommandLine.Parser.Default.ParseArguments(args, options);
			if (parseSuccess)
			{
				var appState = new ApplicationState();
				var copier = new DeepCopier();
				var resxReader = new ResxReader(appState, null);
				var jsonHelper = new JsonHelper();
				var converter = new ResxToJsConverter(copier, resxReader, jsonHelper, appState, null);
				converter.Convert(options);
				DisplayErrorMessages(appState);
			}
			else
			{
				Console.WriteLine("An error occurred while parsing the resx files.");
			}

			Console.ReadLine();
		}

		static void DisplayErrorMessages(IApplicationState appState)
		{
			if (appState.ErrorMesssages != null && appState.ErrorMesssages.Count > 0)
			{
				foreach (var message in appState.ErrorMesssages)
				{
					Console.WriteLine(message);
					Console.WriteLine();
				}

				Console.WriteLine("Please review logs for further details.");
			}
		}
	}
}