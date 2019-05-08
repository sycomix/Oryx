import { createUpdateFailedAction, createItemsUpdatedAction, createRecommendationsUpdatedAction } from '../../actions/cart-actions';
import { request } from '../api';
import axios from 'axios';
import data from '../../../../server/data/sources/initial-data';

export function updateItems() {
    request({
        url: 'cart/api/items'
    }, (err, res) => {
        if (err) {
            createUpdateFailedAction();
            return;
        }
        createItemsUpdatedAction(res.items);
    });
}

export function getRecommendations() {
    var recommenderApi = 'https://tailwind-recommender.azurewebsites.net/api/getrecommendations';

    request({
        url: 'recommender/api/recommender'
    }, (err, res) => {
        if (err) {
            return;
        }
        recommenderApi = res;
        axios.get(recommenderApi)
            .then(res => {
                console.log("Response data: " + res.data);
    
                // The API returns the complete list of items to force the system to get
                // in sync, in case something bad happened to get it out of sync
    
                // let dataResult = data.slice(0,4);
    
                createRecommendationsUpdatedAction(res.data);
            }).catch((err, res) => {
                console.log(err);
            });
    })

}

export function addItem(item) {
    request({
        url: `cart/api/items/${item.id}`,
        method: 'PUT',
        payload: { item }
    }, (err, res) => {
        if (err) {
            createUpdateFailedAction();
            return;
        }

        // The API returns the complete list of items to force the system to get
        // in sync, in case something bad happened to get it out of sync
        createItemsUpdatedAction(res.items);
    });
}

export function removeItem(item) {
    request({
        url: `cart/api/items/${item.id}`,
        method: 'DELETE'
    }, (err, res) => {
        if (err) {
            createUpdateFailedAction();
            return;
        }

        // The API returns the complete list of items to force the system to get
        // in sync, in case something bad happened to get it out of sync
        createItemsUpdatedAction(res.items);
    });
}
