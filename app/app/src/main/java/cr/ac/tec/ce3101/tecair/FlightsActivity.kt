package cr.ac.tec.ce3101.tecair

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.LinearLayout
import android.widget.TextView
import com.google.gson.Gson

class FlightsActivity : AppCompatActivity() {
    private lateinit var flightList: LinearLayout
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_flights)
        flightList = findViewById(R.id.flightList)
    }

    fun applyFilter(view: View) {
        flightList.removeAllViewsInLayout()
        val from = findViewById<EditText>(R.id.fromText).text.toString()
        val to = findViewById<EditText>(R.id.destinyText).text.toString()
        (application as TECAirApp).session?.getFlights(from, to) { flights ->
            run {
                flights.forEach { flightInfo ->
                    val entry = LinearLayout(this).apply {
                        orientation = LinearLayout.HORIZONTAL
                        addView(TextView(this.context).apply {
                            text = flightInfo.flight.no
                            width = 300
                            textSize = 18F
                        })
                        addView(Button(this.context).apply {
                            text = getString(R.string.info)
                            setOnClickListener { _ -> viewInfoFlight(flightInfo) }
                            textSize = 16F
                        })
                        addView(Button(this.context).apply {
                            text = getString(R.string.payment_button)
                            setOnClickListener { _ -> openPaymentView(flightInfo) }
                            textSize = 16F
                        })
                    }
                    flightList.addView(entry)
                }
            }
        }
    }

    private fun viewInfoFlight(flightInfo: FlightWithPath) {
        val message = "${getString(R.string.flight_number)}: ${flightInfo.flight.no} \n" +
                "${getString(R.string.price)}: ${flightInfo.flight.price} \n"
        simpleDialog(this, message)
    }

    private fun openPaymentView(flightInfo: FlightWithPath) {
        val intent = Intent(this, FlightPaymentActivity::class.java).apply {
            putExtra("info", Gson().toJson(flightInfo))
        }
        startActivity(intent)

    }
}