export interface DistributionGroup {
    name: string;
    id: number;
    createdOn: string
    modifiedOn: string
}

export interface Document {
    abstract: string;
    id: number;
    selfLink: string;
    viewLink: string;
    thumbnailLink: string;
    title: string;
}