const bodyParser = require('body-parser');
const express = require('express');

const feedbackService = require('../services/feedbackService');

const router = express.Router();
router.use(bodyParser.urlencoded({ extended: true }));

router.post('/', async (req, res) => {
    try {
        await feedbackService.addFeedback(req.body);
        console.log('Feedback added');
        res.render('index', { pageTitle: 'Feedback', entry: 'feedback' });    
    } catch (error) {
        res.status(500);
        res.send({ error });
    }    
});

module.exports = router;