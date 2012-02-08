using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotDev
{
    /// <summary>
    /// Interface for all editor instances
    /// </summary>
    public interface ISPEditor
    {
        /// <summary>
        /// Saves the editor
        /// </summary>
        void Save();
        /// <summary>
        /// Saves the editor by a file name
        /// </summary>
        /// <param name="fileName"></param>
        void SaveAs(String fileName);
        /// <summary>
        /// Is the file saved
        /// </summary>
        bool IsSaved { get; set; }

        /// <summary>
        /// The content
        /// </summary>
        String Text { get; set; }
        
        /// <summary>
        /// Returns the file name of the editor
        /// </summary>
        String FileName { get; }

    }
}
