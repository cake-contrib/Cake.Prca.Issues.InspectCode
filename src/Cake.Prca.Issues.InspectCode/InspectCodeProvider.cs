namespace Cake.Prca.Issues.InspectCode
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Core.Diagnostics;

    /// <summary>
    /// Provider for code analysis issues reported by JetBrains Inspect Code.
    /// </summary>
    public class InspectCodeProvider : CodeAnalysisProvider
    {
        private readonly InspectCodeSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectCodeProvider"/> class.
        /// </summary>
        /// <param name="log">The Cake log context.</param>
        /// <param name="settings">Settings for reading the log file.</param>
        public InspectCodeProvider(ICakeLog log, InspectCodeSettings settings)
            : base(log)
        {
            settings.NotNull(nameof(settings));

            this.settings = settings;
        }

        /// <inheritdoc />
        public override IEnumerable<ICodeAnalysisIssue> ReadIssues()
        {
            var result = new List<ICodeAnalysisIssue>();

            var logDocument = XDocument.Parse(settings.LogFileContent);

            var solutionPath = Path.GetDirectoryName(logDocument.Descendants("Solution").Single().Value);

            // Loop through all issue tags.
            foreach (var issue in logDocument.Descendants("Issue"))
            {
                // Read affected file from the issue.
                string fileName;
                if (!TryGetFile(issue, solutionPath, out fileName))
                {
                    continue;
                }

                // Read affected line from the issue.
                int line;
                if (!TryGetLine(issue, out line))
                {
                    continue;
                }

                // Read rule code from the issue.
                string rule;
                if (!TryGetRule(issue, out rule))
                {
                    continue;
                }

                // Read message from the issue.
                string message;
                if (!TryGetMessage(issue, out message))
                {
                    continue;
                }

                result.Add(new CodeAnalysisIssue(
                    fileName,
                    line,
                    message,
                    0, // TODO Set based on severity of issueType
                    rule));
            }

            return result;
        }

        /// <summary>
        /// Reads the affected file path from an issue logged in a Inspect Code log.
        /// </summary>
        /// <param name="issue">Issue element from Inspect Code log.</param>
        /// <param name="solutionPath">Path to the solution file.</param>
        /// <param name="fileName">Returns the full path to the affected file.</param>
        /// <returns>True if the file path could be parsed.</returns>
        private static bool TryGetFile(XElement issue, string solutionPath, out string fileName)
        {
            fileName = string.Empty;

            var fileAttr = issue.Attribute("File");
            if (fileAttr == null)
            {
                return false;
            }

            fileName = fileAttr.Value;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            // Combine with path to the solution file.
            fileName = Path.Combine(solutionPath, fileName);

            return true;
        }

        /// <summary>
        /// Reads the affected line from an issue logged in a Inspect Code log.
        /// </summary>
        /// <param name="issue">Issue element from Inspect Code log.</param>
        /// <param name="line">Returns line.</param>
        /// <returns>True if the line could be parsed.</returns>
        private static bool TryGetLine(XElement issue, out int line)
        {
            line = -1;

            var lineAttr = issue.Attribute("Line");

            var lineValue = lineAttr?.Value;
            if (string.IsNullOrWhiteSpace(lineValue))
            {
                return false;
            }

            line = int.Parse(lineValue, CultureInfo.InvariantCulture);

            return true;
        }

        /// <summary>
        /// Reads the rule code from an issue logged in a Inspect Code log.
        /// </summary>
        /// <param name="issue">Issue element from Inspect Code log.</param>
        /// <param name="rule">Returns the code of the rule.</param>
        /// <returns>True if the rule code could be parsed.</returns>
        private static bool TryGetRule(XElement issue, out string rule)
        {
            rule = string.Empty;

            var codeAttr = issue.Attribute("TypeId");
            if (codeAttr == null)
            {
                return false;
            }

            rule = codeAttr.Value;
            if (string.IsNullOrWhiteSpace(rule))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the message from an issue logged in a Inspect Code log.
        /// </summary>
        /// <param name="issue">Issue element from Inspect Code log.</param>
        /// <param name="message">Returns the message of the issue.</param>
        /// <returns>True if the message could be parsed.</returns>
        private static bool TryGetMessage(XElement issue, out string message)
        {
            message = string.Empty;

            var messageAttr = issue.Attribute("Message");
            if (messageAttr == null)
            {
                return false;
            }

            message = messageAttr.Value;
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            return true;
        }
    }
}
