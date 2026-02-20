/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/plugin/chartjs/chart.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/SegmentoVeiculo.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumAcertoViagem.js" />
/// <reference path="../../Enumeradores/EnumEtapaAcertoViagem.js" />
/// <reference path="EtapaAcertoViagem.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/Global/PermissoesPersonalizadas.js" />
/// <reference path="../../Enumeradores/EnumPermissaoPersonalizada.js" />
/// <reference path="OcorrenciaAcertoViagem.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _etapaPesquisa = [
    { text: "Todos", value: 0 },
    { text: "Acerto", value: EnumEtapasAcertoViagem.Acerto },
    { text: "Cargas", value: EnumEtapasAcertoViagem.Cargas },
    { text: "Ocorrências", value: EnumEtapasAcertoViagem.Ocorrencias },
    { text: "Abastecimentos", value: EnumEtapasAcertoViagem.Abastecimentos },
    { text: "Pedágios", value: EnumEtapasAcertoViagem.Pedagios },
    { text: "Outras Despesas", value: EnumEtapasAcertoViagem.OutrasDespesas },
    { text: "Diárias", value: EnumEtapasAcertoViagem.Diarias },
    { text: "Fechamento", value: EnumEtapasAcertoViagem.Fechamento }
];

var _situacaoPesquisa = [
    { text: "Todos", value: 0 },
    { text: "Em Andamento", value: EnumSituacoesAcertoViagem.EmAndamento },
    { text: "Fechados", value: EnumSituacoesAcertoViagem.Fechado },
    { text: "Cancelados", value: EnumSituacoesAcertoViagem.Cancelado }
];

var _etapa = [
    { text: "Acerto", value: EnumEtapasAcertoViagem.Acerto },
    { text: "Cargas", value: EnumEtapasAcertoViagem.Cargas },
    { text: "Ocorrências", value: EnumEtapasAcertoViagem.Ocorrencias },
    { text: "Abastecimentos", value: EnumEtapasAcertoViagem.Abastecimentos },
    { text: "Pedágios", value: EnumEtapasAcertoViagem.Pedagios },
    { text: "Outras Despesas", value: EnumEtapasAcertoViagem.OutrasDespesas },
    { text: "Diárias", value: EnumEtapasAcertoViagem.Diarias },
    { text: "Fechamento", value: EnumEtapasAcertoViagem.Fechamento }
];

var _situacao = [
    { text: "Em Andamento", value: EnumSituacoesAcertoViagem.EmAndamento },
    { text: "Fechado", value: EnumSituacoesAcertoViagem.Fechado },
    { text: "Cancelado", value: EnumSituacoesAcertoViagem.Cancelado }
];

var _gridAcertoViagem;
var _acertoViagem;
var _pesquisaAcertoViagem;
var _PermissoesPersonalizadas;

var PesquisaAcertoViagem = function () {
    this.NumeroAcerto = PropertyEntity({ text: "Número do acerto: ", getType: typesKnockout.int, maxlength: 16 });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.DataAcerto = PropertyEntity({ text: "Data Acerto: ", getType: typesKnockout.date });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário que fez o acerto:", idBtnSearch: guid() });
    this.Etapa = PropertyEntity({ val: ko.observable(0), options: _etapaPesquisa, def: 0, text: "Etapa: " });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoPesquisa, def: 0, text: "Situação: " });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });

    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridAcertoViagem.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var AcertoViagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.AprovacaoAbastecimento = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.AprovacaoPedagio = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });

    this.OcorrenciaSalvo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.DiariaSalvo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.CargaSalvo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.AbastecimentoSalvo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.PedagioSalvo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.DespesaSalvo = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Etapa = PropertyEntity({ val: ko.observable(1), options: _etapa, def: 1 });
    this.Situacao = PropertyEntity({ val: ko.observable(1), options: _situacao, def: 1 });

    this.NumeroAcerto = PropertyEntity({ text: "Número do acerto:", required: true, maxlength: 18, getType: typesKnockout.int, enable: false, visible: ko.observable(false) });
    this.NumeroFrota = PropertyEntity({ text: ko.observable("*Nº Frota: "), required: ko.observable(true), maxlength: 30, enable: ko.observable(true), visible: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Motorista:", idBtnSearch: guid(), enable: ko.observable(true), issue: 145 });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: ko.observable("*Segmento do Veículo:"), idBtnSearch: guid(), enable: ko.observable(true) });
    this.Cheque = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: "Cheque:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false) });

    this.DataInicialMotorista = PropertyEntity({ getType: typesKnockout.date, visible: ko.observable(false) });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, enable: ko.observable(true), issue: 213, visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, enable: ko.observable(true), issue: 214, visible: ko.observable(true) });
    this.DataInicial.dateRangeInit = this.DataInicialMotorista;
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.DataHoraInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.dateTime, enable: ko.observable(true), issue: 213, visible: ko.observable(false) });
    this.DataHoraFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.dateTime, enable: ko.observable(true), issue: 214, visible: ko.observable(false) });

    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 3000, enable: ko.observable(true) });

    this.ValorTotalAlimentacaoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Total Pagamento:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });
    this.ValorAlimentacaoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Repassado:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });
    this.ValorAlimentacaoComprovado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Comprovado:", maxlength: 15, enable: ko.observable(true), val: ko.observable("") });
    this.ValorAlimentacaoSaldo = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });
    this.ValorTotalAdiantamentoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Total Pagamento:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });
    this.ValorAdiantamentoRepassado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Repassado:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });
    this.ValorAdiantamentoComprovado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Comprovado:", maxlength: 15, enable: ko.observable(true), val: ko.observable("") });
    this.ValorAdiantamentoSaldo = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });
    this.SaldoPrevistoAlimentacaoMotorista = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo Previsto Alimentação:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });
    this.SaldoPrevistoOutrasDepesasMotorista = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "Saldo Previsto Outras Despesas:", maxlength: 15, enable: ko.observable(false), val: ko.observable("") });

    this.FormaRecebimentoMotoristaAcerto = PropertyEntity({ val: ko.observable(EnumFormaRecebimentoMotoristaAcerto.NadaFazer), options: EnumFormaRecebimentoMotoristaAcerto.obterOpcoes(), def: EnumFormaRecebimentoMotoristaAcerto.NadaFazer, text: "Forma Recebimento: ", visible: ko.observable(true), enable: ko.observable(true) });
    this.DataVencimentoMotoristaAcerto = PropertyEntity({ getType: typesKnockout.date, required: false, text: "Data Vencimento:", enable: ko.observable(true), visible: ko.observable(false) });
    this.ObservacaoMotoristaAcerto = PropertyEntity({ text: "Observação:", maxlength: 3000, enable: ko.observable(true), visible: ko.observable(false) });
    this.TipoMovimentoMotoristaAcerto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Movimento:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.ObservacaoAcertoMotorista = PropertyEntity({ text: "Observação Acerto:", maxlength: 3000, enable: ko.observable(true), visible: ko.observable(false) });
    this.Titulo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Título:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "banco:", idBtnSearch: guid(), required: false, enable: ko.observable(true), visible: ko.observable(false) });

    this.ListaAbastecimentos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaBonificacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaDescontos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaOutrasDespesas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaDiarias = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaPedagios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaPedagiosCredito = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaVeiculos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaVeiculosFechamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaVeiculosArla = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaInfracoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.CancelarAcerto = PropertyEntity({ eventClick: CancelarAcertoClick, type: types.event, text: "Cancelar Acerto de Viagem", visible: ko.observable(false), enable: ko.observable(true) });
    this.SalvarObservacaoAcerto = PropertyEntity({ eventClick: SalvarObservacaoAcertoClick, type: types.event, text: "Salvar Observação", visible: ko.observable(false), enable: ko.observable(true) });
    this.IniciarAcerto = PropertyEntity({ eventClick: iniciarAcertoClick, type: types.event, text: "Salvar Acerto de Viagem", visible: ko.observable(true), enable: ko.observable(true) });

    this.Assinatura = PropertyEntity({ text: "Assinatura: ", visible: ko.observable(true) });
    



};

//*******EVENTOS*******

function loadAcertoViagem() {
    $("#knockoutCabecalhoAcerto").hide();

    _pesquisaAcertoViagem = new PesquisaAcertoViagem();
    KoBindings(_pesquisaAcertoViagem, "knockoutPesquisaAcertoViagem", null, _pesquisaAcertoViagem.Pesquisar.id);

    new BuscarVeiculos(_pesquisaAcertoViagem.Veiculo);
    new BuscarMotoristasPorStatus(_pesquisaAcertoViagem.Motorista);
    new BuscarFuncionario(_pesquisaAcertoViagem.Operador);
    new BuscarCargaFinalizadas(_pesquisaAcertoViagem.Carga);

    _acertoViagem = new AcertoViagem();
    KoBindings(_acertoViagem, "knockoutCadastroAcertoViagem");

    HeaderAuditoria("AcertoViagem", _acertoViagem);

    new BuscarMotoristas(_acertoViagem.Motorista, RetornoMotoristaAcertoViagem, null, null, true, EnumSituacaoColaborador.Todos);
    new BuscarSegmentoVeiculo(_acertoViagem.SegmentoVeiculo);

    if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
        _acertoViagem.DataHoraInicial.visible(true);
        _acertoViagem.DataHoraFinal.visible(true);

        _acertoViagem.DataInicial.visible(false);
        _acertoViagem.DataFinal.visible(false);
    }

    if (_CONFIGURACAO_TMS.NaoObrigarInformarSegmentoNoAcertoDeViagem) {
        _acertoViagem.SegmentoVeiculo.required(false);
        _acertoViagem.SegmentoVeiculo.text("Segmento do Veículo:");
    }
    
    if (_CONFIGURACAO_TMS.NaoObrigarInformarFrotaNoAcertoDeViagem) {
        _acertoViagem.NumeroFrota.required(false);
        _acertoViagem.NumeroFrota.text("Nº Frota:");
    }

    //buscarAcertoViagens();

    loadCabecalhoAcertoViagem();
    loadEtapaAcertoViagem();
    BuscarProximosDadosAcerto(0, buscarAcertoViagens);
}

function RetornoMotoristaAcertoViagem(data) {
    if (data != null) {
        _acertoViagem.Motorista.codEntity(data.Codigo);
        _acertoViagem.Motorista.val(data.Nome);
        BuscarProximosDadosAcerto(data.Codigo);
    }
}

//*******MÉTODOS*******

function editarAcertoViagem(acertoViagemGrid) {
    limparCamposAcertoViagem();
    _acertoViagem.Codigo.val(acertoViagemGrid.Codigo);
    BuscarPorCodigo(_acertoViagem, "AcertoViagem/BuscarPorCodigo", function (arg) {
        _pesquisaAcertoViagem.ExibirFiltros.visibleFade(false);
        CarregarDadosCabecalho(arg.Data);
        CarregarCargas(arg.Data);
        CarregarOcorrenciasAcerto();
        CarregarPedagios(arg.Data);
        CarregarDespesas(arg.Data);
        CarregarAbastecimentos(arg.Data);
        CarregarFechamento(arg.Data);
        PosicionarEtapa(arg.Data);
        _despesaDoVeiculo.AdicionarDespesa.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirLancarDespesasAcertoViagem, _PermissoesPersonalizadas));
        _diariaAcertoViagem.AdicionarDiaria.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirLancarDiariaAvulsoAcertoViagem, _PermissoesPersonalizadas));
        _acertoViagem.CancelarAcerto.visible(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteCancelamento, _PermissoesPersonalizadas));
        _acertoViagem.SalvarObservacaoAcerto.visible(true);
        $("#knockoutCabecalhoAcerto").show();
    }, null);
}

function buscarAcertoViagens() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarAcertoViagem, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridAcertoViagem = new GridView(_pesquisaAcertoViagem.Pesquisar.idGrid, "AcertoViagem/Pesquisa", _pesquisaAcertoViagem, menuOpcoes, { column: 1, dir: orderDir.desc }, null, null);
    _gridAcertoViagem.CarregarGrid();
}

function BuscarProximosDadosAcerto(codigoMotorista, callbackRetorno) {
    var data = { CodigoMotorista: codigoMotorista };
    executarReST("AcertoViagem/BuscarProximosDados", data, function (arg) {
        if (arg.Success) {
            var retorno = arg.Data;

            _acertoViagem.NumeroAcerto.val(retorno.Numero);
            _acertoViagem.AprovacaoAbastecimento.val(false);
            _acertoViagem.AprovacaoPedagio.val(false);
            _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Acerto);
            _acertoViagem.Situacao.val(EnumSituacoesAcertoViagem.EmAndamento);

            _acertoViagem.DataInicialMotorista.val("");
            _acertoViagem.DataInicial.val("");
            _acertoViagem.DataHoraInicial.val("");

            if (retorno.NumeroFrota !== null) {
               _acertoViagem.NumeroFrota.val(retorno.NumeroFrota);
            } else { _acertoViagem.NumeroFrota.val(""); }

            if (retorno.SegmentoVeiculoCodigo !== null) {
               _acertoViagem.SegmentoVeiculo.codEntity(retorno.SegmentoVeiculoCodigo);
            } else { _acertoViagem.SegmentoVeiculo.codEntity(""); }

            if (retorno.SegmentoVeiculoDescricao !== null) {
               _acertoViagem.SegmentoVeiculo.val(retorno.SegmentoVeiculoDescricao);
            } else { _acertoViagem.SegmentoVeiculo.val(""); }

            if (retorno.Numero > 1 && codigoMotorista > 0 && !_CONFIGURACAO_TMS.NaoBuscarDataInicioViagemAcerto) {
                if (retorno.Data != "") {
                    $("#" + _acertoViagem.DataInicialMotorista.id).val(retorno.Data);
                    _acertoViagem.DataInicial.val(retorno.Data);
                    _acertoViagem.DataHoraInicial.val(retorno.DataHoraInicial);
                }
            }
            $("#" + _acertoViagem.DataInicialMotorista.id).trigger("change");

            _cabecalhoAcertoViagem.NumeroAcerto.val(retorno.Numero);
            _cabecalhoAcertoViagem.DescricaoSituacao.val(_situacao[EnumSituacoesAcertoViagem.EmAndamento - 1].text);

            $("#knockoutCabecalhoAcerto").hide();

            carregarConteudosCargaHTML(loadCargaAcertoViagem);

            if (callbackRetorno !== null && callbackRetorno !== undefined)
                callbackRetorno();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function CarregarDadosAcertoViagem(codigo, posicionarEtapas, etapaPosicionar) {
    var data = { Codigo: codigo, Etapa: etapaPosicionar };
    if (posicionarEtapas == null)
        posicionarEtapas = true;
    executarReST("AcertoViagem/BuscarPorCodigo", data, function (arg) {
        if (arg.Success) {
            _despesaDoVeiculo.AdicionarDespesa.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirLancarDespesasAcertoViagem, _PermissoesPersonalizadas));
            _diariaAcertoViagem.AdicionarDiaria.enable(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermitirLancarDiariaAvulsoAcertoViagem, _PermissoesPersonalizadas));
            _acertoViagem.CancelarAcerto.visible(_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Acerto_PermiteCancelamento, _PermissoesPersonalizadas));
            _acertoViagem.SalvarObservacaoAcerto.visible(true);
            var dataAcertoViagem = { Data: arg.Data };
            PreencherObjetoKnout(_acertoViagem, dataAcertoViagem);

            _acertoViagem.NumeroAcerto.visible(true);

            CarregarDadosCabecalho(arg.Data);
            CarregarCargas(arg.Data);
            CarregarOcorrenciasAcerto();
            CarregarPedagios(arg.Data);
            CarregarDespesas(arg.Data);
            CarregarAbastecimentos(arg.Data);
            CarregarFechamento(arg.Data);
            if (posicionarEtapas)
                PosicionarEtapa(arg.Data);

            if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
                _fechamentoAcertoViagem.RetornarOutraDespesa.text("Reabrir Acerto");
                _fechamentoAcertoViagem.VisualizarDetalhe.text("Imprimir");
                VerificaVisibilidadeBotoesFechamento(false, true);

                _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
                _fechamentoAcertoViagem.VisualizarDetalhe.enable(true);
                _fechamentoAcertoViagem.RetornarOutraDespesa.enable(true);
                _fechamentoAcertoViagem.VisualizarAdiantamentos.visibleFade(false);
                _fechamentoAcertoViagem.VisualizarAdiantamentos.icon("fa fa-eye");

                HabilitarTodosBotoes(false);
            } else if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.EmAndamento) {
                _fechamentoAcertoViagem.RetornarOutraDespesa.text("Retornar Outras Despesas");
                _fechamentoAcertoViagem.VisualizarDetalhe.text("Visualizar Detalhes");
                VerificaVisibilidadeBotoesFechamento(true, false);

                _fechamentoAcertoViagem.FinalizarAcerto.enable(true);
                _fechamentoAcertoViagem.VisualizarDetalhe.enable(true);
                _fechamentoAcertoViagem.RetornarOutraDespesa.enable(true);
                HabilitarTodosBotoes(true);
            } else if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
                _fechamentoAcertoViagem.RetornarOutraDespesa.text("Retornar Outras Despesas");
                _fechamentoAcertoViagem.VisualizarDetalhe.text("Visualizar Detalhes");
                VerificaVisibilidadeBotoesFechamento(true, false);

                _fechamentoAcertoViagem.FinalizarAcerto.enable(false);
                _fechamentoAcertoViagem.VisualizarDetalhe.enable(false);
                _fechamentoAcertoViagem.RetornarOutraDespesa.enable(false);
                HabilitarTodosBotoes(false);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function limparCamposAcertoViagem() {
    LimparCampos(_acertoViagem);

    if (_fechamentoAcertoViagem !== undefined && _fechamentoAcertoViagem !== null) {
        _fechamentoAcertoViagem.VisualizarAdiantamentos.visibleFade(false);
        _fechamentoAcertoViagem.VisualizarAdiantamentos.icon("fa fa-eye");
    }

    if (_CONFIGURACAO_TMS.AcertoDeViagemComDiaria) {
        _acertoViagem.DataHoraInicial.visible(true);
        _acertoViagem.DataHoraFinal.visible(true);

        _acertoViagem.DataInicial.visible(false);
        _acertoViagem.DataFinal.visible(false);
    }
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function preencherListasSelecao() {
    _acertoViagem.ListaCargas.list = new Array();
    _acertoViagem.ListaPedagios.list = new Array();
    _acertoViagem.ListaPedagiosCredito.list = new Array();

    var cargas = new Array();
    var pedagios = new Array();
    var pedagiosCredito = new Array();
    var veiculos = new Array();
    var infracoes = new Array();

    $.each(_cargaAcertoViagem.Cargas.basicTable.BuscarRegistros(), function (i, carga) {
        if (carga.PercentualAcerto.toString().toLowerCase().indexOf("span") >= 0) {

            if (carga.PedagioAcertoCredito.toString().toLowerCase().indexOf("</span>") >= 0)
                carga.PedagioAcertoCredito = Globalize.parseFloat(carga.PedagioAcertoCredito.split("</span>")[1].trim());
            if (carga.PercentualAcerto.toString().toLowerCase().indexOf("</span>") >= 0)
                carga.PercentualAcerto = Globalize.parseFloat(carga.PercentualAcerto.split("</span>")[1].trim());
            if (carga.PedagioAcerto.toString().toLowerCase().indexOf("</span>") >= 0)
                carga.PedagioAcerto = Globalize.parseFloat(carga.PedagioAcerto.split("</span>")[1].trim());
        }
        carga.Destino = "";
        cargas.push({ Carga: carga });
    });

    $.each(_pedagioAcertoViagem.Pedagios.basicTable.BuscarRegistros(), function (i, pedagio) {
        pedagios.push({ Pedagio: pedagio });
    });

    $.each(_pedagioAcertoViagem.PedagiosCredito.basicTable.BuscarRegistros(), function (i, pedagio) {
        pedagiosCredito.push({ Pedagio: pedagio });
    });

    $.each(_fechamentoAcertoViagem.VeiculosFechamento.basicTable.BuscarRegistros(), function (i, veiculo) {
        veiculos.push({ Veiculo: veiculo });
    });

    $.each(_fechamentoAcertoViagem.AcertoViagemInfracao.basicTable.BuscarRegistros(), function (i, infracao) {
        infracoes.push({ Infracao: infracao });
    });

    _acertoViagem.ListaCargas.val(JSON.stringify(cargas));
    _acertoViagem.ListaPedagios.val(JSON.stringify(pedagios));
    _acertoViagem.ListaPedagiosCredito.val(JSON.stringify(pedagiosCredito));
    _acertoViagem.ListaVeiculosFechamento.val(JSON.stringify(veiculos));
    _acertoViagem.ListaInfracoes.val(JSON.stringify(infracoes));
}