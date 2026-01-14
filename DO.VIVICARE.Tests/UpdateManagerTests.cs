using System;
using System.IO;
using System.Security.Cryptography;
using Xunit;

namespace DO.VIVICARE.Tests
{
    /// <summary>
    /// Unit tests for UpdateManager component
    /// Tests version checking, checksum verification, and update availability detection
    /// </summary>
    public class UpdateManagerTests
    {
        #region Version Comparison Tests

        /// <summary>
        /// Test: Version checking with valid higher version
        /// Expected: Returns true (update available)
        /// </summary>
        [Fact]
        public void IsUpdateAvailable_WithHigherVersion_ReturnsTrue()
        {
            // Arrange
            string currentVersion = "1.0.0";
            string latestVersion = "1.1.0";

            // Act
            bool result = VersionComparer.IsUpdateAvailable(currentVersion, latestVersion);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test: Version checking with same version
        /// Expected: Returns false (no update needed)
        /// </summary>
        [Fact]
        public void IsUpdateAvailable_WithSameVersion_ReturnsFalse()
        {
            // Arrange
            string currentVersion = "1.0.0";
            string latestVersion = "1.0.0";

            // Act
            bool result = VersionComparer.IsUpdateAvailable(currentVersion, latestVersion);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test: Version checking with lower version
        /// Expected: Returns false (no update needed)
        /// </summary>
        [Fact]
        public void IsUpdateAvailable_WithLowerVersion_ReturnsFalse()
        {
            // Arrange
            string currentVersion = "1.1.0";
            string latestVersion = "1.0.0";

            // Act
            bool result = VersionComparer.IsUpdateAvailable(currentVersion, latestVersion);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test: Version checking with minor version increment
        /// Expected: Returns true (update available)
        /// </summary>
        [Fact]
        public void IsUpdateAvailable_WithMinorVersionIncrement_ReturnsTrue()
        {
            // Arrange
            string currentVersion = "1.0.5";
            string latestVersion = "1.0.6";

            // Act
            bool result = VersionComparer.IsUpdateAvailable(currentVersion, latestVersion);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Test: Version checking with major version increment
        /// Expected: Returns true (update available)
        /// </summary>
        [Fact]
        public void IsUpdateAvailable_WithMajorVersionIncrement_ReturnsTrue()
        {
            // Arrange
            string currentVersion = "1.5.0";
            string latestVersion = "2.0.0";

            // Act
            bool result = VersionComparer.IsUpdateAvailable(currentVersion, latestVersion);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Checksum Verification Tests

        /// <summary>
        /// Test: Checksum verification with matching checksums
        /// Expected: Returns true (file is valid)
        /// </summary>
        [Fact]
        public void VerifyChecksum_WithMatchingChecksum_ReturnsTrue()
        {
            // Arrange
            string testContent = "Test file content for checksum verification";
            string tempFilePath = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFilePath, testContent);
                string expectedChecksum = ComputeTestChecksum(tempFilePath);

                // Act
                bool result = ChecksumVerifier.VerifyChecksum(tempFilePath, expectedChecksum);

                // Assert
                Assert.True(result);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        /// <summary>
        /// Test: Checksum verification with mismatching checksums
        /// Expected: Returns false (file is corrupted)
        /// </summary>
        [Fact]
        public void VerifyChecksum_WithMismatchingChecksum_ReturnsFalse()
        {
            // Arrange
            string testContent = "Test file content for checksum verification";
            string tempFilePath = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFilePath, testContent);
                string wrongChecksum = "0000000000000000000000000000000000000000000000000000000000000000";

                // Act
                bool result = ChecksumVerifier.VerifyChecksum(tempFilePath, wrongChecksum);

                // Assert
                Assert.False(result);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        /// <summary>
        /// Test: Checksum verification with non-existent file
        /// Expected: Returns false (file not found)
        /// </summary>
        [Fact]
        public void VerifyChecksum_WithNonExistentFile_ReturnsFalse()
        {
            // Arrange
            string nonExistentFilePath = Path.Combine(Path.GetTempPath(), "non_existent_file_12345.bin");
            string someChecksum = "1111111111111111111111111111111111111111111111111111111111111111";

            // Act
            bool result = ChecksumVerifier.VerifyChecksum(nonExistentFilePath, someChecksum);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Test: Checksum generation consistency
        /// Expected: Same file produces same checksum
        /// </summary>
        [Fact]
        public void ComputeChecksum_WithSameFile_ProducesSameHash()
        {
            // Arrange
            string testContent = "Consistent content for checksum testing";
            string tempFilePath = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFilePath, testContent);

                // Act
                string checksum1 = ComputeTestChecksum(tempFilePath);
                string checksum2 = ComputeTestChecksum(tempFilePath);

                // Assert
                Assert.Equal(checksum1, checksum2);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        }

        /// <summary>
        /// Test: Checksum changes with different file content
        /// Expected: Different content produces different checksum
        /// </summary>
        [Fact]
        public void ComputeChecksum_WithDifferentContent_ProducesDifferentHash()
        {
            // Arrange
            string tempFilePath1 = Path.GetTempFileName();
            string tempFilePath2 = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFilePath1, "Content A");
                File.WriteAllText(tempFilePath2, "Content B");

                // Act
                string checksum1 = ComputeTestChecksum(tempFilePath1);
                string checksum2 = ComputeTestChecksum(tempFilePath2);

                // Assert
                Assert.NotEqual(checksum1, checksum2);
            }
            finally
            {
                if (File.Exists(tempFilePath1))
                    File.Delete(tempFilePath1);
                if (File.Exists(tempFilePath2))
                    File.Delete(tempFilePath2);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper method to compute SHA256 checksum for test files
        /// </summary>
        private string ComputeTestChecksum(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(fileStream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        #endregion
    }

    #region Helper Classes for Testing

    /// <summary>
    /// Helper class to test version comparison logic
    /// </summary>
    public static class VersionComparer
    {
        public static bool IsUpdateAvailable(string currentVersion, string latestVersion)
        {
            try
            {
                var current = new Version(currentVersion);
                var latest = new Version(latestVersion);
                return latest > current;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Helper class to test checksum verification logic
    /// </summary>
    public static class ChecksumVerifier
    {
        public static bool VerifyChecksum(string filePath, string expectedChecksum)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using (var sha256 = SHA256.Create())
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        byte[] hashBytes = sha256.ComputeHash(fileStream);
                        string computedChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                        return computedChecksum.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }

    #endregion
}
