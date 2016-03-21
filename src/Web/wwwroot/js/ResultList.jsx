var React = require('react');
var ReactCSSTransitionGroup = require('react-addons-css-transition-group');
var Result = require('Result');

var ResultList = React.createClass({
    render: function () {
        var documents = this.props.data.documents
            ? this.props.data.documents
            : [];
        var resultNodes = documents.map(function (result) {
            return (
                <Result 
                    key={result.id} 
                    title={result.title} 
                    thumbnailLink={result.thumbnailLink} 
                    viewLink={result.viewLink}
                >
                    {result.abstract}
                </Result>
            );
        });
        return (
            <div className="result-list clearfix" data-next-link={this.props.data.nextLink}>
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