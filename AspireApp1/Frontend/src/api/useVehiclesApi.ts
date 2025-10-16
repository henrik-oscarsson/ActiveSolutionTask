import {
    UseQueryResult,
    useQuery,
} from '@tanstack/react-query'
import { Vehicle } from '../models/Vehicle'

const queryKeys = {
    all: () => ['vehicles'] as const,
    getVehicles: () =>
        [[...queryKeys.all(), 'getVehicles']] as const,
}

const API_PATH = process.env.REACT_APP_API_PATH
const API_VERSION = process.env.REACT_APP_API_VERSION

export interface VehiclesApi {
    getVehicles: () => UseQueryResult<Array<Vehicle>>
}

const fetchVehiclesFn = async (): Promise<Array<Vehicle>> => {
    const response = await fetch(`${API_PATH}/${API_VERSION}/Vehicles`, {})
    if (response.ok) {
        const data = await response.json()
        return data as Array<Vehicle>
    }
    return []
}
const useVehiclesApi = (): VehiclesApi => {
    const useGetVehicles = () => {
        return useQuery<Array<Vehicle>>({
            queryKey: queryKeys.getVehicles(),
            queryFn: () => fetchVehiclesFn(),
            enabled: false,
        })
    }

    return {
        getVehicles : useGetVehicles,
    }
}

export default useVehiclesApi
