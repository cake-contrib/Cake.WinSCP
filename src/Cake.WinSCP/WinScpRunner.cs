using System;
using System.IO;
using Cake.Core;
using WinSCP;

namespace Cake.WinSCP
{
    /// <summary>
    /// Cake wrapper for WinSCP.
    /// </summary>
    public class WinScpRunner
    {
        private readonly ICakeContext _context;

        /// <summary>
        /// Creates an instance of WinScpRunner class.
        /// </summary>
        /// <param name="context">Cake context.</param>
        public WinScpRunner(ICakeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        /// <summary>
        /// Synchronizes directories.
        /// </summary>
        /// <param name="url">Session URL (https://winscp.net/eng/docs/session_url).</param>
        /// <param name="remoteFolder">Full path to remote directory.</param>
        /// <param name="localFolder">Full path to local directory.</param>
        /// <param name="removeFiles">When set to true, deletes obsolete files.</param>
        public void SynchronizeDirectories(
            string url,
            string remoteFolder,
            string localFolder,
            bool removeFiles = false)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(remoteFolder))
            {
                throw new ArgumentException(nameof(remoteFolder));
            }

            if (string.IsNullOrWhiteSpace(localFolder))
            {
                throw new ArgumentException(nameof(localFolder));
            }

            if (!Directory.Exists(localFolder))
            {
                throw new DirectoryNotFoundException($"{localFolder} is not found");
            }

            var sessionOptions = new SessionOptions();
            sessionOptions.ParseUrl(url);
            sessionOptions.AddRawSettings("LocalDirectory", _context.Environment.WorkingDirectory.FullPath);

            using (var session = new Session())
            {
                Logger.Log($"Synchronize directories on {url}");

                session.FileTransferred += OnFileTransferred;
                session.Open(sessionOptions);
                var result = session.SynchronizeDirectories(SynchronizationMode.Remote, localFolder, remoteFolder, removeFiles);
                result.Check();

                Logger.Log($"{localFolder} and {remoteFolder} were synchronized");
            }
        }

        /// <summary>
        /// FileTransferred event handler.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnFileTransferred(object sender, TransferEventArgs e)
        {
            if (e.Error == null)
            {
                Logger.Log($"{e.FileName} have been uploaded");
            }
            else
            {
                Logger.Log($"{e.FileName} failed: {e.Error}");
            }
        }
    }
}
