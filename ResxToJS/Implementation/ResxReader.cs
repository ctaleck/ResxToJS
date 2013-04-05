using System.Collections.Generic;

namespace ResxToJs
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Resources;

	public class ResxReader : IResxReader
	{
		public List<ResourceFile> GetResourceFiles(string directory)
		{
			var outputResourceFiles = new List<ResourceFile>();
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

			return outputResourceFiles;
		}

		public Dictionary<string, string> GetKeyValuePairsFromResxFile(ResourceFile resourceFile)
		{
			var resourceFileDict = new Dictionary<string, string>();
			try
			{
				var resourceReader = new ResXResourceReader(resourceFile.ResourceFilePathName);
				foreach (DictionaryEntry d in resourceReader)
				{
					var key = d.Key as string;
					resourceFileDict.Add(key, d.Value.ToString());
				}
				resourceReader.Close();
			}
			catch (Exception ex)
			{
				
			}
			

			return resourceFileDict;
		}
	}
}
