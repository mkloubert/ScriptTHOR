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
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MarcelJoachimKloubert.ScriptTHOR
{
    internal static class Program
    {
        #region Methods (1)

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                var catalog = new AggregateCatalog();

                var currentDir = new DirectoryInfo(Environment.CurrentDirectory);

                var pluginsDir = new DirectoryInfo(Path.Combine(currentDir.FullName, "plugins"));
                if (pluginsDir.Exists)
                {
                    foreach (var pluginFile in pluginsDir.EnumerateFiles("*.dll"))
                    {
                        try
                        {
                            var asm = Assembly.LoadFrom(pluginFile.FullName);

                            catalog.Catalogs
                                   .Add(new AssemblyCatalog(asm));
                        }
                        catch
                        {
                            //TODO: show loading error
                        }
                    }
                }
                else
                {
                    //TODO: show warning
                }

                using (var container = new CompositionContainer(catalog,
                                                                isThreadSafe: true))
                {
                    var executors = container.GetExportedValues<global::MarcelJoachimKloubert.ScriptTHOR.Scripting.IScriptExecutor>()
                                             .ToList();
                    try
                    {
                        var filesToExecute = new List<FileInfo>();

                        foreach (var a in args)
                        {
                            if (string.IsNullOrWhiteSpace(a))
                            {
                                continue;
                            }

                            try
                            {
                                var file = new FileInfo(Path.IsPathRooted(a) ? Path.GetFullPath(a)
                                                                             : Path.GetFullPath(Path.Combine(currentDir.FullName,
                                                                                                             a)));

                                if (file.Exists)
                                {
                                    filesToExecute.Add(file);
                                }
                                else
                                {
                                    //TODO: show warning
                                }
                            }
                            catch
                            {
                                //TODO: show error
                            }
                        }

                        foreach (var file in filesToExecute)
                        {
                            try
                            {
                                var executorsToUse = executors.Where(x => x.IsFileExtensionSupported(file.Extension))
                                                              .ToList();

                                if (executorsToUse.Count == 1)
                                {
                                    try
                                    {
                                        var result = executorsToUse[0].Execute(File.ReadAllText(file.FullName,
                                                                                                Encoding.UTF8));

                                        if (result != null)
                                        {
                                        }
                                        else
                                        {
                                            //TODO: show warning for NO result
                                        }
                                    }
                                    catch
                                    {
                                        //TODO: show error
                                    }
                                }
                                else if (executorsToUse.Count > 1)
                                {
                                    //TODO: show warning for MORE THAN ONE executor found
                                }
                                else
                                {
                                    //TODO: show warning for NO executor found
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    finally
                    {
                        executors.Clear();
                    }
                }
            }
            catch
            {
                //TODO: show exception
            }
        }

        #endregion Methods (1)
    }
}