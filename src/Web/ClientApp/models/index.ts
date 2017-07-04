export interface DocumentFile {
    abstract: string
    , id: number
    , thumbnailLink: string
    , title: string
    , createdOn: string
    , modifiedOn: string
    , pageCount: number
    , size: number
    , version: number
}

export interface Library {
    id: number
    , createdOn: Date
    , modifiedOn: Date
    , name: string
}

export interface DocumentSearchResults {
    documents: DocumentFile[];
    nextLink: string;
    totalCount: number
}


export interface NormalizedDocuments {
    entities: {
        documents: {
            [id: number]: DocumentFile
        }
    }
    , result: number[]
}

export interface NormalizedLibraries {
    entities: {
        libraries: {
            [id: number]: Library
        }
    }
    , result: number[]
}

export interface ModelError {
    key: string;
    value: string[];
}