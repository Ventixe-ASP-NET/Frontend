namespace WebApp.Views.Bookings.BookingModels
{
    public class BookingIndexViewModel
    {
        public List<BookingWithEventModel> Bookings { get; set; } = new();
        public BookingStatsModel Stats { get; set; } = new();
        public BookingChartModel Chart { get; set; }
        public TopCategoriesModel TopCategories { get; set; }
    }
}
