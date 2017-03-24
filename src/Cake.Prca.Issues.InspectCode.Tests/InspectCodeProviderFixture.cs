namespace Cake.Prca.Issues.InspectCode.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Core.Diagnostics;
    using Testing;

    public class InspectCodeProviderFixture
    {
        public InspectCodeProviderFixture(string fileResourceName)
        {
            this.Log = new FakeLog();
            this.Log.Verbosity = Verbosity.Normal;

            using (var stream = this.GetType().Assembly.GetManifestResourceStream("Cake.Prca.Issues.InspectCode.Tests.Testfiles." + fileResourceName))
            {
                using (var sr = new StreamReader(stream))
                {
                    this.Settings =
                        InspectCodeSettings.FromContent(
                            sr.ReadToEnd(),
                            new Core.IO.DirectoryPath(@"c:\Source\Cake.Prca"));
                }
            }
        }

        public FakeLog Log { get; set; }

        public InspectCodeSettings Settings { get; set; }

        public InspectCodeProvider Create()
        {
            return new InspectCodeProvider(this.Log, this.Settings);
        }

        public IEnumerable<ICodeAnalysisIssue> ReadIssues()
        {
            var codeAnalysisProvider = this.Create();
            return codeAnalysisProvider.ReadIssues();
        }
    }
}
