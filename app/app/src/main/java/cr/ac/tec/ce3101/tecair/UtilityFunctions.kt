package cr.ac.tec.ce3101.tecair

import android.widget.EditText

fun verifyEmpty( err_str: String, vararg et: EditText,): Boolean {
    var isComplete = true
    et.forEach { x -> if (x.text.isNullOrEmpty()) run  {
            x.error = err_str
            isComplete = false
        }
    }
    return isComplete
}