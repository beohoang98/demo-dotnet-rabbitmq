import axios from 'axios';

const API_BASE_URL = '/api';

const api = axios.create({
  baseURL: API_BASE_URL,
});

export const wordService = {
  // Get all words
  getAllWords: async () => {
    const response = await api.get('/word');
    return response.data;
  },

  // Get a specific word
  getWord: async (word) => {
    const response = await api.get(`/word/${encodeURIComponent(word)}`);
    return response.data;
  },

  // Generate a new word
  generateWord: async (word) => {
    const formData = new FormData();
    formData.append('word', word);

    const response = await api.post('/word/generate', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};
