
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleApp7
{
    // Custom Attribute used to define
    // the permission required by a method.
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
            // Create a user with specific permissions.
            User user = new User
            {
                Name = "Yaseen",
                Permissions = new List<string>
                {
                    "Users.Create",
                    "Users.View"
                }
            };

            // Get the UserService type using Reflection.
            Type type = typeof(UserService);

            // Get the DeleteUser method using Reflection.
            MethodInfo method = type.GetMethod("DeleteUser");

            // Create an instance of UserService.
            UserService service = new UserService();

            // Read the Custom Attribute from the method.
            RequirePermissionAttribute attribute =
                method?.GetCustomAttribute<RequirePermissionAttribute>();

            // If the method does not require a permission,
            // execute it directly.
            if (attribute == null)
            {
                method?.Invoke(service, null);
            }
            // Check if the current user has the required permission.
            else if (user.Permissions.Contains(attribute.Permission))
            {
                Console.WriteLine("Access Granted");

                // Execute the method dynamically using Reflection.
                method.Invoke(service, null);
            }
            else
            {
                Console.WriteLine("Access Denied");
            }

            Console.ReadKey();
        }
    }
}
