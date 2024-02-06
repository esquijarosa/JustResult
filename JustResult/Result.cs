using System.Diagnostics.Contracts;

namespace JustResult;

/// <summary>
/// Represents a result from a method call.
/// </summary>
/// <remarks>
/// Use this implementation instead of using exceptions to control the flow of execution.
/// </remarks>
public readonly record struct Result
{
	private readonly List<Error>? _errors = null;

	private Result(Error error)
	{
		IsError = true;
		_errors = [error];
	}

	private Result(List<Error> errors)
	{
		IsError = true;
		_errors = new(errors);
	}

	private Result(Exception exception)
	{
		IsError = true;
		_errors = exception.MapErrors();
	}

	/// <summary>
	/// Gets a value indicating if there was an error while executing the method.
	/// </summary>
	/// <remarks>
	/// Use this property in case a <see cref="bool"/> result needs to be returned. You can enrich
	/// the result using the errors list to provide more detail on the result using a specific
	/// <see cref="Error.Code"/>.
	/// </remarks>
	[Pure]
	public bool IsError { get; } = false;

	/// <summary>
	/// Get a value indicating if the method succeded executing.
	/// </summary>
	/// <remarks>
	/// Use this property in case a <see cref="bool"/> result needs to be returned. You can enrich
	/// the result using the errors list to provide more detail on the result using a specific
	/// <see cref="Error.Code"/>.
	/// </remarks>
	[Pure]
	public bool IsSuccess => !IsError;

	/// <summary>
	/// Retrieve the errors in the result if <see cref="IsError"/> is <see langword="true"/>. Empty list otherwise.
	/// </summary>
	[Pure]
	public List<Error> Errors => IsError ? _errors! : [];

	[Pure]
	public static implicit operator Result(Error error) => new(error);

	[Pure]
	public static implicit operator Result(Error[] errors) => new([.. errors]);

	[Pure]
	public static implicit operator Result(List<Error> errors) => new(errors);

	[Pure]
	public static implicit operator Result(Exception exception) => new(exception);

	[Pure]
	public static implicit operator Result(int _) => new();

	[Pure]
	public static implicit operator bool(Result result) => !result.IsError;

	[Pure]
	public static explicit operator List<Error>?(Result result) => result.IsError ? result._errors : default;

	/// <summary>
	/// Executes actions depending on the success of failure of the result.
	/// </summary>
	/// <param name="onSuccess">Action to execute in case a successful result.</param>
	/// <param name="onError">Action to execute in case of a failed result (or error).
	/// The list of <see cref="Error"/>s is provided as an input parameter.</param>
	[Pure]
	public void Switch(Action onSuccess, Action<List<Error>> onError)
	{
		if (IsError)
		{
			onError(_errors!);
			return;
		}

		onSuccess();
	}

	/// <summary>
	/// Executes actions asynchronously depending on the success of failure of the result.
	/// </summary>
	/// <param name="onSuccess">Action to execute in case a successful result.</param>
	/// <param name="onError">Action to execute in case of a failed result (or error).
	/// The list of <see cref="Error"/>s is provided as an input parameter.</param>
	[Pure]
	public async Task SwitchAsync(Func<Task> onSuccess, Func<List<Error>, Task> onError)
	{
		if (IsError)
		{
			await onError(_errors!);
			return;
		}

		await onSuccess();
	}

	/// <summary>
	/// Returns a list of <see cref="Error"/>s in case of a failed result. An empty list otherwise.
	/// </summary>
	/// <param name="onError"><see cref="Func{T, TResult}"/> to execute in case there are errors.</param>
	/// <returns><see cref="List{T}"/> of <see cref="Error"/>s in case there is any.</returns>
	[Pure]
	public List<Error> MapErrors(Func<List<Error>, List<Error>> onError) => IsError ? _errors! : [];

	/// <summary>
	/// Returns a list of <see cref="Error"/>s asynchronously in case of a failed result. An empty list otherwise.
	/// </summary>
	/// <param name="onError"><see cref="Func{T, TResult}"/> to execute in case there are errors.</param>
	/// <returns><see cref="List{T}"/> of <see cref="Error"/>s in case there is any.</returns>
	[Pure]
	public async Task<List<Error>> MapErrorsAsyn(Func<List<Error>, Task<List<Error>>> onError)
		=> IsError ? await onError(_errors!) : [];
}
