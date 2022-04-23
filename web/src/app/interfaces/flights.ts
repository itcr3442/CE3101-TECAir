import { Time } from "@angular/common";

export interface Flights {
    flight_id: number;
    avion: number;
    aeropuerto_origen: string;
    aeropuerto_llegada: string;
    fecha: Date;
    hora_salida: Date;
    hora_llegada: Date;
    disponibilidad: Boolean;
}
