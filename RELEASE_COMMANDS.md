# Release Commands - Quick Reference

## 🎯 **Release Application (v1.2.0, v1.2.1, etc.)**

### When to Use
When you update **UI or Reporter** and want to release a new app version

### Commands

```bash
# 1. Update version numbers in AssemblyInfo.cs
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
git tag -a v1.2.0 -m "Release version 1.2.0"
git push origin v1.2.0
```

### Result
✅ GitHub Actions automatically:
- Builds UI + Reporter
- Creates `DO.VIVICARE-Setup-1.2.0.zip`
- Generates `CHECKSUM.txt`
- Creates GitHub Release with both files

### Release URL
```
https://github.com/artcava/DO.VIVICARE.Reporting/releases/tag/v1.2.0
```

---

## 📄 **Release Single Plugin**

### When to Use
When you update **ONE plugin library** (Document or Report) and want to release independently

### Examples

#### Example 1: ADI Alta Intensita Document Module

```bash
# 1. Update version in AssemblyInfo.cs
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

**Result:**
- GitHub Actions builds only this DLL
- Creates `DO.VIVICARE.Document.ADIAltaIntensita-1.0.5.dll`
- Available for download in Plugin Manager

---

#### Example 2: Dietetica Report Module

```bash
# 1. Update version
# DO.VIVICARE.Report.Dietetica/Properties/AssemblyInfo.cs
# [assembly: AssemblyVersion("1.0.2.0")]

# 2. Commit
git add DO.VIVICARE.Report.Dietetica/
git commit -m "Update: Report.Dietetica v1.0.2 - New formulas"

# 3. Push
git push origin master

# 4. Tag
git tag -a plugin/report.dietetica/1.0.2 -m "Dietetica v1.0.2"

# 5. Push tag
git push origin plugin/report.dietetica/1.0.2
```

---

## 📦 **Plugin ID Reference**

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
plugin/document.zsdfatture/VERSION
```

### Report Plugins
```
plugin/report.allegatoadi/VERSION
plugin/report.dietetica/VERSION
plugin/report.valorizzazione/VERSION
```

---

## 💭 **Common Scenarios**

### Scenario 1: Fix bug in UI

```bash
# Edit UI code
# ...

# 1. Update UI version
# DO.VIVICARE.UI/Properties/AssemblyInfo.cs
# [assembly: AssemblyVersion("1.2.1.0")]

# 2. Commit
git add .
git commit -m "v1.2.1: Fix login bug"
git push origin master

# 3. Release
git tag -a v1.2.1 -m "v1.2.1 - Bug fix"
git push origin v1.2.1

# Result: New installer available with bug fix
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

### Scenario 3: Release everything at once (multiple plugins + app)

```bash
# 1. Update all versions in AssemblyInfo.cs files
# App: 1.3.0
# All plugins: 2.0.0

# 2. Commit everything
git add .
git commit -m "Major release: App v1.3.0 + All plugins v2.0.0"
git push origin master

# 3. Release app first
git tag -a v1.3.0 -m "v1.3.0"
git push origin v1.3.0

# Wait ~2 minutes for GitHub Actions

# 4. Then release each plugin
git tag -a plugin/document.adialtaintensita/2.0.0 -m "v2.0.0"
git push origin plugin/document.adialtaintensita/2.0.0

git tag -a plugin/document.adibassaintensita/2.0.0 -m "v2.0.0"
git push origin plugin/document.adibassaintensita/2.0.0

# ... repeat for all 16 plugins
```

---

## ⚠️ **Troubleshooting**

### Problem: "Push rejected" on tag

```bash
# Solution: Tag may already exist
git tag -d v1.2.0                    # Delete locally
git push origin :refs/tags/v1.2.0   # Delete from GitHub
git tag -a v1.2.0 -m "Message"     # Recreate
git push origin v1.2.0
```

---

### Problem: Need to fix tag message

```bash
# For app tag
git tag -d v1.2.0
git push origin :refs/tags/v1.2.0
git tag -a v1.2.0 -m "Corrected message"
git push origin v1.2.0

# For plugin tag
git tag -d plugin/document.adialtaintensita/1.0.5
git push origin :refs/tags/plugin/document.adialtaintensita/1.0.5
git tag -a plugin/document.adialtaintensita/1.0.5 -m "Corrected"
git push origin plugin/document.adialtaintensita/1.0.5
```

---

### Problem: GitHub Actions failed

```bash
# 1. Check Actions tab
https://github.com/artcava/DO.VIVICARE.Reporting/actions

# 2. Click on failed build
# 3. Expand logs to see error
# 4. Fix the code
# 5. Push fix to master
# 6. Re-create tag with same name (delete + recreate)
git tag -d v1.2.0
git push origin :refs/tags/v1.2.0
git tag -a v1.2.0 -m "Retry"
git push origin v1.2.0
```

---

## 📑 **Workflow at a Glance**

```
APP RELEASE                          PLUGIN RELEASE
┌─────────────────────          ┌─────────────────────
│ 1. Edit AssemblyInfo.cs   │          │ 1. Edit single DLL          │
│    (UI + Reporter)         │          │    AssemblyInfo.cs          │
│                             │          │                              │
│ 2. git commit              │          │ 2. git commit (single dir) │
│                             │          │                              │
│ 3. git push origin master  │          │ 3. git push origin master  │
│                             │          │                              │
│ 4. git tag v1.2.0          │          │ 4. git tag plugin/...      │
│                             │          │                              │
│ 5. git push origin v1.2.0  │          │ 5. git push origin tag     │
├─────────────────────          ├─────────────────────
│ ↓ GitHub Actions          │          │ ↓ GitHub Actions           │
│                             │          │                              │
│ ✅ Builds UI + Reporter     │          │ ✅ Builds single DLL         │
│ ✅ Creates MSI/ZIP           │          │ ✅ Creates DLL               │
│ ✅ Generates checksum       │          │ ✅ Generates checksum        │
│ ✅ Creates Release          │          │ ✅ Creates Release           │
├─────────────────────          ├─────────────────────
│ ✅ Available to users       │          │ ✅ Available in Plugin Mgr  │
└─────────────────────          └─────────────────────
```

---

## 📚 **Full Documentation**

For complete details, see:
- `INSTALLATION_GUIDE.md` - User & developer docs
- `NEXT_STEPS.md` - Implementation roadmap
- `.github/workflows/release.yml` - CI/CD workflow

---

**Last Updated**: 11 Gennaio 2026
