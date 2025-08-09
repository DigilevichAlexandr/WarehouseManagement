using System.Net.Http.Json;
using WarehouseManagement.Shared.Models;
using WarehouseManagement.Shared.DTOs;
using ClientModel = WarehouseManagement.Shared.Models.Client;

namespace WarehouseManagement.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Resources
        public async Task<IEnumerable<Resource>> GetResourcesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Resource>>("api/resources") ?? new List<Resource>();
        }

        public async Task<IEnumerable<Resource>> GetActiveResourcesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Resource>>("api/resources/active") ?? new List<Resource>();
        }

        public async Task<Resource?> GetResourceAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Resource>($"api/resources/{id}");
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            var response = await _httpClient.PostAsJsonAsync("api/resources", resource);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Resource>() ?? resource;
        }

        public async Task UpdateResourceAsync(Resource resource)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/resources/{resource.Id}", resource);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteResourceAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/resources/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ArchiveResourceAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/resources/{id}/archive", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RestoreResourceAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/resources/{id}/restore", null);
            response.EnsureSuccessStatusCode();
        }

        // Units of Measurement
        public async Task<IEnumerable<UnitOfMeasurement>> GetUnitsOfMeasurementAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UnitOfMeasurement>>("api/unitsofmeasurement") ?? new List<UnitOfMeasurement>();
        }

        public async Task<IEnumerable<UnitOfMeasurement>> GetActiveUnitsOfMeasurementAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UnitOfMeasurement>>("api/unitsofmeasurement/active") ?? new List<UnitOfMeasurement>();
        }

        public async Task<UnitOfMeasurement?> GetUnitOfMeasurementAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<UnitOfMeasurement>($"api/unitsofmeasurement/{id}");
        }

        public async Task<UnitOfMeasurement> CreateUnitOfMeasurementAsync(UnitOfMeasurement unit)
        {
            var response = await _httpClient.PostAsJsonAsync("api/unitsofmeasurement", unit);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UnitOfMeasurement>() ?? unit;
        }

        public async Task UpdateUnitOfMeasurementAsync(UnitOfMeasurement unit)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/unitsofmeasurement/{unit.Id}", unit);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteUnitOfMeasurementAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/unitsofmeasurement/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ArchiveUnitOfMeasurementAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/unitsofmeasurement/{id}/archive", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RestoreUnitOfMeasurementAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/unitsofmeasurement/{id}/restore", null);
            response.EnsureSuccessStatusCode();
        }

        // Clients
        public async Task<IEnumerable<ClientModel>> GetClientsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ClientModel>>("api/clients") ?? new List<ClientModel>();
        }

        public async Task<IEnumerable<ClientModel>> GetActiveClientsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ClientModel>>("api/clients/active") ?? new List<ClientModel>();
        }

        public async Task<ClientModel?> GetClientAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ClientModel>($"api/clients/{id}");
        }

        public async Task<ClientModel> CreateClientAsync(ClientModel client)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClientModel>() ?? client;
        }

        public async Task UpdateClientAsync(ClientModel client)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/clients/{client.Id}", client);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteClientAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ArchiveClientAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/clients/{id}/archive", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RestoreClientAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/clients/{id}/restore", null);
            response.EnsureSuccessStatusCode();
        }

        // Balance
        public async Task<IEnumerable<Balance>> GetBalanceAsync(BalanceFilterDto? filter = null)
        {
            var url = "api/balance";
            if (filter != null)
            {
                var queryParams = new List<string>();
                
                if (filter.ResourceIds.Any())
                {
                    queryParams.AddRange(filter.ResourceIds.Select(id => $"ResourceIds={id}"));
                }
                
                if (filter.UnitOfMeasurementIds.Any())
                {
                    queryParams.AddRange(filter.UnitOfMeasurementIds.Select(id => $"UnitOfMeasurementIds={id}"));
                }

                if (queryParams.Any())
                {
                    url += "?" + string.Join("&", queryParams);
                }
            }

            return await _httpClient.GetFromJsonAsync<IEnumerable<Balance>>(url) ?? new List<Balance>();
        }

        // Receipt Documents
        public async Task<IEnumerable<ReceiptDocument>> GetReceiptDocumentsAsync(DocumentFilterDto? filter = null)
        {
            var url = "api/receiptdocuments";
            if (filter != null)
            {
                var queryParams = BuildDocumentFilterQuery(filter);
                if (queryParams.Any())
                {
                    url += "?" + string.Join("&", queryParams);
                }
            }

            return await _httpClient.GetFromJsonAsync<IEnumerable<ReceiptDocument>>(url) ?? new List<ReceiptDocument>();
        }

        public async Task<ReceiptDocument?> GetReceiptDocumentAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ReceiptDocument>($"api/receiptdocuments/{id}");
        }

        public async Task<ReceiptDocument> CreateReceiptDocumentAsync(ReceiptDocument document)
        {
            var response = await _httpClient.PostAsJsonAsync("api/receiptdocuments", document);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ReceiptDocument>() ?? document;
        }

        public async Task UpdateReceiptDocumentAsync(ReceiptDocument document)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/receiptdocuments/{document.Id}", document);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteReceiptDocumentAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/receiptdocuments/{id}");
            response.EnsureSuccessStatusCode();
        }

        // Shipment Documents
        public async Task<IEnumerable<ShipmentDocument>> GetShipmentDocumentsAsync(DocumentFilterDto? filter = null)
        {
            var url = "api/shipmentdocuments";
            if (filter != null)
            {
                var queryParams = BuildDocumentFilterQuery(filter);
                if (queryParams.Any())
                {
                    url += "?" + string.Join("&", queryParams);
                }
            }

            return await _httpClient.GetFromJsonAsync<IEnumerable<ShipmentDocument>>(url) ?? new List<ShipmentDocument>();
        }

        public async Task<ShipmentDocument?> GetShipmentDocumentAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ShipmentDocument>($"api/shipmentdocuments/{id}");
        }

        public async Task<ShipmentDocument> CreateShipmentDocumentAsync(ShipmentDocument document)
        {
            var response = await _httpClient.PostAsJsonAsync("api/shipmentdocuments", document);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShipmentDocument>() ?? document;
        }

        public async Task UpdateShipmentDocumentAsync(ShipmentDocument document)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/shipmentdocuments/{document.Id}", document);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteShipmentDocumentAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/shipmentdocuments/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<ShipmentDocument> SignShipmentDocumentAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/shipmentdocuments/{id}/sign", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShipmentDocument>() ?? new ShipmentDocument();
        }

        public async Task<ShipmentDocument> RevokeShipmentDocumentAsync(int id)
        {
            var response = await _httpClient.PostAsync($"api/shipmentdocuments/{id}/revoke", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShipmentDocument>() ?? new ShipmentDocument();
        }

        private List<string> BuildDocumentFilterQuery(DocumentFilterDto filter)
        {
            var queryParams = new List<string>();

            if (filter.DateFrom.HasValue)
            {
                queryParams.Add($"DateFrom={filter.DateFrom.Value:yyyy-MM-dd}");
            }

            if (filter.DateTo.HasValue)
            {
                queryParams.Add($"DateTo={filter.DateTo.Value:yyyy-MM-dd}");
            }

            if (filter.DocumentNumbers.Any())
            {
                queryParams.AddRange(filter.DocumentNumbers.Select(number => $"DocumentNumbers={Uri.EscapeDataString(number)}"));
            }

            if (filter.ResourceIds.Any())
            {
                queryParams.AddRange(filter.ResourceIds.Select(id => $"ResourceIds={id}"));
            }

            if (filter.UnitOfMeasurementIds.Any())
            {
                queryParams.AddRange(filter.UnitOfMeasurementIds.Select(id => $"UnitOfMeasurementIds={id}"));
            }

            return queryParams;
        }
    }
}
