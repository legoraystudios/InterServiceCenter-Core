namespace InterServiceCenter_Core.Utilities;

public class JsonResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }
    public object? TodaysYear { get; set; }
    public object? LastYear { get; set; }
}