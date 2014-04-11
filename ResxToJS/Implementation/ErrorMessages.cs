﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResxToJs.Implementation
{
	public static class ErrorMessages
	{
		public const string BaseResourceFileNotFound = "Base resource file (without ISO culture string) not found in the specified folder.";

		public const string ResourceFilesNotFound = "Resource files not found in the specified folder";

		public static string DuplicateValueInResX = "The key \"{0}\" and key \"{1}\" have the same value \"{2}\" in \"{3}\" ";
	}
}
