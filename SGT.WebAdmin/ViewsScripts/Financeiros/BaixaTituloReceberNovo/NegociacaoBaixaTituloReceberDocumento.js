//*******MAPEAMENTO KNOUCKOUT*******

var _gridDocumentosNegociacaoBaixaTituloReceber, _documentoNegociacaoBaixaTituloReceber, _acrescimoDescontoDocumentos;

var DocumentoNegociacaoBaixaTituloReceber = function () {
    this.Grid = PropertyEntity({ type: types.local });
    
    this.Tomador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tomador:", idBtnSearch: guid() });
    this.NumeroTitulo = PropertyEntity({ getType: typesKnockout.int, text: "Nº Título:", val: ko.observable(""), def: ""});
    this.DocumentoCTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Documento:", idBtnSearch: guid() });

    this.TituloBaixa = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Documento = PropertyEntity({ type: types.event, text: "Adicionar Documento", idBtnSearch: guid(), enable: ko.observable(true) });
    this.AcrescimoDesconto = PropertyEntity({ eventClick: AcrescimoDescontoClick, type: types.event, text: "Acréscimo / Desconto", visible: ko.observable(true), enable: ko.observable(true) });
    this.RatearValorPagoEntreDocumentos = PropertyEntity({ eventClick: RatearValorPagoEntreDocumentosClick, type: types.event, text: "Valor Pago", visible: ko.observable(true), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Documentos = PropertyEntity({ type: types.local, idGrid: guid(), enable: ko.observable(true) });
}

var AcrescimoDescontoDocumentos = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Valor = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor:", val: ko.observable(""), def: "", required: true, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.ValorMoeda = PropertyEntity({ getType: typesKnockout.decimal, text: "*Valor em Moeda:", val: ko.observable(""), def: "", required: false, visible: ko.observable(true) });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true, issue: 382, enable: ko.observable(true), cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-8 col-lg-8") });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable(""), def: "", required: false, maxlength: 500 });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarAcrescimoDescontoDocumentosClick, type: types.event, text: "Adicionar", icon: "fal fa-plus", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAcrescimoDescontoDocumentosClick, type: types.event, text: "Limpar", icon: "fa fa-rotate-left", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharAcrescimoDescontoDocumentosClick, type: types.event, text: "Fechar", icon: "fa fa-window-close", visible: ko.observable(true) });

    this.ValorMoeda.val.subscribe(function (novoValor) {
        _acrescimoDescontoDocumentos.Valor.val(Globalize.format(ObterValorEmRealBaixaTituloReceber(novoValor), "n2"));
    });
}

//*******EVENTOS*******

function LoadDocumentoNegociacaoBaixaTituloReceber() {

    _documentoNegociacaoBaixaTituloReceber = new DocumentoNegociacaoBaixaTituloReceber();
    KoBindings(_documentoNegociacaoBaixaTituloReceber, "knockoutDocumentosNegociacaoBaixaTituloReceber");

    _acrescimoDescontoDocumentos = new AcrescimoDescontoDocumentos();
    KoBindings(_acrescimoDescontoDocumentos, "knockoutAcrescimoDescontoDocumentos");

    new BuscarTitulosPendentesParaBaixaTituloReceberNova(_documentoNegociacaoBaixaTituloReceber.Documento, RetornoConsultaTituloPendenteAdicionarBaixa);
    new BuscarJustificativas(_acrescimoDescontoDocumentos.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.TitulosReceber, EnumTipoFinalidadeJustificativa.Todas]);
    new BuscarClientes(_documentoNegociacaoBaixaTituloReceber.Tomador);
    new BuscarConhecimentoNotaReferencia(_documentoNegociacaoBaixaTituloReceber.DocumentoCTe, RetornoBuscarConhecimentoNota);

    LoadAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber();
    LoadNegociacaoBaixaTituloReceberDocumentoRateioValorPago();

    BuscarDocumentosNegociacaoBaixaTituloReceber();
}

function AcrescimoDescontoClick(e, sender) {
    limparCamposAcrescimoDescontoDocumentos();

    if (_negociacaoBaixa.Moeda.val() != EnumMoedaCotacaoBancoCentral.Real) {
        _acrescimoDescontoDocumentos.ValorMoeda.visible(true);
        _acrescimoDescontoDocumentos.ValorMoeda.required = true;
        _acrescimoDescontoDocumentos.Valor.enable(false);

        _acrescimoDescontoDocumentos.Justificativa.cssClass("col col-xs-12 col-sm-12 col-md-6 col-lg-6");
        _acrescimoDescontoDocumentos.Valor.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
    } else {
        _acrescimoDescontoDocumentos.ValorMoeda.visible(false);
        _acrescimoDescontoDocumentos.ValorMoeda.required = false;
        _acrescimoDescontoDocumentos.Valor.enable(true);

        _acrescimoDescontoDocumentos.Justificativa.cssClass("col col-xs-12 col-sm-12 col-md-8 col-lg-8");
        _acrescimoDescontoDocumentos.Valor.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
    }

    Global.abrirModal('knockoutAcrescimoDescontoDocumentos');
}

function AdicionarAcrescimoDescontoDocumentosClick(e, sender) {
    _acrescimoDescontoDocumentos.Codigo.val(_baixaTituloReceber.Codigo.val());
    Salvar(_acrescimoDescontoDocumentos, "BaixaTituloReceberNovoAgrupadoDocumentoAcrescimoDesconto/AdicionarAcrescimoDescontoDocumentos", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Valor adicionado com sucesso!");
                _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();
                PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });

                Global.fecharModal('knockoutAcrescimoDescontoDocumentos');
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
}

function FecharAcrescimoDescontoDocumentosClick(e, sender) {
    Global.fecharModal('knockoutAcrescimoDescontoDocumentos');
    limparCamposAcrescimoDescontoDocumentos();
}

function RetornoConsultaTituloPendenteAdicionarBaixa(data) {
    executarReST("BaixaTituloReceberNovo/AdicionarTitulo", { Titulo: data.Codigo, BaixaTituloReceber: _baixaTituloReceber.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });
                _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function RemoverTituloBaixaReceberClick(data) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover o título " + data.Titulo + " desta baixa?", function () {
        executarReST("BaixaTituloReceberNovo/RemoverTitulo", { Titulo: data.Titulo, BaixaTituloReceber: _baixaTituloReceber.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });
                    _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function CarregarDocumentosNegociacaoBaixaTituloReceber() {
    _documentoNegociacaoBaixaTituloReceber.TituloBaixa.val(_baixaTituloReceber.Codigo.val());
    _gridDocumentosNegociacaoBaixaTituloReceber.CarregarGrid();
}

function BuscarDocumentosNegociacaoBaixaTituloReceber() {
    var editarColuna = {
        permite: true,
        callback: EditarValorPagoDocumentoNegociacaoBaixaTituloReceber,
        atualizarRow: false
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        tamanho: 7,
        opcoes: [
            { descricao: "Acréscimos/Descontos", id: guid(), metodo: AbrirTelaAcrescimoDescontoDocumentoNegociacaoBaixaTituloReceber },
            { descricao: "Baixar XML", id: guid(), metodo: BaixarXMLCTeClick, visibilidade: VisibilidadeOpcaoDownload },
            { descricao: "Baixar DACTE", id: guid(), metodo: BaixarDacteClick, visibilidade: VisibilidadeOpcaoDownload },
            { descricao: "Remover Título", id: guid(), metodo: RemoverTituloBaixaReceberClick, visibilidade: VisibilidadeOpcaoRemoverTitulo }
        ]
    };

    _gridDocumentosNegociacaoBaixaTituloReceber = new GridView(_documentoNegociacaoBaixaTituloReceber.Documentos.idGrid, "BaixaTituloReceberNovoAgrupadoDocumento/Pesquisa", _documentoNegociacaoBaixaTituloReceber, menuOpcoes, null, null, null, null, null, null, null, editarColuna, null);
}

function EditarValorPagoDocumentoNegociacaoBaixaTituloReceber(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, ValorPago: dataRow.ValorPago, ValorPagoMoeda: dataRow.ValorPagoMoeda };
    executarReST("BaixaTituloReceberNovoAgrupadoDocumento/AlterarValorPago", data, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                CompararEAtualizarGridEditableDataRow(dataRow, r.Data.Row);
                _gridDocumentosNegociacaoBaixaTituloReceber.AtualizarDataRow(row, dataRow, callbackTabPress);
                PreencherObjetoKnout(_negociacaoBaixa, { Data: r.Data.Negociacao });
            } else {
                _gridDocumentosNegociacaoBaixaTituloReceber.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            _gridDocumentosNegociacaoBaixaTituloReceber.DesfazerAlteracaoDataRow(row);
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}

function BaixarXMLCTeClick(e) {
    var data = { CodigoCTe: e.CodigoCTe };
    executarDownload("CargaCTe/DownloadXML", data);
}

function BaixarDacteClick(e) {
    var data = { CodigoCTe: e.CodigoCTe };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function VisibilidadeOpcaoDownload(e) {
    if (e.TipoDocumento == EnumTipoDocumentoTitulo.CTe)
        return true;

    return false;
}

function VisibilidadeOpcaoRemoverTitulo(e) {
    return e.SituacaoBaixaTitulo === EnumSituacaoBaixaTitulo.Iniciada;
}

function limparCamposAcrescimoDescontoDocumentos() {
    LimparCampos(_acrescimoDescontoDocumentos);
}

function RetornoBuscarConhecimentoNota(data) {
    _documentoNegociacaoBaixaTituloReceber.DocumentoCTe.codEntity(data.Codigo);
    _documentoNegociacaoBaixaTituloReceber.DocumentoCTe.val(data.Numero + "-" + data.Serie);
}