namespace Account.Models;

public class AccountResult
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
}


public class AccountResult<T> : AccountResult
{
    public T? Content { get; set; }
}
