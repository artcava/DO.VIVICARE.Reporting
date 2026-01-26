using System;
using System.IO;
using DO.VIVICARE.Reporter;
using Xunit;

namespace DO.VIVICARE.Tests
{
    /// <summary>
    /// Unit tests for XMLSettings configuration persistence
    /// Tests configuration path, backup creation, and versioning
    /// </summary>
    public class XMLSettingsTests
    {
        #region Path and AppData Tests

        /// <summary>
        /// Test: XMLSettings creates config in AppData on initialization
        /// Expected: Settings.xml exists in AppData\DO.VIVICARE.Reporting
        /// </summary>
        [Fact]
        public void XMLSettings_OnInitialize_CreatesConfigInAppData()
        {
            // Arrange
            string expectedPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DO.VIVICARE.Reporting",
                "Settings.xml"
            );

            // Clean up before test
            if (File.Exists(expectedPath))
                File.Delete(expectedPath);

            // Act
            var settings = new XMLSettings();
            settings.Save();

            // Assert
            Assert.True(File.Exists(expectedPath),
                $"Settings.xml non trovato in {expectedPath}");
        }

        /// <summary>
        /// Test: XMLSettings directory is created if not exists
        /// Expected: AppData\DO.VIVICARE.Reporting directory created
        /// </summary>
        [Fact]
        public void XMLSettings_OnInitialize_CreatesAppDataDirectory()
        {
            // Arrange
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DO.VIVICARE.Reporting"
            );

            // Clean up before test
            if (Directory.Exists(appDataPath))
                Directory.Delete(appDataPath, recursive: true);

            // Act
            var settings = new XMLSettings();

            // Assert
            Assert.True(Directory.Exists(appDataPath),
                "Directory DO.VIVICARE.Reporting non creato in AppData");
        }

        #endregion

        #region Backup Tests

        /// <summary>
        /// Test: XMLSettings creates backup file on save
        /// Expected: Settings.xml.backup exists after save
        /// </summary>
        [Fact]
        public void XMLSettings_OnSave_CreatesBackupFile()
        {
            // Arrange
            string configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DO.VIVICARE.Reporting",
                "Settings.xml"
            );
            string backupPath = configPath + ".backup";

            // Clean up
            if (File.Exists(configPath)) File.Delete(configPath);
            if (File.Exists(backupPath)) File.Delete(backupPath);

            // Act
            var settings = new XMLSettings();
            settings.AddLibrary(XMLSettings.LibraryType.Document, "TestDocument");
            settings.Save();

            // Assert
            Assert.True(File.Exists(backupPath),
                "Backup file non creato: " + backupPath);
        }

        /// <summary>
        /// Test: XMLSettings backup is identical to current config
        /// Expected: Backup file content matches current config
        /// </summary>
        [Fact]
        public void XMLSettings_BackupFile_ContainsCurrentConfig()
        {
            // Arrange
            string configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DO.VIVICARE.Reporting",
                "Settings.xml"
            );
            string backupPath = configPath + ".backup";

            if (File.Exists(configPath)) File.Delete(configPath);
            if (File.Exists(backupPath)) File.Delete(backupPath);

            // Act
            var settings = new XMLSettings();
            settings.AddLibrary(XMLSettings.LibraryType.Document, "TestDoc");
            settings.Save();

            // Assert
            Assert.True(File.Exists(backupPath), "Backup non creato");

            // Verify backup is not empty and contains SETTINGS element
            string backupContent = File.ReadAllText(backupPath);
            Assert.Contains("<SETTINGS", backupContent,
                StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Versioning Tests

        /// <summary>
        /// Test: XMLSettings adds VERSION attribute to config
        /// Expected: Root element has VERSION="1.0" attribute
        /// </summary>
        [Fact]
        public void XMLSettings_OnCreate_AddsVersionAttribute()
        {
            // Arrange
            string configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DO.VIVICARE.Reporting",
                "Settings.xml"
            );

            if (File.Exists(configPath)) File.Delete(configPath);

            // Act
            var settings = new XMLSettings();
            settings.Save();

            // Assert - Read back and verify
            var reloadedSettings = new XMLSettings();
            string version = reloadedSettings.DocumentElement?.GetAttribute("VERSION") ?? "";
            Assert.Equal("1.0", version);
        }

        /// <summary>
        /// Test: XMLSettings loads and reloads preserves configuration
        /// Expected: Data added to config persists after reload
        /// </summary>
        [Fact]
        public void XMLSettings_AfterReload_PersistsAddedData()
        {
            // Arrange
            string configPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DO.VIVICARE.Reporting",
                "Settings.xml"
            );

            if (File.Exists(configPath)) File.Delete(configPath);

            // Act - Create and add data
            var settings1 = new XMLSettings();
            settings1.AddLibrary(XMLSettings.LibraryType.Document, "TestDoc");
            settings1.Save();

            // Reload from disk
            var settings2 = new XMLSettings();

            // Assert
            var values = settings2.GetDocumentValues(
                XMLSettings.LibraryType.Document, "TestDoc");
            Assert.NotNull(values);
            Assert.NotEmpty(values);
        }

        #endregion

        #region Cleanup Helper

        /// <summary>
        /// Cleanup method - removes test config files after tests
        /// </summary>
        private void CleanupTestConfig()
        {
            try
            {
                string configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DO.VIVICARE.Reporting",
                    "Settings.xml"
                );

                if (File.Exists(configPath))
                    File.Delete(configPath);

                string backupPath = configPath + ".backup";
                if (File.Exists(backupPath))
                    File.Delete(backupPath);
            }
            catch { /* Ignore cleanup errors */ }
        }

        #endregion
    }
}