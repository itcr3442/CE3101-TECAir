package cr.ac.tec.ce3101.tecair

import android.content.DialogInterface
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView
import androidx.appcompat.app.AlertDialog
import java.lang.Exception

/**
 * Activity that contains the list of currently active promotions
 * and allows an user to see the details for each promotion
 */
class PromosActivity : AppCompatActivity() {
    private lateinit var promoList: LinearLayout
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_promos)
        promoList = findViewById(R.id.promoList)
        refreshPromoList()
    }
    /**
     * Asks the session for all available promo information and 
     * creates a list with all the available promos and their information
     * inside a horizontal linear layout
     */
    private fun refreshPromoList() {
        promoList.removeAllViewsInLayout()

        (application as TECAirApp).session?.getPromoList() { promos ->
            run {
                promos.forEach { promo ->
                    val entry = LinearLayout(this).apply {
                        orientation = LinearLayout.HORIZONTAL
                        addView(Button(this.context).apply {
                            text = getString(R.string.info)
                            setOnClickListener { _ -> viewInfoPromo(promo) }
                            textSize = 16F
                        })
                        addView(TextView(this.context).apply {
                            text = promo.code
                            minWidth = 100
                        })
                        addView(TextView(this.context).apply {
                            text = "  $${promo.price.toString()}"
                            minWidth = 100
                        })
                        if (promo.img != null) {
                            try {
                                val inferred = promo.img.toByteArray();
                                val img = BitmapFactory.decodeByteArray(inferred, 0, inferred.size)
                                addView(ImageView(this.context).apply {
                                    setImageBitmap(
                                        Bitmap.createScaledBitmap(
                                            img,
                                            img.width,
                                            img.height,
                                            false
                                        )
                                    )
                                    adjustViewBounds = true
                                    maxWidth = 100
                                })
                            } catch (e: Exception){
                                addView(TextView(this.context).apply {
                                    text = " no-image   "
                                })
                            }

                        }
                    }
                    promoList.addView(entry)
                }
            }
        }

    }
    /**
     * Opens a dialog with the information of the provided promo.
     * Intented to be used in the onClick event of the info button
     * of each promotion
     */
    private fun viewInfoPromo(promo: Promo) {
        val builder = AlertDialog.Builder(this)
        var flightNo = (application as TECAirApp).session?.getFlightById(promo.flight)?.no
        if (flightNo == null)
            flightNo = 0
        val message = "${getString(R.string.flight_number)} : $flightNo \n" +
                "${getString(R.string.code)}  : ${promo.code} \n" +
                "${getString(R.string.price)} : ${promo.price} \n" +
                "${getString(R.string.start)}  : ${promo.startTime} \n" +
                "${getString(R.string.end)}  : ${promo.endTime} \n"
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
}
