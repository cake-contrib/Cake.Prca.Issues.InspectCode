namespace Cake.Prca.Issues.InspectCode.Tests
{
    using System.IO;
    using System.Text;
    using Core.IO;
    using Shouldly;
    using Xunit;

    public class InspectCodeSettingsTests
    {
        public sealed class TheInspectCodeSettings
        {
            [Fact]
            public void Should_Throw_If_LogFilePath_Is_Null()
            {
                // Given / When
                var result = Record.Exception(() => 
                    InspectCodeSettings.FromFilePath(
                        null,
                        new DirectoryPath(@"C:\")));

                // Then
                result.IsArgumentNullException("logFilePath");
            }

            [Fact]
            public void Should_Throw_If_RepositoryRoot_For_LogFilePath_Is_Null()
            {
                // Given / When
                var result = Record.Exception(() =>
                    InspectCodeSettings.FromFilePath(
                            new FilePath(@"C:\foo.log"),
                            null));

                // Then
                result.IsArgumentNullException("repositoryRoot");
            }

            [Fact]
            public void Should_Throw_If_LogFileContent_Is_Null()
            {
                // Given / When
                var result = Record.Exception(() =>
                    InspectCodeSettings.FromContent(
                        null,
                        new DirectoryPath(@"C:\")));

                // Then
                result.IsArgumentNullException("logFileContent");
            }

            [Fact]
            public void Should_Throw_If_LogFileContent_Is_Empty()
            {
                // Given / When
                var result = Record.Exception(() =>
                    InspectCodeSettings.FromContent(
                        string.Empty,
                        new DirectoryPath(@"C:\")));

                // Then
                result.IsArgumentOutOfRangeException("logFileContent");
            }

            [Fact]
            public void Should_Throw_If_LogFileContent_Is_WhiteSpace()
            {
                // Given / When
                var result = Record.Exception(() =>
                    InspectCodeSettings.FromContent(
                        " ",
                        new DirectoryPath(@"C:\")));

                // Then
                result.IsArgumentOutOfRangeException("logFileContent");
            }

            [Fact]
            public void Should_Throw_If_RepositoryRoot_For_LogFileContent_Is_Null()
            {
                // Given / When
                var result = Record.Exception(() =>
                    InspectCodeSettings.FromContent(
                        "foo",
                        null));

                // Then
                result.IsArgumentNullException("repositoryRoot");
            }

            [Fact]
            public void Should_Set_Property_Values_Passed_To_Constructor()
            {
                // Given 
                const string logFileContent = "foo";
                var repoRoot = new DirectoryPath(@"C:\");

                // When
                var settings = InspectCodeSettings.FromContent(logFileContent, repoRoot);

                // Then
                settings.LogFileContent.ShouldBe(logFileContent);
                settings.RepositoryRoot.ShouldBe(repoRoot);
            }

            [Fact]
            public void Should_Read_File_From_Disk()
            {
                var fileName = System.IO.Path.GetTempFileName();
                try
                {
                    // Given
                    string expected;
                    using (var ms = new MemoryStream())
                    using (var stream = this.GetType().Assembly.GetManifestResourceStream("Cake.Prca.Issues.InspectCode.Tests.Testfiles.inspectcode.xml"))
                    {
                        stream.CopyTo(ms);
                        var data = ms.ToArray();

                        using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        {
                            file.Write(data, 0, data.Length);
                        }

                        expected = Encoding.UTF8.GetString(data, 0, data.Length);
                    }

                    // When
                    var settings = 
                        InspectCodeSettings.FromFilePath(
                            new FilePath(fileName),
                            new DirectoryPath(@"C:\"));

                    // Then
                    settings.LogFileContent.ShouldBe(expected);
                }
                finally
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
            }
        }
    }
}
