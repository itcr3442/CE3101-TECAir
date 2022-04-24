export interface FlightById {
    flight: {
        id: string,
        no: number,
        comment: string,
        price: number,
        state: number,
    },
    route: { id: string, code: string, comment: string }[],
    segments: {
        id: string,
        fromTime: string,
        toTime: string,
        aircraftCode: string,
        seats: number,
        unavail: number[]
    }[]
}
