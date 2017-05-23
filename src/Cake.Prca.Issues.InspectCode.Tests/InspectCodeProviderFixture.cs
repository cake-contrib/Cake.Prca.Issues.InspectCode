namespace Cake.Prca.Issues.InspectCode.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Core.Diagnostics;
    using Testing;

    internal class InspectCodeProviderFixture
    {
        public InspectCodeProviderFixture(string fileResourceName)
        {
            this.Log = new FakeLog { Verbosity = Verbosity.Normal };

            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Cake.Prca.Issues.InspectCode.Tests.Testfiles." + fileResourceName))
            {
                using (var sr = new StreamReader(stream))
                {
                    this.Settings =
                        InspectCodeSettings.FromContent(
                            sr.ReadToEnd());
                }
            }

            this.PrcaSettings =
                new ReportCodeAnalysisIssuesToPullRequestSettings(@"c:\Source\Cake.Prca");
        }

        public FakeLog Log { get; set; }

        public InspectCodeSettings Settings { get; set; }

        public ReportCodeAnalysisIssuesToPullRequestSettings PrcaSettings { get; set; }

        public InspectCodeProvider Create()
        {
            var provider = new InspectCodeProvider(this.Log, this.Settings);
            provider.Initialize(this.PrcaSettings);
            return provider;
        }

        public IEnumerable<ICodeAnalysisIssue> ReadIssues()
        {
            var codeAnalysisProvider = this.Create();
            return codeAnalysisProvider.ReadIssues(PrcaCommentFormat.PlainText);
        }
    }
}
