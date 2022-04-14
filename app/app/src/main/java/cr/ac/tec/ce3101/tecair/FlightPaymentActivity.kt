package cr.ac.tec.ce3101.tecair

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.google.gson.Gson

class FlightPaymentActivity : AppCompatActivity() {
    private lateinit var flight: Flight
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_flight_payment)
        flight  = Gson().fromJson(intent.getStringExtra("info"), Flight::class.java)
    }
}