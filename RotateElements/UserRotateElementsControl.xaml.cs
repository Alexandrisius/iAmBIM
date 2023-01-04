using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;

namespace RotateElements
{
    /// <summary>
    /// Логика взаимодействия для UserRotateElementsControl.xaml
    /// </summary>
    public partial class UserRotateElementsControl : Window
    {
        public string prevTextBox2 = "";
        public int prevIndexTextbox2 = 0;
        Document _doc;
        Element _elem1;
        Line _lineZ;
        ICollection<ElementId> _elemId;
        readonly Connector connector;
        TransactionGroup _transGroup;

        public UserRotateElementsControl(Document doc, Element elemForRotate, XYZ clickPoint, TransactionGroup tgGroup)
        {
            _doc = doc;
            _elem1 = elemForRotate;
            _lineZ = ConnectorCalculator.GetAxisByTwoElements(elemForRotate, clickPoint, out connector);
            _transGroup = tgGroup;

            InitializeComponent();
        }


        private void Button_Left(object sender, RoutedEventArgs e)
        {
            if (Check.IsChecked == true)
            {
                if (_elemId == null)
                {
                    _elemId = Iterator.GetElements(_doc, _elem1, connector);
                }
                Rotater.TurnLeft(_doc, _lineZ, Double.Parse(AngleText.Text), _elem1, _elemId, 1);
            }
            else
            {
                Rotater.TurnLeft(_doc, _lineZ, Double.Parse(AngleText.Text), _elem1, _elemId, 2);

            }
        }

        private void Button_Right(object sender, RoutedEventArgs e)
        {
            if (Check.IsChecked == true)
            {
                if (_elemId == null)
                {
                    _elemId = Iterator.GetElements(_doc, _elem1, connector);
                }
                Rotater.TurnRight(_doc, _lineZ, Double.Parse(AngleText.Text), _elem1, _elemId, 1);
            }
            else
            {
                Rotater.TurnRight(_doc, _lineZ, Double.Parse(AngleText.Text), _elem1, _elemId, 2);

            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AngleText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (TextBox)sender;
            // Здесь возможно попадется десятичная запятая
            var f = CultureInfo.CurrentUICulture.NumberFormat;

            // Жестко задаем десятичную точку
            f = CultureInfo.GetCultureInfo("en-US").NumberFormat;

            var str = tb.Text;
            var regex = new Regex($"^\\{f.NegativeSign}?\\d*(\\{f.CurrencyDecimalSeparator}\\d*)?$");
            if (regex.IsMatch(str))
            {
                prevTextBox2 = str;
                prevIndexTextbox2 = tb.CaretIndex;
            }
            else
            {
                var savedPrevIndex = prevIndexTextbox2;
                tb.Text = prevTextBox2;
                prevIndexTextbox2 = savedPrevIndex;
                tb.CaretIndex = savedPrevIndex;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_transGroup.GetStatus() == TransactionStatus.Started)
            {
                _transGroup.Assimilate();
            }
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_transGroup.GetStatus() == TransactionStatus.Started)
            {
                _transGroup.RollBack();
            }

        }
    }
}
