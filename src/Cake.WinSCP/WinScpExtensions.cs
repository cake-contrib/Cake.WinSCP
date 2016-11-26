using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;

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
        public static void WinScpSync(
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
        }
    }
}
