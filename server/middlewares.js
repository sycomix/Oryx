const middlewares = {
    rejectIfNoToken: (req, res, next) => {
        if (req.method === 'GET' && !req.query.token) {
            res.status(401).send('Unauthorized');
        }
        else if (!req.body.token) {
            res.status(401).send('Unauthorized');
        }
        else {
            next();
        }
    },
}

module.exports = middlewares;