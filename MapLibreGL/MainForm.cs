using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapLibreGL
{
    public partial class MainForm : Form
    {
        WebMap Map;
        public MainForm()
        {
            InitializeComponent();
            LoadMap();
        }

        private void LoadMap()
        {
            Map = new WebMap();
            Map.Dock = DockStyle.Fill;
            this.Controls.Add(Map);
        }
    }
}
