﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaxiTaxiWPF.TaxiControls
{
    using System.Diagnostics;

    /// <summary>
    /// Interaction logic for Abrechnung.xaml
    /// </summary>
    public partial class Abrechnung : UserControl
    {
        public Abrechnung()
        {
            InitializeComponent();
        }

        private void keyUpEvent(object sender, KeyEventArgs e)
        {
            Debugger.Break();
        }
    }
}
