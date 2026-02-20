/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../Consultas/LayoutEDI.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PlanilhaMap = function () {
    this.Codigo = PropertyEntity({ type: types.map, val: 0, def: 0, getType: typesKnockout.int });
    this.DescricaoTipoCampo = PropertyEntity({ type: types.map, val: "" });
    this.TipoCampo = PropertyEntity({ type: types.map, val: "" });
    this.DescricaoColunaPlanilha = PropertyEntity({ type: types.map, val: "" });
    this.ColunaPlanilha = PropertyEntity({ type: types.map, val: "" });
    this.Posicao = PropertyEntity({ type: types.map, val: "" });
}

var _gridListaColunas;
var _gridConfiguracaoAbastecimento;
var _configuracaoAbastecimento;
var _pesquisaConfiguracaoAbastecimento;

var _pesquisaTipoImportacao = [{ text: "Todos", value: 0 }, { text: "EDI", value: 1 }, { text: "Posto Amigão/Iguaçu", value: 2 }, { text: "Planilha", value: 3 }, { text: "Interno", value: 4 }, { text: "Posto Reforço 4", value: 5 }];
var _tipoImportacao = [{ text: "EDI", value: 1 }, { text: "Posto Amigão/Iguaçu", value: 2 }, { text: "Planilha", value: 3 }, { text: "Interno", value: 4 }, { text: "Posto Reforço 4", value: 5 }];

var _tipoCampo = [
    { text: "Alfanumérico", value: 0 },
    { text: "Numérico", value: 1 },
    { text: "Decimal", value: 2 },
    { text: "Data", value: 3 },
    { text: "Hora", value: 4 },
    { text: "Data E Hora", value: 5 }
];

var _colunaPlanilha = [
    { text: "Data", value: 0 },
    { text: "Hora", value: 1 },
    { text: "Data e Hora", value: 2 },
    { text: "Código Produto", value: 3 },
    { text: "Descrição Produto", value: 4 },
    { text: "Quantidade", value: 5 },
    { text: "Valor Unitário", value: 6 },
    { text: "Valor Total", value: 7 },
    { text: "CNPJ Posto", value: 8 },
    { text: "Nome Posto", value: 9 },    
    { text: "Placa", value: 10 },
    { text: "KM Abastecimento", value: 11 },
    { text: "KM Anterior", value: 12 },
    { text: "Número Cupom", value: 13 },
    { text: "Número Nota", value: 14 },
    { text: "Nome Motorista", value: 15 },
    { text: "CPF Motorista", value: 16 },
    { text: "Horimetro", value: 17 },
    { text: "Placa Veículo Letras", value: 18 },
    { text: "Placa Veículo Número", value: 19 },
    { text: "Endereço Posto", value: 20 },
    { text: "Data Base CRT", value: 21 },
    { text: "Valor Moeda Estrangeira", value: 22 },
    { text: "Local de Armazenamento", value: 23 }
];

var _posicao = [
    { text: "1", value: 1 },
    { text: "2", value: 2 },
    { text: "3", value: 3 },
    { text: "4", value: 4 },
    { text: "5", value: 5 },
    { text: "6", value: 6 },
    { text: "7", value: 7 },
    { text: "8", value: 8 },
    { text: "9", value: 9 },
    { text: "10", value: 10 },
    { text: "11", value: 11 },
    { text: "12", value: 12 },
    { text: "13", value: 13 },
    { text: "14", value: 14 },
    { text: "15", value: 15 },
    { text: "16", value: 16 },
    { text: "17", value: 17 },
    { text: "18", value: 18 },
    { text: "19", value: 19 },
    { text: "20", value: 20 },
    { text: "21", value: 21 },
    { text: "22", value: 22 },
    { text: "23", value: 23 },
    { text: "24", value: 24 },
    { text: "25", value: 25 },
    { text: "26", value: 26 },
    { text: "27", value: 27 },
    { text: "28", value: 28 },
    { text: "29", value: 29 },
    { text: "30", value: 30 },
    { text: "31", value: 31 },
    { text: "32", value: 32 },
    { text: "33", value: 33 },
    { text: "34", value: 34 },
    { text: "35", value: 35 },
    { text: "36", value: 36 },
    { text: "37", value: 37 },
    { text: "38", value: 38 },
    { text: "39", value: 39 },
    { text: "40", value: 40 },
    { text: "41", value: 41 },
    { text: "42", value: 42 },
    { text: "43", value: 43 },
    { text: "44", value: 44 },
    { text: "45", value: 45 },
    { text: "46", value: 46 },
    { text: "47", value: 47 },
    { text: "48", value: 48 },
    { text: "49", value: 49 },
    { text: "50", value: 50 },
    { text: "51", value: 51 },
    { text: "52", value: 52 },
    { text: "53", value: 53 },
    { text: "54", value: 54 },
    { text: "55", value: 55 },
    { text: "56", value: 56 },
    { text: "57", value: 57 },
    { text: "58", value: 58 },
    { text: "59", value: 59 },
    { text: "60", value: 60 },
    { text: "61", value: 61 },
    { text: "62", value: 62 },
    { text: "63", value: 63 },
    { text: "64", value: 64 },
    { text: "65", value: 65 },
    { text: "66", value: 66 },
    { text: "67", value: 67 },
    { text: "68", value: 68 },
    { text: "69", value: 69 },
    { text: "70", value: 70 },
    { text: "71", value: 71 },
    { text: "72", value: 72 },
    { text: "73", value: 73 },
    { text: "74", value: 74 },
    { text: "75", value: 75 },
    { text: "76", value: 76 },
    { text: "77", value: 77 },
    { text: "78", value: 78 },
    { text: "79", value: 79 },
    { text: "80", value: 80 },
    { text: "81", value: 81 },
    { text: "82", value: 82 },
    { text: "83", value: 83 },
    { text: "84", value: 84 },
    { text: "85", value: 85 },
    { text: "86", value: 86 },
    { text: "87", value: 87 },
    { text: "88", value: 88 },
    { text: "89", value: 89 },
    { text: "90", value: 90 },
    { text: "91", value: 91 },
    { text: "92", value: 92 },
    { text: "93", value: 93 },
    { text: "94", value: 94 },
    { text: "95", value: 95 },
    { text: "96", value: 96 },
    { text: "97", value: 97 },
    { text: "98", value: 98 },
    { text: "99", value: 99 },
    { text: "100", value: 100 }
];

var PesquisaConfiguracaoAbastecimento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.TipoImportacao = PropertyEntity({ val: ko.observable(0), options: _pesquisaTipoImportacao, def: 0, text: "Tipo: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoAbastecimento.CarregarGrid();
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

var ConfiguracaoAbastecimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.TipoImportacao = PropertyEntity({ val: ko.observable(1), options: _tipoImportacao, def: 1, text: "*Tipo: ", eventChange: tipoImportacaoChange });

    this.LayoutEDI = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: ko.observable("*Layout EDI:"), idBtnSearch: guid(), visible: ko.observable(true) });
    this.PostoInterno = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: ko.observable("*Posto Interno:"), idBtnSearch: guid(), visible: ko.observable(false) });

    this.UtilizarPrecoDaTabelaDeValoresDoFornecedor = PropertyEntity({ text: "Utilizar o preço da tabela de valores do fornecedor (se houver)", getType: typesKnockout.bool, val: ko.observable(false) });
    this.GerarContasAPagarParaAbastecimentoExternos = PropertyEntity({ text: "Gerar contas a pagar para abastecimentos externos?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoImportarAbastecimentoDuplicado = PropertyEntity({ text: "Não importar abastecimentos duplicados?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.NaoGerarMovimentoFinanceiroFechamentoExterno = PropertyEntity({ text: "Não gerar movimento financeiro de fechamento de abastecimentos externos?", getType: typesKnockout.bool, val: ko.observable(false) });
    this.TipoMovimento = PropertyEntity({ text: "Tipo de Movimento para Pagamentos Externos:", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), visible: ko.observable(false) });

    this.Grid = PropertyEntity({ type: types.local });
    this.ListaColunas = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.TipoCampo = PropertyEntity({ val: ko.observable(0), options: _tipoCampo, def: 1, text: "*Tipo Campo: ", required: false });
    this.ColunaPlanilha = PropertyEntity({ val: ko.observable(0), options: _colunaPlanilha, def: 1, text: "*Coluna: ", required: false });
    this.Posicao = PropertyEntity({ val: ko.observable(1), options: _posicao, def: 1, text: "*Posição: ", required: false });

    this.AdicionarColuna = PropertyEntity({ eventClick: adicionarListaColunaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });


}

//*******EVENTOS*******

function loadConfiguracaoAbastecimento() {

    _pesquisaConfiguracaoAbastecimento = new PesquisaConfiguracaoAbastecimento();
    KoBindings(_pesquisaConfiguracaoAbastecimento, "knockoutPesquisaConfiguracaoAbastecimento", false, _pesquisaConfiguracaoAbastecimento.Pesquisar.id);

    _configuracaoAbastecimento = new ConfiguracaoAbastecimento();
    KoBindings(_configuracaoAbastecimento, "knockoutCadastroConfiguracaoAbastecimento");

    HeaderAuditoria("ConfiguracaoAbastecimento", _configuracaoAbastecimento);

    new BuscarLayoutsEDI(_configuracaoAbastecimento.LayoutEDI);
    new BuscarClientes(_configuracaoAbastecimento.PostoInterno);
    new BuscarTipoMovimento(_configuracaoAbastecimento.TipoMovimento);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: excluirListaColunaClick }] };

    var header = [{ data: "Codigo", visible: false },
    { data: "Posicao", title: "Posição", width: "10%" },
    { data: "DescricaoTipoCampo", title: "Tipo Campo", width: "35%" },
    { data: "TipoCampo", visible: false },
    { data: "DescricaoColunaPlanilha", title: "Coluna", width: "35%" },
    { data: "ColunaPlanilha", visible: false }];

    _gridListaColunas = new BasicDataTable(_configuracaoAbastecimento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    buscarConfiguracaoAbastecimento();
    recarregarGridListaColuna();
    $("#liTabImportacao").hide();

    _configuracaoAbastecimento.GerarContasAPagarParaAbastecimentoExternos.val.subscribe(function (novoValor) {
        _configuracaoAbastecimento.TipoMovimento.visible(novoValor === true);
        if (novoValor === false) {
            _configuracaoAbastecimento.TipoMovimento.val("");
            _configuracaoAbastecimento.TipoMovimento.codEntity(0);
        }
    });
}

function recarregarGridListaColuna() {

    var data = new Array();

    $.each(_configuracaoAbastecimento.ListaColunas.list, function (i, listaColuna) {
        var listaColunaGrid = new Object();

        listaColunaGrid.Codigo = listaColuna.Codigo.val;
        listaColunaGrid.DescricaoTipoCampo = listaColuna.DescricaoTipoCampo.val;
        listaColunaGrid.TipoCampo = listaColuna.TipoCampo.val;
        listaColunaGrid.DescricaoColunaPlanilha = listaColuna.DescricaoColunaPlanilha.val;
        listaColunaGrid.ColunaPlanilha = listaColuna.ColunaPlanilha.val;
        listaColunaGrid.Posicao = listaColuna.Posicao.val;

        data.push(listaColunaGrid);
    });

    _gridListaColunas.CarregarGrid(data);
}

function excluirListaColunaClick(data) {
    $.each(_configuracaoAbastecimento.ListaColunas.list, function (i, listaColunas) {
        if (data.Codigo == listaColunas.Codigo.val) {
            _configuracaoAbastecimento.ListaColunas.list.splice(i, 1);
            return false;
        }
    });

    recarregarGridListaColuna();
}

function adicionarListaColunaClick(e, sender) {
    var valido = true;

    if (valido) {
        var existe = false;
        $.each(_configuracaoAbastecimento.ListaColunas.list, function (i, listaColunas) {
            if (listaColunas.Posicao.val == _configuracaoAbastecimento.Posicao.val() || listaColunas.ColunaPlanilha.val == _configuracaoAbastecimento.ColunaPlanilha.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, "Coluna ou Posição já existe", "Já foi adicionado essa coluna ou essa posição na lista.");
            return;
        }
        var planilhaMap = new PlanilhaMap();

        planilhaMap.Codigo.val = _configuracaoAbastecimento.ListaColunas.list.length + 1;
        planilhaMap.DescricaoTipoCampo.val = _tipoCampo[_configuracaoAbastecimento.TipoCampo.val()].text;
        planilhaMap.TipoCampo.val = _configuracaoAbastecimento.TipoCampo.val();
        planilhaMap.DescricaoColunaPlanilha.val = _colunaPlanilha[_configuracaoAbastecimento.ColunaPlanilha.val()].text;
        planilhaMap.ColunaPlanilha.val = _configuracaoAbastecimento.ColunaPlanilha.val();
        planilhaMap.Posicao.val = _configuracaoAbastecimento.Posicao.val();

        _configuracaoAbastecimento.ListaColunas.list.push(planilhaMap);

        recarregarGridListaColuna();
        $("#" + _configuracaoAbastecimento.TipoCampo.id).focus();

        limparCamposListaColuna();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function limparCamposListaColuna() {
    _configuracaoAbastecimento.TipoCampo.val(0);
    _configuracaoAbastecimento.ColunaPlanilha.val(0);
    _configuracaoAbastecimento.Posicao.val(1);
}


function tipoImportacaoChange(e, sender) {
    if (_configuracaoAbastecimento.TipoImportacao.val() == 1) {//EDI
        _configuracaoAbastecimento.LayoutEDI.visible(true);
        _configuracaoAbastecimento.LayoutEDI.required = true;
        _configuracaoAbastecimento.LayoutEDI.text("*Layout EDI:");

        _configuracaoAbastecimento.PostoInterno.visible(false);
        _configuracaoAbastecimento.PostoInterno.required = false;
        _configuracaoAbastecimento.PostoInterno.text("Posto Interno:");
        LimparCampoEntity(_configuracaoAbastecimento.PostoInterno);

        $("#liTabImportacao").hide();
    } else if (_configuracaoAbastecimento.TipoImportacao.val() == 2) {//Posto Amigao
        _configuracaoAbastecimento.LayoutEDI.visible(false);
        _configuracaoAbastecimento.LayoutEDI.required = false;
        _configuracaoAbastecimento.LayoutEDI.text("Layout EDI:");
        LimparCampoEntity(_configuracaoAbastecimento.LayoutEDI);

        _configuracaoAbastecimento.PostoInterno.visible(false);
        _configuracaoAbastecimento.PostoInterno.required = false;
        _configuracaoAbastecimento.PostoInterno.text("Posto Interno:");
        LimparCampoEntity(_configuracaoAbastecimento.PostoInterno);

        $("#liTabImportacao").hide();
    } else if (_configuracaoAbastecimento.TipoImportacao.val() == 3) {//Planilha
        _configuracaoAbastecimento.LayoutEDI.visible(false);
        _configuracaoAbastecimento.LayoutEDI.required = false;
        _configuracaoAbastecimento.LayoutEDI.text("Layout EDI:");
        LimparCampoEntity(_configuracaoAbastecimento.LayoutEDI);

        _configuracaoAbastecimento.PostoInterno.visible(false);
        _configuracaoAbastecimento.PostoInterno.required = false;
        _configuracaoAbastecimento.PostoInterno.text("Posto Interno:");
        LimparCampoEntity(_configuracaoAbastecimento.PostoInterno);

        $("#liTabImportacao").show();
    } else if (_configuracaoAbastecimento.TipoImportacao.val() == 4) {//Interno
        _configuracaoAbastecimento.LayoutEDI.visible(false);
        _configuracaoAbastecimento.LayoutEDI.required = false;
        _configuracaoAbastecimento.LayoutEDI.text("Layout EDI:");
        LimparCampoEntity(_configuracaoAbastecimento.LayoutEDI);

        _configuracaoAbastecimento.PostoInterno.visible(true);
        _configuracaoAbastecimento.PostoInterno.required = true;
        _configuracaoAbastecimento.PostoInterno.text("*Posto Interno:");

        $("#liTabImportacao").hide();
    } else if (_configuracaoAbastecimento.TipoImportacao.val() == 5) {//Posto Reforço 4
        _configuracaoAbastecimento.LayoutEDI.visible(false);
        _configuracaoAbastecimento.LayoutEDI.required = false;
        _configuracaoAbastecimento.LayoutEDI.text("Layout EDI:");
        LimparCampoEntity(_configuracaoAbastecimento.LayoutEDI);

        _configuracaoAbastecimento.PostoInterno.visible(false);
        _configuracaoAbastecimento.PostoInterno.required = false;
        _configuracaoAbastecimento.PostoInterno.text("Posto Interno:");
        LimparCampoEntity(_configuracaoAbastecimento.PostoInterno);

        $("#liTabImportacao").hide();
    }
}

function adicionarClick(e, sender) {
    resetarTabs();
    Salvar(_configuracaoAbastecimento, "ConfiguracaoAbastecimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridConfiguracaoAbastecimento.CarregarGrid();
                limparCamposConfiguracaoAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function atualizarClick(e, sender) {
    resetarTabs();
    Salvar(_configuracaoAbastecimento, "ConfiguracaoAbastecimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoAbastecimento.CarregarGrid();
                limparCamposConfiguracaoAbastecimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração de abastecimento " + _configuracaoAbastecimento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_configuracaoAbastecimento, "ConfiguracaoAbastecimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConfiguracaoAbastecimento.CarregarGrid();
                    limparCamposConfiguracaoAbastecimento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick(e) {
    limparCamposConfiguracaoAbastecimento();
}

//*******MÉTODOS*******


function editarConfiguracaoAbastecimento(configuracaoAbastecimentoGrid) {
    limparCamposConfiguracaoAbastecimento();
    _configuracaoAbastecimento.Codigo.val(configuracaoAbastecimentoGrid.Codigo);
    BuscarPorCodigo(_configuracaoAbastecimento, "ConfiguracaoAbastecimento/BuscarPorCodigo", function (arg) {
        tipoImportacaoChange(_configuracaoAbastecimento);
        _pesquisaConfiguracaoAbastecimento.ExibirFiltros.visibleFade(false);
        _configuracaoAbastecimento.Atualizar.visible(true);
        _configuracaoAbastecimento.Cancelar.visible(true);
        _configuracaoAbastecimento.Excluir.visible(true);
        _configuracaoAbastecimento.Adicionar.visible(false);
        resetarTabs();
        recarregarGridListaColuna();

    }, null);
}


function buscarConfiguracaoAbastecimento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoAbastecimento, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoAbastecimento = new GridView(_pesquisaConfiguracaoAbastecimento.Pesquisar.idGrid, "ConfiguracaoAbastecimento/Pesquisa", _pesquisaConfiguracaoAbastecimento, menuOpcoes, null);
    _gridConfiguracaoAbastecimento.CarregarGrid();
}


function limparCamposConfiguracaoAbastecimento() {
    _configuracaoAbastecimento.Atualizar.visible(false);
    _configuracaoAbastecimento.Cancelar.visible(false);
    _configuracaoAbastecimento.Excluir.visible(false);
    _configuracaoAbastecimento.Adicionar.visible(true);
    LimparCampos(_configuracaoAbastecimento);
    tipoImportacaoChange(_configuracaoAbastecimento);
    resetarTabs();
    recarregarGridListaColuna();
}

function exibirCamposObrigatorio() {
    exibirMensagem("Atenção", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
}

function resetarTabs() {
    $("#myTab a:first").tab("show");
}