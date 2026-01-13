# Troubleshooting Guide - DO.VIVICARE Reporting

Common issues, solutions, and best practices for using DO.VIVICARE Reporting.

## Table of Contents

- [Installation & Setup](#installation--setup)
- [Plugin Management](#plugin-management)
- [Report Generation](#report-generation)
- [Performance](#performance)
- [File & Output Issues](#file--output-issues)
- [Configuration Problems](#configuration-problems)
- [Frequently Asked Questions](#frequently-asked-questions)
- [Getting Help](#getting-help)

---

## Installation & Setup

### Application Won't Start

**Symptoms:**
- Crashes immediately after launching
- No error message displayed
- "DO.VIVICARE.UI.exe has stopped working"

**Causes & Solutions:**

1. **Missing .NET Framework**
   ```bash
   # Check installed .NET Framework versions
   reg query "HKLM\Software\Microsoft\NET Framework Setup\NDP\v4" /s
   ```
   - Verify .NET Framework 4.6.1+ is installed
   - Download from [Microsoft .NET Framework](https://dotnet.microsoft.com/download/dotnet-framework)
   - Install with administrative privileges

2. **Corrupted Installation**
   - Uninstall: Control Panel > Programs > Uninstall a program
   - Delete leftover files: `C:\Program Files\DO.VIVICARE\`
   - Delete config: `%APPDATA%\DO.VIVICARE\`
   - Reinstall from latest MSI

3. **Insufficient Permissions**
   - Run as Administrator
   - Check folder permissions: `C:\Program Files\DO.VIVICARE\`
   - Ensure write access to `%APPDATA%\DO.VIVICARE\`

4. **Review Application Log**
   ```
   Location: %APPDATA%\DO.VIVICARE\logs\DO.VIVICARE.log
   ```
   - Look for exception details
   - Search for "Error" or "Exception"
   - Copy error messages for support

### Installation Fails

**Symptoms:**
- "MSI installation failed"
- "Error 1602: User cancelled installation"
- "Access denied" during installation

**Solutions:**

1. **Check Disk Space**
   - Minimum required: 500 MB
   - Recommended: 1 GB available
   - Free up disk space before retrying

2. **Administrator Privileges**
   - Right-click MSI > "Run as administrator"
   - User account must have admin rights

3. **Antivirus Interference**
   - Temporarily disable antivirus
   - Add MSI to antivirus whitelist
   - Try again after whitelisting

4. **Failed Dependencies**
   ```bash
   # Check for required components
   wmic product where "name like '%NET Framework%'" list brief
   ```
   - Install missing .NET Framework versions
   - Restart computer after installation

---

## Plugin Management

### Plugins Not Appearing

**Symptoms:**
- Plugin Manager shows "No available plugins"
- Expected plugins are missing
- "Unable to fetch manifest" error

**Solutions:**

1. **Check Internet Connection**
   - Verify working internet connection
   - Test: `ping github.com`
   - Check firewall/proxy settings

2. **Clear Cache**
   ```bash
   # Delete plugin cache
   rmdir /s "%APPDATA%\DO.VIVICARE\cache"
   ```
   - Restart application
   - Plugin Manager will refresh

3. **Verify Manifest Accessibility**
   - Check: https://github.com/artcava/DO.VIVICARE.Reporting/releases
   - Verify GitHub is accessible from your network
   - Check proxy/firewall rules

4. **Network Issues**
   - Check corporate firewall rules
   - Verify proxy settings in Windows
   - Disable VPN temporarily to test

### Plugin Download Fails

**Symptoms:**
- "Download failed" error
- Plugin installation stalls/hangs
- Incomplete plugin file

**Solutions:**

1. **Verify Disk Space**
   ```bash
   # Check available space
   dir C:\ | find /i "volume"
   ```
   - Ensure at least 1 GB available
   - Free space in temp folder

2. **Check Plugin Directory Permissions**
   ```bash
   # Default location
   C:\Program Files\DO.VIVICARE\Plugins\
   ```
   - Right-click folder > Properties > Security
   - Verify user has "Modify" permission
   - Reset permissions if needed

3. **Retry Download**
   - Click [Download] button again
   - Wait for completion (monitor network activity)
   - Check logs: `%APPDATA%\DO.VIVICARE\logs\`

4. **Manual Download**
   - Visit GitHub Releases
   - Download DLL manually
   - Place in: `C:\Program Files\DO.VIVICARE\Plugins\`
   - Restart application

### Plugin Not Loading

**Symptoms:**
- Plugin installed but not available in report generation
- "Plugin failed to load" message
- Report type not appearing in UI

**Solutions:**

1. **Restart Application**
   - Close DO.VIVICARE.UI completely
   - Wait 5 seconds
   - Reopen application
   - Plugin should load

2. **Verify Plugin Compatibility**
   - Check plugin version vs app version
   - See INSTALLATION.md for version matrix
   - Some plugins require minimum app version

3. **Check Plugin Integrity**
   ```bash
   # Verify DLL checksum
   certutil -hashfile "C:\Program Files\DO.VIVICARE\Plugins\Plugin.dll" SHA256
   ```
   - Compare with GitHub release notes
   - If mismatched, re-download

4. **Review Application Log**
   ```
   %APPDATA%\DO.VIVICARE\logs\DO.VIVICARE.log
   ```
   - Search for plugin name
   - Look for load errors
   - Copy error details for support

---

## Report Generation

### Report Generation Times Out

**Symptoms:**
- "Operation timed out" message
- Report generation never completes
- Application freezes

**Solutions:**

1. **Reduce Data Range**
   - Select smaller date range
   - Generate for one month instead of year
   - Test with smaller dataset first

2. **Close Other Applications**
   - Minimize memory usage
   - Close unnecessary programs
   - Check Task Manager for resource hogs
   ```bash
   tasklist /v | sort /+71
   ```

3. **Check Available Resources**
   - RAM: Minimum 2 GB, Recommended 4+ GB
   - CPU: Monitor CPU usage (should not exceed 90%)
   - Disk: Free space for temp files

4. **Increase Timeout** (advanced)
   - Edit: `%APPDATA%\DO.VIVICARE\config.json`
   - Set: `"generationTimeoutSeconds": 600` (10 minutes)
   - Restart application

### Report Generation Fails

**Symptoms:**
- "Report generation failed" error
- No output file created
- Partial or corrupted Excel file

**Solutions:**

1. **Verify Input Data**
   - Check data source accessibility
   - Verify file format (CSV, Excel, DB)
   - Ensure no special characters causing issues
   - Test with sample data first

2. **Check Output Directory**
   ```bash
   # Verify folder exists and permissions
   icacls "C:\Reports"
   ```
   - Verify output directory exists
   - Check user has write permissions
   - Ensure path is valid (no special chars)

3. **Review Error Details**
   - Check logs: `%APPDATA%\DO.VIVICARE\logs\`
   - Look for exception details
   - Note the exact error message

4. **Free Disk Space**
   - Excel generation requires temp space
   - Ensure 2-3x report size available
   - Clean up temp folders:
   ```bash
   del /F /Q %TEMP%\*
   ```

### Excel File Corruption

**Symptoms:**
- "The file is corrupted and cannot be opened"
- Missing or scrambled data in Excel
- Formulas showing as text

**Solutions:**

1. **Update EPPlus Library**
   - Open Solution in Visual Studio
   - Tools > NuGet Package Manager > Manage Packages
   - Update EPPlus to latest 4.5.x version
   - Rebuild and test

2. **Regenerate Report**
   - Delete corrupted output file
   - Generate report again
   - Ensure no interruptions during generation

3. **Try Different Output Location**
   - Network drive issues?
   - Try: `C:\Reports\` instead of network share
   - Verify network share is accessible

4. **Excel Repair**
   - Open Excel
   - File > Open
   - Select corrupted file
   - Choose "Open and Repair"
   - Excel may recover some data

---

## Performance

### Report Generation is Slow

**Expected Performance:**
- Small report (<5K rows): 5-10 seconds
- Medium report (5-50K rows): 15-45 seconds
- Large report (50K+ rows): 1-3 minutes

**If exceeding these, try:**

1. **Reduce Data Scope**
   - Generate for shorter time period
   - Filter by specific criteria
   - Process data in batches

2. **Optimize System Resources**
   - Close unnecessary applications
   - Disable real-time antivirus scanning
   - Increase virtual memory/pagefile

3. **Check Disk Performance**
   ```bash
   # Disk performance test
   diskperf -y
   ```
   - Run on SSD (faster than HDD)
   - Check for disk fragmentation
   - Avoid network drives for temp data

4. **Monitor Resource Usage**
   - Open Task Manager
   - Watch CPU, RAM, Disk during generation
   - Identify resource bottleneck
   - Upgrade if needed

### High Memory Usage

**Symptoms:**
- Task Manager shows >80% RAM usage
- Application slows down during generation
- System becomes unresponsive

**Solutions:**

1. **Reduce Report Scope**
   - Process smaller date ranges
   - Limit to specific report types
   - Generate multiple smaller reports

2. **Increase Available RAM**
   - Close other applications
   - Disable browser tabs/extensions
   - Increase virtual memory:
   ```bash
   # Check current pagefile
   wmic pagefile list
   ```

3. **Enable Large Address Aware** (technical)
   - Recompile with /LARGEADDRESSAWARE flag
   - Requires developer modification

---

## File & Output Issues

### Cannot Save Report to Network Drive

**Symptoms:**
- "Access denied" when saving
- Network path not found
- Timeout during save

**Solutions:**

1. **Verify Network Credentials**
   ```bash
   # Test network connection
   net use Z: \\\\server\\share /user:domain\\username password
   ```
   - Ensure mapped drive is accessible
   - Check credentials have write permissions

2. **Use Local Path Instead**
   - Save to `C:\Reports\` initially
   - Copy to network drive after
   - Verify local save works first

3. **Check Network Speed**
   - Large files over slow network
   - Test with smaller report first
   - Consider local generation + copy

### Report File is Read-Only

**Symptoms:**
- Cannot modify report after generation
- Excel shows "Read-Only" in title bar
- File has lock file (.xlsx~)

**Solutions:**

1. **Remove Read-Only Flag**
   - Right-click file > Properties
   - Uncheck "Read-only"
   - Click Apply > OK

2. **Check File Locks**
   ```bash
   # Find process holding file
   openfiles /query | find "report.xlsx"
   ```
   - Close any open copies
   - Ensure no other users have file open

3. **Save As New File**
   - Open report in Excel
   - File > Save As
   - Save as new filename
   - Delete read-only original

---

## Configuration Problems

### Configuration File Missing

**Symptoms:**
- "config.json not found" error
- Settings not persisting between sessions
- Unable to load previous configuration

**Solutions:**

1. **Restore Config Directory**
   ```bash
   # Location should be
   %APPDATA%\DO.VIVICARE\config.json
   ```
   - Check folder exists
   - Create if missing: `mkdir "%APPDATA%\DO.VIVICARE"`

2. **Reset Configuration**
   - Delete: `%APPDATA%\DO.VIVICARE\config.json`
   - Restart application
   - Re-enter settings

3. **Verify Permissions**
   ```bash
   icacls "%APPDATA%\DO.VIVICARE"
   ```
   - User must have Modify permissions
   - Reset permissions if needed

### Invalid Configuration Format

**Symptoms:**
- "Invalid configuration format" error
- Settings not loading
- Application crash on startup

**Solutions:**

1. **Backup and Reset**
   ```bash
   # Backup current config
   copy "%APPDATA%\DO.VIVICARE\config.json" "backup.json"
   ```
   - Delete corrupted config
   - Restart to create fresh config

2. **Validate JSON Format**
   - Use online JSON validator: https://jsonlint.com
   - Check for missing brackets, commas
   - Fix syntax errors

3. **Compare with Template**
   - Check GitHub repo for config.json.template
   - Compare your file
   - Copy valid structure

---

## Frequently Asked Questions

### Q: What's the maximum report size supported?
**A:** Excel 2007+ format (.xlsx) supports ~1 million rows. Practical limit is 500K rows before performance degrades significantly. For larger datasets, split into multiple reports.

### Q: Can I run multiple reports simultaneously?
**A:** Not recommended. Process reports sequentially to avoid resource contention and potential file locks. Current architecture not optimized for concurrent generation.

### Q: How do I backup my configurations?
**A:** Config stored in `%APPDATA%\DO.VIVICARE\config.json`. Copy this file to backup location. Keep multiple versions if making frequent changes.

### Q: Is there a command-line interface?
**A:** Currently, no CLI. Reports must be generated through UI. Future roadmap includes REST API for programmatic access.

### Q: Can I use this on Mac/Linux?
**A:** No. DO.VIVICARE Reporting requires Windows (.NET Framework) and Excel support. Future modernization (.NET 6+) may enable cross-platform support.

### Q: How often should I update plugins?
**A:** Check monthly. Plugin Manager can check for updates automatically. Security and bug fixes recommended immediately. Feature updates at your discretion.

### Q: What if report data is confidential?
**A:** Ensure:
- Output directory has restricted access
- Disable temporary file caching
- Use NTFS encryption for sensitive data
- See DEPLOYMENT.md for security recommendations

---

## Getting Help

### Check These Resources First

1. **Application Logs**
   ```
   %APPDATA%\DO.VIVICARE\logs\DO.VIVICARE.log
   ```

2. **Documentation**
   - [README.md](README.md) - Overview
   - [INSTALLATION.md](INSTALLATION.md) - Setup
   - [ARCHITECTURE.md](ARCHITECTURE.md) - Technical details

3. **GitHub Issues**
   - Search [existing issues](https://github.com/artcava/DO.VIVICARE.Reporting/issues)
   - Similar problem might be already solved

### Report an Issue

1. **Gather Information**
   ```
   - Exact error message
   - Steps to reproduce
   - System info: Windows version, .NET version
   - Application log excerpt
   - Screenshots if helpful
   ```

2. **Create GitHub Issue**
   - Go to [Issues](https://github.com/artcava/DO.VIVICARE.Reporting/issues)
   - Click "New Issue"
   - Follow template
   - Attach logs or screenshots

3. **Contact Support**
   - Email: mcavallo@welol.it
   - Include issue number from GitHub
   - Attach application log and screenshots

### Ask in Discussions

For questions and feature requests:
- [GitHub Discussions](https://github.com/artcava/DO.VIVICARE.Reporting/discussions)
- Search existing discussions first
- Post in appropriate category

---

**Last Updated**: January 13, 2026  
**Maintained by**: Marco Cavallo
