/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFuncionarioComissao.js" />
/// <reference path="FuncionarioComissaoAprovacao.js" />
/// <reference path="FuncionarioComissaoEtapas.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridFuncionarioComissao;
var _funcionarioComissao;
var _CRUDFuncionarioComissao;
var _pesquisaFuncionarioComissao;
var _gridTitulosFuncionarioComissao;

var _situacaoFuncionarioComissao = [
    { text: "Todos", value: "" },
    { text: "Ag. Aprovação", value: EnumSituacaoFuncionarioComissao.AgAprovacao },
    { text: "Aprovada", value: EnumSituacaoFuncionarioComissao.Aprovada },
    { text: "Cancelado", value: EnumSituacaoFuncionarioComissao.Cancelado },
    { text: "Finalizado", value: EnumSituacaoFuncionarioComissao.Finalizado },
    { text: "Rejeitada", value: EnumSituacaoFuncionarioComissao.Rejeitada },
    { text: "Sem Regra", value: EnumSituacaoFuncionarioComissao.SemRegra }
];

var FuncionarioComissaoTituloMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: ko.observable(0), def: ko.observable(0), getType: typesKnockout.int });
    this.CodigoTitulo = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorISS = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorICMS = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorLiquido = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.PercentualImpostoFederal = PropertyEntity({ type: types.map, val: ko.observable("") });
    this.ValorFinal = PropertyEntity({ type: types.map, val: ko.observable("") });
}

var FuncionarioComissao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.GridTitulos = PropertyEntity({ type: types.local });

    this.Numero = PropertyEntity({ text: "Número: ", getType: typesKnockout.map, enable: false });
    this.DataInicial = PropertyEntity({ text: "*Data Inicial: ", getType: typesKnockout.date, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "*Data Final: ", getType: typesKnockout.date, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.Titulos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.Observacao = PropertyEntity({ text: "Observação: ", getType: typesKnockout.map, enable: ko.observable(true), maxlength: 2000 });

    this.ConsultarTitulo = PropertyEntity({ eventClick: consultarTitulosComissaoClick, type: types.event, text: "Consultar Títulos", enable: ko.observable(true), visible: ko.observable(true) });
    this.ValorTotalFinal = PropertyEntity({ type: types.map, val: ko.observable("0,00"), def: "0,00", text: "Valor Total Final R$:", enable: ko.observable(true) });
    this.QuantidadeTitulos = PropertyEntity({ type: types.map, val: ko.observable(0), def: 0, text: "Quantidade de Títulos:", enable: ko.observable(true) });
}

var CRUDFuncionarioComissao = function () {
    this.Limpar = PropertyEntity({ eventClick: limparCamposComissaoClick, type: types.event, text: "Limpar (Gerar Nova Comissão)", idGrid: guid(), visible: ko.observable(true) });
    this.GerarComissao = PropertyEntity({ eventClick: gerarComissaoClick, type: types.event, text: "Gerar Comissão", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarComissaoClick, type: types.event, text: "Cancelar Comissão", idGrid: guid(), visible: ko.observable(false) });
}

var PesquisaFuncionarioComissao = function () {
    this.DataInicio = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date, visible: true });
    this.DataFim = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date, visible: true });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoFuncionarioComissao, def: "", text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridFuncionarioComissao.CarregarGrid();
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
}


//*******EVENTOS*******

function loadFuncionarioComissao() {
    _funcionarioComissao = new FuncionarioComissao();
    KoBindings(_funcionarioComissao, "knockoutFuncionarioComissao");

    HeaderAuditoria("FuncionarioComissao", _funcionarioComissao);

    _CRUDFuncionarioComissao = new CRUDFuncionarioComissao();
    KoBindings(_CRUDFuncionarioComissao, "knockoutCRUD");

    _pesquisaFuncionarioComissao = new PesquisaFuncionarioComissao();
    KoBindings(_pesquisaFuncionarioComissao, "knockoutPesquisaFuncionarioComissao", false, _pesquisaFuncionarioComissao.Pesquisar.id);

    LoadAutorizacaoFuncionarioComissao();
    LoadEtapasFuncionarioComissao();
    GridTitulosFuncionarioComissao();

    new BuscarFuncionario(_pesquisaFuncionarioComissao.Funcionario);
    new BuscarFuncionario(_funcionarioComissao.Funcionario);

    BuscarFuncionarioComissao();
}

function gerarComissaoClick(e, sender) {
    if (_funcionarioComissao.Titulos.list.length > 0) {
        var listaTitulos = _funcionarioComissao.Titulos.list;
        _funcionarioComissao.Titulos.list = [];

        for (var i = 0; i < listaTitulos.length; i++) {
            var listaTitulosMap = new FuncionarioComissaoTituloMap();
            listaTitulosMap.Codigo.val = listaTitulos[i].Codigo;
            listaTitulosMap.CodigoTitulo.val = listaTitulos[i].CodigoTitulo;
            listaTitulosMap.ValorISS.val = listaTitulos[i].ValorISS;
            listaTitulosMap.ValorICMS.val = listaTitulos[i].ValorICMS;
            listaTitulosMap.ValorLiquido.val = listaTitulos[i].ValorLiquido;
            listaTitulosMap.PercentualImpostoFederal.val = listaTitulos[i].PercentualImpostoFederal;
            listaTitulosMap.ValorFinal.val = listaTitulos[i].ValorFinal;

            _funcionarioComissao.Titulos.list.push(listaTitulosMap);
        }

        _funcionarioComissao.Titulos.text = JSON.stringify(_funcionarioComissao.Titulos.list);

        Salvar(_funcionarioComissao, "FuncionarioComissao/Adicionar", function (arg) {
            _funcionarioComissao.Titulos.list = listaTitulos;
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Comissão salva com sucesso");
                    _gridFuncionarioComissao.CarregarGrid();
                    LimparCamposComissao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    } else {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Favor pesquisar os títulos para gerar a comissão!");
    }
}

function cancelarComissaoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar a Comissão do Funcionário " + _funcionarioComissao.Funcionario.val() + "?", function () {
        executarReST("FuncionarioComissao/CancelarPorCodigo", { Codigo: _funcionarioComissao.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelado com sucesso.");
                    _gridFuncionarioComissao.CarregarGrid();
                    LimparCamposComissao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function limparCamposComissaoClick(e, sender) {
    LimparCamposComissao();
}

function consultarTitulosComissaoClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_funcionarioComissao);

    if (valido) {
        var data = {
            CodigoFuncionario: _funcionarioComissao.Funcionario.codEntity(),
            DataInicial: _funcionarioComissao.DataInicial.val(),
            DataFinal: _funcionarioComissao.DataFinal.val()
        };
        executarReST("FuncionarioComissao/ConsultarTitulosPorFuncionario", data, function (arg) {
            if (arg.Success) {
                _funcionarioComissao.Titulos.list = [];
                if (arg.Data.Titulos.length > 0) {
                    var titulos = arg.Data.Titulos;
                    for (var i = 0; i < titulos.length; i++) {
                        _funcionarioComissao.Titulos.list.push(titulos[i]);
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", "Nenhum título localizado para o Funcionário no período");
                }
                RecarregarGridTitulosFuncionarioComissao();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        })
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

//*******MÉTODOS*******
function BuscarFuncionarioComissao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarComissao, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridFuncionarioComissao = new GridView(_pesquisaFuncionarioComissao.Pesquisar.idGrid, "FuncionarioComissao/Pesquisa", _pesquisaFuncionarioComissao, menuOpcoes);
    _gridFuncionarioComissao.CarregarGrid();
}

function editarComissao(itemGrid) {
    // Limpa os campos
    LimparCamposComissao();

    // Esconde filtros
    _pesquisaFuncionarioComissao.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarComissaoPorCodigo(itemGrid.Codigo);
}

function GridTitulosFuncionarioComissao() {
    if (_gridTitulosFuncionarioComissao && _gridTitulosFuncionarioComissao.Destroy)
        _gridTitulosFuncionarioComissao.Destroy();
    _funcionarioComissao.Titulos.get$().empty();

    if (_funcionarioComissao.Codigo.val() > 0) {
        _gridTitulosFuncionarioComissao = new GridView(_funcionarioComissao.GridTitulos.id, "FuncionarioComissao/PesquisaTitulos", _funcionarioComissao, { column: 1, dir: orderDir.desc });
        _gridTitulosFuncionarioComissao.CarregarGrid();
    } else {
        var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 5, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirTituloClick }] };

        var header = [
            { data: "Codigo", visible: false },
            { data: "CodigoTitulo", title: "Nº Título", width: "6%" },
            { data: "Pessoa", title: "Pessoa", width: "12%" },
            { data: "CPFCNPJ", title: "CPF/CNPJ", width: "7%" },
            { data: "NumeroFatura", title: "Nº Fatura", width: "6%" },
            { data: "DataEmissao", title: "Emissão", width: "7%" },
            { data: "DataVencimento", title: "Vencimento", width: "7%" },
            { data: "DataLiquidacao", title: "Liquidação", width: "7%" },
            { data: "ValorOriginal", title: "Valor Original", width: "7%" },
            { data: "ValorPago", title: "Valor Pago", width: "6%" },
            { data: "ValorISS", title: "ISS", width: "6%" },
            { data: "ValorICMS", title: "ICMS", width: "6%" },
            { data: "ValorLiquido", title: "Valor Líquido", width: "6%" },
            { data: "PercentualImpostoFederal", title: "% Imp. Federal", width: "6%" },
            { data: "ValorFinal", title: "Valor Final", width: "7%" }
        ];

        _gridTitulosFuncionarioComissao = new BasicDataTable(_funcionarioComissao.GridTitulos.id, header, menuOpcoes, { column: 1, dir: orderDir.desc }, null, 5);

        RecarregarGridTitulosFuncionarioComissao();
    }
}

function ExcluirTituloClick(data) {
    for (var i = 0, s = _funcionarioComissao.Titulos.list.length; i < s; i++) {
        if (data.Codigo == _funcionarioComissao.Titulos.list[i].Codigo) {
            _funcionarioComissao.Titulos.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridTitulosFuncionarioComissao();
}

function RecarregarGridTitulosFuncionarioComissao() {
    var data = [];
    var valorFinal = 0.00;
    var quantidadeTitulos = 0;

    $.each(_funcionarioComissao.Titulos.list, function (i, titulo) {
        var itemGrid = new Object();

        itemGrid.Codigo = titulo.Codigo;
        itemGrid.CodigoTitulo = titulo.CodigoTitulo;
        itemGrid.Pessoa = titulo.Pessoa;
        itemGrid.CPFCNPJ = titulo.CPFCNPJ;
        itemGrid.NumeroFatura = titulo.NumeroFatura;
        itemGrid.DataEmissao = titulo.DataEmissao;
        itemGrid.DataVencimento = titulo.DataVencimento;
        itemGrid.DataLiquidacao = titulo.DataLiquidacao;
        itemGrid.ValorOriginal = titulo.ValorOriginal;
        itemGrid.ValorPago = titulo.ValorPago;
        itemGrid.ValorISS = titulo.ValorISS;
        itemGrid.ValorICMS = titulo.ValorICMS;
        itemGrid.ValorLiquido = titulo.ValorLiquido;
        itemGrid.PercentualImpostoFederal = titulo.PercentualImpostoFederal;
        itemGrid.ValorFinal = titulo.ValorFinal;

        data.push(itemGrid);

        quantidadeTitulos = quantidadeTitulos + 1;
        valorFinal = valorFinal + Globalize.parseFloat(titulo.ValorFinal);
    });

    _gridTitulosFuncionarioComissao.CarregarGrid(data);
    _funcionarioComissao.ValorTotalFinal.val(Globalize.format(valorFinal, "n2"));
    _funcionarioComissao.QuantidadeTitulos.val(quantidadeTitulos);
}

function BuscarComissaoPorCodigo(codigo, callback) {
    executarReST("FuncionarioComissao/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            EditarComissao(arg.Data);

            ListarAprovacoes(arg.Data);

            GridTitulosFuncionarioComissao();

            SetarEtapasFuncionarioComissao();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        if (callback != null)
            callback();
    }, null);
}

function EditarComissao(data) {
    _funcionarioComissao.Situacao.val(data.Situacao);
    _funcionarioComissao.Codigo.val(data.Codigo);

    PreencherObjetoKnout(_funcionarioComissao, { Data: data });

    _CRUDFuncionarioComissao.GerarComissao.visible(false);
    _CRUDFuncionarioComissao.Cancelar.visible(true);

    ControleCamposComissao(false);
    ControleCamposCRUD();
}

function LimparCamposComissao() {
    LimparCampos(_funcionarioComissao);
    LimparCampos(_funcionarioComissao);

    ControleCamposCRUD();
    _CRUDFuncionarioComissao.GerarComissao.visible(true);

    ControleCamposComissao(true);
    GridTitulosFuncionarioComissao();

    SetarEtapaInicioFuncionarioComissao();
}

function ControleCamposCRUD() {
    var situacao = _funcionarioComissao.Situacao.val();

    _CRUDFuncionarioComissao.Limpar.visible(true);
    _CRUDFuncionarioComissao.GerarComissao.visible(false);
    _CRUDFuncionarioComissao.Cancelar.visible(false);
    if (situacao == EnumSituacaoFuncionarioComissao.Aprovada) {
        _CRUDFuncionarioComissao.Cancelar.visible(true);
    }
}

function ControleCamposComissao(status) {
    _funcionarioComissao.Funcionario.enable(status);
    _funcionarioComissao.DataInicial.enable(status);
    _funcionarioComissao.DataFinal.enable(status);
    _funcionarioComissao.ConsultarTitulo.visible(status);
    _funcionarioComissao.Observacao.enable(status);
}