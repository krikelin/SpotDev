using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Board;

namespace SpotDev.Mako
{
    public class LuaEngine : IScriptEngine
    {

        /// <summary>
        /// The instance of the Jint parser
        /// </summary>
        LuaInterface.Lua scriptEngine;

        public string errorOutput = "";
        public LuaEngine()
        {
            scriptEngine = new LuaInterface.Lua();

        }

        /// <summary>
        /// Invokes an user function
        /// </summary>
        /// <param name="func"></param>              L
        /// <param name="args"></param>
        /// <returns></returns>
        public object Invoke(string func, params object[] args)
        {
            try
            {
                return scriptEngine.GetFunction(func).Call(args);
            }
            catch
            {
                return "";
            }

        }
        /// <summary>
        /// This function is called by the preprocesor after the code has been compiled from the preprocessing template
        /// </summary>
        /// <param name="scriptCode">The string with the javascript code</param>
        /// <returns>True if sucess, false if failed </returns>
        public string Run(string scriptCode)
        {

                scriptEngine.DoString(scriptCode);
            return "";

        }


        /// <summary>
        /// Thus method is called to make an instance of an certain object accessible for the scripting object.
        /// </summary>
        /// <param name="variableName">The name of the instance at the script level</param>
        /// <param name="varInstance">The object to send</param>
        public void SetVariable(string variableName, object varInstance)
        {
            scriptEngine[variableName] = varInstance;
        }

        /// <summary>
        /// This method is called to set an function to be accessible by the functionName for the script once Executed.
        /// </summary>
        /// <param name="functionName">The desired alias for the function at script level</param>
        /// <param name="functionPointer">The delegate for the function to use at script</param>
        public void SetFunction(string functionName, Delegate functionPointer)
        {

            scriptEngine.RegisterFunction(functionName, functionPointer.Target, functionPointer.Method);
        }
    }
}
