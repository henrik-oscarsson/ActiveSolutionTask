import {
    UseMutationResult,
    UseQueryResult,
    useQuery,
    useMutation,
    useQueryClient
} from '@tanstack/react-query'
import { Booking } from '../models/Booking'
import { CreateBooking } from '../models/CreateBooking'
import { Vehicle } from '../models/Vehicle'

const queryKeys = {
    all: () => ['bookings'] as const,
    getBookings: () =>
        [[...queryKeys.all(), 'getBookings']] as const,
    createBooking: () => [...queryKeys.all(), 'createBooking'] as const,
    getAvailableVehicles: () =>
        [[...queryKeys.all(), 'getAvailableVehicles']] as const,
    pickupBooking: (bookingId: number) => [...queryKeys.all(), 'pickupBooking', bookingId] as const,
    returnBooking: (bookingId: number) => [...queryKeys.all(), 'returnBooking', bookingId] as const,
}

const API_PATH = process.env.REACT_APP_API_PATH
const API_VERSION = process.env.REACT_APP_API_VERSION

export interface BookingsApi {
    getBookings: () => UseQueryResult<Array<Booking>>
    getAvailableVehicles: (pickUpDate: Date, returnDate: Date) => UseQueryResult<Array<Vehicle>>
    createBooking: (newBooking: CreateBooking) => UseMutationResult<Response, Error, CreateBooking, unknown>
    pickupBooking: (bookingId: number) => UseMutationResult<Response, Error, void, unknown>
    returnBooking: (bookingId: number, meterSetting: number) => UseMutationResult<Response, Error, void, unknown>
}

const fetchBookingsFn = async (): Promise<Array<Booking>> => {
    const response = await fetch(`${API_PATH}/${API_VERSION}/Bookings`)
    if (response.ok) {
        const data = await response.json()
        return data as Array<Booking>
    }
    return []
}

const fetchAvailableVehiclesFn = async (pickupDate: Date, returnDate: Date): Promise<Array<Vehicle>> => {
    const response = await fetch(`${API_PATH}/${API_VERSION}/Bookings/available?pickupDate=${pickupDate.toISOString()}&returnDate=${returnDate.toISOString()}`)
    if (response.ok) {
        const data = await response.json()
        return data as Array<Vehicle>
    }
    return []
}

const createBookingsFn = async (newBooking: CreateBooking): Promise<Response> => {
    const response = await fetch(`${API_PATH}/${API_VERSION}/Bookings`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(newBooking),
    })
    return response
}

const pickupBookingFn = async (bookingId: number): Promise<Response> => {
    const response = await fetch(`${API_PATH}/${API_VERSION}/Bookings/pickup/${bookingId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    })
    return response
}

const returnBookingFn = async (bookingId: number, meterSetting: number): Promise<Response> => {
    const response = await fetch(`${API_PATH}/${API_VERSION}/Bookings/return/${bookingId}/${meterSetting}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    })
    return response
}

const useBookingsApi = (): BookingsApi => {
    const queryClient = useQueryClient();

    const useGetBookings = () => {
        return useQuery<Array<Booking>>({
            queryKey: queryKeys.getBookings(),
            queryFn: () => fetchBookingsFn(),
            enabled: false,
            staleTime: 0,
        })
    }

    const useGetAvailableVehicles = (pickupDate: Date, returnDate: Date) => {
        return useQuery<Array<Vehicle>>({
            queryKey: queryKeys.getAvailableVehicles(),
            queryFn: () => fetchAvailableVehiclesFn(pickupDate, returnDate),
            enabled: false,
            staleTime: 0,
        })
    }

    const useCreateBooking = (newBooking: CreateBooking) => {
        return useMutation<Response, Error, CreateBooking, unknown>({
            mutationKey: queryKeys.createBooking(),
            mutationFn: (variables: CreateBooking) => createBookingsFn(variables),
            onSuccess: () => {
                // Invalidate and refetch
                queryClient.invalidateQueries({ queryKey: queryKeys.all() })
            },
        })
    }

    const usePickupBooking = (bookingId: number) => {
        return useMutation<Response, Error, void, unknown>({
            mutationKey: queryKeys.pickupBooking(bookingId),
            mutationFn: () => pickupBookingFn(bookingId),
            onSuccess: () => {
                // Invalidate and refetch
                queryClient.invalidateQueries({ queryKey: queryKeys.all() })
            },
        })
    }

    const useReturnBooking = (bookingId: number, meterSetting: number) => {
        return useMutation<Response, Error, void, unknown>({
            mutationKey: queryKeys.returnBooking(bookingId),
            mutationFn: () => returnBookingFn(bookingId, meterSetting),
            onSuccess: () => {
                // Invalidate and refetch
                queryClient.invalidateQueries({ queryKey: queryKeys.all() })
            },
        })
    }

    // No code needed here for the mutation generic type update.
    return {
        getBookings: useGetBookings,
        getAvailableVehicles: useGetAvailableVehicles,
        createBooking: useCreateBooking,
        pickupBooking: usePickupBooking,
        returnBooking: useReturnBooking,
    }
}

export default useBookingsApi
