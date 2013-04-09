using System.Collections.Generic;

namespace ResxToJs
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Resources;
	using System.Text;

	using ResxToJs.Implementation;
	using ResxToJs.Interfaces;

	using log4net;

	public class ResxToJsConverter : IResxToJsConverter
	{
		private readonly IDeepCopier objectCopier;

		private readonly IResxReader resxReader;

		private readonly IJsonHelper jsonHelper;

		private readonly IApplicationState appState;

		private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public ResxToJsConverter(IDeepCopier objectCopier, IResxReader resxReader, IJsonHelper jsonHelper, IApplicationState appState, ILog logger)
		{
			this.objectCopier = objectCopier;
			this.resxReader = resxReader;
			this.jsonHelper = jsonHelper;
			this.appState = appState;
			// for unit testing, pass in your mocked log4net logger
			if (logger != null) this.logger = logger;
		}

		public void Convert(Options options)
		{
			try
			{
				var resourceFiles = this.GetResourceFiles(options);

				var baseResourceFile = this.GetBaseResourceFile(resourceFiles);

				this.GenerateJsResourceFiles(options, resourceFiles, baseResourceFile);
			}
			catch (Exception ex)
			{
				logger.Error("Errors occurred during conversion", ex);
			}
		}

		private void GenerateJsResourceFiles(Options options, IEnumerable<ResourceFile> resourceFiles, ResourceFile baseResourceFile)
		{
			var baseResourceDict = resxReader.GetKeyValuePairsFromResxFile(baseResourceFile);

			foreach (var resourceFile in resourceFiles)
			{
				var outputJsFilePathName = GetJsOutputFileName(options, resourceFile);

				if (resourceFile.IsBaseResourceType)
				{
					this.WriteOutput(options, baseResourceDict, outputJsFilePathName);
				}
				else
				{
					var cultureSpecificResourceDict = this.GetCultureSpecificResourceDictFromBaseDict(baseResourceDict, resourceFile);

					this.WriteOutput(options, cultureSpecificResourceDict, outputJsFilePathName);
				}
			}
		}

		private static string GetJsOutputFileName(Options options, ResourceFile resourceFile)
		{
			var jsFileName = string.IsNullOrEmpty(options.JavaScriptFileName) 
							? resourceFile.ResourceFilePathName.Substring(resourceFile.ResourceFilePathName.LastIndexOf("\\") + 1) 
							: options.JavaScriptFileName;
			var jsFileNameWithoutPath = jsFileName + ".js";
			var outputJsFilePathName = Path.Combine(options.OutputFolder, jsFileNameWithoutPath);
			return outputJsFilePathName;
		}

		private Dictionary<string, string> GetCultureSpecificResourceDictFromBaseDict(Dictionary<string, string> baseResourceDict, ResourceFile resourceFile)
		{
			var cultureSpecificResourceDict = this.objectCopier.Copy(baseResourceDict);
			var rsxr = new ResXResourceReader(resourceFile.ResourceFilePathName);
			try
			{
				foreach (DictionaryEntry d in rsxr)
				{
					var key = d.Key as string;
					if (key != null)
					{
						cultureSpecificResourceDict[key] = d.Value.ToString();
					}
				}
			}
			finally
			{
				rsxr.Close();
			}
			
			return cultureSpecificResourceDict;
		}

		private ResourceFile GetBaseResourceFile(IEnumerable<ResourceFile> resourceFiles)
		{
			var baseResourceFile = resourceFiles.Where(x => x.IsBaseResourceType).ToList();

			if (!baseResourceFile.Any())
			{
				this.appState.AddError(ErrorMessages.BaseResourceFileNotFound);
				throw new ApplicationException(ErrorMessages.BaseResourceFileNotFound);
			}
			return baseResourceFile.ElementAt(0);
		}

		private List<ResourceFile> GetResourceFiles(Options options)
		{
			var resourceFiles = this.resxReader.GetResourceFiles(options.InputFolder);

			if (resourceFiles.Count == 0)
			{
				this.appState.AddError(ErrorMessages.ResourceFilesNotFound);
				throw new ApplicationException(ErrorMessages.ResourceFilesNotFound);
			}
			return resourceFiles;
		}


		private void WriteOutput(Options options, Dictionary<string, string> resourceDict, string outputLocation)
		{
			var valueDict = new Dictionary<string, string>();
			var resourceObjectName = string.IsNullOrEmpty(options.JsResourceObjectName) ? "Resources" : options.JsResourceObjectName.Trim();
			var sb = new StringBuilder(resourceObjectName + " = {");

			foreach (var entry in resourceDict)
			{
				this.CheckDuplicateValues(options, valueDict, entry, outputLocation);

				var escapedValue = EscapeJsonValue(entry.Value.Trim());

				sb.AppendFormat("\"{0}\":\"{1}\",", entry.Key, escapedValue);
			}

			if (sb.Length > 0)
			{
				sb = sb.Remove(sb.Length - 1, 1);
			}
			sb.Append("};");

			var outputJson = sb.ToString();
			if (options.PrettyPrint)
			{
				outputJson = jsonHelper.PrettyPrintJson(outputJson);
			}

			File.WriteAllText(outputLocation, outputJson);
		}

		private void CheckDuplicateValues(Options options, Dictionary<string, string> valueDict, KeyValuePair<string, string> currentKeyValue, string outputLocation)
		{
			if (options.CheckDuplicatesInResx)
			{
				if (valueDict.ContainsKey(currentKeyValue.Value))
				{
					var message = string.Format(ErrorMessages.DuplicateValueInResX, currentKeyValue.Key, valueDict[currentKeyValue.Value],
											currentKeyValue.Value, outputLocation);
					this.appState.ErrorMesssages.Add(message);
				}
				else
				{
					valueDict.Add(currentKeyValue.Value, currentKeyValue.Key);
				}
			}
		}

		private static string EscapeJsonValue(string value)
		{
			return value.Replace("\"", "\\\"");
		}
	}
}
