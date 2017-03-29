namespace Cake.Prca.Issues.InspectCode.Tests
{
    using System.Linq;
    using Core.IO;
    using Shouldly;
    using Testing;
    using Xunit;

    public class InspectCodeProviderTests
    {
        public sealed class TheInspectCodeProviderCtor
        {
            [Fact]
            public void Should_Throw_If_Log_Is_Null()
            {
                // Given / When
                var result = Record.Exception(() =>
                    new InspectCodeProvider(
                        null,
                        InspectCodeSettings.FromContent(@"foo")));

                // Then
                result.IsArgumentNullException("log");
            }

            [Fact]
            public void Should_Throw_If_Settings_Are_Null()
            {
                // Given / When
                var result = Record.Exception(() => new InspectCodeProvider(new FakeLog(), null));

                // Then
                result.IsArgumentNullException("settings");
            }
        }

        public sealed class TheReadIssuesMethod
        {
            [Fact]
            public void Should_Read_Issue_Correct()
            {
                // Given
                var fixture = new InspectCodeProviderFixture("inspectcode.xml");

                // When
                var issues = fixture.ReadIssues();

                // Then
                issues.Count().ShouldBe(1);
                var issue = issues.Single();
                CheckIssue(
                    issue,
                    @"src\Cake.Prca\CakeAliasConstants.cs",
                    16,
                    "UnusedMember.Global",
                    0,
                    @"Constant 'PullRequestSystemCakeAliasCategory' is never used");
            }

            private static void CheckIssue(
                ICodeAnalysisIssue issue,
                string affectedFileRelativePath,
                int? line,
                string rule,
                int priority,
                string message)
            {
                issue.AffectedFileRelativePath.ToString().ShouldBe(new FilePath(affectedFileRelativePath).ToString());
                issue.AffectedFileRelativePath.IsRelative.ShouldBe(true, "Issue path is not relative");
                issue.Line.ShouldBe(line);
                issue.Rule.ShouldBe(rule);
                issue.Priority.ShouldBe(priority);
                issue.Message.ShouldBe(message);
            }
        }
    }
}
