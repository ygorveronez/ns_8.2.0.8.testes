/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _conferenciaSeparacao;
var _gridConferenciaSeparacao;

/*
 * Declaração das Classes
 */

var ConferenciaSeparacao = function () {
    this.Expedicao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CodigoBarras = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.CargaControleExpedicao.CodigoDeBarra.getRequiredFieldDescription(), required: true, eventClick: ChangeCodigoBarras, visible: ko.observable(true) });
    this.LerCodigoBarrasCamera = PropertyEntity({ type: types.map, text: Localization.Resources.Cargas.CargaControleExpedicao.AbrirCamera, required: false, eventClick: AbrirCamera, enable: ko.observable(true), visible: ko.observable(window.mobileAndTabletCheck()) });

    this.Quantidade = PropertyEntity({ type: types.map, configDecimal: { precision: 3, allowZero: false, allowNegative: false }, getType: typesKnockout.decimal, val: ko.observable("0,000"), def: "", text: Localization.Resources.Cargas.CargaControleExpedicao.Quantidade.getRequiredFieldDescription(), required: ko.observable(true), visible: ko.observable(true) });

    this.Produtos = PropertyEntity({ idGrid: guid() });
    this.Mensagem = PropertyEntity({ type: types.local, visible: ko.observable(false), cssClass: ko.observable(""), eventClick: function () { _conferenciaSeparacao.Mensagem.visible(false) } });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Finalizar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadConferenciaSeparacao() {
    _conferenciaSeparacao = new ConferenciaSeparacao();
    KoBindings(_conferenciaSeparacao, "knockoutConferenciaSeparacao");

    loadGridProdutosSeparacao();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _conferenciaSeparacao.Quantidade.visible(false);
        _conferenciaSeparacao.Quantidade.required(false);
    }

    _conferenciaSeparacao.CodigoBarras.get$()
        .on("keydown", function (e) {
            var ENTER_KEY = 13;
            var key = e.which || e.keyCode || 0;
            if (key == ENTER_KEY)
                ChangeCodigoBarras();
        })
        .on("blur", ChangeCodigoBarras);

    _conferenciaSeparacao.Quantidade.get$()
        .on("keydown", function (e) {
            var ENTER_KEY = 13;
            var key = e.which || e.keyCode || 0;
            if (key == ENTER_KEY)
                adicionarClick(null, null);
        });

    $modalConferenciaSeparacao.on('hidden.bs.modal', limparCamposConferenciaSeparacao);
}

function loadGridProdutosSeparacao() {
    _gridConferenciaSeparacao = new GridView(_conferenciaSeparacao.Produtos.idGrid, "ConferenciaSeparacao/Pesquisa", _conferenciaSeparacao);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    Salvar(_conferenciaSeparacao, "ConferenciaSeparacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CargaControleExpedicao.AtualizadoComSucesso);
                _gridConferenciaSeparacao.CarregarGrid();
                limparCamposConferenciaSeparacao();
                _conferenciaSeparacao.CodigoBarras.get$().focus();
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.CargaControleExpedicao.SeparacaoConferida);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
                _conferenciaSeparacao.CodigoBarras.val("");
                _conferenciaSeparacao.CodigoBarras.get$().focus();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function finalizarClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Cargas.CargaControleExpedicao.FinalizarSeparacao, Localization.Resources.Cargas.CargaControleExpedicao.VoceCertezaDesejaFinalizarSeparacao, function () {
        executarReST("ConferenciaSeparacao/Finalizar", { Expedicao: _conferenciaSeparacao.Expedicao.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    Global.fecharModal('divModalConferenciaSeparacao')
                    //$modalConferenciaSeparacao.modal('hide');                    
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                    _gridConferenciaSeparacao.CarregarGrid();
                    _gridCargaControleExpedicao.CarregarGrid();
                    limparCamposConferenciaSeparacao();
                } else {
                    _conferenciaSeparacao.Mensagem.cssClass("warning");
                    _conferenciaSeparacao.Mensagem.val(arg.Msg);
                    _conferenciaSeparacao.Mensagem.visible(true);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, sender);
    });
}

function EditarConferenciaSeparacaoClick(itemGrid) {
    // Limpa os campos
    limparCamposConferenciaSeparacao();

    // Seta o codigo do objeto
    _conferenciaSeparacao.Expedicao.val(itemGrid.Codigo);
    _conferenciaSeparacao.Carga.val(itemGrid.CodigoCarga);

    // Busca informacoes para edicao
    _gridConferenciaSeparacao.CarregarGrid(function () {
        if (itemGrid.AptoConferir) {
            if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
                _conferenciaSeparacao.Quantidade.visible(true);
                _conferenciaSeparacao.Adicionar.visible(true);
            }
            else
                _conferenciaSeparacao.Adicionar.visible(false);
            _conferenciaSeparacao.CodigoBarras.visible(true);
            _conferenciaSeparacao.Finalizar.visible(true);
        } else {
            _conferenciaSeparacao.CodigoBarras.visible(false);
            _conferenciaSeparacao.Quantidade.visible(false);
            _conferenciaSeparacao.Adicionar.visible(false);
            _conferenciaSeparacao.Finalizar.visible(false);
        }

        //$modalConferenciaSeparacao.modal('show');
        Global.abrirModal('divModalConferenciaSeparacao')
    });
}

function ChangeCodigoBarras() {
    if (_conferenciaSeparacao.CodigoBarras.val() != "")
        BuscarCodigoBarrasSeparacao(_conferenciaSeparacao.CodigoBarras.val());
}

/*
 * Declaração das Funções
 */

function limparCamposConferenciaSeparacao() {
    var tmpCodigoExpedicao = _conferenciaSeparacao.Expedicao.val();
    var tmpCodigoCarga = _conferenciaSeparacao.Carga.val();

    LimparCampos(_conferenciaSeparacao);

    _conferenciaSeparacao.Mensagem.visible(false);
    _conferenciaSeparacao.Expedicao.val(tmpCodigoExpedicao);
    _conferenciaSeparacao.Carga.val(tmpCodigoCarga);
}

function BuscarCodigoBarrasSeparacao(codigobarra) {
    executarReST("ConferenciaSeparacao/ValidaCodigoBarras", { Carga: _conferenciaSeparacao.Carga.val(), CodigoBarras: codigobarra.trim() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                var data = arg.Data;

                if (!data.CodigoBarrasValido) {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.CargaControleExpedicao.CodigoBarraInformadoNaoConstaListaProdutosExpedicao);

                    _conferenciaSeparacao.CodigoBarras.val("");
                    _conferenciaSeparacao.CodigoBarras.get$().focus();

                    return;
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador) {
                    if (data.JaConferido && data.Conferido == data.Quantidade) {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, Localization.Resources.Cargas.CargaControleExpedicao.JaConferidoQuantidadeTotalProduto);
                        _conferenciaSeparacao.CodigoBarras.val("");
                        _conferenciaSeparacao.CodigoBarras.get$().focus();

                        return;
                    }

                    if (data.JaConferido) {
                        _conferenciaSeparacao.Mensagem.cssClass("info");
                        _conferenciaSeparacao.Mensagem.val(Localization.Resources.Gerais.Geral.ProdutoCom + data.Conferido + Localization.Resources.Cargas.CargaControleExpedicao.QuantidadeConferida);
                        _conferenciaSeparacao.Mensagem.visible(true);
                    }

                    _conferenciaSeparacao.Quantidade.get$().focus();
                }
                else if (_conferenciaSeparacao.CodigoBarras.val() != "")
                    adicionarClick();

            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);

            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function AbrirCamera() {
    var etiquetaUsaQRCode = _CONFIGURACAO_TMS.UtilizarEtiquetaDetalhadaWMS;
    if (etiquetaUsaQRCode) {
        LerQRCodeCamera();
    } else {
        LerCodigoBarrasCamera();
    }
}

window.mobileAndTabletCheck = function () {
    let check = false;
    (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
    return check;
};