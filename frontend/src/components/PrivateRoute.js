import React from 'react';
import { Navigate } from 'react-router-dom';
import authService from 'services/authService';

const PrivateRoute = ({ children }) => {
  if (!authService.isAuthenticated()) {
    return <Navigate to="/auth/login" replace />;
  }

  return children;
};

export default PrivateRoute; 