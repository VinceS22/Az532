using Microsoft.WindowsAzure.Storage.Table;

namespace Az532
{
    public class Car : TableEntity
    {
        public Car()
        {

        }

        public Car (int id, int year, string make, string model, string color)
        {
            ID = id;
            Year = year;
            Make = make;
            Model = model;
            Color = color;
            PartitionKey = "car";
            RowKey = ID.ToString();
        }

        public int ID { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
    }
}
