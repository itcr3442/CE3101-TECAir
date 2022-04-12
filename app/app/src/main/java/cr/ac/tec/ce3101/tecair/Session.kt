package cr.ac.tec.ce3101.tecair

interface Session
{
    fun login(auth: (Boolean)-> Unit)
    fun registerUser(user: User, afterOp : (Boolean)->Unit)
    fun getPromoList(): List<Promo>
    fun getFlights(): List<String>
    fun getFlightInfo(): Flight
    fun makeReservation(reservation: Reservation, afterOp: (Boolean) -> Unit)
}