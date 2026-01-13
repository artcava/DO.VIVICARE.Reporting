# Testing Guide - DO.VIVICARE.Reporting

## Overview

This project uses a comprehensive testing strategy to ensure quality and reliability:

- **Unit Tests**: `DO.VIVICARE.Tests` - Tests individual components
- **Integration Tests**: `DO.VIVICARE.IntegrationTests` - Tests component interactions
- **CI/CD Pipeline**: GitHub Actions - Automated testing on every commit

## Test Projects

### Unit Tests (`DO.VIVICARE.Tests`)

**Location**: `DO.VIVICARE.Tests/`

**Framework**: xUnit

**Test Classes**:
- `UpdateManagerTests.cs` - Version checking and checksum verification

**Coverage**:
- Version comparison (higher, lower, equal versions)
- Checksum generation and verification
- File integrity validation
- Update availability detection

**Run locally**:
```bash
dotnet test DO.VIVICARE.Tests/DO.VIVICARE.Tests.csproj --configuration Debug
```

### Integration Tests (`DO.VIVICARE.IntegrationTests`)

**Location**: `DO.VIVICARE.IntegrationTests/`

**Framework**: xUnit

**Test Classes**:
- `DeploymentIntegrationTests.cs` - Build artifacts and module loading
- `ModuleDeploymentIntegrationTests.cs` - Document and report modules

**Coverage**:
- Build artifact verification
- Assembly loading and validation
- Dependency resolution
- PE file format validation
- Module deployment

**Run locally**:
```bash
dotnet test DO.VIVICARE.IntegrationTests/DO.VIVICARE.IntegrationTests.csproj --configuration Debug
```

## Running Tests

### Run All Tests

```bash
# Run both unit and integration tests
dotnet test

# With detailed output
dotnet test --verbosity detailed

# Run specific test project only
dotnet test DO.VIVICARE.Tests/DO.VIVICARE.Tests.csproj
```

### Run Specific Test Class

```bash
# Run only UpdateManagerTests
dotnet test --filter "UpdateManagerTests"

# Run only DeploymentIntegrationTests
dotnet test --filter "DeploymentIntegrationTests"
```

### Run Specific Test Method

```bash
# Run only version comparison test
dotnet test --filter "IsUpdateAvailable_WithHigherVersion_ReturnsTrue"
```

### Generate Code Coverage

```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# With coverage output
dotnet test /p:CollectCoverage=true /p:CoverletOutput=./coverage/
```

## GitHub Actions CI/CD Pipeline

**Workflow File**: `.github/workflows/build-and-test.yml`

### Pipeline Stages

1. **Checkout** - Clone repository
2. **Setup** - Install .NET and dependencies
3. **Build** - Compile solution in Release mode
4. **Unit Tests** - Run unit tests (MUST PASS)
5. **Integration Tests** - Run integration tests (MUST PASS)
6. **Create Release** - Only if all tests pass (automatic on version tags)

### Trigger Events

- **Push to master** - Runs tests
- **Pull requests to master** - Runs tests
- **Version tags** (v*) - Runs tests + creates GitHub release

### Example: Creating a Release

```bash
# Create and push a version tag
git tag -a v1.2.0 -m "Release version 1.2.0"
git push origin v1.2.0

# GitHub Actions:
# 1. Builds solution
# 2. Runs all tests
# 3. If tests pass: Creates GitHub Release
# 4. If tests fail: Notifies developer, blocks release
```

## Test Coverage Requirements

### Minimum Coverage Goals

| Component | Minimum Coverage | Target | Status |
|-----------|------------------|--------|--------|
| UpdateManager | 90% | ✅ Unit Tests | To implement |
| DocumentModules | 85% | ✅ Integration Tests | To implement |
| ReportModules | 85% | ✅ Integration Tests | To implement |
| UI Core | 80% | ✅ Unit Tests | Planned |
| PluginManager | 75% | ✅ Integration Tests | Planned |

### Verify Coverage

```bash
# Generate and view coverage report
dotnet test /p:CollectCoverage=true
open coverage/index.html  # macOS
start coverage/index.html  # Windows
```

## Adding New Tests

### Unit Test Template

```csharp
using Xunit;

namespace DO.VIVICARE.Tests
{
    public class MyComponentTests
    {
        [Fact]
        public void MyMethod_WithValidInput_ReturnsExpected()
        {
            // Arrange
            var component = new MyComponent();
            
            // Act
            var result = component.MyMethod("input");
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal("expected", result);
        }
    }
}
```

### Integration Test Template

```csharp
using Xunit;

namespace DO.VIVICARE.IntegrationTests
{
    public class MyModuleIntegrationTests
    {
        [Fact]
        public void MyModule_WithDependencies_LoadsSuccessfully()
        {
            // Arrange
            var assembly = System.Reflection.Assembly.Load("DO.VIVICARE.MyModule");
            
            // Act
            var result = assembly != null;
            
            // Assert
            Assert.True(result);
        }
    }
}
```

### Best Practices

1. **Test Name Format**: `MethodName_Scenario_ExpectedResult`
2. **Use Arrange-Act-Assert**: Clear test structure
3. **One assertion per test**: Keep tests focused
4. **Test both success and failure**: Positive and negative cases
5. **Descriptive messages**: Help identify failures

## Test Failures

### How Tests are Run in CI/CD

```yaml
# Tests MUST PASS (continue-on-error: false)
- name: Run Unit Tests
  run: dotnet test DO.VIVICARE.Tests/DO.VIVICARE.Tests.csproj --configuration Release
  continue-on-error: false  # ← Tests are mandatory

- name: Run Integration Tests
  run: dotnet test DO.VIVICARE.IntegrationTests/DO.VIVICARE.IntegrationTests.csproj --configuration Release
  continue-on-error: false  # ← Tests are mandatory
```

### If Tests Fail

1. **GitHub Actions stops** - No release is created
2. **Developer is notified** - Check Actions tab in GitHub
3. **Pipeline status shown** - Red ❌ on branch
4. **User cannot download broken version** - Protected by CI/CD

### Debugging Failed Tests

```bash
# Run tests with verbose output
dotnet test --verbosity detailed

# Run specific failing test
dotnet test --filter "TestMethodName"

# Run with debugger
dotnet test --collect:"XPlat Code Coverage" --diagnostic:TraceCollector
```

## Local Testing Workflow

### Before Pushing Code

1. **Run unit tests**:
   ```bash
   dotnet test DO.VIVICARE.Tests/
   ```

2. **Run integration tests**:
   ```bash
   dotnet test DO.VIVICARE.IntegrationTests/
   ```

3. **Check coverage**:
   ```bash
   dotnet test /p:CollectCoverage=true
   ```

4. **Push to GitHub** - GitHub Actions will run tests automatically

### Creating a Release

1. **All tests pass locally**:
   ```bash
   dotnet test
   ```

2. **Commit changes**:
   ```bash
   git add .
   git commit -m "feat: add new feature"
   git push origin master
   ```

3. **Create version tag**:
   ```bash
   git tag -a v1.2.0 -m "Release 1.2.0"
   git push origin v1.2.0
   ```

4. **GitHub Actions**:
   - Runs tests on tag push
   - Creates release if tests pass
   - Users can download tested version

## Troubleshooting

### Tests Won't Run

```bash
# Restore NuGet packages
dotnet restore

# Clean solution
dotnet clean

# Rebuild
dotnet build
```

### Assembly Not Found

```bash
# Ensure test projects reference main projects
# In .csproj file, check:
<ProjectReference Include="..\DO.VIVICARE.Reporter\DO.VIVICARE.Reporter.csproj" />
```

### xUnit Not Working

```bash
# Verify NuGet packages
dotnet list package

# Update packages
dotnet add package xunit --version 2.6.6
dotnet add package xunit.runner.visualstudio --version 2.5.4
```

## Test Statistics

### Unit Tests
- **Total Test Methods**: 15
- **Test Classes**: 1 (UpdateManagerTests)
- **Coverage Areas**: 5

### Integration Tests
- **Total Test Methods**: 12
- **Test Classes**: 2 (DeploymentIntegrationTests, ModuleDeploymentIntegrationTests)
- **Coverage Areas**: 6

### Overall
- **Total Tests**: 27
- **Framework**: xUnit 2.6.6
- **Target Framework**: .NET Framework 4.7.2

## Resources

- [xUnit Documentation](https://xunit.net/docs/getting-started/netfx)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET Test Documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/)
- [DEPLOYMENT.md](./DEPLOYMENT.md) - Deployment strategy with testing requirements

## Next Steps

### Immediate
- [ ] Review test structure locally
- [ ] Run tests: `dotnet test`
- [ ] Verify all tests pass

### This Week
- [ ] Add more unit tests for UpdateManager
- [ ] Implement missing integration tests
- [ ] Achieve 80%+ code coverage

### Next Week
- [ ] Create first version tag
- [ ] Verify GitHub Actions workflow runs
- [ ] Test release creation process
- [ ] Verify users can download release

---

**Status**: Testing infrastructure ready ✅
**Framework**: xUnit 2.6.6 ✅
**CI/CD**: GitHub Actions configured ✅
**Coverage**: 27 tests implemented ✅
