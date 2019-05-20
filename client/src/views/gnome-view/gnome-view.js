import React from 'react';
import Icon from 'react-fa';
import PropTypes from 'prop-types';
import { createAddToCartAction } from '../../actions/cart-actions';

import './gnome-view.css';

export default class GnomeView extends React.Component {
    constructor(props, context) {
        super(props, context);
    }

    onAddToCartClicked = async () => {
        await createAddToCartAction(this.props.item);
    }

    onCoverartClicked = () => {
        this.props.createExpandItemAction(this.props.item.id);
    }

    render() {
        return (
            <div className="gs-gnome">
                <div className="gs-gnome-image"
                    style={{ backgroundImage: `url(${this.props.item.image})` }}
                    onClick={this.onCoverartClicked} ></div>
                <div className="gs-gnome-metadata">
                    <div className="gs-gnome-metadata-title">{this.props.item.name}</div>
                    <div className="gs-gnome-metadata-price">{this.props.item.price}</div>
                    <div className="gs-gnome-metadata-author">
                        <Icon name="camera" className="gs-gnome-metadata-author-icon" />
                        <div>{this.props.item.author}</div>
                    </div>
                    <div className="gs-gnome-metadata-tags">
                        <Icon name="tag" size="lg" className="gs-gnome-metadata-tags-icon" />
                        <div>{this.props.item.tags.join(', ')}</div>
                    </div>
                    <div className="gs-gnome-metadata-bottomrow">
                        <div>{this.props.item.size.width} x {this.props.item.size.height}</div>
                        <div className="gs-gnome-metadata-cart" onClick={this.onAddToCartClicked}>
                            <Icon name="shopping-cart" className="gs-gnome-metadata-cart-icon" />
                            <div>Add to cart</div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
};

GnomeView.propTypes = {
    createExpandItemAction: PropTypes.func.isRequired,
    item: PropTypes.object.isRequired
}