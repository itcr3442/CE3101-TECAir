package cr.ac.tec.ce3101.tecair

import android.content.Context
import android.content.DialogInterface
import android.widget.EditText
import androidx.appcompat.app.AlertDialog

/**
 * Verify if a series of [EditText] widgets are empty or not. Returns
 * true if all fields have information
 */
fun verifyEmpty( err_str: String, vararg et: EditText,): Boolean {
    var isComplete = true
    et.forEach { x -> if (x.text.isNullOrEmpty()) run  {
            x.error = err_str
            isComplete = false
        }
    }
    return isComplete
}

/**
 * opens a simple dialog with the provided text
 * - [Context]: Application context in which the dialog should be displayed
 * - [message]: Message to be displayed in the dialog
 */
fun simpleDialog(context: Context, message: String){
    val builder = AlertDialog.Builder( context)
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
fun swapUserID(user:User, newId: String): User{
    return User(
        user.type,
        newId,
        user.username,
        user.password,
        user.firstName,
        user.lastName,
        user.phoneNumber,
        user.email,
        user.university,
        user.studentId

    )
}

/**
 * Simple data class to store a flight an its segments
 */
data class FlightWithPath(
    var flight: Flight,
    var path: List<Segment>
)
