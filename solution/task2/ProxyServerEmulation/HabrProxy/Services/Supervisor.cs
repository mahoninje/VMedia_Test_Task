namespace HabrProxy.Services
{
    /// <summary>
    /// Class-supervisor
    /// </summary>
    public static class Supervisor
    {
        public static string? RequestedPath { get; set; }
        public static string? OriginalContent { get; set; }
        public static bool IsRedirected { get; set; } = false;
        public static string CurrentPath { get; set; } = "/ru/all";
        public static string ImageContent { get; set; }
    }
}
