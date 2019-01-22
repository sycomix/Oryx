module.exports = {
    'server': {
        'port': process.env.PORT || 3000,
        'https': false
    },
    'dataSource': 'mongodb',
    'mongodb': {
        'port': '27017', //'10255/?ssl=true', 
        'host': 'localhost', //'gnomedba:RL8G74xPjhrGbV9YoCIkoucqLOxP2ywZFVBkkRrwRRVywSu9QXdZCIVHTosSB7oLQL2TX2rYgXbujhysnT9Ztg==@gnomedba.documents.azure.com',
        'dbName': 'gnomesDB',
        'gnomeCollectionName': 'gnomes',
        'orderCollectionName': 'orders',
        'feedbackCollectionName': 'feedback',
        'cartCollectionName': 'carts',
        get url() {
            const mongodbUri = require('mongodb-uri');
            const url = process.env.MONGO_URL || `mongodb://${this.host}:${this.port}`;
            const urlObject = mongodbUri.parse(url);
            urlObject.database = this.dbName;
            return mongodbUri.format(urlObject);
        }
    }
};
