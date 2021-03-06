package cr.ac.tec.ce3101.tecair

import retrofit2.Call
import retrofit2.http.*

/**
 * Server endpoint definitions
 */
interface TECAirService {
    /**
     * Request to validate credentials of the user
     */
    @POST("check_login")
    fun checkLogin(
        @Query("username") username: String,
        @Query("password") password: String
    ): Call<String>

    /**
     * Request user data
     */
    @GET("users/{id}")
    fun getUserInfo(@Path("id") id: String): Call<UserInfo>

    /**
     * Request to create a new user
     */
    @POST("users")
    fun addUser(@Body user: User): Call<String>

    /**
     * Request to delete user
     */
    @DELETE("users/{id}")
    fun deleteUser(@Path("id") id: String): Call<Unit>

    /**
     * Request to update user
     */
    @PUT("users/{id}")
    fun updateUser(@Path("id") id: String, @Body user: User): Call<Unit>

    /**
     * Request to make a flight reservarion
     */
    @POST("flights/{id}/book")
    fun makeBooking(@Path("id") flightId: String, @Body booking: Booking): Call<Unit>

    /**
     * Request to get the active promo list
     */
    @GET("promos")
    fun getPromoList(): Call<List<Promo>>
    /**
     * Request to get the list of available flights
     */
    @GET("flights/booking")
    fun getFlightList(): Call<List<Flight>>
    /**
     * Request to get the list of all active flight segments
     */
    @GET("segments/booking")
    fun getSegmentList(): Call<List<Segment>>
}

data class UserInfo(
    val id: String,
    val firstName: String,
    val lastName: String,
    val phonenumber: String,
    val email: String,
    val university: String,
    val studentId: String,
    val type: Int,
)