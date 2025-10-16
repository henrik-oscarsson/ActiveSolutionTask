import React, { useEffect, useState } from 'react'
import useCustomersApi from '../../api/useCustomersApi'
import useBookingsApi from '../../api/useBookingsApi'
import { CreateBooking as CreateBookingPayload } from '../../models/CreateBooking'
import { FormControl, InputLabel, Select, MenuItem, TextField, Button, FormHelperText } from '@mui/material'

interface CreateBookingProps {
    onCreated?: () => void
}

const CreateBooking: React.FC<CreateBookingProps> = ({ onCreated }) => {
    const bookingsApi = useBookingsApi();

    const [vehicleId, setVehicleId] = useState<number | ''>('')
    const [customerId, setCustomerId] = useState<number | ''>('')
    const [startDateTime, setStartDateTime] = useState<string>('')
    const [endDateTime, setEndDateTime] = useState<string>('')
    const [submitting, setSubmitting] = useState(false)
    const [error, setError] = useState<string | null>(null)

    const { data: vehicles = [], refetch: getAvailableVehicles } = bookingsApi.getAvailableVehicles(new Date(startDateTime), new Date(endDateTime))
    const customersApi = useCustomersApi()
    const { data: customers = [], refetch: getCustomers } = customersApi.getCustomers()

    const createBookingPayload = (): CreateBookingPayload => {
        return {
            vehicleId: vehicleId as number,
            customerId: customerId as number,
            pickUpDate: new Date(startDateTime),
            returnDate: new Date(endDateTime),
        }
    }

    const { mutate: createBooking } = bookingsApi.createBooking(createBookingPayload())

    useEffect(() => {
            const fetchVehicles = async () => {
                await getAvailableVehicles();
            }
            const fetchCustomers = async () => {
                await getCustomers();
            }
            fetchVehicles();
            fetchCustomers();
    }, [getAvailableVehicles, getCustomers, startDateTime, endDateTime]);
    
    const validate = (): string | null => {
        if (vehicleId === '') return 'Please select a vehicle.'
        if (customerId === '') return 'Please select a customer.'
        if (!startDateTime) return 'Please select a start date and time.'
        if (!endDateTime) return 'Please select an end date and time.'
        if (new Date(startDateTime) >= new Date(endDateTime)) return 'End must be after start.'
        return null
    }

    const clearForm = () => {
        setVehicleId('')
        setCustomerId('')
        setStartDateTime('')
        setEndDateTime('')
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setError(null)
        const validationError = validate()
        if (validationError) {
            setError(validationError)
            return
        }

        setSubmitting(true)
        try {
            createBooking(createBookingPayload())
            clearForm()
            onCreated && onCreated()
        } catch (err: any) {
            setError(err.message || 'Failed to create booking')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <form onSubmit={handleSubmit} style={{ maxWidth: 600 }}>
            <TextField
                fullWidth
                margin="normal"
                id="start-datetime"
                label="Start date & time"
                type="datetime-local"
                InputLabelProps={{ shrink: true }}
                value={startDateTime}
                onChange={e => setStartDateTime(e.target.value)}
            />
            <TextField
                fullWidth
                margin="normal"
                id="end-datetime"
                label="End date & time"
                type="datetime-local"
                InputLabelProps={{ shrink: true }}
                value={endDateTime}
                onChange={e => setEndDateTime(e.target.value)}
            />
            <FormControl fullWidth margin="normal">
                <InputLabel id="vehicle-label">Vehicle</InputLabel>
                <Select
                    labelId="vehicle-label"
                    id="vehicle-select"
                    value={vehicleId}
                    label="Vehicle"
                    onChange={e => {
                        const val = e.target.value as unknown as string
                        setVehicleId(val === '' ? '' : Number(val))
                    }}
                >
                    <MenuItem value=""><em>None</em></MenuItem>
                    {vehicles?.map(v => (
                        <MenuItem key={v.id} value={v.id}>{`${v.registrationNumber} (${v.category})`}</MenuItem>
                    ))}
                </Select>
            </FormControl>

            <FormControl fullWidth margin="normal">
                <InputLabel id="customer-label">Customer</InputLabel>
                <Select
                    labelId="customer-label"
                    id="customer-select"
                    value={customerId}
                    label="Customer"
                    onChange={e => {
                        const val = e.target.value as unknown as string
                        setCustomerId(val === '' ? '' : Number(val))
                    }}
                >
                    <MenuItem value=""><em>None</em></MenuItem>
                    {customers?.map(c => (
                        <MenuItem key={c.id} value={c.id}>{`${c.firstName} ${c.lastName}`}</MenuItem>
                    ))}
                </Select>
            </FormControl>

            {error && <FormHelperText error>{error}</FormHelperText>}

            <Button type="submit" variant="contained" color="primary" disabled={submitting || vehicleId === '' || customerId === '' || !startDateTime || !endDateTime} sx={{mt:2}}>
                {submitting ? 'Creating...' : 'Create booking'}
            </Button>
        </form>
    )
}

export default CreateBooking
