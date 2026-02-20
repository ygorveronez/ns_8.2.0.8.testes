//*******MAPEAMENTO*******

var _HTMLCargaPedidoDocumentoMDFe;
var _cargaPedidoDocumentoMDFe;
var _gridCargaPedidoDocumentosMDFe;

var CargaPedidoDocumentoMDFe = function () {
    this.GridDocumentoMDFe = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), idBtnSearch: guid(), text: Localization.Resources.Cargas.Carga.AdicionarMDFes, cssClass: ko.observable("col-12") });
    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.MDFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.AdicionarMDFe, idBtnSearch: guid(), enable: ko.observable(true) });
    this.BuscarMDFesCompativeis = PropertyEntity({ eventClick: BuscarMDFesCompativeisClick, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarMDFesCompativeis, visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverTodosMDFes = PropertyEntity({ eventClick: ExcluirTodosDocumentosMDFeClick, type: types.event, text: Localization.Resources.Cargas.Carga.RemoverTodos, visible: ko.observable(true), enable: ko.observable(true) });

    this.Titulo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.MDFesEmitidosPeloEmbarcadorVinculadosCarga) });
}

function LoadCargaPedidoDocumentoMDFe(indice, callback) {
    if (!_pedidoConsultaDocumento.PedidoTransbordo.val()) {

        $("#divDocumentosCTe_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao").append(_HTMLCargaPedidoDocumentoMDFe.replace(/#knoutCargaPedidoDocumentosMDFe/g, (_cargaAtual.EtapaNotaFiscal.idGrid + "_knoutCargaPedidoDocumentosMDFe")));

        _cargaPedidoDocumentoMDFe = new CargaPedidoDocumentoMDFe();
        KoBindings(_cargaPedidoDocumentoMDFe, _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutCargaPedidoDocumentosMDFe");

        LocalizeCurrentPage();

        new BuscarMDFesSemCarga(_cargaPedidoDocumentoMDFe.MDFe, AdicionarCargaPedidoDocumentoMDFe);

        BuscarCargaPedidoDocumentoMDFe(indice, callback);

    }
}

function BuscarCargaPedidoDocumentoMDFe(indice, callback) {
    let baixarDAMDFe = { descricao: Localization.Resources.Cargas.Carga.BaixarDamdfe, id: guid(), metodo: baixarDAMDFeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMDFe };
    let baixarXML = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLMDFeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadMDFe };
    let excluir = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: ExcluirCargaPedidoDocumentoMDFe, icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDAMDFe, baixarXML, excluir] };

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga) || _FormularioSomenteLeitura || _cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe) {
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDAMDFe, baixarXML] };

        _cargaPedidoDocumentoMDFe.MDFe.enable(false);
        _cargaPedidoDocumentoMDFe.BuscarMDFesCompativeis.enable(false);
    }

    _gridCargaPedidoDocumentosMDFe = new GridView(_cargaPedidoDocumentoMDFe.GridDocumentoMDFe.idGrid, "CargaPedidoDocumentoMDFe/Consultar", _cargaPedidoDocumentoMDFe, menuOpcoes, null);

    if (indice != null) {
        _cargaPedidoDocumentoMDFe.CargaPedido.val(_cargaAtual.Pedidos.val[indice].CodigoCargaPedido);
        _gridCargaPedidoDocumentosMDFe.CarregarGrid(callback);
    }

    if (callback != null)
        callback();
}

function AdicionarCargaPedidoDocumentoMDFe(mdfe) {
    let data = { CodigoMDFe: mdfe.Codigo, CargaPedido: _cargaPedidoDocumentoMDFe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoMDFe/Adicionar", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosMDFe.CarregarGrid();
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ExcluirCargaPedidoDocumentoMDFe(mdfe) {
    let data = { CodigoMDFe: mdfe.CodigoMDFE, CargaPedido: _cargaPedidoDocumentoMDFe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoMDFe/Remover", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosMDFe.CarregarGrid();
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ExcluirTodosDocumentosMDFeClick() {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverTodosOsMDFes, function () {

        let data = { CargaPedido: _cargaPedidoDocumentoMDFe.CargaPedido.val() };

        executarReST("CargaPedidoDocumentoMDFe/RemoverTodos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.MDFesRemovidosComSucesso);
                    _gridCargaPedidoDocumentosMDFe.CarregarGrid();
                    _gridCargaPedidoDocumentosCTe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    });
}

function BuscarMDFesCompativeisClick(e) {
    let data = { CargaPedido: _cargaPedidoDocumentoMDFe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoMDFe/BuscarMDFesCompativeis", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosMDFe.CarregarGrid();
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function VisibilidadeOpcaoDownloadMDFe(e) {
    if (e.SituacaoMDFe == EnumSituacaoMDFe.Autorizado || e.SituacaoMDFe == EnumSituacaoMDFe.Encerrado || e.SituacaoMDFe == EnumSituacaoMDFe.EmEncerramento)
        return true;
    else
        return false;
}