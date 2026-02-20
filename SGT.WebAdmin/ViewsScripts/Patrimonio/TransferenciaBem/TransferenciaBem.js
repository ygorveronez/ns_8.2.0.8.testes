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
/// <reference path="../../Consultas/Bem.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Almoxarifado.js" />
/// <reference path="../../Consultas/CentroResultado.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransferenciaBem;
var _transferenciaBem;
var _pesquisaTransferenciaBem;
var _gridBensTransferencia;

var PesquisaTransferenciaBem = function () {
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Responsável:", idBtnSearch: guid() });
    this.DataEnvio = PropertyEntity({ text: "Data Envio: ", getType: typesKnockout.date });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTransferenciaBem.CarregarGrid();
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

var TransferenciaBem = function () {
    this.Codigo = PropertyEntity({ text: "Código: ", val: ko.observable(0), def: 0, getType: typesKnockout.int, enable: ko.observable(false) });

    this.DataEnvio = PropertyEntity({ text: "*Data Envio: ", getType: typesKnockout.date, required: ko.observable(true) });
    this.DataRecebimento = PropertyEntity({ text: "Data Recebimento: ", getType: typesKnockout.date });
    this.ObservacaoSaida = PropertyEntity({ text: "Observação Saída:", maxlength: 5000, val: ko.observable("") });
    this.ObservacaoEnvio = PropertyEntity({ text: "Observação Envio:", maxlength: 5000, val: ko.observable("") });

    this.Almoxarifado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Almoxarifado:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.CentroResultado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Centro de Resultado:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário Responsável:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.FuncionarioEnvio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Funcionário Envio:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Pessoa:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true) });

    this.GridBem = PropertyEntity({ type: types.local });
    this.Bem = PropertyEntity({ type: types.event, text: "Adicionar Patrimônio(s)", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Bens = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDTransferenciaBem = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });

    this.TermoResponsabilidade = PropertyEntity({ eventClick: termoResponsabilidadeTransferenciaClick, type: types.event, text: ko.observable("Termo de Responsabilidade"), visible: ko.observable(false), icon: "fa fa-fw fa-eye" });
    this.TermoRecolhimentoMaterial = PropertyEntity({ eventClick: termoRecolhimentoMaterialTransferenciaClick, type: types.event, text: ko.observable("Termo de Recolhimento de Material"), visible: ko.observable(false), icon: "fa fa-fw fa-cube" });
};

//*******EVENTOS*******


function loadTransferenciaBem() {
    _transferenciaBem = new TransferenciaBem();
    KoBindings(_transferenciaBem, "knockoutCadastroTransferenciaBem");

    HeaderAuditoria("TransferenciaBem", _transferenciaBem);

    _crudTransferenciaBem = new CRUDTransferenciaBem();
    KoBindings(_crudTransferenciaBem, "knockoutCRUDTransferenciaBem");

    _pesquisaTransferenciaBem = new PesquisaTransferenciaBem();
    KoBindings(_pesquisaTransferenciaBem, "knockoutPesquisaTransferenciaBem", false, _pesquisaTransferenciaBem.Pesquisar.id);

    new BuscarFuncionario(_pesquisaTransferenciaBem.Funcionario);
    new BuscarFuncionario(_transferenciaBem.Funcionario);
    new BuscarFuncionario(_transferenciaBem.FuncionarioEnvio);
    new BuscarAlmoxarifado(_transferenciaBem.Almoxarifado);
    new BuscarCentroResultado(_transferenciaBem.CentroResultado);
    new BuscarClientes(_transferenciaBem.Pessoa);

    let menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: function (data) { ExcluirBemClick(_transferenciaBem.Bem, data); } }] };
    let header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: "Descrição", width: "45%" },
        { data: "NumeroSerie", title: "Número de Série", width: "20%" },
        { data: "DataFimGarantia", title: "Data Fim Garantia", width: "10%" },
        { data: "ValorBem", title: "Valor Patrimônio", width: "15%" }
    ];
    _gridBensTransferencia = new BasicDataTable(_transferenciaBem.GridBem.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarBens(_transferenciaBem.Bem, function (r) {
        if (r != null) {
            let bens = _gridBensTransferencia.BuscarRegistros();
            for (var i = 0; i < r.length; i++)
                bens.push({
                    Codigo: r[i].Codigo,
                    Descricao: r[i].Descricao,
                    NumeroSerie: r[i].NumeroSerie,
                    DataFimGarantia: r[i].DataFimGarantia,
                    ValorBem: r[i].ValorBem
                });

            _gridBensTransferencia.CarregarGrid(bens);
        }
    }, _gridBensTransferencia);

    _transferenciaBem.Bem.basicTable = _gridBensTransferencia;

    buscarTransferenciaBens();
    RecarregarGridBem();
}

function adicionarClick(e, sender) {
    preencherListasSelecaoTransferenciaBem();

    Salvar(_transferenciaBem, "TransferenciaBem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTransferenciaBem.CarregarGrid();
                limparCamposTransferenciaBem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecaoTransferenciaBem();

    Salvar(_transferenciaBem, "TransferenciaBem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTransferenciaBem.CarregarGrid();
                limparCamposTransferenciaBem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Transferência do Patrimônio?", function () {
        ExcluirPorCodigo(_transferenciaBem, "TransferenciaBem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTransferenciaBem.CarregarGrid();
                limparCamposTransferenciaBem();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTransferenciaBem();
}

function termoResponsabilidadeTransferenciaClick(e, sender) {
    let data = { CodigoTransferencia: _transferenciaBem.Codigo.val() };
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

function termoRecolhimentoMaterialTransferenciaClick(e, sender) {
    let data = { CodigoTransferencia: _transferenciaBem.Codigo.val() };
    executarReST("RelatoriosBem/BaixarRelatorioTermoRecolhimentoMaterial", data, function (arg) {
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

function RecarregarGridBem() {

    let data = new Array();
    if (!string.IsNullOrWhiteSpace(_transferenciaBem.Bens.val())) {
        $.each(_transferenciaBem.Bens.val(), function (i, bem) {
            let bemGrid = new Object();

            bemGrid.Codigo = bem.BEM.Codigo;
            bemGrid.Descricao = bem.BEM.Descricao;
            bemGrid.NumeroSerie = bem.BEM.NumeroSerie;
            bemGrid.DataFimGarantia = bem.BEM.DataFimGarantia;
            bemGrid.ValorBem = bem.BEM.ValorBem;

            data.push(bemGrid);
        });
    }

    _gridBensTransferencia.CarregarGrid(data);
}

function ExcluirBemClick(knoutBem, data) {
    let bemGrid = knoutBem.basicTable.BuscarRegistros();

    for (let i = 0; i < bemGrid.length; i++) {
        if (data.Codigo == bemGrid[i].Codigo) {
            bemGrid.splice(i, 1);
            break;
        }
    }

    knoutBem.basicTable.CarregarGrid(bemGrid);
}

function preencherListasSelecaoTransferenciaBem() {
    let bens = new Array();

    $.each(_transferenciaBem.Bem.basicTable.BuscarRegistros(), function (i, bem) {
        bens.push({ BEM: bem });
    });

    _transferenciaBem.Bens.val(JSON.stringify(bens));
}

function buscarTransferenciaBens() {
    let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarTransferenciaBem, tamanho: "15", icone: "" };
    let menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTransferenciaBem = new GridView(_pesquisaTransferenciaBem.Pesquisar.idGrid, "TransferenciaBem/Pesquisa", _pesquisaTransferenciaBem, menuOpcoes, null);
    _gridTransferenciaBem.CarregarGrid();
}

function editarTransferenciaBem(transferenciaBemGrid) {
    limparCamposTransferenciaBem();
    _transferenciaBem.Codigo.val(transferenciaBemGrid.Codigo);
    BuscarPorCodigo(_transferenciaBem, "TransferenciaBem/BuscarPorCodigo", function (arg) {
        _pesquisaTransferenciaBem.ExibirFiltros.visibleFade(false);
        _crudTransferenciaBem.Atualizar.visible(true);
        _crudTransferenciaBem.Cancelar.visible(true);
        _crudTransferenciaBem.Excluir.visible(true);
        _crudTransferenciaBem.Adicionar.visible(false);
        _crudTransferenciaBem.TermoResponsabilidade.visible(true);
        _crudTransferenciaBem.TermoRecolhimentoMaterial.visible(true);

        RecarregarGridBem();
    }, null);
}

function limparCamposTransferenciaBem() {
    _crudTransferenciaBem.Atualizar.visible(false);
    _crudTransferenciaBem.Cancelar.visible(false);
    _crudTransferenciaBem.Excluir.visible(false);
    _crudTransferenciaBem.Adicionar.visible(true);
    _crudTransferenciaBem.TermoResponsabilidade.visible(false);
    _crudTransferenciaBem.TermoRecolhimentoMaterial.visible(false);
    LimparCampos(_transferenciaBem);
    RecarregarGridBem();
}