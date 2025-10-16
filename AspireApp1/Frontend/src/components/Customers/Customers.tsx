import useCustomersApi from "../../api/useCustomersApi";
import { useEffect } from "react";

import { Box, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';

const Customers = () => {
    const customersApi = useCustomersApi();
    const { data: customers = [], refetch: getCustomers } = customersApi.getCustomers()

    useEffect(() => {
        const fetchCustomers = async () => {
            await getCustomers();
        }
        fetchCustomers();
    }, [getCustomers]);

    return (
        <div>
            <Box sx={{ margin: '2em 0em', padding: '1em', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>First name</TableCell>
                                <TableCell>Last name</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {customers.map(c => (
                                <TableRow key={c.id}>
                                    <TableCell>{c.firstName}</TableCell>
                                    <TableCell>{c.lastName}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Box>
        </div>
    );
}

export default Customers;

