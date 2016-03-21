var React = require('react');

var Result = React.createClass({
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
                        {this.props.title}
                    </h4>
                    {this.props.children}
                </div>
            </div>
        );
    }
});

module.exports = Result;