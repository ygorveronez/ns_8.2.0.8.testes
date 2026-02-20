var EnumTipoManobraAcaoHelper = function () {
    this.Todos = "";
    this.NaoInformado = 0;
    this.InicioCarregamento = 1;
    this.Checklist = 2;
    this.Higienizacao = 3;
}

EnumTipoManobraAcaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: "Checklist", value: this.Checklist },
            { text: "Higienização", value: this.Higienizacao },
            { text: "Início de Carregamento", value: this.InicioCarregamento },
            { text: "Não Informado", value: this.NaoInformado }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: "Todos", value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoManobraAcao = Object.freeze(new EnumTipoManobraAcaoHelper());