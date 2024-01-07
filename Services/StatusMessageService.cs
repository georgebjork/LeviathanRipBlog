namespace LeviathanRipBlog.Services;

public class StatusMessageService {

    public event Action<string, string>? OnAlert;

    public void ShowAlert(string message, string alertType = "success")
    {
        OnAlert?.Invoke(message, alertType);
    }
    
}
