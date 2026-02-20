//*******MAPEAMENTO KNOUCKOUT*******

var callbackImagensCarregadas = null;
var codigosCanhotos = [];
var codigosCanhotosInteiros = [];
var imagensConferencia = [];
var imagensInteirasConferencia = [];
var _motivoInconsistenciaDigitacao;
var $tabVisualizao = null;
var dragg = null;
var _buscouMiniaturas = false;

var MotivoInconsistenciaDigitacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Motivo = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Canhotos.Canhoto.Motivo.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.Observacoes = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), def: "", maxlength: 400, text: Localization.Resources.Canhotos.Canhoto.Observacoes.getFieldDescription() });

    this.Adicionar = PropertyEntity({ eventClick: rejeitarCanhotoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: true });
}

//*******EVENTOS*******

function LoadConferencia() {
    _motivoInconsistenciaDigitacao = new MotivoInconsistenciaDigitacao();
    KoBindings(_motivoInconsistenciaDigitacao, "knockoutMotivoInconsistenciaDigitacao");

    new BuscarMotivoInconsistenciaDigitacao(_motivoInconsistenciaDigitacao.Motivo);

    $tabVisualizao = $("#liVisualizacao");
    $tabVisualizao.on('shown.bs.tab', 'a', function () {
        ControlarExibicaoGrid();
    });
    $(window).one('hashchange', function (e) {
        $tabVisualizao.off('shown.bs.tab', 'a');
    });

    dragg = $.draggImagem({
        container: ".DivConferencia",
        image: ".container-drag img"
    });
}

function exibirModalMotivoInconsistenciaDigitacaoClick(imagemSelecionada) {
    _motivoInconsistenciaDigitacao.Codigo.val(imagemSelecionada.Codigo);

    exibirModalMotivoInconsistenciaDigitacao();
}

function exibirModalVisualizarPDFClick(imagemSelecionada) {
    $("#pdf-viewer").attr('src', "Canhotos/RenderizarPDF?Canhoto=" + imagemSelecionada.Codigo);
    Global.abrirModal('divModalVisualizarPDF');
}

function rejeitarCanhotoClick() {
    if (ValidarCamposObrigatorios(_motivoInconsistenciaDigitacao)) {
        executarReST("Canhoto/DescartarCanhoto", RetornarObjetoPesquisa(_motivoInconsistenciaDigitacao), function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    AlteraPropriedadeRejeitadoDoCanhoto(_motivoInconsistenciaDigitacao.Codigo.val(), true, false);

                    fecharModalMotivoInconsistenciaDigitacao();

                    if (!_CONFIGURACAO_TMS.NaoAtualizarTelaCanhotosAposAprovacaoRejeicao) {
                        _gridCanhotos.CarregarGrid();
                    }
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    }
}

function validarImagemCanhotoClick(imagemSelecionada) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.RealmenteDesejaConfirmarDigitalizacaoDestaImagem, function () {
        executarReST("Canhoto/ValidarCanhoto", { Codigo: imagemSelecionada.Codigo, DataEntrega: imagemSelecionada.DataEntregaNotaCliente }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    AlteraPropriedadeRejeitadoDoCanhoto(imagemSelecionada.Codigo, false, false);

                    if (!_CONFIGURACAO_TMS.NaoAtualizarTelaCanhotosAposAprovacaoRejeicao) {
                        _gridCanhotos.CarregarGrid();
                    }
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

function reverterImagemCanhotoClick(imagemSelecionada) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Canhotos.Canhoto.RealmenteDesejaReverterDigitalizacaoDestaImagem, function () {
        executarReST("Canhoto/ReverterCanhoto", { Codigo: imagemSelecionada.Codigo }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data !== false) {
                    AlteraPropriedadeRejeitadoDoCanhoto(imagemSelecionada.Codigo, false, true);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
        }, null);
    });
}

//*******METODOS*******

function ControlarExibicaoGrid() {
    if (callbackImagensCarregadas != null) {
        callbackImagensCarregadas();
        callbackImagensCarregadas = null;
    }
}

function BuscarMiniaturasEExibirGrid() {
    var codigos = obterCodigosCanhotosExibidos();

    _buscouMiniaturas = true;

    _knoutPesquisar.ImagensConferencia.val.removeAll();
    _knoutPesquisar.ImagensInteirasConferencia.val.removeAll();
    _knoutPesquisar.CanhotoInteiroAtual('');
    imagensInteirasConferencia = [];
    codigosCanhotos = [];
    codigosCanhotosInteiros = [];
    imagensConferencia = [];

    iniciarRequisicao();
    _ControlarManualmenteProgresse = true;

    executarReST('Canhoto/ObterMiniaturas', { Codigos: JSON.stringify(codigos) }, function (arg) {
        if (arg.Success) {
            // Do servidor, os objetos sao retornados por ordem de código
            // Aqui é ordenado por ordem da grid

            codigos.forEach(function (cod) {
                for (var i in arg.Data.Imagens) {
                    var obj = arg.Data.Imagens[i];
                    if (!obj.ImagemInteira) {
                        if (obj.Codigo == cod) {
                            var objordenado = arg.Data.Imagens.splice(i, 1);
                            imagensConferencia.push(objordenado[0]);
                            codigosCanhotos.push(cod);
                        }
                    } else {
                        if (obj.Codigo == cod) {
                            var objordenado = arg.Data.Imagens.splice(i, 1);
                            imagensInteirasConferencia.push(objordenado[0]);
                            codigosCanhotosInteiros.push(cod);
                        }
                    }

                }
            });

            codigosCanhotos = [...new Set(codigosCanhotos)];
            codigosCanhotosInteiros = [...new Set(codigosCanhotosInteiros)];

            _knoutPesquisar.ImagensConferencia.val(imagensConferencia);
            _knoutPesquisar.ImagensInteirasConferencia.val(imagensInteirasConferencia);
            setTimeout(dragg.centralize, 500);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        CarregarGridCanhotosInteiros(_knoutPesquisar.ImagensInteirasConferencia.val());
        ControlarAbasVisualizacaoCanhotos(codigosCanhotosInteiros.length, codigosCanhotos.length);

        _ControlarManualmenteProgresse = false;
        finalizarRequisicao();
    });
}

function exibirModalMotivoInconsistenciaDigitacao() {
    Global.abrirModal('divModalMotivoInconsistenciaDigitacao');
    $("#divModalMotivoInconsistenciaDigitacao").one('hidden.bs.modal', function () {
        LimparCampos(_motivoInconsistenciaDigitacao);
    });
}

function fecharModalMotivoInconsistenciaDigitacao() {
    Global.fecharModal('divModalMotivoInconsistenciaDigitacao');
}

function AlteraPropriedadeRejeitadoDoCanhoto(codigo, status, pendenteAprovacao) {
    var i = $.inArray(codigo, codigosCanhotos);//% _gridCanhotos.GetQuantidadeLinhasPorPagina();
    var imagemRejeitada = $.extend(true, {}, imagensConferencia[i]);

    imagemRejeitada.Rejeitado = status;
    imagemRejeitada.PendenteAprovacao = pendenteAprovacao;

    _knoutPesquisar.ImagensConferencia.val.replace(imagensConferencia[i], imagemRejeitada);
}

function ExibirSliceCanhotos() {
    if (!$("#liVisualizacao a").hasClass('active'))
        callbackImagensCarregadas = BuscarMiniaturasEExibirGrid;
    else
        BuscarMiniaturasEExibirGrid();
}
