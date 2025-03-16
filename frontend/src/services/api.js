import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5000',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/auth/login';
      return Promise.reject({ message: 'Sessão expirada. Por favor, faça login novamente.' });
    }

    // Trata erros de validação
    if (error.response?.status === 400) {
      const validationErrors = error.response.data.errors;
      if (validationErrors) {
        const messages = Object.values(validationErrors).flat();
        return Promise.reject({ message: messages.join(', ') });
      }
    }

    // Trata outros erros
    const message = error.response?.data?.message || 'Ocorreu um erro inesperado.';
    return Promise.reject({ message });
  }
);

export default api; 