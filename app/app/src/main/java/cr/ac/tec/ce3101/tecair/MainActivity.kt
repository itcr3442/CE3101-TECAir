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

    fun login(view: View) {
        val username = findViewById<EditText>(R.id.loginUsernameText).text.toString()
        val password = findViewById<EditText>(R.id.loginPasswordText).text.toString()
        val session: Session = if (findViewById<Switch>(R.id.offlineSwitch).isChecked) {
            OfflineSession(
                username, password, this
            )
        } else {
            OnlineSession(
                findViewById<EditText>(R.id.serverSettingsText).text.toString(),
                username, password,
                this
            )
        }
        session.login { success ->
            run {
                if (success) {
                    (application as TECAirApp).session = session
                    val intent = Intent(this, Menu::class.java)
                    startActivity(intent)
                } else {
                    findViewById<TextView>(R.id.credentialsErrorText).visibility = View.VISIBLE
                }
            }
        }


    }

    fun showServerSettings(view: View) {
        val serverSettings = findViewById<EditText>(R.id.serverSettingsText)
        if (serverSettings.visibility == View.VISIBLE) {
            serverSettings.visibility = View.INVISIBLE
        } else {
            serverSettings.visibility = View.VISIBLE
        }
    }

    fun offlineCheck(view: View) {
        isOffline = findViewById<Switch>(R.id.offlineSwitch).isChecked
    }
}