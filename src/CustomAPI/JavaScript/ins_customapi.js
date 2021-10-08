// Metadata
var Insurgo = Insurgo || {};
Insurgo.CustomApi = Insurgo.CustomApi || {};
var ACTION_TYPE = 0;

(function () {
    this.GetTeamsEmailRequest = function (teamName, isEmail) {
        this.TeamName = teamName;
        this.IsEmail = isEmail;
    };

    this.GetEnvironmentVariable = function (envName) {
        this.EnvironmentVariableName = envName;;
    };
}).apply(Insurgo.CustomApi);

Insurgo.CustomApi.GetTeamsEmailRequest.prototype.getMetadata = function () {
    return {
        boundParameter: null,
        parameterTypes: {
            TeamName: {
                typeName: "Edm.String",
                structuralProperty: 1
            },
            IsEmail: {
                typeName: "Edm.Boolean",
                structuralProperty: 1
            }
        },
        operationType: ACTION_TYPE,
        operationName: 'ins_getteamsemail'
    }
};

Insurgo.CustomApi.GetEnvironmentVariable.prototype.getMetadata = function () {
    return {
        boundParameter: null,
        parameterTypes: {
            EnvironmentVariableName: {
                typeName: "Edm.String",
                structuralProperty: 1
            }
        },
        operationType: ACTION_TYPE,
        operationName: 'ins_getenvironmentvariable'
    }
};
// End of Metadata


// Execute Request

var req = new Insurgo.CustomApi.GetTeamsEmailRequest('Testing Team', true);
Xrm.WebApi.execute(req).then(success => success.json()).then(success => console.log(success));

var req1 = new Insurgo.CustomApi.GetEnvironmentVariable('msdyn_IncidentShouldValidatePrimaryContact');
Xrm.WebApi.execute(req1).then(success => success.json()).then(success => console.log(success));

// End of Execute Request