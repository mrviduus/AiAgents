using System.Text.Json;
using InvoiceAgentApi.Models;

namespace InvoiceAgentApi.Services;

public class InvoiceApiClient
{
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _baseUrl = "http://localhost:5000/api/invoices";
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<Invoice>> ListInvoices()
    {
        var response = await _httpClient.GetAsync(_baseUrl);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var invoices = JsonSerializer.Deserialize<List<Invoice>>(json, _jsonOptions);
        return invoices ?? new List<Invoice>();
    }

    public async Task<Invoice> FindInvoiceByName(string name)
    {
        var url = $"{_baseUrl}/by-description?description={Uri.EscapeDataString(name)}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var invoice = JsonSerializer.Deserialize<Invoice>(json, _jsonOptions);
        return invoice!;
    }

    public async Task<Invoice> CreateInvoice(CreateInvoiceRequest request)
    {
        var newInvoice = new Invoice()
        {
            Id = 0,
            Description = request.Description,
            Status = "Pending",
            Amount = request.Amount,
            Date = DateTime.Now,
            Due = request.Due ?? DateTime.Today.AddMonths(1)
        };
        var jsonContent = new StringContent(JsonSerializer.Serialize(newInvoice, _jsonOptions), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_baseUrl, jsonContent);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        var createdInvoice = JsonSerializer.Deserialize<Invoice>(responseJson, _jsonOptions)!;
        return createdInvoice;
    }

    public async Task MarkAsPaid(string invoiceId)
    {
        var request = new UpdateInvoiceRequest
        {
            Status = "Paid"
        };
        var jsonContent = new StringContent(JsonSerializer.Serialize(request, _jsonOptions), System.Text.Encoding.UTF8, "application/json");
        var url = $"{_baseUrl}/{invoiceId}/status";
        var response = await _httpClient.PostAsync(url, jsonContent);
        response.EnsureSuccessStatusCode();
    }

}
