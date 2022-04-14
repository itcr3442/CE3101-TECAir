package cr.ac.tec.ce3101.tecair

import android.content.DialogInterface
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Button
import android.widget.LinearLayout
import android.widget.TextView
import androidx.appcompat.app.AlertDialog

class PromosActivity : AppCompatActivity() {
    private lateinit var promoList: LinearLayout
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_promos)
        promoList = findViewById(R.id.promoList)
        refreshPromoList()
    }

    private fun refreshPromoList() {
        promoList.removeAllViewsInLayout()
        val promos = (application as TECAirApp).session?.getPromoList()
        promos?.forEach { promo ->
            var entry = LinearLayout(this).apply {
                orientation = LinearLayout.HORIZONTAL
                addView(TextView(this.context).apply {
                    text = promo.uuid
                })
                addView(TextView(this.context).apply {
                    text = promo.price.toString()
                })
                addView(Button(this.context).apply {
                    text = getString(R.string.view_info_user_button)
                    setOnClickListener { _ -> viewInfoPromo(promo) }
                    textSize = 16F
                })
            }
            promoList.addView(entry)
        }

    }

    private fun viewInfoPromo(promo: Promo) {
        val builder = AlertDialog.Builder(this)
        var message = "${getString(R.string.flight_from_hint)} : ${promo.from} \n" +
                "${getString(R.string.flight_to_hint)} : ${promo.to} \n"
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