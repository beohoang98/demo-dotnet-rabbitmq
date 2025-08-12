import React, { useState } from 'react';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import { CssBaseline, Container, Box } from '@mui/material';
import Header from './components/Header';
import WordGenerator from './components/WordGenerator';
import WordList from './components/WordList';

const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
  },
});

function App() {
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleWordGenerated = () => {
    // Trigger a refresh of the word list after a short delay
    // to allow the backend to process the message
    setTimeout(() => {
      setRefreshTrigger((prev) => prev + 1);
    }, 2000);
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Box sx={{ minHeight: '100vh', backgroundColor: '#f5f5f5' }}>
        <Header />
        <Container maxWidth="lg">
          <WordGenerator onWordGenerated={handleWordGenerated} />
          <WordList refreshTrigger={refreshTrigger} />
        </Container>
      </Box>
    </ThemeProvider>
  );
}

export default App;
