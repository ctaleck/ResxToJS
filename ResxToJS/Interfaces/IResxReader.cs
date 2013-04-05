using System.Collections.Generic;

namespace ResxToJs
{
	public interface IResxReader
	{
		List<ResourceFile> GetResourceFiles(string directory);

		Dictionary<string, string> GetKeyValuePairsFromResxFile(ResourceFile resourceFile);
	}
}
