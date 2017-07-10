import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import { LibraryPicker, LibraryPickerDispatchProps, LibraryPickerStateProps } from '../components'

type OwnProps = {
    selected: number[]
    , onChange: (libraryIds: number[]) => void
}

const mapStateToProps = (state: ApplicationState, { selected }: OwnProps): LibraryPickerStateProps => ({
    selected
    , libraryOptions: state.entities.libraries.allIds.map(id => state.entities.libraries.byId[id])
})

const mapDispatchToProps = (dispatch: any, { onChange }: OwnProps): LibraryPickerDispatchProps => ({
    onChange
})

export default connect(mapStateToProps, mapDispatchToProps)(LibraryPicker);