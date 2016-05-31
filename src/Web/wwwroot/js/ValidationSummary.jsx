var React = require('react');
var classNames = require('classnames');

var ValidationSummary = React.createClass({
    propTypes: {
        errors: React.PropTypes.object,
        message: React.PropTypes.string
    },
    getDefaultProps: function () {
        return {
            errors: {},
            message: 'You must correct the following errors and try again.'
        }
    },
    getInitialState () {
        return { };
    },
    render: function () {
        var rows = Object.keys(this.props.errors).map(function (key) {
            var row = this.props.errors[key].map(function (error, i) {
                return (<li key={i}>{error}</li>);
            });
                return (row);
        }, this);
        var cssClass = classNames({
            'alert': true,
            'alert-danger': true,
            'hidden': rows.length===0
        });
        return (
            <div className={cssClass}>
                {this.props.message}
                <ul>
                    {rows}
                </ul>
            </div>
        );
    }
});

module.exports = ValidationSummary;