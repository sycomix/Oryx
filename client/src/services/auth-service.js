import { v4 } from 'node-uuid'; // Yes this works in the browser too

class AuthService {
    
    getToken() {
        // Temporary until we implement actual auth
        if(!localStorage.token) {
            localStorage.token = v4();
        }

        return localStorage.token;
    }
}

export default new AuthService();