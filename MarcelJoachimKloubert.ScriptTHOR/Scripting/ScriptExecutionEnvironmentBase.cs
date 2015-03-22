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

using System.Windows.Forms;

namespace MarcelJoachimKloubert.ScriptTHOR.Scripting
{
    /// <summary>
    /// A basic class that can be used as environment for script executions.
    /// </summary>
    public abstract class ScriptExecutionEnvironmentBase
    {
        #region Methods (2)

        /// <summary>
        /// Shows a message window (s. <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window/alert" />).
        /// </summary>
        /// <param name="msg">The message to display.</param>
        public virtual void Alert(object msg)
        {
            MessageBox.Show((msg ?? string.Empty).ToString());
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="context">The execution context.</param>
        public abstract void Execute(ScriptExecutionContext context);

        #endregion Methods (2)
    }
}