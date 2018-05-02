using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Sitecore.Foundation.CodeConsole
{
    public class Console
    {
        private String _codeToExecute;
        private Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider _csProvider;

        public Console()
        {
            this._csProvider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
        }

        /// <summary>
        /// Initializes the console
        /// </summary>
        /// <param name="codeToExecute"></param>
        /// <param name="referencedAssemblies"></param>
        public void Init(String codeToExecute, List<String> referencedAssemblies = null)
        {
            this._codeToExecute = codeToExecute;            
        }

        public CompilerResults Compile(String codeToExecute)
        {
            CompilerResults results = this._csProvider.CompileAssemblyFromSource(GetCompilerParameters(false, false, false, null), codeToExecute);
            return results;
        }

        /// <summary>
        /// Execute the provided code and returns the execution result.
        /// </summary>
        /// <param name="codeToExecute"></param>
        /// <returns></returns>
        public String Execute(String codeToExecute)
        {
            CompilerResults results = this._csProvider.CompileAssemblyFromSource(GetCompilerParameters(false, false, false, null), codeToExecute);
            if (results.NativeCompilerReturnValue > 0)
                return GetCompilationErrors(results.Errors);
            return ExecuteCode(results);
        }

        public Assembly LoadAssembly(String assemblyPath)
        {
            return Assembly.LoadFile(assemblyPath);
        }

        /// <summary>
        /// Executes the compiled assembly
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private string ExecuteCode(CompilerResults results)
        {
            Assembly assemblyToExecute = LoadAssembly(results.PathToAssembly);
            Type scCode = assemblyToExecute.GetType("SCCodeConsole.SCCode");
            object scCodeObject = Activator.CreateInstance(scCode);
            var output = scCode.InvokeMember("Process", BindingFlags.Default | BindingFlags.InvokeMethod, null, scCodeObject, null);
            return output.ToString();
        }

        public String ExecuteAssembly(String assemblyPath)
        {
            Assembly assemblyToExecute = LoadAssembly(assemblyPath);
            Type scCode = assemblyToExecute.GetType("SCCodeConsole.SCCode");
            object scCodeObject = Activator.CreateInstance(scCode);
            var output = scCode.InvokeMember("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, scCodeObject, null);
            return output.ToString();
        }

        private string GetCompilationErrors(CompilerErrorCollection errors)
        {
            StringBuilder sbErrors = new StringBuilder();
            foreach(CompilerError error in errors)
            {
                sbErrors.AppendLine($"{error.ErrorNumber}::{error.ErrorText}");
            }
            return sbErrors.ToString();
        }

        /// <summary>
        /// Get the compiler parameters
        /// </summary>
        /// <param name="generateInMemory"></param>
        /// <param name="generateExecutable"></param>
        /// <param name="includeDebugInformation"></param>
        /// <param name="referencedAssemblies"></param>
        /// <returns></returns>
        private CompilerParameters GetCompilerParameters(Boolean generateInMemory, Boolean generateExecutable, Boolean includeDebugInformation, String[] referencedAssemblies)
        {
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateInMemory = generateInMemory;
            compilerParameters.GenerateExecutable = generateExecutable;
            compilerParameters.IncludeDebugInformation = includeDebugInformation;
            compilerParameters.ReferencedAssemblies.AddRange(GetAssembliesLoadedInAppDomain().ToArray());
            if (referencedAssemblies != null && referencedAssemblies.Count() > 0)
            {
                compilerParameters.ReferencedAssemblies.AddRange(referencedAssemblies);
            }
            return compilerParameters;
        }

        /// <summary>
        /// Gets the list of assemblies loaded in the current application domain.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<String> GetAssembliesLoadedInAppDomain()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Select(a => a.Location);
        }
    }
}