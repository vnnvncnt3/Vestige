namespace VestigeApp.Models;

/// <summary>A guided walking tour offered by Vestige.</summary>
public record Tour(
    string Name,
    string Tag,
    string Blurb,
    string Image,
    string Duration,
    int MaxGuests,
    IReadOnlyList<string> Includes);
