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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Carga.js" />
/// <reference path="CargaCTe.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="Canhoto.js" />
/// <reference path="../../Consultas/Filial.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _pesquisaCarga;
var _gridCargas;
var _carga;
var _todosMDFsEncerrados = false;
var _todosCanhotosEnviados = false;

var PesquisaCarga = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.getFieldDescription(), visible: ko.observable(false) });
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.getFieldDescription(), visible: ko.observable(true) });
    this.CodigoPedidoEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.NumeroPedido.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Empresa = PropertyEntity({ type: types.multiplesEntities, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Veiculo.getFieldDescription(), idBtnSearch: guid() });
    this.Operador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Operador.getFieldDescription(), idBtnSearch: guid() });
    this.Origem = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Origem.getFieldDescription(), idBtnSearch: guid() });
    this.Destino = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Destino.getFieldDescription(), idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.ModeloVeicularCarga.getFieldDescription(), required: true, idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.TipoCarga.getFieldDescription(), required: true, idBtnSearch: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacoesCarga.EmTransporte), def: EnumSituacoesCarga.EmTransporte });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Remetente.getFieldDescription(), idBtnSearch: guid() });
    this.Destinatario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Destinatario.getFieldDescription(), idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroMDFe = PropertyEntity({ getType: typesKnockout.int, text: "MDF-e:" });
    this.NumeroNF = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.EncerramentoCarga.NotaFiscal.getFieldDescription() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.TipoOperacao.getFieldDescription(), idBtnSearch: guid() });
    this.ModeloDocumento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.ModeloDocumento.getFieldDescription(), idBtnSearch: guid() });
    this.ApenasMDFeEncerrados = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.EncerramentoCarga.ApenasMDFeEncerrados });

    this.CargasParaEncerramento = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), def: true });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            buscarCargas();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltrosPesquisa, idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
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
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Avancada, idFade: guid(), icon: ko.observable("fal fa-plus"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Carga = function () {
    this.Descricao = PropertyEntity({ type: types.map });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idGrid: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Cargas.EncerramentoCarga.Transportador.getFieldDescription(), idBtnSearch: guid() });
    this.ValorFrete = PropertyEntity({ type: types.map, text: Localization.Resources.Gerais.Geral.ValorFrete.getFieldDescription(), required: false, idGrid: guid() });
    this.Motorista = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Gerais.Geral.Motorista.getFieldDescription(), idBtnSearch: guid() });
    this.Veiculo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Cargas.EncerramentoCarga.Veiculo.getFieldDescription(), idBtnSearch: guid() });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Cargas.EncerramentoCarga.TipoCarga.getFieldDescription(), idBtnSearch: guid() });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Cargas.EncerramentoCarga.ModeloVeicularCarga.getFieldDescription(), idBtnSearch: guid() });
    this.Peso = PropertyEntity({ type: types.map, codEntity: ko.observable(""), required: false, text: Localization.Resources.Cargas.EncerramentoCarga.Peso.getFieldDescription(), idGrid: guid() });
    this.SituacaoCarga = PropertyEntity({ type: types.map, required: false, idBtnSearch: guid() });

    this.BuscarNovamenteMDFe = PropertyEntity({ eventClick: buscarNovamenteMDFeClick, type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.AtualizarBuscarMDFe, visible: ko.observable(true) });
    this.Encerrar = PropertyEntity({ eventClick: encerrarCargaClick, enable: ko.observable(true), type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.EncerrarCarga, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });

    this.SelecionarArquivo = PropertyEntity({ eventClick: null, type: types.event, text: Localization.Resources.Gerais.Geral.SelecionarCanhotos, visible: ko.observable(true), idGrid: guid() });
    this.EnviarCanhoto = PropertyEntity({ eventClick: enviarCanhotoClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.EnviarCanhoto, visible: ko.observable(true) });

    this.Canhoto = PropertyEntity({ type: types.local, idGrid: guid() });
};

var EncerramentoMDFe = function () {
    this.Codigo = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.CodigoCarga.getFieldDescription() });
    this.Localidade = PropertyEntity({ val: ko.observable("0"), options: null, def: "0", text: Localization.Resources.Gerais.Geral.Municipio.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.Estado = PropertyEntity({ type: types.local, enable: ko.observable(false), text: Localization.Resources.Gerais.Geral.Estado.getFieldDescription() });
    this.DataEncerramento = PropertyEntity({ type: types.map, getType: typesKnockout.date, text: Localization.Resources.Cargas.EncerramentoCarga.DataEncerramento.getFieldDescription(), required: true, idBtnSearch: guid() });
    this.HoraEncerramento = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.EncerramentoCarga.HoraEncerramento.getFieldDescription(), required: true, idBtnSearch: guid() });
    this.EncerrarMDFe = PropertyEntity({ eventClick: encerrarMDFeClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.EncerrarMDFe, visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadEncerramentoCarga() {

    _pesquisaCarga = new PesquisaCarga();
    _carga = new Carga();

    KoBindings(_pesquisaCarga, "knockoutPesquisaEncerramentoCarga", false, _pesquisaCarga.Pesquisar.id);
    KoBindings(_carga, "knockoutCadastroEncerramentoCarga");

    HeaderAuditoria("CargaRegistroEncerramento", _carga);

    new BuscarTransportadores(_pesquisaCarga.Empresa);
    new BuscarVeiculos(_pesquisaCarga.Veiculo);
    new BuscarTiposdeCarga(_pesquisaCarga.TipoCarga);
    new BuscarModelosVeicularesCarga(_pesquisaCarga.ModeloVeicularCarga);
    new BuscarClientes(_pesquisaCarga.Remetente);
    new BuscarClientes(_pesquisaCarga.Destinatario);
    new BuscarFilial(_pesquisaCarga.Filial);
    new BuscarOperador(_pesquisaCarga.Operador);
    new BuscarLocalidades(_pesquisaCarga.Origem, Localization.Resources.Cargas.EncerramentoCarga.BuscarCidadeOrigem, Localization.Resources.Gerais.Geral.CidadesOrigem);
    new BuscarLocalidades(_pesquisaCarga.Destino, Localization.Resources.Cargas.EncerramentoCarga.BuscarCidadeDestino, Localization.Resources.Gerais.Geral.CidadesDestino);
    new BuscarTiposOperacao(_pesquisaCarga.TipoOperacao);
    new BuscarModeloDocumentoFiscal(_pesquisaCarga.ModeloDocumento, null, null, null, true, null, true);

    buscarCargas();
}


function buscarNovamenteMDFeClick(e, sender) {
    buscarMDFe();
}

function encerrarMDFeClick(e, sender) {

    if (ValidarCamposObrigatorios(e)) {
        var dados = {
            Codigo: e.Codigo.val(),
            Carga: _carga.Codigo.val(),
            Localidade: e.Localidade.val(),
            DataEncerramento: e.DataEncerramento.val(),
            HoraEncerramento: e.HoraEncerramento.val()
        }
        executarReST("CargaMDFe/EncerrarMDFe", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.EncerramentoCarga.SolicitacaoRealizada, Localization.Resources.Cargas.EncerramentoCarga.SolicitacaoEncerramentoMDFEnviadaComSucesso);
                    buscarMDFe();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
                Global.fecharModal('divModalEncerramentoMDFe');
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falaha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios);
    }
}

function encerrarCargaClick(e, sender) {

    exibirConfirmacao(Localization.Resources.Gerais.Confirmacao, Localization.Resources.Cargas.EncerramentoCarga.DesejaRealmenteEncerrarCarga, function () {
        var dados = { Carga: _carga.Codigo.val() };
        executarReST("EncerramentoCarga/EncerrarCarga", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.EncerramentoCarga.CargaEncerradaSucesso);
                    LimparCampos(_pesquisaCarga);
                    LimparCamposEncerramento();
                    buscarCargas();
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falaha, arg.Msg);
            }
        });
    });
}

function cancelarClick(e, sender) {
    LimparCampos(_pesquisaCarga);
    LimparCamposEncerramento();
    buscarCargas();
}


function abrirMotalEncerrarMDFeClick(e) {

    var dados = { Codigo: e.Codigo };

    if (e.MDFeAutorizado) {
        executarReST("CargaMDFe/BuscarDadosParaEncerramentoPorCodigo", dados, function (arg) {
            if (arg.Success) {
                var dadosEncerramento = arg.Data;
                var encerramentoMDFe = new EncerramentoMDFe();
                $("#" + encerramentoMDFe.DataEncerramento.id).mask("00:00", { selectOnFocus: true, clearIfNotMatch: true });
                encerramentoMDFe.Codigo.val(dadosEncerramento.Codigo);
                encerramentoMDFe.Localidade.options = dadosEncerramento.Localidades;
                encerramentoMDFe.DataEncerramento.val(dadosEncerramento.DataEncerramento);
                encerramentoMDFe.HoraEncerramento.val(dadosEncerramento.HoraEncerramento);
                encerramentoMDFe.Estado.val(dadosEncerramento.Estado);
                encerramentoMDFe.Localidade.val(dadosEncerramento.Localidades[0].Codigo);
                KoBindings(encerramentoMDFe, "knoutEncerramentoMDFe");
                                
                Global.abrirModal("divModalEncerramentoMDFe");
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falaha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.EncerramentoCarga.AtualSituacaoMFDNaoPermiteSeuEncerramento.format(e.Status));
    }
}


function selecionarCargaClick(e) {
    _carga.Codigo.val(e.Codigo);
    BuscarPorCodigo(_carga, "Carga/BuscarPorCodigo", function (arg) {
        buscarMDFe();
        //loadCanhoto(PaginaOrigem.encerrarCarga);
    }, null);
}



//*******MÉTODOS*******

function buscarCargas() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Selecionar, id: "clasEditar", evento: "onclick", metodo: selecionarCargaClick, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridCargas = new GridView(_pesquisaCarga.Pesquisar.idGrid, "Carga/PesquisaCargasPorSituacao", _pesquisaCarga, menuOpcoes, null, null, function (e) {
        _pesquisaCarga.BuscaAvancada.visibleFade(false);
    });
    _gridCargas.CarregarGrid();
}



function buscarMDFe() {
    var dados = { Carga: _carga.Codigo.val() };
    executarReST("CargaMDFe/BuscarMDFeCarga", dados, function (arg) {

        if (arg.Success) {
            $("#wid-id-4").show();
            _pesquisaCarga.ExibirFiltros.visibleFade(false);
            if (arg.Data.MDFEs != null && arg.Data.MDFEs.length > 0) {
                $("#contentMDFInterEstadual").hide();
                $("#contentMDF").show();

                var opcoes = [];
                if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarMDFe, _PermissoesPersonalizadasEncerramentoCarga)) {
                    opcoes.push({ descricao: Localization.Resources.Gerais.Geral.EncerrarMDFe, id: guid(), metodo: abrirMotalEncerrarMDFeClick });
                }
                var auditar = { descricao: Localization.Resources.Gerais.Geral.Auditar, id: guid(), metodo: OpcaoAuditoria("ManifestoEletronicoDeDocumentosFiscais", "CodigoMDFE"), icone: "", visibilidade: VisibilidadeOpcaoAuditoria };
                opcoes.push(auditar);

                var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: opcoes };
                var header = [{ data: "CodigoMDFE", visible: false },
                { data: "CodigoEmpresa", visible: false },
                { data: "MDFeAutorizado", visible: false },
                { data: "Numero", title: Localization.Resources.Gerais.Geral.Numero, width: "10%", className: "text-align-center", orderable: false },
                { data: "Serie", title: Localization.Resources.Gerais.Geral.Serie, width: "8%", className: "text-align-center", orderable: false },
                { data: "Emissao", title: Localization.Resources.Cargas.EncerramentoCarga.DataEmissao, width: "10%", className: "text-align-center", orderable: false },
                { data: "UFCarga", title: Localization.Resources.Cargas.EncerramentoCarga.UFCarga, width: "20%" },
                { data: "UFDesgarga", title: Localization.Resources.Cargas.EncerramentoCarga.UDescarga, width: "20%" },
                { data: "Status", title: Localization.Resources.Cargas.EncerramentoCarga.Status, width: "10%", className: "text-align-center", orderable: false },
                { data: "RetornoSefaz", title: Localization.Resources.Cargas.EncerramentoCarga.RetornoSefaz, width: "15%", orderable: false }
                ];
                var gridMDFe = new BasicDataTable("tableMDFE", header, menuOpcoes);
                gridMDFe.CarregarGrid(arg.Data.MDFEs);

                if (arg.Data.MDFeEncerrados) {
                    _todosMDFsEncerrados = true;
                } else {
                    _todosMDFsEncerrados = false;
                }
            } else {
                if (arg.Data.MDFEsManual != null && arg.Data.MDFEsManual.length > 0) {
                    $("#contentMDFInterEstadual").html(Localization.Resources.Cargas.EncerramentoCarga.MFDDestaCargaNaoFoiEmitidoPeloMultiEmbarcador);
                }
                _todosMDFsEncerrados = true;
                $("#contentMDFInterEstadual").show();
                $("#contentMDF").hide();
            }
            VerificarEncerramentoLiberado();
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falaha, arg.Msg);
        }

    });
}

function VerificarEncerramentoLiberado() {
    var permiteEncerrarCarga = _CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.EncerramentoCarga_EncerrarCarga, _PermissoesPersonalizadasEncerramentoCarga);
    _carga.Encerrar.visible(permiteEncerrarCarga);
}

function LimparCamposEncerramento() {
    LimparCampos(_carga);
    $("#wid-id-4").hide();
    _pesquisaCarga.ExibirFiltros.visibleFade(true);
}
