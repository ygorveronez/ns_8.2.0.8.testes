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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Enumeradores/EnumTipoOperacaoCargaCTeManual.js" />
/// <reference path="CargaAgrupadaModeloVeicular.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _cargaAgrupada;
var _gridCarga;
var _gridNovaCarga;
var _listaCargasAgrupadas;
var _cargaAgrupadaCorAdicionado = "#AED6F1";
var _preCargaAgrupadaCorAdicionado = "#5a8cae";
var _modalEditarModeloVeicular;

var _situacaoTipoOperacao = [
    { value: EnumTipoOperacaoCargaCTeManual.NovaCarga, text: "Nova Carga" },
    { value: EnumTipoOperacaoCargaCTeManual.ManutencaoCarga, text: "Manutenção de Carga" }
];

var _listaCarga = new Array();

var retornarListaFiliais = (cargas) => {
    const opcoes = [{ text: "Nenhuma especificada ", value: 0 }];
    if (!cargas)
        return opcoes;

    for (let i = 0; i < cargas.length; i++)
        if (cargas[i].FilialCodigo != 0 && !opcoes.some(o => o.value === parseInt(cargas[i].FilialCodigo)))
            opcoes.push({ text: cargas[i].Filial, value: parseInt(cargas[i].FilialCodigo) })

    return opcoes;
}

var CargaAgrupada = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    const dataAtual = moment().add(-1, 'days').format("DD/MM/YYYY");

    this.CargasAgrupadas = PropertyEntity({ type: types.map, val: ko.observable(new Array()), def: new Array(), getType: typesKnockout.dynamic, required: false, idGrid: guid() });

    this.Filial = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.NovaCarga = PropertyEntity({ type: types.listEntity, list: new Array(), visibleFade: ko.observable(false), required: false, codEntity: ko.observable(0), idGrid: guid() });
    this.AgruparCarga = PropertyEntity({ eventClick: agruparCargasClick, type: types.event, text: (_CONFIGURACAO_TMS.PermitirRemoverCargasAgrupamentoCarga ? "Salvar Agrupamento" : "Agrupar Cargas"), visible: ko.observable(true) });
    this.FilialCargaAgrupada = PropertyEntity({ text: "Filial da Carga Agrupada:", val: ko.observable(0), def: 0, options: ko.observable(retornarListaFiliais()) });

    this.FilialCargaAgrupada.options.subscribe((opcoes) => {
        if (!opcoes.some(o => o.value === this.FilialCargaAgrupada.val()))
            this.FilialCargaAgrupada.val(0);
    })

    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: "Número do Pedido:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroCTe = PropertyEntity({ text: "Número do CT-e:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.NumeroNF = PropertyEntity({ text: "Número da NF:", val: ko.observable(""), def: "", visible: ko.observable(true), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid(), visible: false, cssSemClearClass: "" });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Tipo de Operação:", visible: ko.observable(true), idBtnSearch: guid(), cssClass: "col col-sm-3 col-md-3 col-lg-3" });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Modelo Veicular de Carga:", required: false, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Carga:", required: false, idBtnSearch: guid() });
    this.GrupoPessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Clientes:", idBtnSearch: guid(), visible: ko.observable(false) });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Remetente:", idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destinatário:", idBtnSearch: guid() });

    this.DataInicio = PropertyEntity({ text: "Data Início: ", getType: typesKnockout.date, val: ko.observable(dataAtual), def: dataAtual });
    this.DataFim = PropertyEntity({ text: "Data Limite: ", dateRangeInit: this.DataInicio, getType: typesKnockout.date });
    this.SomenteTerceiros = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Somente Terceiros?", visible: ko.observable(false) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;
    this.Situacao = PropertyEntity({ val: ko.observable("0"), options: _situacaoTipoOperacao, def: "0", text: "Situação da Carga: ", required: false });
    this.PlacaDeAgrupamento = PropertyEntity({ text: "Placa de Agrupamento:", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.CargaAgrupadaAlterar = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, text: "Carga Agrupada:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirRemoverCargasAgrupamentoCarga), idBtnSearch: guid() });
    this.TipoOperacaoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga, text: "*Tipo de Operação:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga), idBtnSearch: guid() });
    this.TransportadorCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe, text: "*Transportador:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga && _CONFIGURACAO_TMS.TipoServicoMultisoftware !== EnumTipoServicoMultisoftware.MultiCTe), idBtnSearch: guid() });
    this.RotaCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga, text: "*Rota:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga), idBtnSearch: guid() });
    this.ModeloVeicularCargaCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: _CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga || _CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel, text: "*Modelo Veicular:", visible: ko.observable(_CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga || _CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel), idBtnSearch: guid() });
    this.ZonaDeTransporte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Zona Transporte:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel) });

    this.CargaAgrupadaAlterar.codEntity.subscribe((valor) => {
        preencherGridCargasAgrupamento();
    });

    this.RetornarCargaDocumentoEmitido = PropertyEntity({ text: "Retornar as cargas com documentos já emitidos", type: types.bool, getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(_CONFIGURACAO_TMS.RetornarCargaDocumentoEmitido) });

    this.SomentePermiteAgrupamento = PropertyEntity({ type: types.bool, val: ko.observable(true), def: true, visible: ko.observable(false) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            CarregarCargas();
            carregarNovaCargas();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.PesquisarFilho = PropertyEntity({
        eventClick: function (e) {
            CarregarCargas();

        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
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

    this.SelecionarTodas = PropertyEntity({
        eventClick: adicionarTodasCargas, type: types.event, text: "Todos da Página", idFade: guid(), visible: ko.observable(true)
    });

    if (_CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel)
        this.ModeloVeicularCargaCarga.val.subscribe(buscarDivisaoCapacidadeCarregamentoVeiculo);

};

//*******EVENTOS*******

function loadCargaAgrupada() {
    _cargaAgrupada = new CargaAgrupada();
    KoBindings(_cargaAgrupada, "knockoutCargaAgrupada");

    BuscarFilial(_cargaAgrupada.Filial);
    BuscarTransportadores(_cargaAgrupada.Empresa, null, null, true);
    BuscarVeiculos(_cargaAgrupada.Veiculo, null, _cargaAgrupada.Empresa);
    BuscarClientes(_cargaAgrupada.Destinatario);
    BuscarTransportadores(_cargaAgrupada.TransportadorCarga);
    BuscarRotasFrete(_cargaAgrupada.RotaCarga);
    BuscarTiposOperacao(_cargaAgrupada.TipoOperacaoCarga);
    BuscarModelosVeicularesCarga(_cargaAgrupada.ModeloVeicularCargaCarga);
    BuscarCargaAgrupada(_cargaAgrupada.CargaAgrupadaAlterar);
    BuscarTiposDetalhe(_cargaAgrupada.ZonaDeTransporte);

    loadCargaAgrupadaModeloVeicular();

    if (_CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel) {
        $("#divGridCarga").hide();
        _cargaAgrupada.AgruparCarga.visible(false);
    } else {

        $("#" + _cargaAgrupada.NovaCarga.id).droppable({
            drop: function (event, ui) {
                const id = parseInt(ui.draggable[0].id);
                droppableCarga(id);
            },
            hoverClass: "ui-state-active bgPreCarga"
        });
    }

    carregarHTMLComponenteDivisaoCapacidadeVeicular(function () {
        RegistraComponenteDivisoesCapacidade();

    });

    _modalEditarModeloVeicular = new bootstrap.Modal(document.getElementById("divModalEditarModeloVeicular"), { backdrop: true, keyboard: true });
}

function agruparCargasClick(e, sender) {

    for (var i = 0; i < _listaCargasAgrupadas.length; i++) {
        _listaCargasAgrupadas[i].Descricao = "";
        _listaCargasAgrupadas[i].OrigemDestino = "";
    }
    _cargaAgrupada.CargasAgrupadas.val(JSON.stringify(_listaCargasAgrupadas));
    exibirConfirmacao("Confirmação", "Realmente deseja agrupar essas cargas? Lembrando que é um processo que não poderá mais ser modificado sem o cancelamento total do carregamento.", function () {
        Salvar(_cargaAgrupada, "CargaAgrupada/AgruparCargas", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Agrupamento realizado com sucesso!");
                    _listaCargasAgrupadas = new Array();
                    _cargaAgrupada.CargasAgrupadas.val(_cargaAgrupada.CargasAgrupadas.def);
                    _gridCarga.CarregarGrid();
                    recarregarGridNovaCarga();
                    limparCamposInformacoesCargaAgrupada();

                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function excluirPedidoClick(data) {
    $.each(_listaCarga, function (i, pedido) {
        if (data.Codigo == pedido.Codigo) {
            _listaCarga.splice(i, 1);
            return false;
        }
    });
}

//*******MÉTODOS*******

function limparCamposInformacoesCargaAgrupada() {
    LimparCampo(_cargaAgrupada.TransportadorCarga);
    LimparCampo(_cargaAgrupada.TipoOperacaoCarga);
    LimparCampo(_cargaAgrupada.RotaCarga);
    LimparCampo(_cargaAgrupada.ModeloVeicularCargaCarga);
    LimparCampo(_cargaAgrupada.CargaAgrupadaAlterar);

    $('#divResumoCapacidadeDados').append("");
    $('#divResumoCapacidadeDados').hide();
}

function CarregarCargas() {
    _cargaAgrupada.Carga.visibleFade(true);
    buscarCargas();
}

function buscarCargas() {
    _gridCarga = new GridView("grid-carga-agrupada", "Carga/PesquisaCargasNaGrid", _cargaAgrupada, null, null, 10, null, null, true, null, null, null, null, null, null, callbackRowGridCarga);

    _gridCarga.SetPermitirEdicaoColunas(true);
    _gridCarga.SetSalvarPreferenciasGrid(true);
    _gridCarga.CarregarGrid();
}

function carregarNovaCargas() {
    _listaCargasAgrupadas = new Array();

    var excluir = { descricao: "Excluir", id: guid(), metodo: excluirCargasSelecionadasClick };
    var editarModeloVeicular = { descricao: "Editar Modelo Veicular", id: guid(), metodo: editarModeloVeicularClick, icone: "", visibilidade: visibilidadeOpcaoEditarModeloVeicular };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list, tamanho: 7, opcoes: [
            excluir,
            editarModeloVeicular
        ]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCargaEmbarcador", title: "Número", width: "20%" },
        { data: "OrigemDestino", title: "Origem e Destino", width: "35%" },
        { data: "ModeloVeicularOrigem", title: "Modelo Veicular", width: "15%", visible: _CONFIGURACAO_TMS.PermiteInformarModeloVeicularCargaOrigem },
        { data: "Veiculo", title: "Veículo", width: "15%" }
    ];
    _gridNovaCarga = new BasicDataTable(_cargaAgrupada.NovaCarga.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    recarregarGridNovaCarga();
}

function recarregarGridNovaCarga() {
    const data = new Array();
    $.each(_listaCargasAgrupadas, function (i, cargaAgrupada) {
        cargaAgrupada.DT_RowColor = !cargaAgrupada.CargaDePreCarga ? "" : "#D3D3D3";
        data.push(cargaAgrupada);
    });
    _gridNovaCarga.CarregarGrid(data);
    _cargaAgrupada.FilialCargaAgrupada.options(retornarListaFiliais(data));
}

function excluirCargasSelecionadasClick(carga) {
    $.each(_listaCargasAgrupadas, function (i, cargasSelecionadas) {
        if (carga.Codigo == cargasSelecionadas.Codigo) {
            _listaCargasAgrupadas.splice(i, 1);
            _gridCarga.setarCorGridPorID(carga.Codigo, !carga.CargaDePreCarga ? "" : "#D3D3D3");
            return false;
        }
    });
    recarregarGridNovaCarga();
}

function editarModeloVeicularClick(carga) {
    _modalEditarModeloVeicular.show();

    _edicaoModeloVeicular.CodigoCarga.val(carga.Codigo);

    $("#divModalEditarModeloVeicular").on("hidden.bs.modal", function () {
        LimparCampos(_edicaoModeloVeicular);
    });
}

function adicionarTodasCargas() {
    var cargas = _gridCarga.GridViewTable().data();
    var adicionou = true;
    for (var i = 0; i < cargas.length; i++) {
        if (!adicionarCarga(cargas[i], false)) {
            adicionou = false;
            break;
        }
    }
    if (adicionarCarga) {
        _gridCarga;
    }
}

function adicionarCarga(carga, atualizar) {
    if (validarCarga(carga)) {

        if ((carga.PlacaDeAgrupamento) && (carga.PlacaDeAgrupamento != ""))
            carga.Placa = carga.PlacaDeAgrupamento;

        var recarregar = false;
        if (atualizar && !_CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga) {
            if (_cargaAgrupada.Empresa.codEntity() == 0 && carga.EmpresaCodigo > 0) {
                _cargaAgrupada.Empresa.codEntity(carga.EmpresaCodigo);
                _cargaAgrupada.Empresa.val(carga.Transportador);
                recarregar = true;
            }
        }
        droppableCarga
        _listaCargasAgrupadas.push(carga);
        recarregarGridNovaCarga();

        if (recarregar)
            _gridCarga.CarregarGrid();

        _gridCarga.setarCorGridPorID(carga.Codigo, !carga.CargaDePreCarga ? _cargaAgrupadaCorAdicionado : _preCargaAgrupadaCorAdicionado);
        return true;
    } else {
        return false;
    }
}

function droppableCarga(idCarga) {
    setTimeout(function () {
        var dataRow = _gridCarga.obterDataRow(idCarga);

        if (dataRow)
            adicionarCarga(dataRow.data, true);
    }, 50);
}

function validarCarga(carga) {
    var retorno = true;
    $.each(_listaCargasAgrupadas, function (i, cargaAgrupada) {
        if (cargaAgrupada.Codigo == carga.Codigo) {
            retorno = false;
            exibirMensagem(tipoMensagem.atencao, "Carga já existe", "Está carga já foi adicionada");
            return;
        }
        if (carga.EmpresaCodigo != cargaAgrupada.EmpresaCodigo && carga.RaizCNPJEmpresa != cargaAgrupada.RaizCNPJEmpresa && !_CONFIGURACAO_TMS.PermitirAlterarInformacoesAgrupamentoCarga) {
            retorno = false;
            exibirMensagem(tipoMensagem.atencao, "Transportador diferentes", "O Transportador precisa ser o mesmo para agrupar as suas cargas.");
            return;
        }
    });

    return retorno;
}

function ObterEmCargaAgrupada(codigo) {
    for (var i = 0; i < _listaCargasAgrupadas.length; i++) {

        if (_listaCargasAgrupadas[i].Codigo == codigo)
            return true;
    }

    return false;
}

function callbackRowGridCarga(row, data, c) {
    if (ObterEmCargaAgrupada(data.Codigo)) {
        data.DT_RowColor = !data.CargaDePreCarga ? _cargaAgrupadaCorAdicionado : _preCargaAgrupadaCorAdicionado;
        setarCorDataRow(row, data);
    }
}

function visibilidadeOpcaoEditarModeloVeicular() {
    return _CONFIGURACAO_TMS.PermiteInformarModeloVeicularCargaOrigem;
}

function preencherGridCargasAgrupamento() {
    _listaCargasAgrupadas = [];

    if (_CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel)
        $("#divGridCarga").hide();

    if (_cargaAgrupada.CargaAgrupadaAlterar.codEntity() == 0) {
        recarregarGridNovaCarga();
        return;
    }

    executarReST("CargaAgrupada/BuscarCargasDoAgrupamento", { CodigoCarga: _cargaAgrupada.CargaAgrupadaAlterar.codEntity() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _listaCargasAgrupadas = retorno.Data;
                recarregarGridNovaCarga();

                if (_CONFIGURACAO_TMS.PermitirAgrupamentoDeCargasOrdenavel)
                    $("#divGridCarga").show();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}


function buscarDivisaoCapacidadeCarregamentoVeiculo() {
    _cargaAgrupada.AgruparCarga.visible(true);
    BuscarDadosDivisaoCapacidadeVeiculo(_cargaAgrupada.ModeloVeicularCargaCarga.codEntity()).then(function () {
        //Mostrar campos...
        $("#divComponenteDivisaoCapacidade").show();
        $("#divResumoCapacidade").show();
    });

}


function RegistraComponenteDivisoesCapacidade() {
    if (ko.components.isRegistered('div-agrupar-divisao-capacidade'))
        return;

    ko.components.register('div-agrupar-divisao-capacidade', {
        viewModel: [AgruparPorDivisaoCapacidadeModeloVeicular],
        template: {
            element: 'div-agrupar-divisao-capacidade-templete'
        }
    });
}


function carregarHTMLComponenteDivisaoCapacidadeVeicular(callback) {
    $.get('Content/Static/Carga/CargaAgrupada/AgruparPorDivisaoCapacidadeModeloVeicular.html?dyn=' + guid(), function (html) {
        $('#divComponenteDivisaoCapacidade').html(html);
        loadAgruparPorDivisaoCapacidade();
        callback();
    })
}