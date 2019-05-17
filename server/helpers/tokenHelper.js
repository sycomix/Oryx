const tokenHelper = {
    extractTokenFromAuthHeader: (authorizationHeader) => {
        const BEARER = 'Bearer ';
        if(authorizationHeader.startsWith(BEARER)) {
            const token = authorizationHeader.slice(BEARER.length, authorizationHeader.length);
            return (token) ? token : null;
        }
    
        return null;
    }
}

module.exports = tokenHelper;