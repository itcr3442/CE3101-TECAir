package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View

class MainMenuActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main_menu)
    }
    fun openUserList(view: View){
        val intent = Intent(this, UserListActivity::class.java)
        startActivity(intent)
    }
    fun openUserManagement(view: View){
        val intent = Intent(this, RegisterUserActivity::class.java)
        startActivity(intent)
    }
    fun openFlightMenu(view: View){
        val intent = Intent(this, FlightsActivity::class.java)
        startActivity(intent)
    }
    fun openPromoList(view: View){
        val intent = Intent(this, PromosActivity::class.java)
        startActivity(intent)
    }
}