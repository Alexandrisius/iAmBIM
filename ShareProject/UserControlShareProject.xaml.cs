﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ShareProject
{
    /// <summary>
    /// Логика взаимодействия для UserControlShareProject.xaml
    /// </summary>
    public partial class UserControlShareProject : Window
    {
        int value = 0;

        public UserControlShareProject()
        {
            InitializeComponent();
        }
        public void addValueProgressBar (int add)
        {
            while (value < add)
            {
                value++;
                ProgressBar.Value = value;
            }
            
        }
    }
}
