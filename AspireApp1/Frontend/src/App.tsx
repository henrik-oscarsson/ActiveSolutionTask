import React from 'react';
import './App.css';
import {
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query'
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Link
} from "react-router-dom";
import AppBar from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import {Container, MenuItem, Typography} from "@mui/material";
import Bookings from "./components/Bookings/Bookings";
import Vehicles from "./components/Vehicles/Vehicles";
import Customers from "./components/Customers/Customers";

const queryClient = new QueryClient()

function App() {
  
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <AppBar position="sticky">
          <Toolbar>          
            <MenuItem key='bookings'>
              <Link to="/" className='Menu-link'><Typography sx={{ textAlign: 'center' }}>Bookings</Typography></Link>
            </MenuItem>
            <MenuItem key='customers'>
              <Link to="/customers" className='Menu-link'><Typography sx={{ textAlign: 'center' }}>Customers</Typography></Link>
            </MenuItem>
            <MenuItem key='vehicles'>
              <Link to="/vehicles" className='Menu-link'><Typography sx={{ textAlign: 'center' }}>Vehicles</Typography></Link>
            </MenuItem>
          </Toolbar>
        </AppBar>
        <Container component="main" sx={{maxHeight:'100vh'}}>
          <Routes>
            <Route path="/customers" Component={Customers} />
            <Route path="/vehicles" Component={Vehicles} />
            <Route path="/" Component={Bookings} />
          </Routes>
        </Container>
      </Router>
    </QueryClientProvider>
  );
}


export default App;
