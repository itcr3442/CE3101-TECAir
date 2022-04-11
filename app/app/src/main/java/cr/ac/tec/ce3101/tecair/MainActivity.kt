package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import android.widget.Switch
import android.widget.TextView

class MainActivity : AppCompatActivity() {
    private var isOffline: Boolean = false
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
    }
    fun login(view: View){
        val intent = Intent(this, Menu::class.java)
        startActivity(intent)
    }
    fun showServerSettings(view: View){
        findViewById<EditText>(R.id.serverSettingsText).visibility = View.VISIBLE
    }
    fun offlineCheck(view: View){
        isOffline = findViewById<Switch>(R.id.offlineSwitch).isChecked
    }
}