﻿//******************************************************************************************************
//  EmbeddedResourceProvider.cs - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License a
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/04/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Caching;
using System.Web.Hosting;

namespace GSF.Web.Embedded
{
    /// <summary>
    /// Represents a <see cref="VirtualPathProvider"/> that allows access to embedded resources.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This provider responds to requests for embedded resources when the virtual path begins with an "@", e.g.,
    /// http://localhost/@GSF.Web.Model.Scripts.gsf.web.client.js
    /// </para>
    /// <para>
    /// To use, add the following to the Global.asax.cs Application_Start function:
    /// <code>
    ///     // Add additional virtual path provider to allow access to embedded resources
    ///     HostingEnvironment.RegisterVirtualPathProvider(new EmbeddedResourceProvider());
    /// </code>
    /// Then add the namespace paths to the embedded resources for which you need access to the Web.config &lt;system.webServer&gt; section:
    /// <code>
    /// &lt;system.webServer&gt;
    ///   &lt;handlers&gt;
    ///     &lt;!-- Add embedded resource handler for GSF script resources using slash delimiters --&gt;
    ///     &lt;add name="EmbeddedResourceHandler-GSFScripts" path="@GSF/Web/Model/Scripts/*" verb="*" type="System.Web.StaticFileHandler" allowPathInfo="true" /&gt;
    ///     &lt;!-- Add embedded resource handler for GSF view resources using slash delimiters --&gt;
    ///     &lt;add name="EmbeddedResourceHandler-GSFViews" path="@GSF/Web/Model/Views/*" verb="*" type="System.Web.StaticFileHandler" allowPathInfo="true" /&gt;
    ///     &lt;!-- Add embedded resource handler for GSF handler resources using slash delimiters --&gt;
    ///     &lt;add name="EmbeddedResourceHandler-GSFHandlers" path="@GSF/Web/Model/Handlers/*" verb="*" type="System.Web.UI.SimpleHandlerFactory" allowPathInfo="true" /&gt;
    ///     &lt;!-- Add embedded resource handler for fully qualified type names using dot delimiters (this should be defined last) --&gt;
    ///     &lt;add name="EmbeddedResourceHandler-FQName" path="@*" verb="*" type="System.Web.StaticFileHandler" allowPathInfo="true" /&gt;
    ///   &lt;/handlers&gt;
    /// &lt;/system.webServer&gt;
    /// </code>
    /// </para>
    /// </remarks>
    public class EmbeddedResourceProvider : VirtualPathProvider
    {
        #region [ Members ]

        // Nested Types

        private class EmbeddedResource : VirtualFile
        {
            public EmbeddedResource(string virtualPath) : base(virtualPath)
            {
            }

            public override Stream Open() => WebExtensions.OpenEmbeddedResourceStream(ParseResourceNameFromVirtualPath(VirtualPath));
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <returns>true if the file exists in the virtual file system; otherwise, false.</returns>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <remarks>
        /// Any virtual path that begins with an @ symbol is assumed to be an embedded resource where
        /// the path represents the namespace hierarchy in either slash or dot notation.
        /// </remarks>
        public override bool FileExists(string virtualPath)
        {
            return WebExtensions.EmbeddedResourceExists(ParseResourceNameFromVirtualPath(virtualPath)) || Previous.FileExists(virtualPath);
        }

        /// <summary>
        /// Gets a file from the virtual file system.
        /// </summary>
        /// <returns>An implementation of the <see cref="VirtualFile"/> class that represents a file in the virtual file system.</returns>
        /// <param name="virtualPath">The path to the virtual file.</param>
        public override VirtualFile GetFile(string virtualPath)
        {
            return WebExtensions.EmbeddedResourceExists(ParseResourceNameFromVirtualPath(virtualPath)) ? new EmbeddedResource(virtualPath) : Previous.GetFile(virtualPath);
        }

        /// <summary>
        /// Gets a value that indicates whether a directory exists in the virtual file system.
        /// </summary>
        /// <returns>true if the directory exists in the virtual file system; otherwise, false.</returns>
        /// <param name="virtualDir">The path to the virtual directory.</param>
        public override bool DirectoryExists(string virtualDir)
        {
            // Assume any given resource path exists - GetFile will validate full namespace for resource
            return IsEmbeddedResource(virtualDir) || Previous.DirectoryExists(virtualDir);
        }

        /// <summary>
        /// Gets a virtual directory from the virtual file system.
        /// </summary>
        /// <returns>A descendant of the <see cref="VirtualDirectory" /> class that represents a directory in the virtual file system.</returns>
        /// <param name="virtualDir">The path to the virtual directory.</param>
        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            return Previous.GetDirectory(virtualDir);
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <returns>A <see cref="CacheDependency" /> object for the specified virtual resources.</returns>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param>
        /// <param name="utcStart">The UTC time at which the virtual resources were read.</param>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return IsEmbeddedResource(virtualPath) ? null : Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        #endregion

        #region [ Static ]

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ParseResourceNameFromVirtualPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~/@"))
                virtualPath = virtualPath.Substring(3);
            else if (virtualPath.StartsWith("/@"))
                virtualPath = virtualPath.Substring(2);
            else if (virtualPath.StartsWith("@"))
                virtualPath = virtualPath.Substring(1);

            return virtualPath.Replace('/', '.');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEmbeddedResource(string virtualPath)
        {
            if (virtualPath.StartsWith("~/@"))
                return true;

            if (virtualPath.StartsWith("/@"))
                return true;

            if (virtualPath.StartsWith("@"))
                return true;

            return false;
        }

        #endregion
    }
}
