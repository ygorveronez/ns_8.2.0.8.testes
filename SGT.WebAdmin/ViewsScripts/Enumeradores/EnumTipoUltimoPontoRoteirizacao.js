var EnumTipoUltimoPontoRoteirizacaoHelper = function () {
    this.Todos = "";
    this.Retornando = 1;
    this.AteOrigem = 2;
    this.PontoMaisDistante = 3;
};

EnumTipoUltimoPontoRoteirizacaoHelper.prototype = {
    obterDescricao: function (tipo) {
        switch (tipo) {
            case this.Retornando: return Localization.Resources.Enumeradores.TipoUltimoPontoRoteirizacao.RetornoVazio;
            case this.AteOrigem: return Localization.Resources.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem;
            case this.PontoMaisDistante: return Localization.Resources.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem, value: this.AteOrigem },
            { text: Localization.Resources.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante, value: this.PontoMaisDistante },
            { text: Localization.Resources.Enumeradores.TipoUltimoPontoRoteirizacao.RetornoVazio, value: this.Retornando }
        ];
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoUltimoPontoRoteirizacao.Todos, value: this.Todos }].concat(this.obterOpcoes());
    },
    obterOpcoesNaoSelecionado: function () {
        return [{ text: Localization.Resources.Gerais.Geral.NaoSelecionado, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumTipoUltimoPontoRoteirizacao = Object.freeze(new EnumTipoUltimoPontoRoteirizacaoHelper());
