using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using WinSCP;

namespace Cake.WinSCP
{
    /// <summary>
    /// Cake AddIn to upload files using WinSCP.
    /// <code>
    /// #addin Cake.WinSCP
    /// </code>
    /// </summary>
    [CakeAliasCategory("Deployment")]
    public static class WinScpExtensions
    {
        /// <summary>
        /// Synchronizes directories using WinSCP.
        /// </summary>
        /// <param name="context">Cake context.</param>
        /// <param name="url">Session URL (https://winscp.net/eng/docs/session_url).</param>
        /// <param name="remoteFolder">Full path to remote directory.</param>
        /// <param name="localFolder">Full path to local directory.</param>
        /// <param name="removeFiles">When set to true, deletes obsolete files.</param>
        /// <example>
        /// <code>
        /// WinScpSync(
        ///     "ftp://username:password@site.com/",
        ///     "/public",
        ///     @"c:\projects\site",
        ///     false
        /// );
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static ICakeContext WinScpSync(
            this ICakeContext context,
            string url,
            string remoteFolder,
            string localFolder,
            bool removeFiles = false)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Logger.LogEngine = context.Log;

            var runner = new WinScpRunner(context);
            runner.SynchronizeDirectories(url, remoteFolder, localFolder, removeFiles);

            return context;
        }

        /// <summary>
        /// Synchronizes directories.
        /// </summary>
        /// <param name="context">The cake context</param>
        /// <param name="options">Session Options (https://winscp.net/eng/docs/library_sessionoptions).</param>
        /// <param name="remoteFolder">Full path to remote directory.</param>
        /// <param name="localFolder">Full path to local directory.</param>
        /// <param name="removeFiles">When set to true, deletes obsolete files.</param>
        /// <param name="mode">The mode or direction of syncronization defaults to remote</param>
        /// <param name="mirror">Mirrors local directory with remote directory or vice versa depending on Synchronization mode. Can't be used with `Both` mode</param>
        /// <param name="criteria">The criteria by which to determine if synchronization of a file is necessary defaults to time</param>
        /// <param name="transferOptions">The transfer options (https://winscp.net/eng/docs/library_transferoptions)</param>
        /// <returns>The ICake Context</returns>
        [CakeMethodAlias]
        public static ICakeContext WinScpSync(this ICakeContext context,
            SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool removeFiles = false,
            SynchronizationMode mode = SynchronizationMode.Remote,
            bool mirror = false,
            SynchronizationCriteria criteria = SynchronizationCriteria.Time,
            TransferOptions transferOptions = null)
        {
            new WinScpRunner(context).SynchronizeDirectories(options, remoteFolder, localFolder, removeFiles, mode, mirror, criteria, transferOptions);
            return context;
        }

        /// <summary>
        /// Push files in folder to a remote folder
        /// </summary>
        /// <param name="context">The ICakeContext</param>
        /// <param name="options">Session Options (https://winscp.net/eng/docs/library_sessionoptions).</param>
        /// <param name="localFolder">Full path to local directory.</param>
        /// <param name="remoteFolder">Full path to remote directory.</param>
        /// <param name="remove">Remove local file after upload</param>
        /// <param name="transferOptions">The transfer options (https://winscp.net/eng/docs/library_transferoptions)</param>
        /// <returns>The ICakeContext</returns>
        [CakeMethodAlias]
        public static ICakeContext WinScpPut(this ICakeContext context,
            SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool remove = false,
            TransferOptions transferOptions = null)
        {
            new WinScpRunner(context).PutFiles(options, remoteFolder, localFolder, remove, transferOptions);
            return context;
        }

        /// <summary>
        /// Get a List of Files from the remote server directory
        /// </summary>
        /// <param name="context">The ICakeContext</param>
        /// <param name="options">Session Options (https://winscp.net/eng/docs/library_sessionoptions).</param>
        /// <param name="remoteFolder">Full path to remote directory.</param>
        /// <returns>The IEnumerable list of RemoteFileInfo (https://winscp.net/eng/docs/library_remotefileinfo)</returns>
        [CakeMethodAlias]
        public static IEnumerable<RemoteFileInfo> WinScpList(this ICakeContext context,
            SessionOptions options,
            string remoteFolder)
        {
            return new WinScpRunner(context).GetFileList(options, remoteFolder);
        }

        /// <summary>
        /// Download a file or files from remote server
        /// </summary>
        /// <param name="context">The ICakeContext</param>
        /// <param name="options">Session Options (https://winscp.net/eng/docs/library_sessionoptions).</param>
        /// <param name="localFolder">Full path to local directory.</param>
        /// <param name="remoteFolder">Full path to remote directory.</param>
        /// <param name="remove">Remove remote file after download</param>
        /// <param name="transferOptions">The transfer options (https://winscp.net/eng/docs/library_transferoptions)</param>
        /// <returns>The ICakeContext</returns>
        [CakeMethodAlias]
        public static ICakeContext WinScpGet(this ICakeContext context,
            SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool remove = false,
            TransferOptions transferOptions = null)
        {
            new WinScpRunner(context).GetFiles(options, remoteFolder, localFolder, remove, transferOptions);
            return context;
        }

        /// <summary>
        /// Compares the local and remote directories.
        /// </summary>
        /// <param name="context">The cake context</param>
        /// <param name="options">Session Options (https://winscp.net/eng/docs/library_sessionoptions).</param>
        /// <param name="remoteFolder">Full path to remote directory.</param>
        /// <param name="logDifferences">Log to output the differences between the local and remote directories</param>
        /// <param name="localFolder">Full path to local directory.</param>
        /// <param name="removeFiles">When set to true, considers deleting obsolete files.</param>
        /// <param name="mode">The mode or direction of syncronization defaults to remote</param>
        /// <param name="mirror">Mirrors local directory with remote directory or vice versa depending on Synchronization mode. Can't be used with `Both` mode</param>
        /// <param name="criteria">The criteria by which to determine if synchronization of a file is necessary defaults to time</param>
        /// <param name="transferOptions">The transfer options (https://winscp.net/eng/docs/library_transferoptions)</param>
        /// <returns>The ICake Context</returns>
        [CakeMethodAlias]
        public static IEnumerable<ComparisonDifference> WinScpCompare(this ICakeContext context,
            SessionOptions options,
            string remoteFolder,
            string localFolder,
            bool logDifferences = false,
            bool removeFiles = false,
            SynchronizationMode mode = SynchronizationMode.Remote,
            bool mirror = false,
            SynchronizationCriteria criteria = SynchronizationCriteria.Time,
            TransferOptions transferOptions = null)
        {
            return new WinScpRunner(context).CompareDirectories(options, remoteFolder, localFolder, logDifferences, removeFiles, mode, mirror, criteria, transferOptions);
        }
    }
}