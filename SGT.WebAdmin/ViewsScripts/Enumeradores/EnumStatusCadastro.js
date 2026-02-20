var EnumSatusCadastroHelper = function () {
    this.Cadastrado = 0;
    this.Pendente = 1;
    this.Treinado = 2;
    this.NaoSeAplica = 3;
};

EnumSatusCadastroHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Cadastrado: return "Cadastrado";
            case this.Pendente: return "Pendente";
            case this.Treinado: return "Treinado";
            case this.NaoSeAplica: return "Não se aplica";
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: this.obterDescricao(this.Cadastrado), value: this.Cadastrado },
            { text: this.obterDescricao(this.Pendente), value: this.Pendente },
            { text: this.obterDescricao(this.Treinado), value: this.Treinado },
            { text: this.obterDescricao(this.NaoSeAplica), value: this.NaoSeAplica }
        ];
    }
};

var EnumSatusCadastro = Object.freeze(new EnumSatusCadastroHelper());