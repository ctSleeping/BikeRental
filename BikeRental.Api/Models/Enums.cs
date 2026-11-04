namespace BikeRental.Api.Models;

public enum BikeType { City, Mountain, Electric, Kids }

public enum UserRole { Customer, Admin }

// Computed bike status. Not stored in the database.
public enum BikeStatus { Available, Rented, InMaintenance }