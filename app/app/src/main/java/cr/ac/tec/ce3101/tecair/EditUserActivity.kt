package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import android.widget.Switch
import android.widget.TextView
import com.google.gson.Gson
import java.util.*

/**
 * Activity used for editing user information. 
 * Expects the user's data to be packed in the intent
 */
class EditUserActivity : AppCompatActivity() {
    private lateinit var user: User
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
        setContentView(R.layout.activity_edit_user)
        //parse user information
        user = Gson().fromJson(intent.getStringExtra("info"), User::class.java)

        (application as TECAirApp).session?.changeContext(this)
        //obtain references to graphical objects
        username = findViewById<EditText>(R.id.editUsernameText)
        password = findViewById<EditText>(R.id.editPasswordText)
        firstname = findViewById<EditText>(R.id.editFirstNameText)
        lastname = findViewById<EditText>(R.id.editLastNameText)
        email = findViewById<EditText>(R.id.editEmailText)
        phoneNumber = findViewById<EditText>(R.id.editPhoneText)
        university = findViewById<EditText>(R.id.editUniversityText)
        studentID = findViewById<EditText>(R.id.editStudentIDText)
        errorTxt = findViewById<TextView>(R.id.editUserErrorText)

        //fill the fields with the current user data
        username.setText(user.username)
        password.setText(user.password)
        firstname.setText(user.firstName)
        lastname.setText(user.lastName)
        email.setText(user.email)
        phoneNumber.setText(user.phoneNumber)
        university.setText(user.university)
        studentID.setText(user.studentId)

    }
    /**
     * React to changes in the studentSwitch widget
     * - If ON, shows the EditTexts necessary for adding student data
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
     * Retrieves the new data values and tries to update the user's entry
     * In case the edit operation fails shows an error to the user
     */
    fun editUser(view: View) {
        var isCompleteInfo = verifyEmpty(getString(R.string.empty_error), username, password, firstname, lastname, email, phoneNumber)
        var universityStr: String = ""
        var studentIDStr: String  = ""
        if (isStudent) {
            isCompleteInfo = isCompleteInfo  && verifyEmpty(getString(R.string.empty_error),university, studentID)
            universityStr = university.text.toString()
            studentIDStr = studentID.text.toString()
        }
        if (!isCompleteInfo){
            errorTxt.visibility = View.VISIBLE
            return
        }
        val newUserInfo =
            User(
                0,
                user.id,
                username.text.toString(),
                password.text.toString(),
                firstname.text.toString(),
                lastname.text.toString(),
                email.text.toString(),
                phoneNumber.text.toString(),
                universityStr,
                studentIDStr
            )

        (application as TECAirApp).session?.editUser(newUserInfo) { success ->
            if (success) {
                val intent = Intent(this, MainActivity::class.java)
                startActivity(intent)
                finish()
            } else {
                errorTxt.visibility = View.VISIBLE
            }
        }

    }
}
