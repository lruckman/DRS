export interface Library {
    text: string;
    value: string;
}

export interface DistributionGroup {
    name: string;
    id: string;
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