namespace Peach.TriathlonShop.FunctionApp.Extensions;

public static class SignalRHubName
{
    public const string TestHub = "Tests";
    public const string OrdersHub = "Orders";
    public const string HomePageHub = "HomePage";

    public static List<string> All =>
    [
        TestHub,
        OrdersHub,
        HomePageHub,
    ];
}