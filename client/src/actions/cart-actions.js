import dispatcher from '../dispatcher';
import { CART_ACTIONS } from './actions.js';
// import { addItem, removeItem, getRecommendations } from '../utils/api/cart-api';
import cartService from '../services/cart-service';
import recommenderService from '../services/recommender-service';

export function createItemsUpdatedAction(items) {
    dispatcher.dispatch({
        actionType: CART_ACTIONS.ITEMS_UPDATED_ACTION,
        items
    });
}

export function createRecommendationsUpdatedAction(items) {
    dispatcher.dispatch({
        actionType: CART_ACTIONS.RECS_ADDED_ACTION,
        items
    })
}

export function createUpdateFailedAction() {
    dispatcher.dispatch({
        actionType: CART_ACTIONS.UPDATE_FAILED_ACTION
    });
}

export function createExpandItemAction(id) {
    dispatcher.dispatch({
        actionType: CART_ACTIONS.EXPAND_ITEM_ACTION,
        id
    });
}

export function createCloseExpandedItemAction() {
    dispatcher.dispatch({
        actionType: CART_ACTIONS.CLOSE_EXPANDED_ITEM_ACTION
    });
}

export function createAddToCartAction(item) {
    cartService.addItem(item);
}

export function createRemoveFromCartAction(item) {
    cartService.removeItem(item);
}

export function createRecommendationsAddedAction() {
    recommenderService.getRecommendations();
}

