// import axios from 'axios';
// import AuthService from '../services/AuthService'

// const instance = axios.create();
// instance.interceptors.request.use((config) => {
//     const token = AuthService.getToken();
//     console.log('token', token);

//     if(token != null) {
//         config.headers.Authorization = `Bearear ${token}`;
//     }

//     return config;

// }, (error) => {
//     return Promise.reject(error);
// })