/// <reference path="../../../../js/Global/CRUD.js" />
/// <reference path="../../../../js/Global/Globais.js" />
/// <reference path="../../../../js/Global/Grid.js" />
/// <reference path="../../../../js/Global/Mensagem.js" />
/// <reference path="../../../../js/Global/Rest.js" />
/// <reference path="../../../Consultas/Cliente.js" />

// #region Objetos Globais do Arquivo

var _cargaOrganizacao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var CargaOrganizacao = function () {
    var self = this;
    this.GridCargasOrganizacao = PropertyEntity({ type: types.local, idGrid: guid(), visible: ko.observable(true), visibleLegenda: ko.observable(false) });
    this.AdicionarCargaOrganizacao = PropertyEntity({ idBtnSearch: guid(), type: types.event, text: "Vincular Pré Carga", visible: ko.observable(true), enable: ko.observable(true), idGrid: guid(), codEntity: ko.observable(0) });

    this.BuscaRemetentes = PropertyEntity({ val: ko.observable(""), def: "" });
    this.BuscaDestinatarios = PropertyEntity({ val: ko.observable(""), def: "" });

    this.Atualizar = PropertyEntity({ eventClick: atualizarCargaOrganizacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(true) });
    this.LiberarSemCargaOrganizacao = PropertyEntity({ eventClick: liberarCargaSemCargaOrganizacaoClick, type: types.event, text: "Liberar sem Pré Carga", visible: ko.observable(true), enable: ko.observable(true) });
}
// #endregion Classes

// #region Funções de Inicialização

function loadCargaOrganizacao() {
    _cargaOrganizacao = new CargaOrganizacao();
    KoBindings(_cargaOrganizacao, "knockoutCargaOrganizacao");

    carregarGridCargaOrganizacao();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function atualizarCargaOrganizacaoClick() {
    let dados = {
        Carga: _cargaAtual.Codigo.val(),
        CodigosCargaOrganizacao: JSON.stringify(_cargaOrganizacao.AdicionarCargaOrganizacao.basicTable.BuscarCodigosRegistros()),
    };

    executarReST("Carga/VincularCargaComCargaOrganizacao", dados, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Vinculado Pré Carga com sucesso");
                fecharModalCargaOrganizacao();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function liberarCargaSemCargaOrganizacaoClick() {
    executarReST("Carga/LiberarSemCargaOrganizacao", { Carga: _cargaAtual.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Carga liberada sem Pré Carga");
                fecharModalCargaOrganizacao();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Associadas a Eventos


// #region Cargas Organização
function removerCargaOrganizacaoClick(data) {

    let cargasOrganizacaoGrid = _cargaOrganizacao.AdicionarCargaOrganizacao.basicTable.BuscarRegistros();
    for (let i = 0; i < cargasOrganizacaoGrid.length; i++) {
        if (data.Codigo == cargasOrganizacaoGrid[i].Codigo) {
            cargasOrganizacaoGrid.splice(i, 1);
            break;
        }
    }
    _cargaOrganizacao.AdicionarCargaOrganizacao.basicTable.CarregarGrid(cargasOrganizacaoGrid);
}

function atualizarGridCargaOrganizacao(cargas) {
    let cargasOrganizacao = new Array();

    if (cargas != null && cargas.length > 0) {
        $.each(cargas, function (i, carga) {
            cargasOrganizacao.push({ Codigo: carga.Codigo, CodigoCargaEmbarcador: carga.CodigoCargaEmbarcador, Transportador: carga.Transportador, Motorista: carga.Motorista, DescricaoTracao: carga.DescricaoTracao, DescricaoReboque: carga.DescricaoReboque });
        });
    }                                                                                                                                                                                                                                 

    if (_cargaOrganizacao.AdicionarCargaOrganizacao?.basicTable != null)
        _cargaOrganizacao.AdicionarCargaOrganizacao.basicTable.CarregarGrid(cargasOrganizacao);
}

function carregarGridCargaOrganizacao() {
    let remover = {
        descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: function (datagrid) {
            removerCargaOrganizacaoClick(datagrid);
        }, icone: ""
    };
    let menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [remover] };

    let header = [
        { data: "Codigo", visible: false },
        { data: "CodigoCargaEmbarcador", title: "Carga", width: "50%", className: "text-align-center", orderable: false },
        { data: "Transportador", title: "Transportador", width: "20%", className: "text-align-left", orderable: false },
        { data: "Motorista", title: "Motorista", width: "20%", className: "text-align-left", orderable: false },
        { data: "DescricaoTracao", title: "Tração", width: "20%", className: "text-align-left", orderable: false },
        { data: "DescricaoReboque", title: "Reboque", width: "20%", className: "text-align-left", orderable: false },
    ];

    let _gridCargaOrganizacao = new BasicDataTable(_cargaOrganizacao.AdicionarCargaOrganizacao.idGrid, header, menuOpcoes);

    _gridCargaOrganizacao.CarregarGrid(new Array());

    _cargaOrganizacao.AdicionarCargaOrganizacao.basicTable = _gridCargaOrganizacao;

    BuscarCargaOrganizacao(_cargaOrganizacao.AdicionarCargaOrganizacao, _gridCargaOrganizacao, function (cargas) {
        atualizarGridCargaOrganizacao(cargas);
    })
}
// #endregion



// #region Funções Públicas
function exibirCargaOrganizacao() {
    loadCargaOrganizacao();
    executarReST("Carga/ObterCargaOrganizacao", { Carga: _cargaAtual.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_cargaOrganizacao, retorno);

                atualizarGridCargaOrganizacao(retorno.Data.CargasOrganizacao);

                setarEnableCamposCargaOrganizacao(retorno.Data.PermiteEditar);
                setarVisibilidadeBotoesCargaOrganizacao();

                exibirModalCargaOrganizacao();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalCargaOrganizacao() {
    Global.abrirModal("divModalInformarCargaOrganizacao");
    $("#divModalInformarCargaOrganizacao").one('hidden.bs.modal', function () {
        LimparCampos(_cargaOrganizacao);
    });
}

function fecharModalCargaOrganizacao() {
    Global.fecharModal("divModalInformarCargaOrganizacao");
}

function setarVisibilidadeBotoesCargaOrganizacao() {
    _cargaOrganizacao.Atualizar.visible(true);
    _cargaOrganizacao.LiberarSemCargaOrganizacao.visible(true);
}

function setarEnableCamposCargaOrganizacao(permiteEditar) {
    SetarEnableCamposKnockout(_cargaOrganizacao, permiteEditar);
}

// #endregion Funções Privadas
