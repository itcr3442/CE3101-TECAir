package cr.ac.tec.ce3101.tecair

import androidx.room.*
import java.util.*

/**
 * Room is a library that simplifies interactions with SQLite. The underlying data is still in SQLite,
 * but operations complexity is reduced. Room is recommended instead of direct interaction with
 * the SQLite APIs as noted in this article: https://developer.android.com/training/data-storage/room#kts
 */

@Database(entities = [User::class, Flight::class, Reservation::class, Promo::class], version = 1)
@TypeConverters(StopConverter::class)
abstract class LocalDB : RoomDatabase() {
    abstract fun userDao(): UserDao
    abstract fun promoDao(): PromoDao
    abstract fun flightDao(): FlightDao
}


@Entity(primaryKeys = ["username", "uuid"])
data class User(
    val uuid: String,
    val username: String,
    val password: String,
    val firstname: String,
    val lastname: String,
    val email: String,
    val phoneNumber: String,
    val university: String?,
    val studentId: String?,
)

@Entity(primaryKeys = ["number", "uuid"])
data class Flight(
    val uuid: String,
    val number: String,
    val from: String,
    @TypeConverters(StopConverter::class)
    val stops: List<String>
)

class StopConverter {
    @TypeConverter
    fun toStops(serStops: String): List<String> {
        return serStops.split(',')
    }

    @TypeConverter
    fun fromStops(stops: List<String>): String {
        return stops.joinToString()
    }
}

@Entity(primaryKeys = ["username", "flightNumber", "uuid"])
data class Reservation(
    val uuid: String,
    val paymentInfo: String,
    val username: String,
    val flightNumber: String,
)

@Entity
data class Promo(
    @PrimaryKey val uuid: String,
    val extra_info: String?,
    val from: String,
    val to: String,
    val price: Double,
    val start: String,
    val end: String,
)

@Dao
interface UserDao {
    @Insert
    fun insertAll(vararg users: User)

    @Delete
    fun delete(user: User)

    @Query("SELECT * FROM user")
    fun getAll(): List<User>

    @Query(
        "SELECT username FROM user WHERE username = :username"
    )
    fun findUser(username: String): List<String>

    @Query(
        "SELECT * FROM user WHERE username = :username AND password = :password"
    )
    fun findCredentials(username: String, password: String): List<User>
}

@Dao
interface PromoDao {
    @Query("SELECT * from promo")
    fun getAll(): List<Promo>
}
@Dao
interface FlightDao{
    @Query("SELECT * FROM flight")
    fun getAll(): List<Flight>
}