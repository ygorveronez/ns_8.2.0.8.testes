var EnumTipoComunicacaoIntegracaoHelper = function () {
    this.WebService = 1;
    this.WebServiceREST = 2;
    this.WebServiceSOAP = 3;
    this.DatabaseMSSQL = 4;
    this.DatabaseMySQL = 5;
    this.DatabaseOracle = 6;
    this.DatabasePostgreSQL = 7;
    this.ActiveMQ = 8;
};

EnumTipoComunicacaoIntegracaoHelper.prototype = {
    ObterOpcoes: function () {
        return [
            { text: "Web Service", value: this.WebService },
            { text: "Web Service REST", value: this.WebServiceREST },
            { text: "Web Service SOAP", value: this.WebServiceSOAP },
            { text: "Database MS SQL Server", value: this.DatabaseMSSQL },
            { text: "Database MySql", value: this.DatabaseMySQL },
            { text: "Database Oracle", value: this.DatabaseOracle },
            { text: "Database PostgreSQL", value: this.DatabasePostgreSQL },
            { text: "ActiveMQ", value: this.ActiveMQ }
        ];
    },
    ObterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: "" }].concat(this.ObterOpcoes());
    }
};

var EnumTipoComunicacaoIntegracao = Object.freeze(new EnumTipoComunicacaoIntegracaoHelper());