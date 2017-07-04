import { getJson } from '../../fetchHelpers';
import { typeName, isActionType, Action, Reducer } from 'redux-typed';
import { TypedActionCreator } from '../';
import { Library, NormalizedLibraries } from '../../models';
import { getEntity, normalize } from '../../utilities';

// -----------------
// STATE

type ByIdRecordset = { [id: number]: Library }

export interface State {
    byId: ByIdRecordset
    , allIds: number[]
}

// ----------------
// ACTIONS

@typeName("LIBRARIES_REQUESTED")
class LibrariesRequested extends Action {
    constructor() {
        super();
    }
}
@typeName("LIBRARIES_FAILED")
class LibrariesFailed extends Action {
    constructor(public error: Error) {
        super();
    }
}
@typeName("LIBRARIES_SUCCESS")
class LibrariesSuccess extends Action {
    constructor(public normalized: NormalizedLibraries) {
        super();
    }
}

export const actionCreators = {
    fetch: (): TypedActionCreator<Promise<number[]>> => (dispatch, getState) => {
        dispatch(new LibrariesRequested());
        return getJson(`/api/profile/distributiongroups/`, {})
            .then((data: Library[]) => {
                const normalized = normalize.libraries(data);
                dispatch(new LibrariesSuccess(normalized));
                return normalized.result;
            })
            .catch((error: Error) => {
                dispatch(new LibrariesFailed(error));
                return undefined;
            });
    }
};

// ----------------
// REDUCER

const unloadedState: State = {
    byId: {}
    , allIds: []
}

const addEntities = (state: State, documents: ByIdRecordset): State => {
    const allIds = [...state.allIds];
    const byId = { ...state.byId };

    Object.keys(documents)
        .forEach((key, index) => {

            const library: Library = documents[key];

            if (!byId[library.id]) {
                allIds.push(library.id);
            }

            byId[library.id] = library;

        });

    return {
        byId
        , allIds
    }
}

export const reducer: Reducer<State> = (state, action: any) => {

    if (getEntity<ByIdRecordset>(action, 'libraries')) {
        return addEntities(state, getEntity<ByIdRecordset>(action, 'libraries'));
    }

    return state || unloadedState;
};