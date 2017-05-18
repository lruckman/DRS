import { getJson } from '../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from './';
import { DistributionGroup } from '../models';

// -----------------
// STATE

export interface State {
    byId: { [id: number]: DistributionGroup }
    , allIds: number[]
}

// ----------------
// ACTIONS

@typeName("DISTRIBUTION_GROUPS_FETCH")
class DistributionGroupFetch extends Action {
    constructor() {
        super();
    }
}
@typeName("DISTRIBUTION_GROUPS_FETCH_FAILURE")
class DistributionGroupFetchFailure extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("DISTRIBUTION_GROUPS_FETCH_SUCCESS")
class DistributionGroupFetchSuccess extends Action {
    constructor(public data: DistributionGroup[]) {
        super();
    }
}

export const actionCreators = {
    fetch: (): TypedActionCreator<Promise<DistributionGroup[]>> => (dispatch, getState) => {
        dispatch(new DistributionGroupFetch());
        return getJson(`/api/profile/distributiongroups/`, {})
            .then((data: DistributionGroup[]) => dispatch(new DistributionGroupFetchSuccess(data)))
            .catch((error: Error) => dispatch(new DistributionGroupFetchFailure(error)));
    }
};

// ----------------
// REDUCER

const unloadedState: State = {
    byId: {}
    , allIds: []
}

const addDistributionGroups = (state: State, groups: { [id: number]: DistributionGroup }): State => {
    const allIds: number[] = [...state.allIds];
    const byId: { [id: number]: DistributionGroup } = { ...state.byId };

    Object.keys(groups)
        .forEach((key, index) => {

            const group: DistributionGroup = groups[key];

            if (!byId[group.id]) {
                allIds.push(group.id);
            }

            byId[group.id] = group;

        });

    return {
        ...state
        , byId
        , allIds
    }
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (action.normalized && action.normalized.entities && action.normalized.entities.distributionGroups) {
        return addDistributionGroups(state, action.normalized.entities.distributionGroups);
    }

    return state || unloadedState;
};