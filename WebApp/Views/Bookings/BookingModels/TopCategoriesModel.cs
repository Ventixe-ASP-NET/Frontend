namespace WebApp.Views.Bookings.BookingModels
{
    public class TopCategoriesModel
    {
        public int TotalBookings { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = new();
    }

    public class CategoryViewModel
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
