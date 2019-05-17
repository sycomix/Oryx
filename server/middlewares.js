const extractTokenFromAuthHeader = require('../server/helpers/tokenHelper').extractTokenFromAuthHeader;

const middlewares = {
    rejectIfNoToken: (req, res, next) => {
        const authorizationHeader = req.header('Authorization');
        console.log('auth', authorizationHeader);

        if(authorizationHeader) {
            const token = extractTokenFromAuthHeader(req.header('Authorization'));
            if(token) {
                next();
            }
            else {
                res.status(401).send('Unauthorized');    
            }
        }
        else {
            res.status(401).send('Unauthorized');
        }
    },
}

module.exports = middlewares;