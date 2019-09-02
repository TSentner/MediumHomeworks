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
                new User(1, "Александр", 10)
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
            if (CheckUniqueId(users))
                Users = users;
            else
                throw new Exception("When creating SystemUsers, a non-unique user was found");
            
        }

        private bool CheckUniqueId(User[] users) 
            => !users.GroupBy(user => user.Id)
                     .Select(group => new { Count = group.Count()})
                     .Any(group => group.Count > 1);

        public User GetById(int id)
        {
            var query = Filter(user => user.Id == id);
            return query.Length > 0 ? query[0] : null;
        }

        public User FirstByName(string name)
        {
            var query = Filter(user => user.Name == name);
            return query.Length > 0 ? query[0] : null;
        }

        public User[] SalaryGreaterThan(float comparable) => Filter(user => user.Salary > comparable);
        public User[] SalaryLessThan(float comparable) => Filter(user => user.Salary < comparable);
        public User[] SalaryBetween(float min, float max) => Filter(user => min <= user.Salary && user.Salary <= max);


        private User[] Filter(Predicate<User> query) => Users.Where(user => query(user)).ToArray();
    }
       
}
