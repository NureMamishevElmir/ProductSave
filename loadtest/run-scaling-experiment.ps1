<#
.SYNOPSIS
  Runs the ProductSave horizontal-scaling experiment for the lab report.

.DESCRIPTION
  For each replica count it:
    1. Disables the HPA's influence by fixing the Deployment to N replicas
       (kubectl scale), so the experiment is controlled.
    2. Waits for all api pods to become Ready.
    3. Runs the k6 load test, tagging the run with the replica count and
       writing a JSON summary to results/.
  Afterwards you compare results/summary-rN.json across N to plot
  "replicas vs requests/sec" for the report.

.EXAMPLE
  ./run-scaling-experiment.ps1 -Replicas 1,2,4,8 -BaseUrl http://localhost
#>

param(
  [int[]] $Replicas = @(1, 2, 4),
  [string] $BaseUrl = "http://localhost",
  [string] $Namespace = "productsave",
  [string] $Script = "$PSScriptRoot/k6/products-load.js"
)

$ErrorActionPreference = "Stop"
$resultsDir = Join-Path $PSScriptRoot "results"
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null

# Pause the HPA so it doesn't fight the manual scale during the experiment.
# (Patch minReplicas=maxReplicas=N each round; restore at the end.)
foreach ($n in $Replicas) {
  Write-Host "==== Replicas: $n ====" -ForegroundColor Cyan

  # Note the escaped inner quotes (\"): Windows PowerShell strips bare double
  # quotes when handing arguments to a native exe (kubectl), which corrupts the
  # JSON patch. Backslash-escaping keeps the quotes intact.
  kubectl -n $Namespace patch hpa api --type merge `
    -p ('{{\"spec\":{{\"minReplicas\":{0},\"maxReplicas\":{0}}}}}' -f $n) 2>$null
  kubectl -n $Namespace scale deployment/api --replicas=$n

  Write-Host "Waiting for $n api pod(s) to be Ready..."
  kubectl -n $Namespace rollout status deployment/api --timeout=180s

  $summary = Join-Path $resultsDir "summary-r$n.json"
  Write-Host "Running k6 (BASE_URL=$BaseUrl)..."
  # SLEEP=0 -> closed-model saturation: VUs hammer the API with no think-time,
  # so the API CPU (and eventually Postgres) is the limit, not an artificial
  # client throttle. That is what makes the replicas->RPS relationship real.
  k6 run --tag replicas=$n --summary-export=$summary `
    -e BASE_URL=$BaseUrl -e SLEEP=0 -e PEAK_VUS=300 $Script

  Write-Host "Saved $summary" -ForegroundColor Green
  Write-Host "Cooling down 30s before next round..."
  Start-Sleep -Seconds 30
}

# Restore autoscaling range.
kubectl -n $Namespace patch hpa api --type merge `
  -p '{\"spec\":{\"minReplicas\":2,\"maxReplicas\":10}}' 2>$null

Write-Host "`nDone. Compare requests/sec across:" -ForegroundColor Cyan
Get-ChildItem $resultsDir -Filter "summary-r*.json" | ForEach-Object {
  $j = Get-Content $_.FullName -Raw | ConvertFrom-Json
  $rps = [math]::Round($j.metrics.http_reqs.rate, 1)
  $p95 = [math]::Round($j.metrics.http_req_duration.'p(95)', 1)
  "{0,-22} rps={1,-8} p95={2}ms" -f $_.Name, $rps, $p95
}
