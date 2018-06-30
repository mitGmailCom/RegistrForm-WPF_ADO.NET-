using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace RegistrForm_WPF_ADO.NET_
{
    /// <summary>
    /// Логика взаимодействия для RegistrationForm.xaml
    /// </summary>
    public partial class RegistrationForm : Window
    {
        MainWindow wind = new MainWindow();
        SqlConnection connect;
        private static string stringConect;
        SqlDataReader rdr = null;

        public RegistrationForm()
        {
            InitializeComponent();
            this.Closing += RegistrationForm_Closing;
            Loaded += RegistrationForm_Loaded;
        }

        

        private void RegistrationForm_Loaded(object sender, RoutedEventArgs e)
        {
            connect = new SqlConnection();
            stringConect = ConfigurationManager.ConnectionStrings["connectUsers"].ConnectionString;
            connect.ConnectionString = stringConect;
            wind = this.Owner as MainWindow;
        }

        protected internal void Connect()
        {
            rdr = null;
            connect = new SqlConnection();
            stringConect = ConfigurationManager.ConnectionStrings["connectUsers"].ConnectionString;
            connect.ConnectionString = stringConect;
        }

        protected internal void BlockFinally()
        {
            if (rdr != null)
                rdr.Close();
            if (connect != null)
                connect.Close();
        }


        private void RegistrationForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Connect();
            wind.listBox.Items.Clear();
            Connect();
            string strQueryAllUsers = @"Select * from Users";
            SqlCommand commandAllUsers = new SqlCommand(strQueryAllUsers, connect);
            int line = 0;

            try
            {
                connect.Open();
                rdr = commandAllUsers.ExecuteReader();

                while (rdr.Read())
                {
                    string strr = rdr[1].ToString();
                    wind.listBox.Items.Add(strr);
                    line++;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
            finally
            {
                BlockFinally();
            }
            wind.Activate();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ChangeEnabled(bool flag, params object[] masContrs)
        {
            for (int i = 0; i < masContrs.Length; i++)
            {
                if (masContrs[i] is TextBox)
                    ((TextBox)masContrs[i]).IsEnabled = flag;
                if (masContrs[i] is PasswordBox)
                    ((PasswordBox)masContrs[i]).IsEnabled = flag;
                if (masContrs[i] is ComboBox)
                    ((ComboBox)masContrs[i]).IsEnabled = flag;
            }
        }

        private void TBoxLogin_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            for (int i = 0; i < wind.LoginsList.Count; i++)
            {
                if (textBoxLoginAdd.Text != wind.LoginsList[i])
                {
                    ChangeEnabled(true, textBoxEmailAdd, PaswBowPassAdd, textBoxAdressAdd, textBoxPhoneAdd, chBoxIsAdmin);
                }

                if (textBoxLoginAdd.Text == wind.LoginsList[wind.ControlsNameContextMenu])
                {
                    ChangeEnabled(true, textBoxEmailAdd, PaswBowPassAdd, textBoxAdressAdd, textBoxPhoneAdd, chBoxIsAdmin);
                    return;
                }

                if (textBoxLoginAdd.Text == wind.LoginsList[i])
                {
                    ChangeEnabled(false, textBoxEmailAdd, PaswBowPassAdd, textBoxAdressAdd, textBoxPhoneAdd, chBoxIsAdmin);
                    MessageBox.Show("Login with this name is exists!", "Warning!");
                    return;
                }
            }
        }

        private void TBoxPhone_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            long res = 0;
            bool temp = Int64.TryParse(textBoxPhoneAdd.Text, out res);
            if (!temp)
            {
                ChangeEnabled(false, textBoxEmailAdd, PaswBowPassAdd, textBoxAdressAdd, textBoxLoginAdd, chBoxIsAdmin);
                MessageBox.Show("The Value must contain only numbers!", "Warning");
                return;
            }
            if (temp)
            {
                ChangeEnabled(true, textBoxEmailAdd, PaswBowPassAdd, textBoxAdressAdd, textBoxLoginAdd, chBoxIsAdmin);
                return;
            }
        }


        private void PasswordBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (PaswBowPassAdd.Password != null)
            {
                if (PaswBowPassAdd.Password != ConfirmPasswordAdd.Password)
                {
                    PaswBowPassAdd.BorderBrush = Brushes.Red;
                    ConfirmPasswordAdd.BorderBrush = Brushes.Red;
                }
                if (PaswBowPassAdd.Password == ConfirmPasswordAdd.Password)
                {
                    PaswBowPassAdd.BorderBrush = null ;
                    ConfirmPasswordAdd.BorderBrush = null;
                }
            }
        }

        private void btnRegistr_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            string strQueryEdit = "Insert into Users(Login, Password, Email, Adress, Phone, Admin) values(@login, @pass, @email, @adress, @phone, @isAdmin)";
            SqlCommand commandUpdate = new SqlCommand(strQueryEdit, connect);
            commandUpdate.Parameters.AddWithValue("@login", $"{textBoxLoginAdd.Text }");
            commandUpdate.Parameters.AddWithValue("@pass", $"{PaswBowPassAdd.Password}");
            commandUpdate.Parameters.AddWithValue("@email", $"{textBoxEmailAdd.Text}");
            commandUpdate.Parameters.AddWithValue("@adress", $"{textBoxAdressAdd.Text}");
            //commandUpdate.Parameters.AddWithValue("@phone", Convert.ToInt64(textBoxPhoneEdit.Text));
            long res = 0;
            bool temp = Int64.TryParse(textBoxPhoneAdd.Text, out res);
            if (temp)
                commandUpdate.Parameters.AddWithValue("@phone", res);
            commandUpdate.Parameters.AddWithValue("@isAdmin", $"{chBoxIsAdmin.IsChecked}");
            try
            {
                connect.Open();
                commandUpdate.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
            finally
            {
                BlockFinally();
            }
            wind.LoginsList.Add(textBoxLoginAdd.Text);
            MessageBox.Show("The Data added", "Add Item");
            this.Close();
        }

       

        private void btnShowPass_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (btnShowPass.IsPressed)
            {
                int t = 0;
                PaswBowPassAdd.IsEnabled = true;
                PaswBowPassAdd.Visibility = Visibility.Visible;
                //if (btnShowPass.IsPressed)
                {

                    for (int i = 0; i < StPanelPass.Children.Count; i++)
                    {
                        if (StPanelPass.Children[i] is TextBlock)
                            t = i;
                    }
                    StPanelPass.Children.RemoveAt(t);
                }
            }
        }

        private void btnShowPass_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < StPanelPass.Children.Count; i++)
            {
                if (StPanelPass.Children[i] is TextBlock)
                {
                    TextBlock temp = new TextBlock();
                    temp = StPanelPass.Children[i] as TextBlock;
                    if (temp.Name == "tempTxBl")
                        return;
                }
            }
            TextBlock tempTxBl = new TextBlock();
            tempTxBl.Name = "tempTxBl";
            tempTxBl.Margin = new Thickness(0,10,0,0);
            tempTxBl.Padding = new Thickness(0,0,0,0);
            tempTxBl.Width = 117;
            tempTxBl.Height = PaswBowPassAdd.Height;
            tempTxBl.FontSize = 20;
            tempTxBl.FontWeight = FontWeights.Bold;
            tempTxBl.VerticalAlignment = VerticalAlignment.Bottom;
            tempTxBl.HorizontalAlignment = HorizontalAlignment.Left;
           
            int t = 0;
            if (btnShowPass.IsPressed)
            {
                for (int i = 0; i < StPanelPass.Children.Count; i++)
                {
                    if (StPanelPass.Children[i] is PasswordBox)
                        t = i;
                }
                tempTxBl.Text = PaswBowPassAdd.Password;
                StPanelPass.Children.Insert(t, tempTxBl);
                PaswBowPassAdd.IsEnabled = false;
                PaswBowPassAdd.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
