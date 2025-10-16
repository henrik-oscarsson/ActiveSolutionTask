import { Button, FormControl, TextField } from "@mui/material";
import React, { useCallback, useState } from 'react'

interface ReturnBookingProps {
    bookingId: number,
    onReturned?: (meterSetting: number) => void
}

const ReturnBooking: React.FC<ReturnBookingProps> = ({ bookingId, onReturned }) => {
    const [meterSetting, setMeterSetting] = useState<number | ''>('')

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        let ms = typeof meterSetting === 'number' ? meterSetting : Number(meterSetting)
        if (Number.isNaN(ms) || ms < 0) ms = 0
        onReturned && onReturned(ms)
    }

    const isMeterSettingValid = useCallback(() => (value: number | '') => typeof value === 'number' && value > 0 && meterSetting !== '',
        [meterSetting])

    return (
        <>
            <form onSubmit={handleSubmit} style={{ maxWidth: 600 }}>
                <FormControl fullWidth margin="normal">
                    <TextField
                        label="Meter setting"
                        type="number"
                        value={meterSetting}
                        onChange={e => {
                            const val = e.target.value
                            if (val === '') {
                                setMeterSetting('')
                            } else {
                                const n = parseInt(val)
                                setMeterSetting(Number.isNaN(n) ? '' : Math.max(0, n))
                            }
                        }}
                        inputProps={{ min: 0 }}
                        fullWidth
                        margin="normal"
                        autoFocus
                        error={!isMeterSettingValid()}
                        helperText={!isMeterSettingValid() ? 'Enter a positive integer' : ''}
                    />
                </FormControl>
                <Button type="submit" variant="contained" color="primary" sx={{ mt: 2 }} disabled={!isMeterSettingValid()}>Return</Button>
            </form >
        </>
    )
}

export default ReturnBooking
