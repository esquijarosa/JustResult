using System.Runtime.CompilerServices;

namespace JustResult;

public class Error(string code, string description)
{
	public string Code { get; } = code;

	public string Description { get; } = description;
}