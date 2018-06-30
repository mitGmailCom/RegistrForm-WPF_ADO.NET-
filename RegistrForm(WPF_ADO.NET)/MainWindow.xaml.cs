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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegistrForm_WPF_ADO.NET_
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected internal SqlConnection connect;
        protected internal static string stringConect;
        protected internal SqlDataReader rdr = null;
        protected internal List<string> LoginsList;
        protected internal int ControlsNameContextMenu;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
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

        private void ShowAllUsers()
        {
            listBox.Items.Clear();
            LoginsList = new List<string>();
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
                    listBox.Items.Add(strr);
                    LoginsList.Add(rdr[1].ToString());
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAllUsers();
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            RegistrationForm registration = new RegistrationForm();
            registration.Owner = this;
            registration.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chBoxFiltrForAdmin_Checked(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            Connect();
            string strQueryAllAdmins = @"Select * from Users where Admin = @p3";
            SqlCommand commandAllAdmins = new SqlCommand(strQueryAllAdmins, connect);
            int t = 1;
            commandAllAdmins.Parameters.AddWithValue("@p3", t);

            try
            {
                connect.Open();
                rdr = commandAllAdmins.ExecuteReader();
                while(rdr.Read())
                {
                    listBox.Items.Add(rdr[1]);
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

        private void chBoxFiltrForAdmin_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowAllUsers();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender;
            var ee = e; 
            EditForm editForm = new EditForm();
            editForm.Owner = this;
            editForm.Show();
        }



        private void listBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ControlsNameContextMenu = (sender as ListBox).SelectedIndex;
        }
      
        // выделить элемент
        private void listBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ControlsNameContextMenu = (sender as ListBox).SelectedIndex;
        }

        private void listBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ControlsNameContextMenu = (sender as ListBox).SelectedIndex;
        }


        private void btnDelUser_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            string strQueryEdit = "Delete from Users where Id=@id";
            SqlCommand commandUpdate = new SqlCommand(strQueryEdit, connect);
            commandUpdate.Parameters.AddWithValue("@id", ControlsNameContextMenu+1);
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
            LoginsList.RemoveAt(ControlsNameContextMenu);
            MessageBox.Show("The Data removed", "Delete Item");
            listBox.Items.Clear();
            for (int i = 0; i < LoginsList.Count; i++)
            {
                listBox.Items.Add(LoginsList[i]);
            }
        }


        public int ContMenuOwnerName
        {
            get
            {
                return ControlsNameContextMenu;
            }
        }

        
    }
}