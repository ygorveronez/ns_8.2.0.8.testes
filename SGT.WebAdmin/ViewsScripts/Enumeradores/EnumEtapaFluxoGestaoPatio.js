var EnumEtapaFluxoGestaoPatioHelper = function () {
    this.Todas = 0;
    this.InformarDoca = 5;
    this.ChegadaVeiculo = 9;
    this.Guarita = 10;
    this.CheckList = 20;
    this.TravamentoChave = 30;
    this.Expedicao = 40;
    this.LiberacaoChave = 50;
    this.Faturamento = 60;
    this.InicioViagem = 70;
    this.Posicao = 80;
    this.Entregas = 81;
    this.ChegadaLoja = 90;
    this.DeslocamentoPatio = 91;
    this.SaidaLoja = 100;
    this.FimViagem = 110;
    this.InicioHigienizacao = 120;
    this.FimHigienizacao = 130;
    this.InicioCarregamento = 140;
    this.FimCarregamento = 150;
    this.SolicitacaoVeiculo = 160;
    this.InicioDescarregamento = 170;
    this.FimDescarregamento = 180;
    this.DocumentoFiscal = 190;
    this.DocumentosTransporte = 200;
    this.MontagemCarga = 210;
    this.SeparacaoMercadoria = 220;
    this.AvaliacaoDescarga = 230;
};

EnumEtapaFluxoGestaoPatioHelper.prototype = {
    isEtapaHabilitadaPreCarga: function (etapa) {
        var etapasHabilitadas = this.obterEtapasHabilitadasPreCarga();

        return etapasHabilitadas.indexOf(etapa) > -1;
    },
    obterDescricao: function (valor) {
        switch (valor) {
            case this.Todas: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Todas;
            case this.AvaliacaoDescarga: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.AvaliacaoDescarga;
            case this.CheckList: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.CheckList;
            case this.ChegadaVeiculo: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo;
            case this.Guarita: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.ChegadaPortaria;
            case this.ChegadaLoja: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.ChegadaDestinatario;
            case this.DeslocamentoPatio: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.DeslocamentoPatio;
            case this.InformarDoca: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Doca;
            case this.DocumentoFiscal: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.DocumentoFiscal;
            case this.DocumentosTransporte: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.DocumentosTransporte;
            case this.Entregas: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Entregas;
            case this.Expedicao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Expedicao;
            case this.Faturamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Faturamento;
            case this.FimHigienizacao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimHigienizacao;
            case this.FimViagem: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimViagem;
            case this.FimCarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimCarregamento;
            case this.FimDescarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.FimDescarregamento;
            case this.LiberacaoChave: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.LiberacaoChave;
            case this.InicioHigienizacao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioHigienizacao;
            case this.InicioViagem: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem;
            case this.InicioCarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioCarregamento;
            case this.InicioDescarregamento: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.InicioDescarregamento;
            case this.MontagemCarga: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.MontagemCarga;
            case this.Posicao: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.Posicao;
            case this.SaidaLoja: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.SaidaDestinatario;
            case this.SeparacaoMercadoria: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.SeparacaoMercadoria;
            case this.SolicitacaoVeiculo: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.SolicitacaoVeiculo;
            case this.TravamentoChave: return Localization.Resources.Enumeradores.EtapaFluxoGestaoPatio.TravamentoChave;
            default: return "";
        }
    },
    obterEtapasHabilitadasPreCarga: function () {
        return [
            this.CheckList,
            this.ChegadaVeiculo,
            this.Expedicao,
            this.FimCarregamento,
            this.FimDescarregamento,
            this.FimHigienizacao,
            this.Guarita,
            this.InformarDoca,
            this.InicioCarregamento,
            this.InicioDescarregamento,
            this.InicioHigienizacao,
            this.LiberacaoChave,
            this.SolicitacaoVeiculo,
            this.TravamentoChave,
            this.AvaliacaoDescarga
        ];
    },
    obterOpcoesGatilhoOcorrenciaFinal: function () {
        return [
            { text: this.obterDescricao(this.Todas), value: this.Todas },
            { text: this.obterDescricao(this.AvaliacaoDescarga), value: this.AvaliacaoDescarga },
            { text: this.obterDescricao(this.CheckList), value: this.CheckList },
            { text: this.obterDescricao(this.ChegadaVeiculo), value: this.ChegadaVeiculo },
            { text: this.obterDescricao(this.Guarita), value: this.Guarita },
            { text: this.obterDescricao(this.ChegadaLoja), value: this.ChegadaLoja },
            { text: this.obterDescricao(this.DeslocamentoPatio), value: this.DeslocamentoPatio },
            { text: this.obterDescricao(this.InformarDoca), value: this.InformarDoca },
            { text: this.obterDescricao(this.DocumentoFiscal), value: this.DocumentoFiscal },
            { text: this.obterDescricao(this.DocumentosTransporte), value: this.DocumentosTransporte },
            { text: this.obterDescricao(this.Entregas), value: this.Entregas },
            { text: this.obterDescricao(this.Expedicao), value: this.Expedicao },
            { text: this.obterDescricao(this.Faturamento), value: this.Faturamento },
            { text: this.obterDescricao(this.FimHigienizacao), value: this.FimHigienizacao },
            { text: this.obterDescricao(this.FimViagem), value: this.FimViagem },
            { text: this.obterDescricao(this.FimCarregamento), value: this.FimCarregamento },
            { text: this.obterDescricao(this.FimDescarregamento), value: this.FimDescarregamento },
            { text: this.obterDescricao(this.LiberacaoChave), value: this.LiberacaoChave },
            { text: this.obterDescricao(this.InicioHigienizacao), value: this.InicioHigienizacao },
            { text: this.obterDescricao(this.InicioViagem), value: this.InicioViagem },
            { text: this.obterDescricao(this.InicioCarregamento), value: this.InicioCarregamento },
            { text: this.obterDescricao(this.InicioDescarregamento), value: this.InicioDescarregamento },
            { text: this.obterDescricao(this.MontagemCarga), value: this.MontagemCarga },
            { text: this.obterDescricao(this.Posicao), value: this.Posicao },
            { text: this.obterDescricao(this.SaidaLoja), value: this.SaidaLoja },
            { text: this.obterDescricao(this.SeparacaoMercadoria), value: this.SeparacaoMercadoria },
            { text: this.obterDescricao(this.SolicitacaoVeiculo), value: this.SolicitacaoVeiculo },
            { text: this.obterDescricao(this.TravamentoChave), value: this.TravamentoChave }
        ];
    },
};

var EnumEtapaFluxoGestaoPatio = Object.freeze(new EnumEtapaFluxoGestaoPatioHelper());
