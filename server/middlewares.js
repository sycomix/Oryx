const extractTokenFromAuthHeader = require('../server/helpers/tokenHelper').extractTokenFromAuthHeader;

const middlewares = {
    rejectIfNoToken: (req, res, next) => {
        const authorizationHeader = req.header('Authorization');

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
    rejectIfNoTokenInBody: (req, res, next) => {
        if(!req.body.token) {
            res.status(401).send('Unauthorized');    
        }
        next();
    }
}

module.exports = middlewares;