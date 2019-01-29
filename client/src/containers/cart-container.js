import React from 'react';
import { Container } from 'flux/utils';

import HeaderView from '../views/header-view/header-view';
import EmptyCartView from '../views/empty-cart-view/empty-cart-view';
import NormalCartView from '../views/normal-cart-view/normal-cart-view';
import { createExpandItemAction, createCloseExpandedItemAction } from '../actions/cart-actions';
import cartStore from '../stores/cart-store';
import ExpandedItemView from '../views/expanded-item-view/expanded-item-view';

import './base.css';


class CartContainer extends React.Component {
    render() {
        console.log("Recommendations:" + this.state.cart.recommendations);
        const cartView = this.state.cart.items.length === 0 ?
            <EmptyCartView /> :
            <NormalCartView items={this.state.cart.items} recommendations={this.state.cart.recommendations} createExpandItemAction={createExpandItemAction} />;
        
            let expandedItem;
        if (this.state.cart.expandedItem) {
            expandedItem = <ExpandedItemView item={this.state.cart.expandedItem} createCloseExpandedItemAction={createCloseExpandedItemAction} />;
        }

        return (
            <div>
                <HeaderView pageName="cart" cartCount={this.state.cart.items.length}/>
                {cartView}
                {expandedItem}
            </div>
        );
    }
}

CartContainer.getStores = () => [cartStore];

CartContainer.calculateState = () => ({
    cart: cartStore.getState()
});

export default Container.create(CartContainer);
