using System.ComponentModel;
using McpServer.Models;
using ModelContextProtocol.Server;

[McpServerToolType]
public static class McpTools
{
    [McpServerTool, Description("Retrieves a list of all invoices in InvoiceApp")]
    public static Task<List<Invoice>> ListInvoices(InvoiceApiClient client)
    {
        return client.ListInvoices();
    }

    [McpServerTool, Description("Finds the invoice with this name")]
    public static Task<Invoice> FindInvoiceByName(string invoiceName, InvoiceApiClient client)
    {
        return client.FindInvoiceByName(invoiceName);
    }

    [McpServerTool, Description("Creates an invoice and returns the new invoice object")]
    public static Task<Invoice> CreateInvoice(CreateInvoiceRequest createRequest, InvoiceApiClient client)
    {
        return client.CreateInvoice(createRequest);
    }

    [McpServerTool, Description("Marks an invoice as paid")]
    public static Task MarkAsPaid(string invoiceId, InvoiceApiClient client)
    {
        return client.MarkAsPaid(invoiceId);
    }


}
