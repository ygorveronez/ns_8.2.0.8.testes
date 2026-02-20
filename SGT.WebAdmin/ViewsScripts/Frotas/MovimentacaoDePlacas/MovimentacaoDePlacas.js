/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="../../Consultas/Pais.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/CentroResultado.js" />
/// <reference path="Reboques.js" />
/// <reference path="Motoristas.js" />
/// <reference path="InformarKmAtualVeiculoTracao.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaPlacas;
var _pesquisaGestor;
var _HTMLMovimentacaoDePlacas;
var _HTMLMovimentacaoDePlacasReboque;
var _HTMLMovimentacaoDePlacasMotorista;
var _gridVeiculoCodigo;
var _gridReboqueCodigo;
var _gridMotoristaCodigo;
var _gridEquipamentos;
var _codigoVeiculoSelecionado;
var _codigoGestorSelecionado;
var _opcoes;

const opcoesImpressao = [
    { text: 'Imprimir', value: 0},
    { text: 'PDF', value: 1 },
    { text: 'Excel', value: 2 },
]

var Veiculo = function () {
    this.DivVeiculo = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.DescricaoTipoVeiculo = PropertyEntity({ type: types.map, required: false, text: "Tipo: " });
    this.DescricaoRenavan = PropertyEntity({ type: types.map, required: false, text: "RENAVAN: " });
    this.DescricaoModelo = PropertyEntity({ type: types.map, required: false, text: "Modelo: " });
    this.DescricaoMarca = PropertyEntity({ type: types.map, required: false, text: "Marca: " });
    this.NumeroFrota = PropertyEntity({ type: types.map, required: false, text: "Nº Frota: " });
    this.Placa = PropertyEntity({ type: types.map, required: false, text: "Placa: " });
    this.DescricaoSegmentoVeiculo = PropertyEntity({ type: types.map, required: false, text: "Segmento: " });
    this.Equipamentos = PropertyEntity({ type: types.map, required: false, text: "Equipamento(s): " });
    this.CentroResultado = PropertyEntity({ type: types.map, required: false, text: "Centro de Resultado: " });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.map, required: false, text: "Funcionário Responsável: " });
}

var Reboque = function () {
    this.DivReboque = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.DescricaoTipoVeiculo = PropertyEntity({ type: types.map, required: false, text: "Tipo: " });
    this.DescricaoRenavan = PropertyEntity({ type: types.map, required: false, text: "RENAVAN: " });
    this.DescricaoModelo = PropertyEntity({ type: types.map, required: false, text: "Modelo: " });
    this.DescricaoMarca = PropertyEntity({ type: types.map, required: false, text: "Marca: " });
    this.NumeroFrota = PropertyEntity({ type: types.map, required: false, text: "Nº Frota: " });
    this.Placa = PropertyEntity({ type: types.map, required: false, text: "Placa: " });
    this.DescricaoSegmentoVeiculo = PropertyEntity({ type: types.map, required: false, text: "Segmento: " });
    this.Equipamentos = PropertyEntity({ type: types.map, required: false, text: "Equipamento(s): " });
    this.CentroResultado = PropertyEntity({ type: types.map, required: false, text: "Centro de Resultado: " });
    this.QuantidadePaletes = PropertyEntity({ type: types.map, required: false, text: "Quantidade de Paletes: " });
    this.FuncionarioResponsavel = PropertyEntity({ type: types.map, required: false, text: "Funcionario Responsável: " })
}

var Motorista = function () {
    this.DivMotorista = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.Nome = PropertyEntity({ type: types.map, required: false, text: "Nome: " });
    this.CPF = PropertyEntity({ type: types.map, required: false, text: "CPF: " });
    this.DataVencimentoHabilitacao = PropertyEntity({ type: types.map, required: false, text: "Venc. CNH: " });
    this.Telefone = PropertyEntity({ type: types.map, required: false, text: "Telefone: " });
    this.Celular = PropertyEntity({ type: types.map, required: false, text: "Celular: " });
    this.NumeroHabilitacao = PropertyEntity({ type: types.map, required: false, text: "Nº CNH: ", visible: ko.observable(false) });
    this.DescricaoCidadeEstado = PropertyEntity({ type: types.map, required: false, text: "Localidade: " });
    this.CentroResultado = PropertyEntity({ type: types.map, required: false, text: "Centro de Resultado: " });
    this.DescricaoPrincipal = PropertyEntity({ type: types.map, required: false, text: "Principal: " });
    this.Gestor = PropertyEntity({ type: types.map, required: false, text: "Gestor: " });
}

var PesquisaGestaoVeiculo = function () {
    this.PlacaVeiculo = PropertyEntity({ text: "Placa:", visible: ko.observable(false), val: ko.observable("") });
    this.Placa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Placa:", idBtnSearch: guid(), issue: 143 });
    this.NumeroFrota = PropertyEntity({ text: "Nº Frota:", visible: ko.observable(true), val: ko.observable(""), maxlength: 30 });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid(), issue: 145 });
    this.KMAtual = PropertyEntity({ text: "KM Atual:", visible: ko.observable(false), val: ko.observable(""), maxlength: 30, getType: typesKnockout.int });
    this.SegmentoVeiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Segmento:", idBtnSearch: guid(), visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarVeiculosAlterar(1, false);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.FuncionarioResponsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário Responsável:", idBtnSearch: guid() });
}
var Opcoes = function () {

    this.AlterarGestor = PropertyEntity({ type: types.entity, text: "Alterar Gestor", codEntity: ko.observable(0), visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });

    this.AlterarCentroResultado = PropertyEntity({ type: types.event, text: "Alterar Centro de Resultado do Conjunto", visible: ko.observable(true), enable: ko.observable(true), idBtnSearch: guid() });
    this.GerarPDF = PropertyEntity({ type: types.event, eventClick: function () { ImprimirMovimentacaoDePlacas(1); }, text: "Gerar PDF", icon: "fal fa-file-pdf", visible: ko.observable(true) });
    this.GerarExcel = PropertyEntity({ type: types.event, eventClick: function () { ImprimirMovimentacaoDePlacas(2); }, text: "Gerar Excel", icon: "fal fa-file-excel", visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
    this.Equipamento = PropertyEntity({ type: types.event, text: "Equipamento", idBtnSearch: guid(), visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadMovimentacaoDePlacas() {
    loadInformarKmAtualVeiculoTracao();

    $("#divVeiculoTracao").hide();
    $("#divReboquesVeiculo").hide();
    $("#divMotoristasVeiculo").hide();
    $("#divResultadoPesquisa").hide();
    $("#divResultadoPesquisaReboque").hide();
    $("#divOpcoes").hide();

    iniciarRequisicao();
    $.get("Content/Static/Frota/MovimentacaoDePlacas.html?dyn=" + guid(), function (data) {
        _HTMLMovimentacaoDePlacas = data;
        $.get("Content/Static/Frota/MovimentacaoDePlacasReboque.html?dyn=" + guid(), function (data) {
            _HTMLMovimentacaoDePlacasReboque = data;
            $.get("Content/Static/Frota/MovimentacaoDePlacasMotorista.html?dyn=" + guid(), function (data) {
                _HTMLMovimentacaoDePlacasMotorista = data;
                loadVeiculos(callbackProgress);
            });
        });
    });
}

function callbackProgress() {

    _pesquisaPlacas = new PesquisaGestaoVeiculo();
    KoBindings(_pesquisaPlacas, "knockoutPesquisa", false, _pesquisaPlacas.Pesquisar.id);
    $("#" + _pesquisaPlacas.Placa.id).mask("AAAAAAA", { selectOnFocus: true, clearIfNotMatch: true });

    new BuscarMotoristas(_pesquisaPlacas.Motorista);
    new BuscarVeiculos(_pesquisaPlacas.Placa, RetornoVeiculo);
    new BuscarSegmentoVeiculo(_pesquisaPlacas.SegmentoVeiculo);
    _codigoVeiculoSelecionado = 0;
    _codigoGestorSelecionado = 0;

    buscarVeiculosAlterar();
    configurarDroppable();
}

function RetornoVeiculo(data) {
    _pesquisaPlacas.NumeroFrota.val(data.NumeroFrota);
    _pesquisaPlacas.Placa.val(data.Placa);
    _pesquisaPlacas.Placa.codEntity(data.Codigo);
    _pesquisaPlacas.KMAtual.val(data.KMAtual);
    //_pesquisaPlacas.SegmentoVeiculo.val(data.SegmentoVeiculo);
    //_pesquisaPlacas.SegmentoVeiculo.val(data.CodigoSegmentoVeiculo);
    _pesquisaPlacas.SegmentoVeiculo.visible(false);
    LimparCampoEntity(_pesquisaPlacas.Motorista);
}

//*******MÉTODOS*******

function configurarDroppable() {
    $("#divGridMotorista").droppable({
        drop: droppableItemRetirar,
        hoverClass: "ui-state-active backgroundDropHover",
        activate: activateItem,
        deactivate: deactivateItem
    });

    $("#divGridReboque").droppable({
        drop: droppableItemRetirar,
        hoverClass: "ui-state-active backgroundDropHover",
        activate: activateItem,
        deactivate: deactivateItem
    });

    $("#divReboquesVeiculo").droppable({
        drop: droppableItem,
        hoverClass: "ui-state-active backgroundDropHover",
        activate: activateItem,
        deactivate: deactivateItem
    });

    $("#divMotoristasVeiculo").droppable({
        drop: droppableItem,
        hoverClass: "ui-state-active backgroundDropHover",
        activate: activateItem,
        deactivate: deactivateItem
    });
}

function buscarVeiculosAlterar() {

    LimparCampoEntity(_pesquisaPlacas.SegmentoVeiculo);
    _pesquisaPlacas.PlacaVeiculo.val(_pesquisaPlacas.Placa.val());
    var data = RetornarObjetoPesquisa(_pesquisaPlacas);
    executarReST("MovimentacaoDePlacas/PesquisaVeiculos", data, function (e) {
        if (e.Success) {
            $("#fdsVeiculos").html("");
            $("#fdsReboques").html("");
            $("#fdsMotoristas").html("");
            _gridVeiculoCodigo = new Array();
            _gridReboqueCodigo = new Array();
            _gridMotoristaCodigo = new Array();
            _codigoVeiculoSelecionado = 0;

            if (e.Data != null && e.Data[0].TipoVeiculo == "Tração") {

                _habilitarAlterarGestor = e.Data[0].HabilitarAlterarGestor;
                $.each(e.Data, function (i, veiculoSelecionado) {
                    $("#divResultadoPesquisa").hide();
                    $("#divResultadoPesquisaReboque").hide();

                    //_pesquisaPlacas.AlterarGestor.visible(_habilitarAlterarGestor);
                    _pesquisaPlacas.Placa.val(veiculoSelecionado.Placa);

                    //var knoutGestor = new AlterarGestor();

                    //preecherObjetoGestor(knoutGestor, veiculoSelecionado);

                    if (_pesquisaPlacas.PlacaVeiculo.val().replace(/\_/g, "") != "") {
                        if (veiculoSelecionado.Placa != _pesquisaPlacas.PlacaVeiculo.val().replace(/\_/g, "").toUpperCase())
                            exibirMensagem("aviso", "Veículo selecionado", "Veículo de Tração selecionada pelo Reboque consultado.");
                        _pesquisaPlacas.PlacaVeiculo.val(veiculoSelecionado.Placa);
                    }

                    if (veiculoSelecionado.Motorista != null) {
                        _pesquisaPlacas.Motorista.codEntity(veiculoSelecionado.Motorista.Codigo);
                        _pesquisaPlacas.Motorista.val(veiculoSelecionado.Motorista.Nome);
                    }
                    else
                        LimparCampoEntity(_pesquisaPlacas.Motorista);

                    if (veiculoSelecionado.NumeroFrota > 0)
                        _pesquisaPlacas.NumeroFrota.val(veiculoSelecionado.NumeroFrota);

                    $("#divVeiculoTracao").show();
                    $("#divReboquesVeiculo").show();
                    $("#divMotoristasVeiculo").show();
                    $("#divOpcoes").removeClass("d-none").addClass("d-flex");

                    _codigoVeiculoSelecionado = veiculoSelecionado.Codigo;

                    var knoutVeiculo = new Veiculo();
                    preecherObjetoKnoutVeiculo(knoutVeiculo, veiculoSelecionado);
                    knoutVeiculo.DivVeiculo.id = knoutVeiculo.Codigo.idGrid;
                    var html = "<div id='conteudo_" + knoutVeiculo.DivVeiculo.id + "'></div>";
                    $("#fdsVeiculos").append(html);
                    preencherDadosVeiculo(knoutVeiculo, veiculoSelecionado);
                    _gridVeiculoCodigo.push({ Codigo: veiculoSelecionado.Codigo, idGrid: knoutVeiculo.DivVeiculo.id });

                    if (veiculoSelecionado.Reboques != null) {
                        $.each(veiculoSelecionado.Reboques, function (i, reboque) {

                            var knoutReboque = new Reboque();
                            preecherObjetoKnoutReboque(knoutReboque, reboque);
                            knoutReboque.DivReboque.id = knoutReboque.Codigo.idGrid;
                            var html = "<div id='conteudo_" + knoutReboque.DivReboque.id + "'> </div>";
                            $("#fdsReboques").append(html);
                            preencherDadosReboque(knoutReboque, reboque);
                            _gridReboqueCodigo.push({ Codigo: reboque.Codigo, idGrid: knoutReboque.DivReboque.id });
                        });
                    }

                    if (veiculoSelecionado.Motoristas != null && veiculoSelecionado.Motoristas.length > 0) {
                        $.each(veiculoSelecionado.Motoristas, function (i, motora) {

                            var knoutMotorista = new Motorista();
                            preecherObjetoKnoutMotorista(knoutMotorista, motora);
                            knoutMotorista.DivMotorista.id = knoutMotorista.Codigo.idGrid;
                            var html = "<div id='conteudo_" + knoutMotorista.DivMotorista.id + "'></div>";
                            $("#fdsMotoristas").append(html);
                            preencherDadosMotorista(knoutMotorista, motora);
                            _gridMotoristaCodigo.push({ Codigo: motora.Codigo, idGrid: knoutMotorista.DivMotorista.id, Principal: motora.Principal });
                        });

                    } else if (veiculoSelecionado.Motorista != null) {

                        var knoutMotorista = new Motorista();
                        preecherObjetoKnoutMotorista(knoutMotorista, veiculoSelecionado.Motorista);
                        knoutMotorista.DivMotorista.id = knoutMotorista.Codigo.idGrid;
                        var html = "<div id='conteudo_" + knoutMotorista.DivMotorista.id + "'></div>";
                        $("#fdsMotoristas").append(html);
                        preencherDadosMotorista(knoutMotorista, veiculoSelecionado.Motorista);
                        _gridMotoristaCodigo.push({ Codigo: veiculoSelecionado.Motorista.Codigo, idGrid: knoutMotorista.DivMotorista.id, Principal: motora.Principal });
                    }

                    //preecherObjetoGestor(_pesquisaPlacas, veiculoSelecionado);

                });
            } else if (e.Data != null && e.Data[0].TipoVeiculo == "Reboque") {
                exibirMensagem("aviso", "Reboque sem Vínculo", "Este veículo de reboque não possui nenhum vínculo. Por favor verifique");
                $("#divVeiculoTracao").hide();
                $("#divReboquesVeiculo").hide();
                $("#divMotoristasVeiculo").hide();
                $("#divOpcoes").removeClass("d-flex").addClass("d-none");
                $("#divResultadoPesquisa").hide();
                $("#divResultadoPesquisaReboque").show();
                $("#h4ReboqueSemVinculo").text("O reboque " + _pesquisaPlacas.PlacaVeiculo.val().toUpperCase() + " não está vinculado a uma tração.");
            } else {
                $("#divVeiculoTracao").hide();
                $("#divReboquesVeiculo").hide();
                $("#divMotoristasVeiculo").hide();
                $("#divOpcoes").removeClass("d-flex").addClass("d-none");

                if (_pesquisaPlacas.PlacaVeiculo.val().replace(/\_/g, "") != "" || _pesquisaPlacas.Placa.val().replace(/\_/g, "") != "" || _pesquisaPlacas.NumeroFrota.val() != "" || _pesquisaPlacas.Motorista.val() != "") {
                    exibirMensagem("aviso", "Veículo não localizado", "A consulta realizada não retornou nenhum veículo de tração. Por favor verifique.");
                    $("#divResultadoPesquisaReboque").hide();
                    $("#divResultadoPesquisa").show();
                }
            }
        }
        else {
            //_idVeiculoSelecionado = 0;
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }

        finalizarRequisicao();
    });
}

function preencherDadosVeiculo(knoutVeiculo, veiculo) {
    var idKnout = knoutVeiculo.DivVeiculo.id + "_" + knoutVeiculo.Codigo.val();
    var conteudo = _HTMLMovimentacaoDePlacas.replace(/#idDivVeiculo/g, idKnout);
    conteudo = conteudo.replace("(-1)", "(" + knoutVeiculo.Codigo.val() + ")");

    $("#conteudo_" + knoutVeiculo.DivVeiculo.id).html(conteudo);

    KoBindings(knoutVeiculo, idKnout);
}

function preencherDadosReboque(knoutReboque, reboque) {
    var idKnout = knoutReboque.DivReboque.id + "_" + knoutReboque.Codigo.val() + "_" + "reboque";
    var conteudo = _HTMLMovimentacaoDePlacasReboque.replace(/#idDivReboque/g, idKnout).replace(/#ribbonAlteracao/g, idKnout + "_ribbonAlteracao");
    conteudo = conteudo.replace("(-1)", "(" + knoutReboque.Codigo.val() + ")");
    $("#conteudo_" + knoutReboque.DivReboque.id).html(conteudo);

    KoBindings(knoutReboque, idKnout);
    $("#" + idKnout).draggable({
        helper: "clone",
        revert: true,
        cursor: "move"
    });
}

function preencherDadosMotorista(knoutMotorista, motorista) {
    var idKnout = knoutMotorista.DivMotorista.id + "_" + knoutMotorista.Codigo.val() + "_" + "motorista";
    var conteudo = _HTMLMovimentacaoDePlacasMotorista.replace(/#idDivMotorista/g, idKnout).replace(/#ribbonAlteracao/g, idKnout + "_ribbonAlteracao");
    conteudo = conteudo.replace("(-1)", "(" + knoutMotorista.Codigo.val() + ")");
    $("#conteudo_" + knoutMotorista.DivMotorista.id).html(conteudo);

    KoBindings(knoutMotorista, idKnout);
    $("#" + idKnout).draggable({
        helper: "clone",
        revert: true,
        cursor: "move"
    });
}

var z = 5000;
function activateItem(event, ui) {
    configurarDroppable();

    $("#divReboquesVeiculo").css('z-index', 1);
    $("#divMotoristasVeiculo").css('z-index', 2);
    $("#divGridReboque").css('z-index', 3);
    $("#divGridMotorista").css('z-index', 4);

    var tamanho = $("#" + ui.draggable[0].id).width();
    $(ui.helper[0]).width(tamanho);

    if (ui.draggable[0].id.split("_")[1] == "reboque") {
        $("#divReboquesVeiculo").css('z-index', 1001);
        $("#divGridReboque").css('z-index', 1002);
    } else if (ui.draggable[0].id.split("_")[1] == "motorista") {
        $("#divMotoristasVeiculo").css('z-index', 1001);
        $("#divGridMotorista").css('z-index', 1002);
    } else if (ui.draggable[0].id.split("_")[2] == "reboque") {
        $("#divGridReboque").css('z-index', 1001);
        $("#divReboquesVeiculo").css('z-index', 1002);
    } else if (ui.draggable[0].id.split("_")[2] == "motorista") {
        $("#divGridMotorista").css('z-index', 1001);
        $("#divMotoristasVeiculo").css('z-index', 1002);
    }

    $.blockUI({ message: "" });
}

function deactivateItem(event, ui) {
    $.unblockUI();
    if (_RequisicaoIniciada)
        iniciarRequisicao();

    $("#divReboquesVeiculo").css('z-index', 1);
    $("#divMotoristasVeiculo").css('z-index', 2);
    $("#divGridReboque").css('z-index', 3);
    $("#divGridMotorista").css('z-index', 4);

    configurarDroppable();
}

function droppableItemRetirar(event, ui) {
    var id = ui.draggable[0].id.split("_")[1];
    var operacao = ui.draggable[0].id.split("_")[2];
    var destino = event.target.id;

    if (_gridVeiculoCodigo != null && _gridVeiculoCodigo[0] != null && _gridVeiculoCodigo[0].Codigo > 0) {
        var veiculo = _gridVeiculoCodigo[0].Codigo;
        if (operacao == "reboque" && destino == "divGridReboque")
            removerReboque(veiculo, id);
        else if (operacao == "motorista" && destino == "divGridMotorista")
            removerMotorista(veiculo, id);
    }
}

function droppableItem(event, ui) {
    var id = ui.draggable[0].id.split("_")[0];
    var operacao = ui.draggable[0].id.split("_")[1];
    var destino = event.target.id;

    if (_gridVeiculoCodigo != null && _gridVeiculoCodigo[0] != null && _gridVeiculoCodigo[0].Codigo > 0) {
        var veiculo = _gridVeiculoCodigo[0].Codigo;
        if (operacao == "reboque" && destino == "divReboquesVeiculo")
            informarReboque(veiculo, id);
        else if (operacao == "motorista" && destino == "divMotoristasVeiculo")
            informarMotorista(veiculo, id);
    }
}

function informarMotorista(veiculo, motorista) {

    var contemVinculo = false;
    var placaVinculo = "";
    var nomeMotorista = "";
    var tipoVeiculoVinculo = "";

    $.each(_gridMotorista.GridViewTable().rows().data(), function (i, motoristaGrid) {
        if (motoristaGrid.Codigo.split("_")[0] == motorista) {
            contemVinculo = motoristaGrid.ContemVinculo;
            placaVinculo = motoristaGrid.PlacaVinculo;
            nomeMotorista = motoristaGrid.Nome;
            tipoVeiculoVinculo = motoristaGrid.TipoVinculo;
            return false;
        }
    });

    var contemMotorista = false;
    $.each(_gridMotoristaCodigo, function (i, gridMotoristaCodigo) {
        if (gridMotoristaCodigo.Codigo == motorista) {
            contemMotorista = true;
            return false;
        }
    });

    if (!contemMotorista) {
        var contMotorista = 0;
        if (_gridMotoristaCodigo != null && _gridMotoristaCodigo.length != null)
            contMotorista = _gridMotoristaCodigo.length;

        if (contMotorista >= 1) {
            exibirConfirmacao("Confirmação", "Realmente deseja substituir o motorista principal deste veículo? (Se optar por não, irá ser adicionado mais um motorista ao vínculo atual)", function () {
                var data = {
                    Veiculo: veiculo, Motorista: motorista, SegmentoVeiculo: 0, Substituir: true
                }
                if (contemVinculo) {
                    exibirConfirmacao("Confirmação", "O motorista " + nomeMotorista + " está vinculado à(ao) " + tipoVeiculoVinculo + " " + placaVinculo + ".  Deseja remover o motorista da(do) " + tipoVeiculoVinculo + " " + placaVinculo + " e adicionar à tração " + _pesquisaPlacas.Placa.val() + "?", function () {
                        lancarMotorista(data);
                    });
                } else {
                    lancarMotorista(data);
                }
                LimparCampoEntity(_pesquisaPlacas.Motorista);
            }, function () {
                var data = {
                    Veiculo: veiculo, Motorista: motorista, SegmentoVeiculo: 0, Substituir: false
                }
                if (contemVinculo) {
                    exibirConfirmacao("Confirmação", "O motorista " + nomeMotorista + " está vinculado à(ao) " + tipoVeiculoVinculo + " " + placaVinculo + ".  Deseja remover o motorista da(do) " + tipoVeiculoVinculo + " " + placaVinculo + " e adicionar à tração " + _pesquisaPlacas.Placa.val() + "?", function () {
                        lancarMotorista(data);
                    });
                } else {
                    lancarMotorista(data);
                }
                LimparCampoEntity(_pesquisaPlacas.Motorista);
            });
        } else if (contemVinculo) {
            exibirConfirmacao("Confirmação", "O motorista " + nomeMotorista + " está vinculado à(ao) " + tipoVeiculoVinculo + " " + placaVinculo + ".  Deseja remover o motorista da(do) " + tipoVeiculoVinculo + " " + placaVinculo + " e adicionar à tração " + _pesquisaPlacas.Placa.val() + "?", function () {
                var data = {
                    Veiculo: veiculo, Motorista: motorista, SegmentoVeiculo: 0, Substituir: true
                }
                lancarMotorista(data);
            });
        } else {
            var data = {
                Veiculo: veiculo, Motorista: motorista, SegmentoVeiculo: 0, Substituir: true
            }
            lancarMotorista(data);
        }
    }
}

function lancarMotorista(data) {
    executarReST("MovimentacaoDePlacas/InformarMotorista", data, function (e) {
        if (e.Success) {
            if (e.Data != false) {
                alterarMotorista(e.Data[0], data.Substituir);
                buscarMotoristas(callbackLancarReboqueMotorista);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}


function removerMotorista(veiculo, motorista) {
    var data = {
        Veiculo: veiculo, Motorista: motorista, SegmentoVeiculo: 0//_pesquisaPlacas.SegmentoVeiculo.codEntity()
    }
    executarReST("MovimentacaoDePlacas/RemoverMotorista", data, function (e) {
        if (e.Success) {
            if (e.Data != false) {
                LimparCampoEntity(_pesquisaPlacas.Motorista);
                buscarMotoristas(callbackRemoverReboqueMotorista);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}

function removerReboque(veiculo, reboque) {
    var data = {
        Veiculo: veiculo, Reboque: reboque, KMAtual: _pesquisaPlacas.KMAtual.val(), SegmentoVeiculo: 0//_pesquisaPlacas.SegmentoVeiculo.codEntity()
    }
    exibirModalInformarKmAtualVeiculoTracao(data.Veiculo, data.Reboque, 2, _CONFIGURACAO_TMS.InformarKmMovimentacaoPlaca).then(function (continuar) {
        if (continuar) {
            iniciarRequisicao();
            executarReST("MovimentacaoDePlacas/RemoverReboque", data, function (e) {
                if (e.Success) {
                    if (e.Data !== false) {
                        buscarReboques(callbackRemoverReboqueMotorista);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
                    }
                }
                else {
                    exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
                }
            });
        }
    });
}

function callbackRemoverReboqueMotorista() {
    buscarVeiculosAlterar();
}

function informarReboque(veiculo, reboque) {
    var data = {
        Veiculo: veiculo, Reboque: reboque, SegmentoVeiculo: 0//_pesquisaPlacas.SegmentoVeiculo.codEntity()
    }

    var contemVinculo = false;
    var placaVinculo = "";
    var placaReboque = "";
    $.each(_gridReboque.GridViewTable().rows().data(), function (i, reboqueGrid) {
        if (reboqueGrid.Codigo.split("_")[0] == reboque) {
            contemVinculo = reboqueGrid.ContemVinculo;
            placaVinculo = reboqueGrid.PlacaVinculo;
            placaReboque = reboqueGrid.Placa;
            return false;
        }
    });

    var contemReboque = false;
    $.each(_gridReboqueCodigo, function (i, gridReboqueCodigo) {
        if (gridReboqueCodigo.Codigo == reboque) {
            contemReboque = true;
            return false;
        }
    });

    if (!contemReboque) {
        var countReboques = 0;
        if (_gridReboqueCodigo != null && _gridReboqueCodigo.length != null)
            countReboques = _gridReboqueCodigo.length;

        if (countReboques >= 1) {
            exibirConfirmacao("Confirmação", "Realmente deseja adicionar mais um reboque ao veículo selecionado?", function () {
                if (contemVinculo) {
                    exibirConfirmacao("Confirmação", "O reboque " + placaReboque + " está vinculado a tração " + placaVinculo + ".  Deseja remover o reboque da tração " + placaVinculo + " e adicionar à tração " + _pesquisaPlacas.Placa.val() + "?", function () {
                        lancarReboque(data);
                    });
                } else {
                    lancarReboque(data);
                }
            });
        } else if (contemVinculo) {
            exibirConfirmacao("Confirmação", "O reboque " + placaReboque + " está vinculado a tração " + placaVinculo + ".  Deseja remover o reboque da tração " + placaVinculo + " e adicionar à tração " + _pesquisaPlacas.Placa.val() + "?", function () {
                lancarReboque(data);
            });
        } else {
            lancarReboque(data);
        }
    }
}

function lancarReboque(data) {
    exibirModalInformarKmAtualVeiculoTracao(data.Veiculo, data.Reboque, 1, _CONFIGURACAO_TMS.InformarKmMovimentacaoPlaca).then(function (continuar) {
        if (continuar) {
            iniciarRequisicao();
            executarReST("MovimentacaoDePlacas/InformarReboque", data, function (e) {
                if (e.Success) {
                    if (e.Data != false) {
                        alterarReboque(e.Data[0]);
                        buscarReboques(callbackLancarReboqueMotorista);
                    } else {
                        exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
                    }
                }
                else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", e.Msg);
                }
            });
        }
    });
}

function callbackLancarReboqueMotorista() {
    finalizarRequisicao();
}

function alterarReboque(reboque) {
    var contemReboque = false;
    $.each(_gridReboqueCodigo, function (i, gridReboqueCodigo) {
        if (gridReboqueCodigo.Codigo == reboque.Codigo) {
            contemReboque = true;
            var knoutReboque = new Reboque();
            preecherObjetoKnoutReboque(knoutReboque, reboque);
            knoutReboque.DivReboque.id = gridReboqueCodigo.idGrid;
            preencherDadosReboque(knoutReboque, reboque);
            return false;
        }
    });
    if (!contemReboque && reboque != null) {
        adicionarReboque(reboque);
    }
}

function adicionarReboque(reboque) {
    var knoutReboque = new Reboque();
    preecherObjetoKnoutReboque(knoutReboque, reboque);
    knoutReboque.DivReboque.id = knoutReboque.Codigo.idGrid;
    var html = "<div id='conteudo_" + knoutReboque.DivReboque.id + "'></div>";
    $("#fdsReboques").append(html);
    preencherDadosReboque(knoutReboque, reboque);
    _gridReboqueCodigo.push({ Codigo: reboque.Codigo, idGrid: knoutReboque.DivReboque.id });
}

function preecherObjetoKnoutReboque(knoutReboque, reboque) {
    var dataReboque = { Data: reboque };
    PreencherObjetoKnout(knoutReboque, dataReboque);
}

function alterarMotorista(motorista, substituir) {
    var contemMotorista = false;
    if (substituir) {
        $.each(_gridMotoristaCodigo, function (i, gridMotoristaCodigo) {
            if (gridMotoristaCodigo.Principal) {
                contemMotorista = true;
                var knoutMotorista = new Motorista();
                preecherObjetoKnoutMotorista(knoutMotorista, motorista);
                knoutMotorista.DivMotorista.id = gridMotoristaCodigo.idGrid;
                preencherDadosMotorista(knoutMotorista, motorista);
                return false;
            }
        });
    }

    if (!contemMotorista && motorista != null) {
        adicionarMotorista(motorista, substituir);
    }
}

function adicionarMotorista(motorista, substituir) {
    //$("#fdsMotoristas").html("");
    //_gridMotoristaCodigo = new Array();

    var knoutMotorista = new Motorista();
    preecherObjetoKnoutMotorista(knoutMotorista, motorista);
    knoutMotorista.DivMotorista.id = knoutMotorista.Codigo.idGrid;
    var html = "<div id='conteudo_" + knoutMotorista.DivMotorista.id + "'></div>";
    $("#fdsMotoristas").append(html);
    preencherDadosMotorista(knoutMotorista, motorista);
    _gridMotoristaCodigo.push({ Codigo: motorista.Codigo, idGrid: knoutMotorista.DivMotorista.id, Principal: substituir });
}

function preecherObjetoKnoutMotorista(knoutMotorista, motorista) {
    var dataMotorista = { Data: motorista };
    PreencherObjetoKnout(knoutMotorista, dataMotorista);

    if (motorista.NumeroHabilitacao != null && motorista.NumeroHabilitacao != "")
        knoutMotorista.NumeroHabilitacao.visible(true);
}
function alterarVeiculo(veiculo) {
    $.each(_gridVeiculoCodigo, function (i, gridVeiculoCodigo) {
        if (gridVeiculoCodigo.Codigo == veiculo.Codigo) {
            var knoutVeiculo = new Veiculo();
            preecherObjetoKnoutVeiculo(knoutVeiculo, veiculo);
            knoutVeiculo.DivVeiculo.id = gridVeiculoCodigo.idGrid;
            preencherDadosVeiculo(knoutVeiculo, veiculo);
            return false;
        }
    });
}
function preecherObjetoKnoutVeiculo(knoutVeiculo, veiculo) {
    var dataVeiculo = { Data: veiculo };
    PreencherObjetoKnout(knoutVeiculo, dataVeiculo);
}

function ImprimirMovimentacaoDePlacas(tipo) {
    var data = RetornarObjetoPesquisa(_pesquisaPlacas);
    data.tipoArquivo = tipo;
    executarDownload("MovimentacaoDePlacas/ImprimirMovimentacaoDePlacas", data);
}