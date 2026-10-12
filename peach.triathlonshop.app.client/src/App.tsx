import { Container } from '@mui/material'
import { Outlet } from 'react-router-dom'

function App() {
    return (
        <Container maxWidth="lg" sx={{ py: 4 }}>
            <Outlet />
        </Container>
    );
}

export default App;