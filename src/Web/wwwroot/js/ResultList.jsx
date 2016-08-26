var React = require('react');
var ReactCSSTransitionGroup = require('react-addons-css-transition-group');
var classNames = require('classnames');

var ResultList = React.createClass({
    propTypes: {
        data: React.PropTypes.arrayOf(
            React.PropTypes.shape({
                abstract: React.PropTypes.string,
                file: React.PropTypes.shape({
                    thumbnailLink: React.PropTypes.string,
                    viewLink: React.PropTypes.string
                }),
                id: React.PropTypes.number,
                selfLink: React.PropTypes.string,
                title: React.PropTypes.string
            })
        ),
        nextLink: React.PropTypes.string,
        onSelect: React.PropTypes.func
    },
    getDefaultProps: function() {
        return {
            data: [],
            nextLink: '',
            onSelect: function() {}
        }
    },
    getInitialState: function() {
        return {
            activeIndex: []
        };
    },
    resultHandleClick: function(index, e) {
        e.preventDefault();

        var activeIndex = e.shiftKey
            ? this.state.activeIndex
            : [];

        activeIndex.push(index);

        this.setState({ activeIndex: activeIndex });
        
        this.clickStatus = 1;
        this.clickTimer = setTimeout(function() {
            if (this.clickStatus === 1) {
                this.props.onSelect(this.props.data[index].selfLink);
            }
        }.bind(this), 200);
    },
    resultHandleDblClick: function(index, e) {
        e.preventDefault();
        clearTimeout(this.clickTimer);
        this.clickStatus = 0;
        window.open(this.props.data[index].file.viewLink, '_blank');
    },
    render: function() {
        var resultNodes = this.props.data.map(function(result, index) {
            var resultClass = classNames({
                'active':  this.state.activeIndex.indexOf(index) !== -1,
                'card pulse': true
            });
            var boundClick = this.resultHandleClick.bind(this, index);
            var boundDblClick = this.resultHandleDblClick.bind(this, index);
            return (
                <div key={result.id} className={resultClass}>
                    <a href="#" target="_blank" className="thumbnail-link" onClick={boundClick} onDoubleClick={boundDblClick}>
                        <div className="thumbnail" style={{ backgroundImage: 'url(' + result.file.thumbnailLink + ')' }}>
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