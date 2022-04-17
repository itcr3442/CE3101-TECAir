package cr.ac.tec.ce3101.tecair

interface Session {
    fun getUsername(): String
    fun getPassword(): String
    fun login(auth: (Boolean) -> Unit)
    fun registerUser(user: User, afterOp: (Boolean) -> Unit)
    fun editUser(user: User, afterOp: (Boolean) -> Unit)
    fun deleteUser(user: User, afterOp: (Boolean) -> Unit)
    fun getPromoList(afterOp: (List<Promo>) -> Unit)
    fun getFlights(from: String, to: String, afterOp: (List<FlightWithPath>) -> Unit)
    fun getUserList(): List<User>
    fun makeBooking(flight: String, afterOp: (Boolean) -> Unit)
}