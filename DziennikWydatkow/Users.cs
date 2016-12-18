using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DziennikWydatkow
{
    class Users
    {
        public List<User> UserList; 

        public Users()
        {
            UserList = new List<User>();
        }

        public void addUser(User u)
        {
            UserList.Add(u);
        }

        public bool userExists(string _Username)
        {
            return UserList.Exists(x => String.Compare(x.Username,_Username)==0);
        }

        public bool checkUserPassword(string _Username)
        {
            return UserList.Exists(x => String.Compare(x.Username, _Username) == 0);
        }

        public void Serialize()
        {
            using (FileStream writer = new FileStream("users.xml", FileMode.Create, FileAccess.Write))
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(List<User>));
                ser.WriteObject(writer, UserList);
            }
        }


        public void Deserialize()
        {
            try
            {
                using (FileStream reader = new FileStream("users.xml", FileMode.Open, FileAccess.Read))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(List<User>));
                    UserList = (List<User>)ser.ReadObject(reader);
                }
            }
            catch (FileNotFoundException e)
            {
                UserList = new List<User>();
            }
        }

    }
}
