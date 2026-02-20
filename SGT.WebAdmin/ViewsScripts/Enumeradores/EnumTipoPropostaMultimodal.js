var EnumTipoPropostaMultimodalHelper = function () {
    this.Todos = -1;
    this.Nenhum = 0;
    this.CargaFechada = 1;
    this.CargaFracionada = 2;
    this.Feeder = 3;
    this.VAS = 4;
    this.TakePayFeeder = 5;
    this.TakePayCabotagem = 6;
    this.NoShowCabotagem = 7;
    this.FaturamentoContabilidade = 8;
    this.DemurrageCabotagem = 9;
    this.DetentionCabotagem = 10;
    this.NotaDebito = 11
};

EnumTipoPropostaMultimodalHelper.prototype = {
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Nenhum: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.Nenhum;
            case this.CargaFechada: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoventaTresCargaFechada;
            case this.CargaFracionada: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoventaQuatroCargaFracionada;
            case this.Feeder: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoventaCindoFeeder;
            case this.VAS: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.NovenaSeisVAS;
            case this.TakePayFeeder: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoFeeder;
            case this.TakePayCabotagem: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoCabotagem;
            case this.NoShowCabotagem: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem;
            case this.FaturamentoContabilidade: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade;
            case this.DemurrageCabotagem: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem;
            case this.DetentionCabotagem: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem;
            case this.NotaDebito: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.NotaDebito;
            case this.Todos: return Localization.Resources.Enumeradores.TipoPropostaMultimodal.Todos;
            default: return "";
        }
    },
    obterOpcoes: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.Nenhum, value: this.Nenhum }].concat(this.obterOpcoesPerfil());
    },
    obterOpcoesPesquisa: function () {
        return [{ text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.Todos, value: this.Todos }].concat(this.obterOpcoesPerfil());
    },
    obterOpcoesPerfil: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoventaTresCargaFechada, value: this.CargaFechada },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoventaQuatroCargaFracionada, value: this.CargaFracionada },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoventaCindoFeeder, value: this.Feeder },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NovenaSeisVAS, value: this.VAS },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoFeeder, value: this.TakePayFeeder },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoCabotagem, value: this.TakePayCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem, value: this.NoShowCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade, value: this.FaturamentoContabilidade },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem, value: this.DemurrageCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem, value: this.DetentionCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NotaDebito, value: this.NotaDebito },
        ];
    },
    obterOpcoesSemNumero: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.CargaFechada, value: this.CargaFechada },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.CargaFracionada, value: this.CargaFracionada },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.Feeder, value: this.Feeder },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.VAS, value: this.VAS },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoFeeder, value: this.TakePayFeeder },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoCabotagem, value: this.TakePayCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem, value: this.NoShowCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade, value: this.FaturamentoContabilidade },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem, value: this.DemurrageCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem, value: this.DetentionCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NotaDebito, value: this.NotaDebito },
        ];
    },
    obterOpcoesTakePay: function () {
        return [
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoFeeder, value: this.TakePayFeeder },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.EmbarqueCertoCabotagem, value: this.TakePayCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem, value: this.NoShowCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade, value: this.FaturamentoContabilidade },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem, value: this.DemurrageCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem, value: this.DetentionCabotagem },
            { text: Localization.Resources.Enumeradores.TipoPropostaMultimodal.NotaDebito, value: this.NotaDebito },
        ];
    },
};

var EnumTipoPropostaMultimodal = Object.freeze(new EnumTipoPropostaMultimodalHelper());