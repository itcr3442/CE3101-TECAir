package cr.ac.tec.ce3101.tecair

import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Query

/**
 * Server endpoint definitions
 */
interface TECAirService {
    /**
     * Request to validate credentials of the user
     */
    @GET("check_login")
    fun checkLogin(
        @Query("username") username: String,
        @Query("username") password: String
    ): Call<Unit>

    /**
     * Request to create a new user
     */
    @POST("path-to-user/INSERT")
    fun addUser(@Body user: User): Call<Unit>

    /**
     * Request to delete user
     */
    @POST("path-to-user/DELETE")
    fun deleteUser(@Body user: User): Call<Unit>

    /**
     * Request to update user
     */
    @POST("path-to-user/UPDATE")
    fun updateUser(@Body user: User): Call<Unit>

    /**
     * Request to make a flight reservarion
     */
    @POST("booking url")
    fun makeBooking(@Body booking: Booking): Call<Unit>

    /**
     * Request to get the active promo list
     */
    @GET("promos")
    fun getPromoList(): Call<List<Promo>>
    /**
     * Request to get the list of available flights
     */
    @GET("flights")
    fun getFlightList(): Call<List<Flight>>
    /**
     * Request to get the list of all active flight segments
     */
    @GET("segments")
    fun getSegmentList(): Call<List<Segment>>
    
    /**
     * Request to retrieve the list of active flights with their path
     */
    @GET("flights-with-path")
    fun getFlightsWithPath(
        @Query("from") from: String,
        @Query("to") to: String
    ): Call<List<FlightWithPath>>
}

