import React from 'react';
import Icon from 'react-fa';
import { createRemoveFromCartAction } from '../../actions/cart-actions';

import './normal-cart-view.css';

export default class ItemRow extends React.Component {
    constructor(props, context) {
        super(props, context);
        this.state = {
            quantity: 1
        }
    }

    onQuantityChanged = (e) => {
        this.setState({ quantity: e.target.value });
    }

    onItemRemoved = async () => {
        await createRemoveFromCartAction(this.props.item);
    }

    render() {
        const item = this.props.item;
        return (
            <div key={item.id} className="gs-cartview-normal-leftpane-row">
                <div className="gs-cartview-normal-leftpane-row-product">
                    <img src={item.image} />
                    <div>
                        <div>
                            <span className="gs-cartview-normal-leftpane-row-product-name">{item.name}</span>
                        </div>
                        <div>
                            <span className="gs-cartview-normal-leftpane-row-product-by"> by </span>
                            <span className="gs-cartview-normal-leftpane-row-product-author">{item.author}</span>
                        </div>
                        <div className="gs-cartview-normal-leftpane-row-product-size">{item.size.width} x {item.size.height}</div>
                    </div>
                </div>
                <div className="gs-cartview-normal-leftpane-row-total">$2.99</div>
                <div className="gs-cartview-normal-leftpane-row-quantity">
                    <select name={`checkout-items[${item.id}][quantity]`}
                            value={this.state.quantity}
                            onChange={this.onQuantityChanged}
                            data-item-id={item.id}>
                        <option value="1">1</option>
                        <option value="2">2</option>
                        <option value="3">3</option>
                    </select>
                </div>
                <Icon name="close" className="gs-cartview-normal-leftpane-close" onClick={this.onItemRemoved} />
                <input type="hidden" value={item.id} name={`checkout-items[${item.id}][id]`} />
            </div>
        );
    }
};

ItemRow.propTypes = {
    item: React.PropTypes.object
}