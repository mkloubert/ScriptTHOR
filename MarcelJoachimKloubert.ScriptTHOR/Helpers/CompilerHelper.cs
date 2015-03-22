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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.ScriptTHOR.Helpers
{
    /// <summary>
    /// Helpers class for compiler operations.
    /// </summary>
    public static class CompilerHelper
    {
        #region Methods (2)

        /// <summary>
        /// Create common data for compiler classes.
        /// </summary>
        /// <param name="compilerParams">The variable where to write the compiler parameters to.</param>
        /// <param name="namespacesToImport">The variable where to write the namespaces to import.</param>
        public static void CreateDataForCompiler(out CompilerParameters compilerParams,
                                                 out IList<string> namespacesToImport)
        {
            compilerParams = new CompilerParameters();
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;
            compilerParams.TreatWarningsAsErrors = false;

            // assemblies
            var assemblies = new HashSet<Assembly>();
            {
                assemblies.Add(typeof(object).Assembly);
                assemblies.Add(typeof(global::System.Data.IDbConnection).Assembly);
                assemblies.Add(typeof(global::System.Linq.Enumerable).Assembly);
                assemblies.Add(typeof(global::System.Xml.XmlNode).Assembly);
                assemblies.Add(typeof(global::System.Xml.Linq.XNode).Assembly);
                assemblies.Add(typeof(global::System.Xml.XPath.Extensions).Assembly);
                assemblies.Add(typeof(global::System.Reflection.Assembly).Assembly);
                assemblies.Add(Assembly.GetExecutingAssembly());
            }

            // add references
            compilerParams.ReferencedAssemblies
                          .AddRange(assemblies.Select(x => TryGetAssemblyFile(x))
                                              .Where(x => x != null)
                                              .Distinct()
                                              .ToArray());

            // namespaces
            var namespaces = new HashSet<string>();
            namespaces.Add(typeof(object).Namespace);
            namespaces.Add(typeof(global::System.Data.IDbConnection).Namespace);
            namespaces.Add(typeof(global::System.Linq.Enumerable).Namespace);
            namespaces.Add(typeof(global::System.Xml.XmlNode).Namespace);
            namespaces.Add(typeof(global::System.Xml.Linq.XNode).Namespace);
            namespaces.Add(typeof(global::System.Xml.XPath.Extensions).Namespace);
            namespaces.Add(typeof(global::System.Reflection.Assembly).Namespace);
            namespacesToImport = namespaces.OrderBy(ns => ns, StringComparer.InvariantCultureIgnoreCase)
                                           .ToList();
        }

        private static string TryGetAssemblyFile(Assembly asm)
        {
            string result = null;

            if (asm != null)
            {
                try
                {
                    var pathOrUri = asm.CodeBase;

                    Uri u;
                    if (Uri.TryCreate(pathOrUri, UriKind.RelativeOrAbsolute, out u))
                    {
                        result = Path.GetFullPath(u.LocalPath);
                    }
                    else
                    {
                        result = Path.GetFullPath(pathOrUri);
                    }

                    if (File.Exists(result) == false)
                    {
                        result = null;
                    }
                }
                catch
                {
                    result = null;
                }
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = null;
            }

            return result;
        }

        #endregion Methods (2)
    }
}