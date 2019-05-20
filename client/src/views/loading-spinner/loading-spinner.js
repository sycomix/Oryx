import React from 'react';
import Icon from 'react-fa';

import './loading-spinner.css';

export default React.createClass({

    displayName: 'loading-spinner',

    render() {
        return (
            <div>
                <div className="loading-spinner">
                    <p className="text-center">
                        <span className="fa fa-spinner fa-spin fa-3x"></span>
                    </p>
                </div>
                {/* <p>Awaiting connection...</p> */}
            </div>
        );
    }
});
