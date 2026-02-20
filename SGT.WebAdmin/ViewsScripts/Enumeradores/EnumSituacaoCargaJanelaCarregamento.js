var EnumSituacaoCargaJanelaCarregamentoHelper = function () {
    this.Todas = "";
    this.AgAprovacaoComercial = 1;
    this.SemValorFrete = 2;
    this.SemTransportador = 3;
    this.AgConfirmacaoTransportador = 4;
    this.ProntaParaCarregamento = 5;
    this.ReprovacaoComercial = 6;
    this.AgLiberacaoParaTransportadores = 7;
    this.LiberarAutomaticamenteFaturamento = 8;
    this.AgEncosta = 9;
    this.AgAceiteTransportador = 10;
};

EnumSituacaoCargaJanelaCarregamentoHelper.prototype = {
    obterClasseCor: function (situacao) {
        switch (situacao) {
            case this.AgAceiteTransportador: return "well-burlywood";
            case this.AgAprovacaoComercial: return "well-white";
            case this.AgConfirmacaoTransportador: return "well-orange";
            case this.AgEncosta: return "well-purple";
            case this.AgLiberacaoParaTransportadores: return "well-darkBlue";
            case this.ProntaParaCarregamento: return "well-green";
            case this.ReprovacaoComercial: return "well-red";
            case this.SemTransportador: return "well-blue";
            case this.SemValorFrete: return "well-yellow";
            default: return "";
        }
    },
    obterOpcoes: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoAprovacaoComercial, value: this.AgAprovacaoComercial });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoConfirmacaoTransportador, value: this.AgConfirmacaoTransportador });

        if (_CONFIGURACAO_TMS.UtilizarFilaCarregamento)
            opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoEncosta, value: this.AgEncosta });

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoLiberacaoParaTransportadores, value: this.AgLiberacaoParaTransportadores });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.LiberadaParaFaturamento, value: this.LiberarAutomaticamenteFaturamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, value: this.ProntaParaCarregamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.ReprovacaoComercial, value: this.ReprovacaoComercial });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.SemTransportador, value: this.SemTransportador });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.SemValorDeFrete, value: this.SemValorFrete });

        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.Todas, value: this.Todas }].concat(this.obterOpcoes());
    },
    obterOpcoesPesquisaJanelaCarregamento: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoConfirmacaoTransportador, value: this.AgConfirmacaoTransportador });

        if (_CONFIGURACAO_TMS.UtilizarFilaCarregamento)
            opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoEncosta, value: this.AgEncosta });

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoLiberacaoParaTransportadores, value: this.AgLiberacaoParaTransportadores });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.Faturada, value: 99 }); // A situação não existe. Adicionado um valor diferente de "" (Todas) para possibilitar a identificação desta situação ao pesquisar
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.NaoFaturada, value: 98 }); // A situação não existe. Adicionado um valor diferente de "" (Todas) para possibilitar a identificação desta situação ao pesquisar
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, value: this.ProntaParaCarregamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.SemTransportador, value: this.SemTransportador });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.SemValorDeFrete, value: this.SemValorFrete });

        return opcoes;
    },
    obterOpcoesPesquisaRelatorio: function () {
        var opcoes = [];

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoAprovacaoComercial, value: this.AgAprovacaoComercial });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoConfirmacaoTransportador, value: this.AgConfirmacaoTransportador });

        if (_CONFIGURACAO_TMS.UtilizarFilaCarregamento)
            opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoEncosta, value: this.AgEncosta });

        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.AguardandoLiberacaoParaTransportadores, value: this.AgLiberacaoParaTransportadores });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.Faturada, value: 99 }); // A situação não existe. Adicionado um valor diferente de "" (Todas) para possibilitar a identificação desta situação ao pesquisar
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.NaoFaturada, value: 98 }); // A situação não existe. Adicionado um valor diferente de "" (Todas) para possibilitar a identificação desta situação ao pesquisar
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.LiberadaParaFaturamento, value: this.LiberarAutomaticamenteFaturamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.ProntaParaCarregamento, value: this.ProntaParaCarregamento });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.ReprovacaoComercial, value: this.ReprovacaoComercial });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.SemTransportador, value: this.SemTransportador });
        opcoes.push({ text: Localization.Resources.Enumeradores.SituacaoCargaJanelaCarregamento.SemValorDeFrete, value: this.SemValorFrete });

        return opcoes;
    }
};

var EnumSituacaoCargaJanelaCarregamento = Object.freeze(new EnumSituacaoCargaJanelaCarregamentoHelper());