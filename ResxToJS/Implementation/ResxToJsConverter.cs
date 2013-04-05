using System.Collections.Generic;

namespace ResxToJs
{
	using System.Collections;
	using System.IO;
	using System.Linq;
	using System.Resources;
	using System.Text;

	public class ResxToJsConverter : IResxToJsConverter
	{
		private readonly IDeepCopier objectCopier;

		private readonly IResxReader resxReader;

		private readonly IJsonHelper jsonHelper;

		public ResxToJsConverter(IDeepCopier objectCopier, IResxReader resxReader, IJsonHelper jsonHelper)
		{
			this.objectCopier = objectCopier;
			this.resxReader = resxReader;
			this.jsonHelper = jsonHelper;
		}

		public void Convert(Options options)
		{
			var resourceFiles = resxReader.GetResourceFiles(options.InputFolder);

			var baseResourceFile = resourceFiles.First(x => x.IsBaseResourceType);

			var baseResourceDict = resxReader.GetKeyValuePairsFromResxFile(baseResourceFile);
			
			foreach (var resourceFile in resourceFiles)
			{
				var jsFileNameWithoutPath = resourceFile.ResourceFilePathName.Substring(resourceFile.ResourceFilePathName.LastIndexOf("\\") + 1) + ".js";
				var outputJsFilePathName = Path.Combine(options.OutputFolder, jsFileNameWithoutPath);

				if (resourceFile.IsBaseResourceType)
				{
					WriteOutput(baseResourceDict, outputJsFilePathName);
				}
				else
				{
					var cultureSpecificResourceDict = objectCopier.Copy(baseResourceDict);
					var rsxr = new ResXResourceReader(resourceFile.ResourceFilePathName);
					foreach (DictionaryEntry d in rsxr)
					{
						var key = d.Key as string;
						cultureSpecificResourceDict[key] = d.Value.ToString();
					}
					//Close the reader.
					rsxr.Close();

					WriteOutput(cultureSpecificResourceDict, outputJsFilePathName);
				}
			}
		}

		private void WriteOutput(Dictionary<string, string> dict, string outputLocation)
		{
			var sb = new StringBuilder("Resources = {");
			foreach (var entry in dict)
			{
				sb.AppendFormat("\"{0}\":\"{1}\",", entry.Key, entry.Value);
			}

			if (sb.Length > 0)
			{
				sb = sb.Remove(sb.Length - 1, 1);
			}
			sb.Append("};");
			var prettyJson = jsonHelper.PrettyPrintJson(sb.ToString());
			System.IO.File.WriteAllText(outputLocation, prettyJson);
		}
	}
}
