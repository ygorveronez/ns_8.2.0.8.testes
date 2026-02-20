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


//*******MAPEAMENTO KNOUCKOUT*******

var _ocorrenciaParametroOcorrencia;
var _observacaoCTe;
var _pesquisaOcorrenciaParametroOcorrencia;
var _CRUDOcorrenciaParametroOcorrencia;
var _gridOcorrenciaParametroOcorrencia;

var OcorrenciaParametroOcorrencia = function () {
    // Codigo
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    // Propriedades
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.string, val: ko.observable(""), enable: false });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Carga: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.DataParametro = PropertyEntity({ text: "*Data do Parâmetro: ", required: true, getType: typesKnockout.date, enable: ko.observable(true) });
    this.TipoOcorrencia = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Ocorrência: ", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.ObservacaoCTe = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });

    //this.DataParametro.val.subscribe(VerificarDadosInformados);
    this.TipoOcorrencia.codEntity.subscribe(VerificarDadosInformados);
    this.Carga.codEntity.subscribe(VerificarDadosInformados);
};

var ObservacaoCTe = function () {
    this.Observacao = _ocorrenciaParametroOcorrencia.ObservacaoCTe;
    this.Fechar = PropertyEntity({ eventClick: FecharModalObservacaoCTe, type: types.event, text: "Fechar", enable: ko.observable(true) });
};

var CRUDOcorrenciaParametroOcorrencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Gerar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
};

var PesquisaOcorrenciaParametroOcorrencia = function () {
    this.NumeroInicial = PropertyEntity({ text: "Número Inicial:", getType: typesKnockout.int, val: ko.observable(""), enable: false });
    this.NumeroFinal = PropertyEntity({ text: "Número Final:", getType: typesKnockout.int, val: ko.observable(""), enable: false });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOcorrenciaParametroOcorrencia.CarregarGrid();
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
};

//*******EVENTOS*******
function LoadOcorrenciaParametroOcorrencia() {
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaOcorrenciaParametroOcorrencia = new PesquisaOcorrenciaParametroOcorrencia();
    KoBindings(_pesquisaOcorrenciaParametroOcorrencia, "knockoutPesquisaOcorrenciaParametroOcorrencia", false, _pesquisaOcorrenciaParametroOcorrencia.Pesquisar.id);

    // Instancia Produto
    _ocorrenciaParametroOcorrencia = new OcorrenciaParametroOcorrencia();
    KoBindings(_ocorrenciaParametroOcorrencia, "knockoutOcorrenciaParametroOcorrencia");

    _CRUDOcorrenciaParametroOcorrencia = new CRUDOcorrenciaParametroOcorrencia();
    KoBindings(_CRUDOcorrenciaParametroOcorrencia, "knockoutCRUDOcorrenciaParametroOcorrencia");

    _observacaoCTe = new ObservacaoCTe();
    KoBindings(_observacaoCTe, "knockoutObservacaoCTe");

    new BuscarTipoOcorrencia(_ocorrenciaParametroOcorrencia.TipoOcorrencia, RetornoTipoOcorrencia);
    new BuscarCargas(_ocorrenciaParametroOcorrencia.Carga, null, null, null, null, null, null, _ocorrenciaParametroOcorrencia.TipoOcorrencia, true); //new BuscarCargaPermiteCTeComplementar(_ocorrenciaParametroOcorrencia.Carga);

    LoadParametros();

    HeaderAuditoria("OcorrenciaParametroOcorrencia", _ocorrenciaParametroOcorrencia);

    // Inicia busca
    BuscarOcorrenciaParametroOcorrencia();

    _ocorrenciaParametroOcorrencia.DataParametro.get$().on("blur", VerificarDadosInformados);
}

function RetornoTipoOcorrencia(tipoOcorrencia) {
    _ocorrenciaParametroOcorrencia.TipoOcorrencia.codEntity(tipoOcorrencia.Codigo);
    _ocorrenciaParametroOcorrencia.TipoOcorrencia.val(tipoOcorrencia.Descricao);
    _ocorrenciaParametroOcorrencia.Carga.codEntity(0);
    _ocorrenciaParametroOcorrencia.Carga.val('');
}

function AbrirModalObservacaoCTe() {
    Global.abrirModal('divModalObservacaoCTe');
}

function FecharModalObservacaoCTe() {
    Global.fecharModal('divModalObservacaoCTe');
}

function adicionarClick(e, sender) {
    var data = {
        Carga: _ocorrenciaParametroOcorrencia.Carga.codEntity(),
        TipoOcorrencia: _ocorrenciaParametroOcorrencia.TipoOcorrencia.codEntity(),
        Data: _ocorrenciaParametroOcorrencia.DataParametro.val(),
        Observacao: _ocorrenciaParametroOcorrencia.Observacao.val(),
        ObservacaoCTe: _ocorrenciaParametroOcorrencia.ObservacaoCTe.val(),

        KmInicial: Globalize.parseInt(_parametros.KmInicial.val()),
        KmFinal: Globalize.parseInt(_parametros.KmFinal.val()),
        DataParametro: _parametros.Data.val(),
        HoraInicial: _parametros.HoraInicial.val(),
        HoraFinal: _parametros.HoraFinal.val(),
        TabelaParametros: _parametros.TabelaParametros.val(),
        EnumTipoParametro: _parametros.EnumTipoParametro.val(),
        ValoresAlterados: JSON.stringify(_valoresAlterados)
    };

    executarReST("OcorrenciaParametroOcorrencia/Adicionar", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridOcorrenciaParametroOcorrencia.CarregarGrid();
                limparCamposOcorrenciaParametroOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function cancelarClick(e) {
    limparCamposOcorrenciaParametroOcorrencia();
}

function editarOcorrenciaParametroOcorrenciaClick(itemGrid) {
    // Seta o codigo do Produto
    _ocorrenciaParametroOcorrencia.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_ocorrenciaParametroOcorrencia, "OcorrenciaParametroOcorrencia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaOcorrenciaParametroOcorrencia.ExibirFiltros.visibleFade(false);

                // Alternas os campos de CRUD
                _CRUDOcorrenciaParametroOcorrencia.Adicionar.visible(false);
                _parametros.VisulizarOcorrencias.visible(false);

                ControleCampos(false);

                PreencherObjetoKnout(_parametros, { Data: arg.Data.Parametros });

                switch (arg.Data.Parametros.EnumTipoParametro) {
                    case EnumTipoParametroCalculoOcorrencia.HorasExtra:
                        FluxoOcorrenciaHoraExtra();
                        break;
                    case EnumTipoParametroCalculoOcorrencia.Estadia:
                        FluxoOcorrenciaEstadia();
                        break;
                    case EnumTipoParametroCalculoOcorrencia.Pernoite:
                        FluxoOcorrenciaPernoite();
                        break;
                    case EnumTipoParametroCalculoOcorrencia.Escolta:
                        FluxoOcorrenciaEscolta();
                        break;
                }

                _parametros.Ocorrencia.visible(true);
                GridPreOcorrencias();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarOcorrenciaParametroOcorrencia() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarOcorrenciaParametroOcorrenciaClick, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };


    var configExportacao = {
        url: "OcorrenciaParametroOcorrencia/ExportarPesquisa",
        titulo: "Motivos de Rejeição de Ocorrência"
    };


    // Inicia Grid de busca
    _gridOcorrenciaParametroOcorrencia = new GridViewExportacao(_pesquisaOcorrenciaParametroOcorrencia.Pesquisar.idGrid, "OcorrenciaParametroOcorrencia/Pesquisa", _pesquisaOcorrenciaParametroOcorrencia, menuOpcoes, configExportacao);
    _gridOcorrenciaParametroOcorrencia.CarregarGrid();
}

function limparCamposOcorrenciaParametroOcorrencia() {
    LimparCampos(_ocorrenciaParametroOcorrencia);
    limparCamposParametro();
    _CRUDOcorrenciaParametroOcorrencia.Adicionar.visible(false);
    GridPreOcorrencias();
    ControleCampos(true);
}

function ControleCampos(status) {
    _ocorrenciaParametroOcorrencia.Carga.enable(status);
    _ocorrenciaParametroOcorrencia.DataParametro.enable(status);
    _ocorrenciaParametroOcorrencia.TipoOcorrencia.enable(status);
    _ocorrenciaParametroOcorrencia.Observacao.enable(status);
    _ocorrenciaParametroOcorrencia.ObservacaoCTe.enable(status);

    _parametros.KmInicial.enable(status);
    _parametros.KmFinal.enable(status);
    _parametros.Data.enable(status);
    _parametros.HoraInicial.enable(status);
    _parametros.HoraFinal.enable(status);
}

function VerificarDadosInformados() {
    if (_ocorrenciaParametroOcorrencia.Codigo.val() > 0)
        return;

    if (_ocorrenciaParametroOcorrencia.TipoOcorrencia.codEntity() > 0 && _ocorrenciaParametroOcorrencia.Carga.codEntity() > 0 && _ocorrenciaParametroOcorrencia.DataParametro.val() != "") {
        var data = {
            TipoOcorrencia: _ocorrenciaParametroOcorrencia.TipoOcorrencia.codEntity(),
            Carga: _ocorrenciaParametroOcorrencia.Carga.codEntity(),
            DataParametro: _ocorrenciaParametroOcorrencia.DataParametro.val(),
        };

        executarReST("OcorrenciaParametroOcorrencia/ObterParametros", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _TabelaParametros = arg.Data;
                    _parametros.TabelaParametros.val(_TabelaParametros.Codigo);
                    _parametros.EnumTipoParametro.val(_TabelaParametros.TipoParametroCalculoOcorrencia);

                    _parametros.Ocorrencia.visible(true);
                    _CRUDOcorrenciaParametroOcorrencia.Adicionar.visible(true);

                    switch (_TabelaParametros.TipoParametroCalculoOcorrencia) {
                        case EnumTipoParametroCalculoOcorrencia.HorasExtra:
                            FluxoOcorrenciaHoraExtra();
                            break;
                        case EnumTipoParametroCalculoOcorrencia.Estadia:
                            FluxoOcorrenciaEstadia();
                            break;
                        case EnumTipoParametroCalculoOcorrencia.Pernoite:
                            FluxoOcorrenciaPernoite();
                            break;
                        case EnumTipoParametroCalculoOcorrencia.Escolta:
                            FluxoOcorrenciaEscolta();
                            break;
                    }
                    if (_TabelaParametros.DataSaidaEntrada != "")
                        _parametros.Data.val(_TabelaParametros.DataSaidaEntrada);
                    if (_TabelaParametros.HoraSaida != "")
                        _parametros.HoraInicial.val(_TabelaParametros.HoraSaida);
                    if (_TabelaParametros.HoraEntrada != "")
                        _parametros.HoraFinal.val(_TabelaParametros.HoraEntrada);
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Parâmetros Ocorrência", "Nenhuma tabela de parâmetros encontrada.");
                    SemTabela();
                }
            } else {
                exibirMensagem(tipoMensagem.aviso, "Parâmetros Ocorrência", "Ocorreu uma falha ao buscar Parâmetro para Ocorrência.");
                SemTabela();
            }
        });
    } else {
        SemTabela();
    }
}

function SemTabela() {
    _TabelaParametros = null;
    _parametros.Ocorrencia.visible(false);
    _CRUDOcorrenciaParametroOcorrencia.Adicionar.visible(false);
}