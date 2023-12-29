namespace JustResult;

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

	public bool IsError { get; }

	public bool IsSuccess => !IsError;

	public static implicit operator Result<TValue>(TValue value) => new(value);

	public static implicit operator Result<TValue>(Error error) => new(error);

	public static implicit operator Result<TValue>(Error[] errors) => new([.. errors]);

	public static implicit operator Result<TValue>(List<Error> errors) => new(errors);

	public static implicit operator Result<TValue>(Exception exception) => new(exception);

	public static implicit operator bool(Result<TValue> result) => result.IsSuccess;

	public static implicit operator TValue?(Result<TValue> result) => result.IsSuccess ? result._value! : default;

	public static explicit operator List<Error>?(Result<TValue> result) => result.IsError ? result._errors : default;

	public TResult Match<TResult>(Func<TValue, TResult> success, Func<List<Error>, TResult> failure)
		=> IsSuccess ? success(_value!) : failure(_errors!);

	public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> success, Func<List<Error>, Task<TResult>> failure)
		=> IsSuccess ? await success(_value!) : await failure(_errors!);
}
