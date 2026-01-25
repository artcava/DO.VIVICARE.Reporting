# 🏢 GUIDA AMMINISTRATIVA - DEPLOYMENT
## DO.VIVICARE.Reporting - Deployment e Gestione

**Per:** IT Administrators, System Engineers  
**Data:** 25 Gennaio 2026

---

## 📋 OVERVIEW

Velopack supporta deployment su larga scala:

- ✅ Mass deployment via batch scripts
- ✅ Update management centralizzato
- ✅ Version control automizzato
- ✅ Network distribution supportato

---

## BATCH DEPLOYMENT SCRIPT

```powershell
# deploy-velopack.ps1
Param(
    [string]$MsiPath = "\\\\SERVER\\Software\\DO.VIVICARE.Reporting-1.0.0.msi",
    [string]$LogFile = "C:\\Logs\\DO.VIVICARE.Deploy.log"
)

function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    "[$timestamp] $Message" | Tee-Object -FilePath $LogFile -Append
}

Write-Log "Starting deployment..."

if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Log "ERROR: Must run as Administrator"
    exit 1
}

if (-not (Test-Path $MsiPath)) {
    Write-Log "ERROR: MSI not found at $MsiPath"
    exit 1
}

Write-Log "Installing MSI from $MsiPath"
$process = Start-Process msiexec.exe -ArgumentList "/i `"$MsiPath`" /quiet /norestart" -Wait -PassThru

if ($process.ExitCode -eq 0) {
    Write-Log "SUCCESS: Installation completed"
    exit 0
} else {
    Write-Log "ERROR: Installation failed with exit code $($process.ExitCode)"
    exit 1
}
```

**Usage:**
```powershell
.\deploy-velopack.ps1 -MsiPath "\\\\SERVER\\Software\\DO.VIVICARE.Reporting-1.0.0.msi"
```

---

## UPDATE MANAGEMENT

### Option 1: Auto-Update (Recommended)
- Users get updates automatically
- No IT intervention needed
- Works everywhere

### Option 2: Phased Rollout
```
Week 1: Canary (10% power users)
Week 2: Beta (30% standard users)
Week 3: General (100% all users)
```

---

## SECURITY

### Code Signing Verification
```powershell
$cert = (Get-AuthenticodeSignature "DO.VIVICARE.Reporting-1.0.0.msi").SignerCertificate
Write-Host "Publisher: $($cert.Subject)"
Write-Host "Valid: $(if ($cert) { 'Yes' } else { 'No' })"
```

### Network Share Setup
```
\\\\SERVER\\Software\\DO.VIVICARE\\
  ├── DO.VIVICARE.Reporting-1.0.0.msi
  ├── DO.VIVICARE.Reporting-1.1.0.msi
  └── README.txt

Permissions:
- Domain Users: Read
- Admins: Full Control
```

---

## TROUBLESHOOTING

### MSI Installation Fails
```powershell
msiexec /i "app.msi" /l*v "C:\\Logs\\msi_install.log"

# Common issues:
# 1603: General install error
# 1722: Error calling Windows Installer
# 1921: Service not started
```

### Updates Not Applying
```powershell
Get-Process | Where {$_.Name -like "*DO.VIVICARE*"}
dir "$env:LOCALAPPDATA\\DO.VIVICARE\\updates\"
```

---

## MONITORING

### Event Log
```
Event Viewer → Application
Source: DO.VIVICARE
ID: 1000+ (Installation)
ID: 2000+ (Updates)
```

### Version Report
```powershell
Get-WmiObject Win32_Product | Where {$_.Name -like "*DO.VIVICARE*"} | `
  Select-Object PSComputerName, Name, Version, InstallDate
```
