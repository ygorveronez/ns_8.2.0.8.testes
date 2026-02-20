
//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaCTeAgrupado;
var _cargaCTeAgrupado;
var _CRUDCargaCTeAgrupado;
var _pesquisaCargaCTeAgrupado;

var CargaCTeAgrupado = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", text: "Situação: " });
};

var CRUDCargaCTeAgrupado = function () {
    this.Limpar = PropertyEntity({ eventClick: LimparCargaCTeAgrupadoClick, type: types.event, text: "Limpar/Cancelar", idGrid: guid(), visible: ko.observable(false) });
    this.AlterarMoeda = PropertyEntity({ eventClick: AbrirTelaAlteracaoMoedaCargaCTeAgrupado, type: types.event, text: "Alterar a Moeda", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: AbrirTelaCancelamentoCargaCTeAgrupado, type: types.event, text: "Cancelar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(false) });
    this.Reprocessar = PropertyEntity({ eventClick: ReprocessarCargaCTeAgrupadoClick, type: types.event, text: "Reprocessar", idGrid: guid(), visible: ko.observable(false), enable: ko.observable(false) });
};

var PesquisaCargaCTeAgrupado = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CT-e:", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(""), def: "", options: EnumSituacaoCargaCTeAgrupado.ObterOpcoesPesquisa(), text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaCTeAgrupado.CarregarGrid();
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

function LoadCargaCTeAgrupado() {
    _cargaCTeAgrupado = new CargaCTeAgrupado();

    HeaderAuditoria("CargaCTeAgrupado", _cargaCTeAgrupado);

    _CRUDCargaCTeAgrupado = new CRUDCargaCTeAgrupado();
    KoBindings(_CRUDCargaCTeAgrupado, "knockoutCRUD");

    _pesquisaCargaCTeAgrupado = new PesquisaCargaCTeAgrupado();
    KoBindings(_pesquisaCargaCTeAgrupado, "knockoutPesquisaCargaCTeAgrupado", false, _pesquisaCargaCTeAgrupado.Pesquisar.id);

    LoadEtapasCargaCTeAgrupado();
    LoadSelecaoCargasCargaCTeAgrupado();
    LoadAlteracaoMoedaCargaCTeAgrupado();
    LoadCancelamentoCargaCTeAgrupado();

    new BuscarCargas(_pesquisaCargaCTeAgrupado.Carga, null, null, null, null, null, null, null, null, true);
    new BuscarCTes(_pesquisaCargaCTeAgrupado.CTe);

    LoadDocumentosCargaCTeAgrupado();
    LoadIntegracoes();
    BuscarCargaCTeAgrupado();
}


function LimparCargaCTeAgrupadoClick(e, sender) {
    LimparCamposCargaCTeAgrupado();
    GridSelecaoCargas();
}

//*******MÉTODOS*******

function ReprocessarCargaCTeAgrupadoClick(e, sender) {
    exibirConfirmacao("Confirmação!", "Deseja realmente reprocessar o CT-e agrupado?", function () {
        executarReST("CargaCTeAgrupado/Reprocessar", { Codigo: _cargaCTeAgrupado.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Reprocessamento solicitado com sucesso!");
                    BuscarCargaCTeAgrupadoPorCodigo(_cancelamentoCargaCTeAgrupado.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function BuscarCargaCTeAgrupado() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarCargaCTeAgrupado, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configExportacao = {
        url: "CargaCTeAgrupado/ExportarPesquisa",
        titulo: "CT-es Agrupados",
        id: guid()
    };

    _gridCargaCTeAgrupado = new GridView(_pesquisaCargaCTeAgrupado.Pesquisar.idGrid, "CargaCTeAgrupado/Pesquisa", _pesquisaCargaCTeAgrupado, menuOpcoes, null, null, null, null, null, null, null, null, configExportacao);
    _gridCargaCTeAgrupado.CarregarGrid();
}

function EditarCargaCTeAgrupado(itemGrid) {
    // Limpa os campos
    LimparCamposCargaCTeAgrupado();

    // Esconde filtros
    _pesquisaCargaCTeAgrupado.ExibirFiltros.visibleFade(false);

    // Busca dados
    BuscarCargaCTeAgrupadoPorCodigo(itemGrid.Codigo);
}

function BuscarCargaCTeAgrupadoPorCodigo(codigo) {
    executarReST("CargaCTeAgrupado/BuscarPorCodigo", { Codigo: codigo }, function (arg) {
        if (arg.Data != null) {
            SetarDadosCargaCTeAgrupado(arg.Data);
            EditarSelecaoCargas(arg.Data);
            SetarEtapasCargaCTeAgrupado();

            if (arg.Data.Situacao === EnumSituacaoCargaCTeAgrupado.Finalizado &&
                _CONFIGURACAO_TMS.UtilizaMoedaEstrangeira === true &&
                VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_AlterarMoeda, _PermissoesPersonalizadas) === true) {
                _CRUDCargaCTeAgrupado.AlterarMoeda.visible(true);
                _CRUDCargaCTeAgrupado.AlterarMoeda.enable(true);
            } else {
                _CRUDCargaCTeAgrupado.AlterarMoeda.visible(false);
                _CRUDCargaCTeAgrupado.AlterarMoeda.enable(false);
            }

            if (arg.Data.Situacao === EnumSituacaoCargaCTeAgrupado.Rejeitado) {
                _CRUDCargaCTeAgrupado.Reprocessar.visible(true);
                _CRUDCargaCTeAgrupado.Reprocessar.enable(true);
            }

            if (arg.Data.Situacao === EnumSituacaoCargaCTeAgrupado.Finalizado ||
                arg.Data.Situacao === EnumSituacaoCargaCTeAgrupado.Rejeitado ||
                arg.Data.Situacao === EnumSituacaoCargaCTeAgrupado.AgIntegracao ||
                arg.Data.Situacao === EnumSituacaoCargaCTeAgrupado.FalhaIntegracao) {
                _CRUDCargaCTeAgrupado.Cancelar.visible(true);
                _CRUDCargaCTeAgrupado.Cancelar.enable(true);
            } else {
                _CRUDCargaCTeAgrupado.Cancelar.visible(false);
                _CRUDCargaCTeAgrupado.Cancelar.enable(false);
            }

        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function SetarDadosCargaCTeAgrupado(data) {
    _cargaCTeAgrupado.Codigo.val(data.Codigo);
    _cargaCTeAgrupado.Situacao.val(data.Situacao);
    _CRUDCargaCTeAgrupado.Limpar.visible(true);
}

function LimparCamposCargaCTeAgrupado() {
    LimparCampos(_cargaCTeAgrupado);
    _cargaCTeAgrupado.Situacao.val("");

    _CRUDCargaCTeAgrupado.Limpar.visible(false);
    DesabilitarTodasEtapasCTeAgrupado();
    SetarEtapasCargaCTeAgrupado();

    LimparCamposSelecaoCargas();


    $("#" + _etapaCargaCTeAgrupado.Etapa1.idTab).click();

    _CRUDCargaCTeAgrupado.AlterarMoeda.visible(false);
    _CRUDCargaCTeAgrupado.AlterarMoeda.enable(false);
    _CRUDCargaCTeAgrupado.Cancelar.visible(false);
    _CRUDCargaCTeAgrupado.Cancelar.enable(false);
    _CRUDCargaCTeAgrupado.Reprocessar.visible(false);
    _CRUDCargaCTeAgrupado.Reprocessar.enable(false);
}