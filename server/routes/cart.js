const bodyParser = require('body-parser');
const express = require('express');

const rejectIfNoToken = require('../middlewares').rejectIfNoToken;
const extractTokenFromAuthHeader = require('../helpers/tokenHelper').extractTokenFromAuthHeader;

const gnomesService = require('../services/gnomesService');
const cartService = require('../services/cartService');

const router = express.Router();
router.use(bodyParser.json());

router.get('/', (req, res) => {
    res.render('index', { pageTitle: 'Cart', entry: 'cart' });
});

const getGnomesFromCart = async (cart) => {
    const gnomes = await gnomesService.getGnomes();
    const filtered = cart.items.map((id) => gnomes.filter((gnome) => gnome.id.toString() === id)[0]);
    
    return filtered;
}

const sendGnomeItems = async (token, res) => {
    const cart = await cartService.getCart(token);
    console.log('cart', cart) ;
    if (!cart || !cart.items || cart.items.length === 0) {
        return res.send({ items: [] });
    }

    const gnomesFromCart = await getGnomesFromCart(cart);
    return res.send({ items: gnomesFromCart });
}

router.get('/api/items', rejectIfNoToken, (req, res) => {
    try {
        const token = extractTokenFromAuthHeader(req.header('Authorization'));
        console.log('token en api/items', token);
        sendGnomeItems(token, res);
    } catch (error) {
        res.status(500);
        return res.send({ items: [] })
    }
    
});

router.put('/api/items/:item_id', rejectIfNoToken, async (req, res) => {    
    console.log('Item targetted %s', req.params.item_id);

    try {
        const token = extractTokenFromAuthHeader(req.header('Authorization'));
        await cartService.addToCart(token, req.params.item_id);
        const gnomeInCatalog = await gnomesService.getGnome(req.params.item_id);
    
        if(!gnomeInCatalog) {
            await gnomesService.addGnomes([ req.body.item ]);        
        }
        sendGnomeItems(token, res);

    } catch (error) {
        res.status(500);
        res.send({ items: [] });
    }
});

router.delete('/api/items/:item_id', rejectIfNoToken, async (req, res) => {
    console.log('Item targetted', req.params.item_id);
    
    try {
        const token = extractTokenFromAuthHeader(req.header('Authorization'));
        await cartService.removeFromCart(token, req.params.item_id);
        sendGnomeItems(token, res);    
    } catch (error) {
        res.status(500);
        res.send({ items: [] });
    }
});

module.exports = router;