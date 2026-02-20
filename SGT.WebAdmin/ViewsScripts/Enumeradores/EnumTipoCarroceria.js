var EnumTipoCarroceriaHelper = function () {
    this.Todos = 99;
    this.NaoAplicavel = 0;
    this.Aberta = 1;
    this.FechadaBau = 2;
    this.Graneleira = 3;
    this.PortaContainer = 4;
    this.Utilitario = 5;
    this.Sider = 6;
};

EnumTipoCarroceriaHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Não Aplicável", value: this.NaoAplicavel },
            { text: "Aberta", value: this.Aberta },
            { text: "Fechada/Baú", value: this.FechadaBau },
            { text: "Graneleira", value: this.Graneleira },
            { text: "Container", value: this.PortaContainer },
            { text: "Utilitário", value: this.Utilitario },
            { text: "Sider", value: this.Sider }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoCarroceria = Object.freeze(new EnumTipoCarroceriaHelper());