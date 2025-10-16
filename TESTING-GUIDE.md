# Unit Testing Guide - Content App POC

## Overview
This project now includes unit testing infrastructure with code coverage reporting using **xUnit**, **Coverlet**, and **ReportGenerator**.

---

## Test Projects

### CommentsMgt.DTOs.Tests
- **Framework**: xUnit
- **Target**: CommentsMgt.DTOs project
- **Coverage**: PaginatedResult<T> class
- **Tests**: 10 comprehensive tests
- **Coverage**: 100% line coverage

### CommentsMgt.Domain.Tests
- **Framework**: xUnit
- **Target**: CommentsMgt.Domain project
- **Coverage**: Comment entity class
- **Tests**: 29 comprehensive tests
- **Coverage**: N/A (POCOs with auto-properties don't generate coverable IL code)

### CommentsMgt.Application.Tests
- **Framework**: xUnit
- **Target**: CommentsMgt.Application project
- **Coverage**: CommentService class
- **Tests**: 18 comprehensive tests
- **Coverage**: 100% line coverage
- **Mocking**: Moq v4.20.72

---

## Running Tests

### Option 1: Quick Test Run (No Coverage)
```powershell
dotnet test
```

### Option 2: Test with Coverage (Console Output)
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

### Option 3: Full Coverage Report (HTML)
```powershell
.\run-coverage.ps1
```
This script will:
1. Clean previous results
2. Run all tests with coverage collection
3. Generate HTML report with ReportGenerator
4. Display summary in console
5. Ask if you want to open the report in browser

---

## Coverage Report Location

After running tests with coverage:
- **HTML Report**: `CoverageReport/index.html`
- **Summary**: `CoverageReport/Summary.txt`
- **Badges**: `CoverageReport/badge_*.svg`
- **Raw Data**: `TestResults/**/coverage.cobertura.xml`

---

## Current Test Coverage

### Tier 1 - Completed ✅
- **PaginatedResult<T>**: 100% coverage (10 tests)

### Tier 2 - Completed ✅
- **Comment Entity**: 100% tested (29 tests) - POCOs don't generate coverable code

### Tier 3 - Completed ✅
- **CommentService**: 100% coverage (18 tests) - Using Moq for mocking

### Tier 4 - Pending
- **CommentRepository**: Not yet implemented
- **CommentsController**: Not yet implemented

---

## Test Structure

### PaginatedResult Tests
Located in: `CommentsMgt.DTOs.Tests/UnitTest1.cs`

**Test Coverage (10 tests):**
1. ✅ Constructor_SetsItems_Correctly
2. ✅ Constructor_SetsTotalCount_Correctly
3. ✅ Constructor_SetsPage_Correctly
4. ✅ Constructor_SetsPageSize_Correctly
5. ✅ Constructor_WithEmptyList_StoresEmptyItems
6. ✅ Constructor_WithNullItems_StoresNull
7. ✅ Constructor_WithComplexType_StoresCorrectly
8. ✅ Constructor_WithZeroPage_StoresZero
9. ✅ Constructor_WithNegativeTotalCount_StoresNegative
10. ✅ Constructor_AllPropertiesSet_AllValuesCorrect

### Comment Entity Tests
Located in: `CommentsMgt.Domain.Tests/UnitTest1.cs`

**Default Value Tests (12 tests):**
1. ✅ NewComment_DefaultId_IsEmptyGuid
2. ✅ NewComment_DefaultShownInPortal_IsFalse
3. ✅ NewComment_DefaultIsDeleted_IsFalse
4. ✅ NewComment_DefaultCreatedBy_IsAnonymous
5. ✅ NewComment_DefaultModifiedBy_IsAnonymous
6. ✅ NewComment_DefaultCreatedOn_IsUtcNow
7. ✅ NewComment_DefaultModifiedOn_IsUtcNow
8. ✅ NewComment_DefaultChildren_IsEmptyList
9. ✅ NewComment_DefaultContentParentAlias_IsEmptyString
10. ✅ NewComment_DefaultCommentText_IsEmptyString
11. ✅ NewComment_DefaultParentId_IsNull
12. ✅ NewComment_DefaultCommentStatus_IsNull

**Property Assignment Tests (13 tests):**
1. ✅ SetId_StoresValue
2. ✅ SetContentId_StoresValue
3. ✅ SetContentParentAlias_StoresValue
4. ✅ SetCommentText_StoresValue
5. ✅ SetCommentStatusId_StoresValue
6. ✅ SetParentId_StoresValue
7. ✅ SetShownInPortal_True_StoresTrue
8. ✅ SetCreatedBy_StoresValue
9. ✅ SetModifiedBy_StoresValue
10. ✅ SetCreatedOn_StoresValue
11. ✅ SetModifiedOn_StoresValue
12. ✅ SetIsDeleted_True_StoresTrue
13. ✅ SetChildren_StoresValue

**Integration Tests (4 tests):**
1. ✅ CreateFullComment_AllPropertiesSet_StoresCorrectly
2. ✅ CreateParentWithChildren_HierarchyEstablished
3. ✅ CommentText_LongText_StoresCorrectly
4. ✅ MultipleComments_DifferentIds_AreUnique

### CommentService Tests
Located in: `CommentsMgt.Application.Tests/UnitTest1.cs`

**GetCommentByIdAsync Tests (3 tests):**
1. ✅ GetCommentByIdAsync_ValidId_ReturnsComment
2. ✅ GetCommentByIdAsync_NonExistentId_ReturnsNull
3. ✅ GetCommentByIdAsync_CallsRepository_Once

**GetCommentsByContentIdAsync Tests (2 tests):**
1. ✅ GetCommentsByContentIdAsync_ValidContentId_ReturnsComments
2. ✅ GetCommentsByContentIdAsync_NoComments_ReturnsEmptyList

**GetAllCommentsAsync Tests (2 tests):**
1. ✅ GetAllCommentsAsync_HasComments_ReturnsAll
2. ✅ GetAllCommentsAsync_NoComments_ReturnsEmptyList

**AddCommentAsync Tests (2 tests):**
1. ✅ AddCommentAsync_ValidComment_CallsRepositoryAdd
2. ✅ AddCommentAsync_NullComment_StillCallsRepository

**UpdateCommentAsync Tests (1 test):**
1. ✅ UpdateCommentAsync_ValidComment_CallsRepositoryUpdate

**DeleteCommentAsync Tests (2 tests):**
1. ✅ DeleteCommentAsync_ValidId_CallsRepositoryDelete
2. ✅ DeleteCommentAsync_EmptyGuid_CallsRepository

**UpdateCommentStatusAsync Tests (2 tests):**
1. ✅ UpdateCommentStatusAsync_ValidData_CallsRepository
2. ✅ UpdateCommentStatusAsync_DifferentStatusIds_CallsRepositoryWithCorrectValues

**CascadeStatusToChildrenAsync Tests (1 test):**
1. ✅ CascadeStatusToChildrenAsync_ValidData_CallsRepository

**GetCommentsByContentIdPagedAsync Tests (2 tests):**
1. ✅ GetCommentsByContentIdPagedAsync_ValidParams_ReturnsPaginatedResult
2. ✅ GetCommentsByContentIdPagedAsync_DifferentPageSizes_CallsRepositoryCorrectly

**Constructor Tests (1 test):**
1. ✅ Constructor_WithValidRepository_CreatesInstance

---

## Tools & Packages

### Installed Tools
- **dotnet-reportgenerator-globaltool** (v5.4.17) - Global tool for HTML reports

### NuGet Packages (Test Projects)
- **xunit** - Testing framework
- **xunit.runner.visualstudio** - Visual Studio test runner
- **coverlet.collector** (v6.0.4) - Coverage data collection
- **coverlet.msbuild** (v6.0.4) - MSBuild integration (DTOs.Tests only)
- **Moq** (v4.20.72) - Mocking framework (Application.Tests only)

---

## Configuration Files

### coverlet.runsettings
Configures coverage collection:
- **Formats**: Cobertura, OpenCover, JSON, lcov
- **Includes**: `[CommentsMgt.*]*`
- **Excludes**: Test projects, migrations, generated files

### run-coverage.ps1
PowerShell script for automated coverage workflow:
- Cleans previous results
- Runs tests with coverage
- Generates HTML report
- Opens report in browser

---

## Next Steps

To continue building test coverage:

1. ✅ **Tier 1**: CommentsMgt.DTOs.Tests - COMPLETED
   - ✅ PaginatedResult<T> tests (10 tests)

2. ✅ **Tier 2**: CommentsMgt.Domain.Tests - COMPLETED
   - ✅ Comment entity tests (29 tests)

3. ✅ **Tier 3**: CommentsMgt.Application.Tests - COMPLETED
   - ✅ CommentService tests with mocked repository (18 tests)
   - ✅ Using Moq for mocking ICommentRepository
   - ✅ All 9 service methods tested with 100% coverage

4. ⏳ **Tier 4**: Create `CommentsMgt.Infra.Tests`
   - Test CommentRepository business logic
   - Use EF Core InMemory database
   - Focus on 4 methods with business rules

5. ⏳ **Tier 5**: Create `CommentsMgt.API.Tests`
   - Test CommentsController
   - Mock ICommentService and IConfiguration

---

## Visual Studio Integration

### Test Explorer
- View → Test Explorer
- Run/Debug individual tests
- View test results

### Live Unit Testing (Enterprise only)
- Test → Live Unit Testing → Start
- Real-time coverage highlighting

### Fine Code Coverage (Free Extension)
- Install from Extensions marketplace
- Shows coverage in editor with color coding

---

## CI/CD Integration

### GitHub Actions Example
```yaml
- name: Run Tests with Coverage
  run: dotnet test --collect:"XPlat Code Coverage"
  
- name: Generate Coverage Report
  run: reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage"
```

### Azure DevOps
- Use `PublishCodeCoverageResults@1` task
- Displays coverage in build summary

---

## Coverage Goals

| Layer | Target | Current |
|-------|--------|---------|
| DTOs | 90-100% | ✅ 100% |
| Domain | 90-100% | ⏳ 0% |
| Application | 80-95% | ⏳ 0% |
| Infrastructure | 70-85% | ⏳ 0% |
| API | 70-80% | ⏳ 0% |

---

## Troubleshooting

### Tests not discovered
```powershell
dotnet clean
dotnet build
dotnet test
```

### Coverage files not found
- Check `TestResults` folder exists
- Verify `coverlet.runsettings` is in root directory
- Ensure `coverlet.collector` package is installed

### Report generation fails
- Verify ReportGenerator is installed: `dotnet tool list -g`
- Reinstall if needed: `dotnet tool install -g dotnet-reportgenerator-globaltool`

---

## Resources

- **xUnit Documentation**: https://xunit.net/
- **Coverlet GitHub**: https://github.com/coverlet-coverage/coverlet
- **ReportGenerator**: https://github.com/danielpalme/ReportGenerator
- **Moq (for mocking)**: https://github.com/moq/moq4

---

**Last Updated**: October 16, 2025
**Test Projects**: 3
**Total Tests**: 57 (10 DTOs + 29 Domain + 18 Application)
**Overall Coverage**: 100% (PaginatedResult + CommentService) + 100% tested (Comment entity)
