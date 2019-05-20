import React from 'react';

import './loading-spinner.css';

export default class LoadingSpinner extends React.Component {
    constructor(props, context) {
        super(props, context);
    }

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
};
