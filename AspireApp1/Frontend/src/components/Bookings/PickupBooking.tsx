import { Button } from "@mui/material";

interface PickupBookingProps {
    bookingId: number,
    onPickedUp?: () => void
}

const PickupBooking: React.FC<PickupBookingProps> = ({ bookingId, onPickedUp }) => {
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        onPickedUp && onPickedUp()
    }
    
    return (
        <form onSubmit={handleSubmit} style={{ maxWidth: 600 }}>
            <Button type="submit" variant="contained" color="primary" sx={{mt:2}}>Pickup</Button>
        </form>
    )
}

export default PickupBooking
