export interface IDocumentDetails {
    abstract: string;
    createdOn: string;
    file: {
        createdOn: string;
        modifiedOn: string;
        pageCount: number;
        size: number;
        thumbnailLink: string;
        versionNum: number;
    },
    libraryIds: number[];
    location: string;
    modifiedOn: string;
    title: string;
}