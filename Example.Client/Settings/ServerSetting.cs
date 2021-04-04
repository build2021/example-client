namespace Example.Client.Settings
{
    using System.Diagnostics.CodeAnalysis;

    public class ServerSetting
    {
        [AllowNull]
        public string BaseUrl { get; set; }
    }
}
