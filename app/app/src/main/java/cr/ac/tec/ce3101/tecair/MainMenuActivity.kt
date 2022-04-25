package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View

/**
 * Activity that contains the main menu of the application where
 * the user can navigate to the different functions offered by the
 * application
 */
class MainMenuActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main_menu)
        (application as TECAirApp).session?.changeContext(this)
    }
    /**
     * Opens the [UserListActivity] (For managing user that previously logged in)
     */
    fun openUserList(view: View){
        val intent = Intent(this, UserListActivity::class.java)
        startActivity(intent)
    }
    /**
     * Opens the [UserManagementActivity] which allows for adding new users
     */
    fun openUserManagement(view: View){
        val intent = Intent(this, RegisterUserActivity::class.java)
        startActivity(intent)
    }
    /**
     * Opens the [FlightMenuActivity] in which the user can search for flights based
     * on origin and destiny criteria
     */
    fun openFlightMenu(view: View){
        val intent = Intent(this, FlightsActivity::class.java)
        startActivity(intent)
    }
    /**
     * Opens the [PromoListActivity] in which the user can view information on
     * current active promos and the details about them
     */
    fun openPromoList(view: View){
        val intent = Intent(this, PromosActivity::class.java)
        startActivity(intent)
    }
}
