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

namespace MarcelJoachimKloubert.ScriptTHOR.Scripting
{
    /// <summary>
    /// Describes an object that executes scripts.
    /// </summary>
    public interface IScriptExecutor : IDisposable
    {
        #region Properties (1)

        /// <summary>
        /// Gets if the executor has been disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        #endregion Properties (1)

        #region Methods (2)

        /// <summary>
        /// Executes a script.
        /// </summary>
        /// <param name="src">The script to execute.</param>
        /// <returns>The result of the execution.</returns>
        IScriptExecutionResult Execute(string src);

        /// <summary>
        /// Checks if a file extension is supported or not.
        /// </summary>
        /// <param name="ext">The extension to check.</param>
        /// <returns>Is supported or not.</returns>
        bool IsFileExtensionSupported(string ext);

        #endregion Methods (2)
    }
}