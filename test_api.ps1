param(
    [string]$BaseUrl = "https://localhost:7209"
)

Write-Host "Testing API endpoints..." -ForegroundColor Green

# Ignore SSL certificate errors for testing
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# Test basic health
Write-Host "`nTesting health endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/test/health"
    Write-Host "Health: " -NoNewline
    Write-Host "SUCCESS" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
} catch {
    Write-Host "Health: " -NoNewline
    Write-Host "FAILED" -ForegroundColor Red
    Write-Host $_.Exception.Message
}

# Test database connection
Write-Host "`nTesting database endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/test/database"
    Write-Host "Database: " -NoNewline
    Write-Host "SUCCESS" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
} catch {
    Write-Host "Database: " -NoNewline
    Write-Host "FAILED" -ForegroundColor Red
    Write-Host $_.Exception.Message
}

# Test receipt documents
Write-Host "`nTesting receipt documents..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/test/documents/receipt"
    Write-Host "Receipt Docs: " -NoNewline
    Write-Host "SUCCESS" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
} catch {
    Write-Host "Receipt Docs: " -NoNewline
    Write-Host "FAILED" -ForegroundColor Red
    Write-Host $_.Exception.Message
}

# Test shipment documents
Write-Host "`nTesting shipment documents..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/test/documents/shipment"
    Write-Host "Shipment Docs: " -NoNewline
    Write-Host "SUCCESS" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 2
} catch {
    Write-Host "Shipment Docs: " -NoNewline
    Write-Host "FAILED" -ForegroundColor Red
    Write-Host $_.Exception.Message
}

# Test actual API endpoints
Write-Host "`nTesting actual API endpoints..." -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/receiptdocuments"
    Write-Host "Receipt API: " -NoNewline
    Write-Host "SUCCESS" -ForegroundColor Green
    Write-Host "Count: $($response.Count)"
} catch {
    Write-Host "Receipt API: " -NoNewline
    Write-Host "FAILED" -ForegroundColor Red
    Write-Host $_.Exception.Message
}

try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/shipmentdocuments"
    Write-Host "Shipment API: " -NoNewline
    Write-Host "SUCCESS" -ForegroundColor Green
    Write-Host "Count: $($response.Count)"
} catch {
    Write-Host "Shipment API: " -NoNewline
    Write-Host "FAILED" -ForegroundColor Red
    Write-Host $_.Exception.Message
}

Write-Host "`nTesting completed!" -ForegroundColor Green
