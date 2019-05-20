import { createUpdateFailedAction, createItemsUpdatedAction } from '../actions/cart-actions';
import HttpService from './http-service';
import { config } from '../config';

class CartService {
    async updateItems() {
        try {
            const result = await HttpService.get(config.apiEndpoints.CART);
            createItemsUpdatedAction(result.data.items);
        }
        catch(error) {
            console.error(error);
            createUpdateFailedAction();
        }
    
    }
    
    async addItem(item) {
        try {
            const result = await HttpService.put(`${config.apiEndpoints.CART}/${item.id}`, { item });
            createItemsUpdatedAction(result.data.items);
        } catch (error) {
            console.error(error);
            createUpdateFailedAction();
        }
        
    }
    
    async removeItem(item) {    
        try {
            const result = await HttpService.delete(`${config.apiEndpoints.CART}/${item.id}`);
            createItemsUpdatedAction(result.data.items);
        } catch (error) {
            console.error(error);
            createUpdateFailedAction();
        }
    }
}

export default new CartService();