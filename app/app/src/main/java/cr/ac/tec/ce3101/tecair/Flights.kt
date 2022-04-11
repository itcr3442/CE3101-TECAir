package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText

class Flights : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_flights)
    }
    fun applyFilter(view: View){
        val from = findViewById<EditText>(R.id.fromText).text
        val to = findViewById<EditText>(R.id.destinyText).text

    }
    fun openPaymentView(view: View){
        val intent = Intent(this, Promos::class.java)
        startActivity(intent)

    }
}