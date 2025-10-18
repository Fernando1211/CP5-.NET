# Script para testar a API SafeScribe
$baseUrl = "https://localhost:7000"

# Ignorar certificados SSL para testes locais
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "=== TESTANDO API SAFESCRIBE ===" -ForegroundColor Green

# Teste 1: Verificar se a API está respondendo
Write-Host "`n1. Testando se a API está online..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/swagger" -Method GET -UseBasicParsing
    Write-Host "✅ API está online! Status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "❌ API não está respondendo: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Teste 2: Registrar um usuário
Write-Host "`n2. Testando registro de usuário..." -ForegroundColor Yellow
$registerData = @{
    Username = "testuser"
    Password = "testpass123"
    Role = "Editor"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1/auth/registrar" -Method POST -Body $registerData -ContentType "application/json" -UseBasicParsing
    Write-Host "✅ Usuário registrado com sucesso! Status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "❌ Erro ao registrar usuário: $($_.Exception.Message)" -ForegroundColor Red
}

# Teste 3: Fazer login
Write-Host "`n3. Testando login..." -ForegroundColor Yellow
$loginData = @{
    Username = "testuser"
    Password = "testpass123"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1/auth/login" -Method POST -Body $loginData -ContentType "application/json" -UseBasicParsing
    $loginResult = $response.Content | ConvertFrom-Json
    $token = $loginResult.token
    Write-Host "✅ Login realizado com sucesso! Token obtido." -ForegroundColor Green
    Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Cyan
} catch {
    Write-Host "❌ Erro no login: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Teste 4: Criar uma nota (requer autenticação)
Write-Host "`n4. Testando criação de nota..." -ForegroundColor Yellow
$noteData = @{
    Title = "Nota de Teste"
    Content = "Esta é uma nota de teste criada via API"
} | ConvertTo-Json

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1/notas" -Method POST -Body $noteData -Headers $headers -UseBasicParsing
    $noteResult = $response.Content | ConvertFrom-Json
    $noteId = $noteResult.id
    Write-Host "✅ Nota criada com sucesso! ID: $noteId" -ForegroundColor Green
} catch {
    Write-Host "❌ Erro ao criar nota: $($_.Exception.Message)" -ForegroundColor Red
}

# Teste 5: Buscar a nota criada
Write-Host "`n5. Testando busca de nota..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1/notas/$noteId" -Method GET -Headers $headers -UseBasicParsing
    $note = $response.Content | ConvertFrom-Json
    Write-Host "✅ Nota encontrada! Título: $($note.title)" -ForegroundColor Green
} catch {
    Write-Host "❌ Erro ao buscar nota: $($_.Exception.Message)" -ForegroundColor Red
}

# Teste 6: Testar logout
Write-Host "`n6. Testando logout..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1/auth/logout" -Method POST -Headers $headers -UseBasicParsing
    Write-Host "✅ Logout realizado com sucesso!" -ForegroundColor Green
} catch {
    Write-Host "❌ Erro no logout: $($_.Exception.Message)" -ForegroundColor Red
}

# Teste 7: Tentar usar token após logout (deve falhar)
Write-Host "`n7. Testando token após logout (deve falhar)..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/v1/notas/$noteId" -Method GET -Headers $headers -UseBasicParsing
    Write-Host "❌ PROBLEMA: Token ainda funciona após logout!" -ForegroundColor Red
} catch {
    Write-Host "✅ Token corretamente invalidado após logout!" -ForegroundColor Green
}

Write-Host "`n=== TESTES CONCLUÍDOS ===" -ForegroundColor Green
