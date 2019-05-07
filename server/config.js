module.exports = {
    'server': {
        'port': process.env.PORT || 3000,
        'https': false
    },
    'dataSource': 'mongodb',
    'mongodb': {
        // 'port': '10255/?ssl=true', 
        // 'host': 'tailwindtraders:z9b9AB3hViNJNajy3Z3RgV10Umgv5XbZNEQLX9yQntiU0eTS2pwxxRKQas8aF1h8oPu2MStU9BFdiDYEcyzVQA==@tailwindtraders.documents.azure.com',
        'port': '27017',
        'host': 'mongodb',
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
