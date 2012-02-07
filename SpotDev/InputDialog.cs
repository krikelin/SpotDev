/*
* Copyright (C) 2012 Alexander Forselius
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SpotDev
{
    public partial class InputDialog : Form
    {
        public InputDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
        public String Value
        {
            get
            {
                return this.tbInput.Text;
            }
        }
       
        /// <summary>
        /// Creates a new input dialog
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="label"></param>
        public InputDialog(String title, String text, String label)
        {
            InitializeComponent();
            lText.Text = label;
            tbInput.Text = text;
            this.Text = text;
        }

        private void InputDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
