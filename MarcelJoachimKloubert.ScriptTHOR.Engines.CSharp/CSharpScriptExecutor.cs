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

using MarcelJoachimKloubert.ScriptTHOR.Helpers;
using MarcelJoachimKloubert.ScriptTHOR.Scripting;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.ScriptTHOR.Engines.CSharp
{
    /// <summary>
    /// Executor that uses the C# compiler classes from the .NET framework.
    /// </summary>
    [Export(typeof(global::MarcelJoachimKloubert.ScriptTHOR.Scripting.IScriptExecutor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class CSharpScriptExecutor : ScriptExecutorBase, IScriptExecutor
    {
        #region Properties (1)

        /// <summary>
        /// <see cref="ScriptExecutorBase.SupportedFileExtensions" />
        /// </summary>
        public override IEnumerable<string> SupportedFileExtensions
        {
            get { return new string[] { "cs" }; }
        }

        #endregion Properties (1)

        #region Methods (2)

        private string CreateFullSourceCode(string scriptSrc, IEnumerable<string> namespaces, out string typeName)
        {
            var builder = new StringBuilder();

            var g = Guid.NewGuid();

            var buffer = new byte[8];
            this._CRYPTO_RANDOM.GetBytes(buffer);

            var ul = BitConverter.ToUInt64(buffer, 0);

            var namespaceName = "MarcelJoachimKloubert.ScriptTHOR.Engines.CSharp.Execution";
            var className = string.Format("ScriptExecutionEnvironment_{0:N}_{1}_{2}",
                                          g, ul, this.GetHashCode());

            foreach (var ns in namespaces)
            {
                builder.AppendFormat("using {0};", ns)
                       .AppendLine();
            }

            builder.AppendLine();

            builder.AppendFormat("namespace {0}", namespaceName)
                   .AppendLine()
                   .AppendLine("{");

            builder.AppendFormat("    public class {0} : global::{1}",
                                 className,
                                 typeof(global::MarcelJoachimKloubert.ScriptTHOR.Scripting.ScriptExecutionEnvironmentBase).FullName)
                   .AppendLine();
            builder.AppendLine("    {");

            builder.AppendFormat("        public override void Execute(global::{0} context)",
                                 typeof(global::MarcelJoachimKloubert.ScriptTHOR.Scripting.ScriptExecutionContext).FullName)
                   .AppendLine();
            builder.AppendLine("        {");
            builder.AppendLine()
                   .AppendLine(scriptSrc)
                   .AppendLine();
            builder.AppendLine("        }");

            builder.AppendLine("    }");

            builder.AppendLine("}");

            typeName = namespaceName + "." + className;
            return builder.ToString();
        }

        /// <summary>
        /// <see cref="ScriptExecutorBase.OnExecute(string, ScriptExecutionResult)" />
        /// </summary>
        protected override void OnExecute(string src, ScriptExecutionResult result)
        {
            var compiler = new CSharpCodeProvider();

            CompilerParameters compilerParams;
            IList<string> namespaces;
            CompilerHelper.CreateDataForCompiler(out compilerParams,
                                                 out namespaces);

            string typeName;
            var compilerResult = compiler.CompileAssemblyFromSource(compilerParams,
                                                                    this.CreateFullSourceCode(src, namespaces, out typeName));

            if (compilerResult.Errors.HasErrors == false)
            {
                var type = compilerResult.CompiledAssembly
                                         .GetTypes()
                                         .Single(x => x.FullName == typeName);

                var env = (global::MarcelJoachimKloubert.ScriptTHOR.Scripting.ScriptExecutionEnvironmentBase)Activator.CreateInstance(type);

                var ctx = new ScriptExecutionContext();
                env.Execute(ctx);
            }
            else
            {
            }
        }

        #endregion Methods (2)
    }
}