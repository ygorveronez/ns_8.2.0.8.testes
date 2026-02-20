var EnumDiaSemanaHelper = function () {
    this.Todos = "";
    this.Domingo = 1;
    this.Segunda = 2;
    this.Terca = 3;
    this.Quarta = 4;
    this.Quinta = 5;
    this.Sexta = 6;
    this.Sabado = 7;
};

EnumDiaSemanaHelper.prototype = {
    obterDescricaoResumida: function (diaSemana) {
        switch (diaSemana) {
            case this.Domingo: return Localization.Resources.Enumeradores.DiaSemana.Domingo.toLowerCase();
            case this.Segunda: return Localization.Resources.Enumeradores.DiaSemana.Segunda.toLowerCase();
            case this.Terca: return Localization.Resources.Enumeradores.DiaSemana.Terca.toLowerCase();
            case this.Quarta: return Localization.Resources.Enumeradores.DiaSemana.Quarta.toLowerCase();
            case this.Quinta: return Localization.Resources.Enumeradores.DiaSemana.Quinta.toLowerCase();
            case this.Sexta: return Localization.Resources.Enumeradores.DiaSemana.Sexta.toLowerCase();
            case this.Sabado: return Localization.Resources.Enumeradores.DiaSemana.Sabado.toLowerCase();
            default: return "";
        }
    },
    obterDescricaoSemConfiguracao: function (diaSemana) {
        switch (diaSemana) {
            case this.Domingo: return Localization.Resources.Enumeradores.DiaSemana.Domingo;
            case this.Segunda: return Localization.Resources.Enumeradores.DiaSemana.SegundaFeira;
            case this.Terca: return Localization.Resources.Enumeradores.DiaSemana.TercaFeira;
            case this.Quarta: return Localization.Resources.Enumeradores.DiaSemana.QuartaFeira ;
            case this.Quinta: return Localization.Resources.Enumeradores.DiaSemana.QuintaFeira;
            case this.Sexta: return Localization.Resources.Enumeradores.DiaSemana.SextaFeira;
            case this.Sabado: return Localization.Resources.Enumeradores.DiaSemana.Sabado;
            default: return Localization.Resources.Enumeradores.DiaSemana.SemConfiguracao;
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.DiaSemana.SegundaFeira, value: this.Segunda },
            { text: Localization.Resources.Enumeradores.DiaSemana.TercaFeira, value: this.Terca },
            { text: Localization.Resources.Enumeradores.DiaSemana.QuartaFeira, value: this.Quarta },
            { text: Localization.Resources.Enumeradores.DiaSemana.QuintaFeira, value: this.Quinta },
            { text: Localization.Resources.Enumeradores.DiaSemana.SextaFeira, value: this.Sexta },
            { text: Localization.Resources.Enumeradores.DiaSemana.Sabado, value: this.Sabado },
            { text: Localization.Resources.Enumeradores.DiaSemana.Domingo, value: this.Domingo }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.DiaSemana.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesSemConfiguracao: function () {
        return [{ text: Localization.Resources.Enumeradores.DiaSemana.SemConfiguracao, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumDiaSemana = Object.freeze(new EnumDiaSemanaHelper());