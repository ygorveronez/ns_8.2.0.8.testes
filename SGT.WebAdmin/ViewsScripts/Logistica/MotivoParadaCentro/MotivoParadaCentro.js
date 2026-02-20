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

var _gridMotivoParadaCentro;
var _gridTipoOperacao;
var _motivoParadaCentro;
var _pesquisaMotivoParadaCentro;

var PesquisaMotivoParadaCentro = function () {
    this.Descricao = PropertyEntity();
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true});
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid(), required: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoParadaCentro.CarregarGrid();
        }, type: types.event, idGrid: guid(), visible: ko.observable(true)
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

var MotivoParadaCentro = function () {
    var self = this;
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ required: true, enable: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, enable: ko.observable(true) });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), idBtnSearch: guid(), required: true });
    this.CentroCarregamento.codEntity.subscribe(CentroCarregamentoChanged);
    this.Data = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), enable: ko.observable(true), def: "", required: true });
    this.PeriodoInicio = PropertyEntity({ getType: typesKnockout.time, enable: ko.observable(true), val: ko.observable(""), def: "", required: true });
    this.PeriodoFim = PropertyEntity({ getType: typesKnockout.time, enable: ko.observable(true), val: ko.observable(""), def: "", required: true });
    this.QuantidadeParada = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, enable: ko.observable(true), required: function () { return !self.ContainerTipoOperacao.visible() } });

    this.GridTipoOperacao = PropertyEntity({ type: types.local });
    this.ContainerTipoOperacao = PropertyEntity({ type: types.listEntity, list: new Array(), idGrid: guid(), codEntity: ko.observable(0), visible: ko.observable(false) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), enable: ko.observable(true), idBtnSearch: guid() });
    this.QuantidadeTipoOperacao = PropertyEntity({ val: ko.observable(""), def: "", enable: ko.observable(true), getType: typesKnockout.int });
    this.AdicionarTipoOperacao = PropertyEntity({ eventClick: adicionarTipoOperacaoClick, type: types.event, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadMotivoParadaCentro() {
    _motivoParadaCentro = new MotivoParadaCentro();
    KoBindings(_motivoParadaCentro, "knockoutCadastroMotivoParadaCentro");

    _pesquisaMotivoParadaCentro = new PesquisaMotivoParadaCentro();
    KoBindings(_pesquisaMotivoParadaCentro, "knockoutPesquisaMotivoParadaCentro", false, _pesquisaMotivoParadaCentro.Pesquisar.id);

    HeaderAuditoria("MotivoParadaCentro", _motivoParadaCentro);

    BuscarMotivoParadaCentros();

    new BuscarCentrosCarregamento(_motivoParadaCentro.CentroCarregamento, RetornoSelecaoCentroCarregamento);
    new BuscarTiposOperacao(_motivoParadaCentro.TipoOperacao);
    new BuscarCentrosCarregamento(_pesquisaMotivoParadaCentro.CentroCarregamento);

    loadGridTipoOperacao();
}

function loadGridTipoOperacao() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Logistica.MotivoParadaCentro.Exluir, id: guid(), metodo: excluirTipoOperacaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "TipoOperacao", title: Localization.Resources.Logistica.MotivoParadaCentro.TipoOperacao, width: "30%" },
        { data: "Quantidade", title: Localization.Resources.Logistica.MotivoParadaCentro.Quantidade, width: "15%" }
    ];

    _gridTipoOperacao = new BasicDataTable(_motivoParadaCentro.GridTipoOperacao.id, header, menuOpcoes, { column: 2, dir: orderDir.desc });

    recarregarGridTipoOperacao();
}

function adicionarClick(e, sender) {
    Salvar(e, "MotivoParadaCentro/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridMotivoParadaCentro.CarregarGrid();
                LimparCamposMotivoParadaCentro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "MotivoParadaCentro/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.AtualizadoComSuccesso);
                _gridMotivoParadaCentro.CarregarGrid();
                LimparCamposMotivoParadaCentro();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.RealmenteDesejaExcluirORegistro, function () {
        ExcluirPorCodigo(_motivoParadaCentro, "MotivoParadaCentro/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Successo, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                    _gridMotivoParadaCentro.CarregarGrid();
                    LimparCamposMotivoParadaCentro();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    LimparCamposMotivoParadaCentro();
}

function adicionarTipoOperacaoClick() {
    if (_motivoParadaCentro.TipoOperacao.codEntity() == 0 || _motivoParadaCentro.QuantidadeTipoOperacao.val() == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    var codigoTipoOperacao = _motivoParadaCentro.TipoOperacao.codEntity();
    for (var i = 0; i < _motivoParadaCentro.ContainerTipoOperacao.list.length; i++) {
        if (_motivoParadaCentro.ContainerTipoOperacao.list[i].TipoOperacao.codEntity == codigoTipoOperacao) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Logistica.MotivoParadaCentro.TipoOperacaoJaExiste, Localization.Resources.Logistica.MotivoParadaCentro.JaExisteQuantidadeInformadaTipoOperação + _motivoParadaCentro.TipoOperacao.val() + ".");
            return;
        }
    }

    var data = SalvarListEntity({
        TipoOperacao: _motivoParadaCentro.TipoOperacao,
        QuantidadeTipoOperacao: _motivoParadaCentro.QuantidadeTipoOperacao,
    });

    _motivoParadaCentro.ContainerTipoOperacao.list.push(data);

    recarregarGridTipoOperacao();
    limparCamposTipoOperacao();
}

function excluirTipoOperacaoClick(data) {
    for (var i = 0; i < _motivoParadaCentro.ContainerTipoOperacao.list.length; i++) {
        if (data.Codigo == _motivoParadaCentro.ContainerTipoOperacao.list[i].TipoOperacao.codEntity) {
            _motivoParadaCentro.ContainerTipoOperacao.list.splice(i, 1);
            break;
        }
    }

    recarregarGridTipoOperacao();
}

//*******MÉTODOS*******

function BuscarMotivoParadaCentros() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: EditarMotivoParadaCentro, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMotivoParadaCentro = new GridView(_pesquisaMotivoParadaCentro.Pesquisar.idGrid, "MotivoParadaCentro/Pesquisa", _pesquisaMotivoParadaCentro, menuOpcoes, null);
    _gridMotivoParadaCentro.CarregarGrid();
}

function EditarMotivoParadaCentro(item) {
    LimparCamposMotivoParadaCentro();
    _motivoParadaCentro.Codigo.val(item.Codigo);
    BuscarPorCodigo(_motivoParadaCentro, "MotivoParadaCentro/BuscarPorCodigo", function (arg) {
        _pesquisaMotivoParadaCentro.ExibirFiltros.visibleFade(false);
        _motivoParadaCentro.Atualizar.visible(true);
        _motivoParadaCentro.Cancelar.visible(true);
        _motivoParadaCentro.Excluir.visible(true);
        _motivoParadaCentro.Adicionar.visible(false);

        RetornoSelecaoCentroCarregamento(arg.Data.CentroCarregamento);

        ControleHabilitacaoFomulario(arg.Data.PermiteEditar);
    }, null);
}

function LimparCamposMotivoParadaCentro() {
    _motivoParadaCentro.Atualizar.visible(false);
    _motivoParadaCentro.Cancelar.visible(false);
    _motivoParadaCentro.Excluir.visible(false);
    _motivoParadaCentro.Adicionar.visible(true);
    _motivoParadaCentro.ContainerTipoOperacao.visible(false);
    LimparCampos(_motivoParadaCentro);
    ControleHabilitacaoFomulario(true);
}

function recarregarGridTipoOperacao() {
    var data = new Array();

    if (!string.IsNullOrWhiteSpace(_motivoParadaCentro.ContainerTipoOperacao.list)) {
        $.each(_motivoParadaCentro.ContainerTipoOperacao.list, function (i, tipoOperacao) {
            data.push({
                Codigo: tipoOperacao.TipoOperacao.codEntity,
                TipoOperacao: tipoOperacao.TipoOperacao.val,
                Quantidade: tipoOperacao.QuantidadeTipoOperacao.val,
            });
        });
    }

    _gridTipoOperacao.CarregarGrid(data);
}

function limparCamposTipoOperacao() {
    _motivoParadaCentro.TipoOperacao.codEntity(_motivoParadaCentro.TipoOperacao.defCodEntity);
    _motivoParadaCentro.TipoOperacao.val(_motivoParadaCentro.TipoOperacao.def);
    _motivoParadaCentro.QuantidadeTipoOperacao.val(_motivoParadaCentro.QuantidadeTipoOperacao.def);
}

function ControleHabilitacaoFomulario(habilitar) {
    _motivoParadaCentro.Descricao.enable(habilitar);
    _motivoParadaCentro.Ativo.enable(habilitar);
    _motivoParadaCentro.Data.enable(habilitar);
    _motivoParadaCentro.PeriodoInicio.enable(habilitar);
    _motivoParadaCentro.PeriodoFim.enable(habilitar);
    _motivoParadaCentro.QuantidadeParada.enable(habilitar);
    _motivoParadaCentro.CentroCarregamento.enable(habilitar);
    _motivoParadaCentro.TipoOperacao.enable(habilitar);
    _motivoParadaCentro.QuantidadeTipoOperacao.enable(habilitar);

    recarregarGridTipoOperacao();

    if (habilitar) {
        _gridTipoOperacao.HabilitarOpcoes();
    } else {
        _gridTipoOperacao.DesabilitarOpcoes();
        _motivoParadaCentro.Excluir.visible(false);
        _motivoParadaCentro.Atualizar.visible(false);
    }
}

function CentroCarregamentoChanged(val) {
    if (val == 0)
        _motivoParadaCentro.ContainerTipoOperacao.visible(false);
}

function RetornoSelecaoCentroCarregamento(data) {
    _motivoParadaCentro.CentroCarregamento.val(data.Descricao);
    _motivoParadaCentro.CentroCarregamento.codEntity(data.Codigo);
    _motivoParadaCentro.ContainerTipoOperacao.visible(data.LimiteCarregamentos == EnumLimiteCarregamentosCentroCarregamento.QuantidadeDocas);
}