export interface Vehicle {
    id: number,
    registrationNumber: string,
    meterSetting: number,
    category: VehicleCategory,
}

export enum VehicleCategory {
    Sedan,
    StationWagon,
    Truck
}