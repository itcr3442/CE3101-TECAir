export interface Flight {
    flight:{
        id: string,
        no: number,
        comment: string,
        price: number,
        state: number,
        fullRoute: string, 
    },
    route: { id: string, "code": string, "comment": string }[]
}
