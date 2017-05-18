import * as React from 'react';
import Select from 'react-select';
import { DistributionGroup } from '../models';

import 'react-select/dist/react-select.css';

export type LibraryPickerStateProps = {
    selected: number[]
    , libraryOptions: DistributionGroup[]
}

export type LibraryPickerDispatchProps = {
    onChange: (libraryIds: number[]) => void
}

const LibraryPicker = ({ selected, libraryOptions, onChange }: LibraryPickerStateProps & LibraryPickerDispatchProps) => {

    const handleLibraryChange = (groups: DistributionGroup[]) =>
        onChange(groups.map(group => group.id));

    return <Select multi value={selected} valueKey="id" labelKey="name"
        simpleValue options={libraryOptions} placeholder="All libraries"
        onChange={handleLibraryChange} />
}

export default LibraryPicker;