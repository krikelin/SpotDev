﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Net;
using System.IO;
using System.Threading;
using System.Xml;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
namespace Board
{

    [Serializable]
    public class Spofity
    {
        /// <summary>
        /// Gets or sets if the view is live
        /// </summary>
        public bool Live { get; set; }
        private Stack<int> historySections;
        private Stack<int> forwardSections;
        /// <summary>
        /// History of section
        /// </summary>
        public Stack<int> HistorySections
        {
            get
            {
                if (historySections == null) historySections = new Stack<int>();
                return historySections;
            }

        }

        /// <summary>
        /// History of forward sections
        /// </summary>
        public Stack<int> ForwardSections
        {
            get
            {
                if (forwardSections == null) forwardSections = new Stack<int>();
                return forwardSections;
            }

        }

        /// <summary>
        /// Goback an section
        /// </summary>
        /// <returns></returns>
        public int GoBack()
        {
            if (HistorySections.Count > 0)
            {
                ForwardSections.Push(this.currentSection);
                int d = HistorySections.Pop();

                return d;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// Go forward an sectionn
        /// </summary>
        /// <returns></returns>
        public int GoForward()
        {
            if (ForwardSections.Count > 0)
            {
                HistorySections.Push(this.currentSection);
                int d = ForwardSections.Pop();
                return d;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// Delegate which manage events for mako creation
        /// </summary>
        /// <param name="sender">the current instance to mako</param>
        /// <param name="e">eventargs</param>
        public delegate void MakoCreateEventHandler(object sender, EventArgs e);

        /// <summary>
        /// Occurs when the mako template engine has been init
        /// </summary>
        public event MakoCreateEventHandler MakoGeneration;
        /// <summary>
        /// Gets or sets the parent view for this Spofity instance
        /// </summary>
        public DrawBoard.View ParentView { get; set; }
        /// <summary>
        /// Delegate to manage playbacks from the view's list
        /// </summary>
        /// <param name="sender">The element sender</param>
        /// <param name="uri">The uri to the current playing</param>
        /// <returns>A boolean whether the playing could be started or not</returns>
        public delegate bool ElementPlaybackStarted(Spofity sender, Element element, String uri);


        /// <summary>
        /// occurs when an track has been choosed for playback
        /// </summary>
        public event ElementPlaybackStarted PlaybackStarted;

        /// <summary>
        /// This variable holds the collection of the elements at the Layout Element phase
        /// </summary>
        public XmlDocument LayoutElements { get; set; }

        /// <summary>
        /// The instance to the script engine that rendered the layout elements, and will continue it's lifecycle as helper
        /// scripts for various tasks after the preprocessing.
        /// </summary>
        public IScriptEngine ScriptEngine
        {
            get
            {
                return Engine.RuntimeMachine;
            }
        }
        /// <summary>
        /// Invokes on synchronization
        /// </summary>
        /// <param name="section"></param>
        public void synchronizeContent(object section)
        {
            
            Engine.Invoke("event_flow_update",section);

        }
        /// <summary>
        /// The instance of the makoEngine that rendered the view.
        /// </summary>
        public MakoEngine Engine { get; set; }
        /// <summary>
        /// Play an item
        /// </summary>
        /// <param name="item"></param>
        public void PlayItem(Element item, int Position, int section)
        {
            /**
             * If view is flow, begin sync new content
             * */
            if (View.Sections[CurrentSection].Flow)
            {
                // Request download of new elements
                Thread d = new Thread(synchronizeContent);
                d.Start();
            }
            Playlist = new Queue<Element>();
            // Get the list of elements
            List<Element> elements = this.View.Sections[section].Elements;

            // Remove all play item
            this.ParentBoard.RemoveAllPlaying();
            /**
                * Iterate through the elements, once it has reached the one equal to the input item,
                * begin collecting all items which are classified with the type 'Entry'
                * */

            bool found = false;
            foreach (Element d in elements)
            {
                // If the current has been found and the item is an entry add it to the playlist
                if (found && d.Entry)
                {
                    Playlist.Enqueue(d);
                }

                if (d == item)
                {
                    // Set the playing attribute to true
                    d.SetAttribute("__playing", "true");
                    found = true;
                    continue;
                }
                else
                {
                    d.SetAttribute("__playing", "");
                }

            }

            // Raise the playback started event
            if (PlaybackStarted != null)
                PlaybackStarted(this, item, item.GetAttribute("uri"));
        }
        /// <summary>
        /// Called by the script to initiate an ajax-like process of new elements
        /// </summary>
        /// <param name="adress"></param>
        /// <param name="callback"></param>
        /// <returns>true if sucess, false if not</returns>
        public object __downloadContentAsync(string adress, string callback)
        {
            try
            {
                // Create new content receiver
                ContentReceiver D = new ContentReceiver();

                // define parameters
                D.Adress = adress;
                D.Callback = callback;

                // Start an individual transmittion inside each content receiver
                Thread CN = new Thread(D.DownloadData);
                CN.Start(D.Adress);
                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// List with all content receivers. It are checked regulary for finished downloads.
        /// </summary>
        public List<ContentReceiver> Receivers { get; set; }

        /// <summary>
        /// Content receiver is an class which performs as an storage of downloaded content, which will be handled
        /// separetely
        /// </summary>
        public class ContentReceiver
        {
            /// <summary>
            ///  The adress to the remote or local resource
            /// </summary>
            public string Adress { get; set; }

            /// <summary>
            /// The object received. Package is an boxed false if the download were incomplete.
            /// </summary>
            public object Package { get; set; }

            /// <summary>
            /// Returns if the ContentReceiver has finished transmittion of content
            /// </summary>
            public bool Ready
            {
                get
                {
                    return Package != null;
                }
            }
            /// <summary>
            /// The callback of the event to raise together with the package once deliverd
            /// </summary>
            public string Callback { get; set; }

            /// <summary>
            /// Synchronize data is called by the javascript preparser to get an ready to use JSON parsed data. If the dat can't be parsed as JSON
            /// it will be returned as an common string
            /// </summary>
            /// <param name="receiver">An boxed instance of an ContentReceiver class</param>
            /// <returns></returns>
            public void DownloadData()
            {
#if(obsolote)
                ContentReceiver Receiver = this;
                string uri = Receiver.Adress;
                // Create web request
                WebClient WC = new WebClient();
                /**
                 * Try getting data. If no data was got an all, return FALSE
                 * */
                try
                {
                    String jsonRaw = WC.DownloadString(new Uri((string)uri));
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(jsonRaw);
                    Receiver.Package = jsonRaw;
                    // Convert it to JSON
                    try
                    {
                        Jint.JintEngine Engine = new Jint.JintEngine();
                        Jint.Native.JsObject D = new Jint.Native.JsObject((object)jsonRaw);

                        // Try parse it as json, otherwise try as xml and if not retuurn it as an string
                        try
                        {
                            // Do not allow CLR when reading external scripts for security measurements
                            System.Web.Script.Serialization.JavaScriptSerializer d = new System.Web.Script.Serialization.JavaScriptSerializer();
                            object json = d.DeserializeObject(jsonRaw);
                            Receiver.Package = json;
                        }
                        catch
                        {
                        
                        }


                    }
                    catch
                    {
                        Receiver.Package = jsonRaw;
                    }
                }
                catch
                {

                    Receiver.Package = false;
                }
#endif
            }
        }
        private int currentSection = 0;
        public int CurrentSection
        {
            get
            {
                return currentSection;

            }
            set
            {
                currentSection = value;


            }
        }
        /// <summary>
        /// Get the section which has the current playing item
        /// </summary>
        /// <returns></returns>
        public Section GetPlayingSection()
        {

            foreach (Section t in this.View.Sections)
            {
                foreach (Element _elm in t.Elements)
                {
                    if (_elm.Entry)
                    {
                        if (_elm.GetAttribute("__playing") == "true")
                        {
                            return t;
                        }
                    }
                }
            }


            return null;
        }

        /// <summary>
        /// Set item after current playing as next song.
        /// </summary>
        public void PreviousSong()
        {

            // Raise the playback event again
            GetPlayingSection().PlayIndex--;
        }

        /// <summary>
        /// Set item after current playing as next song.
        /// </summary>
        public void NextSong()
        {

            // Raise the playback event again
            GetPlayingSection().PlayIndex++;
        }
        /// <summary>
        /// The drawboard has an playlist stack which is used for listing of media items.
        /// </summary>
        public Queue<Element> Playlist { get; set; }

        /// <summary>
        /// The element which represents the now playing song
        /// </summary>
        public Element NowPlaying { get; set; }


        public delegate void ActionEvent();
        public event ActionEvent BeginLoading;
        public event ActionEvent FinishedLoading;
        public bool Loaded { get; set; }
        private View view;
        public View View
        {
            get
            {
                return view;
            }
            set
            {
                view = value;
            }
        }
        private string uri;
        public string URI
        {
            get
            {
                return uri;
            }
            set
            {
                uri = value;
            }
        }
        Stream RawData;
        public void Process()
        {


        }
        private int topPos = 0;
        public int TopPos
        {
            get
            {
                return topPos;
            }
            set
            {
                topPos = value;
            }
        }
        private int countItems = 0;
        public int CountItems
        {
            get
            {
                return countItems;
            }
            set
            {
                countItems = value;
            }
        }

        public static int ITEM_HEIGHT = 20;
        public static int LIST_LEFT = 140;
        public static void SecElement(ref Element X, Spofity R)
        {
            X.AssertBounds(false);
            /*int topPos = R.TopPos;
            if(X.GetAttribute("position")==("absolute"))
            {
                try
                {
                X.Top = int.Parse(X.GetAttribute("top").Replace("@top",topPos.ToString()));
                X.Left = int.Parse(X.GetAttribute("left"));
                }
                catch
                {
                    X.Top = 0;
                    X.Left = 0;
                }
            }
            else
            {
                X.Top = topPos;
                X.Left = 0;
            }
	                		
                        switch(X.Type)
                        {
	                				
                            case "sp:space":
	                			
                                topPos+=int.Parse(X.GetAttribute("distance"));
	                				
                                break;
                            case "sp:entry":
                                X.Height = ITEM_HEIGHT;
                                if(X.GetAttribute("position")!="absolute")
                                {
                                    X.Left = LIST_LEFT;
                                    X.Width =-1;
                                    X.Top = topPos;
                                }
	                			
	                				
                                break;
                            case "sp:header":
                                if(X.GetAttribute("position")!="absolute")
                                {
                                    X.Left = LIST_LEFT;
                                    X.Width =-1;
                                    X.Top = topPos;
                                }
                                break;
                            case "sp:label":
                                if(X.GetAttribute("position")!="absolute")
                                {
                                    X.Left = LIST_LEFT;
                                    X.Width =-1;
                                    X.Top = topPos;
                                }
                                        break;
                            case "sp:section":
                                X.Height = ITEM_HEIGHT;
                                if(X.GetAttribute("position")!="absolute")
                                {
	                				
                                    X.Left = LIST_LEFT;
                                }
                                X.Width = -1;
                                break;
                            case "sp:image":
                                X.Width = int.Parse(X.GetAttribute("width"));
                                X.Height = int.Parse(X.GetAttribute("height"));
                                break;
                        }
                        if(X.GetAttribute("position")!="absolute")
                        {
                                topPos+=ITEM_HEIGHT;
                                R.topPos+=ITEM_HEIGHT;
                        }
                    R.CountItems++;*/
        }
        public void Serialize()
        {


        }
        /// <summary>
        /// This method will convert the xml attributes to element attributes
        /// </summary>
        /// <param name="elm">The target element to manage (reference)</param>
        /// <param name="node">The element to inherit the attributes from</param>
        public void AppendElementAttributes(ref Element elm, XmlElement node)
        {
            foreach (XmlAttribute _attribute in node.Attributes)
            {
                // Lower attribute names so we don't get any trouble
                elm.SetAttribute(_attribute.Name.ToLower(), _attribute.Value);
            }
            /**
             * 2011-04-25 17:12 
             * The song name screwed up so much so we decided to change many things in the application!
             * Now attributes to the element can be done through sub elements!
             * */
            foreach (XmlNode CT in node.ChildNodes)
            {
                if (CT.GetType() == typeof(XmlElement))
                {
                    elm.SetAttribute(CT.Name.ToLower(), CT.InnerText);
                }
            }
        }

        /// <summary>
        /// Fetch data from either hard drive or remote web address
        /// </summary>
        /// <param name="address"></param>
        /// <returns>the text data if success, "ERROR "+Message if failed</returns>
        public String SynchronizeData(String address)
        {
            // If address starts with http: loockup it at an remote source and downloda it if possible
            if (address.StartsWith("http:"))
            {
                WebClient WC = new WebClient();
                try
                {
                    String data = WC.DownloadString(address);
                    return data;
                }
                catch (Exception e)
                {
                    return "ERROR: " + e.Message;
                }
            }
            else
            {
                try
                {
                    String result = "";
                    // Grab data from local harddrive
                    using (StreamReader SR = new StreamReader(address))
                    {

                        result = SR.ReadToEnd();
                    }
                    return result;
                }
                catch (Exception e)
                {
                    return "ERROR: " + e.Message;
                }
            }
        }



        #region ScriptFunctions

        /// <summary>
        /// callback for manipulation of layout elements during runtime
        /// </summary>
        /// <param name="name">the name of the tag</param>
        public XmlNodeList __getElementsByTagName(string name)
        {
            return LayoutElements.GetElementsByTagName(name);
        }
        /// <summary>
        /// callback for manipulation of layout elements during runtime
        /// </summary>
        /// <param name="name">the name of the tag</param>
        public object __getElementId(string name)
        {
            object cf = GetElementById(LayoutElements.DocumentElement, name);
            return cf;
        }
        /// <summary>
        /// Method to set inner content of an element from scripside
        /// </summary>
        /// <param name="element"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public object __setContent(object element, string content)
        {
            XmlElement elm = (XmlElement)element;
            elm.InnerText = content;
            return null;
        }
        /// <summary>
        /// Method to set an attribute to an certin value on an layout element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public object __setAttribute(object element, string attribute, string value)
        {
            XmlElement elm = (XmlElement)element;
            elm.SetAttribute(attribute, value);
            return null;
        }
        /// <summary>
        /// Own implementation of getElementById as XmlDocument's getelementsbyId could not be
        /// used without DTD
        /// </summary>
        /// <param name="src">the xml document to work on</param>
        /// <param name="ID">the id of the element to find</param>
        /// <remarks>This function is recursive</remarks>
        /// <returns></returns>
        public XmlElement GetElementById(XmlElement src, string val)
        {

            XmlElement getElement = null;
            if (src.HasAttribute("id"))
                if (src.GetAttribute("id") == val)
                    return src;
            foreach (XmlElement Node in src.ChildNodes)
            {
                if (Node.NodeType == XmlNodeType.Element)
                {
                    XmlElement elm = (XmlElement)Node;
                    getElement = GetElementById(elm, val);
                    if (getElement != null)
                        return getElement;

                }
            }
            return null;
        }

        /// <summary>
        /// This method should be called by the scripting engine after an modification so the element
        /// buffer will be rebuilt with the changes in the layout engine.
        /// </summary>
        public object __commit_changes()
        {
            Render(this.LayoutElements);
            return null;
        }
        #endregion
        /// <summary>
        /// DATE: 2011-04-20
        /// If the tag name is %Inflater the element can inflate another view,
        /// preparse it according to arbitrary parameters and then
        /// return it to this layer. 
        /// </summary>
        /// <param name="CT">The element to use as inflater caller</param>
        /// <param name="host">The element to inflate from</param>
        /// <remarks>
        /// The script running when inflating the child view is not able to interact with the host session,
        /// and have their own scope.
        /// </remarks> 
        public void InflateView(Section srcSection, Element item, XmlNode CT)
        {


            // Create additional makoengine
            MakoEngine ME = new MakoEngine();
            if (this.MakoGeneration != null)
                this.MakoGeneration(ME, new EventArgs());
            XmlElement _Element = (XmlElement)CT;
            String ViewMako = "";
            // Load the view by the attribute src but if the name starts with component_ inflate instead it from the tag name in the folder components
            if (CT.Name.StartsWith("component_"))
            {
                ViewMako = SynchronizeData(CT.Name.Replace("component_", ""));
            }
            else
            {
                ViewMako = SynchronizeData(_Element.GetAttribute("src"));
            }

            // If the operation failed leave the function
            if (ViewMako.StartsWith("ERROR:"))
                return;

            // Add all properties of the element as arg_{attribute} to the child layer
            foreach (XmlAttribute attr in CT.Attributes)
            {
                ME.RuntimeMachine.SetVariable("arg_" + attr.Name, attr.Value);
            }

            // Add all text child elements of the element as arg_{attribute} to the child layer
            foreach (XmlElement attr in CT.ChildNodes)
            {
                ME.RuntimeMachine.SetVariable("arg_" + attr.Name, attr.InnerText);
            }


            // Otherwise preprocess the layer.
            String Result = ME.Preprocess(ViewMako, "", true,srcSection.Parent.URI,true);

            // then inflate the data
            // Create xml document
            XmlDocument d = new XmlDocument();
            d.LoadXml(Result);

            // All inflates can only have one section
            XmlNodeList Sections = d.GetElementsByTagName("section");
            XmlElement iSection = (XmlElement)Sections[0];
            {
                // Create new section
                Section _Section = new Section(this);



                // Inflate all elements of the subsection into the list
                RenderSection(srcSection, iSection);

            }

        }



        /// <summary>
        /// This is an recursive method which will create elements to the 
        /// Board to view.
        /// </summary>
        /// <remarks>This function calls iself. The amount of elements supported is limited to enasure portably and does not follow the whole w3c web standard</remarks>
        /// <param name="C">The base element to begin on</param>
        /// <param name="iSection">The section to start with</param>
        /// <param name="srcSection">The section for the elements to apply on, used by the inflater</param>
        public Element RenderElements(Section srcSection, Element C, XmlElement iSection)
        {
            Element prevElement = null;
            // Iterate through the fields and create elements
            XmlNodeList items = iSection.ChildNodes;
            foreach (XmlNode node in items)
            {
                
                if (node.GetType() == typeof(XmlText))
                {
                    XmlText r = (XmlText)node;

                    continue;
                }
                if (node.GetType() != typeof(XmlElement))
                    continue;

                // Skip the element if this is an attribute
                if (node.Attributes.GetNamedItem("noelm") != null)
                    continue;

                XmlElement item = (XmlElement)node;
                if (item.ParentNode != iSection)
                    continue;

                // Create various kind of controls depending of node type
                Element CT = new Element(srcSection, this.ParentBoard);
                if (prevElement != null)
                {
                    CT.PreviousElement = prevElement;
                    prevElement.NextElement = CT;
                } CT.PreviousElement = prevElement;
                CT.Parent = C;
                // If nothing else specified, the element top should be managed by the drawing cache
                CT.SetAttribute("top", "@TOP");

                // Assert an style template if provided
                if (item.Name == "inflate")
                {
                    InflateView(srcSection, CT, node);
                    continue;
                }

                Dictionary<String, String> Style = new Dictionary<string, string>();
                item.HasAttribute("style");
                CssParser parser = new CssParser();
                Dictionary<String, String> values = parser.ParseCssString(item.GetAttribute("style"));
                CT.stylesheet = values;

                // Append all custom attributes first
                AppendElementAttributes(ref CT, item);
                #region InflaterService



                #endregion

                // Set the type of the element to the item's tag definition
                CT.SetAttribute("type", item.Name);
                CT.SetAttribute("text", item.InnerText);
                CT.Data = CT.GetAttribute("textonly") == "true" ? item.InnerText : item.InnerXml;
                //     ExtractXml(item, CT);


                // By default the top of the element should be set according to the page settings
                CT.SetAttribute("top", "@TOP");

                // Convert attribute bounds to native ones
                CT.AssertBounds(false);

                // The integer specifies the top position. @TOP token means this variable should set the height


                // Tweek the element behaviour according to some specific tag names

                switch (item.Name.ToLower())
                {
                    case "p":
                        CT.SetAttribute("type", "label");
                        break;
                    case "h1":




                        CT.SetAttribute("size", "15");
                        break;
                    case "button":



                        CT.SetAttribute("text", item.InnerText);
                        CT.Data = item.InnerText;

                        break;
                    case "img":

                        CT.SetAttribute("type", "image");

                        break;
                }

                C.Elements.Add(CT);

                // Do this for children
                this.ParentBoard.RaiseElementAddEvent(CT);
                CT = RenderElements(srcSection, CT, item);

                // set ct as previous element
                prevElement = CT;

            }
            return C;
        }
        public enum ParseMode
        {
            Beginning, Attribute, Value
        }
        public string ExtractTagAttribute(Element Output, string tag)
        {
            /***
             * Create new xml document and parse the tag
             * */
            XmlDocument XD = new XmlDocument();
            String xml = "<xml>" + tag + "</xml>";

            XD.LoadXml(xml);

            var elm = XD.DocumentElement.ChildNodes[0];

            /**
             * Convert the element to an Spofity element
             * */

            Output.Type = elm.Name; // Set type of element

            // Assert properties
            foreach (XmlAttribute r in elm.Attributes)
            {
                Output.SetAttribute(r.Name, r.Value);
            }

            // Assert child elements
            foreach (XmlNode Node in elm.ChildNodes)
            {
                Output.SetAttribute(Node.Name, Node.InnerText);
            }
            // Return the element type (TagName)
            return elm.Name;
        }
        /// <summary>
        /// Extract all [attrb=value] implented in the string and return the tag name
        /// 
        /// </summary>
        /// <param name="Output">The element to apply</param>
        /// <param name="tag">The tag to harash</param>
        /// <returns></returns>
        public string ExtractTagAttributes(Element Output, string tag)
        {

            return ExtractTagAttribute(Output, tag);

            /**
             * Current character mode 
             * [ 0 = Beginning of tag,
             *   1 = At attribute name,
             *   2 = inside attribute value ]
             *   */
            ParseMode currentState = ParseMode.Beginning;

            /**
             * If the cursor is inside an "" this variable
             * will be true, and if so it will skip the whitespace
             * */

            bool insideString = false;

            StringBuilder buffer = new StringBuilder();                 // The buffer of chars for the current token

            String bufferReady = "";                                    // The buffer of prevous ready string (attribute name)

            String elementName = "";                                    // The tag name of the element

            List<Board.Attribute> attributes = new List<Attribute>();   // The list of attributes to create

            for (int i = 0; i < tag.Length; i++)
            {

                char token = tag[i];

                /**
                 * If the current character is an " toggle
                 * inside buffer (do not change mode on whitespace)
                 * */
                if (token == '"')
                {
                    insideString = !insideString;
                    continue;
                }

                switch (currentState)
                {
                    case ParseMode.Attribute:

                        if (token == ' ' && !insideString)
                        {
                            /** Flush the buffer and move the content
                             * to the attribute bufffer */
                            bufferReady = buffer.ToString();
                            buffer = new StringBuilder();
                            // Set parse mode to attribute
                            currentState = ParseMode.Value;
                            continue;

                        }
                        break;
                    case ParseMode.Value:
                        if (token == ' ' || token == '>' && !insideString)
                        {
                            if (tag[token - 1] == ' ')
                                continue;
                            // Get the value
                            String value = buffer.ToString();
                            buffer = new StringBuilder();
                            // Create element's attribute
                            Board.Attribute d = new Attribute() { name = bufferReady, value = value };
                            // add the attribute to the element
                            Output.Attributes.Add(d);

                            // If not inside string and reach > or / return
                            if ((token == '>' || token == '/') && !insideString)
                            {
                                return elementName;
                            }
                            else
                            {
                                currentState = ParseMode.Attribute;
                            }
                            continue;
                        }
                        break;
                    case ParseMode.Beginning:
                        if (token == ' ')
                        {
                            if (tag[token - 1] == ' ')
                                continue;
                            elementName = buffer.ToString();
                            // Clear the buffer
                            buffer = new StringBuilder();

                            // Set parse mode to attribute
                            currentState = ParseMode.Attribute;
                            continue;
                        }
                        break;

                }

                // Otherwise append the char to the current buffer
                buffer.Append(token);


            }
            return elementName;
        }
        /// <summary>
        /// Extract element from tag and put it into CT
        /// </summary>
        /// <param name="i">ref int to position</param>
        /// <param name="cf"></param>
        /// <param name="CT"></param>
        public void ExtractTag(ref int totalSize, ref int i, string Cf, Element CT)
        {
            /**
             * Get element contents
             * */

            // Get starttag
            var j = Cf.IndexOf('<', i);        // get index < of first position

            var k = Cf.IndexOf('>', j);             // get index of ending > of tag name

            /** 
             * Handle short tag elements
            */
            if (Cf.Substring(k - 1, 1) == "/")
            {
                Element bOutput = new Element(CT.ParentSection, this.ParentBoard);

                // Get the element
                string type_ = Cf.Substring(j + 1, (k - 1) - j - 2).Trim();
                string type = ExtractTagAttribute(bOutput, type_);
                bOutput.SetAttribute("type", type);
                bOutput.Type = type;
                CT.Elements.Add(bOutput);
                int t_pos_ = j - totalSize;
                bOutput.SetAttribute("t_pos", j.ToString());
                i = Cf.IndexOf("/>", j + 1);

                // Set the length of the element
                bOutput.SetAttribute("t_length", (type_ + 3).Length.ToString());
                return;
            }
            var elmName = Cf.Substring(j + 1, k - j - 1);   // Extract tag name


            // Get Contents
            var m = Cf.IndexOf("</", k);            // Get index of end tag

            var elmContents = Cf.Substring(k + 1, m - k - 1); // Extract tag contents

            i += (k - j) - (m - k) + 3;

            /**
             * Create output element. Tell it can be implemented to the type
             * */
            Element Output = new Element(CT.ParentSection, this.ParentBoard); // Create new element
            int t_pos = j - totalSize;
            Output.SetAttribute("t_pos", j.ToString()); // set element textual position


            var tagName = ExtractTagAttributes(Output, elmName);
            Output.SetAttribute("type", tagName);       // Set the type of inline markup to the elmName
            // increase totalsize
            Output.SetAttribute("t_length", (elmName.Length + elmContents.Length + 4 + 1 + tagName.Length * 2).ToString());
            CT.Elements.Add(Output);                // Add the element to children


            Output.Data = elmContents;              // Set output's textContent


        }

        public void ExtractXml(XmlNode item, Element CT)
        {
            // Extract contents to string

            //    Elements
            var childNodes = item.ChildNodes;

            // The current node
            int currentNode = 0;

            // the current element
            XmlElement currentElm = null;
            CT.Data = item.InnerText;
            CT.InnerXML = item.InnerXml;

            // Numeric opinter to the child elements
            int elmPointer = 0;
            /***
             * Mark all occuranses of XML tags
             * */

            // totalsize of all elements
            int totalSize = 0;
            for (int i = 0; i < item.InnerXml.Length; i++)
            {
                if (i > 0)
                {
                    char ch = item.InnerXml[i];
                    // If the current char is an < extract next child node
                    if (ch == '<')
                    {
                        if (item.InnerXml[i + 1] == '/')
                        {
                            // jump to position after next > 
                            i = item.InnerXml.IndexOf('>', i);
                            continue;
                        }

                        // Store current I
                        int oldI = i;
                        // get element tag name and populate the item
                        ExtractTag(ref totalSize, ref i, item.InnerXml, CT);


                        // Remove the string encloseed by the tag
                        continue;
                    }
                }

            }


        }
        /// <summary>
        /// This is an recursive method which will create elements on an section
        /// Board to view.
        /// </summary>
        /// <remarks>This function calls iself. The amount of elements supported is limited to enasure portably and does not follow the whole w3c web standard</remarks>
        /// <param name="C">The base element to begin on</param>
        /// <param name="iSection">The section to start with</param>
        public Section RenderSection(Section C, XmlElement iSection)
        {
            // Previous element
            Element prevElement = null;
            // Iterate through the fields and create elements
            XmlNodeList items = iSection.ChildNodes;
            foreach (XmlNode node in items)
            {
                if (node.GetType() != typeof(XmlElement))
                    continue;
                XmlElement item = (XmlElement)node;
                // Create various kind of controls depending of node type
                Element CT = new Element(C, this.ParentBoard);

                // if previous element is not null, set it's next element to the current
                if (prevElement != null)
                {
                    CT.PreviousElement = prevElement;

                    prevElement.NextElement = CT;
                }
                // Skip the element if this is an attribute
                if (node.Attributes.GetNamedItem("noelm") != null)
                    continue;
                // If nothing else specified, the element top should be managed by the drawing cache
                CT.SetAttribute("top", "@TOP");

                // Assert an style template if provided
                if (item.Name == "inflate")
                {
                    InflateView(C, CT, node);
                    continue;
                }

                // Assert an style template if provided

                Dictionary<String, String> Style = new Dictionary<string, string>();
                item.HasAttribute("style");
                CssParser parser = new CssParser();
                Dictionary<String, String> values = parser.ParseCssString(item.GetAttribute("style"));

                // Append all custom attributes first
                AppendElementAttributes(ref CT, item);
                // Set the type of the element to the item's tag definition
                CT.SetAttribute("type", item.Name);
                CT.AssertBounds(false);
               
               
                CT.Type = item.Name;
                CT.SetAttribute("text", item.InnerText);
                CT.Data = CT.GetAttribute("textonly") == "true" ? item.InnerText : item.InnerXml;

                // Extract the elent's inner data, but only if it's attribute is set to content
                ///    if(item.HasAttribute("content"))
                //      ExtractXml(item, CT);

                // By default the top of the element should be set according to the page settings
                CT.SetAttribute("top", "@TOP");

                // The integer specifies the top position. @TOP token means this variable should set the height


                // Tweek the element behaviour according to some specific tag names

                switch (item.Name.ToLower())
                {
                    case "a":
                        CT.SetAttribute("type", "label");
                        break;
                    case "p":
                        CT.SetAttribute("type", "label");
                        break;
                    case "h1":




                        CT.SetAttribute("size", "15");
                        break;
                    case "button":



                        CT.SetAttribute("text", item.InnerText);
                        CT.Data = item.InnerText;

                        break;
                    case "img":

                        CT.SetAttribute("type", "image");

                        break;
                }
                this.ParentBoard.RaiseElementAddEvent(CT);
                C.rawList.Add(CT);
              
                // set previous next element to current one
                prevElement = CT;
                
                // Do this for children but only for entries
             
                    RenderElements(C, CT, item);
          
               
            }
            return C;
        }

        public Thread loadThread;
        public void LoadData()
        {

            if (BeginLoading != null)
                BeginLoading();
            loadThread = new Thread(Process);
            loadThread.Start();
        }
        /// <summary>
        /// Render the layoutElements into real elements
        /// </summary>
        public void Render(XmlDocument d)
        {
            //    try
            {



                // Set the xmlHashCode to the xmldocument's instance so it won't be any collision
                this.xmlHashCode = d.GetHashCode();
                // Set this layoutelements to the xmldocument loaded
                this.LayoutElements = d;

                // Create new ghost view
                this.View = new View();
                this.Live = d.DocumentElement.HasAttribute("live");
                // iterate through all sections of the page
                XmlNodeList Sections = d.GetElementsByTagName("section");
                foreach (XmlElement iSection in Sections)
                {

                    // If the section element not has the root element as parent skip it
                    if (iSection.ParentNode.Name != "view")
                        continue;
                    // Create new section
                    Section _Section = new Section(this);

                    // Set the section's reorder mode
                    if (iSection.HasAttribute("reorder"))
                        if (iSection.GetAttribute("reorder") == "true")
                            _Section.Reorder = true;

                    // Append nowplaying handler
                    _Section.PlaybackItemChanged += new ElementPlaybackStarted(_Section_PlaybackItemChanged);

                    // set section name
                    _Section.Name = iSection.GetAttribute("name");

                    // set section as an list (show listheaders) if list attribute exists
                    _Section.List = iSection.HasAttribute("list");
                    _Section.Flow = iSection.HasAttribute("flow");
                    _Section.Alternating = !iSection.HasAttribute("noalt");
                    _Section.Locked = !iSection.HasAttribute("editable");
                    // Render element sections
                    _Section = RenderSection(_Section, iSection);
                    _Section.Header = !iSection.HasAttribute("noheader");


                    /**
                     * Set column headers by the column element is defined, define all columnheaders
                     * */

                    XmlNodeList Columns = iSection.GetElementsByTagName("columnheader");

                    // If the column count is above zero, clear the default set of column headers
                    // and use from the new list
                    if (Columns.Count > 0)
                        _Section.ColumnHeaders = new Dictionary<string, int>();


                    foreach (XmlElement ColumnHeader in Columns)
                    {
                        try
                        {
                            _Section.ColumnHeaders.Add(ColumnHeader.GetAttribute("name"), int.Parse(ColumnHeader.GetAttribute("size")));
                        }
                        catch
                        {
                        }
                    }

                    this.View.Sections.Add(_Section);
                    _Section.ptop = 20;
                    if (this.View != null)
                        if (this.View.Sections != null)
                            // Merge properties of all entries from old view to the new
                            foreach (Section _section1 in this.View.Sections)
                            {
                                foreach (Element er in _section1.Entries)
                                {
                                    foreach (Section newSection in this.View.Sections)
                                    {

                                        foreach (Element newElement in newSection.Entries)
                                        {
                                            /***
                                             * Copy all attribute states on the old elements to the new one
                                              **/
                                            if (newElement.GetAttribute("uri") == er.GetAttribute("uri"))
                                            {
                                                if (er.GetAttribute("__playing") == "true")
                                                {
                                                    newElement.SetAttribute("__playing", "true");
                                                }
                                                if (er.Selected)
                                                {
                                                    newElement.SetAttribute("__selected", "true");
                                                    newElement.Selected = true;
                                                }
                                            }
                                        }
                                    }
                                }

                                // Add the element to the buffer
                                // _section1.SortedBuffer.AddRange(_section1.Elements);

                            }

                    // Copy the elements to an secure buffer

                }

                /***
                 * 2011-04-23 23:03
                 * Load toolbar
                 * */
                XmlNodeList toolItems = d.GetElementsByTagName("toolbar");

                // If the toolbar has elements inflate them
                if (toolItems.Count > 0)
                {
                    XmlNodeList ToolItems = ((XmlElement)toolItems[0]).GetElementsByTagName("item");

                    // Extract all items
                    foreach (XmlElement item in ToolItems)
                    {
                        if (item.GetType() == typeof(XmlElement))
                        {
                            XmlElement Item = (XmlElement)item;

                            // dummy toolsection
                            Section toolSection = new Section(this);
                            Element _Item = new Element(toolSection, this.ParentBoard);

                            // set item type according to tag name
                            _Item.Type = Item.Name;

                            // Attach the element's attributes
                            AppendElementAttributes(ref _Item, item);

                            // if the item has the type menu inflate it's menuitems
                            if (_Item.GetAttribute("type") == "menu")
                            {
                                XmlNodeList MenuItems = item.GetElementsByTagName("menuitem");
                                foreach (XmlElement menuItem in MenuItems)
                                {
                                    Element _menuItem = new Element(toolSection, this.ParentBoard);
                                    _menuItem.Type = "menuitem";
                                    // Append xml attributes
                                    AppendElementAttributes(ref _menuItem, menuItem);
                                    _Item.Elements.Add(_menuItem);
                                }

                            }

                            // add the item to the menubar
                            this.view.Toolbar.Items.Add(_Item);


                        }
                    }
                }
#if(nobug)
                // Reset the ptop
               
                // Clear current buffer 
                this.view.Sections.Clear();


                // Create xml document
                XmlDocument d = this.LayoutElements;

                if (d == null)
                    return;
                this.View = new View();
                // iterate through all sections of the page
                XmlNodeList Sections = d.GetElementsByTagName("section");
                foreach (XmlElement iSection in Sections)
                {
                   

                    // If the section element not has the root element as parent skip it
                    if (iSection.ParentNode.Name != "view")
                        continue;
                    // Create new section
                    Section _Section = new Section(this);

                    // Set the section's reorder mode
                    if (iSection.HasAttribute("reorder"))
                        if (iSection.GetAttribute("reorder") == "true")
                            _Section.Reorder = true;

                    // set section name
                    _Section.Name = iSection.GetAttribute("name");

                    // set section as an list (show listheaders) if list attribute exists
                    _Section.List = iSection.HasAttribute("list");

                    // Render element sections
                    _Section = RenderSection(_Section, iSection);

                    this.view.Sections.Add(_Section);


                }

                SetScriptFunctionality();
                LoadData();
#endif

            }
            //           catch
            {

            }
        }
        /// <summary>
        /// Hashcode of the LayoutElements XMLDocument
        /// </summary>
        private int xmlHashCode = 0;

        /// <summary>
        /// Returns whether the LayoutElements has been changed since last occuration
        /// </summary>
        public void CheckPendingChanges()
        {
            try
            {

                /** If the hashcode of the LayoutElements is different from the one stored
                 * there is an update ongoing
                 * */
                if (xmlHashCode != LayoutElements.GetHashCode())
                {
                    xmlHashCode = LayoutElements.GetHashCode();

                    // Update the views

                }

            }
            catch { }
        }
        /// <summary>
        /// Should be called from an background thread to update the layout elements to new ones. 
        /// </summary>
        public void UpdateAsync()
        {
            /***
             * If the view is not auto-updating disable it completely
             * */
            if (!this.Live)
            {
                return;
            }


            /**
             * Preprocess the page again
             * */
            String r = this.Engine.Preprocess(this.TemplateCode, this.Parameter, false,this.uri);

            /**
             * Only refresh the layout elements and thus the render elements
             * if the new rendering results in changes and not an error token
             * */
            if (r != "NONCHANGE" && !r.StartsWith("ERROR:"))
            {
                try
                {

                    XmlDocument XD = new XmlDocument();
                    XD.LoadXml(r);



                    // Render new time
                    this.LayoutElements = XD;
                    this.Render(XD);
                }
                catch
                {

                }
            }

        }
        /// <summary>
        /// Method to configure scripts dom manipulation functions on layout element
        /// </summary>
        public void SetScriptFunctionality()
        {
            // Set the wrapper function so people can manipulate the layout element level
            this.ScriptEngine.SetFunction("$", new Func<string, object>(__getElementId));
            // Set the commit function so people can turn the modified layout element stadge into an object element stadge
            this.ScriptEngine.SetFunction("commit", new Func<object>(__commit_changes));
            // Set an function so script can change attribute of an element
            this.ScriptEngine.SetFunction("setAttribute", new Func<object, string, string, object>(__setAttribute));

            // Set an function so script can change inner content of an element
            this.ScriptEngine.SetFunction("setContent", new Func<object, string, object>(__setContent));

            // Set an ajax like function
            this.ScriptEngine.SetFunction("downloadContent", new Func<string, string, object>(__downloadContentAsync));
        }

        /// <summary>
        /// The drawboard this view is attached to
        /// </summary>
        public DrawBoard ParentBoard { get; set; }

        /// <summary>
        /// Load an custom HTML page into the special section.
        /// 
        /// It have to be preparsed by the MakoEngine.
        /// </summary>
        /// <remarks>Objects must now call Initialize to make it work perform. The reason for change is to make it able to set event handlers for MakoCreation</remarks>
        /// <param name="data">The ready preprocessed data from Mako alt. common html data</param>
        public Spofity(DrawBoard parentBoard)
        {

            ParentBoard = parentBoard;




        }
        /// <summary>
        /// Holds the preprocessed template
        /// </summary>
        public String TemplateCode { get; set; }
        /// <summary>
        /// The view instances parameter
        /// </summary>
        public String Parameter { get; set; }

        /// <summary>
        /// Initializes the view
        /// </summary>
        /// <param name="data"></param>
        /// <param name="engine"></param>
        /// <param name="pretemplate">The mako syntaxed template. Used for recurring updates</param>
        public void Initialize(string parameter, string pretemplate, string data, MakoEngine engine)
        {

            {
                XmlDocument d = new XmlDocument();
                try
                {
                    this.Receivers = new List<ContentReceiver>();

                    /**
                     * 
                     * Associate instance data
                     * */
                    this.Parameter = parameter;

                    this.Engine = engine;
                    this.TemplateCode = pretemplate;
                    SetScriptFunctionality();
                    
                    d.LoadXml(data.Replace(";", ""));
                    // Render
                    Render(d);


                    LoadData();
                }
                catch(XmlException e)
                {
                    /** Get the lone those where affected */
                    String[] Lines = data.Split('\n');
                    String Cause = Lines[e.LineNumber-1].Replace("<","&lt;").Replace(">","&gt;");

                    // clear output
                    JavaScriptEngine RuntimeMachine = new JavaScriptEngine();
                    // Load error page
                    using (System.IO.StreamReader SR = new System.IO.StreamReader("views\\error.xml"))
                    {
                        MakoEngine ME = new MakoEngine();
                        string errorView = ME.Preprocess(SR.ReadToEnd(), "", false,this.uri, true);
                        RuntimeMachine = new JavaScriptEngine();
                        RuntimeMachine.SetFunction("__printx", new Func<String, object>(ME.__printx));
                        RuntimeMachine.SetVariable("error", Cause + "\n" + e.ToString() + "\n ");

                        RuntimeMachine.Run((errorView));
                        d.LoadXml(ME.Output);
                        
                        // Render
                        Render(d);


                        LoadData();
                    }
                    // TODO AAA
                }
            }

        }

        bool _Section_PlaybackItemChanged(Spofity sender, Element element, string uri)
        {
            if (this.PlaybackStarted != null)
                return this.PlaybackStarted(sender, element, uri);
            return false;
        }

        void Spofity_FinishedLoading()
        {


        }
    }

    public class UL
    {
        public UL()
        {
            lis = new List<LI>();
        }
        [XmlElement("li")]
        private List<LI> lis;
        public List<LI> Lis
        {
            get
            {
                return lis;
            }
            set
            {
                lis = value;
            }
        }
        public class LI
        {
            public LI()
            {
                lis = new List<LI>();
            }
            [XmlElement("li")]
            private List<LI> lis;
            public List<LI> Lis
            {
                get
                {
                    return lis;
                }
                set
                {
                    lis = value;
                }
            }
        }
    }

    [XmlRoot("html")]
    public class HTML : View
    {


        private List<Section> sections;
        [XmlElement("p")]
        public List<Section> Sections
        {
            get
            {
                return sections;
            }
            set
            {
                sections = value;
            }
        }
    }
    [XmlRoot("view")]
    [Serializable]
    public class View
    {

        /// <summary>
        /// Parent Spofity
        /// </summary>
      
        public View()
        {
 
            sections = new List<Section>();
            Sets = new List<Set>();
            Toolbar = new Toolbar();
        }
        public String URI
        {
            get;
            set;
        }
        public String Querystring
        { get; set; }
        public Toolbar Toolbar { get; set; }
        private List<Section> sections;
        [XmlElement("section")]
        public List<Section> Sections
        {
            get
            {
                return sections;
            }
            set
            {
                sections = value;
            }
        }
        [XmlElement("set")]
        public List<Set> Sets { get; set; }
        /*    private List<UL> uls;
            public List<UL> Uls
            {
                get
                {
                    return uls;
                }
                set
                {
                    uls = value;
                }
            }*/
        /* [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("url")]
        public string Url;*/

    }
    public class Toolbar
    {
        public List<Element> Items { get; set; }
        public Toolbar()
        {
            Items = new List<Element>();
        }
    }
    /// <summary>
    /// Element class.
    /// </summary>
    public class Section
    {

        /// <summary>
        /// Resets the ptop and reorder the layout
        /// </summary>
        public void RenderLayout()
        {
            ptop = 20;
            foreach (Element elm in elements)
            {
                elm.AssertBounds(false);
            }
        }
        /// <summary>
        /// Buffer for sorted elements
        /// </summary>
        public List<Element> SortedBuffer { get; set; }

        /// <summary>
        /// Mode of sorting
        /// </summary>
        public enum SortMode { Default, Ascending, Descending };

        /// <summary>
        /// Sort elements
        /// </summary>
        /// <param name="column"></param>
        /// <param name="mode"></param>
        public void Sort(string column, SortMode mode)
        {
            switch (mode)
            {
                case SortMode.Default:
                    // Return to common elements mode
                    this.elements = null;
                    break;
                case SortMode.Ascending:
                    /**
                     * Preserve the default mode in the buffer
                     * */


                    /**
                     * Sort
                     * */

                    // Save this elements in the sorted buffer
                    this.elements = new List<Element>();
                    this.elements.AddRange(this.rawList);

                    for (int i = 0; i < this.elements.Count; i++)
                    {
                        for (int j = 0; j < this.elements.Count; j++)
                        {
                            if (Sorter != null)
                            {
                                Element src = elements[i];
                                Element target = elements[j];
                                switch (Sorter.CompareElement(target, src, column))
                                {
                                    case DrawBoard.CompareResult.None:
                                        continue;
                                    case DrawBoard.CompareResult.After:
                                        // Move the element where it should be
                                        elements.Remove(src);

                                        elements.Insert(j+1, src);
                                        src.SetAttribute("top", target.GetAttribute("top"));

                                        break;
                                    default:
                                        continue;
                                }
                            }

                        }
                    }
                    this.RenderLayout();



                    break;
                case SortMode.Descending:

                    break;
            }
        }
        /// <summary>
        /// This class  defines an sorter
        /// </summary>
        public Board.DrawBoard.IListSorter Sorter { get; set; }


        /// <summary>
        /// Convert the physical index to real index. Moved from Spofity.CS
        /// </summary>
        /// <param name="index">the real index</param>
        /// <returns>The virtual entry index, -1 if failed or the index points to an item of not an entry</returns>
        public int RealIndexToEntryIndex(int index)
        {
            try
            {
                // Get the element from the entries
                Element entry = Entries[index];
                if (entry.Type != "entry")
                    return -1;
                int virtualIndex = Entries.IndexOf(entry);
                return virtualIndex;
            }
            catch
            {
                return -1;
            }
        }

        /// <date>2011-04-24 16:18</date>
        /// <summary>
        /// Insert item at position which is synchronised with the range of items only by entries
        /// </summary>
        /// <param name="elements"></param>
        public void InsertEntryAt(List<Element> elements, int pos)
        {
            // define starting index
            int index = 0;
            foreach (Element ct in elements)
            {
                // only enumerate if the element is an type of entry
                if (ct.Entry)
                {
                    // if index is as the index, insert the item
                    if (index == pos)
                    {
                        // Get physical index of the item
                        int realIndex = this.Elements.IndexOf(ct);
                        // insert the collection here
                        this.elements.InsertRange(realIndex, elements);
                        // break and return
                        return;
                    }
                    index++;
                }
            }
        }
        /// <date>2011-04-24 16:18</date>
        /// <summary>
        /// Insert item at position which is synchronised with the range of items only by entries
        /// </summary>
        /// <param name="elements"></param>
        public void InsertEntryAt(Element elm, int pos)
        {
            List<Element> elements = new List<Element>() { elm };
            InsertEntryAt(elements, pos);
        }
        public List<Element> Entries
        {
            get
            {
                // <date>2011-04-24 16:24</date>

                // Allocate list for only entries
                List<Element> entries = new List<Element>();
                foreach (Element cf in this.Elements)
                {
                    if (cf.Entry)
                        entries.Add(cf);
                }
                return entries;
            }
        }

     //   public WebKit.WebKitBrowser Layout { get; set; }
        private int flowHeight = 120;
        public int FlowHeight
        {
            get
            {
                return flowHeight;
            }
            set
            {
                flowHeight = value;
            }
        }
        private bool header = true;
        public bool Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
            }
        }
        private bool alternating = true;
        public bool Alternating
        {
            get
            {
                return alternating;
            }
            set { alternating = value; }
        }
        /// <summary>
        /// Gets the difference between the total height of 
        /// the elements and the height of the visible boundary
        /// </summary>

        public int ItemOffset
        {
            get
            {
                try
                {
                    return TotalHeight - this.Parent.ParentBoard.Height;
                }
                catch
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// This function calculates the scrol offset of items. Returns -1 if there is an problem
        /// </summary>
        public int TotalHeight
        {
            get
            {
                try
                {
                    // The height of the visible board 
                    int viewHeight = this.Parent.ParentBoard.Bounds.Height;

                    // The integer which will add all element heights
                    int elementTotalHeight = 0;

                    // The outside height
                    int outsideHeight = 0;
                    /**
                     * Position of last object
                     * */
                    int lastPosition = 0;
                    // calculate the total height of all items
                    foreach (Element c in this.Elements)
                    {
                        if (c.Parent != null && c.GetAttribute("noelm") == "true")
                            continue;

                        // Check if this position is higher than any previous one and add it if so
                        int newpos = (c.Top + c.Height) - lastPosition ;
                        
                        // Add the item's top if the item's top is not equal to -1 (@TOP)
                        lastPosition = c.Top + c.Height;

                        elementTotalHeight += newpos;

                    }
                    // If the total elements filling is higher than the view's visible space.
                    if (elementTotalHeight < viewHeight)
                    {
                        elementTotalHeight = viewHeight;
                    }
                    // Return the offset
                    return elementTotalHeight;

                }
                catch
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// Updates the scrollbar
        /// </summary>
        public void AssertScroll()
        {
            Scrollbar ScrollBarY = this.Parent.ParentBoard.ScrollBarY;
            if (this.Parent.ParentBoard.ScrollBarY != null)
            {
                if (this.TotalHeight - this.Parent.ParentBoard.Height == 0)
                {
                    ScrollBarY.Position = 0;
                    ScrollBarY.ThumbHeight = 0;
                    ScrollBarY.Hide();
                    return;
                }
                else
                {
                    ScrollBarY.Show();
                    ScrollBarY.Position = ((float)this.ScrollY / ((float)this.TotalHeight - (float)this.Parent.ParentBoard.Height));
                    ScrollBarY.ThumbHeight = ((float)this.Parent.ParentBoard.Height / (float)this.TotalHeight);
                }
            }
          
        }
        /// <summary>
        /// ScrollY
        /// </summary>
        public int ScrollY { get; set; }
        public int ScrollX { get; set; }

        /// <summary>
        /// Defines an horizontal stream of elements.
        /// </summary>
        public bool Flow { get; set; }




        /// <summary>
        /// Columnheeaders for use in list mode
        /// </summary>
        public Dictionary<String, int> ColumnHeaders { get; set; }
        public int ptop = 20;
        /// <summary>
        /// Rebuilds the collection
        /// </summary>
        public void RebuildList()
        {
            // reset ptop
            ptop = 20;
            foreach (Element ct in this.rawList)
            {
                
         
                ct.AssertBounds(true,true);
            }
        }
        /// <summary>
        /// Gets or sets whether the entries in the view can be reordered.
        /// </summary>
        public bool Reorder { get; set; }

        private bool locked = true;

        /// <summary>
        /// Gets or sets if the view are locked for edit.
        /// </summary>
        [DefaultValue(true)]
        public bool Locked
        {
            get
            {
                return locked;
            }
            set { locked = value; }
        }
        /// <summary>
        /// In order to be able to use filter, an instance of an inherited class will assert the filtered query
        /// </summary>
        public interface IViewFilter
        {
            /// <summary>
            /// Must be implemented to filter
            /// </summary>
            /// <param name="src"></param>
            /// <returns>Wheather the element is visible or not</returns>
            bool FilterElement(Element src, string query);
        }

        /// <summary>
        /// An temporary view if filtered.
        /// </summary>
        public List<Element> FilterView
        {
            get;
            set;
        }
        /// <summary>
        /// An instance of the abstract class ViewFilter
        /// </summary>
        public IViewFilter Filter { get; set; }
        /// <summary>
        /// Generates an filter view according to the query
        /// </summary>
        /// <param name="query"></param>
        public void GenerateFilterView(string query)
        {

            if (query == "" || query == null)
            {
                FilterView = null;
                return;
            }
            FilterView = new List<Element>();

            /**
             * If no instance of the filter class is set, return
             * */
            if (Filter == null)
                return;

            // Reset ptop
            ptop = 20;

            // build view according to filter
            foreach (Element ct in this.elements)
            {
                if (Filter.FilterElement(ct, query))
                {
                    Element copy = ct.Copy();
                    FilterView.Add(copy);
                }
            }

        }
        private string filterQuery;

        /// <summary>
        /// Gets and sets the filtering query. 
        /// </summary>
        public String FilterQuery
        {
            get
            {
                return filterQuery;

            }
            set
            {
                filterQuery = value;
                GenerateFilterView(value);
            }

        }

        /// <summary>
        /// Occurs when playback has been interrupted.
        /// </summary>
        public event Spofity.ElementPlaybackStarted PlaybackItemChanged;
        /// <summary>
        /// The parent Spofity hosting the view
        /// </summary>
        public Spofity Parent { get; set; }
        /// <summary>
        /// Gets the current playing entry
        /// </summary>
        public Element NowPlaying
        {
            get
            {
                foreach (Element d in this.Elements)
                {
                    if (d.Entry && d.GetAttribute("__playing") != "")
                        return d;
                }
                return null;
            }
        }

        /// <summary>
        /// The index of the current item
        /// </summary>
        public int PlayIndex
        {
            get
            {
                // counter
                int i = 0;

                // only increase counter if element is an entry
                foreach (Element d in this.Elements)
                {
                    if (d.Entry)
                    {
                        if (d.GetAttribute("__playing") != "")
                            return i;
                        i++;
                    }

                }
                return i;
            }
            set
            {

                foreach (Element d in this.Elements)
                {
                    if (d.Entry)
                    {
                        if (d.GetAttribute("__playing") != "")
                        {
                            d.SetAttribute("__playing", "");
                        }

                    }

                }
                int counter = 0;
                foreach (Element elm in Elements)
                {
                    if (elm.Entry)
                    {
                        if (counter == value)
                        {
                            elm.SetAttribute("__playing", "true");
                            if (this.PlaybackItemChanged != null)
                                this.PlaybackItemChanged(this.Parent, elm, elm.GetAttribute("uri"));
                        }
                        counter++;
                    }

                }
            }
        }
        /// <summary>
        /// Gets and sets the selected index . Returns -1 if no entries was found. Applies only with elements of type "entry".
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                // index counter
                int i = 0;
                bool foundSelected = false;
                foreach (Element d in this.Elements)
                {
                    if (d.Type != "entry")
                        continue;
                    // if the previous gave foundSelected set this to selected


                    if (d.Selected == true)
                        return i;

                    i++;
                }
                return -1;
            }
            set
            {

                // Deactivate the selected items
                foreach (Element d in this.Elements)
                {

                    if (d.Type != "entry")
                        continue;
                    d.Selected = false;


                }
                // Set the item at the index as selected
                int index = 0;
                foreach (Element d in this.Elements)
                {
                    if (d.Type != "entry")
                        continue;
                    // if index meets the setter, mark it as selected
                    if (index == value)
                        d.Selected = true;

                    index++;



                }
            }

        }
        /// <summary>
        /// Returns an entry at the specified index.
        /// </summary>
        /// <remarks>Only elements of type "entry" is indexed</remarks>
        /// <param name="index">The position of the element to find</param>
        /// <returns>An element if found at the location, NULL otherwise</returns>
        public Element EntryAt(int index)
        {
            int i = 0;
            foreach (Element d in this.elements)
            {



                if (d.Entry)
                {
                    if (index == i)
                        return d;
                    i++;
                }

            }
            return null;
        }

        /// <summary>
        /// Gets and sets if the Element is in an list mode. A list mode will draw column headers straight before the
        /// first entry element and associate it with screen top if the element are outside the screen boundary.
        /// </summary>
        public bool List { get; set; }
        /// <summary>
        /// Definite count of items. Obsolute
        /// </summary>
        public int CountItems { get; set; }

        /// <summary>
        /// Short way to add element
        /// </summary>
        /// <param name="d"></param>
        /// <param name="X"></param>
        public void AddElement(Element d, Spofity X)
        {
            this.rawList.Add(d);
            CountItems++;
            Board.Spofity.SecElement(ref d, X);
            this.Parent.ParentBoard.RaiseElementAddEvent(d);

            /*if(d.GetAttribute("position")!="absolute")
            {
                switch(d.GetAttribute("type"))
                {
                    case "sp:entry":
                        d.Height=32;
                        d.Top = X.TopPos;
                        X.TopPos+=32;
		    				
                        break;
                    case "sp:divider":
                        d.Height = 48;
                        d.Top = X.TopPos+2;
                        X.TopPos+=50;
                        break;
                    case "sp:header":
                        d.Height = 48;
                        d.Top = X.TopPos;
                        X.TopPos+=32;
                        break;
                    case "sp:section":
                        d.Height = 48;
                        d.Top = X.TopPos;
                        X.TopPos+=32;
                        break;
                }
            }*/

        }
        public Section(Spofity parent)
        {

            Parent = parent;

            // Add standard columns (title,position)
            Sets = new List<Set>();
            /**
             * Set default column headers (title,artist,album,engine etc.)
             * */
            ColumnHeaders = new Dictionary<string, int>();
            ColumnHeaders.Add("r", 30);
            ColumnHeaders.Add("No", 50);
            ColumnHeaders.Add("Title", 300);
            ColumnHeaders.Add("Artist", 150);
            ColumnHeaders.Add("Length", 50);
            ColumnHeaders.Add("Album", 200);
            ColumnHeaders.Add("Media Provider", 200);
            ColumnHeaders.Add("User", 200);
            ColumnHeaders = new Dictionary<string, int>();
            Sorter = new Board.DrawBoard.EntrySorter();
            rawList = new List<Element>();
        }
        private string name;
        [XmlElement("set")]
        public List<Set> Sets { get; set; }
        [XmlAttribute("name")]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        /// <summary>
        /// This is the unmodified list of elements in an section of an view.
        /// </summary>
        [XmlElement("element")]
        public List<Element> rawList;

        // This is an extra list of elements which are used for customized views, like sorting and filtering
        private List<Element> elements;

        /// <summary>
        /// The visible elements in an view
        /// </summary>
        public List<Element> Elements
        {
            get
            {
                
                    return rawList;
            }

        }
        /// <summary>
        /// Height of columnheader
        /// </summary>

        public int HeaderHeight
        {
            get
            {
                return Parent.ParentBoard.columnheader_height;
            }
        }
    }
    public class Attribute
    {
        [XmlAttribute("name")]
        public string name;
        [XmlAttribute("value")]
        public string value;
    }

    public class Set
    {
        [XmlElement("entry")]
        public List<Entry> Entries { get; set; }
        public Set()
        {
            Entries = new List<Entry>();
        }
        [XmlAttribute("image")]
        public string Image { get; set; }
    }

    public class Entry
    {
        [XmlAttribute("href")]
        public string Href { get; set; }
        [XmlAttribute("title")]
        public string Title { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        [XmlAttribute("artist")]
        public string Artist { get; set; }
    }

    /// <summary>
    /// An element represents the object drawn on the Board class in an particular view
    /// </summary>
    public class Element
    {
        /// <summary>
        /// Gets or sets the unique identifier of the object
        /// </summary>
        public string Identifier { get; set; }
        public Element PreviousElement { get; set; }
        public Element NextElement { get; set; }
        public Dictionary<string, object> Styles;

        private void ParseStyle(string str)
        {

        }

        /// <summary>
        /// Set an inline style for the element. WORK in progress
        /// </summary>
        /// <param name="Style"></param>
        public void SetStyle(Dictionary<string, string> Style)
        {
            this.Styles = new Dictionary<string, object>();
            foreach (KeyValuePair<String, string> str in Style)
            {
            }
            // TODO: FIX STYLES HERE

        }
        /// <summary>
        /// Store an boxed instance of any class in the element
        /// </summary>
        public object Tag { get; set; }
        /// <summary>
        /// Returns if the element has an pending image
        /// </summary>
        public bool ImagePending
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets if the image has ben requested.
        /// </summary>
        public bool ImageRequested
        {
            get;
            set;
        }
        /// <summary>
        /// This elemeent can be selected and represents an entry
        /// </summary>
        public bool Entry
        {
            get
            {
                return this.GetAttribute("entry") == "true" ||
                    this.GetAttribute("type") == "entry" || this.Type == "entry";
            }
        }
        /// <summary>
        /// A object that can be attached to the element
        /// </summary>
        public object Attachment { get; set; }

        /// <summary>
        ///  Inner XML
        /// </summary>
        public String InnerXML { get; set; }
        /// <summary>
        /// Font for element's textContent
        /// </summary>
        public Font Font { get; set; }
        /// <summary>
        /// Gets the instance of the elements which it wa copied from or Null if it wasn't an copy. Used in the filter   system
        /// </summary>
        public Element Original { get; set; }

        /// <summary>
        /// Bitmap contents of the element
        /// </summary>
        public Bitmap Bitmap { get; set; }
        
        /// <summary>
        /// Asserts font of the object by their attributes
        /// </summary>
        public void AssertFont()
        {
            // Create new font if the font of the instance is NULL
            if (this.Font == null)
                this.Font = new Font("Tahoma", 8.5f, FontStyle.Regular);
            // Set up font size
            float fontSize = 8;
            float.TryParse(this.GetAttribute("size"), out fontSize);
            if (fontSize <= 0)
                fontSize = 8;

            // Assert element font attributes
            if (this.GetAttribute("bold") != "")
                this.Font = new System.Drawing.Font(this.Font, FontStyle.Bold);
            if (this.GetAttribute("size") != "")
                this.Font = new System.Drawing.Font(this.Font.FontFamily, fontSize, this.Font.Style);
            if (this.GetAttribute("font") != "")
                this.Font = new System.Drawing.Font(this.Font.FontFamily, fontSize, this.Font.Style, GraphicsUnit.Pixel);

        }
        /// <summary>
        /// Gets whether this elements is an copy of another element or an original. Used in the filter system
        /// </summary>
        public bool IsCopy
        {
            get
            {
                return Original != null;
            }
        }

        /// <summary>
        /// Copies the instance to a new element
        /// </summary>
        /// <returns>An instance of an copy of this instance</returns>
        public Element Copy()
        {
            Element dc = new Element(this.ParentSection, this.ParentSection.Parent.ParentBoard);
            dc.type = this.type;
            dc.Parent = this.Parent;
            dc.Original = this;
            // Copy the attributes to the new instance
            foreach (Attribute at in this.attributes)
                dc.SetAttribute(at.name, at.value);
            // Copy the bounds
            dc.SetAttribute("left", this.OldLeft.ToString());
            dc.SetAttribute("top", this.OldTop.ToString());
            dc.SetAttribute("width", this.Width.ToString());
            dc.SetAttribute("height", this.Height.ToString());
            dc.AssertBounds(true);
            return dc;
        }

        /// <summary>
        /// Text position for embedding the element inside a text block
        /// </summary>
        public int TextPosition { get; set; }

        public int OldLeft { get; set; }
        public int OldTop { get; set; }

        /// <summary>
        /// Gets and sets whether the object has been called. Currently this property is used to call image download
        /// handler when it tries to draw at the first time but prevent it are done always.
        /// </summary>
        public bool FirstCall;
        public Element()
        {
          
        }
        /// <summary>
        /// Find the textual position of the element
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>An element if found, NULL otherwise</returns>
        public Element ElementAt(int pos)
        {
            foreach (Element child in Elements)
            {
                if (child.GetAttribute("t_pos") == pos.ToString())
                    return child;
            }
            return null;
        }
        /// <summary>
        /// Gets and set the persistency. If true, the ptop will not be 
        /// changed after appending but will use the last valueo of it.
        /// </summary>
        public bool Persistent { get; set; }

        public Element Parent { get; set; }

        /// <summary>
        /// Parsesewidth
        /// </summary>
        /// <param name="width">The width</param>
        /// <param name="padding">The padding</param>
        /// <returns></returns>
        private float ParseWidth(string _width,float padding)
        {
            if (_width == "")
                return 0;
            float awidth;

            int width = 0;
            if (_width.EndsWith("%"))
            {
                float c = float.Parse(_width.Replace("%", "")) / 100;
                awidth = (int)((float)c * (width - padding * 2));
            }
            else
            {
                
                    awidth = float.Parse(_width.Replace("px", ""));
               
                    
            }
            return awidth;
        }
        /// <summary>
        /// Returns how deep the object is residing in the context
        /// </summary>
        public int Depth
        {
            get
            {
                int i = 0;
                Element parent = this.Parent;
                while (parent != null)
                {
                    parent = this.Parent;
                    i++;
                }
                return i;
            }
        }
        /// <summary>
        /// Returns coordinates coordinates for the object bounds
        /// </summary>
        /// <param name="scrollX">scrollX coordinate on view's state</param>
        /// <param name="scrollY">scrollY coordinate on view's state</param>
        /// <param name="top">Reference to top integer to set an dynamtic top positon (if asserted @TOP (-1))</param>
        /// <param name="Bounds">The bounds the object is residing in</param>
        /// <param name="padding">Padding rules applied to the workspace object relying in</param>
        /// <returns>an boolean wheather the object is inside the visible screen bounds</returns>
        /// 
        public Rectangle GetCoordinates(int scrollX, int scrollY, Rectangle Bounds, int padding)
        {
            // TODO: Fix vbox integration inside vboxes with flex
            Element _Element = this;
            

            int left = _Element.Left - scrollX;
            int top = _Element.Top - scrollY;
            
            int width = _Element.Width > 0 ? _Element.Width : Bounds.Width - left;
            int height = _Element.Height > 0 ? _Element.Height : this.Height;


            /***
             * If element is type of V/Hbox set width to the parent's width
             *  */
            if (_Element.GetAttribute("type") == "vbox")
               Width = _Element.Parent != null ? _Element.Parent.Width : this.ParentHost.Width;
                
            if (_Element.GetAttribute("type") == "hbox")
                Width = _Element.Parent != null ? _Element.Parent.Width : this.ParentHost.Width;

           

            /**
             * If element is preceeding an h/vbox assign bounds thereof
             * */
            if (_Element.Parent != null )
            {
                // parentCoordinate´s
               // Rectangle coords = _Element.Parent.GetCoordinates(scrollX, scrollY, this.Bounds, padding);
                if (_Element.Parent.Type == "vbox")
                {
                    
                    int ctop = int.Parse(this.Parent.GetAttribute("_top"));       // top position of parent
                    int cheight = int.Parse(this.Parent.GetAttribute("_height")); // height of parent (currently host)
                    Element parent = null;
                    // find matching parents height
                    while (cheight < 0)
                    {
                        cheight = int.Parse((parent = _Element.Parent).GetAttribute("_height"));
                    }
                    /***
                     * Go through the collection and label
                     * all child elements values in the collection
                     * so we can calculate the dynamic height
                     * */
                    for (int i = 0; i < _Element.Parent.Elements.Count; i++)
                    {
                        Element m = _Element.Parent.Elements[i];
                        m.SetAttribute("s_height", ParseWidth(m.GetAttribute("height"), padding).ToString());


                        /**
                         * If m has an attribute flex=1 assert 
                         * the flex rate of the element
                         * */
                        if (m.GetAttribute("flex") == "1")
                        {
                            

                            float restcount = 0; // height of resting elements
                            float flex = 1;
                            // calculate height of all succeeding elements
                            for (int j = i + 1; j < _Element.Parent.Elements.Count; j++)
                            {
                                Element n = _Element.Parent.Elements[j];

                                // if there is an flex of the element, create a division
                                if (n.GetAttribute("flex") == "1")
                                {
                                    flex /= 2;
                                }
                                else
                                {
                                    restcount += ParseWidth(n.GetAttribute("height"), padding);
                                }
                            }
                            // add flex variant to restcount
                            restcount += restcount * flex;
                            // set restcount to the elements attribute
                            m.SetAttribute(m    .Identifier + "_v_restcount", restcount.ToString());

                        }
                    }

                    /**
                     * Now set bounds for all elements to get
                     * out the current element bounds
                     * */

                    float __toppos = int.Parse(this.Parent.GetAttribute("_top"));
                    width = int.Parse(this.Parent.GetAttribute("_width"));
                    left = int.Parse(this.Parent.GetAttribute("_left")) ;
                    if (_Element.Type == "vbox")
                    {
                        width = 0;
                        int.TryParse(this.GetAttribute("_width"),out width);
                        left = 0;
                        int.TryParse(this.GetAttribute("_left"),out height);
                    }
                    // Now calculate YOUR elements
                    foreach (Element elm in _Element.Parent.Elements)
                    {

                       
                        /**
                         * INVOKE height STEP
                         * If element's flex is set to -1,
                         * revoke the element's height
                         * */
                        if (elm.GetAttribute("flex") == "1")
                        {
                            // if difference between __toppos and cheight is smaller than 10% of t
                            // the actual bounds, reset the height to an default one.
                            if (Diff(__toppos, cheight) > 100)
                            {

                                int s_height = int.Parse(elm.GetAttribute("s_height"));
                                // Increase toppos
                                __toppos += cheight;
                                if (elm.Parent != null)
                                {
                                    float decrease = 0;
                                    float.TryParse(elm.GetAttribute(_Element.Parent.Identifier + "_v_restcount"), out decrease);
                                    __toppos -= decrease;
                                   
                                }
                            }
                        }
                        else // otherwise set height to relative or absolute
                        {

                            __toppos += int.Parse(elm.GetAttribute("s_height"));

                        }
                        // if element is the current one, assert it
                        if (elm == this)
                        {


                            height = int.Parse(elm.GetAttribute("s_height"));
                            top = (int)__toppos - height;
                        }


                    }


                }
                if (_Element.Parent.Type == "hbox")
                {
                    int cleft = int.Parse(this.Parent.GetAttribute("_left"));       // left position of parent
                    int cwidth = int.Parse(this.Parent.GetAttribute("_width")); // width of parent (currently host)

                    
                    /***
                     * Go through the collection and label
                     * all child elements values in the collection
                     * so we can calculate the dynamic width
                     * */
                    for (int i=0; i < _Element.Parent.Elements.Count;i++)
                    {
                        Element m = _Element.Parent.Elements[i];
                        m.SetAttribute("s_width",ParseWidth(m.GetAttribute("width"),padding).ToString());


                        /**
                         * If m has an attribute flex=1 assert 
                         * the flex rate of the element
                         * */
                        if (m.GetAttribute("flex") == "1")
                        {
                          

                            float restcount = 0; // width of resting elements
                            float flex = 1;
                            // calculate width of all succeeding elements
                            for (int j = i + 1; j < _Element.Parent.Elements.Count; j++)
                            {
                                Element n = _Element.Parent.Elements[j];
                                
                                // if there is an flex of the element, create a division
                                if (n.GetAttribute("flex") == "1")
                                {
                                    flex /= 2;
                                }
                                else
                                {
                                    restcount += ParseWidth(n.GetAttribute("width"), padding);
                                }
                            }
                            // add flex variant to restcount
                            restcount += restcount * flex;
                            // set restcount to the elements attribute
                            m.SetAttribute(_Element.Identifier + "_h_restcount", restcount.ToString());

                        }
                    }

                    /**
                     * Now set bounds for all elements to get
                     * out the current element bounds
                     * */

                    float __leftpos = int.Parse(this.Parent.GetAttribute("_left"));
                    height = int.Parse(this.Parent.GetAttribute("_height"));
                    top = int.Parse(this.Parent.GetAttribute("_top"));

                    // Now calculate YOUR elements
                    foreach (Element elm in _Element.Parent.Elements)
                    {

                        // if element is the current one, assert it
                        if (elm == this)
                        {
                            float s_width = int.Parse(elm.GetAttribute("s_width"));
                            float gwidth = 0;
                            float restwidth = 0;
                            if(elm.NextElement!=null)
                             float.TryParse(elm.NextElement.GetAttribute("s_width"), out restwidth) ;
                           
                            float.TryParse(elm.GetAttribute(elm.Identifier+"_h_restcount"), out gwidth);
                            width = (int)(s_width > 0 ? s_width -gwidth : cwidth - gwidth - restwidth);
                            left = (int)__leftpos;
                          //  if (left < 0)
                            //    throw new Exception("Left cannot be lesser than zero");
                        }
                        /**
                         * INVOKE WIDTH STEP
                         * If element's flex is set to -1,
                         * revoke the element's width
                         * */
                        if (elm.GetAttribute("flex") == "1")
                        {
                            // if difference between __leftpos and cwidth is smaller than 10% of t
                            // the actual bounds, reset the width to an default one.
                            if (Diff(__leftpos, cwidth) >0)
                            {

                                int s_width = int.Parse(elm.GetAttribute("s_width"));
                                // Increase leftpos
                                __leftpos += cwidth;
                                float decrease = 0;
                                float.TryParse(elm.GetAttribute(elm.Identifier + "_v_restcount"),out decrease);
                                if (elm.Parent != null)
                                    __leftpos -= decrease;

                            }
                            else
                            {
                                int s_width = 0;
                                __leftpos += s_width;
                            }
                            //if (__leftpos < 0)
                          //      throw new Exception("leftpos cannot be lesser than zero");
                        }
                        else // otherwise set width to relative or absolute
                        {

                            __leftpos += int.Parse(elm.GetAttribute("s_width")) ;
                     //       if (__leftpos < 0)
                       //         throw new Exception("leftpos cannot be lesser than zero");
                        }
                        


                    }


#if (obsolote)
                    for (int i = 0; i < _Element.Parent.Elements.Count; i++ )
                    {
                        Element cf = _Element.Parent.Elements[i];
                        float awidth = 0; // the width of the current element
                        /*
                         *  If we come to yourself, break and begin assigning
                         *  */

                        /**
                         * Compute width of the element
                         * */
                       

                        /** if flex property is set, increase the width
                             * */
                        if (cf.GetAttribute("flex") != "")
                        {
                            int flex = 0;
                            int.TryParse(cf.GetAttribute("flex"), out flex);


                            //awidth = ( this.ParentHost.Width - (__leftpos+awidth)) - __leftpos;

                            int seqWidth = 0;
                          
                               

                            // distance between width and the end of the horizontal bounds
                            float distance = this.ParentHost.Width - (__leftpos + awidth);
                            // difference between the previous value and the current position
                            float iwidth = distance - (awidth + __leftpos);

                            // add the awidth to the distance
                            awidth += iwidth - __leftpos;
                        }

                        if (cf == this)
                        {
                            left = __leftpos;

                            width = (int)awidth;

                        }

                        __leftpos += (int)awidth;



                    }

#endif

                }

            }
           /* if (left < 0 || width < 0 || height < 0 || top < 0)
                throw new Exception("Left cannot be below zero");
            */
            this.SetAttribute("_left", left.ToString());
            this.SetAttribute("_top", top.ToString());
            this.SetAttribute("_width", width.ToString());
            this.SetAttribute("_height", height.ToString());
            Rectangle Rect = new Rectangle(left+padding - scrollX, top+padding , width-padding*2, height-padding*2);
            return Rect;
        }
        /// <summary>
        /// Returns if the comparator is equal to any of the elements of same type in iTems. T and items
        /// is in the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparator"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool AnyOf<T>(T comparator,params T[] items)
        {
            foreach (T item in items)
            {
                if (comparator.Equals(item))
                    return true;
            }
            return false;

        }
        /// <summary>
        /// Gets the difference between two integers
        /// </summary>
        /// <param name="p"></param>
        /// <param name="__leftpos"></param>
        /// <returns></returns>
        private float Diff(float p, float __leftpos)
        {
            return p > __leftpos ? p - __leftpos : __leftpos - p;
        }
        /// <summary>
        /// Function to measure wheather the element is inside the screen bounds, eg. visible
        /// </summary>
        /// <param name="scrollX">the scrollX position</param>
        /// <param name="scrollY">the scrollY position</param>    
        /// <param name="Bounds">The bounds the object is residing in</param>
        /// <param name="padding">Padding rules applied to the workspace object relying in</param>
        /// <returns></returns>
        public bool InsideScreen(int scrollX, int scrollY, Rectangle srcBounds, int padding)
        {
            Rectangle Bounds = GetCoordinates(scrollX, scrollY, srcBounds, padding);
            int top = Bounds.Top;
            int height = Bounds.Height;
            int Left = Bounds.Left;
            int Top = Bounds.Top;
            if (!(top >= 0 && top + height <= this.Height /*&& left >= scrollX && left+width <= scrollX*/))
                return true;
            return false;
        }
        /// <summary>
        /// Stylesheet applied to the element. Not yet in use, only declared
        /// </summary>
        public Dictionary<String, String> stylesheet;
        /// <summary>
        /// ptop is to auto-align elements which has no valid top location - eg. smaller 
        /// than zero and should be applied in this way. Default is 20 so we ge an margin at the top
        /// </summary>
        public int ptop
        {
            get
            {
                return this.ParentSection.ptop;
            }
            set
            {
                this.ParentSection.ptop = value;
            }
        }

        /// <summary>
        /// Assign bounds of the object according to the parameters that is set in the textual parameter list (attributes)
        /// </summary>
        public void AssertBounds(bool copy,bool reordering=false)
        {
#if(obsolote)
            // if this is the first entry in an list view, push down it the amount of column headers
            if (this.ParentSection.Entries.Count > 0 && !reordering)
            {
                if (this.type == "entry" && this.ParentSection.Entries[0] == this)
                {
                    
                    ptop += this.ParentHost.columnheader_height;
                }
            }
#endif
            /**
        * If the current section is an flow, do 
        * not show the entry
        * */
            if (this.ParentSection.Flow && this.Entry)
                return;
            if (this.GetAttribute("noelm") != "")
                return;

            int top = 0;
            int left, width, height;

            // Try get integers from the attributes
            int.TryParse(GetAttribute("top"), out top);
            int.TryParse(GetAttribute("left"), out left);
            int.TryParse(GetAttribute("width"), out width);
            int.TryParse(GetAttribute("height"), out height);

           
            // If height is smaller than one measure the height by the text content
            if (height < 1)
            {
                height = 30;

            }
            if (!copy)
            {
                OldLeft = left;
                OldTop = top;
            }
            // Get if the element is persisten
            bool persistent = false;
            bool.TryParse(GetAttribute("persistent"), out persistent);
            Persistent = persistent;



            // If the top variable is still below one, assign the top to the ptop variable and increase ptop iself, but only
            // for own view controls. If the ptop is below the height of the header drawable (the webkit zone), add the corresponding height to the ptop
            if (top < 1)
            
            {
#if(obsolote)
                if (ptop < this.ParentHost.browser.Height && this.ParentHost.browser.Height < this.ParentHost.Height && !reordering) 
                {
                    ptop = this.ParentHost.browser.Height;
                }
#endif
                top = ptop;
                if (!Persistent)
                {

                    if (Parent!=null&&Parent.GetAttribute("type").Contains("box") )
                    {
                    
                    }
                    else
                    {
                        
                        ptop += height;
                    }
                    if (GetAttribute("type").Contains("box"))
                    {
                        ptop += 5;
                    }
                }
                else
                {


                }

            }
            // Apply the values to the native width/height markup
            this.Width = width;
            this.Left = left;
            this.Height = height;
            this.Top = top;
        }

        /// <summary>
        /// Gets the object's bounds in absolute rectangle
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(Left, Top, Width, Height);
            }
        }

        /// <summary>
        /// Gets and set the top of the element
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Gets and set the left position of the element
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets and sets the width of the element. Width below one is considered as filling width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets and sets the height of the element. Values below one is considered as filling vertically.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets and sets the raw data of the object. For example an text node will has its content stored in this
        /// property.
        /// </summary>
        [XmlElement("data")]
        public string Data
        {
            get;
            set;
        }

        /// <summary>
        /// The drawboard the element is drawn on
        /// </summary>
        public DrawBoard ParentHost { get; set; }

        /// <summary>
        /// Function to set an attribute of the node.
        /// </summary>
        /// <param name="name">the name of the attribute</param>
        /// <param name="value">the value of the attribute</param>
        public void SetAttribute(String name, string value)
        {

            /** boolean indicating wheather an matching attribute was found, 
                * if not add an to the collection
                * */
            bool found = false;
            foreach (Attribute d in attributes)
            {
                if (d.name == name)
                {
                    d.value = value;
                    found = true;
                }
            }

            // If the attribute is an type set the type property to the value
            if (name == type)
            {
                this.type = value;
            }

            // If the attribute was not found create an new attribute
            if (!found)
            {
                Attribute _attr = new Attribute();
                _attr.name = name;
                _attr.value = value;
                this.attributes.Add(_attr);
            }

            // If the element is an copy, give it's original the value
            if (this.IsCopy)
                this.Original.SetAttribute(name, value);



        }
        public Section ParentSection { get; set; }
        public Element(Section parentSection, DrawBoard parentBoard)
        {
            this.ParentSection = parentSection;
            this.ParentHost = parentBoard;
            attributes = new List<Attribute>();
            Elements = new ElementCollection();
            this.Identifier = this.GetHashCode().ToString();
        }

        private bool selected;
        /// <summary>
        /// Gets and sets whether the object is selected on the graphical board
        /// </summary>
        public bool Selected
        {
            get
            {
                
                return this.GetAttribute("__selected") == "true";
            }
            set
            {
                selected = value;
                this.SetAttribute("__selected", value ? "true" : "");
                // If this is an copy set the parent to selected to
                if (IsCopy)
                    this.Original.selected = true;




            }
        }
        /// <summary>
        /// Scroll so the selected will be visible
        /// </summary>
        public void AssertSelection()
        {
            // Get the coordinates for the item
            Rectangle objCoordinates = this.GetCoordinates(this.ParentSection.Parent.ParentBoard.scrollX, this.ParentSection.Parent.ParentBoard.scrollY, new Rectangle(0, 0, this.ParentSection.Parent.ParentBoard.Width, this.ParentSection.Parent.ParentBoard.Height), 0);

            // if the item reach the end 
            if (objCoordinates.Top + objCoordinates.Height >= this.ParentSection.Parent.ParentBoard.Bounds.Height)
            {

                this.ParentHost.scrollY += this.Height;
            }

            // if the object is bellow the beginning
            if (objCoordinates.Top < objCoordinates.Height)
                this.ParentHost.scrollY -= this.Height;

        }
        private string type;

        /// <summary>
        /// Gets and sets the type of element. 
        /// </summary>
        [XmlAttribute("type")]
        public string Type
        {
            get
            {
                if (GetAttribute("type") != "")
                    return GetAttribute("type");
                return type;
            }
            set
            {
                type = value;
                SetAttribute("type", type);
            }
        }
        /// <summary>
        /// An list of arbitrary attributes applied to the object. These attributes provides the properties coming from
        /// the underlying input and are managed by the application. 
        /// </summary>
        private List<Attribute> attributes;
        [XmlElement("attribute")]
        public List<Attribute> Attributes
        {
            get
            {
                return attributes;
            }
            set
            {
                attributes = value;
            }
        }
        /// <summary>
        /// Class for defining children element collection
        /// </summary>
        public class ElementCollection : IEnumerable
        {
            public int Count
            {
                get
                {
                    return elements.Count;
                }
            }
            public ElementCollection()
            {
                elements = new List<Element>();
            }
            int position = -1;
            IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return elements.AsEnumerable<Element>().ToArray().GetEnumerator();
            }

            public Element Parent { get; set; }
            private List<Element> elements;
            [XmlElement("element")]
            public Element this[int index]
            {
                get
                {
                    return elements[index];
                }
                set
                {
                    elements[index] = value;

                }
            }
            public void Add(Element elm)
            {
                elm.Parent = Parent;
                elements.Add(elm);
            }
        }

        /// <summary>
        /// Child elements of the element. All elements can contain own child nodes, but their behaviour and functionality
        /// depends of the target implementation.
        /// </summary>
        public ElementCollection Elements { get; set; }
        public string GetAttribute(string name)
        {
            if (name.Contains(" "))
            {
                StringBuilder Values = new StringBuilder();
                String[] names = name.Split(' ');
                foreach (String prop in names)
                {
                    Values.Append(this.GetAttribute(prop));
                }
                return Values.ToString();
            }

                    
            foreach (Attribute a in attributes)
            {
                if (a.name == name)
                {
                    return a.value;
                }
            }
            return "";
        }


    }
}
