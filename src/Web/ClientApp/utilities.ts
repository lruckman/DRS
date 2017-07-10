import { normalize as _normalize, schema } from 'normalizr';
import { DocumentFile, Library, NormalizedDocuments, NormalizedLibraries } from './models';

export function getEntity<TEntity>(action: any, key: string): TEntity {
    if (action.normalized && action.normalized.entities && action.normalized.entities[key]) {
        return action.normalized.entities[key] as TEntity;
    }
    return null;
}

export const normalize = {
    documents: (data: DocumentFile[]): NormalizedDocuments =>
        _normalize(data, [new schema.Entity('documents')])
    , libraries: (data: Library[]): NormalizedLibraries =>
        _normalize(data, [new schema.Entity('libraries')])
}