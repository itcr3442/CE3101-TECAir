package cr.ac.tec.ce3101.tecair

import android.content.Context
import androidx.room.Room

class OfflineSession(
    private val username: String,
    private val password: String,
    private val cx: Context,
    private val localDB: LocalDB =
        Room.databaseBuilder(cx.applicationContext, LocalDB::class.java, "local-data")
            .allowMainThreadQueries().build()
) : Session {
    override fun login(auth: (Boolean) -> Unit) {
        val users = localDB.userDao().findCredentials(username, password)
        if (users.isNotEmpty()) {
            auth(true)
        } else {
            auth(false)
        }
    }

    override fun registerUser(user: User, afterOp: (Boolean) -> Unit) {
        val users = localDB.userDao().findUser(user.username)
        if (users.isEmpty()) {
            localDB.userDao().insertAll(user)
            afterOp(true)
        } else {
            afterOp(false)
        }
    }

    override fun deleteUser(user: User, afterOp: (Boolean) -> Unit) {
        val users = localDB.userDao().findUser(user.username)
        if (users.isEmpty()) {
            localDB.userDao().delete(user)
            afterOp(true)
        } else {
            afterOp(false)
        }

    }

    override fun getPromoList(): List<Promo> {
        return localDB.promoDao().getAll()
    }

    override fun getFlights(): List<Flight> {
        return localDB.flightDao().getAll()
    }

    override fun getUserList(): List<User> {
        return localDB.userDao().getAll()
    }

    override fun makeReservation(reservation: Reservation, afterOp: (Boolean) -> Unit) {
        TODO("Not yet implemented")
    }
}