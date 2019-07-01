const bodyParser = require('body-parser');
const express = require('express');

const gnomesService = require('../services/gnomesService');

const router = express.Router();
router.use(bodyParser.json());

router.get('/', (req, res) => {
    const renderData = { pageTitle: 'Browse', entry: 'browse' };
    console.log('Render values: ', renderData);
    res.render('index', renderData);
});

router.get('/api/items', async (req, res) => {
    let tags;
    if (req.query.tags) {
        tags = req.query.tags.split(',');
    }

    console.log('tags', tags);

    try {
        const gnomes = await gnomesService.getGnomes(tags);
        console.info('%d gnomes found', gnomes.length);

        if (tags) {
            console.log('Tags used in filter: ', tags);
        }
                
        // remove the scott gnomes from list
        const items = gnomes.filter(item => item.name.indexOf('Scott Gnome') === -1);

        res.send({ items });
    } catch (error) {
        res.status(500);
        res.send({ items: [] })
    }
});

module.exports = router;