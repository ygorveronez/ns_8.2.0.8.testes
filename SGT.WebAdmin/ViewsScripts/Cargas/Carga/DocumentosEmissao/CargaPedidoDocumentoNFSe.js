//*******MAPEAMENTO*******

var _HTMLCargaPedidoDocumentoNFSe;
var _cargaPedidoDocumentoNFSe;
var _gridCargaPedidoDocumentosNFSe;

var CargaPedidoDocumentoNFSe = function () {
    this.GridDocumentoNFSe = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), idBtnSearch: guid(), text: "Adicionar MDF-es", cssClass: ko.observable("col-12") });
    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.NFSe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar MDF-e", idBtnSearch: guid(), enable: ko.observable(true) });
    this.BuscarNFSesCompativeis = PropertyEntity({ eventClick: BuscarNFSesCompativeisClick, type: types.event, text: "Buscar MDF-es Compativeis", visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverTodosNFSes = PropertyEntity({ eventClick: ExcluirTodosDocumentosNFSeClick, type: types.event, text: "Remover Todos", visible: ko.observable(true), enable: ko.observable(true) });

    this.Titulo = PropertyEntity({ text: ko.observable("MDF-es Emitidos pelo Embarcador Vinculados à Carga ") });
}

function LoadCargaPedidoDocumentoNFSe(indice, callback) {
    if (_pedidoConsultaDocumento.PedidoTransbordo.val())
        return;

    $("#divDocumentosCTe_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao").append(_HTMLCargaPedidoDocumentoNFSe.replace(/#knoutCargaPedidoDocumentosNFSe/g, (_cargaAtual.EtapaNotaFiscal.idGrid + "_knoutCargaPedidoDocumentosNFSe")));

    _cargaPedidoDocumentoNFSe = new CargaPedidoDocumentoNFSe();
    KoBindings(_cargaPedidoDocumentoNFSe, _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutCargaPedidoDocumentosNFSe");

    new BuscarNFSesSemCarga(_cargaPedidoDocumentoNFSe.NFSe, AdicionarCargaPedidoDocumentoNFSe);
    BuscarCargaPedidoDocumentoNFSe(indice, callback);
}

function BuscarCargaPedidoDocumentoNFSe(indice, callback) {
    let baixarDANFSe = { descricao: "Baixar DANFSe", id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadNFSe };
    let baixarXML = { descricao: "Baixar XML", id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownloadNFSe };
    let excluir = { descricao: "Remover", id: guid(), metodo: ExcluirCargaPedidoDocumentoNFSe, icone: "" };

    let menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDANFSe, baixarXML, excluir] };

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga) || _FormularioSomenteLeitura || _cargaAtual.SituacaoCarga.val() != EnumSituacoesCarga.AgNFe) {
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: "Opções", tamanho: 7, opcoes: [baixarDANFSe, baixarXML] };

        _cargaPedidoDocumentoNFSe.NFSe.enable(false);
        _cargaPedidoDocumentoNFSe.BuscarNFSesCompativeis.enable(false);
    }

    _gridCargaPedidoDocumentosNFSe = new GridView(_cargaPedidoDocumentoNFSe.GridDocumentoNFSe.idGrid, "CargaPedidoDocumentoNFSe/Consultar", _cargaPedidoDocumentoNFSe, menuOpcoes, null);

    if (indice != null) {
        _cargaPedidoDocumentoNFSe.CargaPedido.val(_cargaAtual.Pedidos.val[indice].CodigoCargaPedido);
        _gridCargaPedidoDocumentosNFSe.CarregarGrid(callback);
    }

    if (callback != null)
        callback();
}

function AdicionarCargaPedidoDocumentoNFSe(mdfe) {
    let data = { CodigoNFSe: mdfe.Codigo, CargaPedido: _cargaPedidoDocumentoNFSe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoNFSe/Adicionar", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosNFSe.CarregarGrid();
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ExcluirCargaPedidoDocumentoNFSe(mdfe) {
    let data = { CodigoNFSe: mdfe.CodigoMDFE, CargaPedido: _cargaPedidoDocumentoNFSe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoNFSe/Remover", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosNFSe.CarregarGrid();
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function ExcluirTodosDocumentosNFSeClick() {

    exibirConfirmacao("Atenção!", "Deseja realmente remover todos os MDF-es?", function () {

        let data = { CargaPedido: _cargaPedidoDocumentoNFSe.CargaPedido.val() };

        executarReST("CargaPedidoDocumentoNFSe/RemoverTodos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "MDF-es removidos com sucesso!");
                    _gridCargaPedidoDocumentosNFSe.CarregarGrid();
                    _gridCargaPedidoDocumentosCTe.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });

    });
}

function BuscarNFSesCompativeisClick(e) {
    let data = { CargaPedido: _cargaPedidoDocumentoNFSe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoNFSe/BuscarNFSesCompativeis", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosNFSe.CarregarGrid();
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function VisibilidadeOpcaoDownloadNFSe(e) {
    return ((data.SituacaoCTe == EnumStatusCTe.AUTORIZADO || data.SituacaoCTe == EnumStatusCTe.ANULADO || data.SituacaoCTe == EnumStatusCTe.CANCELADO || data.SituacaoCTe == EnumStatusCTe.FSDA) && data.TipoDocumentoEmissao == EnumTipoDocumentoEmissao.CTe);
}