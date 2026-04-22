using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private List<UserViewModel> users = new List<UserViewModel>
    {
        new UserViewModel
        {
            Id = 1,
            FullName = "Saraaa",
            Email = "saraaaaa@gmail.com",
            PhoneNumber = "01000000000",
            Username = "sarsora123",
            Role = "Customer",
            IsActive = true
        },
        new UserViewModel
        {
            Id = 2,
            FullName = "Sara Mohamed",
            Email = "sara@gmail.com",
            PhoneNumber = "01111111111",
            Username = "sara456",
            Role = "Worker",
            IsActive = false
        }
    };

  
    public IActionResult Index()
    {
        return View(users);
    }

    public IActionResult Details(int id)
    {
        var user = users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return View(user);
    }
}