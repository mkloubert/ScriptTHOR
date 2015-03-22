//
// ScriptTHOR (https://github.com/mkloubert/ScriptTHOR)
//
// Copyright (c) 2015 Marcel Joachim Kloubert <marcel.kloubert@gmx.net>, All rights reserved.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.ScriptTHOR.Scripting
{
    /// <summary>
    /// A basic script executor.
    /// </summary>
    public abstract partial class ScriptExecutorBase : IScriptExecutor
    {
        #region Fields (3)

        protected readonly Random _RANDOM = new Random();
        protected readonly RNGCryptoServiceProvider _CRYPTO_RANDOM = new RNGCryptoServiceProvider();

        /// <summary>
        /// An unique object for thread safes operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields (1)

        #region Constructors (3)

        // <summary>
        /// Initializes a new instance of the <see cref="ScriptExecutorBase" /> class.
        /// </summary>
        protected ScriptExecutorBase()
            : this(new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptExecutorBase" /> class.
        /// </summary>
        /// <param name="sync">The value for the <see cref="ScriptExecutorBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected ScriptExecutorBase(object sync)
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._SYNC = sync;
        }

        /// <summary>
        /// The desructor.
        /// </summary>
        ~ScriptExecutorBase()
        {
            this.Dispose(false);
        }

        #endregion Constructors (3)

        #region Properties (2)

        /// <summary>
        /// <see cref="IScriptExecutor.IsDisposed" />
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of supported file extensions.
        /// </summary>
        public abstract IEnumerable<string> SupportedFileExtensions { get; }

        #endregion Properties (2)

        #region Methods (8)

        /// <summary>
        /// <see cref="IDisposable.Dispose()" />
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            lock (this._SYNC)
            {
                if (disposing && this.IsDisposed)
                {
                    return;
                }

                this.OnDispose(disposing);

                if (disposing)
                {
                    this.IsDisposed = true;
                }
            }
        }

        /// <summary>
        /// <see cref="IScriptExecutor.Execute(string)" />
        /// </summary>
        public IScriptExecutionResult Execute(string src)
        {
            this.ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(src))
            {
                return null;
            }

            var result = new ScriptExecutionResult();

            try
            {
                this.OnExecute(src, result);
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// <see cref="IScriptExecutor.IsFileExtensionSupported(string)" />
        /// </summary>
        public bool IsFileExtensionSupported(string ext)
        {
            ext = ParseFileExtension(ext);

            return (this.SupportedFileExtensions ?? Enumerable.Empty<string>()).Select(x => ParseFileExtension(x))
                                                                               .Any(x => x == ext);
        }

        /// <summary>
        /// Stores the logic for the <see cref="ScriptExecutorBase.Dispose()" /> method and the destructor.
        /// </summary>
        /// <param name="disposing">
        /// <see cref="ScriptExecutorBase.Dispose()" /> method was called (<see langword="true" />) or
        /// the destructor (<see langword="false" />).
        /// </param>
        protected virtual void OnDispose(bool disposing)
        {
            // dummy
        }

        /// <summary>
        /// Stores the logic for the <see cref="ScriptExecutorBase.Execute(string)" /> method.
        /// </summary>
        /// <param name="src">The script source to execute.</param>
        /// <param name="result">The result object.</param>
        protected abstract void OnExecute(string src, ScriptExecutionResult result);

        private static string ParseFileExtension(string ext)
        {
            ext = (ext ?? string.Empty).ToLower().Trim();
            if (ext.StartsWith("."))
            {
                ext = ext.Substring(1).Trim();
            }

            return ext;
        }

        /// <summary>
        /// Throws an exception if that object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Object has been disposed.
        /// </exception>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                var typeName = this.GetType().FullName;

                throw new ObjectDisposedException(objectName: typeName,
                                                  message: string.Format("'{0}' ({1}) has already been disposed.",
                                                                         typeName, this.GetHashCode()));
            }
        }

        #endregion Methods (8)
    }
}