/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/GrupoProdutoTMS.js" />
/// <reference path="../../Consultas/Almoxarifado.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="../../Consultas/Empresa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="BemSaldo.js" />
/// <reference path="BemFinanciamento.js" />
/// <reference path="BemComponente.js" />
/// <reference path="BemAnexo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridBem;
var _bem;
var _pesquisaBem;
var _crudBem;

var PesquisaBem = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.NumeroSerie = PropertyEntity({ text: "Número de Série: " });
    this.Codigo = PropertyEntity({ text: "Código: ", getType: typesKnockout.int });
    this.FuncionarioAlocado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Alocado:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBem.CarregarGrid();
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

var Bem = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.NumeroSerie = PropertyEntity({ text: "*Número de Série: ", required: ko.observable(true), maxlength: 500 });
    this.DescricaoNota = PropertyEntity({ text: "*Descrição na Nota Fiscal: ", required: ko.observable(true), maxlength: 120, val: ko.observable("") });

    this.DataAquisicao = PropertyEntity({ text: "*Data Aquisição: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataFimGarantia = PropertyEntity({ text: "*Data Fim Garantia: ", getType: typesKnockout.date, required: ko.observable(true) });

    this.Observacao = PropertyEntity({ text: "Observação:", maxlength: 5000, val: ko.observable("") });

    this.GrupoProdutoTMS = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Grupo Produto:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Almoxarifado:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Centro de Resultado:", idBtnSearch: guid(), required: ko.observable(false), visible: ko.observable(true) });
    this.Fornecedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Fornecedor:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.DocumentoEntradaItem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Item do Documento de Entrada:", idBtnSearch: guid(), required: ko.observable(false), enable: ko.observable(false), visible: ko.observable(true) });
    this.Produto = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Empresa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Saldo = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Componentes = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosNovos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.ListaAnexosExcluidos = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });
};

var CRUDBem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Etiqueta = PropertyEntity({ eventClick: etiquetaClick, type: types.event, text: "Etiqueta", visible: ko.observable(false) });
    this.TermoResponsabilidade = PropertyEntity({ eventClick: termoResponsabilidadeBemClick, type: types.event, text: "Termo de Responsabilidade", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadBem() {
    _bem = new Bem();
    KoBindings(_bem, "knockoutCadastroBem");

    HeaderAuditoria("Bem", _bem);

    _crudBem = new CRUDBem();
    KoBindings(_crudBem, "knockoutCRUDBem");

    _pesquisaBem = new PesquisaBem();
    KoBindings(_pesquisaBem, "knockoutPesquisaBem", _pesquisaBem.Pesquisar.id);

    new BuscarGruposProdutosTMS(_bem.GrupoProdutoTMS, null);
    new BuscarAlmoxarifado(_bem.Almoxarifado);
    new BuscarEmpresa(_bem.Empresa);
    new BuscarClientes(_bem.Fornecedor);
    new BuscarCentroResultado(_bem.CentroResultado);
    new BuscarProdutoTMS(_bem.Produto);

    new BuscarFuncionario(_pesquisaBem.FuncionarioAlocado);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _bem.Empresa.visible(false);
        _bem.Empresa.required(false);
    }

    buscarBens();

    loadBemSaldo();
    loadBemFinanciamento();
    loadBemComponente();
    loadBemAnexo();
}

function codigoDescricaoExit() {
    if ($("#" + _bem.Descricao.id).val() != "" && _bem.DescricaoNota.val() == "") {
        _bem.DescricaoNota.val($("#" + _bem.Descricao.id).val().substring(0, 120));
    }
}

function adicionarClick(e, sender) {
    resetarTabs();
    var valido = true;

    if (!validaCamposObrigatoriosBemSaldo()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos obrigatórios do Saldo");
        return;
    }

    if (valido) {
        _bem.Saldo.val(JSON.stringify(RetornarObjetoPesquisa(_bemSaldo)));

        Salvar(_bem, "Bem/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_bem.ListaAnexosNovos.list.length > 0)
                        EnviarBemAnexos(arg.Data.Codigo);
                    else {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                        _gridBem.CarregarGrid();
                        limparCamposBens();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function atualizarClick(e, sender) {
    resetarTabs();
    var valido = true;

    if (!validaCamposObrigatoriosBemSaldo()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por Favor, Informe todos os campos obrigatórios do Saldo");
        return;
    }

    if (valido) {
        _bem.Saldo.val(JSON.stringify(RetornarObjetoPesquisa(_bemSaldo)));
        if (_bem.ListaAnexosExcluidos.list.length > 0)
            _bem.ListaAnexosExcluidos.text = JSON.stringify(_bem.ListaAnexosExcluidos.list);

        Salvar(_bem, "Bem/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    if (_bem.ListaAnexosNovos.list.length > 0)
                        EnviarBemAnexos(_bem.Codigo.val());
                    else {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                        _gridBem.CarregarGrid();
                        limparCamposBens();
                    }
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    }
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Patrimônio " + _bem.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_bem, "Bem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridBem.CarregarGrid();
                limparCamposBens();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposBens();
}

function etiquetaClick(e, sender) {
    exibirMensagem(tipoMensagem.aviso, "Aviso", "Etiqueta ainda está em desenvolvimento");
}

function termoResponsabilidadeBemClick(e, sender) {
    var data = { CodigoBem: _bem.Codigo.val() };
    executarReST("RelatoriosBem/BaixarRelatorioTermoResponsabilidade", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Aguarde que seu relatório está sendo gerado.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******MÉTODOS*******

function EnviarBemAnexos(codigoBem) {
    var file;
    var fileCount = _bem.ListaAnexosNovos.list.length;
    var documentos = new Array();

    for (var i = 0; i < _bem.ListaAnexosNovos.list.length; i++) {
        file = _bem.ListaAnexosNovos.list[i].Arquivo;
        var formData = new FormData();
        formData.append("upload", file);
        var data = {
            ListaAnexos: "",
            DescricaoAnexo: _bem.ListaAnexosNovos.list[i].DescricaoAnexo.val,
            CodigoBem: codigoBem
        };
        enviarArquivo("Bem/EnviarAnexos?callback=?", data, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    documentos.push({ Codigo: codigoBem });
                    if (documentos.length == fileCount) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo com sucesso");
                        _gridBem.CarregarGrid();
                        limparCamposBens();
                    }
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    }
}

function buscarBens() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarBem, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridBem = new GridView(_pesquisaBem.Pesquisar.idGrid, "Bem/Pesquisa", _pesquisaBem, menuOpcoes, null);
    _gridBem.CarregarGrid();
}

function editarBem(produtoGrid) {
    limparCamposBens();
    _bem.Codigo.val(produtoGrid.Codigo);
    BuscarPorCodigo(_bem, "Bem/BuscarPorCodigo", function (arg) {
        _pesquisaBem.ExibirFiltros.visibleFade(false);
        _crudBem.Atualizar.visible(true);
        _crudBem.Cancelar.visible(true);
        _crudBem.Excluir.visible(true);
        _crudBem.Adicionar.visible(false);
        _crudBem.Etiqueta.visible(true);
        _crudBem.TermoResponsabilidade.visible(true);
        _bem.Empresa.enable(false);

        buscarBemFinanciamento();
        $("#liTabFinanciamento").show();

        RecarregarGridBemComponente();
        RecarregarGridBemAnexo();

        if (arg.Data.Saldo !== null && arg.Data.Saldo !== undefined) {
            PreencherObjetoKnout(_bemSaldo, { Data: arg.Data.Saldo });
        }
    }, null);
}

function limparCamposBens() {
    _crudBem.Atualizar.visible(false);
    _crudBem.Cancelar.visible(false);
    _crudBem.Excluir.visible(false);
    _crudBem.Adicionar.visible(true);
    _crudBem.Etiqueta.visible(false);
    _crudBem.TermoResponsabilidade.visible(false);
    _bem.Empresa.enable(true);
    LimparCampos(_bem);

    limparCamposBemSaldo();
    LimparCamposBemComponente();
    LimparCamposBemAnexo();

    RecarregarGridBemAnexo();

    $("#liTabFinanciamento").hide();
    $("#liTabBem a").click();

    resetarTabs();
}

function resetarTabs() {
    $(".nav-tabs").each(function () {
        $(this).find("a:first").tab("show");
    });
}