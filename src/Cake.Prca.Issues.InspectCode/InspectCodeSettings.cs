namespace Cake.Prca.Issues.InspectCode
{
    using System.IO;
    using Core.IO;

    /// <summary>
    /// Settings for <see cref="InspectCodeProvider"/>.
    /// </summary>
    public class InspectCodeSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InspectCodeSettings"/> class.
        /// </summary>
        /// <param name="logFilePath">Path to the the Inspect Code log file.</param>
        /// <param name="repositoryRoot">Root path of the repository.</param>
        protected InspectCodeSettings(FilePath logFilePath, DirectoryPath repositoryRoot)
        {
            logFilePath.NotNull(nameof(logFilePath));
            repositoryRoot.NotNull(nameof(repositoryRoot));

            this.RepositoryRoot = repositoryRoot;

            using (var stream = new FileStream(logFilePath.FullPath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(stream))
                {
                    this.LogFileContent = sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectCodeSettings"/> class.
        /// </summary>
        /// <param name="logFileContent">Content of the the Inspect Code log file.</param>
        /// <param name="repositoryRoot">Root path of the repository.</param>
        protected InspectCodeSettings(string logFileContent, DirectoryPath repositoryRoot)
        {
            logFileContent.NotNullOrWhiteSpace(nameof(logFileContent));
            repositoryRoot.NotNull(nameof(repositoryRoot));

            this.RepositoryRoot = repositoryRoot;

            this.LogFileContent = logFileContent;
        }

        /// <summary>
        /// Gets the content of the log file.
        /// </summary>
        public string LogFileContent { get; private set; }

        /// <summary>
        /// Gets the Root path of the repository.
        /// </summary>
        public DirectoryPath RepositoryRoot { get; private set; }

        /// <summary>
        /// Returns a new instance of the <see cref="InspectCodeSettings"/> class from a log file on disk.
        /// </summary>
        /// <param name="logFilePath">Path to the JetBrains Inspect Code log file.</param>
        /// <param name="repositoryRoot">Root path of the repository.</param>
        /// <returns>Instance of the <see cref="InspectCodeSettings"/> class.</returns>
        public static InspectCodeSettings FromFilePath(FilePath logFilePath, DirectoryPath repositoryRoot)
        {
            return new InspectCodeSettings(logFilePath, repositoryRoot);
        }

        /// <summary>
        /// Returns a new instance of the <see cref="InspectCodeSettings"/> class from the content
        /// of a JetBrains Inspect Code log file.
        /// </summary>
        /// <param name="logFileContent">Content of the JetBrains Inspect Code log file.</param>
        /// <param name="repositoryRoot">Root path of the repository.</param>
        /// <returns>Instance of the <see cref="InspectCodeSettings"/> class.</returns>
        public static InspectCodeSettings FromContent(string logFileContent, DirectoryPath repositoryRoot)
        {
            return new InspectCodeSettings(logFileContent, repositoryRoot);
        }
    }
}
