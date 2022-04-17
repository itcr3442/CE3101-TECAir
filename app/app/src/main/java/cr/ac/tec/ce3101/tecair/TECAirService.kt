package cr.ac.tec.ce3101.tecair

import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Query

interface TECAirService {
    @GET("check_login")
    fun checkLogin(
        @Query("username") username: String,
        @Query("username") password: String
    ): Call<Unit>

    @POST("path-to-user/INSERT")
    fun addUser(@Body user: User): Call<Unit>

    @POST("path-to-user/DELETE")
    fun deleteUser(@Body user: User): Call<Unit>

    @POST("path-to-user/UPDATE")
    fun updateUser(@Body user: User): Call<Unit>

    @POST("booking url")
    fun makeBooking(@Body booking: Booking): Call<Unit>

    @GET("promos")
    fun getPromoList(): Call<List<Promo>>
    @GET("flights")
    fun getFlightList(): Call<List<Flight>>
    @GET("segments")
    fun getSegmentList(): Call<List<Segment>>

    @GET("flights-with-path")
    fun getFlightsWithPath(
        @Query("from") from: String,
        @Query("to") to: String
    ): Call<List<FlightWithPath>>
}

