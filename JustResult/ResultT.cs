using System.Diagnostics.Contracts;

namespace JustResult;

/// <summary>
/// Represents a result of type <see cref="TValue"/> from a method call.
/// </summary>
/// <remarks>
/// Use this implementation instead of using exceptions to control the flow of execution.
/// </remarks>
/// <typeparam name="TValue">Any concrete type that holds the successful result of a method.</typeparam>
public readonly record struct Result<TValue>
{
	private readonly TValue? _value;
	private readonly List<Error>? _errors = null;

	private Result(TValue value)
	{
		IsError = false;
		_value = value;
		_errors = default;
	}

	private Result(Error error)
	{
		IsError = true;
		_value = default;
		_errors = [error];
	}

	private Result(List<Error> errors)
	{
		IsError = true;
		_value = default;
		_errors = new(errors);
	}

	private Result(Exception exception)
	{
		IsError = true;
		_value = default;
		_errors = exception.MapErrors();
	}

	/// <summary>
	/// Gets a value indicating if there was an error while executing the method.
	/// </summary>
	/// <remarks>
	/// This property should not be used as a <see cref="bool"/> result. For that
	/// use case utilize the non generic <see cref="Result"/> instead.
	/// </remarks>
	[Pure]
	public bool IsError { get; }

	/// <summary>
	/// Get a value indicating if the method succeded executing.
	/// </summary>
	/// <remarks>
	/// This property should not be used as a <see cref="bool"/> result. For that
	/// use case utilize the non generic <see cref="Result"/> instead.
	/// </remarks>
	[Pure]
	public bool IsSuccess => !IsError;

	/// <summary>
	/// Retrieve the value in the result if <see cref="IsSuccess"/> is <see langword="true"/>
	/// </summary>
	/// <exception cref="InvalidOperationException">If the <see cref="IsError"/> is <see langword="true"/>.</exception>
	[Pure]
	public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("Can not retrieve value from an 'Error' result.");

	/// <summary>
	/// Retrieve the errors in the result if <see cref="IsError"/> is <see langword="true"/>. Empty list otherwise.
	/// </summary>
	[Pure]
	public List<Error> Errors => IsError ? _errors! : [];

	[Pure]
	public static implicit operator Result<TValue>(TValue value) => new(value);

	[Pure]
	public static implicit operator Result<TValue>(Error error) => new(error);

	[Pure]
	public static implicit operator Result<TValue>(Error[] errors) => new([.. errors]);

	[Pure]
	public static implicit operator Result<TValue>(List<Error> errors) => new(errors);

	[Pure]
	public static implicit operator Result<TValue>(Exception exception) => new(exception);

	[Pure]
	public static implicit operator bool(Result<TValue> result) => result.IsSuccess;

	[Pure]
	public static explicit operator TValue?(Result<TValue> result) => result.IsSuccess ? result._value! : default;

	[Pure]
	public static explicit operator List<Error>?(Result<TValue> result) => result.IsError ? result._errors : default;

	/// <summary>
	/// Matches the result to a specific type using the provided <see cref="Func{T, TResult}"/>s.
	/// </summary>
	/// <typeparam name="TResult">The resulting type once the result is matched.</typeparam>
	/// <param name="success"><see cref="Func{T, TResult}"/> to execute in case of success. The input parameter will be of type <see cref="TValue"/>.</param>
	/// <param name="failure"><see cref="Func{T, TResult}"/> to execute in case of failure. The input parameter will be of type <see cref="List{T}"/>, with T as <see cref="Error"/>.</param>
	/// <returns>The matched result.</returns>
	[Pure]
	public TResult Match<TResult>(Func<TValue, TResult> success, Func<List<Error>, TResult> failure)
		=> IsSuccess ? success(_value!) : failure(_errors!);

	/// <summary>
	/// Matches the result to a specific type using the provided asynchonous <see cref="Func{T, TResult}"/>s.
	/// </summary>
	/// <typeparam name="TResult">The resulting type once the result is matched.</typeparam>
	/// <param name="success"><see cref="Func{T, TResult}"/> to execute in case of success. The input parameter will be of type <see cref="TValue"/>.</param>
	/// <param name="failure"><see cref="Func{T, TResult}"/> to execute in case of failure. The input parameter will be of type <see cref="List{T}"/>, with T as <see cref="Error"/>.</param>
	/// <returns>The matched result.</returns>
	[Pure]
	public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> success, Func<List<Error>, Task<TResult>> failure)
		=> IsSuccess ? await success(_value!) : await failure(_errors!);
}
