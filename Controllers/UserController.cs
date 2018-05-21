using Microsoft.AspNetCore.Mvc;
using aspNetCore.Repository;
using Microsoft.AspNetCore.Authorization;

namespace aspNetCore.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
		// GET: api/User
		[Authorize("Bearer")]
		[HttpGet]
        public IActionResult Get()
        {
			var repository = new UserRepository();			
			return new ObjectResult(repository.GetUsers());
        }

		// GET: api/User/id

		// POST: api/User
        
        // PUT: api/User/id
        
        // DELETE: api/ApiWithActions/id

    }
}
