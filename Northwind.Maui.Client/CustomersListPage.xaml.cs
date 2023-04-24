using System.Net.Http.Headers; // MediaTypeWithQualityHeaderValue 
using System.Net.Http.Json; // ReadFromJsonAsync<T>

namespace Northwind.Maui.Client;

public partial class CustomersListPage : ContentPage
{
	public CustomersListPage()
	{
		InitializeComponent();
        CustomersListViewModel viewModel = new();
        //string address = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5008" : "https://localhost:5002";


        try
        {
            HttpClient client = new()
            {
                BaseAddress = new Uri("http://10.0.2.2:5008")   // ip = 192.168.2.3 
            };

            client.DefaultRequestHeaders.Accept.Add(
              new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client
              .GetAsync("api/customers").Result;

            response.EnsureSuccessStatusCode();

            IEnumerable<CustomerDetailViewModel> customersFromService =
              response.Content.ReadFromJsonAsync
              <IEnumerable<CustomerDetailViewModel>>().Result;

            foreach (CustomerDetailViewModel c in customersFromService
              .OrderBy(customer => customer.CompanyName))
            {
                viewModel.Add(c);
            }
        }
        catch (Exception ex)
        {
            DisplayAlert(title: "Exception",
              message: $"App will use sample data due to: {ex.Message}",
              cancel: "OK");

            viewModel.AddSampleData();
        }
        BindingContext = viewModel;
    }
    async void Customer_Tapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not CustomerDetailViewModel c) return;
        // navigate to the detail view and show the tapped customer
        await Navigation.PushAsync(new CustomerDetailPage(
          BindingContext as CustomersListViewModel, c));
    }
    async void Customers_Refreshing(object sender, EventArgs e)
    {
        if (sender is not ListView listView) return;
        listView.IsRefreshing = true;
        // simulate a refresh
        await Task.Delay(1500);
        listView.IsRefreshing = false;
    }

    void Customer_Deleted(object sender, EventArgs e)
    {
        MenuItem menuItem = sender as MenuItem;
        if (menuItem.BindingContext is not CustomerDetailViewModel c) return;
        (BindingContext as CustomersListViewModel).Remove(c);
    }
    async void Customer_Phoned(object sender, EventArgs e)
    {
        MenuItem menuItem = sender as MenuItem;
        if (menuItem.BindingContext is not CustomerDetailViewModel c) return;
        if (await DisplayAlert("Dial a Number",
          "Would you like to call " + c.Phone + "?",
          "Yes", "No"))
        {
            PhoneDialer.Open(c.Phone);
        }
    }
    async void Add_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CustomerDetailPage(
          BindingContext as CustomersListViewModel));
    }
}