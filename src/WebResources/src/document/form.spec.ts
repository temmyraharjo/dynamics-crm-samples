import { expect } from 'chai';
import { XrmMockGenerator } from 'xrm-mock';
import { initFormCreate } from './form';
import { TestApiContext } from '@insurgo/niam.xrm.client.test';
import * as sinon from 'sinon';
 
describe('new_customdocument form tests', () => {
    beforeEach(() => {
        XrmMockGenerator.initialise();
        XrmMockGenerator.Attribute.createString('ins_documentnumber');
        XrmMockGenerator.Attribute.createDate('ins_date');
    });
 
    describe('on form update', () => {
        beforeEach(() => {
            sinon.stub(XrmMockGenerator.getFormContext().ui, 'getFormType').
                returns(XrmEnum.FormType.Update);
        });
 
        it('initFormCreate skip set', () => {
            initFormCreate(XrmMockGenerator.getEventContext());
 
            var formContext = XrmMockGenerator.getFormContext();
 
            expect(formContext.getAttribute('ins_documentnumber').getValue()).to.empty;
            expect(formContext.getAttribute('ins_date').getValue()).to.undefined;
        });
    });
 
    describe('on form create', () => {
        let testContext: TestApiContext;
        beforeEach(() => {
            sinon.stub(XrmMockGenerator.getFormContext().ui, 'getFormType').
                returns(XrmEnum.FormType.Create);
        });
 
        it('initFormCreate set name and document date', () => {
            initFormCreate(XrmMockGenerator.getEventContext());
 
            var formContext = XrmMockGenerator.getFormContext();
 
            expect(formContext.getAttribute('ins_documentnumber').getValue()).to.equal('New Document');
            expect(formContext.getAttribute('ins_date').getValue()).to.not.null;
        });
    });
});