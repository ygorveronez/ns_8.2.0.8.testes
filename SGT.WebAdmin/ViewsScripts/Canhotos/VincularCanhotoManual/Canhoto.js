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


//*******MAPEAMENTO KNOUCKOUT*******

var _canhoto;

var Canhoto = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SemImagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Imagem = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.PDFViewer = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.Canhoto = PropertyEntity({ text: "Canhoto:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataEntrega = PropertyEntity({ getType: typesKnockout.date, text: "Data Entrega: ", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: ConfirmarClick, type: types.event, text: "Confirmar", visible: ko.observable(true) });
    this.Descartar = PropertyEntity({ eventClick: DescartarClick, type: types.event, text: "Descartar", visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadCanhoto() {
    //-- Knouckout
    // Instancia objeto principal
    _canhoto = new Canhoto();
    KoBindings(_canhoto, "knockoutCanhoto");

    $('#divModalVincularCanhoto').on('hidden.bs.modal', ModalOcultou);

    new BuscarCanhotos(_canhoto.Canhoto, RetornoCanhoto);
}

function ConfirmarClick(e, sender) {
    Salvar(_canhoto, "VincularCanhotoManual/Confirmar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                ConfirmadoOuDescartado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function DescartarClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja descartar essa digitalização?", function () {
        ExcluirPorCodigo(_canhoto, "VincularCanhotoManual/Descartar", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Descartado com sucesso");
                    ConfirmadoOuDescartado();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function abrirCanhotoClick(itemGrid) {
    // Limpa os campos
    LimparCamposCanhoto();

    BuscarImagemPorCodigo(itemGrid.Codigo);
}



//*******MÉTODOS*******
function BuscarImagemPorCodigo(codigo) {
    _canhoto.Codigo.val(codigo);

    BuscarPorCodigo(_canhoto, "VincularCanhotoManual/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                AlteraControlePassagem(arg.Data.Codigo);

                if (arg.Data.Extensao == EnumExtensaoArquivo.PDF) {
                    _canhoto.SemImagem.val(false);
                    _canhoto.Imagem.visible(false);
                    _canhoto.PDFViewer.get$().attr("src", GerarURLRenderizacao(arg.Data.Codigo));
                    _canhoto.PDFViewer.visible(true);
                }
                else {
                    _canhoto.PDFViewer.visible(false);

                    if (arg.Data.Imagem) {
                        _canhoto.SemImagem.val(false);
                        _canhoto.Imagem.visible(true);
                        _canhoto.Imagem.get$().attr("src", "data:image/png;base64," + arg.Data.Imagem);
                    }
                    else
                        _canhoto.SemImagem.val(true);
                }

                if (_canhoto.Situacao.val() != EnumSituacaoleituraImagemCanhoto.ImagemNaoReconhecida && _canhoto.Situacao.val() != EnumSituacaoleituraImagemCanhoto.SemCanhoto && _canhoto.Situacao.val() != EnumSituacaoleituraImagemCanhoto.FalhaProcessamento) {
                    _canhoto.Confirmar.visible(false);
                    _canhoto.Descartar.visible(false);
                    _canhoto.Canhoto.visible(false);
                }

                if (!$("#divModalVincularCanhoto").is(":visible"))
                    Global.abrirModal('divModalVincularCanhoto');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }, null);
}

function GerarURLRenderizacao(codigo) {
    return "Canhotos/RenderizarPDF?Codigo=" + codigo;
}

function RetornoCanhoto(canhoto) {
    _canhoto.Canhoto.val(canhoto.Descricao);
    _canhoto.Canhoto.codEntity(canhoto.Codigo);
    _canhoto.Confirmar.get$().focus();
}

function LimparCamposCanhoto() {
    _canhoto.Descartar.visible(true);
    _canhoto.Confirmar.visible(true);
    _canhoto.Canhoto.visible(true);
    LimparCampos(_canhoto);
}

function AlteraControlePassagem(codigo) {
    _indexCanhotoAberto = -1;
    for (var i in _dataGridCarregada) {
        if (_dataGridCarregada[i].DT_RowId == codigo) {
            _indexCanhotoAberto = parseInt(i);
            break;
        }
    }
}

function ProximaImagem() {
    _indexCanhotoAberto++;
    if (_indexCanhotoAberto < _dataGridCarregada.length) {
        var codigo = _dataGridCarregada[_indexCanhotoAberto].Codigo;
        BuscarImagemPorCodigo(codigo);
    } else {
        Global.fecharModal("divModalVincularCanhoto");
        RecarregarGrid();
    }
}

function ConfirmadoOuDescartado() {
    if (_buscandoApenasNaoReconhecidos) {
        _atualizarAoFechar = true;
        if (_dataGridCarregada.length <= (_indexCanhotoAberto + 1))
            RecarregarGrid(ProximaImagem);
        else
            ProximaImagem();
    } else {
        _gridCanhoto.CarregarGrid();
        LimparCamposCanhoto();
        Global.fecharModal("divModalVincularCanhoto");
    }
}