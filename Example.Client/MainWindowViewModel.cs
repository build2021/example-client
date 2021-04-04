namespace Example.Client
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Example.Client.Models;

    using Rester;

    using Smart.ComponentModel;
    using Smart.Windows.ViewModels;

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IHttpClientFactory httpClientFactory;

        public NotificationValue<int?> Id { get; } = new();
        public NotificationValue<string?> Name { get; } = new();
        public NotificationValue<bool?> Flag { get; } = new();
        public NotificationValue<DateTime?> DateTime { get; } = new();

        public NotificationValue<HttpStatusCode?> HttpStatusCode { get; } = new();

        public ICommand ExecuteCommand { get; }

        public MainWindowViewModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;

            ExecuteCommand = MakeAsyncCommand(Execute, () => Id.Value.HasValue).Observe(Id);
        }

        private async Task Execute()
        {
            using var client = httpClientFactory.CreateClient(ConnectionNames.Server);
            var result = await client.GetAsync<DataGetResponse>($"/v1/data/get/{Id.Value}").ConfigureAwait(false);
            if (result.IsSuccess())
            {
                Name.Value = result.Content.Name;
                Flag.Value = result.Content.Flag;
                DateTime.Value = result.Content.DateTime;
            }
            else
            {
                Name.Value = null;
                Flag.Value = null;
                DateTime.Value = null;
            }

            HttpStatusCode.Value = result.StatusCode;
        }
    }
}
