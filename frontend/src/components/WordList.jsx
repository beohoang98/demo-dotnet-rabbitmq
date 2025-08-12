import React, { useState, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  Grid,
  Card,
  CardContent,
  CardActionArea,
  Chip,
  Alert,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
} from '@mui/material';
import { Refresh as RefreshIcon } from '@mui/icons-material';
import { wordService } from '../services/api';

const WordList = ({ refreshTrigger }) => {
  const [words, setWords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedWord, setSelectedWord] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [wordDetails, setWordDetails] = useState(null);
  const [loadingDetails, setLoadingDetails] = useState(false);

  const fetchWords = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await wordService.getAllWords();
      setWords(Array.isArray(data) ? data : []);
    } catch (err) {
      setError('Failed to fetch words. Please try again.');
      console.error('Error fetching words:', err);
    } finally {
      setLoading(false);
    }
  };

  const fetchWordDetails = async (word) => {
    try {
      setLoadingDetails(true);
      const data = await wordService.getWord(word);
      setWordDetails(data);
    } catch (err) {
      setWordDetails(null);
      console.error('Error fetching word details:', err);
    } finally {
      setLoadingDetails(false);
    }
  };

  const handleWordClick = (word) => {
    setSelectedWord(word);
    setDialogOpen(true);
    fetchWordDetails(word.word || word);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setSelectedWord(null);
    setWordDetails(null);
  };

  useEffect(() => {
    fetchWords();
  }, [refreshTrigger]);

  if (loading) {
    return (
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        minHeight={200}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert
        severity="error"
        action={
          <Button
            color="inherit"
            size="small"
            onClick={fetchWords}
            startIcon={<RefreshIcon />}
          >
            Retry
          </Button>
        }
      >
        {error}
      </Alert>
    );
  }

  if (words.length === 0) {
    return (
      <Paper elevation={1} sx={{ p: 4, textAlign: 'center' }}>
        <Typography variant="h6" color="text.secondary" gutterBottom>
          No words found
        </Typography>
        <Typography variant="body2" color="text.secondary">
          Generate your first word using the form above!
        </Typography>
      </Paper>
    );
  }

  return (
    <>
      <Box
        sx={{
          mb: 2,
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <Typography variant="h6">Dictionary Words ({words.length})</Typography>
        <Button
          size="small"
          onClick={fetchWords}
          startIcon={<RefreshIcon />}
          disabled={loading}
        >
          Refresh
        </Button>
      </Box>

      <Grid container spacing={2}>
        {words.map((word, index) => (
          <Grid item xs={12} sm={6} md={4} key={word.id || index}>
            <Card elevation={2}>
              <CardActionArea onClick={() => handleWordClick(word)}>
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {word.word || word}
                  </Typography>
                  {word.definition && (
                    <Typography
                      variant="body2"
                      color="text.secondary"
                      sx={{
                        overflow: 'hidden',
                        textOverflow: 'ellipsis',
                        display: '-webkit-box',
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: 'vertical',
                      }}
                    >
                      {word.definition}
                    </Typography>
                  )}
                  {word.partOfSpeech && (
                    <Chip
                      label={word.partOfSpeech}
                      size="small"
                      sx={{ mt: 1 }}
                      color="primary"
                      variant="outlined"
                    />
                  )}
                </CardContent>
              </CardActionArea>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Word Details Dialog */}
      <Dialog
        open={dialogOpen}
        onClose={handleCloseDialog}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>{selectedWord?.word || selectedWord}</DialogTitle>
        <DialogContent>
          {loadingDetails ? (
            <Box display="flex" justifyContent="center" p={3}>
              <CircularProgress />
            </Box>
          ) : wordDetails ? (
            <Box>
              {wordDetails.partOfSpeech && (
                <Chip
                  label={wordDetails.partOfSpeech}
                  color="primary"
                  sx={{ mb: 2 }}
                />
              )}
              {wordDetails.definition && (
                <Box mb={2}>
                  <Typography variant="h6" gutterBottom>
                    Definition
                  </Typography>
                  <Typography variant="body1">
                    {wordDetails.definition}
                  </Typography>
                </Box>
              )}
              {wordDetails.example && (
                <Box mb={2}>
                  <Typography variant="h6" gutterBottom>
                    Example
                  </Typography>
                  <Typography variant="body1" style={{ fontStyle: 'italic' }}>
                    "{wordDetails.example}"
                  </Typography>
                </Box>
              )}
              {wordDetails.synonyms && (
                <Box mb={2}>
                  <Typography variant="h6" gutterBottom>
                    Synonyms
                  </Typography>
                  <Typography variant="body1">
                    {wordDetails.synonyms}
                  </Typography>
                </Box>
              )}
              {wordDetails.antonyms && (
                <Box mb={2}>
                  <Typography variant="h6" gutterBottom>
                    Antonyms
                  </Typography>
                  <Typography variant="body1">
                    {wordDetails.antonyms}
                  </Typography>
                </Box>
              )}
            </Box>
          ) : (
            <Typography>
              Word details are being processed. Please check back later.
            </Typography>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Close</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default WordList;
