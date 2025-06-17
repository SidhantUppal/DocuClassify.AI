// src/services/trainingService.ts

import axios from 'axios';

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

const api = axios.create({
  baseURL: `${apiBaseUrl}/training`,
});


export const uploadTrainingData = async (file: File, label: string) => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('label', label);
  return api.post('/upload-training-data', formData);
};

export const getTrainingData = () => {
  return api.get('/training-data');
};

export const validateTrainingData = (id: string) => {
  return api.put(`/training-data/${id}/validate`);
};

export const startTraining = () => {
  return api.post('/start-training');
};

export const getTrainingStatus = (jobId: string) => {
  return api.get(`/training-status/${jobId}`);
};

export const getModelMetrics = () => {
  return api.get('/model-metrics');
};
