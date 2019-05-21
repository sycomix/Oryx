import React from 'react';

import './empty-cart-view.css';

export default class EmptyCartView extends React.Component {
    constructor(props, context) {
        super(props, context);
    }

    render() {
        return (
            <div className="gs-cartview-empty">
                <div className="gs-cartview-empty-tagline">Your Shopping Cart is Empty :(</div>
                {/* <img src="/img/empty-shopping-cart-icon.png" /> */}
                <a className="gs-cartview-empty-browse" href="/browse">Browse Gnomes</a>
            </div>
        );
    }
};
