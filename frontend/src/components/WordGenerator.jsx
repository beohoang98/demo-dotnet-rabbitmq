import React, { useState } from 'react';
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Alert,
  CircularProgress,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { wordService } from '../services/api';

const WordGenerator = ({ onWordGenerated }) => {
  const [word, setWord] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!word.trim()) return;

    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      await wordService.generateWord(word.trim());
      setSuccess(`Word "${word.trim()}" has been queued for generation!`);
      setWord('');
      if (onWordGenerated) {
        onWordGenerated();
      }
    } catch (err) {
      setError(
        err.response?.data?.message ||
          'Failed to generate word. Please try again.'
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Paper elevation={3} sx={{ p: 3, mb: 4 }}>
      <Typography variant="h6" gutterBottom>
        Generate New Word
      </Typography>
      <Box
        component="form"
        onSubmit={handleSubmit}
        sx={{ display: 'flex', gap: 2, alignItems: 'flex-start' }}
      >
        <TextField
          fullWidth
          variant="outlined"
          label="Enter a word to generate"
          value={word}
          onChange={(e) => setWord(e.target.value)}
          disabled={loading}
          placeholder="e.g., example, dictionary, programming"
        />
        <Button
          type="submit"
          variant="contained"
          disabled={loading || !word.trim()}
          startIcon={loading ? <CircularProgress size={20} /> : <AddIcon />}
          sx={{ minWidth: 120, height: 56 }}
        >
          {loading ? 'Generating...' : 'Generate'}
        </Button>
      </Box>
      {error && (
        <Alert severity="error" sx={{ mt: 2 }}>
          {error}
        </Alert>
      )}
      {success && (
        <Alert severity="success" sx={{ mt: 2 }}>
          {success}
        </Alert>
      )}
    </Paper>
  );
};

export default WordGenerator;
