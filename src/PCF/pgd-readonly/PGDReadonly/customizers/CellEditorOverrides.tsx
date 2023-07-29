import { CellEditorOverrides, CellEditorProps, GetEditorParams } from '../types';
import { stateData } from '../index';

export const cellEditorOverrides: CellEditorOverrides = {
    ["Text"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Email"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Phone"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Ticker"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["URL"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["TextArea"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Lookup"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Customer"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Owner"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["MultiSelectPicklist"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["OptionSet"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["TwoOptions"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Duration"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Language"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Multiple"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["TimeZone"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Integer"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Currency"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Decimal"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["FloatingPoint"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["AutoNumber"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["DateOnly"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["DateAndTime"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Image"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["File"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["Persona"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["RichText"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col),
    ["UniqueIdentifier"]: (props: CellEditorProps, col: GetEditorParams) => renderControl(props, col)
}

function renderControl(_props: CellEditorProps, col: GetEditorParams) {
    if (stateData.Settings.length > 0 && stateData.Settings[0].attributes.indexOf(col.colDefs[col.columnIndex].name.toLowerCase()) > -1) {
        col.stopEditing(true);
    }

    return null;
}