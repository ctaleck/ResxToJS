using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResxToJs.Interfaces
{
	public interface IApplicationState
	{
		List<string> ErrorMesssages { get; }
		void AddError(string message);
	}
}
