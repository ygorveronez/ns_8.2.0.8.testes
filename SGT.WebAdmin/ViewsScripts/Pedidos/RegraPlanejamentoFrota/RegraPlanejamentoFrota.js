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
/// <reference path="../../Consultas/Regiao.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/Estado.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/TabelaFreteVigencia.js" />
/// <reference path="../../Consultas/TipoOperacao.js" />
/// <reference path="../../Consultas/RotaFrete.js" />
/// <reference path="../../Consultas/CanalEntrega.js" />
/// <reference path="../../Enumeradores/EnumTipoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoIntegracao.js" />
/// <reference path="../../Enumeradores/EnumTipoPagamentoEmissao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoAlteracaoTabelaFrete.js" />
/// <reference path="../../Enumeradores/EnumTipoEmissaoCTeDocumentos.js" />
/// <reference path="../../Enumeradores/EnumTipoCalculoTabelaFrete.js" />
/// <reference path="Origem.js" />
/// <reference path="Destino.js" />
/// <reference path="../../Enumeradores/EnumTipoFrota.js" />
//*******MAPEAMENTO KNOUCKOUT*******

var _gridTabelaFreteCliente;
var _regraPlanejamentoFrota;
var _pesquisaRegraPlanejamentoFrota;
var _CRUDRegraPlanejamentoFrota;
var _tipoIntegracao = [];

var PesquisaRegraPlanejamentoFrota = function () {
    this.NumeroSequencial = PropertyEntity({ text: "Número: ", maxlength: 50});
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.VigenciaInicial = PropertyEntity({ text: "Vigência Inicial:", getType: typesKnockout.date });
    this.VigenciaFinal = PropertyEntity({ text: "Vigência Final:", dateRangeInit: this.VigenciaInicial, getType: typesKnockout.date });

    this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupos de Pessoas: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CidadeOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade Origem: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.EstadoOrigem = PropertyEntity({ options: EnumEstado.obterOpcoesPesquisaComExterior(), text: "Estado de Origem: ", idBtnSearch: guid() });
    this.EstadoDestino = PropertyEntity({ options: EnumEstado.obterOpcoesPesquisaComExterior(), text: "Estado de Destino: ", idBtnSearch: guid() });

    this.CidadeDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cidade de Destino: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.TipoOpercao = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipos de Operação: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoCarga = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Tipos de Carga: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CentroResultdo = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Centros de Resultado: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ModeloVeicular = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Modelo Veicular: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.NivelCooperado = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: "Nível de Cooperado: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.VigenciaInicial.dateRangeLimit = this.VigenciaFinal;
    this.VigenciaFinal.dateRangeInit = this.VigenciaInicial;

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFreteCliente.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
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

var RegraPlanejamentoFrota = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroSequencial = PropertyEntity({ text: "Número Sequencial:", val: ko.observable(""), enable: false });
    this.Descricao = PropertyEntity({ text: "Descrição:", val: ko.observable("") });
    this.VigenciaInicial = PropertyEntity({ text: "Vigência Inicial:", getType: typesKnockout.date });
    this.VigenciaFinal = PropertyEntity({ text: "Vigência Final:", dateRangeInit: this.VigenciaInicial, getType: typesKnockout.date });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    //this.GrupoPessoas = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Grupo de Pessoas: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.ValorDeDaMercadoria = PropertyEntity({ text: "Valor De da Mercadoria:", getType: typesKnockout.decimal });
    this.ValorAteDaMercadoria = PropertyEntity({ text: "Valor Até da Mercadoria:", getType: typesKnockout.decimal });
    this.TipoFrota = PropertyEntity({ val: ko.observable(EnumTipoFrota.NaoDefinido), options: EnumTipoFrota.obterOpcoes(), def: EnumTipoFrota.NaoDefinido, text: "Tipo de Frota: " });

    

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
    this.CEPDestinoDiasUteis = PropertyEntity({ text: "Prazo (Dias Úteis):", val: ko.observable(""), def: "", getType: typesKnockout.int, maxlength: 11, visible: ko.observable(false) });
    this.AdicionarCEPDestino = PropertyEntity({ eventClick: AdicionarCEPDestinoClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true), enable: ko.observable(true) });

    //#endregion Destinos

    this.VigenciaFinal.dateRangeInit = this.VigenciaInicial;

    //Card Grupos de Pessoas
    this.GruposPessoas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

7    //Card Tipo de Operações
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Tipos Cargas
    this.TiposCargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Centros de Resultado
    this.CentrosResultado = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card modelos Veiculares da Carga
    this.ModelosVeicularesCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card niveis cooperados
    this.NiveisCooperados = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Modelos Veiculares da Carga Tração
    this.ModelosVeicularesTracaoCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Modelos Veiculares da Carga Reboque
    this.ModelosVeicularesReboqueCarga = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Tecnologia Rastreador
    this.TecnologiaRastreadores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Licença
    this.Licencas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Liberações GR
    this.LiberacoesGR = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    //Card Liberações GR de Veículo
    this.LiberacoesGRVeiculo = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDRegraPlanejamentoFrota = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadRegraPlanejamentoFrota() {
    _regraPlanejamentoFrota = new RegraPlanejamentoFrota();
    KoBindings(_regraPlanejamentoFrota, "knockoutCadastroRegraPlanejamentoFrota");

    _CRUDRegraPlanejamentoFrota = new CRUDRegraPlanejamentoFrota();
    KoBindings(_CRUDRegraPlanejamentoFrota, "knockoutCRUDCadastroTabelaFreteCliente");

    _pesquisaRegraPlanejamentoFrota = new PesquisaRegraPlanejamentoFrota();
    KoBindings(_pesquisaRegraPlanejamentoFrota, "knockoutPesquisaTabelaFreteCliente", false, _pesquisaRegraPlanejamentoFrota.Pesquisar.id);


    new BuscarGruposPessoas(_pesquisaRegraPlanejamentoFrota.GrupoPessoas, null, null);
    new BuscarTiposOperacao(_pesquisaRegraPlanejamentoFrota.TipoOpercao, null, null, null);
    new BuscarTiposdeCarga(_pesquisaRegraPlanejamentoFrota.TipoCarga, null, null);
    new BuscarCentroResultado(_pesquisaRegraPlanejamentoFrota.CentroResultdo, null, null, null, null, null);
    new BuscarModelosVeicularesCarga(_pesquisaRegraPlanejamentoFrota.ModeloVeicular, null, null, null, null, null);
    new BuscarTipoTerceiro(_pesquisaRegraPlanejamentoFrota.NivelCooperado, null);
    new BuscarLocalidades(_pesquisaRegraPlanejamentoFrota.CidadeOrigem, null, null, null);
    new BuscarLocalidades(_pesquisaRegraPlanejamentoFrota.CidadeDestino, null, null, null);
    HeaderAuditoria("RegraPlanejamentoFrota", _regraPlanejamentoFrota);

    buscarRegraPlanejamentoFrota();

    LoadOrigens();
    LoadDestinos();
    LoadTiposCargasRegraPlanejamentoFrota();
    LoadGrupoPessoaRegraPlanejamentoFrota();
    LoadTipoOperacaoRegraPlanejamentoFrota();
    LoadCentroResultadoRegraPlanejamentoFrota();
    LoadModeloVeicularCargaRegraPlanejamentoFrota();
    LoadNivelCooperadoRegraPlanejamentoFrota();
    loadRegraRegraPlanejamentoFrota();
    LoadModeloVeicularTracaoCargaRegraPlanejamentoFrota();
    LoadModeloVeicularReboqueCargaRegraPlanejamentoFrota();
    LoadTecnologiaRastreadorRegraPlanejamentoFrota();
    LoadLicencaRegraPlanejamentoFrota();
    LoadLiberacaoGRRegraPlanejamentoFrota();
    LoadLiberacaoGRVeiculoRegraPlanejamentoFrota();
}

function PreencherListasDeSelecaoRegraPlanejamentoFrota() {
    _regraPlanejamentoFrota.Origens.val(JSON.stringify(_regraPlanejamentoFrota.Origem.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.ClientesOrigem.val(JSON.stringify(_regraPlanejamentoFrota.ClienteOrigem.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.EstadosOrigem.val(JSON.stringify(_regraPlanejamentoFrota.EstadoOrigem.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.RegioesOrigem.val(JSON.stringify(_regraPlanejamentoFrota.RegiaoOrigem.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.RotasOrigem.val(JSON.stringify(_regraPlanejamentoFrota.RotaOrigem.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.PaisesOrigem.val(JSON.stringify(_regraPlanejamentoFrota.PaisOrigem.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.ListaCEPsOrigem.val(JSON.stringify(_regraPlanejamentoFrota.CEPsOrigem.val()));

    _regraPlanejamentoFrota.Destinos.val(JSON.stringify(_regraPlanejamentoFrota.Destino.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.ClientesDestino.val(JSON.stringify(_regraPlanejamentoFrota.ClienteDestino.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.EstadosDestino.val(JSON.stringify(_regraPlanejamentoFrota.EstadoDestino.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.RegioesDestino.val(JSON.stringify(_regraPlanejamentoFrota.RegiaoDestino.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.RotasDestino.val(JSON.stringify(_regraPlanejamentoFrota.RotaDestino.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.PaisesDestino.val(JSON.stringify(_regraPlanejamentoFrota.PaisDestino.basicTable.BuscarRegistros()));
    _regraPlanejamentoFrota.ListaCEPsDestino.val(JSON.stringify(_regraPlanejamentoFrota.CEPsDestino.val()));

    _regraPlanejamentoFrota.CentrosResultado.val(JSON.stringify(_gridCentroResultadoRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.ModelosVeicularesCarga.val(JSON.stringify(_gridModeloVeicularCargaRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.NiveisCooperados.val(JSON.stringify(_gridNivelCooperadoRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.GruposPessoas.val(JSON.stringify(_gridGrupoPessoaRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.Licencas.val(JSON.stringify(_gridLicencaRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.LiberacoesGR.val(JSON.stringify(_gridLiberacaoGRRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.LiberacoesGRVeiculo.val(JSON.stringify(_gridLiberacaoGRVeiculoRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.ModelosVeicularesReboqueCarga.val(JSON.stringify(_gridModeloVeicularCargaReboqueRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.ModelosVeicularesTracaoCarga.val(JSON.stringify(_gridModeloVeicularCargaTracaoRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.TecnologiaRastreadores.val(JSON.stringify(_gridTecnologiaRastreadorRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.TiposOperacao.val(JSON.stringify(_gridTipoOperacaoRegraPlanejamentoFrota.BuscarRegistros()));
    _regraPlanejamentoFrota.TiposCargas.val(JSON.stringify(_gridTiposCargasRegraPlanejamentoFrota.BuscarRegistros()));
}

function adicionarClick(e, sender) {
    if (ValidarRegraPlanejamentoFrota() === true) {
        PreencherListasDeSelecaoRegraPlanejamentoFrota();
        
        var dados = $.extend({}, _regraPlanejamentoFrota, _regraRegraPlanejamentoFrota);

        Salvar(dados, "RegraPlanejamentoFrota/Adicionar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                    _gridTabelaFreteCliente.CarregarGrid();
                    limparCamposRegraPlanejamentoFrota();
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
    if (ValidarRegraPlanejamentoFrota() === true) {
        PreencherListasDeSelecaoRegraPlanejamentoFrota();

        var dados = $.extend({}, _regraPlanejamentoFrota, _regraRegraPlanejamentoFrota);
        console.log(dados);
        Salvar(dados, "RegraPlanejamentoFrota/Atualizar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridTabelaFreteCliente.CarregarGrid();
                    limparCamposRegraPlanejamentoFrota();
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
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Cadastro?", function () {
        ExcluirPorCodigo(_regraPlanejamentoFrota, "RegraPlanejamentoFrota/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTabelaFreteCliente.CarregarGrid();
                    limparCamposRegraPlanejamentoFrota();
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
    limparCamposRegraPlanejamentoFrota();
}

function buscarRegraPlanejamentoFrota() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), evento: "onclick", metodo: function (regraGrid) { editarRegraPlanejamentoFrota(regraGrid, false); }, tamanho: "5", icone: "" };
    var duplicar = { descricao: Localization.Resources.Gerais.Geral.Duplicar, id: guid(), evento: "onclick", metodo: function (regraGrid) { editarRegraPlanejamentoFrota(regraGrid, true); }, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, duplicar] };

    _gridTabelaFreteCliente = new GridView(_pesquisaRegraPlanejamentoFrota.Pesquisar.idGrid, "RegraPlanejamentoFrota/Pesquisa", _pesquisaRegraPlanejamentoFrota, menuOpcoes, null, null);
    _gridTabelaFreteCliente.CarregarGrid();
}

function editarRegraPlanejamentoFrota(tabelaFreteClienteGrid, duplicar) {
    limparCamposRegraPlanejamentoFrota();

    if (!duplicar)
         _regraPlanejamentoFrota.Codigo.val(tabelaFreteClienteGrid.Codigo);

    executarReST("RegraPlanejamentoFrota/BuscarPorCodigo", { Codigo: tabelaFreteClienteGrid.Codigo, Duplicar: duplicar }, function (e) {
        if (e.Success) {
            PreencherObjetoKnout(_regraPlanejamentoFrota, e);
            PreencherObjetoKnout(_regraRegraPlanejamentoFrota, e);

            _pesquisaRegraPlanejamentoFrota.ExibirFiltros.visibleFade(false);

            _regraPlanejamentoFrota.Origem.basicTable.CarregarGrid(_regraPlanejamentoFrota.Origens.val());
            _regraPlanejamentoFrota.ClienteOrigem.basicTable.CarregarGrid(_regraPlanejamentoFrota.ClientesOrigem.val());
            _regraPlanejamentoFrota.EstadoOrigem.basicTable.CarregarGrid(_regraPlanejamentoFrota.EstadosOrigem.val());
            _regraPlanejamentoFrota.RegiaoOrigem.basicTable.CarregarGrid(_regraPlanejamentoFrota.RegioesOrigem.val());
            _regraPlanejamentoFrota.RotaOrigem.basicTable.CarregarGrid(_regraPlanejamentoFrota.RotasOrigem.val());
            _regraPlanejamentoFrota.PaisOrigem.basicTable.CarregarGrid(_regraPlanejamentoFrota.PaisesOrigem.val());
            RecarregarGridCEPOrigem();
            ValidarOrigensDisponiveis();

            _regraPlanejamentoFrota.Destino.basicTable.CarregarGrid(_regraPlanejamentoFrota.Destinos.val());
            _regraPlanejamentoFrota.ClienteDestino.basicTable.CarregarGrid(_regraPlanejamentoFrota.ClientesDestino.val());
            _regraPlanejamentoFrota.EstadoDestino.basicTable.CarregarGrid(_regraPlanejamentoFrota.EstadosDestino.val());
            _regraPlanejamentoFrota.RegiaoDestino.basicTable.CarregarGrid(_regraPlanejamentoFrota.RegioesDestino.val());
            _regraPlanejamentoFrota.RotaDestino.basicTable.CarregarGrid(_regraPlanejamentoFrota.RotasDestino.val());
            _regraPlanejamentoFrota.PaisDestino.basicTable.CarregarGrid(_regraPlanejamentoFrota.PaisesDestino.val());
            RecarregarGridCEPDestino();
            ValidarDestinosDisponiveis();

            RecarregarGridCentroResultadoRegraPlanejamentoFrota();
            RecarregarGridModeloVeicularCargaRegraPlanejamentoFrota();
            RecarregarGridNivelCooperadoRegraPlanejamentoFrota();
            RecarregarGridGrupoPessoaRegraPlanejamentoFrota();
            RecarregarGridLicencaRegraPlanejamentoFrota();
            RecarregarGridLiberacaoGRRegraPlanejamentoFrota();
            RecarregarGridLiberacaoGRVeiculoRegraPlanejamentoFrota();
            RecarregarGridModeloVeicularCargaReboqueRegraPlanejamentoFrota();
            RecarregarGridModeloVeicularCargaTracaoRegraPlanejamentoFrota();
            RecarregarGridTecnologiaRastreadorRegraPlanejamentoFrota();
            RecarregarGridTipoOperacaoRegraPlanejamentoFrota();
            RecarregarGridTiposCargasRegraPlanejamentoFrota();

            _CRUDRegraPlanejamentoFrota.Excluir.visible(!duplicar);
            _CRUDRegraPlanejamentoFrota.Atualizar.visible(!duplicar);
            _CRUDRegraPlanejamentoFrota.Adicionar.visible(duplicar);
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", e.Msg);
        }
    });
}

function limparCamposRegraPlanejamentoFrota() {
    _CRUDRegraPlanejamentoFrota.Atualizar.visible(false);
    _CRUDRegraPlanejamentoFrota.Cancelar.visible(true);
    _CRUDRegraPlanejamentoFrota.Excluir.visible(false);
    _CRUDRegraPlanejamentoFrota.Adicionar.visible(true);

    _regraPlanejamentoFrota.Origem.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.ClienteOrigem.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.EstadoOrigem.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.RegiaoOrigem.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.RotaOrigem.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.PaisOrigem.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.CEPsOrigem.val([]);

    _regraPlanejamentoFrota.Destino.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.ClienteDestino.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.EstadoDestino.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.RegiaoDestino.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.RotaDestino.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.PaisDestino.basicTable.CarregarGrid(new Array());
    _regraPlanejamentoFrota.CEPsDestino.val([]);

    LimparCampos(_regraRegraPlanejamentoFrota);
    LimparCampos(_regraPlanejamentoFrota);

    RecarregarGridCEPDestino();
    RecarregarGridCEPOrigem();

    LimparCamposCentroResultadoRegraPlanejamentoFrota();
    LimparCamposModeloVeicularCargaRegraPlanejamentoFrota();
    LimparCamposNivelCooperadoRegraPlanejamentoFrota();
    LimparCamposGrupoPessoaRegraPlanejamentoFrota();
    LimparCamposLicencaRegraPlanejamentoFrota();
    LimparCamposLiberacaoGRRegraPlanejamentoFrota();
    LimparCamposLiberacaoGRVeiculoRegraPlanejamentoFrota();
    LimparCamposModeloVeicularCargaReboqueRegraPlanejamentoFrota();
    LimparCamposModeloVeicularCargaTracaoRegraPlanejamentoFrota();
    LimparCamposTecnologiaRastreadorRegraPlanejamentoFrota();
    LimparCamposTipoOperacaoRegraPlanejamentoFrota();
    LimparCamposTiposCargasRegraPlanejamentoFrota();

    ValidarOrigensDisponiveis();
    ValidarDestinosDisponiveis();

    $('.nav-tabs').each(function () {
        $(this).find('li:eq(0) a').tab('show');
    });
}

function ValidarRegraPlanejamentoFrota() {
    var valido = true;
    var mensagem = "";

    var regiao = "região";
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
        regiao = "rota";

    //if (_regraPlanejamentoFrota.Destino.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.ClienteDestino.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.RegiaoDestino.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.RotaDestino.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.PaisDestino.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.EstadoDestino.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.CEPsDestino.val().length <= 0) {
    //    valido = false;
    //    mensagem += "Selecione ao menos um destino (cep/cliente/cidade/estado/" + regiao + "/país).<br/>";
    //}

    //if (_regraPlanejamentoFrota.Origem.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.ClienteOrigem.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.RegiaoOrigem.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.RotaOrigem.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.EstadoOrigem.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.PaisOrigem.basicTable.BuscarRegistros().length <= 0 &&
    //    _regraPlanejamentoFrota.CEPsOrigem.val().length <= 0) {
    //    valido = false;
    //    mensagem += "Selecione ao menos uma origem (cep/cliente/cidade/estado/" + regiao + "/país).<br/>"
    //}

    if (!valido)
        exibirMensagem(tipoMensagem.atencao, "Atenção!", mensagem);

    return valido;
}