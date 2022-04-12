package cr.ac.tec.ce3101.tecair

import android.content.Context
import android.content.DialogInterface
import android.content.Intent
import androidx.appcompat.app.AlertDialog
import androidx.core.content.ContextCompat.startActivity
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

class OnlineSession(
    url: String,
    private val username: String,
    private val  password: String,
    private val cx: Context
    ) : Session {
    private val service: TECAirService

    init{
        val retrofit = Retrofit.Builder().baseUrl(url).addConverterFactory(GsonConverterFactory.create()).build()
        service = retrofit.create(TECAirService::class.java)
    }

    override fun login(auth: (Boolean)-> Unit){

        service.checkLogin(username, password).enqueue(
            object: Cb<Unit>(){
                override fun onResponse(call: Call<Unit>, response: Response<Unit>) {
                    if (response.code() == 200) {
                        auth(true)
                    }else {
                        auth(false)
                    }
                }
            }
        )
    }
    override fun registerUser(user: User, afterOp: (Boolean)->Unit){
        TODO()
        (cx as TECAirApp).localDB.userDao().insertUser(user)
    }

    override fun getPromoList(): List<Promo> {
        TODO("Not yet implemented")
    }

    override fun getFlights(): List<String> {
        TODO("Not yet implemented")
    }

    override fun getFlightInfo(): Flight {
        TODO("Not yet implemented")
    }

    override fun makeReservation(reservation: Reservation, afterOp: (Boolean) -> Unit) {
        TODO("Not yet implemented")
    }

    /**
     * Callback wrapper class.
     * In case of request failure, the application must communicate
     */
    abstract inner class Cb<T> : Callback<T> {
        override fun onFailure(call: Call<T>, err: Throwable) {
            val builder = AlertDialog.Builder(cx)
            builder
                .setCancelable(false)
                .setMessage("Network error")
                .setPositiveButton(
                    "Ok",
                    DialogInterface.OnClickListener { dialog, which -> Unit }
                )
                .create()
            builder.create().show()
        }
    }

}