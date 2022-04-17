package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.LinearLayout
import android.widget.TextView
import com.google.gson.Gson

class UserListActivity : AppCompatActivity() {
    private lateinit var userList: LinearLayout
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_user_list)
        userList = findViewById(R.id.userList)
        refreshUserList()
    }

    private fun refreshUserList() {
        userList.removeAllViewsInLayout()
        val users = (application as TECAirApp).session?.getUserList()
        users?.forEach { user ->
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

    private fun viewInfoUser(user: User) {
        var message = "${getString(R.string.login_hint)} : ${user.username} \n" +
                "${getString(R.string.firstname_hint)} : ${user.first_name} \n" +
                "${getString(R.string.lastname_hint)} : ${user.last_name} \n" +
                "${getString(R.string.phone_hint)} : ${user.phonenumber} \n" +
                "${getString(R.string.email_hint)} : ${user.email} \n"
        if (user.university != null) {
            message += "${getString(R.string.university_hint)} : ${user.university} \n" +
                    "${getString(R.string.student_id_hint)} : ${user.student_id} \n"

        }
        simpleDialog(this, message)
    }

    private fun editUser(user: User) {
        val session = (application as TECAirApp).session
        //will only allow editing the current user
        if  (session == null || session.getUsername() == user.username || session.getPassword() == user.password){
            simpleDialog(this, getString(R.string.edit_non_current_user_error))
        }else {
            val intent = Intent(this, EditUserActivity::class.java).apply {
                putExtra("info", Gson().toJson(user))
            }
        }
    }

    private fun deleteUser(user: User) {
        val session = (application as TECAirApp).session
        //will only allow editing the current user
        if  (session == null || session.getUsername() == user.username || session.getPassword() == user.password){
            simpleDialog(this, getString(R.string.delete_non_current_user_error))
        }else {
            (application as TECAirApp).session?.deleteUser(user) { success ->
                run {
                    if (success) {
                        val intent = Intent(this, MainActivity::class.java)
                        (application as TECAirApp).session = null
                        startActivity(intent)
                    } else {
                        simpleDialog(this, getString(R.string.delete_user_error))
                    }
                }

            }
        }
    }
}