/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFrota.js" />
/// <reference path="../../Enumeradores/EnumSituacaoDoConjuntoFrota.js" />
/// <reference path="Carga.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _pesquisaPlanejamentoFrota;
var _planejamentoFrota;
var _cargaSelecionada;
var _frotasCarga;
var _caminhoImagemPais = "../../../img/PlanejamentoFrota/";
var _gridJanelaCarregamento;

//var _gridPlanejamentoFrota;

var PesquisaPlanejamentoFrota = function () {
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid(), somenteVeiculosAtivos: true });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });

    this.SituacaoDaFrota = PropertyEntity({ text: "Situação da Frota:", val: ko.observable(EnumSituacaoFrota.Todos), options: EnumSituacaoFrota.obterOpcoesPesquisa(), def: EnumSituacaoFrota.Todos });
    this.SituacaoDoConjunto = PropertyEntity({ text: "Situação do Conjunto:", val: ko.observable(EnumSituacaoDoConjuntoFrota.Todos), options: EnumSituacaoDoConjuntoFrota.obterOpcoesPesquisa(), def: EnumSituacaoDoConjuntoFrota.Todos });

    this.VeiculoNecessitaManutencao = PropertyEntity({ text: "Veículo precisa de manutenção", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VeiculoComCarga = PropertyEntity({ text: "Veículo com carga no momento", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.MotoristaNecessitaIrCasa = PropertyEntity({ text: "Motorista precisa ir para casa", getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarPlanejamentoFrotas(1, false, null, null);
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtro de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade()) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var PlanejamentoFrota = function () {
    this.Frotas = PropertyEntity({ type: types.local, val: ko.observableArray([]) });

    this.Grid = PropertyEntity({ type: types.local });
    this.MostrarTipoGrid = PropertyEntity({ eventClick: () => exibirInformacoesComo("Grid"), type: types.event, idFade: guid() });
    this.MostrarTipoCard = PropertyEntity({ eventClick: () => exibirInformacoesComo("Card"), type: types.event, idFade: guid() });

    this.TipoVisual = PropertyEntity({ val: ko.observable("Card") })
};

var CardFrota = function (data) {
    if (data == null)
        data = {};

    this.Data = data;
    this.CodigoFrota = PropertyEntity({ val: ko.observable(data["CodigoFrota"]) });
    this.VeiculoTracao = PropertyEntity({ val: ko.observable(data["VeiculoTracao"]) });
    this.VeiculoReboque1 = PropertyEntity({ val: ko.observable(data["VeiculoReboque1"]) });
    this.VeiculoReboque2 = PropertyEntity({ val: ko.observable(data["VeiculoReboque2"]) });
    this.MotoristaPrincipal = PropertyEntity({ val: ko.observable(data["MotoristaPrincipal"]) });
    this.MotoristaAuxiliar = PropertyEntity({ val: ko.observable(data["MotoristaAuxiliar"]) });
    this.DataInicioVigencia = PropertyEntity({ val: ko.observable(data["DataInicioVigenciaFormatada"]) });
    this.DataFimVigencia = PropertyEntity({ val: ko.observable(data["DataFimVigenciaFormatada"]) });
    this.LocalVeiculoFrota = PropertyEntity({ val: ko.observable(data["LocalVeiculoFrota"]) });
    this.LatitudeFrota = PropertyEntity({ val: ko.observable(data["LatitudeFrota"]) });
    this.LongitudeFrota = PropertyEntity({ val: ko.observable(data["LongitudeFrota"]) });
    this.NomeMotoristaPrincipal = PropertyEntity({ val: ko.observable(data["NomeMotoristaPrincipal"]) });
    this.NomeMotoristaAuxiliar = PropertyEntity({ val: ko.observable(data["NomeMotoristaAuxiliar"]) });
    this.ModeloVeicularFormatado = PropertyEntity({ val: ko.observable(data["ModeloVeicularFormatado"]) });
    this.PlacaVeiculoTracao = PropertyEntity({ val: ko.observable(data["PlacaVeiculoTracao"]) });
    this.PlacaVeiculoReboque1 = PropertyEntity({ val: ko.observable(data["PlacaVeiculoReboque1"]) });
    this.PlacaVeiculoReboque2 = PropertyEntity({ val: ko.observable(data["PlacaVeiculoReboque2"]) });
    this.DescricaoLocalVeiculoFrota = PropertyEntity({ val: ko.observable(data["DescricaoLocalVeiculoFrota"]) });
    this.UFDescricaoLocalVeiculoFrota = PropertyEntity({ val: ko.observable(data["UFDescricaoLocalVeiculoFrota"]) });
    this.DescricaoStatus = PropertyEntity({ val: ko.observable(data["DescricaoStatus"]) });
    this.PossuiProgramacaoFutura = PropertyEntity({ val: ko.observable(data["PossuiProgramacaoFutura"]) });
    this.LocalPlanejamentoFormatado = PropertyEntity({ val: ko.observable(data["LocalPlanejamentoFormatado"]) });
    this.PaisDestino = PropertyEntity({ val: ko.observable(data["PaisDestino"]) });
    this.ManutencaoExpirada = PropertyEntity({ val: ko.observable(data["ManutencaoExpirada"]) });
    this.ExisteManutencaoProxima = PropertyEntity({ val: ko.observable(data["ExisteManutencaoProxima"]) });
    this.DistanciaKM = PropertyEntity({ val: ko.observable(data["DistanciaKM"]) });
    this.DataDisponivelCarregamento = PropertyEntity({ val: ko.observable(data["DataDisponivelCarregamento"]) });
    this.LocalidadeOrigem = PropertyEntity({ val: ko.observable(data["LocalidadeOrigem"]) });
    this.LocalidadeDestino = PropertyEntity({ val: ko.observable(data["LocalidadeDestino"]) });

    this.DataUltimaPosicao = PropertyEntity({ val: ko.observable(data["DataUltimaPosicao"]) });
    this.PosicaoAtual = PropertyEntity({ val: ko.observable(data["PosicaoAtual"]) });

    var ImagePaisPath = "";
    if (data["PaisDestino"] != 1058)
        ImagePaisPath = _caminhoImagemPais + data["PaisDestino"] + ".png";

    this.ImagePaisPath = PropertyEntity({ val: ko.observable(ImagePaisPath) });

    var Veiculos = data["PlacaVeiculoTracao"];

    if (data["PlacaVeiculoTracao"] == null)
        Veiculos = "Sem tração"

    if (data["PlacaVeiculoReboque1"] != undefined)
        Veiculos = Veiculos + ' / ' + data["PlacaVeiculoReboque1"];

    if (data["PlacaVeiculoReboque2"] != undefined)
        Veiculos = Veiculos + ' / ' + data["PlacaVeiculoReboque2"];


    this.Veiculos = PropertyEntity({ val: Veiculos });

    this.VisualizarDados = PropertyEntity({
        eventClick: function (e) {
            if (e == undefined) return;
            exibirDadosClick();
        }, type: types.event, text: "Dados da Carga", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

}

//*******EVENTOS*******

function loadPlanejamentoFrota() {
    _pesquisaPlanejamentoFrota = new PesquisaPlanejamentoFrota();
    KoBindings(_pesquisaPlanejamentoFrota, "knoutPesquisaPlanejamentoFrota", false, _pesquisaPlanejamentoFrota.Pesquisar.id);

    _planejamentoFrota = new PlanejamentoFrota();
    KoBindings(_planejamentoFrota, "knoutContainerPlanejamentoFrota");

    new BuscarVeiculos(_pesquisaPlanejamentoFrota.Veiculo);
    new BuscarMotoristas(_pesquisaPlanejamentoFrota.Motorista, null, null, null, true);
    new BuscarLocalidades(_pesquisaPlanejamentoFrota.Origem);
    new BuscarLocalidades(_pesquisaPlanejamentoFrota.Destino);

    loadPlanejamentoCarga();
    _cargaSelecionada = null;
    _frotasCarga = null;
    buscarPlanejamentoFrotas(1, false, null, null);
    configurarDroppable();

    loadGridPlanejamentoFrota();
}

//*******MÉTODOS*******

function buscarPlanejamentoFrotas(page, eventoPorPaginacao, cargaSelecionada, frotas) {
    var itensPorPagina = 12;

    var data = RetornarObjetoPesquisa(_pesquisaPlanejamentoFrota);
    data.FiltroPesquisa = RetornarJsonFiltroPesquisa(_pesquisaPlanejamentoFrota);

    data.inicio = itensPorPagina * (page - 1);
    data.limite = itensPorPagina;
    data.dataPesquisa = _planejamentoCarga.DataCarga.val();
    data.codigoCarga = cargaSelecionada;

    _cargaSelecionada = cargaSelecionada;
    _frotasCarga = frotas;

    executarReST("PlanejamentoFrota/Pesquisa", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                _planejamentoFrota.Frotas.val.removeAll();

                if (arg.Data.Frotas != undefined) {
                    for (var i = 0; i < arg.Data.Frotas.length; i++) {
                        var data = arg.Data.Frotas[i];
                        var card = new CardFrota(data);
                        _planejamentoFrota.Frotas.val.push(card);
                    }
                }

                configurarPaginacao(page, eventoPorPaginacao, arg, itensPorPagina);
                window.scrollTo(0, 0);
                $("#modal-filter").fadeOut("fast");

                drogableFrota();

                if (frotas != undefined)
                    marcarFrotasSelecionadas(frotas);

                RecarregarGridJanelaCarregamento();

                setTimeout(function () {
                    $("[rel=popover-hover]").popover({ trigger: "hover", container: "body", delay: { "show": 1000, "hide": 0 } });
                }, 1000);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function drogableFrota() {
    for (var i = 0; i < _planejamentoFrota.Frotas.val().length; i++) {
        var data = _planejamentoFrota.Frotas.val()[i];
        $("#card_" + data.CodigoFrota.val()).draggable({
            helper: "original",
            revert: true,
            cursor: "move"
        });
    }
}

function configurarDroppable() {

    for (var i = 0; i < _planejamentoCarga.Cargas.val().length; i++) {
        var carga = _planejamentoCarga.Cargas.val()[i];
        $("#carga_" + carga.Codigo.val()).droppable({
            drop: droppableItemcarga,
            hoverClass: "ui-state-active backgroundDropHover",
            activate: activateItem,
            deactivate: deactivateItem
        });
    }

    $("#knoutContainerPlanejamentoFrota").droppable({
        hoverClass: "ui-state-active backgroundDropHover",
        activate: activateItem,
        deactivate: deactivateItem
    });
}

function exibirDadosClick() {

}

function droppableItemcarga(event, ui) {
    var idFrota = ui.draggable[0].id.split("_")[1];
    var idCarga = event.target.id.split("_")[1];

    exibirConfirmacao("Vinculo carga a frota", "Você tem certeza que deseja vincular a carga?", function () {
        adicionarFrotaACarga(idCarga, idFrota);
    });
}

function activateItem(event, ui) {
    configurarDroppable();
    var tamanho = $("#" + ui.draggable[0].id).width();
    $(ui.helper[0]).width(tamanho);

    $("#" + ui.draggable[0].id).css('z-index', 5000);
    $("#divCargas").css('z-index', 4999);

    $.blockUI({ message: "" });
}

function deactivateItem(event, ui) {
    $.unblockUI();

    $("#divCargas").css('z-index', 2);
    $("#" + ui.draggable[0].id).css('z-index', 1);

    configurarDroppable();
}

function adicionarFrotaACarga(carga, frota) {
    var data = {
        Frota: frota, Carga: carga
    }

    iniciarRequisicao();

    executarReST("PlanejamentoFrota/VincularFrotaACarga", data, function (e) {
        if (e.Success) {
            if (e.Data !== false) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Frota vinculada com sucesso");
                buscarPlanejamentoFrotas(1, false, _cargaSelecionada, _frotasCarga);
                recarregarCargas();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
    });
}

function removerFrotaCarga(data) {
    var codigo = data.CodigoFrota.val();
    var carga = data.CodigoCarga.val();

    exibirConfirmacao("Vinculo carga a frota", "Você tem certeza que deseja remover a frota planejada da carga?", function () {
        var data = {
            Frota: codigo, Carga: carga
        }

        iniciarRequisicao();

        executarReST("PlanejamentoFrota/RemoverFrotaACarga", data, function (e) {
            if (e.Success) {
                if (e.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Frota removida com sucesso");
                    buscarPlanejamentoFrotas(1, false, _cargaSelecionada, _frotasCarga);
                    recarregarCargas();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    });

}

function configurarPaginacao(page, paginou, arg, itensPorPagina) {
    var clicouNoPaginar = false;

    if (!paginou) {
        if (arg.QuantidadeRegistros > 0) {

            var paginas = Math.ceil((arg.QuantidadeRegistros / itensPorPagina));

            if (paginas > 1) {
                $("#divPaginacaoCards").html('<ul style="float:right" id="paginacaoFrotas" class="pagination"></ul>');
                $('#paginacaoFrotas').twbsPagination({
                    first: 'Primeiro',
                    prev: 'Anterior',
                    next: 'Próximo',
                    last: 'Último',
                    totalPages: paginas,
                    visiblePages: 5,
                    onPageClick: function (event, page) {
                        if (clicouNoPaginar)
                            buscarPlanejamentoFrotas(page, true, _cargaSelecionada, _frotasCarga);

                        clicouNoPaginar = true;
                    }
                });
            } else
                $("#divPaginacaoCards").html('');

        }
        else
            $("#divPaginacaoCards").html('<span>Nenhum Registro Encontrado</span>');
    }
}

function marcarFrotasSelecionadas(frotas) {
    for (var i = 0; i < _planejamentoFrota.Frotas.val().length; i++) {
        var data = _planejamentoFrota.Frotas.val()[i];
        $("#card_" + data.CodigoFrota.val()).removeClass('card-selected');
    }

    for (var i = 0; i < frotas.length; i++) {
        var frota = frotas[i].CodigoFrota.val();
        $("#card_" + frota).addClass('card-selected');//se tem marca
    }
}

function exibirInformacoesComo(tipo) {
    if (tipo == "Card") {
        _planejamentoFrota.TipoVisual.val("Card");

        if (_planejamentoFrota.Frotas.val().length == 0)
            $("#divPaginacaoCards").html('<span>Nenhum Registro Encontrado</span>')

        ativeFundo(false);

        drogableFrota();
    }

    if (tipo == "Grid") {

        _planejamentoFrota.TipoVisual.val("Grid");
        ativeFundo(true)
        RecarregarGridJanelaCarregamento()
    }

}

function loadGridPlanejamentoFrota() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Detalhes", id: guid(), metodo: MostrarDetalhes }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Veiculo", title: "Veiculo", width: "15%" },
        { data: "Motorista", title: "Motorista", width: "15%" },
        { data: "Modelo", title: "Modelo", width: "10%" },
        { data: "Situacao", title: "Situação", width: "10%" },
        { data: "Origem", title: "Origem", width: "10%" },
        { data: "Destino", title: "Destino", width: "10%" },
        { data: "LocalAtual", title: "Local Atual", width: "10%" }
    ];

    _gridJanelaCarregamento = new BasicDataTable(_planejamentoFrota.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc }, null, 10, null, null, null, null, true);

    RecarregarGridJanelaCarregamento();
}

function MostrarDetalhes() {

}

function RecarregarGridJanelaCarregamento() {
    const lista = _planejamentoFrota.Frotas.val();

    if (!lista.length)
        return _gridJanelaCarregamento.CarregarGrid([]);

    const data = new Array();

    $.each(lista, function (i, item) {
        let itemGrid = new Object();

        itemGrid.Codigo = item.CodigoFrota.val();
        itemGrid.Veiculo = obterPlacas(item.PlacaVeiculoTracao.val(), item.PlacaVeiculoReboque1.val(), item.PlacaVeiculoReboque2.val());
        itemGrid.Motorista = item.NomeMotoristaPrincipal.val()
        itemGrid.Modelo = item.ModeloVeicularFormatado.val();
        itemGrid.Situacao = item.DescricaoStatus.val();
        itemGrid.Origem = item.LocalidadeOrigem.val();
        itemGrid.Destino = item.LocalidadeDestino.val();
        itemGrid.LocalAtual = item.LocalPlanejamentoFormatado.val();

        data.push(itemGrid);
    });

    _gridJanelaCarregamento.CarregarGrid(data);
}

function ativeFundo(ativo) {
    if (ativo)
        return $("#knoutContainerPlanejamentoFrota").css("background", "#fff");

    $("#knoutContainerPlanejamentoFrota").css("background", "none");
}

function obterPlacas(placa1, placa2, placa3) {
    let placa = "";

    if (!placa1)
        placa = "Sem tração";

    if (placa1)
        placa = placa1;

    if (placa2)
        placa = `${placa} / ${placa2}`;

    if (placa3)
        placa = `${placa} / ${placa3}`;

    return placa;
}