using HyberBot;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    Hyber hyber;

    public async Task MainAsync()
    {
        hyber = new Hyber();

        await hyber.Login();
    }
}