namespace EduPulse.DTOs.Common;

public class Result
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public int StatusCode { get; set; }

    public static Result Success(string message = "İşlem başarılı.", int statusCode = 200)
    {
        return new Result
        {
            IsSuccess = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static Result Failure(string message, int statusCode = 400)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data, string message = "İşlem başarılı.", int statusCode = 200)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Message = message,
            StatusCode = statusCode,
            Data = data
        };
    }

    public static new Result<T> Failure(string message, int statusCode = 400)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}