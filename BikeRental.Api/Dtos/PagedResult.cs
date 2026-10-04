namespace BikeRental.Api.Dtos;

// Wrapper for any paged list. <T> means "a list of anything":
// PagedResult<BikeDto> is a page of bikes, and later
// PagedResult<RentalDto> could be a page of rentals.
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    // Computed, not stored: 5 bikes with pageSize 2 → 3 pages.
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}