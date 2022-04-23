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
                        if (promo.img != null) {
                            val img = BitmapFactory.decodeByteArray(promo.img, 0, promo.img.size)
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

                        }
                        addView(TextView(this.context).apply {
                            text = promo.code
                        })
                        addView(TextView(this.context).apply {
                            text = "$ ${promo.price.toString()}"
                        })
                        addView(Button(this.context).apply {
                            text = getString(R.string.info)
                            setOnClickListener { _ -> viewInfoPromo(promo) }
                            textSize = 16F
                        })
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
        val message = "${getString(R.string.flight_number)} : ${promo.flight} \n" +
                "${getString(R.string.code)}  : ${promo.code} \n" +
                "${getString(R.string.price)} : ${promo.price} \n" +
                "${getString(R.string.start)}  : ${promo.start_time} \n" +
                "${getString(R.string.end)}  : ${promo.end_time} \n"
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
