//*******MAPEAMENTO*******

var _HTMLCargaPedidoDocumentoCTe;
var _cargaPedidoDocumentoCTe;
var _gridCargaPedidoDocumentosCTe;
var _gridCargaCTesTransbordo;

var CargaPedidoDocumentoCTe = function () {
    this.GridDocumentoCTe = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), idBtnSearch: guid(), text: Localization.Resources.Cargas.Carga.AdicionarCTes, cssClass: ko.observable("col-12") });
    this.ImprimirRelacaoEntrega = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.ImprimirRelacaoDeEntrega, eventClick: ImprimirRelacaoEntregaClick, visible: ko.observable(false), icon: "fal fa-print" });
    this.ImprimirRelacaoSeparacaoVolume = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.ImprimirRelacaoDeSeparacaoDeVolume, eventClick: ImprimirRelacaoSeparacaoVolumeClick, visible: ko.observable(false), icon: "fal fa-print" });

    this.CargaPedido = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idTab: guid(), enable: ko.observable(true) });
    this.CTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.AdicionarCTe, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    //this.ConfiguracaoEmissaoCTe = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "" });
    this.BuscarCTesCompativeis = PropertyEntity({ eventClick: buscarCTesCompativeisClick, type: types.event, text: Localization.Resources.Cargas.Carga.BuscarCTesCompativeis, visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverTodosCTes = PropertyEntity({ eventClick: ExcluirTodosDocumentosCTeClick, type: types.event, text: Localization.Resources.Cargas.Carga.RemoverTodos, visible: ko.observable(true), enable: ko.observable(true) });

    this.GridCTesParaTransbordo = PropertyEntity({ val: ko.observable(0), def: 0, idGrid: guid(), idBtnSearch: guid(), visible: ko.observable(false) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.DescricaoCarga, idBtnSearch: guid(), enable: ko.observable(true) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.Carga.NumeroPedidoEmbarcador.getFieldDescription(), maxlength: 50, visible: ko.observable(true), enable: ko.observable(false) });
    this.NumeroNotaFiscal = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.NotaFiscal.getFieldDescription(), maxlength: 12, visible: ko.observable(true), enable: ko.observable(false) });
    this.NumeroCTe = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.Documento.getFieldDescription(), maxlength: 12, visible: ko.observable(true), enable: ko.observable(false) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: Localization.Resources.Cargas.Carga.MarcarTodas, visible: ko.observable(true), icon: "fal fa-check-square" });
    this.Confirmar = PropertyEntity({ type: types.event, text: Localization.Resources.Cargas.Carga.ConfirmarSelecao, eventClick: confirmarCTesTransbordoClick, visible: ko.observable(false), icon: "fal fa-chevron-down" });
    this.Pesquisar = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, eventClick: PesquisarCTesTransbordoClick, visible: ko.observable(true), enable: ko.observable(false), icon: "fal fa-search" });

    this.DocumentoCTeNFe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.ChaveDoDocumento.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), eventClick: ChangeDocumentoCTeNFe, required: false, enable: ko.observable(true) });
    this.EtiquetaVolume = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.Carga.EtiquetaDoVolume.getFieldDescription(), idBtnSearch: guid(), visible: ko.observable(true), eventClick: ChangeEtiquetaVolume, required: false, enable: ko.observable(true) });

    this.titulo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.CTesEmitidosPeloEmbarcadorVinculadosCarga) });

    this.ValorTotalMercadoria = PropertyEntity({ getType: typesKnockout.int, text: Localization.Resources.Cargas.Carga.ValorTotalDaMercadoria.getFieldDescription(), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });
    this.CargaSVM = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.TituloParaTransbordo = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.Carga.CTesParaTransbordo) });
    this.ConsultaInicial = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });

    this.Buscar = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Buscar) });
    this.Adicionar = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar) });

}

function loadCargaPedidoDocumentoCTe(indice, callback) {

    var ENTER_KEY = 13;
    $("#divDocumentosCTe_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao").html(_HTMLCargaPedidoDocumentoCTe.replace(/#knoutCargaPedidoDocumentosCTe/g, (_cargaAtual.EtapaNotaFiscal.idGrid + "_knoutCargaPedidoDocumentosCTe")));

    _cargaPedidoDocumentoCTe = new CargaPedidoDocumentoCTe();
    KoBindings(_cargaPedidoDocumentoCTe, _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutCargaPedidoDocumentosCTe");

    LocalizeCurrentPage();

    //new BuscarCTesSemCarga(_cargaPedidoDocumentoCTe.CTe, adicionarCargaPedidoDocumentoCTe);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: Localization.Resources.Cargas.Carga.Numero, width: "80%" }
    ];

    let gridCTesSemCargaSelecao = new BasicDataTable(_cargaPedidoDocumentoCTe.CTe.idGrid, header, null, { column: 0, dir: orderDir.asc });

    BuscarCTesSemCarga(_cargaPedidoDocumentoCTe.CTe, adicionarCargaPedidoDocumentoCTe, gridCTesSemCargaSelecao);

    _cargaPedidoDocumentoCTe.CTe.basicTable = gridCTesSemCargaSelecao;
    gridCTesSemCargaSelecao.CarregarGrid([]);

    //new BuscarDocumentosFaturamentoParaFatura(_documentoFatura.Documento, AdicionarDocumentosFatura, _gridDocumentosFaturaSelecao, _fatura.Codigo);

    //_documentoFatura.Documento.basicTable = _gridDocumentosFaturaSelecao;
    //_gridDocumentosFaturaSelecao.CarregarGrid([]);

    new BuscarCargaFinalizadas(_cargaPedidoDocumentoCTe.Carga);

    if (_pedidoConsultaDocumento.PedidoTransbordo.val()) {

        if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe)
            _cargaPedidoDocumentoCTe.GridCTesParaTransbordo.visible(true);

        _cargaPedidoDocumentoCTe.GridDocumentoCTe.cssClass("col-12 col-md-6");
        $("#divConteudoEsquerda_" + _cargaAtual.EtapaNotaFiscal.idGrid + "_knoutDocumentosParaEmissao").attr("class", "col-12")

        if (_cargaAtual.CargaSVM.val()) {
            _cargaPedidoDocumentoCTe.titulo.text("CT-es Multi-Modal que serão associados");
            _cargaPedidoDocumentoCTe.TituloParaTransbordo.text("CT-es Multi-Modal para associação");
        }
    }

    if (_cargaAtual.SituacaoCarga.val() == EnumSituacoesCarga.AgNFe) {
        _cargaPedidoDocumentoCTe.BuscarCTesCompativeis.enable(true);
        _cargaPedidoDocumentoCTe.RemoverTodosCTes.enable(true);
    }
    else {
        _cargaPedidoDocumentoCTe.RemoverTodosCTes.enable(false);
        _cargaPedidoDocumentoCTe.BuscarCTesCompativeis.enable(false);
        _cargaPedidoDocumentoCTe.Carga.enable(false);
        _cargaPedidoDocumentoCTe.DocumentoCTeNFe.enable(false);
        _cargaPedidoDocumentoCTe.EtiquetaVolume.enable(false);
        _cargaPedidoDocumentoCTe.NumeroNotaFiscal.enable(false);
        _cargaPedidoDocumentoCTe.NumeroCTe.enable(false);
        _cargaPedidoDocumentoCTe.NumeroPedidoEmbarcador.enable(false);
        _cargaPedidoDocumentoCTe.Pesquisar.enable(false);
    }

    _cargaPedidoDocumentoCTe.CargaSVM.val(_cargaAtual.CargaSVM.val());

    var baixarDACTE = { descricao: Localization.Resources.Cargas.Carga.BaixarDacte, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarPDF = { descricao: Localization.Resources.Cargas.Carga.BaixarPdf, id: guid(), metodo: baixarDacteClick, icone: "", visibilidade: VisibilidadeDownloadOutrosDoc };
    var baixarXML = { descricao: Localization.Resources.Cargas.Carga.BaixarXml, id: guid(), metodo: baixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var excluir = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: excluirDocumentoCTe, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF, excluir] };

    if (!VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Carga_InformarDocumentosFiscais, _PermissoesPersonalizadasCarga) || _FormularioSomenteLeitura || !_cargaPedidoDocumentoCTe.BuscarCTesCompativeis.enable()) {
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [baixarDACTE, baixarXML, baixarPDF] };

        _cargaPedidoDocumentoCTe.CTe.enable(false);
        _cargaPedidoDocumentoCTe.RemoverTodosCTes.enable(false);
        _cargaPedidoDocumentoCTe.BuscarCTesCompativeis.enable(false);
        _cargaPedidoDocumentoCTe.Carga.enable(false);
        _cargaPedidoDocumentoCTe.DocumentoCTeNFe.enable(false);
        _cargaPedidoDocumentoCTe.EtiquetaVolume.enable(false);
        _cargaPedidoDocumentoCTe.NumeroNotaFiscal.enable(false);
        _cargaPedidoDocumentoCTe.NumeroCTe.enable(false);
        _cargaPedidoDocumentoCTe.NumeroPedidoEmbarcador.enable(false);
        _cargaPedidoDocumentoCTe.Pesquisar.enable(false);
    }

    var editarColuna = { permite: true, callback: callbackEditarColuna, atualizarRow: true };

    _gridCargaPedidoDocumentosCTe = new GridView(_cargaPedidoDocumentoCTe.GridDocumentoCTe.idGrid, "CargaPedidoDocumentoCTe/Consultar", _cargaPedidoDocumentoCTe, menuOpcoes, { column: 6, dir: orderDir.asc }, null, null, null, null, null, null, editarColuna);

    buscarCargaPedidoDocumentoCTe(indice, callback);

    _cargaPedidoDocumentoCTe.DocumentoCTeNFe.get$()
        .on("change", ChangeDocumentoCTeNFe)
        .on("keydown", function (e) {
            var key = e.which || e.keyCode || 0;
            if (key == ENTER_KEY) ChangeDocumentoCTeNFe();
        });

    _cargaPedidoDocumentoCTe.EtiquetaVolume.get$()
        .on("change", ChangeEtiquetaVolume)
        .on("keydown", function (e) {
            var key = e.which || e.keyCode || 0;
            if (key == ENTER_KEY) ChangeEtiquetaVolume();
        });    

    if (_CONFIGURACAO_TMS.PossuiWMS === true) {
        _cargaPedidoDocumentoCTe.ImprimirRelacaoEntrega.visible(false);
        _cargaPedidoDocumentoCTe.ImprimirRelacaoSeparacaoVolume.visible(true);
    }

    buscarCTesTransbordo();
}

function callbackEditarColuna(dataRow, row, head, callbackTabPress) {
    var data = {
        Codigo: dataRow.Codigo,
        Ordem: dataRow.Ordem,
        CodigoCargaPedido: dataRow.CodigoCargaPedido
    };

    executarReST("CargaPedidoDocumentoCTe/AlterarOrdem", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
                //CompararEAtualizarGridEditableDataRow(dataRow, arg.Data)
                //_gridCargaPedidoDocumentosCTe.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                ExibirErroDataRow(row, arg.Msg, tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso);
            }
        } else {
            ExibirErroDataRow(row, arg.Msg, tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha);
        }
    });
}

function buscarCTesTransbordo() {

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _cargaPedidoDocumentoCTe.SelecionarTodos,
        somenteLeitura: false
    }

    _cargaPedidoDocumentoCTe.ConsultaInicial.val(true);

    _gridCargaCTesTransbordo = new GridView(_cargaPedidoDocumentoCTe.GridCTesParaTransbordo.idGrid, "CargaPedidoDocumentoCTe/ConsultarCTesParaTransbordo", _cargaPedidoDocumentoCTe, null, null, 10, null, null, null, multiplaescolha);
    _gridCargaCTesTransbordo.CarregarGrid(function () {
        _cargaPedidoDocumentoCTe.Confirmar.visible(true);
        _cargaPedidoDocumentoCTe.ConsultaInicial.val(false);
    });
}

function buscarCargaPedidoDocumentoCTe(indice, callback) {
    if (indice != null) {
        _cargaPedidoDocumentoCTe.CargaPedido.val(_cargaAtual.Pedidos.val[indice].CodigoCargaPedido);
        _gridCargaPedidoDocumentosCTe.CarregarGrid(callback);

        ObterDadosGeraisCargaPedidoDocumentoCTe();
    }

    if (callback != null)
        callback();

}

function adicionarCargaPedidoDocumentoCTe(ctes) {

    let codigosCTes = ctes.map(o => o.Codigo);

    let data = { CodigosCTes: JSON.stringify(codigosCTes), CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoCTe/Adicionar", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function confirmarCTesTransbordoClick(e) {
    var data = { CargaPesquisada: _cargaPedidoDocumentoCTe.Carga.codEntity(), SelecionarTodos: _cargaPedidoDocumentoCTe.SelecionarTodos.val(), CTesSelecionados: JSON.stringify(_gridCargaCTesTransbordo.ObterMultiplosSelecionados()), CTesNaoSelecionados: JSON.stringify(_gridCargaCTesTransbordo.ObterMultiplosNaoSelecionados()), CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };
    executarReST("CargaPedidoDocumentoCTe/AdicionarCTesParaTransbordo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridCargaPedidoDocumentosCTe.CarregarGrid();

                ObterDadosGeraisCargaPedidoDocumentoCTe();

                _cargaPedidoDocumentoCTe.Carga.val("");
                _cargaPedidoDocumentoCTe.Carga.codEntity(0);
                _cargaPedidoDocumentoCTe.NumeroNotaFiscal.val("");
                _cargaPedidoDocumentoCTe.NumeroCTe.val("");
                _cargaPedidoDocumentoCTe.NumeroPedidoEmbarcador.val("");

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTesVinculadosCargaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ImprimirRelacaoSeparacaoVolumeClick(e) {
    var data = { CargaPesquisada: _cargaPedidoDocumentoCTe.Carga.codEntity(), CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };
    executarReST("CargaPedidoDocumentoCTe/ImprimirRelacaoSeparacaoVolume", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AguardeQueSeuRelatorioEstaSendoGerado);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    });
}

function ImprimirRelacaoEntregaClick(e) {
    var data = { CargaPesquisada: _cargaPedidoDocumentoCTe.Carga.codEntity(), CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };
    executarReST("CargaPedidoDocumentoCTe/ImprimirRelacaoEntrega", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                BuscarProcessamentosPendentes();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.AguardeQueSeuRelatorioEstaSendoGerado);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
        }
    })
}

function PesquisarCTesTransbordoClick() {
    _gridCargaCTesTransbordo.CarregarGrid();
}

function excluirDocumentoCTe(cte) {
    var data = { CodigoCTe: cte.CodigoCTE, CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoCTe/RemoverCTe", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
                ObterDadosGeraisCargaPedidoDocumentoCTe();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ExcluirTodosDocumentosCTeClick() {

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.Carga.DesejaRealmenteRemoverTodosOsCTes, function () {

        let data = { CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };

        executarReST("CargaPedidoDocumentoCTe/RemoverTodos", data, function (arg) {
            if (arg.Success) {
                if (arg.Data != false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.CTesRemovidosComSucesso);
                    _gridCargaPedidoDocumentosCTe.CarregarGrid();
                    ObterDadosGeraisCargaPedidoDocumentoCTe();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });

    });
}

function buscarCTesCompativeisClick(e) {
    var data = { CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoCTe/BuscarCTesCompativeis", data, function (arg) {
        if (arg.Success) {
            if (arg.Data != false) {
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ChangeEtiquetaVolume() {
    setTimeout(function () {
        if (_cargaPedidoDocumentoCTe.EtiquetaVolume.val() != "")
            BuscarEtiquetaVolume(_cargaPedidoDocumentoCTe.EtiquetaVolume.val());
    }, 100);
}

function BuscarEtiquetaVolume(etiqueta) {
    var data = { CargaPesquisada: _cargaPedidoDocumentoCTe.Carga.codEntity(), Etiqueta: etiqueta.trim(), CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };
    executarReST("CargaPedidoDocumentoCTe/AdicionarEtiquetaVolumeParaTransbordo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
                _cargaPedidoDocumentoCTe.EtiquetaVolume.val("");
                _cargaPedidoDocumentoCTe.EtiquetaVolume.codEntity(0);
                _cargaPedidoDocumentoCTe.EtiquetaVolume.get$().focus();
                ObterDadosGeraisCargaPedidoDocumentoCTe();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentoVinculadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ChangeDocumentoCTeNFe() {
    setTimeout(function () {
        if (_cargaPedidoDocumentoCTe.DocumentoCTeNFe.val() != "")
            BuscarDocumentoCTeNFe(_cargaPedidoDocumentoCTe.DocumentoCTeNFe.val());
    }, 100);
}

function BuscarDocumentoCTeNFe(documento) {
    var data = { CargaPesquisada: _cargaPedidoDocumentoCTe.Carga.codEntity(), Documento: documento.trim(), CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };
    executarReST("CargaPedidoDocumentoCTe/AdicionarDocumentoParaTransbordo", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridCargaPedidoDocumentosCTe.CarregarGrid();
                _cargaPedidoDocumentoCTe.DocumentoCTeNFe.val("");
                _cargaPedidoDocumentoCTe.DocumentoCTeNFe.codEntity(0);
                _cargaPedidoDocumentoCTe.DocumentoCTeNFe.get$().focus();
                ObterDadosGeraisCargaPedidoDocumentoCTe();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.Carga.DocumentoVinculadoComSucesso);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function ObterDadosGeraisCargaPedidoDocumentoCTe(cte) {
    var data = { CargaPedido: _cargaPedidoDocumentoCTe.CargaPedido.val() };

    executarReST("CargaPedidoDocumentoCTe/ObterDadosGerais", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _cargaPedidoDocumentoCTe.ValorTotalMercadoria.val(arg.Data.ValorTotalMercadoria);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}