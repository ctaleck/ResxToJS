using System.Text;

namespace ResxToJs
{
	public class JsonHelper : IJsonHelper
	{
		private const string IndentString = "    ";

		public string PrettyPrintJson(string jsonString)
		{
			var quoted = false;
			var sb = new StringBuilder();
			for (var i = 0; i < jsonString.Length; i++)
			{
				var ch = jsonString[i];
				sb.Append(ch);
				switch (ch)
				{
					case '{':
						sb.Append("\n");
						break;

					case '"':
						var escaped = false;
						var index = i;
						while (index > 0 && jsonString[--index] == '\\')
							escaped = !escaped;
						if (!escaped)
							quoted = !quoted;
						break;

					case ':':
						sb.Append(" ");
						break;

					case ',':
						if (!quoted)
						{
							sb.Append("\n");
						}
						break;
				}
			}

			return sb.ToString();
		}
	}
}
