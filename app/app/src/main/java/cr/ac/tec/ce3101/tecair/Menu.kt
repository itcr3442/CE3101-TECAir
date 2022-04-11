package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View

class Menu : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_menu)
    }
    fun openUserManagement(view: View){
        val intent = Intent(this, UserManagement::class.java)
        startActivity(intent)
    }
    fun openFlightMenu(view: View){
        val intent = Intent(this, Flights::class.java)
        startActivity(intent)
    }
    fun openPromoList(view: View){
        val intent = Intent(this, Promos::class.java)
        startActivity(intent)
    }
}