import React from 'react';
import GnomeListView from '../../views/gnome-list-view/gnome-list-view';
import { createExpandItemAction } from '../../actions/cart-actions';
import AuthService from '../../services/auth-service';

import ItemRow from './item-row';

import './normal-cart-view.css';


export default class NormalCartView extends React.Component {
    constructor(props, context) {
        super(props, context);
        this.token = AuthService.getToken();
    }

    render() {
        let items = this.props.items;
        // TODO: items is an array of IDs, need to look up or convert to whole objects
        const itemRows = items.map((item) => <ItemRow item={item} key={item.id} />);
        
        return (
            <div className="gs-cartview-normal">

                <div className="gs-cartview-normal-header">
                    You have <span className="gs-cartview-normal-header-count">
                        {items.length}
                    </span> {items.length === 1 ? 'item' : 'items'} in your cart.
                </div>

                <form className="gs-cartview-normal-body" action="/checkout" method="post">

                    <div className="gs-cartview-normal-leftpane">
                        <div className="gs-cartview-normal-leftpane-header">
                            <div className="gs-cartview-normal-leftpane-header-products">Products</div>
                            <div className="gs-cartview-normal-leftpane-header-total">Total</div>
                            <div className="gs-cartview-normal-leftpane-header-quantity">Quantity</div>
                        </div>
                        {itemRows}
                        <input type="hidden" name="token" value={this.token} />
                    </div>

                    <div className="gs-cartview-normal-rightpane">
                        <div className="gs-cartview-normal-rightpane-label">Name</div>
                        <input placeholder="required" className="gs-cartview-normal-rightpane-input" name="checkout-name" min="1" />
                        <div className="gs-cartview-normal-rightpane-label">Email Address</div>
                        <input placeholder="required" className="gs-cartview-normal-rightpane-input" name="checkout-email" type="email" min="1" />
                        <div className="gs-cartview-normal-rightpane-submitcontainer">
                            <button type="submit" className="gs-cartview-normal-rightpane-submit">
                                <div>Place order</div>
                            </button>
                        </div>
                    </div>
                </form>

                <div className="gs-cartview-normal-header">
                    Recommended based on your selection:
                </div>
                <GnomeListView items={this.props.recommendations} createExpandItemAction={createExpandItemAction} />
            </div>
        );
    }
}

NormalCartView.propTypes = {
    items: React.PropTypes.arrayOf(React.PropTypes.object).isRequired,
    recommendations: React.PropTypes.arrayOf(React.PropTypes.object)
}

