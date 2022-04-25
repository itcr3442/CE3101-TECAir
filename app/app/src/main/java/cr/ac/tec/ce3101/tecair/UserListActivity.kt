package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.LinearLayout
import android.widget.TextView
import com.google.gson.Gson

/**
 * This activity allows to view previosly logged user information.
 * Be aware, the different action buttons provided will only work for the
 * current user, as it is the one who is authenticated. No user should be able
 * to edit other's information
 */
class UserListActivity : AppCompatActivity() {
    private lateinit var userList: LinearLayout
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_user_list)
        (application as TECAirApp).session?.changeContext(this)
        userList = findViewById(R.id.userList)
        refreshUserList()
    }

    /**
     * Retrieves the user list for the current session and fills
     * a Horizontal LinearLayout with the username and buttons to
     * view the information, edit, or delete an user
     */
    private fun refreshUserList() {
        userList.removeAllViewsInLayout()
        (application as TECAirApp).session?.getUserList { user ->
            var entry = LinearLayout(this).apply {
                orientation = LinearLayout.HORIZONTAL
                addView(TextView(this.context).apply {
                    text = user.username
                    width = 300
                    textSize = 18F
                })
                addView(Button(this.context).apply {
                    text = getString(R.string.info)
                    setOnClickListener { _ -> viewInfoUser(user) }
                    textSize = 16F
                })
                addView(Button(this.context).apply {
                    text = getString(R.string.edit_user_button)
                    setOnClickListener { _ -> editUser(user) }
                    textSize = 16F
                })
                addView(Button(this.context).apply {
                    text = getString(R.string.delete_user_button)
                    setOnClickListener { _ -> deleteUser(user) }
                    textSize = 16F
                })
            }
            userList.addView(entry)
        }

    }
    
    /**
     * Displays a dialog message with the information of the provided user
     */
    private fun viewInfoUser(user: User) {
        var message = "${getString(R.string.login_hint)} : ${user.username} \n" +
                "${getString(R.string.firstname_hint)} : ${user.firstName} \n" +
                "${getString(R.string.lastname_hint)} : ${user.lastName} \n" +
                "${getString(R.string.phone_hint)} : ${user.phoneNumber} \n" +
                "${getString(R.string.email_hint)} : ${user.email} \n"
        if (user.university != null) {
            message += "${getString(R.string.university_hint)} : ${user.university} \n" +
                    "${getString(R.string.student_id_hint)} : ${user.studentId} \n"

        }
        simpleDialog(this, message)
    }
    /**
     * Opens [EditUserActivity] with the selected user's information
     */
    private fun editUser(user: User) {
        val session = (application as TECAirApp).session
        //will only allow editing the current user
        if  (session == null || session.getUsername() != user.username || session.getPassword() != user.password){
            simpleDialog(this, getString(R.string.edit_non_current_user_error))
        }else {
            val intent = Intent(this, EditUserActivity::class.java).apply {
                putExtra("info", Gson().toJson(user))
            }
            startActivity(intent)
        }
    }
    
    /**
     * Delete the provided user. As only the current user can delete itself, it will
     * automatically destroy the current session.
     */
    private fun deleteUser(user: User) {
        val session = (application as TECAirApp).session
        //will only allow editing the current user
        if  (session == null || session.getUsername() != user.username || session.getPassword() != user.password){
            simpleDialog(this, getString(R.string.delete_non_current_user_error))
        }else {
            (application as TECAirApp).session?.deleteUser(user) { success ->
                run {
                    if (success) {
                        val intent = Intent(this, MainActivity::class.java)
                        startActivity(intent)
                        finish()
                    } else {
                        simpleDialog(this, getString(R.string.delete_user_error))
                    }
                }

            }
        }
    }
}
