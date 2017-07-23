import * as React from 'react';
import { ContextMenu, MenuItem } from 'react-contextmenu';
import { connectMenu } from 'react-contextmenu/src/connectMenu';

export type DocumentContextMenuStateProps = {
    id: string
}

export type DocumentContextMenuDispatchProps = {
    trigger: {
        onItemClick: (e: React.SyntheticEvent<any>, data: { action: string, id: number }, target:any) => void
    }
}

type OwnProps = DocumentContextMenuDispatchProps & DocumentContextMenuStateProps;

const DocumentContextMenu = ({ id, trigger }: OwnProps) => {
    const handleItemClick = trigger ? trigger.onItemClick : null;
    return <ContextMenu
        id={id}
        className="menu"
    >
        <MenuItem
            data={{ action: 'edit', id: 0 }}
            onClick={handleItemClick}
        >
            <i className="fa fa-pencil"></i> Edit
        </MenuItem>
        <MenuItem
            divider
        />
        <MenuItem
            data={{ action: 'delete', id: 0 }}
            onClick={handleItemClick}
        >
            <i className="fa fa-trash"></i> Delete
        </MenuItem>
    </ContextMenu>
}

export default connectMenu('documentContextMenu')(DocumentContextMenu);