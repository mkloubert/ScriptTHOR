//
//  That plugin is free software and part of the ScriptTHOR project (https://github.com/mkloubert/ScriptTHOR):
//  you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  It is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with that plugin.  If not, see <http://www.gnu.org/licenses/>.
//

using MarcelJoachimKloubert.ScriptTHOR.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ScriptTHOR.Engines.Jurassic
{
    /// <summary>
    /// Executor that uses Jurassic JavaScript engine (http://jurassic.codeplex.com/).
    /// </summary>
    [Export(typeof(global::MarcelJoachimKloubert.ScriptTHOR.Scripting.IScriptExecutor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed partial class JurassicScriptExecutor : ScriptExecutorBase, IScriptExecutor
    {
        #region Properties (1)

        /// <summary>
        /// <see cref="ScriptExecutorBase.SupportedFileExtensions" />
        /// </summary>
        public override IEnumerable<string> SupportedFileExtensions
        {
            get { return new string[] { "js" }; }
        }

        #endregion Properties (1)

        #region Methods (1)

        /// <summary>
        /// <see cref="ScriptExecutorBase.OnExecute(string, ScriptExecutionResult)" />
        /// </summary>
        protected override void OnExecute(string src, ScriptExecutionResult result)
        {
            var env = new JSScriptExecutionEnvironment();

            var engine = new global::Jurassic.ScriptEngine();
            engine.ForceStrictMode = false;
            engine.EnableDebugging = false;
            engine.CompatibilityMode = global::Jurassic.CompatibilityMode.Latest;

            engine.SetGlobalFunction("alert", new Action<object>(env.Alert));

            engine.Execute(src);
        }

        #endregion Methods (1)
    }
}