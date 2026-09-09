namespace FastFood.Enums;

public enum eOrderState
{
    Created = 1,        // Kreirana (poslata iz aplikacije)
    Ready = 2,          // Spremna za preuzimanje
    Completed = 3,      // Preuzeta / zavrsena
    Cancelled = 4       // Otkazana
}

