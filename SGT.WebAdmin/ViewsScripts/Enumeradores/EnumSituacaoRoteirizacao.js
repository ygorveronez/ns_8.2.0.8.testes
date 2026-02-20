var EnumSituacaoRoteirizacaoHelper = function () {
    this.Todas = -1;
    this.SemDefinicao = 0;
    this.Aguardando = 1;
    this.Concluido = 2;
    this.Erro = 3;
    this.EmZonaExclusao = 4;
};

EnumSituacaoRoteirizacaoHelper.prototype = {
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.SituacaoRoteirizacao.Todas, value: this.Todas },
            { text: Localization.Resources.Enumeradores.SituacaoRoteirizacao.SemDefinicao, value: this.SemDefinicao },
            { text: Localization.Resources.Enumeradores.SituacaoRoteirizacao.Aguardando, value: this.Aguardando },
            { text: Localization.Resources.Enumeradores.SituacaoRoteirizacao.Concluido, value: this.Concluido },
            { text: Localization.Resources.Enumeradores.SituacaoRoteirizacao.Erro, value: this.Erro },
            { text: Localization.Resources.Enumeradores.SituacaoRoteirizacao.EmZonaExclusao, value: this.EmZonaExclusao }
        ];
    },
}

var EnumSituacaoRoteirizacao = Object.freeze(new EnumSituacaoRoteirizacaoHelper());