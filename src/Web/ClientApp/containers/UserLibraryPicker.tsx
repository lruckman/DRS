import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { LibraryPicker, LibraryPickerStateProps, LibraryPickerDispatchProps } from '../components'

type ContainerProps = {
    selected: number[]
    , onChange: (libraryIds: number[]) => void
}

const mapStateToProps = (state: ApplicationState, { selected }: ContainerProps): LibraryPickerStateProps => ({
    selected
    , libraryOptions: state.distributionGroups.allIds.map(id => state.distributionGroups.byId[id])
})

const mapDispatchToProps = (dispatch: any, { onChange }: ContainerProps): LibraryPickerDispatchProps => ({
    onChange
})

export default connect(mapStateToProps, mapDispatchToProps)(LibraryPicker);