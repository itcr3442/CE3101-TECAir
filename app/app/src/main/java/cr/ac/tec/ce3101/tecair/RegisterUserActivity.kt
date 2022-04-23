package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import android.widget.Switch
import android.widget.TextView
import androidx.room.util.UUIDUtil
import java.util.*

/**
 * Activity used for providing information to create
 * a new user
 */
class RegisterUserActivity : AppCompatActivity() {
    private var isStudent: Boolean = false
    private lateinit var username: EditText
    private lateinit var password: EditText
    private lateinit var firstname: EditText
    private lateinit var lastname: EditText
    private lateinit var email: EditText
    private lateinit var phoneNumber: EditText
    private lateinit var university: EditText
    private lateinit var studentID: EditText
    private lateinit var errorTxt: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_register_user)
        //obtain references to graphical objects
        username = findViewById<EditText>(R.id.newUsernameText)
        password = findViewById<EditText>(R.id.newPasswordText)
        firstname = findViewById<EditText>(R.id.newFirstNameText)
        lastname = findViewById<EditText>(R.id.newLastNameText)
        email = findViewById<EditText>(R.id.newEmailText)
        phoneNumber = findViewById<EditText>(R.id.newPhoneText)
        university = findViewById<EditText>(R.id.newUniversityText)
        studentID = findViewById<EditText>(R.id.newStudentIDText)
        errorTxt = findViewById<TextView>(R.id.newUserErrorText)
    }

    /**
     * React to changes in the studentSwitch widget
     * - If ON, shows the EditTexts neccesary for adding student data
     */
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

    /**
     * Retrieve the new user data from the different textfields and try to create the new user.
     * In case of failure, will make an error text visible
     */
    fun registerUser(view: View) {
        var isCompleteInfo = verifyEmpty(getString(R.string.empty_error), username, password, firstname, lastname, email, phoneNumber)
        var universityStr: String? = null
        var studentIDStr: String? = null
        if (isStudent) {
            isCompleteInfo = isCompleteInfo  && verifyEmpty(getString(R.string.empty_error),university, studentID)
            universityStr = university.text.toString()
            studentIDStr = studentID.text.toString()
        }
        if (!isCompleteInfo){
            errorTxt.visibility = View.VISIBLE
            return
        }
        val newUser =
            User(
                UUID.randomUUID().toString(),
                username.text.toString(),
                password.text.toString(),
                firstname.text.toString(),
                lastname.text.toString(),
                email.text.toString(),
                phoneNumber.text.toString(),
                universityStr,
                studentIDStr
            )

        (application as TECAirApp).session?.registerUser(newUser) { success ->
            if (success) {
                val intent = Intent(this, MainMenuActivity::class.java)
                startActivity(intent)
            } else {
                errorTxt.visibility = View.VISIBLE
            }
        }

    }
}
