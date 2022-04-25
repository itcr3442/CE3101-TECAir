namespace backend.dal;

// Estos tipos son enumeraciones que definen dominios discretos.  Se utilizan
// para restringir los valores posibles en los campos particulares en el modelo
// relacional.

public enum UserType
{
    Pax,
    Manager,
}

public enum FlightState
{
    Booking,
    Checkin,
    Closed,
}
