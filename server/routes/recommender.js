const bodyParser = require('body-parser');
const express = require('express');
const router = express.Router();

router.use(bodyParser.json());

router.get('/api/recommender', (req, res) => {
    res.send(process.env.RECOMMENDER || "https://tailwind-recommender.azurewebsites.net/api/getrecommendations");
});

module.exports = router;