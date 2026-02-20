//*******MAPEAMENTO KNOUCKOUT*******

var _situacaoTituloPesquisa = [
    { text: "Todos", value: "" },
    { text: "Iniciada", value: 1 },
    { text: "Negociação", value: 2 },
    { text: "Finalizada", value: 3 },
    { text: "Cancelada", value: 4 },
    { text: "Em Geração", value: 5 },
    { text: "Em Finalização", value: 6 }
];

var _etapaBaixa = [
    { text: "Iniciada", value: 1 },
    { text: "Negociação", value: 2 },
    { text: "Finalizada", value: 3 },
    { text: "Cancelada", value: 4 }
];

var _tipoPessoa = [
    { text: "Pessoa", value: 1 },
    { text: "Grupo de Pessoa", value: 2 }
];

var _titulosDeAgrupamento = [
    { text: "Todos", value: "" },
    { text: "Gerado de negociação", value: 1 },
    { text: "Não gerado de negociação", value: 0 }
];
var _PermissoesPersonalizadas;
var _gridBaixaTitulosReceber, _baixaTituloReceber, _pesquisaBaixaTituloReceber, _gridTitulosReceberPendentes;

var PesquisaBaixaTituloReceber = function () {
    this.NumeroTitulo = PropertyEntity({ text: "Número do Título:", getType: typesKnockout.int, maxlength: 16 });
    this.NumeroFatura = PropertyEntity({ text: "Número da Fatura:", getType: typesKnockout.int, maxlength: 16, visible: ko.observable(true) });
    this.DataInicial = PropertyEntity({ text: "Data Baixa Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Baixa Final:", getType: typesKnockout.date });
    this.DataBaseInicial = PropertyEntity({ text: "Data Base Inicial:", getType: typesKnockout.date });
    this.DataBaseFinal = PropertyEntity({ text: "Data Base Final:", getType: typesKnockout.date });
    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: 1, text: "Tipo de Pessoa:", eventChange: TipoPessoaPesquisaChange });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Pessoa:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ val: ko.observable(0), options: _situacaoTituloPesquisa, def: 0, text: "Situação:" });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Conhecimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Pagamento:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NumeroDocOriginario = PropertyEntity({ text: "N° Doc Originário:", getType: typesKnockout.int, maxlength: 16 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBaixaTitulosReceber.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var BaixaTituloReceber = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Etapa = PropertyEntity({ val: ko.observable(1), options: _etapaBaixa, def: 1 });
    this.CodigoFatura = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.TipoPessoa = PropertyEntity({ val: ko.observable(1), options: _tipoPessoa, def: 1, text: "Tipo Pessoa: ", eventChange: TipoPessoaTitulosPendentesChange, enable: ko.observable(true) });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoa:", idBtnSearch: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TitulosDeAgrupamento = PropertyEntity({ val: ko.observable(""), options: _titulosDeAgrupamento, def: "", text: "Agrupado: ", enable: ko.observable(true) });

    this.Fatura = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fatura:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Conhecimento = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "CTe:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Vencto. Inicial: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataFinal = PropertyEntity({ text: "Vencto. Final: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoInicial = PropertyEntity({ text: "Emissão Inicial: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataEmissaoFinal = PropertyEntity({ text: "Emissão Final: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.NumeroTitulo = PropertyEntity({ text: "Número do Título: ", getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true), visible: true, configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.NumeroPedido = PropertyEntity({ text: "Número Pedido: ", getType: typesKnockout.string, enable: ko.observable(true), visible: ko.observable(true) });
    this.NumeroOcorrencia = PropertyEntity({ text: "Número da Ocorrência: ", enable: ko.observable(true), visible: ko.observable(true) });
    this.PesquisarTitulosPendentes = PropertyEntity({ eventClick: PesquisarTitulosPendentesClick, type: types.event, text: "Pesquisar", visible: ko.observable(true), enable: ko.observable(true) });
    this.NumeroDocumentoOriginario = PropertyEntity({ text: "Nº Doc. Originário: ", getType: typesKnockout.int, val: ko.observable(""), enable: ko.observable(true), visible: true, configInt: { precision: 0, allowZero: false, thousands: '' } });
    this.Valor = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Valor: ", enable: ko.observable(true), visible: ko.observable(true) });

    this.DataProgramacaoPagamentoInicial = PropertyEntity({ text: "Progr. Inicial: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });
    this.DataProgramacaoPagamentoFinal = PropertyEntity({ text: "Progr. Final: ", getType: typesKnockout.date, enable: ko.observable(true), required: false });

    this.TitulosPendentes = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaTitulos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.NumeroFatura = PropertyEntity({ text: "Número da Fatura: ", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true), visible: false });
    this.ParcelaTitulo = PropertyEntity({ text: "Parcela: ", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true), visible: false });
    this.ValorPendenteTitulo = PropertyEntity({ text: "Valor: ", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true), visible: true });
    this.ValorPendenteTituloMoeda = PropertyEntity({ text: "Valor em Moeda: ", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.DataBaixa = PropertyEntity({ text: "*Data baixa: ", getType: typesKnockout.date, enable: ko.observable(true), required: true, val: ko.observable(Global.DataAtual()) });
    this.DataBase = PropertyEntity({ text: "*Data base: ", getType: typesKnockout.date, enable: ko.observable(true), required: true });

    this.DataBaseCRT = PropertyEntity({ text: "*Data base CRT:", getType: typesKnockout.dateTime, enable: ko.observable(false), visible: ko.observable(false), required: false });

    this.ValorBaixado = PropertyEntity({ getType: typesKnockout.decimal, required: false, text: "*Valor:", maxlength: 15, val: ko.observable("0,00"), def: ko.observable("0,00"), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 300, enable: ko.observable(true) });

    this.ListaParcelasNegociacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CTesRemovidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.PercentualProcessadoGeracao = PropertyEntity({ val: ko.observable("0%"), def: "0%", text: ko.observable("Gerando Informações para esta Baixa"), visible: ko.observable(false) });

    this.SalvarObservacao = PropertyEntity({ eventClick: SalvarObservacaoClick, type: types.event, text: "Salvar Observação", visible: ko.observable(false), enable: ko.observable(true) });
    this.SalvarDatas = PropertyEntity({ eventClick: SalvarDatasClick, type: types.event, text: "Salvar Datas", visible: ko.observable(false), enable: ko.observable(true) });
    this.CancelarBaixa = PropertyEntity({ eventClick: CancelarBaixaClick, type: types.event, text: "Cancelar Baixa", visible: ko.observable(false), enable: ko.observable(true) });
    this.GerarBaixa = PropertyEntity({ eventClick: GerarBaixaClick, type: types.event, text: "Gerar Baixa", visible: ko.observable(true), enable: ko.observable(true) });

    this.ImportarPlanilha = PropertyEntity({
        type: types.local,
        text: "Importar Planilha",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "BaixaTituloReceberNovo/Importar",
        UrlConfiguracao: "BaixaTituloReceberNovo/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O011_BaixaTituloAPagar,
        CallbackSomenteSeSucesso: false,
        FecharModalSeSucesso: false,
        CallbackImportacao: function (r) {
            _baixaTituloReceber.ValorPendenteTitulo.val(r.Data.Retorno.ValorPendente);
            _gridTitulosReceberPendentes.AtualizarRegistrosSelecionados(r.Data.Retorno.Titulos);
            _gridTitulosReceberPendentes.CarregarGrid();
        },
        ParametrosRequisicao: function () {
            return {};
        }
    });
};

//*******EVENTOS*******

function loadBaixaTitulosReceber() {
    _pesquisaBaixaTituloReceber = new PesquisaBaixaTituloReceber();
    KoBindings(_pesquisaBaixaTituloReceber, "knockoutPesquisaTituloReceber");

    _baixaTituloReceber = new BaixaTituloReceber();
    KoBindings(_baixaTituloReceber, "knockoutCadastroTituloReceber");

    HeaderAuditoria("TituloBaixa", _baixaTituloReceber);

    new BuscarClientes(_pesquisaBaixaTituloReceber.Pessoa);
    new BuscarGruposPessoas(_pesquisaBaixaTituloReceber.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarFuncionario(_pesquisaBaixaTituloReceber.Operador);
    new BuscarConhecimentoNotaReferencia(_pesquisaBaixaTituloReceber.Conhecimento, RetornoBuscarPesquisaConhecimento, null);
    new BuscarCargas(_pesquisaBaixaTituloReceber.Carga, null, null, [EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.AgIntegracao]);
    new BuscarTipoPagamento(_pesquisaBaixaTituloReceber.TipoPagamento);

    new BuscarGruposPessoas(_baixaTituloReceber.GrupoPessoa, null, null, null, EnumTipoGrupoPessoas.Clientes);
    new BuscarClientes(_baixaTituloReceber.Pessoa);
    new BuscarFatura(_baixaTituloReceber.Fatura, RetornoBuscarFatura);
   
    new BuscarConhecimentoNotaReferencia(_baixaTituloReceber.Conhecimento);
    new BuscarCargas(_baixaTituloReceber.Carga, null, null, [EnumSituacoesCarga.EmTransporte, EnumSituacoesCarga.Encerrada, EnumSituacoesCarga.AgIntegracao]);

    BuscarBaixaTitulosReceber();
    BuscarTitulosAReceberPendentes();

    loadCabecalhoBaixaTituloReceber();
    loadEtapaBaixaTituloReceber();
    loadNegociacaoBaixa();
    loadIntegracaoBaixa();
    LoadSignalRBaixaTituloReceber();
    carregarLancamentoCheque("conteudoCheque");

    buscarDadosOperador();

}

function buscarDadosOperador() {
    executarReST("Usuario/DadosUsuarioLogado", null, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.FinanceiroBloquearInformacaoDataBaixaReceberCte, _PermissoesPersonalizadas) && !r.Data.UsuarioAdministrador) {
                    _baixaTituloReceber.DataBaixa.val(Global.DataAtual());
                    _baixaTituloReceber.DataBaixa.enable(false);
                }
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RetornoBuscarFatura(data) {
    _baixaTituloReceber.Fatura.codEntity(data.Codigo);
    _baixaTituloReceber.Fatura.val(data.Numero);
}

function RetornoBuscarPesquisaConhecimento(data) {
        _pesquisaBaixaTituloReceber.Conhecimento.codEntity(data.Codigo);
        _pesquisaBaixaTituloReceber.Conhecimento.val(data.Numero + "-" + data.Serie);
}

function RetornoBuscarConhecimento(data) {
    _baixaTituloReceber.Conhecimento.codEntity(data.Codigo);
    _baixaTituloReceber.Conhecimento.val(data.Numero + "-" + data.Serie);
}

function TipoPessoaPesquisaChange(e, sender) {
    if (_pesquisaBaixaTituloReceber.TipoPessoa.val() == 1) {
        _pesquisaBaixaTituloReceber.Pessoa.visible(true);
        _pesquisaBaixaTituloReceber.GrupoPessoa.visible(false);
        LimparCampoEntity(_pesquisaBaixaTituloReceber.GrupoPessoa);
    } else if (_pesquisaBaixaTituloReceber.TipoPessoa.val() == 2) {
        _pesquisaBaixaTituloReceber.Pessoa.visible(false);
        _pesquisaBaixaTituloReceber.GrupoPessoa.visible(true);
        LimparCampoEntity(_pesquisaBaixaTituloReceber.Pessoa);
    }
}

function TipoPessoaTitulosPendentesChange(e, sender) {
    if (_baixaTituloReceber.TipoPessoa.val() == 1) {
        _baixaTituloReceber.Pessoa.visible(true);
        _baixaTituloReceber.GrupoPessoa.visible(false);
        LimparCampoEntity(_baixaTituloReceber.GrupoPessoa);
    } else if (_baixaTituloReceber.TipoPessoa.val() == 2) {
        _baixaTituloReceber.Pessoa.visible(false);
        _baixaTituloReceber.GrupoPessoa.visible(true);
        LimparCampoEntity(_baixaTituloReceber.Pessoa);
    }
}

function PesquisarTitulosPendentesClick(e, sender) {
    _gridTitulosReceberPendentes.CarregarGrid();
}

function GerarBaixaClick(e, sender) {

    var titulosSelecionados = null;

    if (_baixaTituloReceber.SelecionarTodos.val()) {
        titulosSelecionados = _gridTitulosReceberPendentes.ObterMultiplosNaoSelecionados();
    } else {
        titulosSelecionados = _gridTitulosReceberPendentes.ObterMultiplosSelecionados();
    }

    var codigosTitulos = new Array();

    for (var i = 0; i < titulosSelecionados.length; i++)
        codigosTitulos.push(titulosSelecionados[i].DT_RowId);

    if (codigosTitulos && (codigosTitulos.length > 0 || _baixaTituloReceber.SelecionarTodos.val())) {
        _baixaTituloReceber.ListaTitulos.val(JSON.stringify(codigosTitulos));

        //executarReST("BaixaTituloReceberNovo/ObterDetalhesTitulosPendentesSelecionados", RetornarObjetoPesquisa(_baixaTituloReceber), function (r) {
        //    if (r.Success) {
        //        if (r.Data) {

        //            var valorTotal = 0;
        //            var possuiMoedaEstrangeira = false;

        //            for (var i = 0; i < r.Data.length; i++) {
        //                var detalhesTitulos = r.Data[i];

        //                valorTotal += detalhesTitulos.ValorOriginal;

        //                if (detalhesTitulos.Moeda != null && detalhesTitulos.Moeda != EnumMoedaCotacaoBancoCentral.Real) 
        //                    possuiMoedaEstrangeira = true;
        //            }

        //            if (possuiMoedaEstrangeira) {
        //                _baixaTituloReceber.DataBaseCRT.visible(true);
        //                _baixaTituloReceber.DataBaseCRT.enable(true);
        //                _baixaTituloReceber.DataBaseCRT.required = true;
        //            }

        //            _baixaTituloReceber.ValorPendenteTitulo.val(Globalize.format(valorTotal, "n2"));
        //        } else {
        //            exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
        //        }
        //    } else {
        //        exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        //    }
        //});
    } else {
        _baixaTituloReceber.ListaTitulos.val("");
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário selecionar ao menos um título para gerar a baixa.");
    }

    Salvar(_baixaTituloReceber, "BaixaTituloReceberNovo/GerarBaixa", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                DesabilitaCamposTitulosPendentes(false);
                _gridTitulosReceberPendentes.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

//*******MÉTODOS*******

function EditarTituloReceber(tituloReceberGrid) {
    LimparCamposBaixaTituloReceber(false);
    _baixaTituloReceber.Codigo.val(tituloReceberGrid.Codigo);
    BuscarPorCodigo(_baixaTituloReceber, "BaixaTituloReceberNovo/BuscarPorCodigo", function (arg) {
        _pesquisaBaixaTituloReceber.ExibirFiltros.visibleFade(false);

        CarregarDadosCabecalho(arg.Data);
        PosicionarEtapa(arg.Data);
        _gridTitulosReceberPendentes.CarregarGrid();   

        $("#knockoutCabecalhoTituloReceber").show();
    });
}

function BuscarBaixaTitulosReceber() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarTituloReceber, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBaixaTitulosReceber = new GridView(_pesquisaBaixaTituloReceber.Pesquisar.idGrid, "BaixaTituloReceberNovo/Pesquisa", _pesquisaBaixaTituloReceber, menuOpcoes, null, null, null);
    _gridBaixaTitulosReceber.CarregarGrid();
}

function BuscarTitulosAReceberPendentes() {

    _baixaTituloReceber.SelecionarTodos.visible(true);
    _baixaTituloReceber.SelecionarTodos.val(false);

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () {
            AtualizarValorPendente();
        },
        callbackNaoSelecionado: function () {
            AtualizarValorPendente();
        },
        eventos: function () {
        },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _baixaTituloReceber.SelecionarTodos,
        somenteLeitura: false
    };

    _gridTitulosReceberPendentes = new GridView(_baixaTituloReceber.TitulosPendentes.idGrid, "BaixaTituloReceberNovo/PesquisaTitulosPendentes", _baixaTituloReceber, null, null, 20, null, null, null, multiplaescolha);
    _gridTitulosReceberPendentes.CarregarGrid();
}

function LimparCamposBaixaTituloReceber(carregargrid) {
    LimparCampos(_baixaTituloReceber);
    LimparCampos(_progressNegociacaoBaixaReceber);
    DesabilitaCamposTitulosPendentes(true);
    _baixaTituloReceber.ValorBaixado.val("0,00");
    _baixaTituloReceber.CancelarBaixa.visible(false);
    _baixaTituloReceber.SalvarObservacao.visible(false);
    _baixaTituloReceber.GerarBaixa.enable(true);
    _gridTitulosReceberPendentes.AtualizarRegistrosSelecionados([]);
    if (carregargrid)
        _gridTitulosReceberPendentes.CarregarGrid();
    $("#knockoutCabecalhoTituloReceber").hide();
}

function CarregarListaTitulos() {
    var titulosSelecionados = _gridTitulosReceberPendentes.ObterMultiplosSelecionados();

    if (titulosSelecionados.length > 0) {
        var dataGrid = new Array();

        $.each(titulosSelecionados, function (i, titulo) {

            var obj = new Object();
            obj.Codigo = titulo.Codigo;
            obj.CNPJPessoa = titulo.CNPJPessoa;
            obj.CodigoTitulo = titulo.CodigoTitulo;
            obj.NumeroParcela = titulo.NumeroParcela;
            obj.DataVencimento = titulo.DataVencimento;
            obj.Pessoa = titulo.Pessoa;
            obj.Valor = titulo.Valor;

            dataGrid.push(obj);
        });

        _baixaTituloReceber.ListaTitulos.val(JSON.stringify(dataGrid));
    }
}

function DesabilitaCamposTitulosPendentes(v) {
    _baixaTituloReceber.TipoPessoa.enable(v);
    _baixaTituloReceber.GrupoPessoa.enable(v);
    _baixaTituloReceber.Pessoa.enable(v);
    _baixaTituloReceber.NumeroPedido.enable(v);
    _baixaTituloReceber.NumeroOcorrencia.enable(v);
    _baixaTituloReceber.TitulosDeAgrupamento.enable(v);
    _baixaTituloReceber.Carga.enable(v);
    _baixaTituloReceber.Fatura.enable(v);
    _baixaTituloReceber.Conhecimento.enable(v);
    _baixaTituloReceber.DataInicial.enable(v);
    _baixaTituloReceber.DataFinal.enable(v);
    _baixaTituloReceber.SelecionarTodos.enable(v);
    _baixaTituloReceber.PesquisarTitulosPendentes.enable(v);
    _baixaTituloReceber.TitulosPendentes.enable(v);
    _gridTitulosReceberPendentes.SetarRegistrosSomenteLeitura(!v);
}

function AtualizarValorPendente() {

    if (_baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Finalizada || _baixaTituloReceber.Etapa.val() === EnumEtapasBaixaTituloPagar.Cancelada) {
        return;
    }

    var titulosSelecionados = null;

    if (_baixaTituloReceber.SelecionarTodos.val()) {
        titulosSelecionados = _gridTitulosReceberPendentes.ObterMultiplosNaoSelecionados();
    } else {
        titulosSelecionados = _gridTitulosReceberPendentes.ObterMultiplosSelecionados();
    }

    var codigosTitulos = new Array();

    for (var i = 0; i < titulosSelecionados.length; i++)
        codigosTitulos.push(titulosSelecionados[i].DT_RowId);

    if (codigosTitulos && (codigosTitulos.length > 0 || _baixaTituloReceber.SelecionarTodos.val())) {
        _baixaTituloReceber.ListaTitulos.val(JSON.stringify(codigosTitulos));

        executarReST("BaixaTituloReceberNovo/ObterDetalhesTitulosPendentesSelecionados", RetornarObjetoPesquisa(_baixaTituloReceber), function (r) {
            if (r.Success) {
                if (r.Data) {
                    var valorTotal = 0;
                    var valorTotalMoeda = 0;
                    var possuiMoedaEstrangeira = false;

                    for (var i = 0; i < r.Data.length; i++) {
                        var detalhesTitulos = r.Data[i];

                        valorTotal += detalhesTitulos.ValorOriginal;
                        valorTotalMoeda += detalhesTitulos.ValorOriginalMoeda;

                        if (detalhesTitulos.Moeda != null && detalhesTitulos.Moeda != EnumMoedaCotacaoBancoCentral.Real)
                            possuiMoedaEstrangeira = true;
                    }

                    _baixaTituloReceber.DataBaseCRT.visible(possuiMoedaEstrangeira);
                    _baixaTituloReceber.DataBaseCRT.enable(possuiMoedaEstrangeira);
                    _baixaTituloReceber.DataBaseCRT.required = possuiMoedaEstrangeira;
                    _baixaTituloReceber.ValorPendenteTituloMoeda.visible(possuiMoedaEstrangeira)

                    _baixaTituloReceber.ValorPendenteTituloMoeda.val(Globalize.format(valorTotalMoeda, "n2"));
                    _baixaTituloReceber.ValorPendenteTitulo.val(Globalize.format(valorTotal, "n2"));
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    } else {
        _baixaTituloReceber.ListaTitulos.val("");

        _baixaTituloReceber.DataBaseCRT.visible(false);
        _baixaTituloReceber.DataBaseCRT.enable(false);
        _baixaTituloReceber.DataBaseCRT.required = false;
        _baixaTituloReceber.ValorPendenteTituloMoeda.visible(false)

        _baixaTituloReceber.ValorPendenteTituloMoeda.val("0,00");
        _baixaTituloReceber.ValorPendenteTitulo.val("0,00");
    }

}

function SalvarObservacaoClick(e, sender) {
    Salvar(_baixaTituloReceber, "BaixaTituloReceberNovo/SalvarObservacao", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function SalvarDatasClick(e, sender) {
    if (_baixaTituloReceber == null || _baixaTituloReceber.Codigo == null || _baixaTituloReceber.Codigo.val() == null || _baixaTituloReceber.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor, inicie uma baixa antes de salvar as Datas.");
        return;
    }

    Salvar(_baixaTituloReceber, "BaixaTituloReceberNovo/SalvarDatas", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                CarregarDadosCabecalho(arg.Data);
                PosicionarEtapa(arg.Data);
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function CancelarBaixaClick(e, sender) {
    executarReST("BaixaTituloReceberNovo/ValidarCancelamentoBaixa", { Codigo: _baixaTituloReceber.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!r.Data.Valido) {
                    if (r.Data.PermiteCancelarBaixa) {
                        exibirConfirmacao("Confirmação", r.Data.Mensagem, function () {
                            FinalizarCancelamentoBaixa();
                        }, null, "Confirmar", "Cancelar");
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", r.Data.Mensagem, 30000);
                    }
                } else {
                    exibirConfirmacao("Confirmação", "Deseja realmente cancelar esta baixa?", function () {
                        FinalizarCancelamentoBaixa();
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function FinalizarCancelamentoBaixa() {
    Salvar(_baixaTituloReceber, "BaixaTituloReceberNovo/CancelarBaixa", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Baixa cancelada com sucesso.");

                LimparCamposBaixaTituloReceber(true);
                LimparCampos(_cabecalhoBaixaTituloReceber);
                LimparCampos(_integracaoBaixa);
                LimparCampos(_negociacaoBaixa);

                var data = {
                    Etapa: EnumEtapasBaixaTituloReceber.Iniciada
                };

                PosicionarEtapa(data);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

