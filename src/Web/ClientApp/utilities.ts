export function getEntity<TEntity>(action: any, key: string): TEntity {
    if (action.normalized && action.normalized.entities && action.normalized.entities[key]) {
        return action.normalized.entities[key] as TEntity;
    }
    return null;
}