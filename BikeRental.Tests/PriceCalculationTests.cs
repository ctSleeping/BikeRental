using BikeRental.Api.Services;

namespace BikeRental.Tests;

public class PriceCalculationTests
{
    private static readonly DateTime Start = new(2026, 10, 15, 9, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(0, 3.50)]     // minimum charge is one hour
    [InlineData(10, 3.50)]    // 10 min -> 1 h
    [InlineData(60, 3.50)]    // exactly 1 h -> 1 h
    [InlineData(61, 7.00)]    // just over 1 h -> 2 h
    [InlineData(140, 10.50)]  // 2 h 20 min -> 3 h
    public void CalculatePrice_ChargesEveryStartedHour(int minutes, double expected)
    {
        var end = Start.AddMinutes(minutes);

        var price = RentalService.CalculatePrice(Start, end, 3.50m);

        Assert.Equal((decimal)expected, price);
    }

    [Fact]
    public void CalculatePrice_UsesGivenHourlyPrice()
    {
        var price = RentalService.CalculatePrice(Start, Start.AddHours(2), 8.00m);

        Assert.Equal(16.00m, price);
    }
}