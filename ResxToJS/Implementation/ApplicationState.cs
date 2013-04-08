using System.Collections.Generic;

namespace ResxToJs.Implementation
{
	using ResxToJs.Interfaces;

	public class ApplicationState : IApplicationState
	{
		private static readonly List<string> ErrorMessages = new List<string>();

		public List<string> ErrorMesssages
		{
			get
			{
				return ErrorMessages;
			}
		}

		public void AddError(string message)
		{
			ErrorMessages.Add(message);
		}


	}
}
