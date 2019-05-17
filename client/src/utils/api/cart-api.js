import { createUpdateFailedAction, createItemsUpdatedAction, createRecommendationsUpdatedAction } from '../../actions/cart-actions';
import { request } from '../api';
import axios from 'axios';
import HttpService from '../../services/HttpService';

export async function updateItems() {
    // request({
    //     url: 'cart/api/items'
    // }, (err, res) => {
    //     if (err) {
    //         createUpdateFailedAction();
    //         return;
    //     }
    //     createItemsUpdatedAction(res.items);
    // });
    try {
        console.log('update items');
        const result = await HttpService.get('cart/api/items');
        createItemsUpdatedAction(result.data.items);
    }
    catch(error) {
        console.error(error);
        createUpdateFailedAction();
    }

}

export function getRecommendations() {    
    HttpService.get('recommender/api/recommender')
        .then((result) => {
            console.log('resultRecommender', result.data);
            const axiosWithoutInterceptor = axios.create();
            axiosWithoutInterceptor.get(result.data)
                .then((res) => {
                    console.log("Response data: " + res.data);
                    createRecommendationsUpdatedAction(res.data);
                })
                .catch((error) => console.error(error));
        })
        .catch((error) => console.error(error));
    

}

export function addItem(item) {
    // intentar usar async/await y try/catch, hay que ver dónde comienzan las llamadas y si afecta a react
    HttpService.put(`cart/api/items/${item.id}`, { item })
        .then((result) => {
            console.log(result.data.items);
            createItemsUpdatedAction(result.data.items);
        })
        .catch((error) => {
            console.error(error);
            createUpdateFailedAction();
        });
}

export function removeItem(item) {    
    HttpService.delete(`cart/api/items/${item.id}`)
        .then((result) => createItemsUpdatedAction(result.data.items))
        .catch((error) => {
            console.error(error);
            createUpdateFailedAction();
        });
}
