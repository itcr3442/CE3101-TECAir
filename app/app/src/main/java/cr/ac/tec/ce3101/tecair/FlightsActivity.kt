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

/**
 * Activity where the user can search for available flights
 * based on origin and destiny criteria
 */
class FlightsActivity : AppCompatActivity() {
    private lateinit var flightList: LinearLayout
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_flights)
        flightList = findViewById(R.id.flightList)
    }
    
    /**
     * Retrieves the list of flights that meet the search 
     * criteria and loads them to a LinearLayout in the activity
     */
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
                            text = "${getString(R.string.flight_number)}: ${flightInfo.flight.no}"
                            minWidth = 300
                            textSize = 16F
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
    /**
     * Opens a dialog with the information of the provided flight.
     * Intended to be called from the "info" button of each flight
     */
    private fun viewInfoFlight(flightInfo: FlightWithPath) {
        var route = "${getString(R.string.route)}:\n"
        flightInfo.path.forEach { segment ->
            run {
                route += "${segment.fromLoc} - ${segment.toLoc}  \n"
            }
        }
        val message = "${getString(R.string.flight_number)}: ${flightInfo.flight.no} \n" +
                "${getString(R.string.price)}: ${flightInfo.flight.price} \n"+
                "$route \n"

        simpleDialog(this, message)
    }
    /**
     * Opens the FlightPaymentActivity so the user can proceed with the 
     * reservation of a flight
     */
    private fun openPaymentView(flightInfo: FlightWithPath) {
        val intent = Intent(this, FlightPaymentActivity::class.java).apply {
            putExtra("info", Gson().toJson(flightInfo))
        }
        startActivity(intent)
    }
}
