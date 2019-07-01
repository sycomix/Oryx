import { ReduceStore } from 'flux/utils';
import dispatcher from '../dispatcher';
import { CART_ACTIONS } from '../actions/actions';
import cartService from '../services/cart-service';
import recommenderService from '../services/recommender-service';

// Kickstart the initial fetch of items
cartService.updateItems();
recommenderService.getRecommendations();

class CartStore extends ReduceStore {

    getInitialState() {
        return {
            items: [],
            recommendations: [],
            expandedItem: null
        };
    }

    reduce(state, action) {
        switch (action.actionType) {
            case CART_ACTIONS.ITEMS_UPDATED_ACTION: {
                return {
                    items: action.items,
                    recommendations: state.recommendations
                };
            }

            case CART_ACTIONS.RECS_ADDED_ACTION: {
                return {
                    items: state.items,
                    recommendations: action.items
                }
            }

            case CART_ACTIONS.EXPAND_ITEM_ACTION: {
                const expandedItem = state.recommendations.filter((item) => item.id === action.id)[0];
                if (!expandedItem) {
                    throw new Error(`Internal error: id ${action.id} does not exist`);
                }
                return Object.assign({}, state, {
                    expandedItem
                });
            }

            case CART_ACTIONS.CLOSE_EXPANDED_ITEM_ACTION: {
                return Object.assign({}, state, {
                    expandedItem: null
                });
            }

            
            default: {
                return state;
            }
        }
    }
}

export default new CartStore(dispatcher);
