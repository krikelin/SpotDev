using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Board
{
    /// <summary>
    /// Interface for dealing with various script engines to connect. All engines must be wrapped by this interface.
    /// </summary>

    public interface IScriptEngine
    {
        
        /// <summary>
        /// This function is called by the preprocesor after the code has been compiled from the preprocessing template
        /// </summary>
        /// <param name="scriptCode">The string with the javascript code</param>
        /// <returns>True if sucess, false if failed </returns>
        string Run(string scriptCode);

        /// <summary>
        /// Invoke an function in the client script from the executable code
        /// </summary>
        /// <param name="Function"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        object Invoke(string Function, params object[] arguments);

        /// <summary>
        /// Thus method is called to make an instance of an certain object accessible for the scripting object.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="varInstance"></param>
        void SetVariable(string variableName, object varInstance);

        /// <summary>
        /// This method is called to set an function to be accessible by the functionName for the script once Executed.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="functionPointer"></param>
        void SetFunction(string functionName, Delegate functionPointer);

       
    }

   
}
