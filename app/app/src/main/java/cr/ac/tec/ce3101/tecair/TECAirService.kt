package cr.ac.tec.ce3101.tecair

import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.Query

interface TECAirService {
    @GET("check_login")
    fun checkLogin(@Query("username") username: String, @Query("username") password: String): Call<Unit>

    @GET("promos")
    fun getPromoList(): Call<List<Promo>>

    @GET("reservations")
    fun getReservationList(): Call<List<Reservation>>

    @GET("users")
    fun getUserList(): Call<List<User>>

    @GET("flights")
    fun getFlights(): Call<List<Flight>>
}

