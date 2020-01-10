using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterMelon_API.Models;

namespace WaterMelon_API.Helpers
{
    public static class UserExtensionMethods
    {
        public static IEnumerable<User> WithoutPassword(this IEnumerable<User> users)
        {
            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            user.Password = null;
            return user;
        }
    }
}
