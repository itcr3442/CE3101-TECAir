package cr.ac.tec.ce3101.tecair

import android.content.Context
import androidx.room.Room
import java.util.*

/** 
 * Session type for user session without connection to the main server
 * Look into [Session] for documentation on the overriden functions
 */
class OfflineSession(
    private val username: String,
    private val password: String,
    var cx: Context,
    private val cache: LocalDB =
        Room.databaseBuilder(cx.applicationContext, LocalDB::class.java, "cache")
            .allowMainThreadQueries().build(),
    private val pendingOps: PendingOpDB =
        Room.databaseBuilder(cx.applicationContext, PendingOpDB::class.java, "pending-ops")
            .allowMainThreadQueries().build()
) : Session {
    override fun changeContext(cx: Context) {
        this.cx = cx
    }

    override fun getUsername(): String {
        return username
    }

    override fun getPassword(): String {
        return password
    }

    override fun login(auth: (Boolean) -> Unit) {
        val credentials = cache.userDao().findCredentials(username, password)
        if (credentials != null) {
            auth(true)
        } else {
            auth(false)
        }
    }

    override fun registerUser(user: User, afterOp: (Boolean) -> Unit) {
        val previous = cache.userDao().findUser(user.username)
        if (previous == null) {
            cache.userDao().insertAll(user)
            pendingOps.userOpDao().insertAll(UserOp(UUID.randomUUID().toString(), "INSERT", user))
            afterOp(true)
        } else {
            afterOp(false)
        }
    }

    override fun editUser(user: User, afterOp: (Boolean) -> Unit) {
        val previous = cache.userDao().findUser(user.username)
        if (previous !=null ) {
            cache.userDao().updateUser(user)
            pendingOps.userOpDao().insertAll(UserOp(UUID.randomUUID().toString(), "UPDATE", user))
            afterOp(true)
        }else{
            afterOp(false)
        }
    }

    override fun deleteUser(user: User, afterOp: (Boolean) -> Unit) {
        val previous = cache.userDao().findUser(user.username)
        if (previous !=null ) {
            cache.userDao().delete(user)
            pendingOps.userOpDao().insertAll(UserOp(UUID.randomUUID().toString(), "DELETE", user))
            afterOp(true)
        } else {
            afterOp(false)
        }
    }

    override fun getPromoList(afterOp: (List<Promo>) -> Unit) {
        afterOp(cache.promoDao().getAll())
    }

    override fun getFlights(from: String, to: String, afterOp: (List<FlightWithPath>) -> Unit){
        val filteredFlights = mutableListOf<FlightWithPath>()
        val flights = cache.flightDao().getAll()
        flights.forEach lit@{ flight ->
            run {
                val segments =
                    cache.segmentDao().getFlightSegments(flight.id)
                var path = mutableListOf<Segment>()
                var i = 0
                var foundFrom = false
                while (i < segments.size) {
                    if (segments[i].fromLoc == from) {
                        foundFrom = true
                        break
                    }
                    i++
                }
                if (!foundFrom) {
                    return@lit
                }
                path.add(segments[i])
                if (segments[i].toLoc == to) {
                    filteredFlights.add(FlightWithPath(flight, path))
                } else {
                    i++
                    var success = false;
                    while (i < segments.size) {
                        path.add(segments[i])
                        if (segments[i].toLoc == to) {
                            success = true
                            break
                        }
                        i++
                    }
                    if (success) {
                        filteredFlights.add(FlightWithPath(flight, path))
                    }
                }

            }
        }
        afterOp(filteredFlights.toList())
    }

    override fun getFlightById(id: String): Flight? {
        return cache.flightDao().getFlightById(id)
    }

    override fun getUserList(forEachUser: (User)-> Unit ) {
        cache.userDao().getAll().forEach(forEachUser)
    }

    override fun makeBooking(flight: String, promoCode: String, afterOp: (Boolean) -> Unit) {
        val previous = pendingOps.BookingDao().getBooking(username, flight)
        if (previous != null){
            afterOp(false)
        }else{
            var promo = cache.promoDao().getForFlight(flight, promoCode)
            val currentUser = cache.userDao().findUser(username) ?: return
            pendingOps.BookingDao().insertAll(Booking(flight, currentUser.id, promo))
            afterOp(true)
        }
    }
}
