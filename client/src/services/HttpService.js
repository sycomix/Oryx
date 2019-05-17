import axios from 'axios';
import AuthService from '../services/AuthService'

axios.interceptors.request.use((config) => {
    const token = AuthService.getToken();

    if(token != null) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;

}, (error) => {
    return Promise.reject(error);
})

class HttpService {
    get(url, paramsObject = null) {
        if(paramsObject) {
            return axios.get(url, { params: paramsObject} );
        }
        return axios.get(url);
    }

    post(url, data) {
        return axios.post(url, { data: data});
    }

    put(url, data) {
        return axios.put(url, { data: data});
    }

    delete(url, data) {
        return axios.delete(url, { data: data});
    }
}

export default new HttpService();