namespace BikeRental.Api.Model
{
    public class Bike
    {
        public int Id { get; set; }
        public string Model { get; set; }

        public Bike(int id, string name)
        {
            Id = id;
            Model = name;
        }
    }
}
