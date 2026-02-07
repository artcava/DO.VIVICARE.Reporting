# Release Commands - Quick Reference

## 🎯 **Release Application with Velopack (v1.2.0, v1.2.1, etc.)**

### When to Use
When you update **UI or Reporter** and want to release a new app version with:
- Automatic code signing (SHA256 timestamped)
- Velopack packaging (delta updates + installer)
- GitHub release with auto-generated release notes
- **NO manual manifest.json updates needed** ✅

### How It Works (Automated)

```
1. Push tag v1.2.0
         ↓
2. GitHub Actions triggers
         ↓
3. Build & Test (all configurations)
         ↓
4. Import code signing certificate
         ↓
5. Create Velopack release package
         ↓
6. Pack with Velopack CLI
         ↓
7. Sign with certificate (SHA256 + timestamp)
         ↓
8. Upload to GitHub Release
         ↓
9. Cleanup certificate from store
```

### Commands

```bash
# 1. Update version numbers in AssemblyInfo.cs files
# DO.VIVICARE.UI/Properties/AssemblyInfo.cs
# [assembly: AssemblyVersion("1.2.0.0")]

# DO.VIVICARE.Reporter/Properties/AssemblyInfo.cs  
# [assembly: AssemblyVersion("1.2.0.0")]

# 2. Commit changes
git add .
git commit -m "v1.2.0: UI and Reporter updates"

# 3. Push to master
git push origin master

# 4. Create and push tag (triggers GitHub Actions)
# ⚠️ IMPORTANT: Tag format MUST be v*.*.* (NOT v*.*.*.0 with 4 numbers)
git tag -a v1.2.0 -m "Release version 1.2.0 - Feature X, Bug fix Y"
git push origin v1.2.0
```

### What Happens Automatically

✅ GitHub Actions CI/CD Pipeline:
- Builds UI + Reporter in Release mode
- Runs all unit and integration tests
- Creates Velopack release package
- **Automatically signs binaries** with certificate from GitHub Secrets
- Generates installer + delta update packages
- Creates GitHub Release with signed artifacts
- Generates release notes (title + what's new)
- **Cleans up certificate** from certificate store

### Result Files

```
GitHub Release: v1.2.0
├── DO.VIVICARE.Reporting-1.2.0.nupkg          (full package)
├── DO.VIVICARE.Reporting-1.2.0-delta.nupkg    (delta update)
├── DO.VIVICARE.Reporting-1.2.0.msi            (installer - signed)
├── DO.VIVICARE.Reporting-Setup.exe            (bootstrapper)
└── RELEASE_NOTES_GENERATED.md                 (auto-generated notes)
```

### Release URL
```
https://github.com/artcava/DO.VIVICARE.Reporting/releases/tag/v1.2.0
```

### Installation for End Users

**Automatic (First Install):**
```
User downloads: DO.VIVICARE.Reporting-Setup.exe
User runs: DO.VIVICARE.Reporting-Setup.exe
         ↓
         App installed
         ↓
         App auto-checks for updates on launch
         ↓
         Velopack handles delta updates transparently
```

**Manual (Alternative):**
```
User downloads: DO.VIVICARE.Reporting-1.2.0.msi
User runs: msiexec /i DO.VIVICARE.Reporting-1.2.0.msi
```

---

## 🚀 **Certificate & Signing Setup (ONE-TIME SETUP)

### Prerequisites

✅ GitHub Secrets configured:
```bash
gh secret set CODESIGN_CERTIFICATE_BASE64 --body "<base64-cert>"
gh secret set CODESIGN_PASSWORD --body "<password>"
```

✅ Certificate thumbprint automatically detected by CI/CD pipeline

### How to Create Signing Certificate (if you don't have one)

```powershell
# PowerShell as Administrator

# 1. Create self-signed certificate (valid 1 year)
$cert = New-SelfSignedCertificate `
  -CertStoreLocation "cert:\CurrentUser\My" `
  -Subject "CN=DO.VIVICARE.Reporting" `
  -FriendlyName "DO.VIVICARE Code Signing" `
  -Type CodeSigningCert `
  -NotAfter (Get-Date).AddYears(1)

# 2. Export to PFX
$password = ConvertTo-SecureString -String "YourPassword123!" -AsPlainText -Force
Export-PfxCertificate `
  -Cert $cert `
  -FilePath "C:\Users\YourName\DO.VIVICARE-CodeSign.pfx" `
  -Password $password

# 3. Convert to Base64 for GitHub Secret
$base64 = [Convert]::ToBase64String((Get-Content "C:\Users\YourName\DO.VIVICARE-CodeSign.pfx" -Encoding Byte))
echo "Copy this and paste in GitHub Secret:"
echo $base64

# 4. Add to GitHub Secrets
gh secret set CODESIGN_CERTIFICATE_BASE64 --body $base64
gh secret set CODESIGN_PASSWORD --body "YourPassword123!"
```

---

## 📦 **Release Single Plugin**

### When to Use
When you update **ONE plugin library** (Document or Report) and want to release independently

### How It Works (Same Automated Process)

```
1. Push tag plugin/document.adialtaintensita/1.0.5
         ↓
2. GitHub Actions triggers plugin-specific job
         ↓
3. Build & Test
         ↓
4. Package DLL
         ↓
5. Generate checksum (SHA256)
         ↓
6. Upload to GitHub Release
```

**⚠️ Key Difference:** Plugin releases are NOT code-signed (only DLL packages)

### Commands

```bash
# 1. Update version in single project
# DO.VIVICARE.Document.ADIAltaIntensita/Properties/AssemblyInfo.cs
# [assembly: AssemblyVersion("1.0.5.0")]

# 2. Commit ONLY this project
git add DO.VIVICARE.Document.ADIAltaIntensita/
git commit -m "Update: Document.ADIAltaIntensita v1.0.5 - Bug fixes"

# 3. Push to master
git push origin master

# 4. Create tag with plugin format
# Format: plugin/<plugin-id>/<version>
git tag -a plugin/document.adialtaintensita/1.0.5 -m "ADI Alta Intensita v1.0.5"

# 5. Push tag (triggers GitHub Actions for this plugin only)
git push origin plugin/document.adialtaintensita/1.0.5
```

### Result
```
GitHub Release: plugin/document.adialtaintensita/1.0.5
├── DO.VIVICARE.Document.ADIAltaIntensita-1.0.5.dll
└── CHECKSUM.txt
```

### Installation (User in Plugin Manager)
```
Plugin Manager detects new version:
  1. Shows "Update available: 1.0.5"
  2. User clicks "Download"
  3. Verifies SHA256 checksum automatically
  4. Installs to Plugins folder
  5. Available immediately (no restart needed)
```

---

## 📄 **Plugin ID Reference

### Document Plugins
```
plugin/document.adialtaintensita/VERSION
plugin/document.adibassaintensita/VERSION
plugin/document.asst/VERSION
plugin/document.comuni/VERSION
plugin/document.laziohealthworker/VERSION
plugin/document.minsan/VERSION
plugin/document.prestazioni/VERSION
plugin/document.prezzi/VERSION
plugin/document.rendiconto/VERSION
plugin/document.report16/VERSION
plugin/document.report18/VERSION
plugin/document.valorizzazione/VERSION
plugin/document.valorizzazioniadialta/VERSION
plugin/document.zsdfatture/VERSION
```

### Report Plugins
```
plugin/report.allegatoadi/VERSION
plugin/report.dietetica/VERSION
plugin/report.valorizzazione/VERSION
```

---

## 💭 **Common Scenarios

### Scenario 1: Fix critical bug in UI

```bash
# Edit UI code to fix bug
# ...

# 1. Update UI version
# DO.VIVICARE.UI/Properties/AssemblyInfo.cs
# [assembly: AssemblyVersion("1.2.1.0")]

# 2. Commit
git add .
git commit -m "v1.2.1: Fix critical login bug"
git push origin master

# 3. Release (fully automated)
git tag -a v1.2.1 -m "v1.2.1 - Critical bug fix"
git push origin v1.2.1

# Result: New signed installer + delta package automatically created
```

---

### Scenario 2: Update 2 plugins (ADI Alta + Dietetica)

```bash
# First: Release ADI Alta Intensita
git add DO.VIVICARE.Document.ADIAltaIntensita/
git commit -m "Update: Document.ADIAltaIntensita v1.0.6"
git push origin master
git tag -a plugin/document.adialtaintensita/1.0.6 -m "v1.0.6"
git push origin plugin/document.adialtaintensita/1.0.6

# Wait for GitHub Actions to complete (check Actions tab)

# Second: Release Dietetica Report
git add DO.VIVICARE.Report.Dietetica/
git commit -m "Update: Report.Dietetica v1.0.3"
git push origin master
git tag -a plugin/report.dietetica/1.0.3 -m "v1.0.3"
git push origin plugin/report.dietetica/1.0.3

# Both plugins now available in Plugin Manager
```

---

### Scenario 3: Release everything at once (major version)

```bash
# 1. Update all versions in AssemblyInfo.cs files
# App: 2.0.0
# All plugins: 2.0.0

# 2. Commit everything
git add .
git commit -m "Major release: App v2.0.0 + All plugins v2.0.0"
git push origin master

# 3. Release app FIRST (code-signed)
git tag -a v2.0.0 -m "v2.0.0 - Major release"
git push origin v2.0.0

# ⏱️ Wait ~3-5 minutes for GitHub Actions to complete
# ↓ Check: https://github.com/artcava/DO.VIVICARE.Reporting/actions

# 4. Then release each plugin (one by one)
git tag -a plugin/document.adialtaintensita/2.0.0 -m "v2.0.0"
git push origin plugin/document.adialtaintensita/2.0.0

# ⏱️ Wait ~1 minute between each plugin tag

git tag -a plugin/document.adibassaintensita/2.0.0 -m "v2.0.0"
git push origin plugin/document.adibassaintensita/2.0.0

# ... repeat for remaining 15 plugins
```

---

## ✅ **Manifest.json (Plugin Registry) - NO LONGER MANUAL

### Old Workflow (DEPRECATED ❌)
```
Tag v1.2.0
    ↓
GitHub Actions
    ↓
Update manifest.json ← MANUAL STEP (no longer needed)
    ↓
Commit + push manifest
```

### New Workflow (VELOPACK ✅)
```
Tag v1.2.0
    ↓
GitHub Actions
    ↓
Velopack creates full release package
    ↓
Code signing (automatic)
    ↓
Upload to GitHub Release
    ✅ Done!
```

### Manifest.json Now Contains (Plugin Registry Only)
```json
{
  "documents": [
    {
      "id": "document.adialtaintensita",
      "name": "ADI Alta Intensita",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/.../releases/download/..."
    }
    // ... other plugins
  ],
  "reports": [
    // ... report plugins
  ]
}
```

**Application version is determined by:**
- Velopack's internal versioning (based on tag)
- GitHub releases (source of truth)
- NOT by manifest.json ✅

---

## ⚠️ **Troubleshooting

### Problem: "Invalid tag format"

```bash
# ❌ WRONG (4 version numbers)
git tag v1.2.0.0

# ✅ CORRECT (3 version numbers)
git tag v1.2.0

# If you made a mistake:
git tag -d v1.2.0.0              # Delete locally
git push origin :refs/tags/v1.2.0.0   # Delete from GitHub
git tag -a v1.2.0 -m "Message"   # Recreate with correct format
git push origin v1.2.0
```

---

### Problem: "Certificate not found" in GitHub Actions

```bash
# Check secrets are set
gh secret list

# Should see:
# CODESIGN_CERTIFICATE_BASE64
# CODESIGN_PASSWORD

# If missing, add them:
gh secret set CODESIGN_CERTIFICATE_BASE64 --body "<base64>"
gh secret set CODESIGN_PASSWORD --body "<password>"
```

---

### Problem: "GitHub Actions failed"

```bash
# 1. Check Actions tab
https://github.com/artcava/DO.VIVICARE.Reporting/actions

# 2. Click on failed build
# 3. Expand logs to see error details
# 4. Fix the code (e.g., missing DLL file)
# 5. Push fix to master
git add .
git commit -m "Fix: Resolve build error"
git push origin master

# 6. Re-create tag (same format)
git tag -d v1.2.0
git push origin :refs/tags/v1.2.0
git tag -a v1.2.0 -m "Retry"
git push origin v1.2.0
```

---

### Problem: "DLL not found in artifacts"

```bash
# Verify project is building locally first
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release /p:Platform="Any CPU"

# Check DLL exists at expected path
ls "DO.VIVICARE.Reporter/bin/Release/DO.VIVICARE.Reporter.dll"

# Commit and push fix, then retry tag
```

---

## 📊 **Workflow at a Glance

```
APP RELEASE (with Velopack + Signing)    PLUGIN RELEASE
┌─────────────────────────────          ┌─────────────────────
│ 1. Edit AssemblyInfo.cs     │          │ 1. Edit single DLL's         │
│    (UI + Reporter)           │          │    AssemblyInfo.cs           │
│                               │          │                               │
│ 2. git add . && git commit   │          │ 2. git add <dir> && commit  │
│ 3. git push origin master    │          │ 3. git push origin master   │
│                               │          │                               │
│ 4. git tag v1.2.0            │          │ 4. git tag plugin/...       │
│ 5. git push origin v1.2.0    │          │ 5. git push origin tag      │
├─────────────────────────────          ├─────────────────────
│ ↓ AUTOMATIC GitHub Actions  │          │ ↓ AUTOMATIC GitHub Actions  │
│                               │          │                               │
│ ✅ Build & Test              │          │ ✅ Build & Test              │
│ ✅ Create Velopack package   │          │ ✅ Package DLL              │
│ ✅ Import certificate        │          │ ✅ Generate checksum        │
│ ✅ Sign all binaries (SHA256)│          │ ✅ Create Release           │
│ ✅ Generate installer        │          │                               │
│ ✅ Generate delta package    │          │ (No signing for plugins)    │
│ ✅ Generate release notes    │          │                               │
│ ✅ Upload to Release         │          │ ✅ Available in Plugin Mgr  │
│ ✅ Cleanup certificate       │          │                               │
├─────────────────────────────          ├─────────────────────
│ ✅ Installer ready (signed)  │          │ ✅ DLL ready (in release)   │
│ ✅ Velopack handles updates  │          │ ✅ Plugin Mgr auto-detects  │
└─────────────────────────────          └─────────────────────
```

---

## 🔄 **Quick Comparison: Old vs New

| Aspect | Old (ClickOnce) | New (Velopack) |
|--------|-----------------|----------------|
| **Signing** | Manual code signing | Automated in GitHub Actions |
| **Package Format** | ZIP + MSI | Velopack (.nupkg + delta) |
| **Manifest Updates** | Manual edit + commit | Fully automated ✅ |
| **Delta Updates** | Not supported | Supported ✅ (bandwidth saving) |
| **Installer** | ClickOnce-based | Velopack Setup.exe |
| **Release Notes** | Manual writing | Auto-generated |
| **Time to Release** | 15-20 minutes | 3-5 minutes |
| **Rollback** | Manual version management | Velopack handles it |
| **Certificate Mgmt** | Local store management | GitHub Secrets |

---

## 📚 **Related Documentation

- **Installation Guide:** [INSTALLATION_GUIDE.md](INSTALLATION_GUIDE.md)
- **Deployment Strategy:** [DEPLOYMENT.md](DEPLOYMENT.md)
- **CI/CD Workflow:** `.github/workflows/ci-cd.yml`

---

## 🎯 **Summary: The Perfect Release

```bash
# That's it! Three simple steps:
git add .
git commit -m "v1.2.0: Description"
git push origin master
git tag -a v1.2.0 -m "Release notes"
git push origin v1.2.0

# Everything else is AUTOMATIC ✅
# - Code signing
# - Installer generation
# - Release creation
# - Release notes
# - Velopack packaging
# - Delta updates
```

---

**Last Updated:** 26 Gennaio 2026  
**Status:** ✅ Velopack Integration Complete  
**Maintainer:** Marco Cavallo
