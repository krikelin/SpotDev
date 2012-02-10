using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpotDev
{
    /// <summary>
    /// A component like editor, file manager, browser or whatever
    /// </summary>
    public interface ISPComponent
    {
        void Save();
        void Save(String fileName);
        bool IsSaved { get; set; }
        void LoadFile(String fileName);
        void Undo();
        void Redo();
        void Cut();
        void Copy();
        void Paste();
        bool Close();
        event EventHandler Changed;
        event EventHandler Saved;
    }
}
