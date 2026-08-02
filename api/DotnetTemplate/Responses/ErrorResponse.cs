namespace DotnetTemplate.Responses;

public sealed class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int Status { get; set; }
}
