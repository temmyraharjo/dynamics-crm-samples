import { IInputs, IOutputs } from "./generated/ManifestTypes";

import 'bootstrap';

interface IOptionSet {
    text: string;
    value: string;
}

class MultiCheckBox {
    constructor(
        private container: HTMLDivElement,
        private options: IOptionSet[],
        private currentValue: boolean | undefined,
        private onChangeEvent: (event: Event) => void) {
    }

    render(): HTMLSelectElement {
        const mainDiv = document.createElement('div');
        mainDiv.className = 'row';

        const ctrl = document.createElement('select');
        ctrl.id = 'dev_select';
        ctrl.className = 'form-select';
        ctrl.onchange = this.onChangeEvent;

        let defaultIndex = 0;
        this.options.forEach((item, index) => {
            const optCtrl = document.createElement('option');
            optCtrl.value = item.value;
            optCtrl.text = item.text;

            if (this.currentValue !== undefined && item.value !== '' && this.currentValue === !!JSON.parse(item.value)) {
                defaultIndex = index;
            }

            ctrl.appendChild(optCtrl);
        });

        ctrl.selectedIndex = defaultIndex;

        return ctrl;
    }

    updateView(value: boolean | undefined): void {
        this.currentValue = value;
        let defaultIndex = 0;
        this.options.forEach((item, index) => {
            if (this.currentValue !== undefined && item.value !== '' && this.currentValue === !!JSON.parse(item.value)) {
                defaultIndex = index;
            }
        });
        // @ts-ignore
        const ctrl = this.container.getElementById('dev_select') as HTMLSelectElement;
        ctrl.selectedIndex = defaultIndex;
    }
}

export class TwoOptionSetJs implements ComponentFramework.StandardControl<IInputs, IOutputs> {
    private _container: HTMLDivElement;
    private _notifyOutputChanged: () => void;
    private _options: IOptionSet[] = [];
    private _currentValue: boolean | undefined = undefined;
    private _control: MultiCheckBox;

    /**
     * Empty constructor.
     */
    constructor() {

    }

    /**
     * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
     * Data-set values are not initialized here, use updateView.
     * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
     * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
     * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
     * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
     */
    public async init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void,
        state: ComponentFramework.Dictionary, container: HTMLDivElement): Promise<void> {
        // Add control initialization code
        this._notifyOutputChanged = notifyOutputChanged;
        const mainAttribute = context.parameters.booleanProperty?.attributes?.LogicalName || '';
        this._currentValue = context.parameters.booleanProperty?.raw;
        // @ts-ignore - this is a hack to get the component to render
        const entityTypeName = context?.page?.entityTypeName;
        const metadata = await context.utils.getEntityMetadata(entityTypeName, [mainAttribute]);
        const options = metadata.Attributes.getByName(mainAttribute).OptionSet;
        this._options.push({ text: '-', value: '' });
        this._options.push(options[0]);
        this._options.push(options[1]);


        this._control = new MultiCheckBox(this._container, this._options, this._currentValue, (event: Event) => {
            const target = event.target as HTMLSelectElement;
            const value = target.value;
            this._currentValue = value === '' ? undefined : !!JSON.parse(value);
            this._notifyOutputChanged();
        });
        const ctrl = this._control.render();

        container.appendChild(ctrl);
    }


    /**
     * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
     * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
     */
    public updateView(context: ComponentFramework.Context<IInputs>): void {
        this._control.updateView(context.parameters.booleanProperty?.raw);
    }

    /**
     * It is called by the framework prior to a control receiving new data.
     * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
     */
    public getOutputs(): IOutputs {
        return { booleanProperty: this._currentValue };
    }

    /**
     * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
     * i.e. cancelling any pending remote calls, removing listeners, etc.
     */
    public destroy(): void {
        // Add code to cleanup control if necessary
    }
}
