package cr.ac.tec.ce3101.tecair

/**
 * Defines the base operations that any type of session should be 
 * able to perform
*/
interface Session {
    /**
     * Retrieves the username used in the session
     */
    fun getUsername(): String
    /**
     * Retrieves the password used in the session
     */
    fun getPassword(): String
    /**
     * Authenticates the current user
     */
    fun login(auth: (Boolean) -> Unit)
    /**
     *  Adds a new user to the system
     */
    fun registerUser(user: User, afterOp: (Boolean) -> Unit)
    /**
     *  Updates the information related to a particular user
     */
    fun editUser(user: User, afterOp: (Boolean) -> Unit)
    /**
     *  Deletes a registered user
     */
    fun deleteUser(user: User, afterOp: (Boolean) -> Unit)
    /**
     *  Retrieves the information of all promotions available
     */
    fun getPromoList(afterOp: (List<Promo>) -> Unit)
    /**
     *  Retrieves the information of all available flights
     */
    fun getFlights(from: String, to: String, afterOp: (List<FlightWithPath>) -> Unit)
    /**
     *  Retrieves the list of users who have logged into the application in the past
     */
    fun getUserList(): List<User>
    /**
     *  Process a flight reservation request
     */
    fun makeBooking(flight: String, afterOp: (Boolean) -> Unit)
}
