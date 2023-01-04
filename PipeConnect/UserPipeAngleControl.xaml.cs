using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using PipeConnect;

namespace PipeConnect
{
    /// <summary>
    /// Логика взаимодействия для UserPipeAngleControl.xaml
    /// </summary>
    public partial class UserPipeAngleControl : Window
    {
        public string prevTextBox2 = "";
        public int prevIndexTextbox2 = 0;
        Document _doc;
        Connector _con1;//Перемещаемый элемент
        Connector _con2;//Статичный элемент
        TransactionGroup _transGroup;

        public UserPipeAngleControl(Document doc, Connector con1, Connector con2, TransactionGroup transGroup)
        {
            _doc = doc;
            _con1 = con1;
            _con2 = con2;
            _transGroup = transGroup;
            InitializeComponent();
        }

        private void Button_Left(object sender, RoutedEventArgs e)
        {
            TurnByClick.TurnLeft(_doc, _con1, Double.Parse(AngleText.Text));
        }

        private void Button_Right(object sender, RoutedEventArgs e)
        {
            TurnByClick.TurnRight(_doc, _con1, Double.Parse(AngleText.Text));
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

        private void SlopeText_TextChanged(object sender, TextChangedEventArgs e)
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

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ConnectElement.ConnectTo(_con1, _con2, _doc);

            if (_transGroup.GetStatus() == TransactionStatus.Started)
            {
                _transGroup.Assimilate();
            }
            this.Close();
        }

        private void Slope_Click(object sender, RoutedEventArgs e)
        {
            int typeBoxItem = Box.SelectedIndex;

            SlopePipeFiting.SlopePipe(_doc, _con1, typeBoxItem, Double.Parse(SlopeText.Text));
           
            this.Close();

        }

        private void Button_KeyDown_Right(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                Button_Right(Right, null);
            }
            if (e.Key == Key.Q)
            {
                Button_Left(Left, null);

            }
        }

        private void Button_KeyDown_Left(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                Button_Right(Right, null);
            }
            if (e.Key == Key.Q)
            {
                Button_Left(Left, null);

            }
        }

        List<Connector> freeCon = new List<Connector>();

        private void Button_Reflex(object sender, RoutedEventArgs e)
        {
            TurnByClick.TurnAroundAxis(_doc, _con1, _con2, out Connector used, freeCon);
            freeCon.Add(used);
            _con1 = used;
        }

        private void WPF_Closed(object sender, EventArgs e)
        {
            if (_transGroup.GetStatus() == TransactionStatus.Started)
            {
                _transGroup.RollBack();
            }
            
        }
    }
}
