import { Label } from '@fluentui/react';
import { stateData } from '..';
import { CellRendererOverrides, CellRendererProps, GetRendererParams } from '../types';
import React = require('react');

export const cellRendererOverrides: CellRendererOverrides = {
    ["TwoOptions"]: (props: CellRendererProps, col: GetRendererParams) => renderControl(props, col),
}

function renderControl(props: CellRendererProps, col: GetRendererParams) {
    const columnName = col.colDefs[col.columnIndex].name.toLowerCase();
    if (stateData.Setting.attributes.length > 0 && stateData.Setting.attributes.indexOf(columnName) > -1) {
        return <Label>{props.formattedValue}</Label>;
    }

    return null;
}
