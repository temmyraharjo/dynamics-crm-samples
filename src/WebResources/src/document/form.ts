import { ins_document } from '../entities';
import { Fx } from '@insurgo/niam.xrm.client';

export function formLoaded(context: Xrm.Events.EventContext) {
    initFormCreate(context);
}

export function initFormCreate(context: Xrm.Events.EventContext) {
    const fx = new Fx<ins_document>(context);
    if (fx.formContext.ui.getFormType() !== XrmEnum.FormType.Create) return;

    fx.set('ins_documentnumber', 'New Document');
    fx.set('ins_date', new Date());
}