//*******MAPEAMENTO KNOUCKOUT*******

var _etapaPesquisa = [
    { text: "Todos", value: 0 },
    { text: "Fatura", value: EnumEtapasFatura.Fatura },
    { text: "Cargas", value: EnumEtapasFatura.Cargas },
    { text: "Finalização", value: EnumEtapasFatura.Fechamento },
    { text: "Integração", value: EnumEtapasFatura.Intregacao }
];

var _situacaoPesquisa = [
    { text: "Todas", value: "" },
    { text: "Em Andamento", value: EnumSituacoesFatura.EmAndamento },
    { text: "Em Fechamento", value: EnumSituacoesFatura.EmFechamento },
    { text: "Em Cancelamento", value: EnumSituacoesFatura.EmCancelamento },
    { text: "Fechada", value: EnumSituacoesFatura.Fechado },
    { text: "Liquidada", value: EnumSituacoesFatura.Liquidado },
    { text: "Cancelada", value: EnumSituacoesFatura.Cancelado },
    { text: "Sem Regra", value: EnumSituacoesFatura.SemRegraAprovacao },
    { text: "Aguardando Aprovação", value: EnumSituacoesFatura.AguardandoAprovacao },
    { text: "Aprovação Rejeitada", value: EnumSituacoesFatura.AprovacaoRejeitada },
    { text: "Problema com a Integração", value: EnumSituacoesFatura.ProblemaIntegracao }

];

var _etapa = [
    { text: "Fatura", value: EnumEtapasFatura.Fatura },
    { text: "Cargas", value: EnumEtapasFatura.Cargas },
    { text: "Finalização", value: EnumEtapasFatura.Fechamento },
    { text: "Integração", value: EnumEtapasFatura.Intregacao }
];

var _situacao = [
    { text: "Em Andamento", value: EnumSituacoesFatura.EmAndamento },
    { text: "Fechado", value: EnumSituacoesFatura.Fechado },
    { text: "Cancelado", value: EnumSituacoesFatura.Cancelado },
    { text: "Sem Regra", value: EnumSituacoesFatura.SemRegraAprovacao },
    { text: "Aguardando Aprovação", value: EnumSituacoesFatura.AguardandoAprovacao },
    { text: "Aprovação Rejeitada", value: EnumSituacoesFatura.AprovacaoRejeitada }
];

var _tipoPessoa = [
    { text: "Pessoa", value: 1 },
    { text: "Grupo de Pessoas", value: 2 }
];

var _tipoPropostaMultimodal = [
    { text: "Todos", value: 0 },
    { text: "Carga Fechada", value: 1 },
    { text: "Carga Fracionada", value: 2 },
    { text: "Feeder", value: 3 },
    { text: "VAS", value: 4 },
    { text: "Embarque Certo - Feeder", value: 5 },
    { text: "Embarque Certo - Cabotagem", value: 6 },
    { text: "No Show", value: 7 },
    { text: "Demurrage - Cabotagem", value: 9 },
    { text: "Detention - Cabotagem", value: 10 },
];

var _gridFatura, _fatura, _pesquisaFatura, _contatoClienteFatura;
var PesquisaFatura = function () {
    this.NumeroFatura = PropertyEntity({ text: "Número da Fatura: ", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroPreFatura = PropertyEntity({ text: "Pré-Fatura: ", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroCTe = PropertyEntity({ text: "Número do Documento: ", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroCarga = PropertyEntity({ text: "Número da Carga: ", getType: typesKnockout.string, maxlength: 16 });
    this.DataFatura = PropertyEntity({ text: "Data da Fatura: ", getType: typesKnockout.date });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: 1, text: "Tipo de Pessoa: ", eventChange: TipoPessoaPesquisaChange });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Etapa = PropertyEntity({ val: ko.observable(0), options: _etapaPesquisa, def: 0, text: "Etapa: " });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoPesquisa, def: 0, text: "Situação: " });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroDeResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Visible = PropertyEntity({ visible: false, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (_pesquisaFatura.Visible.visibleFade()) {
                _pesquisaFatura.Visible.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                _pesquisaFatura.Visible.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", icon: ko.observable("fal fa-plus"), visible: ko.observable(true)
    });
    this.NumeroNota = PropertyEntity({ text: "Nº Nota: ", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroControleCliente = PropertyEntity({ text: "Nº Controle Cliente:", maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroReferenciaEDI = PropertyEntity({ text: "Nº Ref. EDI:", maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroOS = PropertyEntity({ text: "Nº O.S.:", maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroBooking = PropertyEntity({ text: "Nº Booking:", maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });

    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Terminal de Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Terminal de Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Navio/Viagem/Direção:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroControle = PropertyEntity({ text: "Nº Controle:", maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable([]), options: _tipoPropostaMultimodal, def: [], getType: typesKnockout.selectMultiple, text: "Tipo Proposta: ", required: false, visible: ko.observable(true) });

    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(false), text: ko.observable("Tipo da Operação:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.DataEmissaoInicial = PropertyEntity({ text: "Data Emissão do Título de: ", getType: typesKnockout.date });
    this.DataEmissaoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });
    this.DataVencimentoInicial = PropertyEntity({ text: "Data Vencimento do Título de: ", getType: typesKnockout.date });
    this.DataVencimentoFinal = PropertyEntity({ text: "Até: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFatura.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.EnviarEmailLote = PropertyEntity({ eventClick: EnviarEmailLoteClick, type: types.event, text: "Enviar Faturas em Lote", idGrid: guid(), visible: ko.observable(false) });
};

var Fatura = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Etapa = PropertyEntity({ val: ko.observable(1), options: _etapa, def: 1 });
    this.Situacao = PropertyEntity({ val: ko.observable(1), options: _situacao, def: 1 });

    this.NumeroFatura = PropertyEntity({ text: "Nº Fatura:", required: false, maxlength: 18, getType: typesKnockout.int, enable: ko.observable(false), visible: ko.observable(true), val: ko.observable("") });
    this.NumeroPreFatura = PropertyEntity({ text: "Nº Pré-Fatura:", required: false, maxlength: 18, getType: typesKnockout.int, enable: ko.observable(true), visible: ko.observable(true), val: ko.observable("") });
    this.NumeroFaturaOriginal = PropertyEntity({ text: "Nº Original: ", getType: typesKnockout.int, maxlength: 16, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: 1, text: "*Tipo de Pessoa: ", eventChange: TipoPessoaChange, issue: 306, enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Pessoa:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), issue: 52 });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*Grupo de Pessoas:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), issue: 58 });
    this.GerarDocumentosAutomaticamente = PropertyEntity({ type: types.bool, text: "Gerar documentos automaticamente para esta fatura", issue: 1910, def: false, val: ko.observable(false), enable: ko.observable(true), visible: ko.observable(true) });
    this.GerarDocumentosApenasCanhotosAprovados = PropertyEntity({ type: types.bool, text: "Gerar documentos apenas de Canhotos Aprovados", def: false, val: ko.observable(false), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarOpcaoGerarFaturasApenasCanhotosAprovados) });
    this.NaoUtilizarMoedaEstrangeira = PropertyEntity({ type: types.bool, text: "Não utilizar moeda estrangeira (utilizará o valor em reais dos documentos)", def: false, val: ko.observable(false), enable: ko.observable(true), visible: ko.observable(false) });
    this.NovoModelo = PropertyEntity({ type: types.bool, text: "Novo Modelo de Fatura", def: true, val: ko.observable(true), enable: ko.observable(false), visible: ko.observable(false) });

    this.DataFatura = PropertyEntity({ text: "*Data Fatura: ", getType: typesKnockout.date, enable: ko.observable(true), issue: 331, required: true, def: ko.observable(Global.DataAtual()) });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, enable: ko.observable(true), issue: 331, required: true, val: ko.observable(), def: ("") });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", dateRangeInit: this.DataInicial, getType: typesKnockout.date, enable: ko.observable(true), required: true, issue: 331, val: ko.observable(), def: ("") });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Tipo de Carga:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Tipo de Operação:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*Transportador:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.AliquotaICMS = PropertyEntity({ getType: typesKnockout.decimal, text: "Alíquota do ICMS:", enable: ko.observable(true), visible: ko.observable(false), val: ko.observable(""), maxlength: 5 });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Veículo:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });

    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Navio/Viagem/Direção:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Terminal de Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Terminal de Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Destino:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroBooking = PropertyEntity({ text: "Nº Booking:", maxlength: 300, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoPropostaMultimodal = PropertyEntity({ val: ko.observable([]), options: _tipoPropostaMultimodal, def: [], getType: typesKnockout.selectMultiple, text: "Tipo Proposta: ", required: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.PaisOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("Pais Origem:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Filial:"), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true), required: ko.observable(true) });
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.CentroDeResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.DataBaseCRT = PropertyEntity({ val: ko.observable(), text: ko.observable("Data Base CRT"), getType: typesKnockout.dateTime, required: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira) });
    this.DataBaseAtual = PropertyEntity({ val: ko.observable(Global.DataHoraAtual), getType: typesKnockout.dateTime, visible: false, def: Global.DataHoraAtual });
    this.DataBaseCRT.dateRangeLimit = this.DataBaseAtual;
    this.DataBaseAtual.dateRangeInit = this.DataBaseCRT;

    this.TipoOSConvertido = PropertyEntity({ val: ko.observable([]), options: EnumTipoOSConvertido.obterOpcoesPedido(), def: [], getType: typesKnockout.selectMultiple, text: "Tipo OS Convertido: ", required: false, enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true) });

    this.ListaCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaParcelas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.CancelarFatura = PropertyEntity({ eventClick: CancelarCargaClick, type: types.event, text: "Cancelar Fatura", visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarGatura = PropertyEntity({ eventClick: GerarFaturaClick, type: types.event, text: "Gerar Fatura", visible: ko.observable(true), enable: ko.observable(true) });
    this.ImportarPreFatura = PropertyEntity({ eventClick: ImportarEDIClick, type: types.event, text: "Importar Pré Fatura", visible: ko.observable(true), enable: ko.observable(true), icon: "fal fa-upload" });

};

//*******EVENTOS*******

function carregarLancamentoFatura(idDivConteudo, callback) {
    $.get("Content/Static/Fatura/FaturaModais.html?dyn=" + guid(), function (data) {
        $("#ModaisFatura").html(data);
        $.get("Content/Static/Fatura/ConteudoFatura.html?dyn=" + guid(), function (dataConteudo) {
            $("#" + idDivConteudo).html(dataConteudo);

            $("#knockoutCabecalhoFatura").hide();

            _fatura = new Fatura();
            KoBindings(_fatura, "knockoutCadastroFatura");

            BuscarClientes(_fatura.Pessoa);
            BuscarTiposOperacao(_fatura.TipoOperacao);
            BuscarTransportadores(_fatura.Transportador);
            BuscarGruposPessoas(_fatura.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
            BuscarTiposdeCarga(_fatura.TipoCarga, null, _fatura.GrupoPessoa, null, _fatura.Pessoa);
            BuscarPedidoViagemNavio(_fatura.PedidoViagemNavio);
            BuscarTipoTerminalImportacao(_fatura.TerminalOrigem);
            BuscarTipoTerminalImportacao(_fatura.TerminalDestino);
            BuscarVeiculos(_fatura.Veiculo);
            BuscarLocalidades(_fatura.Origem);
            BuscarLocalidades(_fatura.Destino);
            BuscarPaises(_fatura.PaisOrigem);
            BuscarFilial(_fatura.Filial);
            BuscarClientes(_pesquisaFatura.Tomador);
            BuscarClientes(_fatura.Tomador);
            BuscarCentroResultado(_pesquisaFatura.CentroDeResultado)
            BuscarCentroResultado(_fatura.CentroDeResultado)

            _fatura.DataFatura.val(Global.DataAtual());

            $('#' + _fatura.DataFatura.id).bind('blur', function () { AtualizarDatas(); });
            AtualizarDatas = function () {
                if (_CONFIGURACAO_TMS.PreencherPeriodoFaturaComDataAtual === true) {
                    _fatura.DataInicial.val(_fatura.DataFatura.val());
                    _fatura.DataFinal.val(_fatura.DataFatura.val());
                }
            }

            $("#" + _fatura.DataBaseCRT.id).focusin(function (e) {
                e.preventDefault();
                LimparCampo(_fatura.DataBaseAtual);
            });

            ConfigurarLayoutPorTipoServico();

            buscarFaturas();

            loadCabecalhoFatura();
            loadEtapaFatura();
            loadCarga();
            loadFechamentoFatura();
            loadIntegracaoFatura();
            LoadDocumentoFatura();
            LoadImportacaoEDIFatura();
            LoadSignalRFatura();
            LoadAprovacaoAprovacaoFatura();

            if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal && !_CONFIGURACAO_TMS.HabilitarFuncionalidadesProjetoGollum) {
                $("#camposMultimodalPesquisa").hide();
                _pesquisaFatura.EnviarEmailLote.visible(false);
                _fatura.PedidoViagemNavio.visible(false);
                _fatura.TerminalOrigem.visible(false);
                _fatura.TerminalDestino.visible(false);
                _fatura.TipoPropostaMultimodal.visible(false);
                _fatura.Destino.visible(false);
                _fatura.NumeroBooking.visible(false);
            }
            else
                _fatura.PaisOrigem.visible(false);

            if (!_CONFIGURACAO_TMS.ExigirInformarFilialEmissaoFaturas) {
                _fatura.Filial.required = false;
                _fatura.Filial.text("Filial:");
            }

            HeaderAuditoria("Fatura", _fatura);

            BuscarProximosDadosFatura();

            _contatoClienteFatura = new ContatoCliente("btnContatoCliente", _fatura.Codigo, EnumTipoDocumentoContatoCliente.Fatura);

            if (callback != null)
                callback();

        });
    });
}

function loadFatura() {
    _pesquisaFatura = new PesquisaFatura();
    KoBindings(_pesquisaFatura, "knockoutPesquisaFatura", false, _pesquisaFatura.Pesquisar.id);

    BuscarClientes(_pesquisaFatura.Pessoa);
    BuscarGruposPessoas(_pesquisaFatura.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    BuscarFuncionario(_pesquisaFatura.Operador);
    BuscarTransportadores(_pesquisaFatura.Empresa);

    BuscarTipoTerminalImportacao(_pesquisaFatura.TerminalOrigem);
    BuscarTipoTerminalImportacao(_pesquisaFatura.TerminalDestino);
    BuscarPedidoViagemNavio(_pesquisaFatura.PedidoViagemNavio);
    BuscarLocalidades(_pesquisaFatura.Origem);
    BuscarLocalidades(_pesquisaFatura.Destino);
    BuscarTiposOperacao(_pesquisaFatura.TipoOperacao);

    carregarLancamentoFatura("conteudoFatura");
}

function ConfigurarLayoutPorTipoServico() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _fatura.Pessoa.text("Pessoa:")
        _fatura.Pessoa.required = false;

        _fatura.GrupoPessoa.text("Grupo de Pessoas:")
        _fatura.GrupoPessoa.required = false;

        _fatura.TipoOperacao.text("*Tipo de Operação:");
        _fatura.Transportador.required = true;
    } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
        _fatura.Transportador.text("Empresa/Filial:");
        _fatura.AliquotaICMS.visible(true);
    }

    if (_CONFIGURACAO_TMS.UtilizaMoedaEstrangeira === true)
        _fatura.NaoUtilizarMoedaEstrangeira.visible(true);

    if (_CONFIGURACAO_TMS.NaoObrigarTipoOperacaoFatura && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _fatura.TipoOperacao.required = false;
        _fatura.TipoOperacao.text("Tipo de Operação:");
    }
}

function TipoPessoaChange(e, sender) {
    if (_fatura.TipoPessoa.val() == 1) {
        _fatura.Pessoa.visible(true);
        _fatura.GrupoPessoa.visible(false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _fatura.Pessoa.required = true;
            _fatura.GrupoPessoa.required = false;
        }

        LimparCampoEntity(_fatura.GrupoPessoa);
    } else if (_fatura.TipoPessoa.val() == 2) {
        _fatura.Pessoa.visible(false);
        _fatura.GrupoPessoa.visible(true);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
            _fatura.Pessoa.required = false;
            _fatura.GrupoPessoa.required = true;
        }

        LimparCampoEntity(_fatura.Pessoa);
    }
}

function TipoPessoaPesquisaChange(e, sender) {
    if (_pesquisaFatura.TipoPessoa.val() == 1) {
        _pesquisaFatura.Pessoa.visible(true);
        _pesquisaFatura.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaFatura.GrupoPessoa);
    } else if (_pesquisaFatura.TipoPessoa.val() == 2) {
        _pesquisaFatura.Pessoa.visible(false);
        _pesquisaFatura.GrupoPessoa.visible(true);
        LimparCampoEntity(_pesquisaFatura.Pessoa);
    }
}

//*******MÉTODOS*******

function editarFatura(faturaGrid) {
    limparCamposFatura();
    _fatura.Codigo.val(faturaGrid.Codigo);
    BuscarPorCodigo(_fatura, "Fatura/BuscarPorCodigo", function (arg) {
        _pesquisaFatura.ExibirFiltros.visibleFade(false);

        TipoPessoaChange(); 
        CarregarDadosCabecalho(arg.Data);
        PosicionarEtapa(arg.Data);
        _fatura.CancelarFatura.visible(true);
        $("#knockoutCabecalhoFatura").show();

        _contatoClienteFatura.ShowButton();

    }, null);
}

function ReenviarFatura(faturaGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente reenviar os e-mails desta fatura?", function () {
        executarReST("FaturaIntegracao/EnviarTodosLayoutFatura", { Codigo: faturaGrid.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Solcitação realizada com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        });
    });
}

function buscarFaturas() {
    let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarFatura };
    let reenviarFatura = { descricao: "Reenviar Fatura", id: guid(), evento: "onclick", metodo: ReenviarFatura };
    let menuOpcoes;

    if (!_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal) {
        editar["tamanho"] = "10";
        menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar], tamanho: 5 };
    }
    else
        menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, reenviarFatura], tamanho: 5 };

    let configExportacao = {
        url: "Fatura/ExportarPesquisa",
        titulo: "Faturas"
    };

    _gridFatura = new GridViewExportacao(_pesquisaFatura.Pesquisar.idGrid, "Fatura/Pesquisa", _pesquisaFatura, menuOpcoes, configExportacao, null, null, null);
    _gridFatura.CarregarGrid();
}

function limparCamposFatura() {
    LimparCampos(_fatura);
    _fatura.CancelarFatura.visible(false);
    $("#fdsCargasDaFatura").show();
    $("#fdsPercentualCargasDocumento").hide();
    _fatura.DataFatura.val(Global.DataAtual());
    _etapaFatura.Etapa2.visible(false);
    _etapaFatura.Etapa6.visible(false);
    _fatura.ImportarPreFatura.visible(true);
    _fechamentoFatura.PercentualProcessadoFechamento.visible(false);
    _cabecalhoFatura.PercentualProcessadoCancelamento.val("0%");
    _cabecalhoFatura.PercentualProcessadoCancelamento.visible(false);
    _contatoClienteFatura.HideButton();
    TipoPessoaChange();
}

function BuscarProximosDadosFatura() {
    executarReST("Fatura/BuscarProximosDados", null, function (arg) {
        if (arg.Success) {
            let retorno = arg.Data;

            _fatura.NumeroFatura.val(retorno.Numero);
            _fatura.Etapa.val(EnumEtapasFatura.Documentos);
            _fatura.Situacao.val(EnumSituacoesFatura.EmAndamento);

            if (!_CONFIGURACAO_TMS.PreencherPeriodoFaturaComDataAtual) {
                _fatura.DataInicial.val("");

                if (retorno.Numero > 1) {
                    if (retorno.Data != "") {
                        _fatura.DataInicial.val(retorno.Data);
                    }
                }
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
    });
}

function exibirCamposObrigatorio() {
    exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function ImportarEDIClick() {
    AbrirTelaImportacaoEDIFatura();
}

function EnviarEmailLoteClick(e, sender) {

    exibirConfirmacao("Atenção!", "Deseja enviar os dados da fatura de forma agrupada para os tomadores pendentes de envio contidos no filtro realizado?", function () {
        let data = RetornarObjetoPesquisa(_pesquisaFatura);
        executarReST("Fatura/EnviarFaturaAgrupada", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Solcitação realizada com sucesso");
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        });
    });
}