/// <reference path="../JanelaCarregamentoTransportador/CargaLacre.js" />

var _detalhesCarga;
var _informacoesTransportador;
var _pesquisaJanelaCarregamentoTransportador;
var _cadastroCargaLacre;
var _agendamentoColetaListaAnexos;

var DetalhesCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoContainerReboque = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoContainerSegundoReboque = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoContainerVeiculo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoCargaJanelaCarregamento = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.CargaDeComplemento = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExigirConfirmacaoTracao = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExigirDefinicaoReboquePedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExigirMaxGrossJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExigirNumeroContainerJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.ExigirTaraContainerJanelaCarregamentoTransportador = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PermitirInformarAnexoContainerCarga = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiGenset = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.AlertarTransportadorNaoIMOCargasPerigosas = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PossuiInformacoesIMO = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.NumeroReboques = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoVeiculo = PropertyEntity({ val: ko.observable(""), visible: false });

    this.Numero = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Carga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroPedido = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDoPedido.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataDeCarregamento.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataLimiteCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataLimiteDeCarregamento.getFieldDescription(), visible: ko.observable(true), val: ko.observable(""), def: "" });
    this.PrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.PrevisaoDeEntrega.getFieldDescription(), val: ko.observable(""), def: "" });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.TipoDeCarga.getFieldDescription(), val: ko.observable(""), def: "" });
    this.ModeloVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ModeloDeVeiculo.getFieldDescription(), val: ko.observable(""), def: "", permitirAlterar: ko.observable(false), enable: ko.observable(false) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Gerais.Geral.Motorista.getRequiredFieldDescription(), label: "Motorista:", val: ko.observable(""), def: "", enable: ko.observable(false), required: true });
    this.Ajudante = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), idBtnSearch: guid(), text: Localization.Resources.Gerais.Geral.Ajudante.getFieldDescription(), label: "Ajudantes:", val: ko.observable(""), def: "", enable: ko.observable(false), required: false, visible: ko.observable(false) });
    this.Transportador = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Transportador.getFieldDescription(), type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", visible: ko.observable(false), enable: ko.observable(false) });
    this.Remetente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Remetente.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Origem = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Origem.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Destino = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destino.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Destinatario = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Destinatario.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroEntregas = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDeEntregas.getFieldDescription(), val: ko.observable(""), def: "" });
    this.NumeroEntregasFinais = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDeEntregasFinais.getFieldDescription(), val: ko.observable(""), def: "" });
    this.ValorFrete = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDoFrete.getFieldDescription(), val: ko.observable(""), def: "" });
    this.ValorICMS = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDoICMS.getFieldDescription(), val: ko.observable(""), def: "" });
    this.ValorTotalFrete = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorTotal.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Peso = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Peso.getFieldDescription(), val: ko.observable(""), def: "" });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription(), val: ko.observable(""), def: "" });
    this.ObservacaoCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ObservacaoDoCarregamento.getFieldDescription(), val: ko.observable(""), def: "" });
    this.DataRetiradaCtrnVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataRetiradaCTRN.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(false) });
    this.DataRetiradaCtrnReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataRetiradaCTRN.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(false) });
    this.DataRetiradaCtrnSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataRetiradaCTRN.getFieldDescription()), getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(false) });
    this.GensetVeiculo = PropertyEntity({ text: ko.observable("Genset: "), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.GensetReboque = PropertyEntity({ text: ko.observable("Genset: "), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.GensetSegundoReboque = PropertyEntity({ text: ko.observable("Genset: "), maxlength: 100, enable: ko.observable(true), visible: ko.observable(false) });
    this.MaxGrossVeiculo = PropertyEntity({ text: ko.observable("Max Gross: "), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.MaxGrossReboque = PropertyEntity({ text: ko.observable("Max Gross: "), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.MaxGrossSegundoReboque = PropertyEntity({ text: ko.observable("Max Gross: "), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.NumeroContainerVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroContainer.getFieldDescription()), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(false) });
    this.NumeroContainerReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroContainer.getFieldDescription()), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(false) });
    this.NumeroContainerSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroContainer.getFieldDescription()), maxlength: 1000, enable: ko.observable(true), visible: ko.observable(false) });
    this.TaraContainerVeiculo = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TaraContainer.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.TaraContainerReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TaraContainer.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.TaraContainerSegundoReboque = PropertyEntity({ text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.TaraContainer.getFieldDescription()), getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(false) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.Veiculo.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(false), required: true, cssClass: ko.observable("col col-xs-12"), popover: "<strong>Clique aqui para visualizar os detalhes</strong>", detalhesClick: exibirDetalhesVeiculoClick, verDetalhesVisible: ko.observable(false) });
    this.Reboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.VeiculoCarreta.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false), required: false, cssClass: ko.observable("col col-xs-12"), popover: "<strong>Clique aqui para visualizar os detalhes</strong>", detalhesClick: exibirDetalhesReboqueClick, pedidosClick: exibirPedidosReboqueClick, anexosClick: exibirAnexosReboqueClick, checklistClick: adicionarReboqueChecklistClick, visibleChecklist: ko.observable(false), verDetalhesVisible: ko.observable(false) });
    this.SegundoReboque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Veículo (Carreta 2):"), idBtnSearch: guid(), enable: ko.observable(false), visible: ko.observable(false), required: false, cssClass: ko.observable("col col-xs-12"), popover: "<strong>Clique aqui para visualizar os detalhes</strong>", detalhesClick: exibirDetalhesSegundoReboqueClick, pedidosClick: exibirPedidosSegundoReboqueClick, anexosClick: exibirAnexosSegundoReboqueClick, checklistClick: adicionarSegundoReboqueChecklistClick, visibleChecklist: ko.observable(false), verDetalhesVisible: ko.observable(false) });
    this.ObservacaoInformadaPeloTransportador = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ObservacaoDoTransportador.getFieldDescription(), maxlength: 300, visible: ko.observable(false), enable: ko.observable(true) });
    this.ExigirMotivoRejeicaoCarga = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MotivoRejeicaoCarga = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.MotivoRejeicaoCarga.getFieldDescription(), val: ko.observable(""), maxlength: 300, visible: this.ExigirMotivoRejeicaoCarga.val });
    this.NumeroDoca = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Doca.getFieldDescription(), val: ko.observable(""), def: "" });

    this.CIOT = PropertyEntity({ text: "CIOT: ", maxlength: 12, required: false, visible: ko.observable(false) });
    this.ValorValePedagio = PropertyEntity({ getType: typesKnockout.decimal, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ValorDoValePedagio.getFieldDescription(), required: false, maxlength: 12 });
    this.NumeroCompraValePedagio = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.NumeroDeCompraValePedagio.getFieldDescription(), maxlength: 12, required: false, cssClass: ko.observable("col col-3") });
    this.FornecedorValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.FornecedorDoValePedagio.getFieldDescription()), required: false, idBtnSearch: guid(), issue: 58, visible: ko.observable(true) });
    this.ResponsavelValePedagio = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), defCodEntity: "", text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ResponsavelDoValePedagio.getFieldDescription()), required: false, idBtnSearch: guid(), issue: 58, visible: ko.observable(true) });

    this.Componentes = ko.observableArray();
    this.ListaCargaLacre = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, codEntity: ko.observable(0), def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: _CONFIGURACAO_TMS.PermitirInformarLacreJanelaCarregamentoTransportador == true });
    this.Lacre = PropertyEntity({ type: types.entity, codEntity: ko.observable(""), text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.Lacres), required: ko.observable(false), val: ko.observable(false) });

    this.ExibirDatasCarregamento = PropertyEntity({ eventClick: function (e) { exibirDatasCarregamentoCargaJanelaTransportadorClick(e.Codigo.val()); }, enable: ko.observable(true), type: types.event });
    this.ConfirmarCarga = PropertyEntity({ eventClick: confirmarCargaClick, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.AceitarSemInformarDados.getFieldDescription(), idGrid: guid(), visible: ko.observable(false) });
    this.SalvarDadosTransporte = PropertyEntity({ eventClick: salvarDadosTransporteClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Confirmar), idGrid: guid(), visible: ko.observable(false) });
    this.AnexosAgendamento = PropertyEntity({ eventClick: anexoAgendamentoClick, type: types.event, text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.AnexosAgendamentoColeta), idGrid: guid(), visible: ko.observable(false) });
    this.ImprimirOrdemColeta = PropertyEntity({ eventClick: imprimirOrdemColetaClick, type: types.event, text: ko.observable(Localization.Resources.Logistica.JanelaCarregamentoTransportador.ImprimirOrdemColeta), idGrid: guid(), visible: ko.observable(false) });
    this.RejeitarCarga = PropertyEntity({ eventClick: rejeitarCargaClick, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Rejeitar, idGrid: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.AdicionarCargaLacre = PropertyEntity({ eventClick: adicionarCargaLacreModalClick, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.AdicionarLacre, visible: ko.observable(false), required: ko.observable(false) });
    this.VerDetalhesPedidos = PropertyEntity({ eventClick: exibirDetalhesPedidosClick, type: types.event, text: Localization.Resources.Gerais.Geral.VerDetalhes, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe ? !_CONFIGURACAO_TMS.NaoHabilitarDetalhesCarga : false) });
    this.AlterarDataCarregamento = PropertyEntity({ eventClick: function (e) { AbrirModalAlterarDataCarregamentoJanelaCarregamentoTransportadorClick(e) }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.AlterarDataAgendamento, visible: ko.observable(false) });
    this.ReenviarDocumentosReprovados = PropertyEntity({ eventClick: function (e) { EnviarDocumentosReprovados(e) }, type: types.event, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.ReenviarDocumentosReprovados, visible: ko.observable(false) });
    this.CargaPerigosa = PropertyEntity({ val: ko.observable(true), required: false, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.CargaPerigosa.getFieldDescription(), visible: ko.observable(true) });
    this.Agendado = PropertyEntity({ val: ko.observable(true), required: false, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.Agendado.getFieldDescription(), visible: ko.observable(false) });
    this.DataAgendamento = PropertyEntity({ val: ko.observable(true), required: false, text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataAgendamento.getFieldDescription(), visible: ko.observable(false) });
    this.DataPrevisaoTerminoCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.JanelaCarregamentoTransportador.DataPrevisaoTerminoCarregamento.getFieldDescription(), val: ko.observable("") });

    this.ContainerReboqueAnexo = new CargaVeiculoContainerAnexo();
    this.ContainerSegundoReboqueAnexo = new CargaVeiculoContainerAnexo();
    this.ContainerVeiculoAnexo = new CargaVeiculoContainerAnexo();
}
function obterDetalhesDaCarga(e) {
    $.get("Content/Static/Carga/ModalDetalhesCarga/DetalhesCarga.html?dyn=" + guid(), function (html) {
        $("#ModalDetalhesCargaContainer").html(html);

        _detalhesCarga = new DetalhesCarga();
        KoBindings(_detalhesCarga, "knockoutDadosCarga");

        var somenteDisponiveis = !_CONFIGURACAO_TMS.NaoExigeInformarDisponibilidadeDeVeiculo;

        executarReST("Usuario/DadosUsuarioLogado", {}, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false && arg.Data != null) {
                    _informacoesTransportador = arg.Data;
                }
            }
        });

        BuscarMotoristas(_detalhesCarga.Motorista, null, _detalhesCarga.Transportador, null, null, null, null, null, true);
        BuscarClientes(_detalhesCarga.FornecedorValePedagio);
        BuscarTransportadores(_detalhesCarga.Transportador);
        BuscarClientes(_detalhesCarga.ResponsavelValePedagio);
        BuscarVeiculos(_detalhesCarga.Veiculo, retornoConsultaVeiculo, _detalhesCarga.Transportador, _detalhesCarga.ModeloVeiculo, null, true, null, _detalhesCarga.TipoCarga, true, somenteDisponiveis, _detalhesCarga.Codigo, _detalhesCarga.TipoVeiculo);
        BuscarVeiculos(_detalhesCarga.Reboque, retornoConsultaReboque, _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, _detalhesCarga.Codigo, "1");
        BuscarVeiculos(_detalhesCarga.SegundoReboque, retornoConsultaSegundoReboque, _detalhesCarga.Transportador, null, null, true, null, null, true, somenteDisponiveis, _detalhesCarga.Codigo, "1");
        BuscarAjudante(_detalhesCarga.Ajudante);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            new BuscarModelosVeicularesCarga(
                _detalhesCarga.ModeloVeiculo,
                retornoConsultaModeloVeicular,
                null,
                null,
                [EnumTipoModeloVeicularCarga.Geral, EnumTipoModeloVeicularCarga.Reboque],
                null,
                null,
                null,
                null,
                _detalhesCarga.Codigo
            );
        } else {
            new BuscarModelosVeicularesCarga(_detalhesCarga.ModeloVeiculo, retornoConsultaModeloVeicular, null, _detalhesCarga.TipoCarga);
        }

        if (_CONFIGURACAO_TMS.ExibirDetalhesAgendamentoJanelaTransportador) {
            _detalhesCarga.Agendado.visible(true);
            _detalhesCarga.DataAgendamento.visible(true);
        } else {
            _detalhesCarga.Agendado.visible(false);
            _detalhesCarga.DataAgendamento.visible(false);
        }

        $("#" + _detalhesCarga.GensetVeiculo.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
        $("#" + _detalhesCarga.GensetReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });
        $("#" + _detalhesCarga.GensetSegundoReboque.id).mask("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", { selectOnFocus: true, optional: true });

        var codigo;
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
            codigo = e.CodigoJanelaCarregamentoTransportador.val()
        else
            codigo = e.Codigo

        loadCargaLacre();

        executarReST("JanelaCarregamentoTransportador/ObterDetalhesCarga", { Codigo: codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    const dados = retorno.Data;
                    limparCamposDetalhesCarga();

                    PreencherObjetoKnout(_detalhesCarga, retorno);

                    buscarConfiguracoesCentroCarregamento(e);
                    obterInformacaoExigirInformarLacre(e);

                    const permitirEditar = (
                        (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) &&
                        (
                            (dados.SituacaoCarga == EnumSituacoesCarga.AgTransportador) ||
                            (dados.SituacaoCarga == EnumSituacoesCarga.CalculoFrete) ||
                            (dados.SituacaoCarga == EnumSituacoesCarga.Nova) ||
                            (
                                (dados.SituacaoCarga == EnumSituacoesCarga.AgNFe) &&
                                (dados.ExigeNotaFiscalParaCalcularFrete || _CONFIGURACAO_TMS.PermitirInformarDadosTransportadorCargaEtapaNFe)
                            )
                        )
                    );

                    const habilitarEdicaoDadosTransporte = permitirEditar && retorno.Data.PermitirEdicaoDadosTransporte;
                    const habilitarEdicaoVeiculos = habilitarEdicaoDadosTransporte && retorno.Data.PermitirEdicaoVeiculos;

                    preencherCargaLacre(dados.CargaLacres, habilitarEdicaoDadosTransporte);

                    _detalhesCarga.Componentes.push.apply(_detalhesCarga.Componentes, dados.Componentes);

                    _detalhesCarga.ImprimirOrdemColeta.visible(dados.Situacao === EnumSituacaoCargaJanelaCarregamentoTransportador.Confirmada);
                    _detalhesCarga.Transportador.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.ModeloVeiculo.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.ObservacaoInformadaPeloTransportador.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.Motorista.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.Ajudante.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.Veiculo.enable(habilitarEdicaoVeiculos);
                    _detalhesCarga.Reboque.enable(habilitarEdicaoVeiculos);
                    _detalhesCarga.SegundoReboque.enable(habilitarEdicaoVeiculos);
                    _detalhesCarga.DataRetiradaCtrnVeiculo.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.DataRetiradaCtrnReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.DataRetiradaCtrnSegundoReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.GensetVeiculo.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.GensetReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.GensetSegundoReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.MaxGrossVeiculo.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.MaxGrossReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.MaxGrossSegundoReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.NumeroContainerVeiculo.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.NumeroContainerReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.NumeroContainerSegundoReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.TaraContainerVeiculo.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.TaraContainerReboque.enable(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.TaraContainerSegundoReboque.enable(habilitarEdicaoDadosTransporte);

                    controlarExibicaoCamposVeiculo(habilitarEdicaoDadosTransporte);

                    _detalhesCarga.ContainerReboqueAnexo.preencherAnexos(dados.AnexosReboque);
                    _detalhesCarga.ContainerSegundoReboqueAnexo.preencherAnexos(dados.AnexosSegundoReboque);
                    _detalhesCarga.ContainerVeiculoAnexo.preencherAnexos(dados.AnexosVeiculo);
                    _detalhesCarga.TipoVeiculo.val(dados.ExigirConfirmacaoTracao ? "0" : "");
                    _detalhesCarga.ModeloVeiculo.permitirAlterar(dados.CentroCarregamento.PermitirAlterarModeloVeicularCargaJanelaCarregamentoTransportador);

                    _detalhesCarga.CIOT.visible(dados.Veiculo.Tipo == "T");
                    _detalhesCarga.AnexosAgendamento.visible(dados.ListaAnexosAgendamento.length > 0);
                    _detalhesCarga.Transportador.visible(dados.CentroCarregamento.PermitirAlterarTransportador && _informacoesTransportador.Empresa.Matriz);
                    _detalhesCarga.ObservacaoInformadaPeloTransportador.visible(dados.CentroCarregamento.PermiteTransportadorInformarObservacoesJanelaCarregamentoTransportador);
                    _detalhesCarga.AdicionarCargaLacre.visible(habilitarEdicaoDadosTransporte);
                    _detalhesCarga.ExigirMotivoRejeicaoCarga.val(dados.CentroCarregamento.ExigirMotivoRejeicaoCarga);
                    _detalhesCarga.PossuiInformacoesIMO.val(dados.PossuiInformacoesIMO);
                    _detalhesCarga.CargaPerigosa.val(dados.CargaPerigosa);
                    _detalhesCarga.AlertarTransportadorNaoIMOCargasPerigosas.val(dados.AlertarTransportadorNaoIMOCargasPerigosas);

                    _detalhesCarga.ConfirmarCarga.visible(permitirEditar && Boolean(e.HorarioLimiteConfirmarCarga.val()));

                    const exibirBotaoRejeitar = permitirEditar && (_CONFIGURACAO_TMS.PermitirRejeitarCargaJanelaCarregamentoTransportador || dados.PermitirRejeitarCargaJanelaCarregamentoTransportador);
                    _detalhesCarga.RejeitarCarga.visible(exibirBotaoRejeitar);
                    _detalhesCarga.RejeitarCarga.enable(exibirBotaoRejeitar);
                    _detalhesCarga.CIOT.visible(exibirBotaoRejeitar && dados.CentroCarregamento.ExigirMotivoRejeicaoCarga);

                    _detalhesCarga.SalvarDadosTransporte.visible(permitirEditar);
                    _detalhesCarga.SalvarDadosTransporte.text(_detalhesCarga.ConfirmarCarga.visible() ? "Confirmar com Dados" : "Confirmar");
                    _detalhesCarga.Ajudante.visible(dados.PermiteInformarAjudantes);

                    controleOpcaoChecklist(dados.ExigirChecklist);

                    if (_detalhesCarga.CargaPerigosa.val() == "Sim") {
                        $(".cargaPerigosa").css({ "color": "#000000", "font-weight": "bold", "font-size": "15px" });
                    } else {
                        $(".cargaPerigosa").css({ "color": "", "font-weight": "", "font-size": "" });
                    }

                    Global.abrirModal("divModalDetalhesCargaJanelaCarregamento");
                    $('#divModalDetalhesCargaJanelaCarregamento').on('shown.bs.modal', function () {
                        if (permitirEditar) {
                            exibirMensagemPadraoInformarDadosTransporte();
                        }
                    });
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }
        });
    });
}

function limparCamposDetalhesCarga() {
    LimparCampos(_detalhesCarga);

    _detalhesCarga.Componentes.removeAll();
    _detalhesCarga.ContainerReboqueAnexo.limparAnexos();
    _detalhesCarga.ContainerSegundoReboqueAnexo.limparAnexos();
    _detalhesCarga.ContainerVeiculoAnexo.limparAnexos();

    _detalhesCarga.ListaCargaLacre.val([]);
}

function confirmarCargaClick() {
    executarReST("JanelaCarregamentoTransportador/ConfirmarCarga", { Carga: _detalhesCarga.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.fecharModal('divModalDetalhesCargaJanelaCarregamento');

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Carga confirmada com sucesso!");

                var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                $.each(retorno.Data.Cargas, function (j, carga) {
                    for (var i = 0; i < cargas.length; i++) {
                        if (cargas[i].Codigo.val() == carga.Codigo) {
                            AdicionarCarga(carga, i);
                            break;
                        }
                    }
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}
function adicionarReboqueChecklistClick() {
    limparCamposChecklistVeiculo();
    carregarChecklistVeiculo(_detalhesCarga.CodigoJanelaCarregamentoTransportador.val(), _detalhesCarga.Reboque.codEntity());
}

function adicionarSegundoReboqueChecklistClick() {
    limparCamposChecklistVeiculo();
    carregarChecklistVeiculo(_detalhesCarga.CodigoJanelaCarregamentoTransportador.val(), _detalhesCarga.SegundoReboque.codEntity());
}

function rejeitarCargaClick() {
    var dados = {
        Carga: _detalhesCarga.Codigo.val(),
        MotivoRejeicaoCarga: _detalhesCarga.MotivoRejeicaoCarga.val()
    };

    if (_detalhesCarga.ExigirMotivoRejeicaoCarga.val() && dados.MotivoRejeicaoCarga.length < 20) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, "O motivo da rejeição precisa ter mais de 20 caracteres.");
        return;
    }

    executarReST("JanelaCarregamentoTransportador/RejeitarCarga", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                Global.fecharModal('divModalDetalhesCargaJanelaCarregamento');

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Carga rejeitada com sucesso!");

                var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                $.each(retorno.Data.Cargas, function (j, carga) {
                    for (var i = 0; i < cargas.length; i++) {
                        if (cargas[i].Codigo.val() == carga.Codigo) {
                            AdicionarCarga(carga, i);
                            break;
                        }
                    }
                });
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function imprimirOrdemColetaClick() {
    executarDownload("JanelaCarregamentoTransportador/ImprimirOrdemColeta", { Codigo: _detalhesCarga.Codigo.val() });
}

function salvarDadosTransporteClick() {
    if (!ValidarCamposObrigatorios(_detalhesCarga)) {
        exibirMensagem(tipoMensagem.atencao, "Campos obrigatórios!", "Informe os campos obrigatórios!");
        return;
    }

    var dadosTransporteCargaSalvar = {
        Carga: _detalhesCarga.Codigo.val(),
        ModeloVeiculo: _detalhesCarga.ModeloVeiculo.codEntity(),
        Veiculo: _detalhesCarga.Veiculo.codEntity(),
        Reboque: _detalhesCarga.Reboque.codEntity(),
        Transportador: _detalhesCarga.Transportador.codEntity(),
        SegundoReboque: _detalhesCarga.SegundoReboque.codEntity(),
        Motorista: _detalhesCarga.Motorista.codEntity(),
        Ajudante: JSON.stringify(recursiveMultiplesEntities(_detalhesCarga.Ajudante)),
        CIOT: _detalhesCarga.CIOT.val(),
        ValorValePedagio: _detalhesCarga.ValorValePedagio.val(),
        NumeroCompraValePedagio: _detalhesCarga.NumeroCompraValePedagio.val(),
        FornecedorValePedagio: _detalhesCarga.FornecedorValePedagio.codEntity(),
        ResponsavelValePedagio: _detalhesCarga.ResponsavelValePedagio.codEntity(),
        DataRetiradaCtrnVeiculo: _detalhesCarga.DataRetiradaCtrnVeiculo.val(),
        GensetVeiculo: _detalhesCarga.GensetVeiculo.val(),
        MaxGrossVeiculo: _detalhesCarga.MaxGrossVeiculo.val(),
        NumeroContainerVeiculo: _detalhesCarga.NumeroContainerVeiculo.val(),
        TaraContainerVeiculo: _detalhesCarga.TaraContainerVeiculo.val(),
        DataRetiradaCtrnReboque: _detalhesCarga.DataRetiradaCtrnReboque.val(),
        GensetReboque: _detalhesCarga.GensetReboque.val(),
        MaxGrossReboque: _detalhesCarga.MaxGrossReboque.val(),
        NumeroContainerReboque: _detalhesCarga.NumeroContainerReboque.val(),
        TaraContainerReboque: _detalhesCarga.TaraContainerReboque.val(),
        DataRetiradaCtrnSegundoReboque: _detalhesCarga.DataRetiradaCtrnSegundoReboque.val(),
        GensetSegundoReboque: _detalhesCarga.GensetSegundoReboque.val(),
        MaxGrossSegundoReboque: _detalhesCarga.MaxGrossSegundoReboque.val(),
        NumeroContainerSegundoReboque: _detalhesCarga.NumeroContainerSegundoReboque.val(),
        TaraContainerSegundoReboque: _detalhesCarga.TaraContainerSegundoReboque.val(),
        SelecaoQualquerVeiculoConfirmada: false,
        CargaLacres: obterCargaLacresSalvar(),
        ObservacaoInformadaPeloTransportador: _detalhesCarga.ObservacaoInformadaPeloTransportador.val(),
        VerificarMDFeAbertoForaDoSistema: true,
        PossuiInformacoesIMO: _detalhesCarga.PossuiInformacoesIMO.val(),
        AlertarTransportadorNaoIMOCargasPerigosas: _detalhesCarga.AlertarTransportadorNaoIMOCargasPerigosas.val(),
    };

    salvarDadosTransporte(dadosTransporteCargaSalvar);
}

function salvarDadosTransporte(dadosTransporteCargaSalvar) {
    if (!dadosTransporteCargaSalvar.PossuiInformacoesIMO && dadosTransporteCargaSalvar.AlertarTransportadorNaoIMOCargasPerigosas) {
        exibirConfirmacao("Confirmação", "Você realmente deseja salvar sem o transportador possuir as informações de IMO cadastrada?", function () {
            executarReST("JanelaCarregamentoTransportador/SalvarDadosTransporteCarga", dadosTransporteCargaSalvar, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data) {
                        if (retorno.Data.AvisoMDFeEmAberto) {
                            exibirConfirmacao("Confirmação", "Antes de prosseguir com a confirmação, verifique se existem MDF-es não encerrados fora do sistema. Deseja continuar?", function () {
                                dadosTransporteCargaSalvar.VerificarMDFeAbertoForaDoSistema = false;
                                salvarDadosTransporte(dadosTransporteCargaSalvar);
                            });

                            return;
                        }

                        if (retorno.Data.ConfirmarSelecaoQualquerVeiculo) {
                            exibirConfirmacao("Confirmação", retorno.Data.Mensagem + " Deseja realmente continuar?", function () {
                                dadosTransporteCargaSalvar.SelecaoQualquerVeiculoConfirmada = true
                                salvarDadosTransporte(dadosTransporteCargaSalvar);
                            });

                            return;
                        }

                        if (retorno.Data.SelecionarPeriodoCarregamento) {
                            exibirModalSelecaoPeriodoCarregamento(dadosTransporteCargaSalvar, retorno.Data.PeriodosCarregamento);
                            return;
                        }

                        if (retorno.Data.SolicitarNotasFiscaisAoSalvarDadosTransportador) {
                            solicitarNotasFiscais(dadosTransporteCargaSalvar);
                        }

                        if (retorno.Data.ExigirInformacaoLacreJanelaCarregamento) {
                            if (_detalhesCarga.ListaCargaLacre.val() == 0) {
                                exibirMensagem(tipoMensagem.aviso, "Falha", retorno.Data.Mensagem);
                                return;
                            }
                        }

                        var cargaReferencia = retorno.Data.Cargas[0];

                        if (!string.IsNullOrWhiteSpace(cargaReferencia.MensagemLicenca))
                            exibirMensagem(tipoMensagem.atencao, "Atenção", cargaReferencia.MensagemLicenca);

                        Global.fecharModal('divModalDetalhesCargaJanelaCarregamento');
                        if (cargaReferencia.LembrarEspelhamento) {
                            exibirAlerta("Favor espelhar o sinal do veículo para a conta \"" + cargaReferencia.DescricaoRastreador + "\".", "Encerrar MDF-es em aberto da placa \"" + cargaReferencia.Placa + "\".");
                        }

                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Dados salvos com sucesso!");

                        exibirTermoConfirmacaoChegadaHorario(cargaReferencia);

                        if (!_CONFIGURACAO_TMS.NaoExigeInformarDisponibilidadeDeVeiculo)
                            _gridPlacasDisponiveis.CarregarGrid();

                        if (isNaN(_detalhesCarga.CodigoContainerReboque.val()) && (cargaReferencia.CodigoContainerReboque > 0))
                            _detalhesCarga.ContainerReboqueAnexo.enviarArquivosAnexados(cargaReferencia.CodigoContainerReboque);

                        if (isNaN(_detalhesCarga.CodigoContainerSegundoReboque.val()) && (cargaReferencia.CodigoContainerSegundoReboque > 0))
                            _detalhesCarga.ContainerSegundoReboqueAnexo.enviarArquivosAnexados(cargaReferencia.CodigoContainerSegundoReboque);

                        if (isNaN(_detalhesCarga.CodigoContainerVeiculo.val()) && (cargaReferencia.CodigoContainerVeiculo > 0))
                            _detalhesCarga.ContainerVeiculoAnexo.enviarArquivosAnexados(cargaReferencia.CodigoContainerVeiculo);

                        var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                        $.each(retorno.Data.Cargas, function (j, carga) {
                            for (var i = 0; i < cargas.length; i++) {
                                if (cargas[i].Codigo.val() == carga.Codigo) {
                                    AdicionarCarga(carga, i);
                                    break;
                                }
                            }
                        });
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            });
        })
        return;
    } else {
        executarReST("JanelaCarregamentoTransportador/SalvarDadosTransporteCarga", dadosTransporteCargaSalvar, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    if (retorno.Data.AvisoMDFeEmAberto) {
                        exibirConfirmacao("Confirmação", "Antes de prosseguir com a confirmação, verifique se existem MDF-es não encerrados fora do sistema. Deseja continuar?", function () {
                            dadosTransporteCargaSalvar.VerificarMDFeAbertoForaDoSistema = false;
                            salvarDadosTransporte(dadosTransporteCargaSalvar);
                        });

                        return;
                    }

                    if (retorno.Data.ConfirmarSelecaoQualquerVeiculo) {
                        exibirConfirmacao("Confirmação", retorno.Data.Mensagem + " Deseja realmente continuar?", function () {
                            dadosTransporteCargaSalvar.SelecaoQualquerVeiculoConfirmada = true
                            salvarDadosTransporte(dadosTransporteCargaSalvar);
                        });

                        return;
                    }

                    if (retorno.Data.SelecionarPeriodoCarregamento) {
                        exibirModalSelecaoPeriodoCarregamento(dadosTransporteCargaSalvar, retorno.Data.PeriodosCarregamento);
                        return;
                    }

                    if (retorno.Data.SolicitarNotasFiscaisAoSalvarDadosTransportador) {
                        solicitarNotasFiscais(dadosTransporteCargaSalvar);
                    }

                    if (retorno.Data.ExigirInformacaoLacreJanelaCarregamento) {
                        if (_detalhesCarga.ListaCargaLacre.val() == 0) {
                            exibirMensagem(tipoMensagem.aviso, "Falha", retorno.Data.Mensagem);
                            return;
                        }
                    }

                    var cargaReferencia = retorno.Data.Cargas[0];

                    if (!string.IsNullOrWhiteSpace(cargaReferencia.MensagemLicenca))
                        exibirMensagem(tipoMensagem.atencao, "Atenção", cargaReferencia.MensagemLicenca);

                    Global.fecharModal('divModalDetalhesCargaJanelaCarregamento');
                    if (cargaReferencia.LembrarEspelhamento) {
                        exibirAlerta("Favor espelhar o sinal do veículo para a conta \"" + cargaReferencia.DescricaoRastreador + "\".", "Encerrar MDF-es em aberto da placa \"" + cargaReferencia.Placa + "\".");
                    }

                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Dados salvos com sucesso!");

                    exibirTermoConfirmacaoChegadaHorario(cargaReferencia);

                    if (!_CONFIGURACAO_TMS.NaoExigeInformarDisponibilidadeDeVeiculo)
                        _gridPlacasDisponiveis.CarregarGrid();

                    if (isNaN(_detalhesCarga.CodigoContainerReboque.val()) && (cargaReferencia.CodigoContainerReboque > 0))
                        _detalhesCarga.ContainerReboqueAnexo.enviarArquivosAnexados(cargaReferencia.CodigoContainerReboque);

                    if (isNaN(_detalhesCarga.CodigoContainerSegundoReboque.val()) && (cargaReferencia.CodigoContainerSegundoReboque > 0))
                        _detalhesCarga.ContainerSegundoReboqueAnexo.enviarArquivosAnexados(cargaReferencia.CodigoContainerSegundoReboque);

                    if (isNaN(_detalhesCarga.CodigoContainerVeiculo.val()) && (cargaReferencia.CodigoContainerVeiculo > 0))
                        _detalhesCarga.ContainerVeiculoAnexo.enviarArquivosAnexados(cargaReferencia.CodigoContainerVeiculo);

                    var cargas = _pesquisaJanelaCarregamentoTransportador.Cargas();

                    $.each(retorno.Data.Cargas, function (j, carga) {
                        for (var i = 0; i < cargas.length; i++) {
                            if (cargas[i].Codigo.val() == carga.Codigo) {
                                AdicionarCarga(carga, i);
                                break;
                            }
                        }
                    });
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        });
    }
}


function controlarExibicaoCamposVeiculo(habilitarEdicaoDadosTransporte) {
    var reboqueVisivel = false;
    var segundoReboqueVisivel = false;

    if (_detalhesCarga.ExigirConfirmacaoTracao.val()) {
        reboqueVisivel = (_detalhesCarga.NumeroReboques.val() >= 1);
        segundoReboqueVisivel = (_detalhesCarga.NumeroReboques.val() > 1);
    }

    var exibirSomenteCampoVeiculo = (!reboqueVisivel && !segundoReboqueVisivel);
    var tamanhoColuna = 12;

    if (_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga)
        tamanhoColuna -= 3;

    if (_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga)
        tamanhoColuna -= 3;

    if (!reboqueVisivel)
        LimparCampoEntity(_detalhesCarga.Reboque);

    if (!segundoReboqueVisivel)
        LimparCampoEntity(_detalhesCarga.SegundoReboque);

    _detalhesCarga.Veiculo.cssClass("col col-xs-12" + (exibirSomenteCampoVeiculo ? " col-sm-" + tamanhoColuna : ""));
    _detalhesCarga.Veiculo.required = habilitarEdicaoDadosTransporte;
    _detalhesCarga.Veiculo.text((_detalhesCarga.Veiculo.required ? "*" : "") + (reboqueVisivel ? "Tração (Cavalo):" : "Veiculo:"));
    _detalhesCarga.Veiculo.verDetalhesVisible(!reboqueVisivel && _detalhesCarga.PermitirInformarAnexoContainerCarga.val() && habilitarEdicaoDadosTransporte);

    _detalhesCarga.Reboque.cssClass("col col-xs-12 col-sm-" + tamanhoColuna);
    _detalhesCarga.Reboque.required = (reboqueVisivel && habilitarEdicaoDadosTransporte);
    _detalhesCarga.Reboque.text((_detalhesCarga.Reboque.required ? "*" : "") + (segundoReboqueVisivel ? "Veículo (Carreta 1):" : "Veículo (Carreta):"));
    _detalhesCarga.Reboque.visible(reboqueVisivel);
    _detalhesCarga.Reboque.verDetalhesVisible(reboqueVisivel && (_detalhesCarga.ExigirDefinicaoReboquePedido.val() || _detalhesCarga.PermitirInformarAnexoContainerCarga.val()) && habilitarEdicaoDadosTransporte);

    _detalhesCarga.SegundoReboque.cssClass("col col-xs-12 col-sm-" + tamanhoColuna);
    _detalhesCarga.SegundoReboque.required = (segundoReboqueVisivel && habilitarEdicaoDadosTransporte);
    _detalhesCarga.SegundoReboque.text((_detalhesCarga.SegundoReboque.required ? "*" : "") + "Veículo (Carreta 2):");
    _detalhesCarga.SegundoReboque.visible(segundoReboqueVisivel);
    _detalhesCarga.SegundoReboque.verDetalhesVisible(segundoReboqueVisivel && (_detalhesCarga.ExigirDefinicaoReboquePedido.val() || _detalhesCarga.PermitirInformarAnexoContainerCarga.val()) && habilitarEdicaoDadosTransporte);

    _detalhesCarga.DataRetiradaCtrnVeiculo.required = (habilitarEdicaoDadosTransporte && exibirSomenteCampoVeiculo && _CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && _detalhesCarga.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador.val());
    _detalhesCarga.DataRetiradaCtrnVeiculo.text((_detalhesCarga.DataRetiradaCtrnVeiculo.required ? "*" : "") + "Data Retirada CTRN:");
    _detalhesCarga.DataRetiradaCtrnVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && exibirSomenteCampoVeiculo);

    _detalhesCarga.DataRetiradaCtrnReboque.required = (_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && _detalhesCarga.Reboque.required && _detalhesCarga.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador.val());
    _detalhesCarga.DataRetiradaCtrnReboque.text((_detalhesCarga.DataRetiradaCtrnReboque.required ? "*" : "") + "Data Retirada CTRN:");
    _detalhesCarga.DataRetiradaCtrnReboque.visible(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && _detalhesCarga.Reboque.visible());

    _detalhesCarga.DataRetiradaCtrnSegundoReboque.required = (_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && _detalhesCarga.SegundoReboque.required && _detalhesCarga.ExigirDataRetiradaCtrnJanelaCarregamentoTransportador.val());
    _detalhesCarga.DataRetiradaCtrnSegundoReboque.text((_detalhesCarga.DataRetiradaCtrnSegundoReboque.required ? "*" : "") + "Data Retirada CTRN:");
    _detalhesCarga.DataRetiradaCtrnSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarDataRetiradaCtrnCarga && _detalhesCarga.SegundoReboque.visible());

    _detalhesCarga.GensetVeiculo.required = (habilitarEdicaoDadosTransporte && exibirSomenteCampoVeiculo && _detalhesCarga.PossuiGenset.val());
    _detalhesCarga.GensetVeiculo.text((_detalhesCarga.GensetVeiculo.required ? "*" : "") + "Genset:");
    _detalhesCarga.GensetVeiculo.visible(exibirSomenteCampoVeiculo && _detalhesCarga.PossuiGenset.val());

    _detalhesCarga.GensetReboque.required = (_detalhesCarga.Reboque.required && _detalhesCarga.PossuiGenset.val());
    _detalhesCarga.GensetReboque.text((_detalhesCarga.GensetReboque.required ? "*" : "") + "Genset:");
    _detalhesCarga.GensetReboque.visible(_detalhesCarga.Reboque.visible() && _detalhesCarga.PossuiGenset.val());

    _detalhesCarga.GensetSegundoReboque.required = (_detalhesCarga.SegundoReboque.required && _detalhesCarga.PossuiGenset.val());
    _detalhesCarga.GensetSegundoReboque.text((_detalhesCarga.GensetSegundoReboque.required ? "*" : "") + "Genset:");
    _detalhesCarga.GensetSegundoReboque.visible(_detalhesCarga.SegundoReboque.visible() && _detalhesCarga.PossuiGenset.val());

    _detalhesCarga.MaxGrossVeiculo.required = (habilitarEdicaoDadosTransporte && exibirSomenteCampoVeiculo && _CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && _detalhesCarga.ExigirMaxGrossJanelaCarregamentoTransportador.val());
    _detalhesCarga.MaxGrossVeiculo.text((_detalhesCarga.MaxGrossVeiculo.required ? "*" : "") + "Max Gross:");
    _detalhesCarga.MaxGrossVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && exibirSomenteCampoVeiculo);

    _detalhesCarga.MaxGrossReboque.required = (_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && _detalhesCarga.Reboque.required && _detalhesCarga.ExigirMaxGrossJanelaCarregamentoTransportador.val());
    _detalhesCarga.MaxGrossReboque.text((_detalhesCarga.MaxGrossReboque.required ? "*" : "") + "Max Gross:");
    _detalhesCarga.MaxGrossReboque.visible(_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && _detalhesCarga.Reboque.visible());

    _detalhesCarga.MaxGrossSegundoReboque.required = (_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && _detalhesCarga.SegundoReboque.required && _detalhesCarga.ExigirMaxGrossJanelaCarregamentoTransportador.val());
    _detalhesCarga.MaxGrossSegundoReboque.text((_detalhesCarga.MaxGrossSegundoReboque.required ? "*" : "") + "Max Gross:");
    _detalhesCarga.MaxGrossSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarMaxGrossCarga && _detalhesCarga.SegundoReboque.visible());

    _detalhesCarga.NumeroContainerVeiculo.required = (habilitarEdicaoDadosTransporte && exibirSomenteCampoVeiculo && _CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && _detalhesCarga.ExigirNumeroContainerJanelaCarregamentoTransportador.val());
    _detalhesCarga.NumeroContainerVeiculo.text((_detalhesCarga.NumeroContainerVeiculo.required ? "*" : "") + "Número Container:");
    _detalhesCarga.NumeroContainerVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && exibirSomenteCampoVeiculo);

    _detalhesCarga.NumeroContainerReboque.required = (_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && _detalhesCarga.Reboque.required && _detalhesCarga.ExigirNumeroContainerJanelaCarregamentoTransportador.val());
    _detalhesCarga.NumeroContainerReboque.text((_detalhesCarga.NumeroContainerReboque.required ? "*" : "") + "Número Container:");
    _detalhesCarga.NumeroContainerReboque.visible(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && _detalhesCarga.Reboque.visible());

    _detalhesCarga.NumeroContainerSegundoReboque.required = (_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && _detalhesCarga.SegundoReboque.required && _detalhesCarga.ExigirNumeroContainerJanelaCarregamentoTransportador.val());
    _detalhesCarga.NumeroContainerSegundoReboque.text((_detalhesCarga.NumeroContainerSegundoReboque.required ? "*" : "") + "Número Container:");
    _detalhesCarga.NumeroContainerSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && _detalhesCarga.SegundoReboque.visible());

    _detalhesCarga.TaraContainerVeiculo.required = (habilitarEdicaoDadosTransporte && exibirSomenteCampoVeiculo && _CONFIGURACAO_TMS.PermitirInformarNumeroContainerCarga && _detalhesCarga.ExigirTaraContainerJanelaCarregamentoTransportador.val());
    _detalhesCarga.TaraContainerVeiculo.text((_detalhesCarga.TaraContainerVeiculo.required ? "*" : "") + "Tara Container:");
    _detalhesCarga.TaraContainerVeiculo.visible(_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && exibirSomenteCampoVeiculo);

    _detalhesCarga.TaraContainerReboque.required = (_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && _detalhesCarga.Reboque.required && _detalhesCarga.ExigirTaraContainerJanelaCarregamentoTransportador.val());
    _detalhesCarga.TaraContainerReboque.text((_detalhesCarga.TaraContainerReboque.required ? "*" : "") + "Tara Container:");
    _detalhesCarga.TaraContainerReboque.visible(_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && _detalhesCarga.Reboque.visible());

    _detalhesCarga.TaraContainerSegundoReboque.required = (_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && _detalhesCarga.SegundoReboque.required && _detalhesCarga.ExigirTaraContainerJanelaCarregamentoTransportador.val());
    _detalhesCarga.TaraContainerSegundoReboque.text((_detalhesCarga.TaraContainerSegundoReboque.required ? "*" : "") + "Tara Container:");
    _detalhesCarga.TaraContainerSegundoReboque.visible(_CONFIGURACAO_TMS.PermitirInformarTaraContainerCarga && _detalhesCarga.SegundoReboque.visible());
}

function retornoConsultaModeloVeicular(modeloVeicularSelecionado) {
    _detalhesCarga.ModeloVeiculo.codEntity(modeloVeicularSelecionado.Codigo);
    _detalhesCarga.ModeloVeiculo.val(modeloVeicularSelecionado.Descricao);
    _detalhesCarga.ExigirDefinicaoReboquePedido.val(modeloVeicularSelecionado.ExigirDefinicaoReboquePedido);
    _detalhesCarga.NumeroReboques.val(modeloVeicularSelecionado.NumeroReboques);

    controlarExibicaoCamposVeiculo(_detalhesCarga.ModeloVeiculo.enable());
}

function retornoConsultaReboque(reboqueSelecionado) {
    if (_detalhesCarga.SegundoReboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_detalhesCarga.Reboque);
    }
    else {
        _detalhesCarga.Reboque.codEntity(reboqueSelecionado.Codigo);
        _detalhesCarga.Reboque.entityDescription(reboqueSelecionado.Placa);
        _detalhesCarga.Reboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaSegundoReboque(reboqueSelecionado) {
    if (_detalhesCarga.Reboque.codEntity() == reboqueSelecionado.Codigo) {
        exibirMensagem(tipoMensagem.atencao, "Veículo (Carreta 1)", "Não é possível selecionar duas carretas iguais.");

        LimparCampoEntity(_detalhesCarga.SegundoReboque);
    }
    else {
        _detalhesCarga.SegundoReboque.codEntity(reboqueSelecionado.Codigo);
        _detalhesCarga.SegundoReboque.entityDescription(reboqueSelecionado.Placa);
        _detalhesCarga.SegundoReboque.val(reboqueSelecionado.Placa);
    }
}

function retornoConsultaVeiculo(veiculoSelecionado) {
    _detalhesCarga.Veiculo.codEntity(veiculoSelecionado.Codigo);

    if (_detalhesCarga.ExigirConfirmacaoTracao.val()) {
        _detalhesCarga.Veiculo.entityDescription(veiculoSelecionado.Placa);
        _detalhesCarga.Veiculo.val(veiculoSelecionado.Placa);
    }
    else {
        _detalhesCarga.Veiculo.entityDescription(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
        _detalhesCarga.Veiculo.val(veiculoSelecionado.ConjuntoPlacasSemModeloVeicular);
    }

    Global.setarFocoProximoCampo(_detalhesCarga.Veiculo.id);

    executarReST("Veiculo/BuscarPorCodigo", { Codigo: veiculoSelecionado.Codigo }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _detalhesCarga.CIOT.visible(true);
                _detalhesCarga.CIOT.val(retorno.Data.CIOT);
                _detalhesCarga.ValorValePedagio.val(retorno.Data.ValorValePedagio);
                _detalhesCarga.ResponsavelValePedagio.val(retorno.Data.ResponsavelValePedagio.Descricao);
                _detalhesCarga.FornecedorValePedagio.val(retorno.Data.FornecedorValePedagio.Descricao);
                _detalhesCarga.ResponsavelValePedagio.codEntity(retorno.Data.ResponsavelValePedagio.Codigo);
                _detalhesCarga.FornecedorValePedagio.codEntity(retorno.Data.FornecedorValePedagio.Codigo);
                _detalhesCarga.NumeroCompraValePedagio.val(retorno.Data.NumeroCompraValePedagio);

                _detalhesCarga.RejeitarCarga.enable(true);
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function buscarConfiguracoesCentroCarregamento(e) {
    var data;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
        data = { Carga: e.Carga.val(), CodigoJanelaCarregamentoTransportador: e.CodigoJanelaCarregamentoTransportador.val(), CodigoJanelaCarregamento: e.Codigo.val() };
    else
        data = { Carga: e.Carga, CodigoJanelaCarregamentoTransportador: e.CodigoJanelaCarregamentoTransportador, CodigoJanelaCarregamento: e.Codigo };

    executarReST("JanelaCarregamentoTransportador/BuscarConfiguracoesCentroCarregamento", data, function (retorno) {
        if (retorno.Success && retorno.Data) {
            _detalhesCarga.AlterarDataCarregamento.visible(retorno.Data.PermitirAlterarDataCarregamento);
            _detalhesCarga.ReenviarDocumentosReprovados.visible(retorno.Data.PermiteReenviarIntegracaoDocumentosRejeitados);

        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });

}

function obterInformacaoExigirInformarLacre(e) {
    var data;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe)
        data = { CodigoJanelaCarregamento: e.Codigo.val() };
    else
        data = { CodigoJanelaCarregamento: e.Codigo };
    executarReST("JanelaCarregamentoTransportador/ObterInformacaoExigirInformarLacre", data, function (retorno) {
        if (retorno.Success && retorno.Data.ExigirInformacaoLacreJanelaCarregamento) {
            _detalhesCarga.AdicionarCargaLacre.required(true);
            _detalhesCarga.Lacre.text(Localization.Resources.Logistica.JanelaCarregamentoTransportador.Lacres.getRequiredFieldDescription());
        }
    });
}

function EnviarDocumentosReprovados(data) {
    executarReST("JanelaCarregamentoTransportador/EnviarDocumentosReprovados", { CodigoCarga: _detalhesCarga.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Logistica.JanelaCarregamentoTransportador.ReenvioSolicitadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, r.Msg);
        }
    });
}

function controleOpcaoChecklist(habilitar) {
    _detalhesCarga.Reboque.visibleChecklist(habilitar);
    _detalhesCarga.SegundoReboque.visibleChecklist(habilitar);
}

function exibirDetalhesCarga(e) {
    var exigeAceiteDoTermoTransporte = e.ExigeTermoAceiteParaTransporte.val();

    if (!exigeAceiteDoTermoTransporte)
        obterDetalhesDaCarga(e);
    else {
        var data = {
            Callback: function () { obterDetalhesDaCarga(e); },
            TermoAceite: e.TermoAceite.val(),
            CodigoJanelaCarregamentoTransportador: e.CodigoJanelaCarregamentoTransportador.val()
        };

        exibirModalAceiteTermoTransporte(data);
    }
}

function exibirPedidosReboqueClick(cargaSelecionada) {
    exibirDetalhesPedidos(cargaSelecionada.Codigo.val(), function (knoutDetalhePedido, pedido) {
        if (pedido.NumeroReboque != EnumNumeroReboque.ReboqueUm)
            knoutDetalhePedido.OcultarPedido.val(true);
    });
}

function exibirAnexosReboqueClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerReboqueAnexo, cargaSelecionada.CodigoContainerReboque.val());
}

function exibirAnexosSegundoReboqueClick(cargaSelecionada) {
    exibirCargaVeiculoContainerAnexo(cargaSelecionada.ContainerSegundoReboqueAnexo, cargaSelecionada.CodigoContainerSegundoReboque.val());
}
function exibirPedidosSegundoReboqueClick(cargaSelecionada) {
    exibirDetalhesPedidos(cargaSelecionada.Codigo.val(), function (knoutDetalhePedido, pedido) {
        if (pedido.NumeroReboque != EnumNumeroReboque.ReboqueDois)
            knoutDetalhePedido.OcultarPedido.val(true);
    });
}
function anexoAgendamentoClick() {
    _agendamentoColetaListaAnexos.Adicionar.visible(false);

    Global.abrirModal('divModalAnexoAgendamentoColeta');
    $("#divModalAnexoAgendamentoColeta").one("hidden.bs.modal", function () {
        Global.fecharModal("divModalAdicionarAnexoAgendamentoColeta");
    });
}

function exibirDetalhesPedidosClick(cargaSelecionada) {
    exibirDetalhesPedidos(cargaSelecionada.Codigo.val());
}