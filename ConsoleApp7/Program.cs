using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleApp7
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequirePermissionAttribute : Attribute
    {
        public string Permission { get; }
        public RequirePermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
    public class UserService
    {
        [RequirePermission("Users.Create")]


        public void CreateUser()
        {
            Console.WriteLine("User Created");
        }
        [RequirePermission("Users.Delete")]

        public void DeleteUser()
        {
            Console.WriteLine("User Deleted");
        }


        [RequirePermission("Users.View")]
        public void ViewUsers()
        {
            Console.WriteLine("Displaying Users");
        }
    }

    public class User
    {
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            User user = new User
            {
                Name = "Yaseen",
                Permissions = new List<string> {  "Users.Create",  "Users.View" } };
            Type type = typeof(UserService);
            MethodInfo method = type.GetMethod("DeleteUser");
            RequirePermissionAttribute attribute =
                method.GetCustomAttribute<RequirePermissionAttribute>();

            Console.WriteLine(attribute.Permission);
            Console.ReadKey();
        }
    }
}
