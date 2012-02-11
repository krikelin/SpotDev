using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Board
{
    /// <summary>
    /// Class for parsing CSS
    /// </summary>
    public class CssParser
    {
        public CssParser()
        {
        }
       
        /// <summary>
        /// Parses an set of CSS expressions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Dictionary<String, Dictionary<String, string>> ParseCSSFile(string input)
        {
            // Allocate memory for the list
            Dictionary<String, String> Output = new Dictionary<string, string>();
            int parserLevel = 0;
            // Selector string buffer
            StringBuilder Selector = new StringBuilder();
            // buffer for contents of the section
            StringBuilder Paragraph = new StringBuilder();
            for (var i = 0; i < input.Length; i++)
            {
                // Get current character
                char ch = input[i];
                // go down if current character
                if (ch == '{')
                {
                    parserLevel--;
                    // if parserlevel is at root level skip the token
                    if(parserLevel > -1)
                        continue;
                }
                // go up if current character is }
                if (ch == '}')
                {
                    parserLevel++;

                    // If parser level is at root, finalize the current section
                    if (parserLevel == 0)
                    {
                        // Add the paragraph to the buffer
                        Output.Add(Selector.ToString(), Paragraph.ToString());

                        // Clear buffers
                        Output.Clear();
                        Paragraph = new StringBuilder();

                        // if parserlevel is at root level skip the token
                        if (parserLevel < -1)
                            continue;
                    }
                }
                
                // if outside expression gain attention
                if (parserLevel == 0)
                {
                    Selector.Append(ch);
                }
                else
                {
                    Paragraph.Append(ch);
                }
            }

            // Create dictionary for css strings
            Dictionary<String, Dictionary<String, String>> Result = new Dictionary<string, Dictionary<string, string>>();
            
            // Parse substrings
            foreach (KeyValuePair<string, string> item in Output)
            {
                Dictionary<String, string> code = ParseCssString(item.Value);
                Result.Add(item.Key, code);
            }

            // Return the result
            return Result;

        }

        /// <summary>
        /// Source code
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Function to parse an css clausul.
        /// </summary>
        /// <param name="expressions">Ann CSS inline expression { inside }</param>
        /// <returns>An dictionary of expressions inside</returns>
        public Dictionary<String, String> ParseCssString(String expressions)
        {
            // Dictionary for the output values
            Dictionary<String, String> Output = new Dictionary<string, string>();

            // Create an string builder for the expression to put data on
            StringBuilder Attribute = new StringBuilder();
            StringBuilder Value = new StringBuilder();

            // Denotates if inside string
            bool insideString = false;
            // boolean indicating you're on the expression side (after first : token)
            bool inExpression = false;
            for (int i = 0; i < expressions.Length; i++)
            {
                // Get current char
                char c = expressions[i];

                // if char is an " toggle inside string and continue
                if (c == '"')
                {
                    insideString = !insideString;
                    continue;
                }
               
                   

                // If not in an expression create an attribute expression
                if (!inExpression)
                {
                    // If c has reached an : switch over to expression mode
                    if (c == ':')
                    {
                        inExpression = true;
                        continue;
                    }

                    // else append an c
                    Attribute.Append(c);
                }
                else
                {
                    // if c reached an ; end the expression create 
                    // an entry to the dictionary with trimmed whitespaces and not inside an string
                    if (insideString)
                    {
                        continue;
                    }
                    if (c == ';')
                    {
                        Output.Add(Attribute.ToString().Trim(), Value.ToString().Trim());

                        // Reset all variables

                        inExpression = false;

                        // flush all buffers
                        Attribute = new StringBuilder();
                        Value = new StringBuilder();
                        continue;
                    }
                    Value.Append(c);

                }
            }
            return Output;

        }
    }
}
