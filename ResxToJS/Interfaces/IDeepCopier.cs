using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResxToJs
{
	public interface IDeepCopier
	{
		T Copy<T>(T obj);
	}
}
