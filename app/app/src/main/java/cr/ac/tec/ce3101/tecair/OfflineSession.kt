package cr.ac.tec.ce3101.tecair

import android.content.Context

class OfflineSession(
    private val username: String,
    private val  password: String,
    private val cx: Context
): Session {
    override fun login(auth: (Boolean) -> Unit) {
        val users = (cx as TECAirApp).localDB.userDao().findUser(username, password)
        if (users.isNotEmpty()){
            auth(true)
        }else{
            auth(false)
        }
    }

    override fun registerUser(user: User, afterOp: (Boolean) -> Unit) {
        TODO("Not yet implemented")
    }

    override fun getPromoList(): List<Promo> {
        TODO("Not yet implemented")
    }

    override fun getFlights(): List<String> {
        TODO("Not yet implemented")
    }

    override fun getFlightInfo(): Flight {
        TODO("Not yet implemented")
    }

    override fun makeReservation(reservation: Reservation, afterOp: (Boolean) -> Unit) {
        TODO("Not yet implemented")
    }
}