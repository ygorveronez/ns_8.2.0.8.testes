var EnumCarroceriaVeiculoHelper = function () {
    this.GradeBaixa = 1;
    this.GradeAlta = 2;
    this.Sider = 3;
    this.Bau = 4;
    this.GradeBaixaEOuSider = 5;
    this.SiderEOuBau = 6;
    this.Silo = 7;
    this.Basculante = 8;
};

EnumCarroceriaVeiculoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Grade Baixa", value: this.GradeBaixa },
            { text: "Grade Alta", value: this.GradeAlta },
            { text: "Sider", value: this.Sider },
            { text: "Baú", value: this.Bau },
            { text: "Grade baixa e/ou Sider", value: this.GradeBaixaEOuSider },
            { text: "Sider e/ou Baú", value: this.SiderEOuBau },
            { text: "Silo", value: this.Silo },
            { text: "Basculante", value: this.Basculante },
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Gerais.Geral.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
}

var EnumCarroceriaVeiculo = Object.freeze(new EnumCarroceriaVeiculoHelper());