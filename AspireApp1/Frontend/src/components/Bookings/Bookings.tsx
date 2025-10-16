import React, {useEffect, useState, useRef} from 'react';
import useBookingsApi from "../../api/useBookingsApi";
import CreateBooking from './CreateBooking'
import { Box, Button, Dialog, DialogTitle, DialogContent, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Tooltip } from '@mui/material';
import { format, parseISO, formatDistanceToNow } from 'date-fns'
import { toZonedTime } from 'date-fns-tz'
import PickupBooking from './PickupBooking'
import ReturnBooking from './ReturnBooking'
import useSettingsApi from '../../api/useSettingsApi';

const Bookings = () => {
    const bookingsApi = useBookingsApi();
    const { data: bookings = [], refetch: getBookings } = bookingsApi.getBookings()
    const [selectedBookingId, setSelectedBookingId] = useState<number | null>(null);
    const [meterSetting, setMeterSetting] = useState<number | null>(null);
    const { mutate: pickupBooking } = bookingsApi.pickupBooking(selectedBookingId || 0);
    const { mutate: returnBooking } = bookingsApi.returnBooking(selectedBookingId || 0, meterSetting || 0);
    const settingsApi = useSettingsApi();
    const { data: settings, refetch: getSettings } = settingsApi.getSettings();
    const [bookingCreated, setbookingCreated] = useState(0);
    const [openCreate, setOpenCreate] = useState(false);
    const [openPickup, setOpenPickup] = useState(false);
    const [openReturn, setOpenReturn] = useState(false);
    const tableRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const fetchBookings = async () => {
            await getBookings();
        }
        fetchBookings();
    }, [getBookings, bookingCreated]);

    useEffect(() => {
        const fetchSettings = async () => {
            await getSettings();
        }
        fetchSettings();
    }, [getSettings]);

    // Only clear selection on outside click when no modal/dialog is open.
    useEffect(() => {
        const handleClickOutside = (e: MouseEvent) => {
            // If any dialog is open, ignore outside clicks
            if (openCreate || openPickup || openReturn) return

            const el = tableRef.current
            if (!el) return
            if (e.target && !el.contains(e.target as Node)) {
                setSelectedBookingId(null)
            }
        }

        document.addEventListener('mousedown', handleClickOutside)
        return () => document.removeEventListener('mousedown', handleClickOutside)
    }, [openCreate, openPickup, openReturn])
    
    const refreshListing = () => {
        setTimeout(() => setbookingCreated(bookingCreated + 1), 1000)
    }

    const formatDateTime = (input?: string | Date | null) => {
        if (!input) return ''
    const parsed = typeof input === 'string' ? parseISO(input) : input as Date
    // convert UTC/ISO to the local timezone
    const d = toZonedTime(parsed, Intl.DateTimeFormat().resolvedOptions().timeZone)
        try {
            const formatted = format(d, 'PPp')
            const relative = formatDistanceToNow(d, { addSuffix: true })
            return `${formatted} (${relative})`
        } catch (e) {
            return String(d)
        }
    }
    
    const tooltipText = `Based on a base rent per day of ${settings?.baseRent} SEK and a kilometer price ${settings?.baseKilometerPrice} SEK. Estimated number of days: ${settings?.defaultNumberOfDays} and kilometers: ${settings?.defaultNumberOfKilometers}.`;

    console.log('render Bookings', bookings[0])

    return (
        <div>
            <Box sx={{ margin: '2em 0em', padding: '1em', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <Button variant='contained' onClick={() => setOpenCreate(true)} sx={{ ml: 'auto' }}>New booking</Button>
                <Dialog open={openCreate} onClose={() => setOpenCreate(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>New booking</DialogTitle>
                    <DialogContent>
                        <CreateBooking onCreated={() => {
                            setOpenCreate(false)
                            refreshListing()
                        }} />
                    </DialogContent>
                </Dialog>
                <Dialog open={openPickup} onClose={() => setOpenPickup(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>Pickup booking</DialogTitle>
                    <DialogContent>
                        {selectedBookingId && <PickupBooking bookingId={selectedBookingId} onPickedUp={() => {
                            pickupBooking()
                            setOpenPickup(false)
                            setSelectedBookingId(null)
                            refreshListing()
                        }} />}
                    </DialogContent>
                </Dialog>
                <Dialog open={openReturn} onClose={() => setOpenReturn(false)} maxWidth="sm" fullWidth>
                    <DialogTitle>Return booking</DialogTitle>
                    <DialogContent>
                        {selectedBookingId && <ReturnBooking bookingId={selectedBookingId} onReturned={(meterSetting) => {
                            setMeterSetting(meterSetting)
                            setOpenReturn(false)
                            setTimeout(() => {
                                returnBooking()
                                setSelectedBookingId(null)
                                refreshListing()
                            }, 500)
                        }} />}
                    </DialogContent>
                </Dialog>
            </Box>
            <h2>Existing bookings</h2>
            <Box sx={{ marginTop: '2em' }}>
                <TableContainer component={Paper} ref={tableRef}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Pickup date</TableCell>
                                <TableCell>Return date</TableCell>
                                <TableCell>Registration</TableCell>
                                <TableCell>Customer</TableCell>
                                <TableCell><Tooltip title={tooltipText}><span>Est. cost *</span></Tooltip></TableCell>
                                <TableCell>Act cost</TableCell>
                                <TableCell>Pickup</TableCell>
                                <TableCell>Return</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {bookings.map(b => (
                                <TableRow key={b.id}
                                          hover
                                          selected={selectedBookingId === b.id}
                                          onClick={() => setSelectedBookingId(b.id)}
                                          sx={{ cursor: 'pointer', transition: 'background-color 250ms ease' , '&.Mui-selected, &.Mui-selected:hover': { backgroundColor: (theme) => theme.palette.action.selected } }}
                                >
                                    <TableCell>{formatDateTime(b.scheduledPickUpDate)}</TableCell>
                                    <TableCell>{formatDateTime(b.scheduledReturnDate)}</TableCell>
                                    <TableCell>{b.vehicleRegistrationNumber}</TableCell>
                                    <TableCell>{b.customerName}</TableCell>
                                    <TableCell>{b.estimatedCost}</TableCell>
                                    <TableCell>{b.actualCost}</TableCell>
                                    <TableCell>
                                        <Button size="small" variant="outlined" onClick={() => setOpenPickup(true)} disabled={(selectedBookingId !== b.id || b.isPickedUp === true)}>Pickup</Button>
                                    </TableCell>
                                    <TableCell>
                                        <Button size="small" variant="outlined" onClick={() => setOpenReturn(true)} disabled={(selectedBookingId !== b.id || b.isPickedUp === false || b.isReturned === true)}>Return</Button>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Box>
        </div>
    );
}

export default Bookings;

