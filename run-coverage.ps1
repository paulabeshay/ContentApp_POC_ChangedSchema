# PowerShell script to run unit tests with code coverage and generate HTML report

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running Unit Tests with Code Coverage" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous test results
Write-Host "Cleaning previous test results..." -ForegroundColor Yellow
if (Test-Path "TestResults") {
    Remove-Item -Path "TestResults" -Recurse -Force
}
if (Test-Path "CoverageReport") {
    Remove-Item -Path "CoverageReport" -Recurse -Force
}

# Run tests with coverage collection
Write-Host ""
Write-Host "Running tests with coverage collection..." -ForegroundColor Yellow
dotnet test --configuration Release `
    --settings coverlet.runsettings `
    --collect:"XPlat Code Coverage" `
    --results-directory:"./TestResults" `
    --logger:"console;verbosity=detailed"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Tests failed! Exit code: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Find coverage files
Write-Host ""
Write-Host "Locating coverage files..." -ForegroundColor Yellow
$coverageFiles = Get-ChildItem -Path "TestResults" -Filter "coverage.cobertura.xml" -Recurse

if ($coverageFiles.Count -eq 0) {
    Write-Host "No coverage files found!" -ForegroundColor Red
    exit 1
}

Write-Host "Found $($coverageFiles.Count) coverage file(s)" -ForegroundColor Green

# Generate HTML report
Write-Host ""
Write-Host "Generating HTML coverage report..." -ForegroundColor Yellow
$coveragePaths = $coverageFiles | ForEach-Object { $_.FullName }
$coveragePathsString = $coveragePaths -join ";"

reportgenerator `
    -reports:"$coveragePathsString" `
    -targetdir:"CoverageReport" `
    -reporttypes:"Html;Badges;TextSummary" `
    -verbosity:"Info"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Report generation failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Display summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Coverage Report Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if (Test-Path "CoverageReport\Summary.txt") {
    Get-Content "CoverageReport\Summary.txt"
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Coverage report generated successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Report location: $(Resolve-Path 'CoverageReport\index.html')" -ForegroundColor Yellow
Write-Host ""

# Ask to open report
$openReport = Read-Host "Open coverage report in browser? (Y/N)"
if ($openReport -eq "Y" -or $openReport -eq "y") {
    Start-Process "CoverageReport\index.html"
}
