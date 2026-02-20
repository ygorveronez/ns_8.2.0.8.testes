//*******MAPEAMENTO KNOUCKOUT*******

var _gridDocumentosFaturaSelecao, _documentoFatura, _gridDocumentosFatura, _crudDocumentoFatura, _percentualDocumentoFatura, _modalAcrescimoDescontoDocumentosFatura;

var DocumentoFatura = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Documento = PropertyEntity({ type: types.event, text: "Adicionar Documento", idBtnSearch: guid() });
    this.Fatura = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Serie = PropertyEntity({ text: "Série:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: _CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS ? "Empresa/Filial:" : "Transportador:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable("Carga:"), idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Documentos = PropertyEntity({ type: types.local, idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador), enable: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) });

    this.ImportarPlanilha = PropertyEntity({
        type: types.local,
        text: "Importar Planilha",
        visible: ko.observable(true),
        accept: ".xls,.xlsx,.csv",
        cssClass: "btn-default botaoDentroSmartAdmimForm",
        UrlImportacao: "FaturaDocumento/Importar",
        UrlConfiguracao: "FaturaDocumento/ConfiguracaoImportacao",
        CodigoControleImportacao: EnumCodigoControleImportacao.O006_DocumentoFatura,
        CallbackImportacao: function () {
            _gridDocumentosFatura.CarregarGrid();
        },
        ParametrosRequisicao: function () {
            var parametros = { Fatura: _fatura.Codigo.val() };
            return parametros;
        }
    });

    this.DownloadLoteXML = PropertyEntity({ eventClick: DownloadLoteXMLDocumentosFaturaClick, type: types.event, text: "Baixar Lote de XML", icon: "fal fa-download", visible: ko.observable(true), enable: ko.observable(true) });
    this.DownloadLoteDACTE = PropertyEntity({ eventClick: DownloadLoteDACTEDocumentosFaturaClick, type: types.event, text: "Baixar Lote de XML", icon: "fal fa-download", visible: ko.observable(true), enable: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function () {
            if (_documentoFatura.SelecionarTodos.val() == true) {
                setTimeout(_documentoFatura.SelecionarTodos.eventClick, 100);
            }
            _gridDocumentosFatura.CarregarGrid();
        },
        type: types.event, text: "Pesquisar", icon: "fal fa-search", visible: ko.observable(true), enable: ko.observable(true)
    });
};

var PercentualDocumentoFatura = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var CRUDDocumentoFatura = function () {
    this.ConfirmarDocumentosFatura = PropertyEntity({ eventClick: ConfirmarDocumentoFaturaClick, type: types.event, text: "Confirmar Documentos", icon: "fal fa-chevron-down", visible: ko.observable(true), enable: ko.observable(true) });
    this.AcrescimoDesconto = PropertyEntity({ eventClick: AcrescimoDescontoClick, type: types.event, text: "Acréscimo / Desconto", visible: ko.observable(true), enable: ko.observable(true) });
    this.DarBaixa = PropertyEntity({ eventClick: DarBaixaDocumentosFaturaClick, type: types.event, text: "Dar baixa", visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador), enable: ko.observable(true) });
};

//*******EVENTOS*******

function LoadDocumentoFatura() {

    CarregarHTMLDocumentoFatura().then(function () {
        _documentoFatura = new DocumentoFatura();
        KoBindings(_documentoFatura, "knockoutFaturaDocumento");

        _percentualDocumentoFatura = new PercentualDocumentoFatura();
        KoBindings(_percentualDocumentoFatura, "knockoutPercentualFaturaDocumento");

        _crudDocumentoFatura = new CRUDDocumentoFatura();
        KoBindings(_crudDocumentoFatura, "knockoutCRUDFaturaDocumento");

        new BuscarTransportadores(_documentoFatura.Empresa);
        new BuscarCargas(_documentoFatura.Carga);

        var header = [
            { data: "Codigo", visible: false },
            { data: "Documento", title: "Documento", width: "80%" }
        ];

        _gridDocumentosFaturaSelecao = new BasicDataTable(_documentoFatura.Grid.id, header, null, { column: 0, dir: orderDir.asc });

        new BuscarDocumentosFaturamentoParaFatura(_documentoFatura.Documento, AdicionarDocumentosFatura, _gridDocumentosFaturaSelecao, _fatura.Codigo, _CONFIGURACAO_TMS.AtivarColunaNumeroContainerConsultaDocumentosFatura);

        _documentoFatura.Documento.basicTable = _gridDocumentosFaturaSelecao;
        _gridDocumentosFaturaSelecao.CarregarGrid([]);

        LoadAcrescimoDescontoDocumentoFatura();

        BuscarDocumentosFatura();

        if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_BloquearAcrescimoDesconto, _PermissoesPersonalizadas))
            _crudDocumentoFatura.AcrescimoDesconto.visible(false);
        else
            _crudDocumentoFatura.AcrescimoDesconto.visible(true);
    
    });
}

function AcrescimoDescontoClick(e, sender) {
    limparCamposAcrescimoDescontoDocumentos();
    _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.val(_fechamentoFatura.TipoMoeda.val());

    if (_acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.val() !== null && _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.val() !== undefined && _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.val() !== EnumMoedaCotacaoBancoCentral.Real) {
        _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.visible(true);
        _acrescimoDescontoDocumentos.DataBaseCRT.visible(true);
        _acrescimoDescontoDocumentos.ValorMoedaCotacao.visible(true);
        _acrescimoDescontoDocumentos.ValorOriginalMoedaEstrangeira.visible(true);
    }
    else {
        _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.visible(false);
        _acrescimoDescontoDocumentos.DataBaseCRT.visible(false);
        _acrescimoDescontoDocumentos.ValorMoedaCotacao.visible(false);
        _acrescimoDescontoDocumentos.ValorOriginalMoedaEstrangeira.visible(false);
    }

    if (!_modalAcrescimoDescontoDocumentosFatura)
        _modalAcrescimoDescontoDocumentosFatura = new bootstrap.Modal(document.getElementById("knockoutAcrescimoDescontoDocumentos"), { backdrop: 'static', keyboard: true });

    _modalAcrescimoDescontoDocumentosFatura.show();
}

function AdicionarAcrescimoDescontoDocumentosClick(e, sender) {
    _acrescimoDescontoDocumentos.Codigo.val(_fatura.Codigo.val());
    Salvar(_acrescimoDescontoDocumentos, "FaturaDocumento/AdicionarAcrescimoDescontoDocumentos", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor adicionado com sucesso!");
                _gridDocumentosFatura.CarregarGrid();

                _modalAcrescimoDescontoDocumentosFatura.hide();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CancelarAcrescimoDescontoDocumentosClick(e, sender) {
    limparCamposAcrescimoDescontoDocumentos();
    _acrescimoDescontoDocumentos.MoedaCotacaoBancoCentral.val(_fechamentoFatura.TipoMoeda.val());
}

function FecharAcrescimoDescontoDocumentosClick(e, sender) {
    _modalAcrescimoDescontoDocumentosFatura.hide();
    limparCamposAcrescimoDescontoDocumentos();
}

function DownloadLoteXMLDocumentosFaturaClick() {
    executarDownload("FaturaDocumento/DownloadLoteXML", { Fatura: _fatura.Codigo.val() });
}

function DownloadLoteDACTEDocumentosFaturaClick() {
    executarDownload("FaturaDocumento/DownloadLoteDACTE", { Fatura: _fatura.Codigo.val() });
}

function CarregarDocumentosFatura() {
    _documentoFatura.Fatura.val(_fatura.Codigo.val());
    _gridDocumentosFatura.CarregarGrid();
}

function CarregarHTMLDocumentoFatura(callback) {
    var prom = new promise.Promise();
    $.get("Content/Static/Fatura/DocumentoFatura.html?dyn=" + guid(), function (data) {
        $("#contentDocumentosFatura").html(data);
        prom.done();
    });
    return prom;
}

function AdicionarDocumentosFatura(documentos) {
    var codigosDocumentos = new Array();

    for (var i = 0; i < documentos.length; i++)
        codigosDocumentos.push(documentos[i].Codigo);

    executarReST("FaturaDocumento/AdicionarDocumentosFatura", { Fatura: _fatura.Codigo.val(), Documentos: JSON.stringify(codigosDocumentos) }, function (r) {
        if (r.Success) {
            if (r.Data) {
                _gridDocumentosFatura.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function BuscarDocumentosFatura() {
    var somenteLeitura = false;

    var editarColuna = {
        permite: true,
        callback: EditarValorACobrarDocumento,
        atualizarRow: false
    };
    var multiplaescolha = null;
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiEmbarcador) {
        multiplaescolha = {
            basicGrid: null,
            eventos: {},
            selecionados: new Array(),
            naoSelecionados: new Array(),
            SelecionarTodosKnout: _documentoFatura.SelecionarTodos,
            somenteLeitura: false
        }
    }

    var DarBaixaFatura = { descricao: "Dar Baixa", id: guid(), evento: "onclick", metodo: DarBaixaDocumentoFaturaClick, tamanho: "10", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [
            { descricao: "Acréscimos/Descontos", id: guid(), metodo: AbrirTelaAcrescimoDescontoFatura, visibilidade: VisibilidadeAcrescimoDesconto },
            { descricao: "Remover", id: guid(), metodo: RemoverDocumentoFatura },
            { descricao: "Baixar DACTE", id: guid(), metodo: DownloadDACTEDocumentoFatura, visibilidade: VisibilidadeDownloadDACTE },
            { descricao: "Baixar XML", id: guid(), metodo: DownloadXMLDocumentoFatura, visibilidade: VisibilidadeDownloadXMLCTe }
        ]
    };

    if (_CONFIGURACAO_TMS.PermitirDarBaixaFaturasCTe === true) {
        menuOpcoes.opcoes.push(DarBaixaFatura);
    }

    _gridDocumentosFatura = new GridView(_documentoFatura.Documentos.idGrid, "FaturaDocumento/Pesquisa", _documentoFatura, menuOpcoes, null, 20, null, null, null, multiplaescolha, null, editarColuna, null);
}

function DarBaixaDocumentosFaturaClick() {

    var dados = {};

    dados.SelecionarTodos = _documentoFatura.SelecionarTodos.val();
    dados.ItensSelecionados = JSON.stringify(_gridDocumentosFatura.ObterMultiplosSelecionados());
    dados.ItensNaoSelecionados = JSON.stringify(_gridDocumentosFatura.ObterMultiplosNaoSelecionados());

    if (_gridDocumentosFatura.ObterMultiplosSelecionados().length <= 0) {
        exibirMensagem(tipoMensagem.aviso, "Atenção", "Nenhum documento selecionado!");
        return;
    }

    exibirConfirmacao("Atenção!", "Deseja realmente dar baixa nos documentos da fatura?", function () {
        executarReST("FaturaDocumento/DarBaixaDocumentos", dados, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Efetuado baixa com sucesso!");
                    _gridDocumentosFatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    })
}

function DarBaixaDocumentoFaturaClick(e, sender) {
    console.log(e);
    exibirConfirmacao("Atenção!", "Deseja realmente dar baixa no documento " + e.Documento + " da fatura?", function () {
        executarReST("FaturaDocumento/DarBaixaDocumento", { Documento: e.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Efetuado baixa com sucesso!");
                    _gridDocumentosFatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });

}

function RemoverDocumentoFatura(documento) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o documento " + documento.Documento + " da fatura?", function () {
        executarReST("FaturaDocumento/RemoverDocumentoFatura", { Fatura: _fatura.Codigo.val(), Documento: documento.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    _gridDocumentosFatura.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function EditarValorACobrarDocumento(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, ValorACobrar: dataRow.ValorACobrar };
    executarReST("FaturaDocumento/AlterarDadosDocumento", data, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, r.Data)
                _gridDocumentosFatura.AtualizarDataRow(row, dataRow, callbackTabPress);
            } else {
                _gridDocumentosFatura.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            _gridDocumentosFatura.DesfazerAlteracaoDataRow(row);
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function ConfirmarDocumentoFaturaClick() {
    executarReST("FaturaDocumento/ConfirmarDocumentos", { Fatura: _fatura.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_fatura, r);
                CarregarDadosCabecalho(r.Data);
                PosicionarEtapa(r.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function AtualizarProgressFaturaNovo(codigo, percentual) {
    if (_fatura.Codigo.val() == codigo) {
        SetarPercentualProcessamento(percentual);

        $("#knockoutFaturaDocumento").hide();
        $("#knockoutPercentualFaturaDocumento").show();

        if (_crudDocumentoFatura != null)
            _crudDocumentoFatura.ConfirmarDocumentosFatura.visible(false);
    }
}

function SetarPercentualProcessamentoFaturaNovo(percentual) {
    finalizarRequisicao();
    var strPercentual = parseInt(percentual) + "%";
    _percentualDocumentoFatura.PercentualProcessado.val(strPercentual);
    $("#" + _percentualDocumentoFatura.PercentualProcessado.id).css("width", strPercentual);
}

function DownloadDACTEDocumentoFatura(e, sender) {
    var data = { CodigoCTe: e.CodigoCTe };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function DownloadXMLDocumentoFatura(e, sender) {
    var data = { CodigoCTe: e.CodigoCTe };
    executarDownload("CargaCTe/DownloadXML", data);
}

function VisibilidadeDownloadXMLCTe(e) {
    if (e.TipoDocumento == EnumTipoDocumentoFaturamento.CTe && (e.Modelo == "57" || e.Modelo == "39"))
        return true;
    else
        return false;
}

function VisibilidadeDownloadDACTE(e) {
    if (e.TipoDocumento == EnumTipoDocumentoFaturamento.CTe)
        return true;
    else
        return false;
}

function VisibilidadeAcrescimoDesconto(e) {
    if (!_CONFIGURACAO_TMS.UsuarioAdministrador && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Fatura_BloquearAcrescimoDesconto, _PermissoesPersonalizadas))
        return false;
    else
        return true;
}