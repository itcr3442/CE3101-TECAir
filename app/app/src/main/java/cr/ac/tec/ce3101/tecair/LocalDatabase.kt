package cr.ac.tec.ce3101.tecair

import androidx.room.*
import java.util.*

/**
 * Room is a library that simplifies interactions with SQLite. The underlying data is still in SQLite,
 * but operations complexity is reduced. Room is recommended instead of direct interaction with
 * the SQLite APIs as noted in this article: https://developer.android.com/training/data-storage/room#kts
 */

@Database(entities = [User::class, Flight::class, Reservation::class, Promo::class],version=1)
abstract class LocalDB: RoomDatabase(){
	abstract fun userDao(): UserDao
}


@Entity
data class User(
    @PrimaryKey val username: String,
    val password: String,
    val firstname: String,
    val lastname: String,
    val email: String,
    val phoneNumber: String,
    val university: String?,
    val studentId: String?,
    )

@Entity
data class Flight(
    @PrimaryKey val number: String,
    val from: String,
    val stops: List<String>
)

@Entity(primaryKeys=["username", "number"])
data class Reservation(
    @PrimaryKey val paymentInfo: String,
    val username: String,
    val number: String,
)

@Entity
data class Promo(
    @PrimaryKey val code: String,
    val from: String,
    val to: String,
    val price: Double,
    val start: Date,
    val end: Date,
)

@Dao
interface UserDao{
    @Insert
    fun insertUser(user: User)

    @Query("SELECT username, firstname, lastname FROM user")
    fun getAll(): List<User>

    @Query(
        "SELECT username, password FROM user WHERE username = :username AND password = :password")
    fun findUser(username: String, password: String): List<User>
}
