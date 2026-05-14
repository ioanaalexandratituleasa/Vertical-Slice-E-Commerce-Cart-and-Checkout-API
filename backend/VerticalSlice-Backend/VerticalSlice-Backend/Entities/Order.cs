namespace VerticalSlice_Backend.Entities
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateOnly DateOrder { get; set; }
        public string Adress { get; set; }
        public int UserID { get; set; }

    }
}
