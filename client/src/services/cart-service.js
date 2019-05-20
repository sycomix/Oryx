import { createUpdateFailedAction, createItemsUpdatedAction } from '../actions/cart-actions';
import HttpService from './http-service';

class CartService {
    async updateItems() {
        try {
            const result = await HttpService.get('cart/api/items');
            createItemsUpdatedAction(result.data.items);
        }
        catch(error) {
            console.error(error);
            createUpdateFailedAction();
        }
    
    }
    
    async addItem(item) {
        try {
            const result = await HttpService.put(`cart/api/items/${item.id}`, { item });
            createItemsUpdatedAction(result.data.items);
        } catch (error) {
            console.error(error);
            createUpdateFailedAction();
        }
        
    }
    
    async removeItem(item) {    
        try {
            const result = await HttpService.delete(`cart/api/items/${item.id}`);
            createItemsUpdatedAction(result.data.items);
        } catch (error) {
            console.error(error);
            createUpdateFailedAction();
        }
    }
}

export default new CartService();