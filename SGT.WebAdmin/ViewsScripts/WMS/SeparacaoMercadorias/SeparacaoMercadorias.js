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
/// <reference path="../../Enumeradores/EnumSituacaoSelecaoSeparacao.js" />
/// <reference path="CameraCodigoBarrasPosicao.js" />
/// <reference path="CameraCodigoBarras.js" />
/// <reference path="CameraQRCode.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _separacaoMercadorias;
var _pesquisaSeparacaoMercadorias;
var _gridSeparacaoMercadorias;
var _gridProdutosSeparacao;
var _descarteLoteConfigDecimal = { precision: 3, allowZero: false, allowNegative: false };
var _situacaoSelecaoSeparacao = [
    { text: "Todos", value: "" },
    { text: "Enviada", value: EnumSituacaoSelecaoSeparacao.Enviada },
    { text: "Finalizada", value: EnumSituacaoSelecaoSeparacao.Finalizada },
    { text: "Cancelada", value: EnumSituacaoSelecaoSeparacao.Cancelada },
];

var SeparacaoMercadorias = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, idFade: guid(), visibleFade: ko.observable(false) });

    this.Carga = PropertyEntity({ text: "Carga: " });
    this.Produtos = PropertyEntity({ text: "Produtos: ", idGrid: guid() });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0 });

    this.Posicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Local Armazenamento:", idBtnSearch: guid(), visible: ko.observable(true), eventClick: ChangePosicao, required: true });
    this.LerCodigoBarrasCameraPosicao = PropertyEntity({ type: types.map, text: "Abrir câmera:", required: false, eventClick: AbrirCameraPosicao, enable: ko.observable(true), visible: ko.observable(window.mobileAndTabletCheck()) });

    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Produto Embarcador:", idBtnSearch: guid(), visible: ko.observable(true), eventClick: ChangeProdutoEmbarcador, required: true, enable: ko.observable(true) });
    this.Quantidade = PropertyEntity({ type: types.map, configDecimal: _descarteLoteConfigDecimal, getType: typesKnockout.decimal, val: ko.observable("0,000"), def: "", text: "*Quantidade:", enable: ko.observable(true), required: true, visible: ko.observable(true) });

    this.CodigoBarras = PropertyEntity({ type: types.map, text: "*Código de Barras:", required: false, eventClick: ConferirVolumeClick, enable: ko.observable(true), visible: ko.observable(false) });
    this.LerCodigoBarrasCamera = PropertyEntity({ type: types.map, text: "Abrir câmera:", required: false, eventClick: AbrirCamera, enable: ko.observable(true), visible: ko.observable(window.mobileAndTabletCheck()) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Finalizar = PropertyEntity({ eventClick: finalizarClick, type: types.event, text: "Finalizar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: limparClick, type: types.event, text: "Limpar", visible: ko.observable(true) });
    this.GerarImpressao = PropertyEntity({ eventClick: gerarImpressaoClick, type: types.event, text: "Gerar Impressão", visible: ko.observable(true) });
}

var PesquisaSeparacaoMercadorias = function () {
    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoSelecaoSeparacao.Enviada), def: EnumSituacaoSelecaoSeparacao.Enviada, options: _situacaoSelecaoSeparacao, text: "Situação:" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSeparacaoMercadorias.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}


//*******EVENTOS*******
function loadSeparacaoMercadorias() {
    var ENTER_KEY = 13;
    //-- Knouckout
    // Instancia pesquisa
    _pesquisaSeparacaoMercadorias = new PesquisaSeparacaoMercadorias();
    KoBindings(_pesquisaSeparacaoMercadorias, "knockoutPesquisaSeparacaoMercadorias", false, _pesquisaSeparacaoMercadorias.Pesquisar.id);

    // Instancia objeto principal
    _separacaoMercadorias = new SeparacaoMercadorias();
    KoBindings(_separacaoMercadorias, "knockoutSeparacaoMercadorias");

    HeaderAuditoria("Separacao", _separacaoMercadorias);

    GridProdutosSeparacao();

    // Inicia busca
    buscarSeparacaoMercadorias();

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) {
        _separacaoMercadorias.Quantidade.visible(false);
        _separacaoMercadorias.ProdutoEmbarcador.visible(false);
        _separacaoMercadorias.Adicionar.visible(false);
        _separacaoMercadorias.CodigoBarras.visible(true);
    }

    _separacaoMercadorias.Posicao.get$()
        .on("change", ChangePosicao)
        .on("keydown", function (e) {
            var key = e.which || e.keyCode || 0;
            if (key == ENTER_KEY) ChangePosicao();
        });

    _separacaoMercadorias.ProdutoEmbarcador.get$()
        .on("change", ChangeProdutoEmbarcador)
        .on("keydown", function (e) {
            var key = e.which || e.keyCode || 0;
            if (key == ENTER_KEY) ChangeProdutoEmbarcador();
        });

    _separacaoMercadorias.CodigoBarras.get$()
        .on("keydown", function (e) {
            var ENTER_KEY = 13;
            var key = e.which || e.keyCode || 0;
            if (key === ENTER_KEY)
                ConferirVolumeClick();
        });
}

function ConferirVolumeClick(e, sender) {
    setTimeout(function () {
        var erro = false;
        var codigoBarrasLocalizar = _separacaoMercadorias.CodigoBarras.val();
        var numeroNS = "";
        var numeroSerie = "";
        var cnpjRemetente = "";
        var numeroNota = "";
        var serieNota = "";
        var volume = "";

        if (codigoBarrasLocalizar.length === 34) {
            cnpjRemetente = codigoBarrasLocalizar.substring(0, 14);
            cnpjRemetente = Globalize.parseFloat(cnpjRemetente);

            numeroNota = codigoBarrasLocalizar.substring(14, 20);
            numeroNota = Globalize.parseFloat(numeroNota);

            serieNota = codigoBarrasLocalizar.substring(20, 22);
            serieNota = Globalize.parseFloat(serieNota);

            volume = codigoBarrasLocalizar.substring(22, 26);
        }
        else if (codigoBarrasLocalizar.length === 13) {
            numeroNS = codigoBarrasLocalizar.substring(0, 10);
            volume = codigoBarrasLocalizar.substring(10, 13);
        } else if (codigoBarrasLocalizar.length === 31) {
            cnpjRemetente = codigoBarrasLocalizar.substring(0, 14);
            cnpjRemetente = Globalize.parseFloat(cnpjRemetente);

            numeroNota = codigoBarrasLocalizar.substring(14, 23);
            numeroNota = Globalize.parseFloat(numeroNota);

            volume = codigoBarrasLocalizar.substring(23, 27);

            serieNota = codigoBarrasLocalizar.substring(27, 29);
            serieNota = Globalize.parseFloat(serieNota);
        } else {
            exibirMensagem(tipoMensagem.aviso, "Aviso", "Código de barras inválido.");
            _separacaoMercadorias.CodigoBarras.val("");
            _separacaoMercadorias.CodigoBarras.get$().focus();
            erro = true;
            return;
        }

        if (!erro) {
            var data = {
                Codigo: _separacaoMercadorias.Codigo.val(),
                Posicao: _separacaoMercadorias.Posicao.codEntity(),
                CodigoBarrasLocalizar: codigoBarrasLocalizar,
                NumeroNS: numeroNS,
                Volume: volume,
                CNPJRemetente: cnpjRemetente,
                NumeroNota: numeroNota,
                SerieNota: serieNota
            };
            executarReST("SeparacaoMercadorias/ConferirVolume", data, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Volume conferido com sucesso");

                    _gridProdutosSeparacao.CarregarGrid();
                    LimparCamposProdutoSeparado();
                    _separacaoMercadorias.Posicao.get$().focus();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    //_separacaoMercadorias.CodigoBarras.val("");
                    _separacaoMercadorias.CodigoBarras.get$().focus();
                }
            });
        }
    }, 100);
}

function adicionarClick(e, sender) {
    Salvar(_separacaoMercadorias, "SeparacaoMercadorias/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridProdutosSeparacao.CarregarGrid();
                LimparCamposProdutoSeparado();
                _separacaoMercadorias.Posicao.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function finalizarClick(e, sender) {
    exibirConfirmacao("Finalizar Separação", "Você tem certeza que deseja Finalizar a Separação?", function () {
        executarReST("SeparacaoMercadorias/Finalizar", { Codigo: _separacaoMercadorias.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridSeparacaoMercadorias.CarregarGrid();
                    limparCamposSeparacaoMercadorias();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function cancelarClick(e, sender) {
    exibirConfirmacao("Cancelar Separação", "Você tem certeza que deseja Cancelar a Separação?", function () {
        executarReST("SeparacaoMercadorias/Cancelar", { Codigo: _separacaoMercadorias.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                    _gridSeparacaoMercadorias.CarregarGrid();
                    limparCamposSeparacaoMercadorias();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, sender);
    });
}

function gerarImpressaoClick(e, sender) {
    executarDownload("SeparacaoMercadorias/Imprimir", { Codigo: _separacaoMercadorias.Codigo.val() });
}

function limparClick(e) {
    limparCamposSeparacaoMercadorias();
}

function editarSeparacaoMercadoriasClick(itemGrid) {
    // Limpa os campos
    limparCamposSeparacaoMercadorias();

    // Seta o codigo do objeto
    _separacaoMercadorias.Codigo.val(itemGrid.Codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_separacaoMercadorias, "SeparacaoMercadorias/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                // Esconde pesqusia
                _pesquisaSeparacaoMercadorias.ExibirFiltros.visibleFade(false);
                _separacaoMercadorias.Codigo.visibleFade(true);

                _gridProdutosSeparacao.CarregarGrid();

                if (_separacaoMercadorias.Situacao.val() != EnumSituacaoSelecaoSeparacao.Pendente) {
                    _separacaoMercadorias.Finalizar.visible(false);
                    _separacaoMercadorias.Cancelar.visible(false);

                    _separacaoMercadorias.Adicionar.visible(false);
                    _separacaoMercadorias.Posicao.visible(false);
                }

                if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
                    _separacaoMercadorias.Adicionar.visible(false);

                // Alternas os campos de CRUD
                _separacaoMercadorias.GerarImpressao.visible(true);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function ChangeProdutoEmbarcador() {
    if (_separacaoMercadorias.ProdutoEmbarcador.val() != "")
        BuscarProdutoEmbarcadorSeparacao(_separacaoMercadorias.ProdutoEmbarcador.val());
}

function ChangePosicao() {
    if (_separacaoMercadorias.Posicao.val() != "")
        BuscarPosicaoSeparacao(_separacaoMercadorias.Posicao.val());
}



//*******MÉTODOS*******
function buscarSeparacaoMercadorias() {
    //-- Grid
    // Opcoes
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSeparacaoMercadoriasClick, tamanho: "20", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    // Inicia Grid de busca
    _gridSeparacaoMercadorias = new GridView(_pesquisaSeparacaoMercadorias.Pesquisar.idGrid, "SeparacaoMercadorias/Pesquisa", _pesquisaSeparacaoMercadorias, menuOpcoes, null);
    _gridSeparacaoMercadorias.CarregarGrid();
}

function GridProdutosSeparacao() {
    _gridProdutosSeparacao = new GridView(_separacaoMercadorias.Produtos.idGrid, "SeparacaoMercadorias/ProdutosSeparacao", _separacaoMercadorias);
}

function limparCamposSeparacaoMercadorias() {
    _pesquisaSeparacaoMercadorias.ExibirFiltros.visibleFade(true);
    _separacaoMercadorias.Codigo.visibleFade(false);
    LimparCampos(_separacaoMercadorias);

    _separacaoMercadorias.Finalizar.visible(true);
    _separacaoMercadorias.Cancelar.visible(true);
    _separacaoMercadorias.Posicao.visible(true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador)
        _separacaoMercadorias.Adicionar.visible(true);
    else
        _separacaoMercadorias.Adicionar.visible(false);
}

function LimparCamposProdutoSeparado() {
    _separacaoMercadorias.Posicao.val(_separacaoMercadorias.Posicao.def);
    _separacaoMercadorias.Posicao.codEntity(_separacaoMercadorias.Posicao.defCodEntity);
    _separacaoMercadorias.ProdutoEmbarcador.val(_separacaoMercadorias.ProdutoEmbarcador.def);
    _separacaoMercadorias.ProdutoEmbarcador.codEntity(_separacaoMercadorias.ProdutoEmbarcador.defCodEntity);
    _separacaoMercadorias.Quantidade.val(_separacaoMercadorias.Quantidade.def);
    _separacaoMercadorias.CodigoBarras.val("");
}

function BuscarProdutoEmbarcadorSeparacao(produto) {
    executarReST("SeparacaoMercadorias/BuscarProdutoEmbarcador", { Separacao: _separacaoMercadorias.Codigo.val(), Produto: produto.trim() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _separacaoMercadorias.ProdutoEmbarcador.val(arg.Data.Descricao);
                _separacaoMercadorias.ProdutoEmbarcador.codEntity(arg.Data.Codigo);
                _separacaoMercadorias.Quantidade.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                _separacaoMercadorias.ProdutoEmbarcador.get$().focus();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function BuscarPosicaoSeparacao(posicao) {
    executarReST("SeparacaoMercadorias/BuscarPosicao", { Separacao: _separacaoMercadorias.Codigo.val(), Posicao: posicao.trim() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _separacaoMercadorias.Posicao.val(arg.Data.Abreviacao);
                _separacaoMercadorias.Posicao.codEntity(arg.Data.Codigo);
                _separacaoMercadorias.ProdutoEmbarcador.get$().focus();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function AbrirCameraPosicao() {    
    LerCodigoBarrasCameraPosicao();    
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