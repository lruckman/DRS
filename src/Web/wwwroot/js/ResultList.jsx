var React = require('react');
var ReactCSSTransitionGroup = require('react-addons-css-transition-group');
var classNames = require('classnames');

var ResultList = React.createClass({
    propTypes: {
        data: React.PropTypes.arrayOf(
                React.PropTypes.shape({
                    abstract: React.PropTypes.string,
                    id: React.PropTypes.number,
                    selfLink: React.PropTypes.string,
                    thumbnailLink: React.PropTypes.string,
                    title: React.PropTypes.string
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
        this.props.onSelect(this.props.data[index].selfLink);
    },
    render: function () {
        var resultNodes = this.props.data.map(function (result, index) {
            var resultClass = classNames({
                'active': index === this.state.activeIndex
            });
            var boundClick = this.resultHandleClick.bind(this, index);
            return (
                <div key={result.id} className="card">
                    <a href="#" target="_blank" className="thumbnail-link" onClick={boundClick}>
                        <div className="thumbnail" style={{backgroundImage: 'url(' + result.thumbnailLink + ')'}}>
                        </div>
                    </a>
                    <div className="title-container">
                        <a href={result.selfLink} title={result.title} className="title" onClick={boundClick}>
                            {result.title}
                        </a>
                        <span className="abstract">
                            {result.abstract}
                        </span>
                    </div>
                </div>
            );
        }, this);
        return (
            <div data-next-link={this.props.nextLink}>
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