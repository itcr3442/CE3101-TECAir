package cr.ac.tec.ce3101.tecair

import android.content.Context
import android.content.DialogInterface
import android.widget.EditText
import androidx.appcompat.app.AlertDialog

fun verifyEmpty( err_str: String, vararg et: EditText,): Boolean {
    var isComplete = true
    et.forEach { x -> if (x.text.isNullOrEmpty()) run  {
            x.error = err_str
            isComplete = false
        }
    }
    return isComplete
}

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

data class FlightWithPath(
    var flight: Flight,
    var path: List<Segment>
)