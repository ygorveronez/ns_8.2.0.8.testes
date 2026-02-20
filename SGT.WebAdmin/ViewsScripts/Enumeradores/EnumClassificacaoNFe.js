var EnumClassificacaoNFeHelper = function () {
    this.Todos = -1;
    this.SemClassificacao = 0;
    this.Revenda = 1;
    this.NaoRevenda = 2;
    this.NFEletronicos = 3;
    this.GrandesVolumes = 4;
    this.MateriaPrima = 5;
    this.Retira = 6;
    this.VM = 7;
    this.Remessa = 8;
    this.Venda = 9;
};

EnumClassificacaoNFeHelper.prototype = {
    obterOpcoes: function () {
        var opcoes = [
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.SemClassificacao, value: this.SemClassificacao },
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.Revenda, value: this.Revenda },
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.NaoRevenda, value: this.NaoRevenda },
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.NFEletronicos, value: this.NFEletronicos },
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.Retira, value: this.Retira },
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.VM, value: this.VM },
        ];

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
            opcoes.push(
                { text: Localization.Resources.Enumeradores.ClassificacaoNFe.GrandesVolumes, value: this.GrandesVolumes },
                { text: Localization.Resources.Enumeradores.ClassificacaoNFe.MateriaPrima, value: this.MateriaPrima });

        return opcoes;
    },
    obterOpcoesTipoOperacaoRemessaVenda: function () {
        var opcoes = [
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.SemClassificacao, value: this.SemClassificacao },
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.Remessa, value: this.Remessa },
            { text: Localization.Resources.Enumeradores.ClassificacaoNFe.Venda, value: this.Venda },
        ];
        return opcoes;
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.ClassificacaoNFe.Todos, value: this.Todos }].concat(this.obterOpcoes());
    }
};

var EnumClassificacaoNFe = Object.freeze(new EnumClassificacaoNFeHelper());