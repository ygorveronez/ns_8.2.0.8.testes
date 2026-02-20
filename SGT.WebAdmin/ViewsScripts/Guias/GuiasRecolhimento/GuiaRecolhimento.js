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

var _guiaRecolhimento;
var _dataGridCarregada = [];
var _indexGuiaAberto = -1;


var GuiaRecolhimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoAnexo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SemImagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Imagem = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.PDFViewer = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.DataEntrega = PropertyEntity({ getType: typesKnockout.date, text: "Data Entrega: ", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.GuiaRecolhimento = PropertyEntity({ text: "Guia:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Confirmar = PropertyEntity({ eventClick: ConfirmarClick, type: types.event, text: "Confirmar", visible: ko.observable(true) });
    this.Descartar = PropertyEntity({ eventClick: DescartarClick, type: types.event, text: "Descartar", visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadGuiaRecolhimento() {
    //-- Knouckout
    // Instancia objeto principal
    _guiaRecolhimento = new GuiaRecolhimento();
    KoBindings(_guiaRecolhimento, "knockoutGuiaRecolhimento");

    $('#divModalVincularGuiaRecolhimento').on('hidden.bs.modal', ModalOcultou);
}

function ConfirmarClick(e, sender) {
    Salvar(_guiaRecolhimento, "VincularGuia/Confirmar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                Global.fecharModal("divModalVincularGuiaRecolhimento");
                recarregarGridGuias();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function DescartarClick(e, sender) {
    var dados = RetornarObjetoPesquisa(_guiaRecolhimento)

    exibirConfirmacao("Confirmação", "Realmente deseja descartar essa digitalização?", function () {
        executarReST("GuiaNacionalRecolhimentoTributoEstual/Descartar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Descartado com sucesso");
                    _atualizarGridAoFecharModal = true;
                    Global.fecharModal("divModalVincularGuiaRecolhimento");
                    recarregarGridGuias();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function abrirGuiaRecolhimentoClick(itemGrid) {
    // Limpa os campos
    LimparCamposGuiaRecolhimento();

    BuscarImagemPorCodigo(itemGrid.Codigo, 1);
}

function abrirComprovanteClick(itemGrid) {
    // Limpa os campos
    LimparCamposGuiaRecolhimento();

    BuscarImagemPorCodigo(itemGrid.Codigo, 2);
}



//*******MÉTODOS*******
function BuscarImagemPorCodigo(codigo, tipoAnexo) {

    _guiaRecolhimento.Codigo.val(codigo);
    _guiaRecolhimento.TipoAnexo.val(tipoAnexo)

    var dados = {
        Codigo: codigo,
        TipoAnexo: tipoAnexo
    }

    executarReST("GuiaNacionalRecolhimentoTributoEstual/BuscarPorCodigo", dados, function (arg) {
        if (arg.Success) {
            console.log(arg.Data)
            if (arg.Data) {
                AlteraControlePassagem(arg.Data.Codigo);

                if (arg.Data.Extensao == "pdf") {
                    _guiaRecolhimento.SemImagem.val(false);
                    _guiaRecolhimento.Imagem.visible(false);
                    _guiaRecolhimento.PDFViewer.get$().attr("src", GerarURLRenderizacao(arg.Data.Codigo));
                    _guiaRecolhimento.PDFViewer.visible(true);
                }
                else {
                    _guiaRecolhimento.PDFViewer.visible(false);

                    if (arg.Data.Imagem) {
                        _guiaRecolhimento.SemImagem.val(false);
                        _guiaRecolhimento.Imagem.visible(true);
                        _guiaRecolhimento.Imagem.get$().attr("src", "data:image/png;base64," + arg.Data.Imagem);
                    }
                    else
                        _guiaRecolhimento.SemImagem.val(true);
                }

                if (!$("#divModalVincularGuiaRecolhimento").is(":visible"))
                    Global.abrirModal('divModalVincularGuiaRecolhimento');
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }, null);
}

function GerarURLRenderizacao(codigo) {
    return "Guias/RenderizarPDF?Codigo=" + codigo;
}

function LimparCamposGuiaRecolhimento() {
    _guiaRecolhimento.Descartar.visible(true);
    _guiaRecolhimento.Confirmar.visible(true);
    _guiaRecolhimento.GuiaRecolhimento.visible(true);
    LimparCampos(_guiaRecolhimento);
}

function AlteraControlePassagem(codigo) {
    _indexGuiaRecolhimentoAberto = -1;
    for (var i in _dataGridCarregada) {
        if (_dataGridCarregada[i].DT_RowId == codigo) {
            _indexGuiaRecolhimentoAberto = parseInt(i);
            break;
        }
    }
}