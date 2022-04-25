package cr.ac.tec.ce3101.tecair

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.TextView
import com.google.gson.Gson

/**
 * Activity in which the user can make a flight reservation request.  
 */
class FlightPaymentActivity : AppCompatActivity() {
    private lateinit var flightData: FlightWithPath
    private lateinit var flightID: TextView
    private lateinit var flightInfo: TextView
    private lateinit var cardNumber: TextView
    private lateinit var cve: TextView
    private lateinit var expiration: TextView
    private lateinit var promoCode: TextView
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_flight_payment)
        (application as TECAirApp).session?.changeContext(this)
        flightID = findViewById(R.id.flightID)
        flightInfo = findViewById(R.id.flightInfo)
        cardNumber = findViewById(R.id.cardNumberText)
        cve = findViewById(R.id.CVEText)
        expiration = findViewById(R.id.expirationText)
        promoCode = findViewById(R.id.promoCode)

        flightData = Gson().fromJson(intent.getStringExtra("info"), FlightWithPath::class.java)
        flightID.text = "${getString(R.string.flight_number)}: ${flightData.flight.no}"
        var route = "${getString(R.string.route)}:\n"
        flightData.path.forEach { segment ->
            run {
                route += "${segment.fromLoc} - ${segment.toLoc}  \n"
            }
        }
        val extra = "${getString(R.string.info)} ${flightData.flight.comment}\n" +
                "${getString(R.string.price)}: ${flightData.flight.price} \n" + route + getString(R.string.promo_disclaimer)
        flightInfo.text = extra
    }

    fun payAndBook(view: View) {
        //if there was a payment validation system, it would be here
        // ~ Imagine the code block
        // After payment, we process the booking
        (application as TECAirApp).session?.makeBooking(flightData.flight.id, promoCode.text.toString()) { success ->
            run {
                if (success) {
                    finish()
                } else {
                    simpleDialog(this, getString(R.string.booking_error))
                }
            }
        }

    }
}
