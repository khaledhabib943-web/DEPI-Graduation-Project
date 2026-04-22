using Microsoft.AspNetCore.Mvc;

public class CustomerController : Controller
{
    private List<CustomerViewModel> customers = new List<CustomerViewModel>
    {
        new CustomerViewModel
        {
            Id = 1,
            FullName = "Saraaa",
            Email = "saraaaaa@gmail.com",
            Address = "Cairo",
            PhoneNumber = "01000000000"
        },
        new CustomerViewModel
        {
            Id = 2,
            FullName = "Sara Mohamed",
            Email = "sara@gmail.com",
            Address = "Alexandria",
            PhoneNumber = "01111111111"
        }
    };

    public IActionResult Index()
    {
        return View(customers);
    }

    public IActionResult Details(int id)
    {
        var customer = customers.FirstOrDefault(c => c.Id == id);

        if (customer == null)
            return NotFound();

        return View(customer);
    }
}