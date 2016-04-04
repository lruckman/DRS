var React = require('react');

var Result = React.createClass({
    propTypes: {
        viewLink: React.PropTypes.string,
        thumbnailLink: React.PropTypes.string,
        title: React.PropTypes.string,
        icon: React.PropTypes.string,
        children: React.PropTypes.string
    },
    getDefaultProps: function () {
        return {
            viewLink: '',
            thumbnailLink: '',
            title: '',
            icon: '',
            children: ''
        }
    },
    render: function() {
        return (
            <div className="result media">
                <div className="media-left">
                    <a href={this.props.viewLink} target="_blank">
                        <img src={this.props.thumbnailLink} className="media-object thumbnail" alt={this.props.title} style={{width: 150 + 'px'}} />
                    </a>
                </div>
                <div className="media-body">
                    <h4 className="media-heading">
                        <i className={this.props.icon}></i>&nbsp;
                        {this.props.title}
                    </h4>
                    {this.props.children}
                </div>
            </div>
        );
    }
});

module.exports = Result;