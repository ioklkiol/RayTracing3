﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Step2
{
    public partial class Form1 : Form
    {
        public static Form1 main;
        private Renderer renderer = new Renderer();

        public Form1()
        {
            InitializeComponent();
            main = this;
            renderer.Init();
            //this.WindowState = FormWindowState.Minimized;
            //this.ShowInTaskbar = false;
        }

      
    }

}

