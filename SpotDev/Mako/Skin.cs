/********************************************************
 * 
 * This is the skin architecture for MediaChrome
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
namespace Board
{
    /// <summary>
    /// Skin architecture.
    /// </summary>
    [XmlRoot("skin",Namespace="http://skin.mediachrome.net/2011/skin-definition")]
    [Serializable]
    public class Skin
    {
        JavaScriptEngine engine;
       
        public String Directory { get; set; }

        public IScriptEngine ScriptEngine
        {
         get{return engine;}
        }
        /// <summary>
        /// Sets css wrapper functions
        /// </summary>
        public void SetCSSFunctions()
        {
             engine.SetFunction("url",new Func<string,object>(__css_url));
        }

        /// <summary>
        /// An wrapper for URL property
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public object __css_url(string url)
        {
            return url;
        }

        /// <summary>
        /// Loads an skin from the defined file
        /// </summary>
        /// <param name="File">file name of the xml skin</param>
        public Skin(String File,bool css = false)
        {
            engine = new JavaScriptEngine();

          // If css mode load the components as CSS instead of files
            if(css)
            {
                CssParser Parser = new CssParser();
                using(StreamReader SR = new StreamReader(File))
                {
                    Dictionary<String,Dictionary<string,string>> result = Parser.ParseCSSFile(SR.ReadToEnd());
                    foreach(KeyValuePair<String,Dictionary<string,string>> item in result)
                    {
                        Component d = new Component(this);
                        d.Rel=item.Key;
                        d.SetCSS( item.Value);
                        this.components.Add(d);
                    }
                }
            }else{
                    
                XmlSerializer r = new XmlSerializer(typeof(Skin));
            Skin a = (Skin)r.Deserialize(new StreamReader(File));
            this.components = a.components;
            }
            // Set this as parent on all component definitions
            foreach (Component d in components)
            {
                d.Parent = this;
            }
            this.Components = new ComponentCollection(this, components);
            this.Directory = new FileInfo(File).Directory.FullName;

        }   
        public void SkinControl(Control C,string part)
        {
            C.BackgroundImage = this.Components[part].BackgroundImage;
            C.ForeColor = this.Components[part].ForeColor;
            C.BackColor = this.Components[part].BackColor;
            C.Font = this.Components[part].Font;

            
        }
        public Skin()
        {
            Components = new ComponentCollection(this, components);
            
        }
       
        [XmlElement("name")]
        public string Name;
        [XmlElement("Author")]
        public string Author;
        [XmlElement("Copyright")]
        public string Copyright;

        
        public ComponentCollection Components;
        /// <summary>
        /// List of components
        /// </summary>
        [XmlElement("component")]
        public List<Component> components;
        
    }
    /// <summary>
    /// Defines an collection of components
    /// </summary>
    public class ComponentCollection : IDictionary<string, Component>
    {
        
       
        public Skin Parent { get; set; }
        public ComponentCollection(Skin skin, List<Component> lst)
        {
            Parent = skin;
            source = lst;
        }
        private List<Component> source;

        public List<Component> Source
        {
            get
            {
                if (source == null)
                    source = new List<Component>();
                return source;
            }
        }

        public void Add(string key, Component value)
        {
            Source.Add( value);
            value.Parent = this.Parent;
            

        }

        public bool ContainsKey(string key)
        {
       
            foreach (Component d in Source)
            {
                if (d.Rel == key)
                    return true;
            } 
            return false;
        }

        public ICollection<string> Keys
        {
            get
            {
                List<String> Keys = new List<string>();
                foreach (Component d in Source)
                    Keys.Add(d.Rel);
                return Keys;
            }
        }

        public bool Remove(string key)
        {
            foreach (Component r in Source)
            {
                if (r.Rel == key)
                {
                    Source.Remove(r);
                    return true;
                }
            }
            return false;
        }

        public bool TryGetValue(string key, out Component value)
        {
            value = this[key];
            return value != null;
        }

        public ICollection<Component> Values
        {
            get { return source; }
        }

        public Component this[string key]
        {
            get
            {
                   foreach(Component r in Source)
                   {
                       if(r.Rel == key)
                           return r;
                   }
                return null;
            }
            set
            {
               Component comp = new Component(this.Parent);
                comp.Rel = key;
                Source.Add(comp);
            }
        }

        public void Add(KeyValuePair<string, Component> item)
        {
            this.Source.Add(item.Value);
        }

        public void Clear()
        {
            this.Source.Clear();
        }

        public bool Contains(KeyValuePair<string, Component> item)
        {
            if (item.Value == this[item.Key])
                return true;
            return false;
        }

        public void CopyTo(KeyValuePair<string, Component>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return Source.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, Component> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, Component>> GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return source.GetEnumerator();
        }
    }

   /// <summary>
   /// An component to skin
   /// </summary>
    [Serializable]
    public class Component
    {
        [NonSerialized]
        private Dictionary<String,string> CssValues;

        /// <summary>
        /// Set CSS values
        /// </summary>
        /// <param name="values"></param>
        public void SetCSS(Dictionary<string, string> values)
        {
            this.CssValues = values;
        }
        public Color GetCSSColor(string key)
        {
            return ColorTranslator.FromHtml(CssValues[key]);
        }
        public Image GetCSSImage(string key)
        {
            Parent.ScriptEngine.Run("return url('" + CssValues[key] + "')");
            return Bitmap.FromFile((string)Parent.ScriptEngine.Invoke("url",CssValues[key]));
        }
        /// <summary>
        /// Gets or sets the parent skin
        /// </summary>
        public Skin Parent {get;set;}
        public Component(Skin Parent)
        {
            this.Parent = Parent;
            CssValues = new Dictionary<string,string>();
            
        }
        public Component()
        {
           CssValues = new Dictionary<string,string>();
            
        }
       
        /// <summary>
        /// The component to skin (an control or element)
        /// </summary>
        [XmlAttribute("rel")]
        public string Rel;
        [XmlAttribute("BackColor")]
        public string backColor;
        [XmlAttribute("ForeColor")]
        public string foreColor;
        [XmlAttribute("SeparatorImage")]    
        public string separatorImage;
        public Image SeparatorImage
        {
            get
            {
                return Bitmap.FromFile(separatorImage.Replace("~",Parent.Directory));
            }
        }
        public Color BackColor
        {
            get
            {
                return ColorTranslator.FromHtml(backColor);
            }
        }
        [XmlAttribute("BackgroundImage")]
        public String backgroundImage;
        public Image BackgroundImage
        {
            get
            {
                try
                {
                    if (!backgroundImage.StartsWith("res:"))
                    {
                        return Bitmap.FromFile(backgroundImage.Replace("~", Parent.Directory));
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {

                    return null;
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the part of the skin is enabled
        /// </summary>
        public bool Enabled { get; set; }
        public Color ForeColor
        {
            get
            {
                return ColorTranslator.FromHtml(foreColor);
            }
        }
       
        private string skin;
        /// <summary>
        /// Family of font
        /// </summary>
        [XmlAttribute("FontFamily")]
        private string fontFamily { get; set; }

        [XmlAttribute("Bold")]
        private bool bold { get; set; }

        /// <summary>
        /// Font size
        /// </summary>
        [XmlAttribute("FontSize")]
        private float fontSize {get;set;}

        /// <summary>
        /// Gets the font
        /// </summary>
        public Font Font
        {
            get
            {
                if (fontSize == 0)
                    fontSize = 8;

                return new Font(fontFamily, fontSize, FontStyle.Regular);
            }
        }
        /// <summary>
        /// Skin component
        /// </summary>
        public string Skin
        {
            get
            {
                return skin;
            }
            set { skin = value; }
        }
    }
}
