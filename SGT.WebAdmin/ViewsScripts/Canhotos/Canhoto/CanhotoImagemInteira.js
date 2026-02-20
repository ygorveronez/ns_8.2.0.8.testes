/// <reference path="Conferencia.js" />
/// <reference path="Canhoto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var CanhotoImagemInteira = function (canhoto) {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Numero = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.Miniatura = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });
    this.ArquivoPDF = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Rejeitado = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.VisibilidadeRodape = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConfiguracaoEmbarcador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PendenteAprovacao = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExigeAprovacaoDigitalizacaoCanhoto = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermitirReverter = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PermissaoReverter = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DataDigitalizacao = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.ImagemInteira = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DT_RowColor = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "" });

    //Integração Comprovei
    this.PossuiIntegracaoComprovei = PropertyEntity({ text: "Integração IA", getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoCanhotoComprovei = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoNumeroComprovei = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoEncontrouDataComprovei = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ValidacaoAssinaturaComprovei = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });

    this.DataEntregaNotaCliente = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.DataDeEntrega.getFieldDescription(), getType: typesKnockout.date, val: ko.observable(""), def: "", cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-3 col-lg-3") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.SituacaoDoPOD.getFieldDescription(), val: ko.observable(_opcoesSituacaoDigitalizacaoPOD[0].value), options: _opcoesSituacaoDigitalizacaoPOD, def: _opcoesSituacaoDigitalizacaoPOD[0].value, cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-4 col-lg-4") });
    this.Observacao = PropertyEntity({ text: Localization.Resources.Canhotos.Canhoto.Observacao.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 1000, visible: ko.observable(false) });

    this.Acoes = PropertyEntity({ cssClass: ko.observable("col col-xs-12 col-sm-12 col-md-5 col-lg-5") });
    this.RejeitarImagem = PropertyEntity({ visible: ko.observable(), eventClick: rejeitarCanhotoImamgeInteiraClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Rejeitar });
    this.AprovarImagem = PropertyEntity({ visible: ko.observable(), eventClick: aprovarImagemCanhotoInteiroClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Aprovar });
    this.ReverterImagem = PropertyEntity({ visible: ko.observable(false), eventClick: reverterImagemCanhotoInteiroClick, type: types.event, text: Localization.Resources.Canhotos.Canhoto.Reverter });

    this.DownloadCanhotoImagemInteira = PropertyEntity({ eventClick: downloadCanhotoImagemInteiraClick, type: types.event });
    this.VisualizarPDF = PropertyEntity({
        eventClick: function (e) {
            exibirModalVisualizarPDFClick({ Codigo: e.Codigo.val() })
        }
    });

    this.Situacao.val.subscribe(function (valor) {
        var optionSelected = _opcoesSituacaoDigitalizacaoPOD.find(op => op.value == valor);
        if (optionSelected.exigeObservacao) {
            _knoutPesquisar.CanhotoInteiroAtual().DataEntregaNotaCliente.cssClass("col col-xs-12 col-sm-12 col-md-2 col-lg-2");
            _knoutPesquisar.CanhotoInteiroAtual().Situacao.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
            _knoutPesquisar.CanhotoInteiroAtual().Acoes.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
            _knoutPesquisar.CanhotoInteiroAtual().Observacao.visible(true);
        } else {
            _knoutPesquisar.CanhotoInteiroAtual().Observacao.visible(false);
            _knoutPesquisar.CanhotoInteiroAtual().DataEntregaNotaCliente.cssClass("col col-xs-12 col-sm-12 col-md-3 col-lg-3");
            _knoutPesquisar.CanhotoInteiroAtual().Situacao.cssClass("col col-xs-12 col-sm-12 col-md-4 col-lg-4");
            _knoutPesquisar.CanhotoInteiroAtual().Acoes.cssClass("col col-xs-12 col-sm-12 col-md-5 col-lg-5");
        }

    });

    PreencherObjetoKnout(this, { Data: canhoto });
};

function LoadGridCanhotosInteiros() {
    var header = [
        { data: "Ordem", visible: false },
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número  Canhoto" }
    ];

    _gridImagensInteiras = new BasicDataTable("grid-canhotos-imagem-inteira", header, null, { column: 0, dir: orderDir.asc }, null, 10, null, false, null, null, null, null, null, canhotoInteiroClick);
    _gridImagensInteiras.CarregarGrid([]);
}

function CarregarGridCanhotosInteiros(data) {
    _gridImagensInteiras.CarregarGrid(data);
}

function canhotoInteiroClick(row, data, dataTable) {
    $(row).click(function () {
        carregarCanhotoImagemInteira(data);
    });
}

function downloadCanhotoImagemInteiraClick(e) {
    if (e.Codigo.val() > 0 && e.GuidNomeArquivo != "") {
        var dados = {
            Codigo: e.Codigo.val()
        }
        executarDownload("Canhoto/DownloadCanhoto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Canhotos.Canhoto.CanhotoNaoEnviado, Localization.Resources.Canhotos.Canhoto.NaoFoiEnviadoCanhotoParaEstaNota);
    }
}

function rejeitarCanhotoImamgeInteiraClick(imagemSelecionada) {
    if (imagemSelecionada != undefined) {
        var optionSelected = _opcoesSituacaoDigitalizacaoPOD.find(op => op.value == imagemSelecionada.Situacao.val());
        if (optionSelected != undefined && optionSelected.value == 0) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.NecessarioInformarSituacaoDoPOD);
        } else if (optionSelected != undefined && optionSelected.exigeObservacao && imagemSelecionada.Observacao.val().length < 20) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.OCampoDeObservacaoDeveTerNoMinimo20Caracteres);
        } else if (optionSelected != undefined) {
            var data = {
                Codigo: imagemSelecionada.Codigo.val(),
                Motivo: imagemSelecionada.Situacao.val(),
                Observacoes: imagemSelecionada.Observacao.val()
            };

            executarReST("Canhoto/DescartarCanhoto", data, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data !== false) {
                        AlteraStatusDoCanhotoInteiro(imagemSelecionada.Codigo.val(), true, false);

                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }, null);
        }
    }
}

function aprovarImagemCanhotoInteiroClick(imagemSelecionada) {
    if (imagemSelecionada != undefined) {
        if (_CONFIGURACAO_TMS.ExigirDataEntregaNotaClienteCanhotos && imagemSelecionada.DataEntregaNotaCliente.val() == "") {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Canhotos.Canhoto.NecessarioInformarADataDeEntrega);
        } else {
            executarReST("Canhoto/ValidarCanhoto", { Codigo: imagemSelecionada.Codigo.val(), DataEntrega: imagemSelecionada.DataEntregaNotaCliente.val() }, function (retorno) {
                if (retorno.Success) {
                    if (retorno.Data !== false) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AutorizadoSucesso);
                        AlteraStatusDoCanhotoInteiro(imagemSelecionada.Codigo.val(), false, false);
                    }
                    else
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                }
                else
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
            }, null);
        }
    }
}

function reverterImagemCanhotoInteiroClick(imagemSelecionada) {
    if (imagemSelecionada != undefined) {
        executarReST("Canhoto/ReverterCanhoto", { Codigo: imagemSelecionada.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    AlteraStatusDoCanhotoInteiro(imagemSelecionada.Codigo.val(), false, true);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    }
}

function carregarCanhotoImagemInteira(registro) {
    var knoutCanhotoImagemInteira = null;
    knoutCanhotoImagemInteira = new CanhotoImagemInteira(registro);

    knoutCanhotoImagemInteira.PermissaoReverter.val(VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Canhoto_PermitirReverterImagem, _PermissoesPersonalizadasCanhotos));

    _knoutPesquisar.CanhotoInteiroAtual(knoutCanhotoImagemInteira);

    ConfigurarCamposKnockout(_knoutPesquisar.CanhotoInteiroAtual(), false);

    if (registro.ArquivoPDF)
        renderizarPDFCanhotoClick(registro);
}

function AlteraStatusDoCanhotoInteiro(codigo, status, pendenteAprovacao) {
  
    var quantidadeRegistrosNaGrid = codigosCanhotosInteiros.length;
    var i = $.inArray(codigo.toString(), codigosCanhotosInteiros);
    if (i === -1)
        i = $.inArray(codigo, codigosCanhotosInteiros);
    var imagemRejeitada = $.extend(true, {}, imagensInteirasConferencia[i]);

    var j = i + 1 < quantidadeRegistrosNaGrid ? i + 1 : 0;
    var proximoRegistro = $.extend(true, {}, imagensInteirasConferencia[j]);

    imagemRejeitada.Rejeitado = status;
    imagemRejeitada.PendenteAprovacao = pendenteAprovacao;
    imagemRejeitada.DT_RowColor = obterCorLinhaCanhotoInteiro(status, pendenteAprovacao);
    imagemRejeitada.PermitirReverter = permitirReverter(status, pendenteAprovacao, imagemRejeitada.DataDigitalizacao);

    carregarCanhotoImagemInteira(proximoRegistro);
    _knoutPesquisar.ImagensInteirasConferencia.val.replace(imagensInteirasConferencia[i], imagemRejeitada);
    CarregarGridCanhotosInteiros(_knoutPesquisar.ImagensInteirasConferencia.val());
}


function obterCorLinhaCanhotoInteiro(status, pendenteAprovacao) {
    if (status)
        return "#EBCCCC";
    else if (pendenteAprovacao)
        return "#FFFFFF";
    else
        return "#DFF0D8";
}

function permitirReverter(status, pendenteAprovacao, dataDigitalizacao) {
    var dataDigit = moment(dataDigitalizacao, "DD/MM/YYYY");

    if (status || pendenteAprovacao)
        return false;
    else
        dataDigit = moment();

    var agora = moment();
    var dataDiferenca = agora.diff(dataDigit, 'days');

    if (_prazoParaReverterDigitalizacaoAposAprovacao > 0 && dataDiferenca > _prazoParaReverterDigitalizacaoAposAprovacao)
        return false;

    return true;
}

function renderizarPDFCanhotoClick(imagemSelecionada) {
    $("#pdf-viewer-canhoto-inteiro").attr('src', "Canhotos/RenderizarPDF?Canhoto=" + imagemSelecionada.Codigo);
}