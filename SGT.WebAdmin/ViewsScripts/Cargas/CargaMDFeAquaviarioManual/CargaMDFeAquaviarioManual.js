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
/// <reference path="Cargas.js" />
/// <reference path="CTes.js" />
/// <reference path="Etapas.js" />
/// <reference path="Impressao.js" />
/// <reference path="MDFe.js" />
/// <reference path="SignalR.js" />
/// <reference path="Terminais.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCargaMDFeAquaviario;
var _cargaMDFeAquaviario;
var _pesquisaCargaMDFeAquaviario;
var _crudCargaMDFeAquaviario;

var _situacaoMDFeManualPesquisa = [
    { value: "", text: "Todas" },
    { value: EnumSituacaoMDFeManual.EmDigitacao, text: "Em Digitação" },
    { value: EnumSituacaoMDFeManual.EmEmissao, text: "Em Emissão" },
    { value: EnumSituacaoMDFeManual.Finalizado, text: "Finalizado" },
    { value: EnumSituacaoMDFeManual.Rejeicao, text: "Rejeitado" },
    { value: EnumSituacaoMDFeManual.Cancelado, text: "Cancelado" },
    { value: EnumSituacaoMDFeManual.ProcessandoIntegracao, text: "Processando Integração" }
];

var PesquisaCargaMDFeAquaviario = function () {
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.MDFe = PropertyEntity({ text: "Número MDF-e:", maxlength: 12, enable: ko.observable(true) });
    this.CTe = PropertyEntity({ text: "Número CT-e:", maxlength: 12, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Transportador:"), idBtnSearch: guid(), issue: 69, visible: ko.observable(true), enable: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", issue: 16, idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoMDFeManualPesquisa, def: "", text: "Situação: " });

    this.TerminalOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Terminal Origem:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.TerminalDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Terminal Destino:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Navio/Viagem/Direção:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCargaMDFeAquaviario.CarregarGrid();
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

    this.BuscaAvancada = PropertyEntity({
        eventClick: function (e) {
            if (e.BuscaAvancada.visibleFade() == true) {
                e.BuscaAvancada.visibleFade(false);
                e.BuscaAvancada.icon("fal fa-plus");
            } else {
                e.BuscaAvancada.visibleFade(true);
                e.BuscaAvancada.icon("fal fa-minus");
            }
        }, type: types.event, text: "Avançada", idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var CargaMDFeAquaviario = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.UsarDadosCTe = PropertyEntity({ val: ko.observable(false), text: "Usar os dados dos CT-es para emissão do MDF-e?", def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Origem:", idBtnSearch: guid(), issue: 16, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid(), issue: 69, visible: ko.observable(false), required: false, enable: ko.observable(true) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Navio/Viagem/Direção:"), idBtnSearch: guid(), visible: ko.observable(false), required: false, enable: ko.observable(true) });
    this.PortoOrigem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Porto Embarque:"), idBtnSearch: guid(), visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.PortoDestino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Porto Desembarque:"), idBtnSearch: guid(), visible: ko.observable(true), required: true, enable: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoMDFeManual.EmDigitacao), def: EnumSituacaoMDFeManual.EmDigitacao });
    this.SituacaoCancelamento = PropertyEntity({ val: ko.observable(EnumSituacaoMDFeManual.EmDigitacao), def: EnumSituacaoMDFeManual.EmDigitacao });

    this.CTes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Cargas = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.TerminaisCarregamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TerminaisDescarregamento = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaTerminalDestino = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ListaTerminalOrigem = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

var CRUDCargaMDFeAquaviario = function () {
    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", visible: ko.observable(false) });
    this.Emitir = PropertyEntity({ eventClick: EmitirClick, type: types.event, text: "Emitir", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

//*******EVENTOS*******


function loadCargaMDFeAquaviario() {
    ConsultarIntegracaoMDFeAquaviarioManual().then(function () {

        _cargaMDFeAquaviario = new CargaMDFeAquaviario();
        KoBindings(_cargaMDFeAquaviario, "tabDadosGerais");

        _pesquisaCargaMDFeAquaviario = new PesquisaCargaMDFeAquaviario();
        KoBindings(_pesquisaCargaMDFeAquaviario, "knockoutPesquisaCargaMDFeAquaviario", false, _pesquisaCargaMDFeAquaviario.Pesquisar.id);

        _crudCargaMDFeAquaviario = new CRUDCargaMDFeAquaviario();
        KoBindings(_crudCargaMDFeAquaviario, "knockoutCRUDCargaMDFeAquaviario");

        HeaderAuditoria("CargaMDFeManual", _cargaMDFeAquaviario);

        new BuscarLocalidadesBrasil(_cargaMDFeAquaviario.Origem, "Buscar Origem", "Origens", RetornoConsultaOrigem);
        //new BuscarPedidoViagemNavio(_cargaMDFeAquaviario.PedidoViagemNavio);
        new BuscarPorto(_cargaMDFeAquaviario.PortoOrigem, retornoPortoOrigem);
        new BuscarPorto(_cargaMDFeAquaviario.PortoDestino);
        //new BuscarTransportadores(_cargaMDFeAquaviario.Empresa, callbackTransportador);

        new BuscarPedidoViagemNavio(_pesquisaCargaMDFeAquaviario.PedidoViagemNavio);
        new BuscarTipoTerminalImportacao(_pesquisaCargaMDFeAquaviario.TerminalOrigem);
        new BuscarTipoTerminalImportacao(_pesquisaCargaMDFeAquaviario.TerminalDestino);
        new BuscarTransportadores(_pesquisaCargaMDFeAquaviario.Empresa);
        new BuscarLocalidadesBrasil(_pesquisaCargaMDFeAquaviario.Origem, "Buscar Origem", "Origens");
        new BuscarCargas(_pesquisaCargaMDFeAquaviario.Carga);

        LoadConexaoSignalRCargaMDFeAquaviario();
        loadEtapasMDFe();
        loadTerminais();
        LoadCTes();
        LoadCargas();
        loadMDFe();
        LoadImpressaoMDFeManual();
        loadMDFeAquaviarioManualIntegracoes();
        BuscarCargaMDFeAquaviario();

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
            _terminais.Empresa.visible(false);
            _terminais.Empresa.required = false;
            _pesquisaCargaMDFeAquaviario.Empresa.visible(false);
            SetarEnableCamposKnockout(_cargasMDFeManual, true);
            SetarEnableCamposKnockout(_ctesMDFeManual, true);
        } else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
            _terminais.Empresa.text("*Empresa/Filial: ");
            _pesquisaCargaMDFeAquaviario.Empresa.text("*Empresa/Filial: ");
        }
        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) {
            SetarEnableCamposKnockout(_cargasMDFeManual, false);
            SetarEnableCamposKnockout(_ctesMDFeManual, false);
        }
    });
}

function retornoPortoOrigem(data) {
    _cargaMDFeAquaviario.PortoOrigem.codEntity(data.Codigo);
    _cargaMDFeAquaviario.PortoOrigem.val(data.Descricao);

    if (data.CodigoLocalidade > 0) {
        _cargaMDFeAquaviario.Origem.codEntity(data.CodigoLocalidade);
        _cargaMDFeAquaviario.Origem.val(data.Localidade);
    }
}


function callbackTransportador(data) {
    _terminais.Empresa.codEntity(data.Codigo);
    _terminais.Empresa.val(data.Descricao);
    SetarEnableCamposKnockout(_cargasMDFeManual, true);
    SetarEnableCamposKnockout(_ctesMDFeManual, true);
    _terminais.Empresa.enable(false);
}

function RetornoConsultaOrigem(origem) {
    _cargaMDFeAquaviario.Origem.val(origem.Descricao);
    _cargaMDFeAquaviario.Origem.codEntity(origem.Codigo);
}

function SalvarClick() {
    if (!ValidarCamposObrigatorios(_cargaMDFeAquaviario)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    if (_ctesMDFeManual.CtesInfo.basicTable.BuscarRegistros().length <= 0 && _cargasMDFeManual.CargasInfo.basicTable.BuscarRegistros().length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar ao menos um CT-e ou uma Carga para realizar a emissão do MDF-e.");
        return;
    }

    if (_terminais.TerminalCarregamento.basicTable.BuscarRegistros().length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar ao menos um terminal de carregamento");
        return;
    }

    if (_terminais.TerminaisDescarregamento.basicTable.BuscarRegistros().length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar ao menos um terminal de descarregamento");
        return;
    }

    PreencherDadosGerais();

    Salvar(_cargaMDFeAquaviario, "CargaMDFeAquaviarioManual/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso.");
                PreencherObjetoKnout(_cargaMDFeAquaviario, arg);
                SetarDadosGeraisCargaMDFeAquaviario(arg);
                _gridCargaMDFeAquaviario.CarregarGrid();
            } else {
                $("#knockoutDadosMDFe").before('<p class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fal fa-info"></i><strong>Atenção!</strong> Retorno da geração do MDF-e:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function EmitirClick(e, sender) {
    if (!ValidarCamposObrigatorios(_cargaMDFeAquaviario)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    if (_ctesMDFeManual.CtesInfo.basicTable.BuscarRegistros().length <= 0 && _cargasMDFeManual.CargasInfo.basicTable.BuscarRegistros().length <= 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar ao menos um CT-e ou uma Carga para realizar a emissão do MDF-e.");
        return;
    }

    if (!ValidarCamposObrigatorios(_terminais)) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    PreencherDadosGerais();

    exibirConfirmacao("Confirmação", "Realmente deseja autorizar a emissão do MDF-e?", function (confi) {
        Salvar(_cargaMDFeAquaviario, "CargaMDFeAquaviarioManual/Emitir", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Emissão realizada com sucesso.");
                    PreencherObjetoKnout(_cargaMDFeAquaviario, arg);
                    SetarDadosGeraisCargaMDFeAquaviario(arg);
                    _gridCargaMDFeAquaviario.CarregarGrid();
                } else {
                    $("#knockoutDadosMDFe").before('<p class="alert alert-info no-margin"><button class="close" data-dismiss="alert">×</button><i class="fal fa-info"></i><strong>Atenção!</strong> Retorno da geração do MDF-e:<br/>' + arg.Msg.replace(/\n/g, "<br />") + '</p>');
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, sender);
    });
}

function CancelarClick() {
    SetarEtapaMDFe();
    LimparCamposCargaMDFeAquaviario();
}

//*******MÉTODOS*******

function PreencherDadosGerais() {
    var ctes = new Array();
    var cargas = new Array();

    $.each(_ctesMDFeManual.CtesInfo.basicTable.BuscarRegistros(), function (i, cte) {
        ctes.push(cte.Codigo);
    });

    $.each(_cargasMDFeManual.CargasInfo.basicTable.BuscarRegistros(), function (i, cte) {
        cargas.push(cte.Codigo);
    });

    _cargaMDFeAquaviario.CTes.val(JSON.stringify(ctes));
    _cargaMDFeAquaviario.Cargas.val(JSON.stringify(cargas));

    _cargaMDFeAquaviario.TerminaisCarregamento.val(JSON.stringify(_terminais.TerminalCarregamento.basicTable.BuscarRegistros()));
    _cargaMDFeAquaviario.TerminaisDescarregamento.val(JSON.stringify(_terminais.TerminalDescarregamento.basicTable.BuscarRegistros()));
    _cargaMDFeAquaviario.ListaTerminalOrigem.val(PreencherListaCodigosTerminalOrigem());
    _cargaMDFeAquaviario.ListaTerminalDestino.val(PreencherListaCodigosTerminalDestino());
    _cargaMDFeAquaviario.Empresa.codEntity(_terminais.Empresa.codEntity());
    _cargaMDFeAquaviario.PedidoViagemNavio.codEntity(_terminais.PedidoViagemNavio.codEntity());
    _cargaMDFeAquaviario.Empresa.val(_terminais.Empresa.codEntity());
    _cargaMDFeAquaviario.PedidoViagemNavio.val(_terminais.PedidoViagemNavio.codEntity());
}

function BuscarCargaMDFeAquaviario() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: EditarCargaMDFeAquaviario, tamanho: "6", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCargaMDFeAquaviario = new GridView(_pesquisaCargaMDFeAquaviario.Pesquisar.idGrid, "CargaMDFeAquaviarioManual/Pesquisa", _pesquisaCargaMDFeAquaviario, menuOpcoes, null);
    _gridCargaMDFeAquaviario.CarregarGrid();
}

function EditarCargaMDFeAquaviario(cargaMDFeAquaviarioGrid) {
    LimparCamposCargaMDFeAquaviario();
    _cargaMDFeAquaviario.Codigo.val(cargaMDFeAquaviarioGrid.Codigo);
    BuscarCargaMDFeAquaviarioPorCodigo();
}

function BuscarCargaMDFeAquaviarioPorCodigo(callback) {
    BuscarPorCodigo(_cargaMDFeAquaviario, "CargaMDFeAquaviarioManual/BuscarPorCodigo", function (arg) {
        SetarDadosGeraisCargaMDFeAquaviario(arg);
        _terminais.Empresa.enable(false);

        _terminais.Empresa.codEntity(_cargaMDFeAquaviario.Empresa.codEntity());
        _terminais.PedidoViagemNavio.codEntity(_cargaMDFeAquaviario.PedidoViagemNavio.codEntity());
        _terminais.Empresa.val(_cargaMDFeAquaviario.Empresa.val());
        _terminais.PedidoViagemNavio.val(_cargaMDFeAquaviario.PedidoViagemNavio.val());

        _terminais.TerminalCarregamento.basicTable.SetarRegistros(_cargaMDFeAquaviario.TerminaisCarregamento.val());
        _terminais.TerminalDescarregamento.basicTable.SetarRegistros(_cargaMDFeAquaviario.TerminaisDescarregamento.val());

        if (_cargaMDFeAquaviario.Situacao.val() == EnumSituacaoMDFeManual.EmDigitacao) {
            SetarEnableCamposKnockout(_cargasMDFeManual, true);
            SetarEnableCamposKnockout(_ctesMDFeManual, true);
            _terminais.TerminalCarregamento.basicTable.HabilitarOpcoes();
            _terminais.TerminalDescarregamento.basicTable.HabilitarOpcoes();
        } else {
            _terminais.TerminalCarregamento.basicTable.DesabilitarOpcoes();
            _terminais.TerminalDescarregamento.basicTable.DesabilitarOpcoes();
        }

        if (callback != null)
            callback();
    }, null);
}

function SetarDadosGeraisCargaMDFeAquaviario(arg) {
    RecarregarCTes(arg.Data.CTes);
    RecarregarCargas(arg.Data.Cargas);

    _pesquisaCargaMDFeAquaviario.ExibirFiltros.visibleFade(false);

    _terminais.Empresa.codEntity(_cargaMDFeAquaviario.Empresa.codEntity());
    _terminais.PedidoViagemNavio.codEntity(_cargaMDFeAquaviario.PedidoViagemNavio.codEntity());
    _terminais.Empresa.val(_cargaMDFeAquaviario.Empresa.val());
    _terminais.PedidoViagemNavio.val(_cargaMDFeAquaviario.PedidoViagemNavio.val());

    _terminais.TerminalCarregamento.basicTable.SetarRegistros(_cargaMDFeAquaviario.TerminaisCarregamento.val());
    _terminais.TerminalDescarregamento.basicTable.SetarRegistros(_cargaMDFeAquaviario.TerminaisDescarregamento.val());

    if (_cargaMDFeAquaviario.Situacao.val() == EnumSituacaoMDFeManual.EmDigitacao) {
        _crudCargaMDFeAquaviario.Emitir.visible(true);
        _crudCargaMDFeAquaviario.Salvar.visible(false);
        _terminais.TerminalCarregamento.basicTable.HabilitarOpcoes();
        _terminais.TerminalDescarregamento.basicTable.HabilitarOpcoes();
    } else {
        _crudCargaMDFeAquaviario.Emitir.visible(false);
        _crudCargaMDFeAquaviario.Salvar.visible(false);
        SetarEnableCamposKnockout(_cargaMDFeAquaviario, false);
        SetarEnableCamposKnockout(_terminais, false);

        _terminais.TerminalCarregamento.basicTable.DesabilitarOpcoes();
        _terminais.TerminalDescarregamento.basicTable.DesabilitarOpcoes();
    }

    SetarEtapaMDFe();
}

function LimparCamposCargaMDFeAquaviario() {
    _crudCargaMDFeAquaviario.Emitir.visible(true);
    _crudCargaMDFeAquaviario.Salvar.visible(false);

    SetarEnableCamposKnockout(_cargasMDFeManual, true);
    SetarEnableCamposKnockout(_ctesMDFeManual, true);

    SetarEnableCamposKnockout(_terminais, true);
    SetarEnableCamposKnockout(_cargaMDFeAquaviario, true);

    _terminais.TerminalCarregamento.basicTable.SetarRegistros([]);
    _terminais.TerminalCarregamento.basicTable.HabilitarOpcoes();

    _terminais.TerminalDescarregamento.basicTable.SetarRegistros([]);
    _terminais.TerminalDescarregamento.basicTable.HabilitarOpcoes();

    LimparCampos(_cargaMDFeAquaviario);
    LimparCampos(_ctesMDFeManual);
    LimparCampos(_terminais);

    RecarregarCargas();
    RecarregarCTes();
}
