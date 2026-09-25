using BikeRental.Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BikeRental.Api.Controllers
{
    [Route("api/bikes")]
    [ApiController]
    public class BikesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetBikes()
        {
            List<Bike> bikes = new List<Bike>();
            bikes.Add(new Bike(1, "Jopo"));
            return Ok(bikes);
        }
    }
}
