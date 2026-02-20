/// <reference path="Resumo.js" />
/// <reference path="Etapas.js" />
/// <reference path="CTe.js" />
/// <reference path="../../Enumeradores/EnumModalidadePessoa.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Consultas/GrupoPessoa.js" />
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
/// <reference path="../../../js/Global/Auditoria.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCIOT.js" />
/// <reference path="Etapas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridCIOT;
var _CIOT;
var _pesquisaCIOT;

var _situacaoCIOT = [
    { text: "Todas", value: "" },
    { text: "Aberto", value: EnumSituacaoCIOT.Aberto },
    { text: "Encerrado", value: EnumSituacaoCIOT.Encerrado },
    { text: "Cancelado", value: EnumSituacaoCIOT.Cancelado },
    { text: "Ag. Integração", value: EnumSituacaoCIOT.AgIntegracao },
    { text: "Pendente", value: EnumSituacaoCIOT.Pendencia },
    { text: "Pgto. Autorizado", value: EnumSituacaoCIOT.PagamentoAutorizado }
];

var _operadorasCIOT = [];

var PesquisaCIOT = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: _situacaoCIOT, def: "", text: "Situação:" });
    this.NumeroCarga = PropertyEntity({ text: "Nº da Carga:" });

    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Veículo:", idBtnSearch: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Motorista:", idBtnSearch: guid() });
    this.Operadora = PropertyEntity({ val: ko.observable(""), options: EnumOperadoraCIOT.ObterOpcoesPesquisa(), def: "", text: "Operadora:" });
    this.CodigoVerificador = PropertyEntity({ text: "Cód. Verificador:", getType: typesKnockout.string, maxlength: 10 });
    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.string, maxlength: 12 });
    this.Filial = PropertyEntity({ required: true, codEntity: ko.observable(0), type: types.multiplesEntities, text: "Filial", idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCIOT.CarregarGrid();
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
};

var CIOT = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Operadora = PropertyEntity({ text: "*Operadora:", options: ko.observable(_operadorasCIOT), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true), required: true, enable: ko.observable(true) });
    this.DataFinalViagem = PropertyEntity({ text: "*Data de Término:", getType: typesKnockout.date, val: ko.observable(""), def: "", required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Terceiro:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Veículo:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Motorista:", idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("*Transportador:"), idBtnSearch: guid(), required: true, visible: ko.observable(true), enable: ko.observable(true) });

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCIOT.Aberto), def: EnumSituacaoCIOT.Aberto, text: "Situação:", getType: typesKnockout.int });

    this.HistoricoIntegracao = PropertyEntity({ eventClick: HistoricoIntegracaoCIOTClick, type: types.event, text: "Histórico de Integrações", icon: "fa-history", visible: ko.observable(false) });
};

var CRUDCIOT = function () {
    this.Abrir = PropertyEntity({ eventClick: AbrirClick, type: types.event, text: ko.observable("Abrir"), visible: ko.observable(true) });
    this.Encerrar = PropertyEntity({ eventClick: EncerrarClick, type: types.event, text: "Encerrar", visible: ko.observable(false) });
    this.EncerrarGerencialmente = PropertyEntity({ eventClick: EncerrarGerencialmenteClick, type: types.event, text: "Encerrar Gerencialmente", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Interromper = PropertyEntity({ eventClick: InterromperClick, type: types.event, text: "Interromper", visible: ko.observable(false) });
    this.GerarNovo = PropertyEntity({ eventClick: GerarNovoCIOTClick, type: types.event, text: "Limpar Campos", visible: ko.observable(true) });
    this.ImprimirContrato = PropertyEntity({ eventClick: ImprimirContratoClick, type: types.event, text: ko.observable("Imprimir Contrato"), visible: ko.observable(false) });

    this.EnviarIntegracaoQuitacaoAX = PropertyEntity({ eventClick: EnviarIntegracaoQuitacaoAXClick, type: types.event, text: "Enviar Integração Quitação AX", visible: ko.observable(false) });
    this.BuscarConciliacaoCIOT = PropertyEntity({ eventClick: BuscarConciliacaoCIOTClick, type: types.event, text: "Buscar Conciliação CIOT", visible: ko.observable(false) });
};

//*******EVENTOS*******

function LoadCIOT() {
    var conteudoexterno = $("#knockoutPesquisaCIOT").length === 0;
    _CIOT = new CIOT();
    KoBindings(_CIOT, "knockoutAbertura");

    _CRUDCIOT = new CRUDCIOT();
    KoBindings(_CRUDCIOT, "knockoutCRUDCIOT");

    _pesquisaCIOT = new PesquisaCIOT();

    if (!conteudoexterno)
        KoBindings(_pesquisaCIOT, "knockoutPesquisaCIOT", false, _pesquisaCIOT.Pesquisar.id);

    new BuscarClientes(_pesquisaCIOT.Transportador, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
    new BuscarVeiculos(_pesquisaCIOT.Veiculo);
    new BuscarMotoristas(_pesquisaCIOT.Motorista);
    new BuscarFilial(_pesquisaCIOT.Filial);

    new BuscarClientes(_CIOT.Transportador, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);
    new BuscarTransportadores(_CIOT.Empresa);
    new BuscarVeiculos(_CIOT.Veiculo, null, null, null, _CIOT.Motorista, null, null, null, null, null, null, null, null, null, null, null, null, null, _CIOT.Transportador, "0");
    new BuscarMotoristas(_CIOT.Motorista);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS)
        _CIOT.Empresa.text("*Empresa/Filial:");

    LoadEtapa();
    LoadCTe();
    LoadResumo();
    LoadEncerramento();
    LoadHistoricoIntegracaoCIOT();

    BuscarOperadorasCIOT();

    if (!conteudoexterno) {
        HeaderAuditoria("CIOT", _CIOT);
        BuscarCIOTs();
    }
}

function LoadHistoricoIntegracaoCIOT() {
    var download = { descricao: "Download Arquivos", id: guid(), evento: "onclick", metodo: DownloadArquivosHistoricoIntegracaoCIOT, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [download]
    };

    _gridHistoricoIntegracaoCIOT = new GridView("tblHistoricoIntegracaoCIOT", "CIOT/ConsultarHistoricoIntegracao", _CIOT, menuOpcoes, { column: 1, dir: orderDir.desc });
}

function HistoricoIntegracaoCIOTClick(integracao) {
    _gridHistoricoIntegracaoCIOT.CarregarGrid().then(function () {
        Global.abrirModal("divModalHistoricoIntegracaoCIOT");
    });
}

function DownloadArquivosHistoricoIntegracaoCIOT(historicoConsulta) {
    executarDownload("CIOT/DownloadArquivosHistoricoIntegracao", { Codigo: historicoConsulta.Codigo });
}

function AbrirClick(e, sender) {
    Salvar(_CIOT, "CIOT/Abrir", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "CIOT aberto com sucesso!");
                _CIOT.Codigo.val(r.Data.Codigo);
                BuscarCIOTPorCodigo();
                _gridCIOT.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    }, sender);
}

function EncerrarClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente encerrar o CIOT?", function () {
        executarReST("CIOT/Encerrar", { Codigo: _CIOT.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Encerramento realizado com sucesso!");
                    BuscarCIOTPorCodigo();
                    _gridCIOT.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function EncerrarGerencialmenteClick(e, sender) {
    exibirConfirmacao("Atenção!", 'Deseja realmente encerrar GERENCIALMENTE este CIOT? Este processo não encerrará o CIOT na operadora, assim como não é possível reverter esta ação, pois serão geradas movimentações financeiras e demais dados referente ao processo.', function () {
        executarReST("CIOT/EncerrarGerencialmente", { Codigo: _CIOT.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Encerramento realizado com sucesso!");
                    BuscarCIOTPorCodigo();
                    _gridCIOT.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function CancelarClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente cancelar o CIOT?", function () {
        executarReST("CIOT/Cancelar", { Codigo: _CIOT.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento realizado com sucesso!");
                    BuscarCIOTPorCodigo();
                    _gridCIOT.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function InterromperClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente interromper o CIOT?", function () {
        executarReST("CIOT/Interromper", { Codigo: _CIOT.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Interrupção realizada com sucesso!");
                    BuscarCIOTPorCodigo();
                    _gridCIOT.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function ImprimirContratoClick(e, sender) {
    executarDownload("CIOT/DownloadContrato", { Codigo: _CIOT.Codigo.val() });
}

function GerarNovoCIOTClick() {
    LimparCamposCIOT();
}

function EnviarIntegracaoQuitacaoAXClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente enviar a integração de quitação com AX?", function () {
        executarReST("CIOT/EnviarIntegracaoQuitacaoAX", { Codigo: _CIOT.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Integração enviada com sucesso!");
                    _gridCIOT.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        });
    });
}

function BuscarConciliacaoCIOTClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente realizar a conciliação dos CIOT's pendentes?", function () {
        executarReST("CIOT/BuscarConciliacaoCIOT", null, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Conciliação realizada com sucesso!");
                    LimparCamposCIOT();
                    _gridCIOT.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        });
    });
}

//*******MÉTODOS*******

function BuscarCIOTs() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarCIOT, tamanho: "10", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCIOT = new GridView(_pesquisaCIOT.Pesquisar.idGrid, "CIOT/Pesquisa", _pesquisaCIOT, menuOpcoes, { column: 0, dir: orderDir.desc }, null);

    _gridCIOT.CarregarGrid();
}

function BuscarOperadorasCIOT() {
    executarReST("ConfiguracaoCIOT/ObterConfiguracoesAtivas", {}, function (r) {
        if (r.Success) {
            if (r.Data) {
                _CIOT.Operadora.options(r.Data.map(function (item) {
                    return {
                        value: item.Codigo.toString(),
                        text: item.Descricao
                    };
                }));

                if (r.Data.length == 1) {
                    _CIOT.Operadora.def = r.Data[0].Codigo.toString();
                    _CIOT.Operadora.val(_CIOT.Operadora.def);
                }

                _CRUDCIOT.BuscarConciliacaoCIOT.visible(true);

            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function EditarCIOT(ciotGrid, cb) {
    LimparCamposCIOT();

    _CIOT.Codigo.val(ciotGrid.Codigo);

    BuscarCIOTPorCodigo(null, cb);
}

function BuscarCIOTPorCodigo(exibirLoading, cb) {
    BuscarPorCodigo(_CIOT, "CIOT/BuscarPorCodigo", function (arg) {
        PreecherResumoCIOT(arg.Data);
        PreecherCamposEdicaoCIOT();

        if (arg.Data.PossuiIntegracaoAX && arg.Data.Situacao == EnumSituacaoCIOT.Encerrado)
            _CRUDCIOT.EnviarIntegracaoQuitacaoAX.visible(true);
        else
            _CRUDCIOT.EnviarIntegracaoQuitacaoAX.visible(false);
    }, null, exibirLoading);
}

function PreecherCamposEdicaoCIOT() {
    _pesquisaCIOT.ExibirFiltros.visibleFade(false);

    _CIOT.DataFinalViagem.enable(false);
    _CIOT.Transportador.enable(false);
    _CIOT.Empresa.enable(false);
    _CIOT.Veiculo.enable(false);
    _CIOT.Motorista.enable(false);
    _CIOT.Operadora.enable(false);
    _CRUDCIOT.Abrir.visible(false);
    _CIOT.HistoricoIntegracao.visible(true);

    if (_CIOT.Situacao.val() !== EnumSituacaoCIOT.Aberto) {
        _CRUDCIOT.Encerrar.visible(false);
        _CRUDCIOT.Cancelar.visible(true);
        _CRUDCIOT.Interromper.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador);

        if (_CIOT.Situacao.val() === EnumSituacaoCIOT.Encerrado || _CIOT.Situacao.val() === EnumSituacaoCIOT.PagamentoAutorizado)
            _CRUDCIOT.ImprimirContrato.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador);
    } else {
        _CRUDCIOT.Encerrar.visible(true);
        _CRUDCIOT.Cancelar.visible(true);
        _CRUDCIOT.Interromper.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador);
        _CRUDCIOT.ImprimirContrato.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador);

        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CIOT_EncerrarGerencialmente, _PermissoesPersonalizadas))
            _CRUDCIOT.EncerrarGerencialmente.visible(true);
    }

    SetarEtapaCIOT();
}

function LimparCamposCIOT() {
    _CIOT.HistoricoIntegracao.visible(false);
    _CIOT.DataFinalViagem.enable(true);
    _CIOT.Transportador.enable(true);
    _CIOT.Empresa.enable(true);
    _CIOT.Veiculo.enable(true);
    _CIOT.Motorista.enable(true);
    _CIOT.Operadora.enable(true);

    _CRUDCIOT.Abrir.visible(true);
    _CRUDCIOT.Encerrar.visible(false);
    _CRUDCIOT.EncerrarGerencialmente.visible(false);
    _CRUDCIOT.Cancelar.visible(false);
    _CRUDCIOT.Interromper.visible(false);
    _CRUDCIOT.ImprimirContrato.visible(false);
    _CRUDCIOT.EnviarIntegracaoQuitacaoAX.visible(false);

    LimparCampos(_CIOT);
    LimparCamposCTe();
    LimparResumoCIOT();

    SetarEtapaInicioCIOT();
}