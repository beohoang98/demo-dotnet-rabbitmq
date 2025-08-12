import React from 'react';
import { AppBar, Toolbar, Typography, Box } from '@mui/material';
import { Book as BookIcon } from '@mui/icons-material';

const Header = () => {
  return (
    <AppBar position="static" sx={{ mb: 4 }}>
      <Toolbar>
        <BookIcon sx={{ mr: 2 }} />
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          Dictionary Generator
        </Typography>
        <Typography variant="subtitle1">Demo .NET RabbitMQ App</Typography>
      </Toolbar>
    </AppBar>
  );
};

export default Header;
