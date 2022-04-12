package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import android.widget.Switch

class UserManagement : AppCompatActivity() {
    private var isStudent: Boolean = false
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_user_management)
    }

    fun studentCheck(view: View) {
        isStudent = findViewById<Switch>(R.id.studentSwitch).isChecked
        if (isStudent) {
            findViewById<EditText>(R.id.newUniversityText).visibility = View.VISIBLE
            findViewById<EditText>(R.id.newStudentIDText).visibility = View.VISIBLE
        } else {
            findViewById<EditText>(R.id.newUniversityText).visibility = View.INVISIBLE
            findViewById<EditText>(R.id.newStudentIDText).visibility = View.INVISIBLE
        }
    }

    fun registerUser(view: View) {
        val username = findViewById<EditText>(R.id.newUsernameText).text.toString()
        val password = findViewById<EditText>(R.id.newPasswordText).text.toString()
        val firstname = findViewById<EditText>(R.id.newFirstNameText).text.toString()
        val lastname = findViewById<EditText>(R.id.newLastNameText).text.toString()
        val email = findViewById<EditText>(R.id.newEmailText).text.toString()
        val phoneNumber = findViewById<EditText>(R.id.newPhoneText).text.toString()
        var university: String? = null
        var studentId: String? = null
        if (isStudent) {
            university = findViewById<EditText>(R.id.newUniversityText).text.toString()
            studentId = findViewById<EditText>(R.id.newStudentIDText).text.toString()
        }
        val newUser =
            User(username, password, firstname, lastname, email, phoneNumber, university, studentId)

        (application as TECAirApp).session?.registerUser(newUser) { success ->
            if (success) {
                val intent = Intent(this, Menu::class.java)
                startActivity(intent)
            } else {
                TODO("Must communicate that the user couldn't be added")
            }
        }

    }
}