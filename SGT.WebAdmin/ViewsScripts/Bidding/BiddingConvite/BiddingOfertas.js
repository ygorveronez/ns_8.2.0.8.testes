/// <reference path="Origem.js" />
/// <reference path="Destino.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../ViewsScripts/Consultas/TipoCarga.js" />
/// <reference path="../../../ViewsScripts/Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Enumeradores/EnumTipoLanceBidding.js" />
/// <reference path="../../Enumeradores/EnumSimNaoNA.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../Consultas/ModeloCarroceria.js" />


var _ofertas, _gridRotas, _gridRotasBasicTable, _origens, _camposRota, _adicionarRotaBotoes, flagOrigem, flagDestino, _origensDestinos;
var _rotaSelecionada, rota = new Array();
var codigosOrigem, codigosDestino;
var AdicionarRotaBotoes = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarRotaClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRotaClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRotaClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var Ofertas = function () {
    this.PrazoOferta = PropertyEntity({ text: ko.observable("*Prazo Preenchimento Oferta:"), required: ko.observable(true), getType: typesKnockout.dateTime, val: ko.observable(""), enable: ko.observable(true) });
    this.PermitirInformarVeiculosVerdes = PropertyEntity({ type: types.bool, val: ko.observable(false), def: false, text: "Permitir informar veículos verdes" });
    this.TipoLance = PropertyEntity({ def: EnumTipoLanceBidding.NaoSelecionado, options: EnumTipoLanceBidding.ObterOpcoes(), val: ko.observable(EnumTipoLanceBidding.NaoSelecionado), text: "*Tipo de lance:", required: ko.observable(true) });
    this.TipoLance.val.subscribe(function () {
        HabilitarCamposModal();
    });
}

var GridRotas = function () {
    this.Rotas = PropertyEntity({ type: types.local });
    this.Adicionar = PropertyEntity({ eventClick: adicionarOfertaModalClick, type: types.event, text: "Adicionar Rota", visible: ko.observable(true) });
    this.Excluir = PropertyEntity({ eventClick: excluirRotasSelecionadasClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Importar = PropertyEntity({
        type: types.local,
        text: Localization.Resources.Gerais.Geral.Importar,
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv,.txt",
        ManterArquivoServidor: false,
        UrlImportacao: "BiddingConvite/ImportarRotas",
        UrlConfiguracao: "BiddingConvite/ConfiguracaoImportacaoRotas",
        CodigoControleImportacao: EnumCodigoControleImportacao.O067_BiddingConviteOfertaRotas,
        CallbackImportacao: (args) => {
            let dados = args.Data.Retorno;
            dados.forEach(x => {
                x.Codigo = 0;
                SetValorDefaultRegistroGridRotas(x);
                rota.push(x);
            });

            _gridRotasBasicTable.CarregarGrid(rota);
            _gridRotas.Rotas.val(rota);
        }
    });
}

var CamposRota = function () {
    this.RotaDescricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string });
    this.Frequencia = PropertyEntity({ text: "Frequência Mensal sem ajudante:", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.Volume = PropertyEntity({ text: "Volume (caixas expedidas por mês):", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.Peso = PropertyEntity({ text: "Peso (Ton) Mês:", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.AdicionalAPartirDaEntregaNumero = PropertyEntity({ text: "Adicional a partir da entrega N°:", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.NumeroEntrega = PropertyEntity({ text: "Quantidade de Entregas:", required: false, getType: typesKnockout.int, enable: ko.observable(true) });
    this.ValorCargaMes = PropertyEntity({ text: "Valor Carga:", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.QuilometragemMedia = PropertyEntity({ text: "KM Média da Rota:", required: ko.observable(false), getType: typesKnockout.decimal, });
    this.TipoCarga = PropertyEntity({ required: true, codEntity: ko.observable(0), type: types.multiplesEntities, text: "*Tipos Carga:", idBtnSearch: guid(), enable: ko.observable(false) });
    this.ModeloVeicular = PropertyEntity({ required: true, type: types.multiplesEntities, codEntity: ko.observable(0), text: "*Modelo Veicular:", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", required: false, getType: typesKnockout.text, maxlength: 150 });

    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.GrupoModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo Modelo Veicular: ", idBtnSearch: guid(), required: false });
    this.FiliaisParticipante = PropertyEntity({ codEntity: ko.observable(0), type: types.multiplesEntities, text: "*Filiais Participantes:", idBtnSearch: guid(), enable: ko.observable(true), required: true });
    this.ModeloCarroceria = PropertyEntity({ text: "Modelo Carroceria Veículo: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), enable: ko.observable(true), required: ko.observable(true), visible: ko.observable(true) });
    this.FrequenciaMensalComAjudante = PropertyEntity({ text: "Frequência Mensal com Ajudante: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.QuantidadeAjudantePorVeiculo = PropertyEntity({ text: "Quantidade de Ajudantes por Veículo: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, required: false, enable: ko.observable(true) });
    this.MediaEntregasFracionada = PropertyEntity({ text: "Média de Entregas Fracionadas:", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.MaximaEntregasFacionada = PropertyEntity({ text: "Máxima de Entregas Fracionadas: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.Inconterm = PropertyEntity({ val: ko.observable(EnumInconterm.CIF), options: EnumInconterm.obterOpcoes(), def: EnumInconterm.CIF, text: "Incoterm: ", visible: ko.observable(true), required: false });
    this.QuantidadeViagensPorAno = PropertyEntity({ text: "Quantidade de Cargas Ano: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.VolumeTonAno = PropertyEntity({ text: "Volume (Ton) Ano: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.VolumeTonViagem = PropertyEntity({ text: "Volume (Ton) Carga: ", val: ko.observable(""), def: ko.observable(""), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false }, enable: ko.observable(true) });
    this.ValorMedioNFe = PropertyEntity({ text: "Valor Médio NF-e:", required: false, getType: typesKnockout.decimal });
    this.TempoColeta = PropertyEntity({ text: "Tempo de Coleta:", getType: typesKnockout.time, enable: ko.observable(true) });
    this.TempoDescarga = PropertyEntity({ text: "Tempo de Descarga:", getType: typesKnockout.time, enable: ko.observable(true) });
    this.Compressor = PropertyEntity({ val: ko.observable(EnumSimNaoNA.NaoAplicavel), options: EnumSimNaoNA.obterOpcoesNA(), def: EnumSimNaoNA.NaoAplicavel, text: "Compressor: ", visible: ko.observable(true), enable: ko.observable(true) });

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
}

var OrigensDestinos = function () {
    //#region Origens
    this.Origem = PropertyEntity({ type: types.event, text: "Adicionar Origem", idBtnSearch: guid(), issue: 16, enable: ko.observable(true) });
    this.GridOrigem = PropertyEntity({ type: types.local });
    this.Origens = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ClienteOrigem = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid(), issue: 55, enable: ko.observable(true) });
    this.GridClienteOrigem = PropertyEntity({ type: types.local });
    this.ClientesOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.EstadoOrigem = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid(), issue: 12, enable: ko.observable(true) });
    this.GridEstadoOrigem = PropertyEntity({ type: types.local });
    this.EstadosOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RegiaoOrigem = PropertyEntity({ type: types.event, text: "Adicionar Região", idBtnSearch: guid(), issue: 110, enable: ko.observable(true) });
    this.GridRegiaoOrigem = PropertyEntity({ type: types.local });
    this.RegioesOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RotaOrigem = PropertyEntity({ type: types.event, text: "Adicionar Rota", idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridRotaOrigem = PropertyEntity({ type: types.local });
    this.RotasOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PaisOrigem = PropertyEntity({ type: types.event, text: "Adicionar País", idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridPaisOrigem = PropertyEntity({ type: types.local });
    this.PaisesOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ListaCEPsOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.CEPsOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.GridCEPOrigem = PropertyEntity({ type: types.local });
    this.CEPOrigemInicial = PropertyEntity({ text: "CEP Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPOrigemFinal = PropertyEntity({ text: "CEP Final:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.AdicionarCEPOrigem = PropertyEntity({ eventClick: AdicionarCEPOrigemClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });
    //#endregion Origens

    //#region Destinos
    this.FreteValidoParaQualquerDestino = PropertyEntity({ text: "O valor desta tabela é válido para qualquer um dos destinos informados", issue: 727, val: ko.observable(false), def: false, required: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Destino = PropertyEntity({ type: types.event, text: "Adicionar Destino", idBtnSearch: guid(), issue: 16, enable: ko.observable(true) });
    this.GridDestino = PropertyEntity({ type: types.local });
    this.Destinos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ClienteDestino = PropertyEntity({ type: types.event, text: "Adicionar Cliente", idBtnSearch: guid(), issue: 55, enable: ko.observable(true) });
    this.GridClienteDestino = PropertyEntity({ type: types.local });
    this.ClientesDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.EstadoDestino = PropertyEntity({ type: types.event, text: "Adicionar Estado", idBtnSearch: guid(), issue: 12, enable: ko.observable(true) });
    this.GridEstadoDestino = PropertyEntity({ type: types.local });
    this.EstadosDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RegiaoDestino = PropertyEntity({ type: types.event, text: "Adicionar Região", idBtnSearch: guid(), issue: 110, enable: ko.observable(true) });
    this.GridRegiaoDestino = PropertyEntity({ type: types.local });
    this.RegioesDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.RotaDestino = PropertyEntity({ type: types.event, text: "Adicionar Rota", idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridRotaDestino = PropertyEntity({ type: types.local });
    this.RotasDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.PaisDestino = PropertyEntity({ type: types.event, text: "Adicionar País", idBtnSearch: guid(), issue: 0, enable: ko.observable(true) });
    this.GridPaisDestino = PropertyEntity({ type: types.local });
    this.PaisesDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.ListaCEPsDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.CEPsDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.GridCEPDestino = PropertyEntity({ type: types.local });
    this.CEPDestinoInicial = PropertyEntity({ text: "CEP Inicial:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.CEPDestinoFinal = PropertyEntity({ text: "CEP Final:", val: ko.observable(""), def: "", getType: typesKnockout.cep, visible: ko.observable(true) });
    this.AdicionarCEPDestino = PropertyEntity({ eventClick: AdicionarCEPDestinoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });
    //#endregion Destinos

    this.ListaBaseline = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
}

function loadOfertas() {
    _ofertas = new Ofertas();
    KoBindings(_ofertas, "knockoutOfertaOpcoes");

    _gridRotas = new GridRotas();
    KoBindings(_gridRotas, "knockoutGridRotas");

    _origensDestinos = new OrigensDestinos();
    KoBindings(_origensDestinos, "knockoutOrigemDestino");

    _camposRota = new CamposRota();
    KoBindings(_camposRota, "knockoutCamposRota");

    _adicionarRotaBotoes = new AdicionarRotaBotoes();
    KoBindings(_adicionarRotaBotoes, "knockoutAdicionarRota");

    loadGridRotas();
    loadOrigens();
    loadDestinos();
    loadFuncoesBotoes();
    loadBaseline();
}

function loadFuncoesBotoes() {
    BuscarModelosVeicularesCarga(_camposRota.ModeloVeicular);
    BuscarTiposdeCarga(_camposRota.TipoCarga);
    BuscarGrupoModeloVeicular(_camposRota.GrupoModeloVeicular);
    BuscarFilial(_camposRota.FiliaisParticipante);
    BuscarClientes(_camposRota.Tomador);
    BuscarModelosCarroceria(_camposRota.ModeloCarroceria);
}

function loadGridRotas() {
    const linhasPorPagina = 10;
    const opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarRotaClick, icone: "", visiblidade: true };
    const menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", visible: false },
        { data: "Frequencia", visible: false },
        { data: "Peso", visible: false },
        { data: "AdicionalAPartirDaEntregaNumero", visible: false },
        { data: "Volume", visible: false },
        { data: "ValorCargaMes", visible: false },
        { data: "QuilometragemMedia", visible: false },
        { data: "TipoCarga", visible: false },
        { data: "ModeloVeicular", visible: false },
        { data: "FlagOrigem", visible: false },
        { data: "FlagDestino", visible: false },
        { data: "CidadeOrigem", visible: false },
        { data: "ClienteOrigem", visible: false },
        { data: "EstadoOrigem", visible: false },
        { data: "RegiaoOrigem", visible: false },
        { data: "RotaOrigem", visible: false },
        { data: "PaisOrigem", visible: false },
        { data: "CEPOrigem", visible: false },
        { data: "CidadeDestino", visible: false },
        { data: "ClienteDestino", visible: false },
        { data: "EstadoDestino", visible: false },
        { data: "RegiaoDestino", visible: false },
        { data: "RotaDestino", visible: false },
        { data: "PaisDestino", visible: false },
        { data: "CEPDestino", visible: false },
        { data: "Observacao", visible: false },
        { data: "Baseline", visible: false },
        { data: "Tomador", visible: false },
        { data: "GrupoModeloVeicular", visible: false },
        { data: "ModeloCarroceria", visible: false },
        { data: "FrequenciaMensalComAjudante", visible: false },
        { data: "QuantidadeAjudantePorVeiculo", visible: false },
        { data: "MediaEntregasFracionada", visible: false },
        { data: "MaximaEntregasFacionada", visible: false },
        { data: "Inconterm", visible: false },
        { data: "QuantidadeViagensPorAno", visible: false },
        { data: "VolumeTonAno", visible: false },
        { data: "VolumeTonViagem", visible: false },
        { data: "ValorMedioNFe", visible: false },
        { data: "TempoColeta", visible: false },
        { data: "TempoDescarga", visible: false },
        { data: "Compressor", visible: false },
        { data: "Rota", title: "Rota", width: "100%", className: "text-align-left" }
    ];

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };
    _gridRotasBasicTable = new BasicDataTable(_gridRotas.Rotas.id, header, menuOpcoes, null, configRowsSelect, linhasPorPagina, null, null, null, null, null, null, null, null, null, null, callbackRegistroSelecionadoChange);
    _gridRotasBasicTable.CarregarGrid([]);
}

function callbackRegistroSelecionadoChange(registro, selecionado) {
    var rotasSelecionadas = _gridRotasBasicTable.ListaSelecionados();

    _gridRotas.Excluir.visible(rotasSelecionadas.length > 0);
}

function excluirRotasSelecionadasClick() {
    var rotasSelecionadas = _gridRotasBasicTable.ListaSelecionados();

    rota = rota.filter(function (registro) {
        var excluir = rotasSelecionadas.some(function (rotaSelecionada) {
            return rotaSelecionada.Codigo === registro.Codigo;
        });

        return !excluir;
    });

    _gridRotas.Rotas.val(rota);
    _gridRotasBasicTable.CarregarGrid(rota);
}

//Funções Click
function adicionarOfertaModalClick() {
    Global.abrirModal('divModalAdicionarRota');
    $("#divModalAdicionarRota").one('hidden.bs.modal', function () {
        _gridRotasBasicTable.CarregarGrid(rota);
        LimparCampos(_camposRota);
        _camposRota.ModeloVeicular.multiplesEntities();
        _camposRota.TipoCarga.multiplesEntities();
        _camposRota.FiliaisParticipante.multiplesEntities();
        LimparOrigemDestino();
        trocarControles(false);
        _rotaSelecionada = null;
        _gridRotas.Rotas.val(rota);
        limparCamposValoresBaseline();
    });
}

function adicionarRotaClick() {
    if (!ValidarCamposObrigatorios(_camposRota)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    if (!checarOrigemDestino()) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Você precisa selecionar origem e destino!");
        return;
    }

    rota.push({
        Codigo: guid(),
        Rota: _camposRota.RotaDescricao.val(), Descricao: _camposRota.RotaDescricao.val(),
        Frequencia: _camposRota.Frequencia.val(), Volume: _camposRota.Volume.val(), Peso: _camposRota.Peso.val(),
        AdicionalAPartirDaEntregaNumero: _camposRota.AdicionalAPartirDaEntregaNumero.val(),
        NumeroEntrega: _camposRota.NumeroEntrega.val(),
        ValorCargaMes: _camposRota.ValorCargaMes.val(),
        QuilometragemMedia: _camposRota.QuilometragemMedia.val(), TipoCarga: _camposRota.TipoCarga.multiplesEntities(),
        ModeloVeicular: _camposRota.ModeloVeicular.multiplesEntities(), CidadeOrigem: _origensDestinos.Origem.basicTable.BuscarRegistros(),
        FiliaisParticipante: _camposRota.FiliaisParticipante.multiplesEntities(),
        CidadeDestino: _origensDestinos.Destino.basicTable.BuscarRegistros(), ClienteOrigem: _origensDestinos.ClienteOrigem.basicTable.BuscarRegistros(),
        ClienteDestino: _origensDestinos.ClienteDestino.basicTable.BuscarRegistros(), EstadoOrigem: _origensDestinos.EstadoOrigem.basicTable.BuscarRegistros(),
        EstadoDestino: _origensDestinos.EstadoDestino.basicTable.BuscarRegistros(), RegiaoOrigem: _origensDestinos.RegiaoOrigem.basicTable.BuscarRegistros(),
        RegiaoDestino: _origensDestinos.RegiaoDestino.basicTable.BuscarRegistros(), PaisOrigem: _origensDestinos.PaisOrigem.basicTable.BuscarRegistros(),
        PaisDestino: _origensDestinos.PaisDestino.basicTable.BuscarRegistros(), CEPOrigem: _origensDestinos.CEPsOrigem.val(), CEPDestino: _origensDestinos.CEPsDestino.val(),
        RotaOrigem: _origensDestinos.RotaOrigem.basicTable.BuscarRegistros(), RotaDestino: _origensDestinos.RotaDestino.basicTable.BuscarRegistros(),
        Observacao: _camposRota.Observacao.val(), FlagOrigem: flagOrigem, FlagDestino: flagDestino,
        Baseline: _baseline.ListaBaseline.val(),
        Tomador: { Codigo: _camposRota.Tomador.codEntity(), Descricao: _camposRota.Tomador.val() },
        GrupoModeloVeicular: { Codigo: _camposRota.GrupoModeloVeicular.codEntity(), Descricao: _camposRota.GrupoModeloVeicular.val() },
        ModeloCarroceria: { Codigo: _camposRota.ModeloCarroceria.codEntity(), Descricao: _camposRota.ModeloCarroceria.val() },
        FrequenciaMensalComAjudante: _camposRota.FrequenciaMensalComAjudante.val(),
        QuantidadeAjudantePorVeiculo: _camposRota.QuantidadeAjudantePorVeiculo.val(),
        MediaEntregasFracionada: _camposRota.MediaEntregasFracionada.val(),
        MaximaEntregasFacionada: _camposRota.MaximaEntregasFacionada.val(),
        Inconterm: _camposRota.Inconterm.val(),
        QuantidadeViagensPorAno: _camposRota.QuantidadeViagensPorAno.val(),
        VolumeTonAno: _camposRota.VolumeTonAno.val(),
        VolumeTonViagem: _camposRota.VolumeTonViagem.val(),
        ValorMedioNFe: _camposRota.ValorMedioNFe.val(),
        TempoColeta: _camposRota.TempoColeta.val(),
        TempoDescarga: _camposRota.TempoDescarga.val(),
        Compressor: _camposRota.Compressor.val(),
    });

    Global.fecharModal('divModalAdicionarRota');
}

function excluirRotaClick() {
    let index = questionario.indexOf(_rotaSelecionada);
    rota.splice(index, 1);
    Global.fecharModal('divModalAdicionarRota');
}

function atualizarRotaClick() {
    if (!ValidarCamposObrigatorios(_camposRota)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    if (!checarOrigemDestino()) {
        exibirMensagem(tipoMensagem.atencao, "Rota sem origens e/ou destinos", "Sua rota está salva sem origens e/ou destinos!");
        return;
    }

    _rotaSelecionada.Rota = _camposRota.RotaDescricao.val();
    _rotaSelecionada.Descricao = _camposRota.RotaDescricao.val();
    _rotaSelecionada.Frequencia = _camposRota.Frequencia.val();
    _rotaSelecionada.Peso = _camposRota.Peso.val();
    _rotaSelecionada.AdicionalAPartirDaEntregaNumero = _camposRota.AdicionalAPartirDaEntregaNumero.val();
    _rotaSelecionada.Volume = _camposRota.Volume.val();
    _rotaSelecionada.NumeroEntrega = _camposRota.NumeroEntrega.val();
    _rotaSelecionada.ValorCargaMes = _camposRota.ValorCargaMes.val();
    _rotaSelecionada.QuilometragemMedia = _camposRota.QuilometragemMedia.val();
    _rotaSelecionada.Observacao = _camposRota.Observacao.val();
    _rotaSelecionada.Tomador = { Codigo: _camposRota.Tomador.codEntity(), Descricao: _camposRota.Tomador.val() };
    _rotaSelecionada.GrupoModeloVeicular = { Codigo: _camposRota.GrupoModeloVeicular.codEntity(), Descricao: _camposRota.GrupoModeloVeicular.val() };
    _rotaSelecionada.ModeloCarroceria = { Codigo: _camposRota.ModeloCarroceria.codEntity(), Descricao: _camposRota.ModeloCarroceria.val() };
    _rotaSelecionada.FrequenciaMensalComAjudante = _camposRota.FrequenciaMensalComAjudante.val();
    _rotaSelecionada.QuantidadeAjudantePorVeiculo = _camposRota.QuantidadeAjudantePorVeiculo.val();
    _rotaSelecionada.MediaEntregasFracionada = _camposRota.MediaEntregasFracionada.val();
    _rotaSelecionada.MaximaEntregasFacionada = _camposRota.MaximaEntregasFacionada.val();
    _rotaSelecionada.Inconterm = _camposRota.Inconterm.val();
    _rotaSelecionada.QuantidadeViagensPorAno = _camposRota.QuantidadeViagensPorAno.val();
    _rotaSelecionada.VolumeTonAno = _camposRota.VolumeTonAno.val();
    _rotaSelecionada.VolumeTonViagem = _camposRota.VolumeTonViagem.val();
    _rotaSelecionada.ValorMedioNFe = _camposRota.ValorMedioNFe.val();
    _rotaSelecionada.TempoColeta = _camposRota.TempoColeta.val();
    _rotaSelecionada.TempoDescarga = _camposRota.TempoDescarga.val();
    _rotaSelecionada.Compressor = _camposRota.Compressor.val();

    _rotaSelecionada.ModeloVeicular = _camposRota.ModeloVeicular.multiplesEntities();
    _rotaSelecionada.FiliaisParticipante = _camposRota.FiliaisParticipante.multiplesEntities();
    _rotaSelecionada.TipoCarga = _camposRota.TipoCarga.multiplesEntities();


    _rotaSelecionada.CidadeOrigem = _origensDestinos.Origem.basicTable.BuscarRegistros();
    _rotaSelecionada.EstadoOrigem = _origensDestinos.EstadoOrigem.basicTable.BuscarRegistros();
    _rotaSelecionada.PaisOrigem = _origensDestinos.PaisOrigem.basicTable.BuscarRegistros();
    _rotaSelecionada.RegiaoOrigem = _origensDestinos.RegiaoOrigem.basicTable.BuscarRegistros();
    _rotaSelecionada.ClienteOrigem = _origensDestinos.ClienteOrigem.basicTable.BuscarRegistros();
    _rotaSelecionada.RotaOrigem = _origensDestinos.RotaOrigem.basicTable.BuscarRegistros();
    _rotaSelecionada.CEPOrigem = _origensDestinos.CEPsOrigem.val();
    _rotaSelecionada.FlagOrigem = flagOrigem;

    _rotaSelecionada.CidadeDestino = _origensDestinos.Destino.basicTable.BuscarRegistros();
    _rotaSelecionada.EstadoDestino = _origensDestinos.EstadoDestino.basicTable.BuscarRegistros();
    _rotaSelecionada.PaisDestino = _origensDestinos.PaisDestino.basicTable.BuscarRegistros();
    _rotaSelecionada.RegiaoDestino = _origensDestinos.RegiaoDestino.basicTable.BuscarRegistros();
    _rotaSelecionada.ClienteDestino = _origensDestinos.ClienteDestino.basicTable.BuscarRegistros();
    _rotaSelecionada.RotaDestino = _origensDestinos.RotaDestino.basicTable.BuscarRegistros();
    _rotaSelecionada.CEPDestino = _origensDestinos.CEPsDestino.val();
    _rotaSelecionada.FlagDestino = flagDestino;

    Global.fecharModal("divModalAdicionarRota");
}

function editarRotaClick(registroSelecionado) {
    _rotaSelecionada = registroSelecionado;

    _camposRota.RotaDescricao.val(registroSelecionado.Rota);
    _camposRota.Frequencia.val(registroSelecionado.Frequencia);
    _camposRota.Peso.val(registroSelecionado.Peso);
    _camposRota.AdicionalAPartirDaEntregaNumero.val(registroSelecionado.AdicionalAPartirDaEntregaNumero);
    _camposRota.Volume.val(registroSelecionado.Volume);
    _camposRota.ValorCargaMes.val(registroSelecionado.ValorCargaMes);
    _camposRota.QuilometragemMedia.val(registroSelecionado.QuilometragemMedia);
    _camposRota.TipoCarga.multiplesEntities(registroSelecionado.TipoCarga);
    _camposRota.ModeloVeicular.multiplesEntities(registroSelecionado.ModeloVeicular);
    _camposRota.FiliaisParticipante.multiplesEntities(registroSelecionado.FiliaisParticipante);
    _camposRota.NumeroEntrega.val(registroSelecionado.NumeroEntrega);
    _camposRota.Observacao.val(registroSelecionado.Observacao);
    _camposRota.Tomador.val(registroSelecionado.Tomador.Descricao);
    _camposRota.Tomador.codEntity(registroSelecionado.Tomador.Codigo);
    _camposRota.GrupoModeloVeicular.val(registroSelecionado.GrupoModeloVeicular.Descricao);
    _camposRota.GrupoModeloVeicular.codEntity(registroSelecionado.GrupoModeloVeicular.Codigo);
    _camposRota.ModeloCarroceria.codEntity(registroSelecionado.ModeloCarroceria.Codigo);
    _camposRota.ModeloCarroceria.val(registroSelecionado.ModeloCarroceria.Descricao);
    _camposRota.FrequenciaMensalComAjudante.val(registroSelecionado.FrequenciaMensalComAjudante);
    _camposRota.QuantidadeAjudantePorVeiculo.val(registroSelecionado.QuantidadeAjudantePorVeiculo);
    _camposRota.MediaEntregasFracionada.val(registroSelecionado.MediaEntregasFracionada);
    _camposRota.MaximaEntregasFacionada.val(registroSelecionado.MaximaEntregasFacionada);
    _camposRota.Inconterm.val(registroSelecionado.Inconterm);
    _camposRota.QuantidadeViagensPorAno.val(registroSelecionado.QuantidadeViagensPorAno);
    _camposRota.VolumeTonAno.val(registroSelecionado.VolumeTonAno);
    _camposRota.VolumeTonViagem.val(registroSelecionado.VolumeTonViagem);
    _camposRota.ValorMedioNFe.val(registroSelecionado.ValorMedioNFe);
    _camposRota.TempoColeta.val(registroSelecionado.TempoColeta);
    _camposRota.TempoDescarga.val(registroSelecionado.TempoDescarga);
    _camposRota.Compressor.val(registroSelecionado.Compressor);
    _baseline.ListaBaseline.val(registroSelecionado.Baseline);

    if (registroSelecionado.FlagOrigem == "Cidade") {
        _origensDestinos.Origem.basicTable.SetarRegistros(_rotaSelecionada.CidadeOrigem);
        _origensDestinos.Origem.basicTable.CarregarGrid(_rotaSelecionada.CidadeOrigem);
    }
    else if (registroSelecionado.FlagOrigem == "Cliente") {
        _origensDestinos.ClienteOrigem.basicTable.SetarRegistros(_rotaSelecionada.ClienteOrigem);
        _origensDestinos.ClienteOrigem.basicTable.CarregarGrid(_rotaSelecionada.ClienteOrigem);
    }
    else if (registroSelecionado.FlagOrigem == "Estado") {
        _origensDestinos.EstadoOrigem.basicTable.SetarRegistros(_rotaSelecionada.EstadoOrigem);
        _origensDestinos.EstadoOrigem.basicTable.CarregarGrid(_rotaSelecionada.EstadoOrigem);
    }
    else if (registroSelecionado.FlagOrigem == "Regiao") {
        _origensDestinos.RegiaoOrigem.basicTable.SetarRegistros(_rotaSelecionada.RegiaoOrigem);
        _origensDestinos.RegiaoOrigem.basicTable.CarregarGrid(_rotaSelecionada.RegiaoOrigem);
    }
    else if (registroSelecionado.FlagOrigem == "Rota") {
        _origensDestinos.RotaOrigem.basicTable.SetarRegistros(_rotaSelecionada.RotaOrigem);
        _origensDestinos.RotaOrigem.basicTable.CarregarGrid(_rotaSelecionada.RotaOrigem);
    }
    else if (registroSelecionado.FlagOrigem == "Pais") {
        _origensDestinos.PaisOrigem.basicTable.SetarRegistros(_rotaSelecionada.PaisOrigem);
        _origensDestinos.PaisOrigem.basicTable.CarregarGrid(_rotaSelecionada.PaisOrigem);
    }
    else if (registroSelecionado.FlagOrigem == "CEP") {
        _origensDestinos.CEPsOrigem.val(_rotaSelecionada.CEPOrigem);
        RecarregarGridCEPOrigem();
    }

    if (registroSelecionado.FlagDestino == "Cidade") {
        _origensDestinos.Destino.basicTable.SetarRegistros(_rotaSelecionada.CidadeDestino);
        _origensDestinos.Destino.basicTable.CarregarGrid(_rotaSelecionada.CidadeDestino);
    }
    else if (registroSelecionado.FlagDestino == "Cliente") {
        _origensDestinos.ClienteDestino.basicTable.SetarRegistros(_rotaSelecionada.ClienteDestino);
        _origensDestinos.ClienteDestino.basicTable.CarregarGrid(_rotaSelecionada.ClienteDestino);
    }
    else if (registroSelecionado.FlagDestino == "Estado") {
        _origensDestinos.EstadoDestino.basicTable.SetarRegistros(_rotaSelecionada.EstadoDestino);
        _origensDestinos.EstadoDestino.basicTable.CarregarGrid(_rotaSelecionada.EstadoDestino);
    }
    else if (registroSelecionado.FlagDestino == "Regiao") {
        _origensDestinos.RegiaoDestino.basicTable.SetarRegistros(_rotaSelecionada.RegiaoDestino);
        _origensDestinos.RegiaoDestino.basicTable.CarregarGrid(_rotaSelecionada.RegiaoDestino);
    }
    else if (registroSelecionado.FlagDestino == "Rota") {
        _origensDestinos.RotaDestino.basicTable.SetarRegistros(_rotaSelecionada.RotaDestino);
        _origensDestinos.RotaDestino.basicTable.CarregarGrid(_rotaSelecionada.RotaDestino);
    }
    else if (registroSelecionado.FlagDestino == "Pais") {
        _origensDestinos.PaisDestino.basicTable.SetarRegistros(_rotaSelecionada.PaisDestino);
        _origensDestinos.PaisDestino.basicTable.CarregarGrid(_rotaSelecionada.PaisDestino);
    }
    else if (registroSelecionado.FlagDestino == "CEP") {
        _origensDestinos.CEPsDestino.val(_rotaSelecionada.CEPDestino);
        RecarregarGridCEPDestino();
    }

    ValidarDestinosDisponiveis();
    ValidarOrigensDisponiveis();

    trocarControles(true);

    adicionarOfertaModalClick();
}

//Funções privadas
function LimparOrigemDestino() {
    ZerarTabelasDestino();
    ZerarTabelasOrigem();
    ValidarOrigensDisponiveis(true);
    ValidarDestinosDisponiveis(true);
}

function trocarControles(isEdicao) {
    if (isEdicao) {
        _adicionarRotaBotoes.Adicionar.visible(false);
        _adicionarRotaBotoes.Atualizar.visible(true);
        _adicionarRotaBotoes.Excluir.visible(true);
    }
    else {
        _adicionarRotaBotoes.Adicionar.visible(true);
        _adicionarRotaBotoes.Atualizar.visible(false);
        _adicionarRotaBotoes.Excluir.visible(false);
    }
}

function checarOrigemDestino() {
    if (
        _origensDestinos.Origem.basicTable.BuscarRegistros().length > 0 ||
        _origensDestinos.ClienteOrigem.basicTable.BuscarRegistros().length > 0 ||
        _origensDestinos.EstadoOrigem.basicTable.BuscarRegistros().length > 0 ||
        _origensDestinos.RegiaoOrigem.basicTable.BuscarRegistros().length > 0 ||
        _origensDestinos.RotaOrigem.basicTable.BuscarRegistros().length > 0 ||
        _origensDestinos.PaisOrigem.basicTable.BuscarRegistros().length > 0 ||
        _origensDestinos.CEPsOrigem.val().length > 0
    ) {
        if (
            _origensDestinos.Destino.basicTable.BuscarRegistros().length > 0 ||
            _origensDestinos.ClienteDestino.basicTable.BuscarRegistros().length > 0 ||
            _origensDestinos.EstadoDestino.basicTable.BuscarRegistros().length > 0 ||
            _origensDestinos.RegiaoDestino.basicTable.BuscarRegistros().length > 0 ||
            _origensDestinos.RotaDestino.basicTable.BuscarRegistros().length > 0 ||
            _origensDestinos.PaisDestino.basicTable.BuscarRegistros().length > 0 ||
            _origensDestinos.CEPsDestino.val().length > 0
        ) {
            return true;
        }
    }

    return false;

}

function obterOfertaSalvar() {
    const biddingOferta = RetornarObjetoPesquisa(_ofertas);
    preencherRotas(biddingOferta);

    return biddingOferta;
}

function preencherOferta(dadosBidding) {
    dadosBidding["OfertaCampos"] = JSON.stringify(RetornarObjetoPesquisa(_ofertas));
    dadosBidding["Oferta"] = ObterListaRotasSalvar();
}

function ObterListaRotasSalvar() {
    const listaRotas = obterListaRotas();
    const listaRotasSalvar = new Array();

    for (let i = 0; i < listaRotas.length; i++) {
        const rota = listaRotas[i];

        listaRotasSalvar.push({
            Codigo: rota.Codigo,
            Descricao: rota.Descricao,
            FlagOrigem: rota.FlagOrigem,
            FlagDestino: rota.FlagDestino,
            CEPOrigem: rota.CEPOrigem,
            CEPDestino: rota.CEPDestino,
            CidadeDestino: rota.CidadeDestino,
            CidadeOrigem: rota.CidadeOrigem,
            ClienteOrigem: rota.ClienteOrigem,
            ClienteDestino: rota.ClienteDestino,
            EstadoOrigem: rota.EstadoOrigem,
            EstadoDestino: rota.EstadoDestino,
            QuilometragemMedia: rota.QuilometragemMedia,
            Frequencia: rota.Frequencia,
            Peso: rota.Peso,
            AdicionalAPartirDaEntregaNumero: rota.AdicionalAPartirDaEntregaNumero,
            NumeroEntrega: rota.NumeroEntrega,
            Observacao: rota.Observacao,
            PaisOrigem: rota.PaisOrigem,
            PaisDestino: rota.PaisDestino,
            RegiaoOrigem: rota.RegiaoOrigem,
            RegiaoDestino: rota.RegiaoDestino,
            RotaOrigem: rota.RotaOrigem,
            RotaDestino: rota.RotaDestino,
            ValorCargaMes: rota.ValorCargaMes,
            Volume: rota.Volume,
            ModeloVeicular: rota.ModeloVeicular,
            FiliaisParticipante: rota.FiliaisParticipante,
            TipoCarga: rota.TipoCarga,
            Baseline: rota.Baseline,
            Tomador: rota.Tomador,
            GrupoModeloVeicular: rota.GrupoModeloVeicular,
            ModeloCarroceria: rota.ModeloCarroceria,
            FrequenciaMensalComAjudante: rota.FrequenciaMensalComAjudante,
            QuantidadeAjudantePorVeiculo: rota.QuantidadeAjudantePorVeiculo,
            MediaEntregasFracionada: rota.MediaEntregasFracionada,
            MaximaEntregasFacionada: rota.MaximaEntregasFacionada,
            Inconterm: rota.Inconterm,
            QuantidadeViagensPorAno: rota.QuantidadeViagensPorAno,
            VolumeTonAno: rota.VolumeTonAno,
            VolumeTonViagem: rota.VolumeTonViagem,
            ValorMedioNFe: rota.ValorMedioNFe,
            TempoColeta: rota.TempoColeta,
            TempoDescarga: rota.TempoDescarga,
            Compressor: rota.Compressor,
        });
    }

    return JSON.stringify(listaRotasSalvar);
}

function obterListaRotas() {
    return _gridRotas.Rotas.val().slice();
}

function SetValorDefaultRegistroGridRotas(registro) {

    let headers = _gridRotasBasicTable.ObterHeader();

    for (let i = 0; i < Object.keys(headers).length; i++) {
        let header = headers[i];

        if (registro[header.data] == undefined || registro[header.data] == null) {
            registro[header.data] = new Array();
        }
    }
}

//Funções publicas
function salvarOfertas(codigo) {
    executarReST("BiddingOferta/Adicionar", { CodigoConvite: codigo, Oferta: obterOfertaSalvar() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Oferta adicionada com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function salvarRotas(codigo) {
    _gridRotasBasicTable.BuscarRegistros().forEach(addRota);
    executarReST("BiddingOfertaRota/Adicionar", { CodigoOferta: codigo, Rotas: JSON.stringify(_gridRotasBasicTable.BuscarRegistros()) }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Rota adicionadas com sucesso");
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

// Cálculo callback Origem/Destino por Cidade.
function CalculoQuilometragemMediaCidadeOrigem(codigoOrigem) {
    codigosOrigem = codigoOrigem[0];

    if (codigosDestino != undefined && codigosDestino != 0)
        CalculoKMMediaRotaCidade(codigosDestino, codigosOrigem);
}

function CalculoQuilometragemMediaCidadeDestino(codigoDestino) {
    codigosDestino = codigoDestino[0];

    if (codigosOrigem != undefined && codigosOrigem != 0)
        CalculoKMMediaRotaCidade(codigosDestino, codigosOrigem);
}

function CalculoKMMediaRotaCidade(codigosDestino, codigosOrigem) {
    executarReST("BiddingConvite/CalculoKMMediaRotaCidade", { CodigoDestino: codigosDestino, CodigoOrigem: codigosOrigem }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data && retorno.Data.QuilometragemMedia) {
                _camposRota.QuilometragemMedia.val(retorno.Data.QuilometragemMedia);
                _camposRota.QuilometragemMedia.required(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

// Cálculo callback Origem/Destino por Clientes.
function CalculoQuilometragemMediaClienteOrigem(codigoOrigem) {
    codigosOrigem = codigoOrigem[0];

    if (codigosDestino != undefined && codigosDestino != 0)
        CalculoKMMediaRotaCliente(codigosDestino, codigosOrigem);
}

function CalculoQuilometragemMediaClienteDestino(codigoDestino) {
    codigosDestino = codigoDestino[0];

    if (codigosOrigem != undefined && codigosOrigem != 0)
        CalculoKMMediaRotaCliente(codigosDestino, codigosOrigem);
}

function CalculoKMMediaRotaCliente(codigosDestino, codigosOrigem) {
    executarReST("BiddingConvite/CalculoKMMediaRotaCliente", { CodigoDestino: codigosDestino, CodigoOrigem: codigosOrigem }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data && retorno.Data.QuilometragemMedia) {
                _camposRota.QuilometragemMedia.val(retorno.Data.QuilometragemMedia);
                _camposRota.QuilometragemMedia.required(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

// Cálculo callback Origem/Destino por Rota.
function CalculoQuilometragemMediaRotaOrigem(codigoOrigem) {
    codigosOrigem = codigoOrigem[0];

    if (codigosDestino != undefined && codigosDestino != 0)
        CalculoKMMediaRota(codigosDestino, codigosOrigem);
}

function CalculoQuilometragemMediaRotaDestino(codigoDestino) {
    codigosDestino = codigoDestino[0];

    if (codigosOrigem != undefined && codigosOrigem != 0)
        CalculoKMMediaRota(codigosDestino, codigosOrigem);
}

function CalculoKMMediaRota(codigosDestino, codigosOrigem) {
    executarReST("BiddingConvite/CalculoKMMediaRota", { CodigoDestino: codigosDestino, CodigoOrigem: codigosOrigem }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data && retorno.Data.QuilometragemMedia) {
                _camposRota.QuilometragemMedia.val(retorno.Data.QuilometragemMedia);
                _camposRota.QuilometragemMedia.required(true);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function HabilitarCamposModal() {
    let tipoLance = _ofertas.TipoLance.val();

    _camposRota.NumeroEntrega.enable(true);
    _camposRota.ValorCargaMes.enable(true);
    _camposRota.FrequenciaMensalComAjudante.enable(true);
    _camposRota.MediaEntregasFracionada.enable(true);
    _camposRota.QuantidadeAjudantePorVeiculo.enable(true);
    _camposRota.Frequencia.enable(true);
    _camposRota.Peso.enable(true);
    _camposRota.Volume.enable(true);
    _camposRota.MaximaEntregasFacionada.enable(true);
    _camposRota.AdicionalAPartirDaEntregaNumero.enable(true);
    _camposRota.QuantidadeViagensPorAno.enable(true);
    _camposRota.VolumeTonAno.enable(true);
    _camposRota.VolumeTonViagem.enable(true);
    _camposRota.TempoColeta.enable(true);
    _camposRota.TempoDescarga.enable(true);
    _camposRota.Compressor.enable(true);
    _camposRota.ModeloCarroceria.enable(true);

    if (tipoLance == EnumTipoLanceBidding.LancePorPeso && tipoLance == EnumTipoLanceBidding.LancePorCapacidade && tipoLance == EnumTipoLanceBidding.permiteLancePorFreteViagem && tipoLance == EnumTipoLanceBidding.LancePorViagemEntregaAjudante) {
        _camposRota.Peso.enable(false);
        _camposRota.Volume.enable(false);
        _camposRota.MaximaEntregasFacionada.enable(false);
        _camposRota.AdicionalAPartirDaEntregaNumero.enable(false);
        return;
    }

    if (tipoLance != EnumTipoLanceBidding.LancePorPeso && tipoLance != EnumTipoLanceBidding.LancePorCapacidade && tipoLance != EnumTipoLanceBidding.LancePorFreteViagem && tipoLance != EnumTipoLanceBidding.LancePorViagemEntregaAjudante) {
        return;
    }

    if (tipoLance == EnumTipoLanceBidding.LancePorPeso || tipoLance == EnumTipoLanceBidding.LancePorCapacidade || tipoLance == EnumTipoLanceBidding.LancePorFreteViagem || tipoLance == EnumTipoLanceBidding.LancePorViagemEntregaAjudante) {
        _camposRota.Peso.enable(false);
        _camposRota.Volume.enable(false);
        _camposRota.MaximaEntregasFacionada.enable(false);
        _camposRota.AdicionalAPartirDaEntregaNumero.enable(false);
    }

    _camposRota.QuantidadeViagensPorAno.enable(!(tipoLance == EnumTipoLanceBidding.LancePorPeso || tipoLance == EnumTipoLanceBidding.LancePorCapacidade));
    _camposRota.Compressor.enable(tipoLance == EnumTipoLanceBidding.LancePorPeso);
    _camposRota.Compressor.val(tipoLance == EnumTipoLanceBidding.LancePorPeso ? 1 : 2);
    _camposRota.VolumeTonAno.enable(tipoLance != EnumTipoLanceBidding.LancePorFreteViagem)

    if (tipoLance == EnumTipoLanceBidding.LancePorViagemEntregaAjudante) {
        _camposRota.VolumeTonViagem.enable(false);
        _camposRota.TempoColeta.enable(false);
        _camposRota.TempoDescarga.enable(false);
    }
    else {
        _camposRota.NumeroEntrega.enable(false);
        _camposRota.ValorCargaMes.enable(false);
        _camposRota.FrequenciaMensalComAjudante.enable(false);
        _camposRota.MediaEntregasFracionada.enable(false);
        _camposRota.QuantidadeAjudantePorVeiculo.enable(false);
        _camposRota.Frequencia.enable(false);
    }
}
