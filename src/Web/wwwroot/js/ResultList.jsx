var React = require('react');
var ReactCSSTransitionGroup = require('react-addons-css-transition-group');
var classNames = require('classnames');

var ResultList = React.createClass({
    propTypes: {
        data: React.PropTypes.arrayOf(
                React.PropTypes.shape({
                    id: React.PropTypes.number,
                    abstract: React.PropTypes.string,
                    location: React.PropTypes.string,
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
    getInitialState: function () {
        return {
            activeIndex: 0
        };
    },
    resultHandleClick: function (index, e) {
        e.preventDefault();
        this.setState({ activeIndex: index });
        this.props.onSelect(this.props.data[index].location);
    },
    render: function () {
        var resultNodes = this.props.data.map(function (result, index) {
            var resultClass = classNames({
                'list-group-item': true,
                'active': index === this.state.activeIndex
            });
            var boundClick = this.resultHandleClick.bind(this, index);
            return (
                <a href="#" target="_blank" className={resultClass} key={result.id} onClick={boundClick}>
                    <i className={result.icon}></i>&nbsp;{result.title}
                </a>
            );
        }, this);
        return (
            <div className="list-group" data-next-link={this.props.nextLink}>
                <ReactCSSTransitionGroup 
                    transitionName="result" 
                    transitionEnterTimeout={500} 
                    transitionLeaveTimeout={300} >
                  {resultNodes}
                </ReactCSSTransitionGroup>
            </div>
        );
    }
});

module.exports = ResultList;