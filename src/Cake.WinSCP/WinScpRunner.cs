using System;
using System.IO;
using System.Collections.Generic;
using Cake.Core;
using WinSCP;

namespace Cake.WinSCP
{
    /// <summary>
    /// Cake wrapper for WinSCP.
    /// </summary>
    internal class WinScpRunner
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

        public void SynchronizeDirectories(SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool removeFiles = false,
            SynchronizationMode mode = SynchronizationMode.Remote,
            bool mirror = false,
            SynchronizationCriteria criteria = SynchronizationCriteria.Time,
            TransferOptions transferOptions = null)
        {
            options.AddRawSettings("LocalDirectory", _context.Environment.WorkingDirectory.FullPath);

            using (var session = new Session())
            {
                session.FileTransferred += OnFileTransferred;
                session.Open(options);
                session.FileExists(remoteFolder);
                session.CreateDirectory(remoteFolder);
                var result = session.SynchronizeDirectories(mode, localFolder, remoteFolder, removeFiles, mirror, criteria, transferOptions);
                result.Check();

                Logger.Log($"{localFolder} and {remoteFolder} were synchronized");
            }
        }

        public void PutFiles(SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool removeFiles = false,
            TransferOptions transferOptions = null)
        {
            options.AddRawSettings("LocalDirectory", _context.Environment.WorkingDirectory.FullPath);

            using (var session = new Session())
            {
                session.FileTransferred += OnFileTransferred;
                session.Open(options);
                session.FileExists(remoteFolder);
                session.CreateDirectory(remoteFolder);
                var result = session.PutFiles(localFolder, remoteFolder, removeFiles, transferOptions);
                result.Check();

                Logger.Log($"{localFolder} was pushed to {remoteFolder}");
            }
        }

        public IEnumerable<RemoteFileInfo> GetFileList(SessionOptions options, string remoteFolder)
        {
            options.AddRawSettings("LocalDirectory", _context.Environment.WorkingDirectory.FullPath);

            using (var session = new Session())
            {
                session.Open(options);
                var result = session.ListDirectory(remoteFolder);

                Logger.Log($"{remoteFolder} list was queried");
                return result.Files;
            }
        }

        public void GetFiles(SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool removeFiles = false,
            TransferOptions transferOptions = null)
        {
            options.AddRawSettings("LocalDirectory", _context.Environment.WorkingDirectory.FullPath);

            using (var session = new Session())
            {
                session.Open(options);
                var result = session.GetFiles(remoteFolder, localFolder, removeFiles, transferOptions);
                result.Check();
                Logger.Log($"{remoteFolder} was downloaded to {localFolder}");
            }
        }

        public IEnumerable<ComparisonDifference> CompareDirectories(SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool logDifferences = false,
            bool removeFiles = false,
            SynchronizationMode mode = SynchronizationMode.Remote,
            bool mirror = false,
            SynchronizationCriteria criteria = SynchronizationCriteria.Time,
            TransferOptions transferOptions = null)
        {
            options.AddRawSettings("LocalDirectory", _context.Environment.WorkingDirectory.FullPath);

            using (var session = new Session())
            {
                session.Open(options);
                Logger.Log($"{localFolder} and {remoteFolder} are being compared");
                var result = session.CompareDirectories(mode, localFolder, remoteFolder, removeFiles, mirror, criteria, transferOptions) as IEnumerable<ComparisonDifference>;
                if (logDifferences)
                {
                    if (result == null)
                    {
                        Logger.Log("Compare Result was Null");
                        return result;
                    }
                    Logger.Log(String.Format("|{0,16}|{1,10}|{2,20}|{3,20}|", "Action".PadRight(16), "Directory".PadRight(10), "Local File Name".PadRight(20), "Remote File Name".PadRight(20)));
                    foreach (var diff in result)
                    {
                        var localFile = diff?.Local?.FileName;
                        if (String.IsNullOrEmpty(localFile))
                        {
                            localFile = Path.GetFileName(localFile);
                        }

                        var remoteFile = diff?.Remote?.FileName;
                        if (!String.IsNullOrEmpty(remoteFile))
                        {
                            remoteFile = Path.GetFileName(remoteFile);
                        }

                        Logger.Log(String.Format("|{0,16}|{1,10}|{2,20}|{3,20}|",
                            diff?.Action.ToString().PadRight(16),
                            diff?.IsDirectory.ToString().PadRight(10),
                            localFile?.PadRight(20),
                            remoteFile?.PadRight(20)));
                    }
                }
                return result;
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