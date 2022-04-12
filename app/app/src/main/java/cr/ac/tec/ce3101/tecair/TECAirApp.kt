package cr.ac.tec.ce3101.tecair

import android.app.Application
import androidx.room.Room

class TECAirApp: Application() {
    var session: Session? = null
    val localDB: LocalDB =
        Room.databaseBuilder(applicationContext, LocalDB::class.java, "local-data").build()
    fun syncDB(){
        TODO("Must ask for all the data")
        localDB.clearAllTables()
    }
}