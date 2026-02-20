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
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="../../Consultas/OrdemServico.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../Enumeradores/EnumEntradaSaida.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGuaritaCheckList.js" />
/// <reference path="Manutencao.js" />
/// <reference path="Abastecimento.js" />
/// <reference path="ManutencaoEquipamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _girdCheckList;
var _guaritaCheckList;
var _pesquisaGuaritaCheckList;
var _CRUD;

var PesquisaGuaritaCheckList = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.OrdemServico = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "O.S.:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Operador:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });

    this.Tipo = PropertyEntity({ val: ko.observable(EnumEntradaSaida.Todos), options: EnumEntradaSaida.obterOpcoesPesquisa(), def: EnumEntradaSaida.Todos, text: "Entrada/Saída:" });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoGuaritaCheckList.Todos), options: EnumSituacaoGuaritaCheckList.obterOpcoesPesquisa(), def: EnumSituacaoGuaritaCheckList.Todos, text: "Situação:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _girdCheckList.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
    this.Novo = PropertyEntity({
        eventClick: NovoClick, type: types.event, text: "Novo Check List", icon: ko.observable("fal fa-check-square-o"), visible: ko.observable(true)
    });
};

var GuaritaCheckList = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    //this.Operador = PropertyEntity({ text: "Operador: ", enable: ko.observable(false) });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Operador:", idBtnSearch: guid(), enable: ko.observable(false) });
    this.Veiculo = PropertyEntity({ text: "Veículo: ", enable: ko.observable(false) });
    this.Motorista = PropertyEntity({ text: "Motorista: ", enable: ko.observable(false) });
    this.OrdemServico = PropertyEntity({ text: "O.S.: ", enable: ko.observable(false), visible: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "Tipo: ", enable: ko.observable(false) });
    this.Carga = PropertyEntity({ text: "Carga: ", enable: ko.observable(false), visible: ko.observable(true) });

    this.Data = PropertyEntity({ text: "*Data: ", getType: typesKnockout.dateTime, enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoGuaritaCheckList.Todos), options: EnumSituacaoGuaritaCheckList.obterOpcoes(), def: EnumSituacaoGuaritaCheckList.Todos, text: "Situação:", enable: ko.observable(true) });

    this.CheckList = PropertyEntity({ val: GetSetCheckList, def: [] });
    this.Croquis = PropertyEntity({ val: GetSetCroquis, def: [] });
    this.Manutencao = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.Abastecimento = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
    this.ManutencaoEquipamento = PropertyEntity({ val: ko.observable(new Object), def: ko.observable(new Object), getType: typesKnockout.dynamic });
};

var CRUDGuaritaCheckList = function () {
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: "Imprimir", visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadGuaritaCheckList() {
    _guaritaCheckList = new GuaritaCheckList();
    KoBindings(_guaritaCheckList, "knockoutGuaritaCheckList");

    HeaderAuditoria("GuaritaCheckList", _guaritaCheckList);

    _CRUD = new CRUDGuaritaCheckList();
    KoBindings(_CRUD, "knockoutCRUD");

    _pesquisaGuaritaCheckList = new PesquisaGuaritaCheckList();
    KoBindings(_pesquisaGuaritaCheckList, "knockoutPesquisaGuaritaCheckList", false, _pesquisaGuaritaCheckList.Pesquisar.id);

    new BuscarCargas(_pesquisaGuaritaCheckList.Carga);
    new BuscarOrdemServico(_pesquisaGuaritaCheckList.OrdemServico);
    new BuscarFuncionario(_pesquisaGuaritaCheckList.Operador);
    new BuscarVeiculos(_pesquisaGuaritaCheckList.Veiculo);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiNFe) {
        _pesquisaGuaritaCheckList.Carga.visible(false);
        _pesquisaGuaritaCheckList.OrdemServico.visible(false);
        _guaritaCheckList.Carga.visible(false);
        _guaritaCheckList.OrdemServico.visible(false);
    }

    $.get("Content/Static/GestaoPatio/GuaritaCheckList.html?dyn=" + guid(), function (data) {
        $(".check-list-html").html(data);

        BuscarGuaritaCheckList();

        LoadVistoriaRecebimento();
        LoadCroquis();
        loadNovoCheckList();
        loadAnexos();
        loadGuaritaCheckListManutencao();
        loadGuaritaCheckListAbastecimento();
        loadGuaritaCheckListManutencaoEquipamento();
    });
}

function NovoClick(e, sender) {
    Global.abrirModal('divModalNovoCheckList');
}

function atualizarClick(e, sender) {
    if (!validaCamposObrigatoriosGuaritaCheckListManutencao()) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe todos os campos Obrigatórios da Manutenção");
        return;
    } else if (!validaCamposObrigatoriosGuaritaCheckListAbastecimento()) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe todos os campos Obrigatórios do Abastecimento");
        return;
    } else if (!validaCamposObrigatoriosGuaritaCheckListManutencaoEquipamento()) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe todos os campos Obrigatórios da Manutenção de Equipamento");
        return;
    }

    _guaritaCheckList.Manutencao.val(JSON.stringify(RetornarObjetoPesquisa(_guaritaCheckListManutencao)));
    _guaritaCheckList.Abastecimento.val(JSON.stringify(RetornarObjetoPesquisa(_guaritaCheckListAbastecimento)));
    _guaritaCheckList.ManutencaoEquipamento.val(JSON.stringify(RetornarObjetoPesquisa(_guaritaCheckListManutencaoEquipamento)));

    Salvar(_guaritaCheckList, "GuaritaCheckList/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _girdCheckList.CarregarGrid();
                LimparCamposGuaritaCheckList();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    LimparCamposGuaritaCheckList();
}

function ImprimirClick(e) {
    executarDownload("GuaritaCheckList/Imprimir", { Codigo: _guaritaCheckList.Codigo.val() });
}

//*******MÉTODOS*******

function BuscarGuaritaCheckList() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarGuaritaCheckList, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _girdCheckList = new GridView(_pesquisaGuaritaCheckList.Pesquisar.idGrid, "GuaritaCheckList/Pesquisa", _pesquisaGuaritaCheckList, menuOpcoes, null);
    _girdCheckList.CarregarGrid();
}

function editarGuaritaCheckList(dataRow) {
    LimparCamposGuaritaCheckList();
    _guaritaCheckList.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_guaritaCheckList, "GuaritaCheckList/BuscarPorCodigo", function (arg) {
        $("#guaritachecklist").show();

        executarReST("Usuario/DadosUsuarioLogado", {}, function (argUsuario) {
            if (argUsuario.Success) {
                if (argUsuario.Data !== false && argUsuario.Data != null) {
                    _CRUD.Atualizar.visible(true);

                    _pesquisaGuaritaCheckList.ExibirFiltros.visibleFade(false);
                    EditarVistoriaRecebimento(arg.Data);
                    EditarCroquis(arg.Data);
                    EditarListarAnexos(arg);

                    _guaritaCheckList.Operador.codEntity(argUsuario.Data.Codigo);
                    _guaritaCheckList.Operador.val(argUsuario.Data.Nome);
                    _guaritaCheckList.Data.val(argUsuario.Data.DataHoraAtual);

                    if (_guaritaCheckList.Situacao.val() !== EnumSituacaoGuaritaCheckList.Aberto) {
                        _guaritaCheckList.Data.enable(false);
                        _guaritaCheckList.Situacao.enable(false);
                        _CRUD.Atualizar.visible(false);
                        BloquearAlteracaoServicos();
                        BloquearAlteracaoServicosEquipamento();
                        SetarEnableCamposKnockout(_guaritaCheckListAbastecimento, false);
                    }

                    if (arg.Data.Manutencao !== null && arg.Data.Manutencao !== undefined) {
                        PreencherObjetoKnout(_guaritaCheckListManutencao, { Data: arg.Data.Manutencao });
                        BuscarManutencoesGuaritaCheckList();
                    }

                    if (arg.Data.Abastecimento !== null && arg.Data.Abastecimento !== undefined)
                        PreencherObjetoKnout(_guaritaCheckListAbastecimento, { Data: arg.Data.Abastecimento });

                    if (arg.Data.ManutencaoEquipamento !== null && arg.Data.ManutencaoEquipamento !== undefined) {
                        PreencherObjetoKnout(_guaritaCheckListManutencaoEquipamento, { Data: arg.Data.ManutencaoEquipamento });
                        BuscarManutencoesEquipamentoGuaritaCheckList();
                    }
                }
            }
        });
    }, null);
}

function LimparCamposGuaritaCheckList() {
    _CRUD.Atualizar.visible(false);
    _guaritaCheckList.Data.enable(true);
    _guaritaCheckList.Situacao.enable(true);

    LimparCampos(_guaritaCheckList);
    LimparVistoriaRecebimento();
    LimparCroquis();
    limparCamposGuaritaCheckListManutencao();
    limparCamposGuaritaCheckListAbastecimento();
    limparCamposGuaritaCheckListManutencaoEquipamento();

    $("#guaritachecklist").hide();
    _pesquisaGuaritaCheckList.ExibirFiltros.visibleFade(true);
    $("#knockoutGuaritaCheckList").click();
}