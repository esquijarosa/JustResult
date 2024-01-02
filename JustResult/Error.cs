namespace JustResult;

/// <summary>
/// Represents an error in a failed <see cref="Result"/> or <see cref="Result{TValue}"/>.
/// </summary>
/// <param name="code">Error code.</param>
/// <param name="description">Error message.</param>
public class Error(string code, string description)
{
	/// <summary>
	/// Gets the error code for this <see cref="Error"/>.
	/// </summary>
	public string Code { get; } = code;

	/// <summary>
	/// Gets the error message (description) for this <see cref="Error"/>.
	/// </summary>
	public string Description { get; } = description;
}