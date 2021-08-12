export type ins_document = {
    ins_date: Date;
    ins_description: string;
    ins_documentnumber: string;
    ins_ownerid: Xrm.LookupValue[];
    ins_securityid: Xrm.LookupValue[];
    ins_type: number;
    ownerid: Xrm.LookupValue[];
}

export enum document_type_enum {
    invoice = 100000000,
    orderlist,
    shippinglist
}