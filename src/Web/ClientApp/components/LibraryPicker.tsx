import * as React from 'react';
import Select from 'react-select';
import { Library } from '../models';

import 'react-select/dist/react-select.css';

export type LibraryPickerStateProps = {
    selected: number[]
    , libraryOptions: Library[]
}

export type LibraryPickerDispatchProps = {
    onChange: (libraryIds: number[]) => void
}

type OwnProps = LibraryPickerDispatchProps & LibraryPickerStateProps;

const LibraryPicker = ({ selected, libraryOptions, onChange }: OwnProps) =>
    <Select
        multi
        value={selected}
        valueKey="id"
        labelKey="name"
        simpleValue
        options={libraryOptions}
        placeholder="All libraries"
        onChange={(groups: Library[]) => onChange(groups.map(group => group.id))}
    />;

export default LibraryPicker;