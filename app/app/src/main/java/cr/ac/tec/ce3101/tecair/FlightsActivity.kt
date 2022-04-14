package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.EditText
import android.widget.LinearLayout
import com.google.gson.Gson

class FlightsActivity : AppCompatActivity() {
    private lateinit var flightList: LinearLayout
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_flights)
        flightList = findViewById(R.id.flightList)
    }
    fun applyFilter(view: View){
        val flights = (application as TECAirApp).session?.getFlights()
        val from = findViewById<EditText>(R.id.fromText).text
        val to = findViewById<EditText>(R.id.destinyText).text
        TODO("Falta aplicar la lógica de filtro")

    }
    fun openPaymentView(flight: Flight){
        val intent = Intent(this, FlightPaymentActivity::class.java).apply {
            putExtra("info", Gson().toJson(flight))
        }
        startActivity(intent)

    }
}