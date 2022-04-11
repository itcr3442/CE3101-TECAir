package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import android.widget.Switch

class UserManagement : AppCompatActivity() {
    private var isStudent : Boolean = false
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_user_management)
    }
    fun studentCheck(view: View){
        isStudent = findViewById<Switch>(R.id.studentSwitch).isChecked
        if (isStudent){
            findViewById<EditText>(R.id.newUniversityText).visibility = View.VISIBLE
            findViewById<EditText>(R.id.newStudentIDText).visibility = View.VISIBLE
        }else{
            findViewById<EditText>(R.id.newUniversityText).visibility = View.INVISIBLE
            findViewById<EditText>(R.id.newStudentIDText).visibility = View.INVISIBLE
        }
    }
    fun registerUser(view:View){
        val username = findViewById<EditText>(R.id.newUsernameText).text
        val password = findViewById<EditText>(R.id.newPasswordText).text
        val firstname = findViewById<EditText>(R.id.newFirstNameText).text
        val lastname = findViewById<EditText>(R.id.newLastNameText).text
        val email  = findViewById<EditText>(R.id.newEmailText).text
        val phoneNumber = findViewById<EditText>(R.id.newPhoneText).text
        if (isStudent){
            val university = findViewById<EditText>(R.id.newUniversityText).text
            val studentId = findViewById<EditText>(R.id.newStudentIDText).text
        }

        val intent = Intent(this, Menu::class.java)
        startActivity(intent)
    }
}