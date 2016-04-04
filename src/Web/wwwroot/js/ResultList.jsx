var React = require('react');
var ReactCSSTransitionGroup = require('react-addons-css-transition-group');
var Result = require('Result');

var ResultList = React.createClass({
    propTypes: {
        data: React.PropTypes.arrayOf(
                React.PropTypes.shape({
                    id: React.PropTypes.number,
                    abstract: React.PropTypes.string,
                    viewLink: React.PropTypes.string,
                    thumbnailLink: React.PropTypes.string,
                    title: React.PropTypes.string,
                    icon: React.PropTypes.string
                })
        ),
        nextLink: React.PropTypes.string,
        onSelect: React.PropTypes.func
    },
    getDefaultProps: function () {
        return {
            data: [],
            nextLink: '',
            onSelect: function() {}
        }
    },
    handleSelect: function() {
        // this.props.onSelect();
    },
    render: function () {
        var resultNodes = this.props.data.map(function (result) {
            return (
                <Result 
                    key={result.id} 
                    title={result.title} 
                    thumbnailLink={result.thumbnailLink} 
                    viewLink={result.viewLink}
                    icon={result.icon}
                >
                    {result.abstract}
                </Result>
            );
        });
        return (
            <div className="result-list clearfix" data-next-link={this.props.nextLink}>
                <ReactCSSTransitionGroup 
                    transitionName="result" 
                    transitionEnterTimeout={500} 
                    transitionLeaveTimeout={300}
                >
                  {resultNodes}
                </ReactCSSTransitionGroup>
            </div>
        );
    }
});

module.exports = ResultList;