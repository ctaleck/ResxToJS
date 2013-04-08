using System.Collections.Generic;

namespace ResxToJs
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Resources;

	using ResxToJs.Interfaces;

	using log4net;

	public class ResxReader : IResxReader
	{
		private readonly IApplicationState appState;

		private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public ResxReader(IApplicationState appState, ILog logger)
		{
			this.appState = appState;

			if (logger != null)
			{
				this.logger = logger;
			}
		}

		public List<ResourceFile> GetResourceFiles(string directory)
		{
			var outputResourceFiles = new List<ResourceFile>();
			if (Directory.Exists(directory))
			{
				var resourceFiles = Directory.GetFiles(directory, "*.resx");
				foreach (var filePathName in resourceFiles)
				{
					var resourceFile = new ResourceFile() { IsBaseResourceType = false, ResourceFilePathName = filePathName };

					var nameWithoutResx = filePathName.Remove(filePathName.LastIndexOf("."), 5);

					// The file which does not have the ISO culture code in it is the base resource file.
					if (nameWithoutResx.IndexOf(".") == -1)
					{
						resourceFile.IsBaseResourceType = true;
					}
					outputResourceFiles.Add(resourceFile);
				}
			}

			return outputResourceFiles;
		}

		public Dictionary<string, string> GetKeyValuePairsFromResxFile(ResourceFile resourceFile)
		{
			var resourceFileDict = new Dictionary<string, string>();
			var resourceReader = new ResXResourceReader(resourceFile.ResourceFilePathName);
			try
			{	
				foreach (DictionaryEntry d in resourceReader)
				{
					var key = d.Key as string;
					if (key != null)
					{	
						resourceFileDict.Add(key, d.Value.ToString());
					}
				}
			}
			finally
			{
				resourceReader.Close();
			}

			return resourceFileDict;
		}
	}
}
