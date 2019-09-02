using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1Users
{
    class Program
    {
        static void Main(string[] args)
        {
            var DBUsers = new SystemUsers(new User[]
            {
                new User(0, "Сергей", 100),
                new User(1, "Егор", 200),
                new User(2, "Александр", 10)
            });
        }
    }

    class User
    {
        public int Id;
        public string Name;
        public float Salary;

        public User(int id, string name, float salary)
        {
            Id = id;
            Name = name;
            Salary = salary;
        }
    }

    class SystemUsers
    {
        private User[] Users;

        public SystemUsers(User[] users)
        {
            Users = users;
        }

        public User GetById(int id)
        {
            foreach (var item in Users)
                if (item.Id == id)
                    return item;

            return null;
        }

        public User FirstByName(string name)
        {
            foreach (var item in Users)
                if (item.Name == name)
                    return item;

            return null;
        }

        public User[] SalaryGreaterThan(float comparable) => Users.Where(user => user.Salary > comparable).ToArray();
        public User[] SalaryLessThan(float comparable) => Users.Where(user => user.Salary < comparable).ToArray();
        public User[] SalaryBetween(float min, float max) => Users.Where(user => min <= user.Salary && user.Salary <= max).ToArray();

    }
       
}
