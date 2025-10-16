import useVehiclesApi from "../../api/useVehiclesApi";
import {useEffect} from "react";

import { Box, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';

const Vehicles = () => {
    const vehiclesApi = useVehiclesApi();
    const { data: vehicles = [], refetch: getVehicles } = vehiclesApi.getVehicles()

    useEffect(() => {
        const fetchVehicles = async () => {
            await getVehicles();
        }
        fetchVehicles();
    }, [getVehicles]);

    return (
        <div>
            <Box sx={{ margin: '2em 0em', padding: '1em', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Category</TableCell>
                                <TableCell>Registration number</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {vehicles.map(v => (
                                <TableRow key={v.id}>
                                    <TableCell>{v.category}</TableCell>
                                    <TableCell>{v.registrationNumber}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Box>
        </div>
    );
}

export default Vehicles;

