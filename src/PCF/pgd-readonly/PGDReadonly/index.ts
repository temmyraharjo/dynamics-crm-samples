import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { cellRendererOverrides } from "./customizers/CellRendererOverrides";
import { cellEditorOverrides } from "./customizers/CellEditorOverrides";
import { PAOneGridCustomizer, SettingModel } from "./types";
import * as React from "react";

const stateData: { Setting: SettingModel, entity: string} = { Setting: { entity: '', attributes: [] }, entity: ''};
export class PGDReadonly implements ComponentFramework.ReactControl<IInputs, IOutputs> {
    /**
     * Empty constructor.
     */
    constructor() { }

    /**
     * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
     * Data-set values are not initialized here, use updateView.
     * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
     * @param _notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
     * @param _state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
     */
    public async init(
        context: ComponentFramework.Context<IInputs>,
        _notifyOutputChanged: () => void,
        _state: ComponentFramework.Dictionary
    ): Promise<void> {
        const contextObj: any = context;
        stateData.entity = contextObj.client._customControlProperties.pageType === 'EntityList' ?
            (contextObj.page?.entityTypeName ?? '') : (contextObj.client._customControlProperties.descriptor.Parameters?.TargetEntityType ?? '');

        const environmentVariableName = 'Grid Customizer';
        const result = await context.webAPI.retrieveMultipleRecords("environmentvariabledefinition", `?$filter=displayname eq '${environmentVariableName}'&$select=environmentvariabledefinitionid&$expand=environmentvariabledefinition_environmentvariablevalue($select=value)`);
        const current = result && result.entities ? result.entities[0] : {};
        const value = current['environmentvariabledefinition_environmentvariablevalue'] ? current['environmentvariabledefinition_environmentvariablevalue'][0].value : null;
        const models: SettingModel[] = value ? JSON.parse(value) : [];
        const filterSetting = models.filter(e => e.entity == stateData.entity);
        stateData.Setting = filterSetting.length > 0 ? filterSetting[0] : { entity: '', attributes: [] };

        const eventName = context.parameters.EventName.raw;
        if (eventName) {
            const paOneGridCustomizer: PAOneGridCustomizer = { cellRendererOverrides, cellEditorOverrides };
            contextObj.factory.fireEvent(eventName, paOneGridCustomizer);
        }
    }

    /**
     * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
     * @param _context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
     * @returns ReactElement root react element for the control
     */
    public updateView(_context: ComponentFramework.Context<IInputs>): React.ReactElement {
        return React.createElement(React.Fragment);
    }

    /**
     * It is called by the framework prior to a control receiving new data.
     * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
     */
    public getOutputs(): IOutputs {
        return {};
    }

    /**
     * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
     * i.e. cancelling any pending remote calls, removing listeners, etc.
     */
    public destroy(): void {
        // Add code to cleanup control if necessary
    }
}

export { stateData };
