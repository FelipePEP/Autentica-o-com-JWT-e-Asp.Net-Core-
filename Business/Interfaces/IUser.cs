using System.Collections.Generic;
using aspNetCore.Business.Models;

namespace aspNetCore.Business.Interfaces
{
    public interface IUser
    {
		IList<User> GetUsers();
		User GetUser(string login);
    }
}
