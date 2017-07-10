import * as React from 'react';
import moment from 'moment';

export type UpdatingDateLabelStateProps = {
    date: string
}

export type UpdatingDateLabelDispatchProps = {}

type OwnState = {
    fromNow: string
}

type OwnProps = UpdatingDateLabelStateProps & UpdatingDateLabelDispatchProps;

class UpdatingDateLabel extends React.Component<OwnProps, OwnState> {

    interval = null;

    constructor(props: OwnProps) {
        super(props);
        const { date } = this.props;
        this.state = {
            fromNow: moment(props.date).fromNow()
        }
    }

    componentWillReceiveProps(newProps: OwnProps) {
        if (newProps.date !== this.props.date) {
            this.setState({
                fromNow: moment(newProps.date).fromNow()
            });
        }
    }

    componentDidMount() {
        const { date } = this.props;
        this.interval = setInterval(() => this.setState({ fromNow: moment(date).fromNow() }), 60000);
    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }

    public render() {
        const { fromNow } = this.state;
        const { date } = this.props;
        return <span title={moment(date).format('MMMM Do YYYY, h:mm:ss a')}>
            {fromNow}
        </span>
    }
}

export default UpdatingDateLabel;