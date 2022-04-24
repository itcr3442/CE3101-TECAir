export interface FlightById {
    id: string,
    no: number,
    comment: string,
    price: number,
    state: number,
    route: { id: string, "code": string, "comment": string }[]
}
