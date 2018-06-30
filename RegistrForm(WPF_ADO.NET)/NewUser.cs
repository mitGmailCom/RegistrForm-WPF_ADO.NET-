using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrForm_WPF_ADO.NET_
{
    public class NewUser:IDataErrorInfo
    {
        string LoginUser { get; set; }
        string PasswordUser { get; set; }
        string EmailUser { get; set; }
        string AdressUser { get; set; }
        long PhoneUser { get; set; }
        bool IsAdminUser { get; set; }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "PhoneUser":
                        if ((PhoneUser.ToString()).Length > 12)
                        {
                            error = "Length > 12";
                        }
                        break;
                    //case "Name":
                    //    //Обработка ошибок для свойства Name
                    //    break;
                    //case "Position":
                    //    //Обработка ошибок для свойства Position
                    //    break;
                }
                return error;
            }
        }

        public NewUser()
        { }
    }
}
