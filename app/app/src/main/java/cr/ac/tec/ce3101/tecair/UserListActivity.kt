package cr.ac.tec.ce3101.tecair

import android.content.DialogInterface
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.LinearLayout
import android.widget.TextView
import androidx.appcompat.app.AlertDialog
import androidx.core.view.children

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
                    text = getString(R.string.view_info_user_button)
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
        val builder = AlertDialog.Builder(this)
        var message = "${getString(R.string.login_hint)} : ${user.username} \n" +
                "${getString(R.string.firstname_hint)} : ${user.firstname} \n" +
                "${getString(R.string.lastname_hint)} : ${user.lastname} \n" +
                "${getString(R.string.phone_hint)} : ${user.phoneNumber} \n" +
                "${getString(R.string.email_hint)} : ${user.email} \n"
        if (user.university != null) {
            message += "${getString(R.string.university_hint)} : ${user.university} \n" +
                    "${getString(R.string.student_id_hint)} : ${user.studentId} \n"

        }
        builder
            .setCancelable(false)
            .setMessage(message)
            .setPositiveButton(
                "Ok",
                DialogInterface.OnClickListener { _, _ -> Unit }
            )
            .create()
        builder.create().show()

    }

    private fun editUser(user: User) {
        TODO("Todavía no está el view para editar el usuario y se debe agregar el update al UserDao")
    }

    private fun deleteUser(user: User) {
        (application as TECAirApp).session?.deleteUser(user) { success ->
            run {
                if (success) {
                    refreshUserList()
                } else {
                    val builder = AlertDialog.Builder(this)
                    builder
                        .setCancelable(false)
                        .setMessage(getString(R.string.delete_user_error))
                        .setPositiveButton(
                            "Ok",
                            DialogInterface.OnClickListener { _, _ -> Unit }
                        )
                        .create()
                    builder.create().show()
                }
            }

        }
    }
}