const bodyParser = require('body-parser');
const express = require('express');

const rejectIfNoToken = require('../middlewares').rejectIfNoToken;

const orderService = require('../services/orderService');
const cartService = require('../services/cartService');

const router = express.Router();
router.use(bodyParser.urlencoded({ extended: true }));

router.post('/', rejectIfNoToken, async (req, res) => {
    try {
        await orderService.addOrder({
            items: req.body['checkout-items'],
            name: req.body['checkout-name'],
            email: req.body['checkout-email'],
            token: req.body.token
        });
        
        console.log('Order added');
        await cartService.clearCart(req.body.token);
    
        res.render('index', { pageTitle: 'Checkout', entry: 'checkout' });    
    } catch (error) {
        res.status(500);
        res.send({ error })
    }
});

module.exports = router;