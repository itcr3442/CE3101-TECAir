package cr.ac.tec.ce3101.tecair

import android.content.Context
import androidx.room.Room
import com.google.gson.GsonBuilder
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

/**
 * Session type for user session with connection to the main server
 * Look into [Session] for documentation on the overriden functions
 */
class OnlineSession(
    url: String,
    private val username: String,
    private val password: String,
    private val cx: Context,
    private val cache: LocalDB =
        Room.databaseBuilder(cx.applicationContext, LocalDB::class.java, "cache")
            .allowMainThreadQueries().build(),
    private val pendingOps: PendingOpDB =
        Room.databaseBuilder(cx.applicationContext, PendingOpDB::class.java, "pending-ops")
            .allowMainThreadQueries().build()
) : Session {
    private val service: TECAirService

    init {
        val gson = GsonBuilder().serializeNulls().create()
        val retrofit =
            Retrofit.Builder().baseUrl(url).addConverterFactory(GsonConverterFactory.create(gson))
                .build()
        service = retrofit.create(TECAirService::class.java)
    }

    override fun getUsername(): String {
        return username
    }

    override fun getPassword(): String {
        return password
    }

    /**
     * Syncrhonization routine. Performs al pending operations and refreshes the local
     * database cache
     */
    fun synchronize(): Boolean {
        //re-validate saved credentials
        val localUsers = cache.userDao().getAll()
        //we'll repopulate them after user checks
        cache.clearAllTables()

        localUsers.forEach { user ->
            run {
                service.checkLogin(user.username, user.password).enqueue(
                    object : Cb<String>() {
                        override fun onResponse(call: Call<String>, response: Response<String>) {
                            if (response.code() == 200) {
                                service.getUserInfo(response.body()!!).enqueue(
                                    object : Cb<UserInfo>() {
                                        override fun onResponse(
                                            call: Call<UserInfo>,
                                            response: Response<UserInfo>
                                        ) {
                                            if (response.code() == 200) {
                                                val userinfo = response.body()!!
                                                val currentUser = User(
                                                    userinfo.type,
                                                    userinfo.id,
                                                    user.username,
                                                    user.password,
                                                    userinfo.firstName,
                                                    userinfo.lastName,
                                                    userinfo.phonenumber,
                                                    userinfo.email,
                                                    userinfo.university,
                                                    userinfo.studentId
                                                )
                                                cache.userDao().insertAll(currentUser)
                                            }
                                        }
                                    })
                            }
                        }
                    }
                )
            }
        }
        //send user changes
        val userChanges = pendingOps.userOpDao().getAll()
        userChanges.forEach { change ->
            run {
                if (change.user.username != username) {
                    when (change.operation) {
                        "INSERT" -> registerUser(change.user) {}
                        "UPDATE" -> editUser(change.user) {}
                        "DELETE" -> deleteUser(change.user) {}
                    }
                    pendingOps.userOpDao().delete(change)
                } else {
                    return false
                }
            }
        }

        //send booking info
        pendingOps.BookingDao().getAll().forEach { booking: Booking ->
            run {
                service.makeBooking(booking.flight, booking).enqueue(
                    object : Cb<Unit>() {
                        override fun onResponse(call: Call<Unit>, response: Response<Unit>) {
                            if (response.code() != 200) {
                                simpleDialog(cx, cx.getString(R.string.booking_sync_error))
                            }else{
                                pendingOps.BookingDao().delete(booking)
                            }
                        }
                    }
                )
            }
        }

        //update flights
        service.getFlightList().enqueue(
            object : Cb<List<Flight>>() {
                override fun onResponse(
                    call: Call<List<Flight>>,
                    response: Response<List<Flight>>
                ) {
                    response.body()?.forEach { flight -> cache.flightDao().insertAll(flight) }
                }
            }
        )
        //update segments
        service.getSegmentList().enqueue(
            object : Cb<List<Segment>>() {
                override fun onResponse(
                    call: Call<List<Segment>>,
                    response: Response<List<Segment>>
                ) {
                    response.body()?.forEach { segment -> cache.segmentDao().insertAll(segment) }
                }
            }
        )
        //update promos
        service.getPromoList().enqueue(
            object : Cb<List<Promo>>() {
                override fun onResponse(call: Call<List<Promo>>, response: Response<List<Promo>>) {
                    response.body()?.forEach { promo -> cache.promoDao().insertAll(promo) }
                }
            }
        )
        return true
    }

    override fun login(auth: (Boolean) -> Unit) {
        service.checkLogin(username, password).enqueue(
            object : Cb<String>() {
                override fun onResponse(call: Call<String>, response: Response<String>) {
                    if (response.code() == 200) {
                        //
                        service.getUserInfo(response.body()!!).enqueue(
                            object : Cb<UserInfo>() {
                                override fun onResponse(
                                    call: Call<UserInfo>,
                                    response: Response<UserInfo>
                                ) {
                                    if (response.code() == 200) {
                                        val userinfo = response.body()!!
                                        val currentUser = User(
                                            userinfo.type,
                                            userinfo.id,
                                            username,
                                            password,
                                            userinfo.firstName,
                                            userinfo.lastName,
                                            userinfo.phonenumber,
                                            userinfo.email,
                                            userinfo.university,
                                            userinfo.studentId
                                        )
                                        cache.userDao().deleteByUsername(username)
                                        cache.userDao().insertAll(currentUser)
                                        auth(synchronize())
                                    } else {
                                        auth(false)
                                    }
                                }
                            }
                        )
                        //
                    } else {
                        auth(false)
                    }
                }
            }
        )
    }

    override fun registerUser(user: User, afterOp: (Boolean) -> Unit) {
        service.addUser(user).enqueue(
            object : Cb<String>() {
                override fun onResponse(call: Call<String>, response: Response<String>) {
                    if (response.code() == 200) {
                        cache.userDao().insertAll(swapUserID(user, response.body()!!))
                        afterOp(true)
                    } else {
                        afterOp(false)
                    }
                }
            }
        )
    }

    override fun editUser(user: User, afterOp: (Boolean) -> Unit) {
        service.updateUser(user.id, user).enqueue(
            object : Cb<Unit>() {
                override fun onResponse(call: Call<Unit>, response: Response<Unit>) {
                    if (response.code() == 200) {
                        afterOp(true)
                        cache.userDao().updateUser(user)
                    } else {
                        afterOp(false)
                    }
                }
            }
        )
    }

    override fun deleteUser(user: User, afterOp: (Boolean) -> Unit) {
        service.deleteUser(user.id).enqueue(
            object : Cb<Unit>() {
                override fun onResponse(call: Call<Unit>, response: Response<Unit>) {
                    if (response.code() == 200) {
                        afterOp(true)
                        cache.userDao().delete(user)
                    } else {
                        afterOp(false)
                    }
                }
            }
        )
    }

    override fun getPromoList(afterOp: (List<Promo>) -> Unit) {
        service.getPromoList().enqueue(
            object : Cb<List<Promo>>() {
                override fun onResponse(call: Call<List<Promo>>, response: Response<List<Promo>>) {
                    cache.promoDao().clearTable()
                    val promos = response.body()!!
                    promos.forEach { promo ->
                        cache.promoDao().insertAll(promo)
                    }
                    afterOp(promos)
                }
            }
        )
    }

    override fun getFlights(from: String, to: String, afterOp: (List<FlightWithPath>) -> Unit) {
        cache.flightDao().clearTable()
        cache.segmentDao().clearTable()
        //update flights
        service.getFlightList().enqueue(
            object : Cb<List<Flight>>() {
                override fun onResponse(
                    call: Call<List<Flight>>,
                    response: Response<List<Flight>>
                ) {
                    response.body()?.forEach { flight -> cache.flightDao().insertAll(flight) }
                    //update segments
                    service.getSegmentList().enqueue(
                        object : Cb<List<Segment>>() {
                            override fun onResponse(
                                call: Call<List<Segment>>,
                                response: Response<List<Segment>>
                            ) {
                                response.body()
                                    ?.forEach { segment -> cache.segmentDao().insertAll(segment) }

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
                        }
                    )
                }
            }
        )
    }


    override fun getFlightById(id: String): Flight? {
        return cache.flightDao().getFlightById(id)
    }


    override fun getUserList(forEachUser: (User) -> Unit) {
        val users = cache.userDao().getAll()
        users.forEach { user: User ->
            run {
                service.checkLogin(user.username, user.password).enqueue(
                    object : Cb<String>() {
                        override fun onResponse(call: Call<String>, response: Response<String>) {
                            if (response.code() == 200) {
                                forEachUser(user)
                            } else {
                                //user information removed as is no longer valid
                                cache.userDao().delete(user)
                            }
                        }
                    }
                )
            }
        }
    }

    override fun makeBooking(flight: String, promoCode: String, afterOp: (Boolean) -> Unit) {
        val previous = pendingOps.BookingDao().getBooking(username, flight)
        if (previous != null) {
            afterOp(false)
        }else {
            var promo = cache.promoDao().getForFlight(flight, promoCode)
            val currentUser = cache.userDao().findUser(username) ?: return
            val booking = Booking(flight, currentUser.id, promo)
            service.makeBooking(flight, booking).enqueue(
                object : Cb<Unit>() {
                    override fun onResponse(call: Call<Unit>, response: Response<Unit>) {
                        if (response.code() == 200) {
                            afterOp(true)
                            simpleDialog(cx, "Enjoy your flight")
                        } else {
                            afterOp(false)
                        }
                    }
                }
            )
        }
    }

    /**
     * Callback wrapper class.
     * In case of request failure, the application must communicate
     */
    abstract inner class Cb<T> : Callback<T> {
        override fun onFailure(call: Call<T>, err: Throwable) {
            simpleDialog(cx, "Network Error")
        }
    }

}
