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
            var DBUsers = new UsersManager(new User[]
            {
                new User(0, "Сергей", 100),
                new User(1, "Егор", 200),
                new User(2, "Александр", 10)
            });
        }
    }

    class User
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public float Salary { get; private set; }

        public User(int id, string name, float salary)
        {
            Id = id;
            Name = name;
            Salary = salary;
        }
    }

    class UsersManager
    {
        private User[] _users;

        public UsersManager(User[] users)
        {
            if (CheckUniqueId(users))
                _users = users;
            else
                throw new Exception("When creating SystemUsers, a non-unique user was found");
        }

        private bool CheckUniqueId(User[] users)
        {
            var allId = (from user in users
                         orderby user.Id
                         select user.Id).ToArray();
            for (int i = 1; i < allId.Length; i++)
                if (allId[i - 1] == allId[i])
                    return false;

            return true;
        }


        public User GetById(int id) => _users.FirstOrDefault(user => user.Id == id);
        public User FirstByName(string name) => _users.FirstOrDefault(user => user.Name == name);

        public User[] SalaryGreaterThan(float comparable) => Filter(user => user.Salary > comparable);
        public User[] SalaryLessThan(float comparable) => Filter(user => user.Salary < comparable);
        public User[] SalaryBetween(float min, float max) => Filter(user => min <= user.Salary && user.Salary <= max);


        private User[] Filter(Predicate<User> query) => _users.Where(user => query(user)).ToArray();
    }
       
}
