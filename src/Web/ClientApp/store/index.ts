import { ActionCreatorGeneric } from 'redux-typed';
import { Dispatch } from 'redux';

import * as DocumentStore from './Document';
import * as DistributionGroupStore from './DistributionGroup';
import * as UserErrorStore from './UserError';

export interface ApplicationState {
    distributionGroups: DistributionGroupStore.State
    , documents: DocumentStore.State
    , userErrors: UserErrorStore.State
}

export const reducers = {
    distributionGroups: DistributionGroupStore.reducer
    , documents: DocumentStore.reducer
    , userErrors: UserErrorStore.reducer
};

export interface TypedActionCreatorGeneric<TState, TReturn> extends Function {
    (dispatch: Dispatch<TState>, getState: () => TState): TReturn;
};

export type TypedActionCreator<TReturn> = TypedActionCreatorGeneric<ApplicationState, TReturn>;