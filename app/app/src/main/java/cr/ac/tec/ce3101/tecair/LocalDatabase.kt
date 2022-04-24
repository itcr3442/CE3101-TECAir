package cr.ac.tec.ce3101.tecair

import androidx.room.*
import java.util.*

/**
 * Room is a library that simplifies interactions with SQLite. The underlying data is still in SQLite,
 * but operations complexity is reduced. Room is recommended instead of direct interaction with
 * the SQLite APIs as noted in this article: https://developer.android.com/training/data-storage/room#kts
 */


/**
 * Database intended to store the latest information retrieved from the online
 * database. Must be synchronized frequently as to mantain a close resemblance
 * to the Online database because it works as the local database in offline mode
 */
@Database(
    entities = [User::class, Flight::class, Segment::class, Promo::class],
    version = 1
)
abstract class LocalDB : RoomDatabase() {
    abstract fun userDao(): UserDao
    abstract fun flightDao(): FlightDao
    abstract fun segmentDao(): SegmentDao
    abstract fun promoDao(): PromoDao
}

/**
 * Database for storing operations performed in the offline mode. 
 * During syncronization, it is used to see which operations are pending,
 * and after resolving them, the database is cleared
 */
@Database(entities = [UserOp::class, Booking::class], version = 1)
abstract class PendingOpDB : RoomDatabase() {
    abstract fun userOpDao(): UserOpDao
    abstract fun BookingDao(): BookingDao

}

/**
 * Operations to be synchronized
 */
@Entity
data class UserOp(
    @PrimaryKey val uuid: String,
    val operation: String,
    @Embedded
    val user: User
)

/** 
 * Stores only previously logged users, must validate credentials again
 * when doing synchronization
 */
@Entity(primaryKeys = ["username", "id"])
data class User(
    val type: Int = 0,
    val id: String, //uuid
    val username: String,
    val password: String,
    val firstName: String,
    val lastName: String,
    val phoneNumber: String,
    val email: String,
    val university: String,
    val studentId: String,
)

/**
 * Stores flight information
 */
@Entity(primaryKeys = ["id", "no"])
data class Flight(
    val id: String, //uuid
    val no: String,
    val comment: String?,
    val price: Double
)

/**
 * Stores a flight sub-route (segment) information
 */
@Entity
data class Segment(
    @PrimaryKey val id: String,
    val flight: String,
    val seq_no: Int,
    val from_loc: String, //uuid
    val from_time: String,
    val to_loc: String, //uuid
    val to_time: String,
    val aircraft: String,
)

/**
 * Stores reservation information
 */
@Entity(primaryKeys = ["flight", "pax"])
data class Booking(
    val flight: String, //uuid
    val pax: String, //uuid
    val promo: String
)

/**
 * Stores information of an available promo
 */
@Entity
data class Promo(
    @PrimaryKey val id: String,
    val code: String,
    val flight: String,
    val price: Double,
    val start_time: Double,
    val end_time: String,
    @ColumnInfo(typeAffinity = ColumnInfo.BLOB)
    val img: ByteArray?,
) {
    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (javaClass != other?.javaClass) return false

        other as Promo

        if (id != other.id) return false

        return true
    }

    override fun hashCode(): Int {
        return id.hashCode()
    }
}

/**
 * Interface to perform sql operations on databases that have 
 * [UserOp] as a entity
 */
@Dao
interface UserOpDao {
    @Insert
    fun insertAll(vararg data: UserOp)

    @Delete
    fun delete(userOp: UserOp)

    @Query("SELECT * FROM UserOp")
    fun getAll(): List<UserOp>
}

/**
 * Interface to perform sql operations on databases that have a 
 * [Booking] entity
 */
@Dao
interface BookingDao {
    @Insert
    fun insertAll(vararg data: Booking)

    @Query("SELECT * FROM Booking")
    fun getAll(): List<Booking>

    @Query("SELECT * FROM Booking WHERE pax=:pax AND flight=:flight")
    fun getBooking(pax: String, flight: String): Booking?
}


/**
 * Interface to perform sql operations on databases that have a 
 * [User] entity
 */
@Dao
interface UserDao {
    @Insert
    fun insertAll(vararg users: User)

    @Delete
    fun delete(user: User)

    @Update
    fun updateUser(user: User)

    @Query("DELETE FROM user where username = :username")
    fun deleteByUsername(username: String)

    @Query("SELECT * FROM user")
    fun getAll(): List<User>

    @Query(
        "SELECT username FROM user WHERE username = :username"
    )
    fun findUser(username: String): String?

    @Query(
        "SELECT * FROM user WHERE username = :username AND password = :password"
    )
    fun findCredentials(username: String, password: String): User?
}

/**
 * Interface to perform sql operations on databases that have a 
 * [Segment] entity
 */
@Dao
interface SegmentDao {
    @Insert
    fun insertAll(vararg segment: Segment)
    
    @Query("SELECT * FROM segment WHERE flight = :flightID ORDER BY seq_no DESC")
    fun getFlightSegments(flightID:String): List<Segment>

    @Query("DELETE FROM segment")
    fun clearTable()
}

/**
 * Interface to perform sql operations on databases that have a 
 * [Promo] entity
 */
@Dao
interface PromoDao {
    @Insert
    fun insertAll(vararg promo: Promo)

    @Query("SELECT id from promo where flight = :flightID")
    fun getForFlight(flightID: String): String?

    @Query("SELECT * from promo")
    fun getAll(): List<Promo>

    @Query("DELETE FROM promo")
    fun clearTable()
}

/**
 * Interface to perform sql operations on databases that have a 
 * [Flight] entity
 */
@Dao
interface FlightDao {
    @Insert
    fun insertAll(vararg flight: Flight)

    @Query("SELECT * FROM flight")
    fun getAll(): List<Flight>

    @Query("SELECT * FROM flight")
    fun getFromTo(): List<Flight>

    @Query("DELETE FROM flight")
    fun clearTable()
}
