package cr.ac.tec.ce3101.tecair

import android.app.Application
import androidx.room.Room

/**
 * Globally accesible singleton that contains the application state
 * which is contained in a session
 */
class TECAirApp: Application() {
    var session: Session? = null
}
