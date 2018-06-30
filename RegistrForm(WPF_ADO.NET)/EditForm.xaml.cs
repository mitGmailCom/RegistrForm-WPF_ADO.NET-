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
    /// Логика взаимодействия для EditForm.xaml
    /// </summary>
    public partial class EditForm : Window
    {
        SqlConnection connect;
        private static string stringConect;
        SqlDataReader rdr = null;
        MainWindow wind;

        public EditForm()
        {
            InitializeComponent();
            Loaded += EditForm_Loaded;
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

        private void EditForm_Loaded(object sender, RoutedEventArgs e)
        {
            this.Closing += EditForm_Closing;
            wind = this.Owner as MainWindow;
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
                    if (line == wind.ControlsNameContextMenu)
                    {
                        textBoxLoginEdit.Text = rdr[1].ToString();
                        PaswBowPassEdit.Password = rdr[2].ToString();
                        textBoxEmailEdit.Text = rdr[3].ToString();
                        textBoxAdressEdit.Text = rdr[4].ToString();
                        textBoxPhoneEdit.Text = rdr[5].ToString();
                        var tt = rdr[6].ToString();
                        if (rdr[6].ToString() == "True")
                            chBoxIsAdminEdit.IsChecked = true;
                        if (rdr[6].ToString() == "False")
                            chBoxIsAdminEdit.IsChecked = false;
                        break;
                    }
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
        }


        private void EditForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            string strQueryEdit = "Update Users set Login=@login, Password=@pass, Email=@email, Adress=@adress, Phone=@phone, Admin=@isAdmin where Id=@id";
            SqlCommand commandUpdate = new SqlCommand(strQueryEdit, connect);
            commandUpdate.Parameters.AddWithValue("@login", $"{textBoxLoginEdit.Text }");
            commandUpdate.Parameters.AddWithValue("@pass", $"{PaswBowPassEdit.Password}");
            commandUpdate.Parameters.AddWithValue("@email", $"{textBoxEmailEdit.Text}");
            commandUpdate.Parameters.AddWithValue("@adress", $"{textBoxAdressEdit.Text}");
            //commandUpdate.Parameters.AddWithValue("@phone", Convert.ToInt64(textBoxPhoneEdit.Text));
            long res = 0;
            bool temp = Int64.TryParse(textBoxPhoneEdit.Text, out res);
            if (temp)
                commandUpdate.Parameters.AddWithValue("@phone", res);
            commandUpdate.Parameters.AddWithValue("@isAdmin", $"{chBoxIsAdminEdit.IsChecked}");
            commandUpdate.Parameters.AddWithValue("@id", $"{wind.ControlsNameContextMenu + 1}");
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
            wind.LoginsList[wind.ControlsNameContextMenu] = textBoxLoginEdit.Text;
            MessageBox.Show("The Data is stored", "Save");
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
       
        
        private void textBoxLoginEdit_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            for (int i = 0; i < wind.LoginsList.Count; i++)
            {
                if (textBoxLoginEdit.Text != wind.LoginsList[i])
                {
                    ChangeEnabled(true, textBoxEmailEdit, PaswBowPassEdit, textBoxAdressEdit, textBoxPhoneEdit, chBoxIsAdminEdit);
                }

                if (textBoxLoginEdit.Text == wind.LoginsList[wind.ControlsNameContextMenu])
                {
                    ChangeEnabled(true, textBoxEmailEdit, PaswBowPassEdit, textBoxAdressEdit, textBoxPhoneEdit, chBoxIsAdminEdit);
                    return;
                }

                if (textBoxLoginEdit.Text == wind.LoginsList[i])
                {
                    ChangeEnabled(false, textBoxEmailEdit, PaswBowPassEdit, textBoxAdressEdit, textBoxPhoneEdit, chBoxIsAdminEdit);
                    MessageBox.Show("Login with this name is exists!", "Warning!");
                    return;
                }
            }
        }
       

        private void textBoxPhoneEdit_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (textBoxPhoneEdit.Text != null)
            {
                long res = 0;
                bool temp = Int64.TryParse(textBoxPhoneEdit.Text, out res);
                if (!temp)
                {
                    ChangeEnabled(false, textBoxEmailEdit, PaswBowPassEdit, textBoxAdressEdit, textBoxLoginEdit, chBoxIsAdminEdit);
                    MessageBox.Show("The Value must contain only numbers!", "Warning");
                    return;
                }
                if (temp)
                {
                    ChangeEnabled(true, textBoxEmailEdit, PaswBowPassEdit, textBoxAdressEdit, textBoxLoginEdit, chBoxIsAdminEdit);
                    return;
                }
            }
        }
    }

}
