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

var _guia;

var Guia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Situacao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.SemImagem = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Imagem = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.PDFViewer = PropertyEntity({ type: types.local, visible: ko.observable(false) });
    this.TipoAnexo = PropertyEntity({ text: "Tipo Arquivo:", val: ko.observable(EnumTipoAnexoGuia.Guia), def: ko.observable(EnumTipoAnexoGuia.Guia), options: EnumTipoAnexoGuia.obterOpcoes(), visible: ko.observable(true), required: true });
    this.Guia = PropertyEntity({ text: "Guia:", type: types.entity, codEntity: ko.observable(0), required: true, idBtnSearch: guid(), visible: ko.observable(true), val: ko.observable("") });

    this.Confirmar = PropertyEntity({ eventClick: ConfirmarClick, type: types.event, text: "Confirmar", visible: ko.observable(true) });
    this.Descartar = PropertyEntity({ eventClick: ExcluirAnexo, type: types.event, text: "Excluir", visible: ko.observable(true) });
}


//*******EVENTOS*******
function loadGuia() {
    //-- Knouckout
    // Instancia objeto principal
    _guia = new Guia();
    KoBindings(_guia, "knockoutGuia");

    
    new BuscarGuiasRecolhimento(_guia.Guia, RetornoBuscaGuia);

    $('#divModalVincularGuia').on('hidden.bs.modal', ModalOcultou);
}

function ConfirmarClick(e, sender) {
    Salvar(_guia, "VincularGuia/Confirmar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Confirmado com sucesso");
                Global.fecharModal("divModalVincularGuia");
                RecarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function ExcluirAnexo(e) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        executarReST("VincularGuia/Descartar", { Codigo: e.Codigo.val() }, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Anexo excluído com sucesso");
                    Global.fecharModal("divModalVincularGuia");
                    RecarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        });
    });
}


function abrirGuiaClick(itemGrid) {
    // Limpa os campos
    LimparCamposGuia();

    BuscarImagemPorCodigo(itemGrid.Codigo);
}



//*******MÉTODOS*******
function BuscarImagemPorCodigo(codigo) {
    _guia.Codigo.val(codigo);

    BuscarPorCodigo(_guia, "VincularGuia/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            console.log(arg.Data)
            if (arg.Data) {
                AlteraControlePassagem(arg.Data.Codigo);

                if (arg.Data.Extensao == "pdf") {
                    _guia.SemImagem.val(false);
                    _guia.Imagem.visible(false);
                    _guia.PDFViewer.get$().attr("src", GerarURLRenderizacao(arg.Data.Codigo));
                    _guia.PDFViewer.visible(true);
                }
                else {
                    _guia.PDFViewer.visible(false);

                    if (arg.Data.Imagem) {
                        _guia.SemImagem.val(false);
                        _guia.Imagem.visible(true);
                        _guia.Imagem.get$().attr("src", "data:image/png;base64," + arg.Data.Imagem);
                    }
                    else
                        _guia.SemImagem.val(true);
                }

                if (!$("#divModalVincularGuia").is(":visible")) {
                    console.log(arg.Data)
                    _guia.TipoAnexo.val(arg.Data.TipoAnexo)
                    Global.abrirModal('divModalVincularGuia');

                }
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

function LimparCamposGuia() {
    _guia.Descartar.visible(true);
    _guia.Confirmar.visible(true);
    _guia.Guia.visible(true);
    LimparCampos(_guia);
}

function AlteraControlePassagem(codigo) {
    _indexGuiaAberto = -1;
    for (var i in _dataGridCarregada) {
        if (_dataGridCarregada[i].DT_RowId == codigo) {
            _indexGuiaAberto = parseInt(i);
            break;
        }
    }
}

function ProximaImagem() {
    _indexGuiaAberto++;
    if (_indexGuiaAberto < _dataGridCarregada.length) {
        var codigo = _dataGridCarregada[_indexGuiaAberto].Codigo;
        BuscarImagemPorCodigo(codigo);
    } else {
        Global.fecharModal("divModalVincularGuia");
        RecarregarGrid();
    }
}

function ConfirmadoOuDescartado() {
    if (_buscandoApenasNaoReconhecidos) {
        _atualizarAoFechar = true;
        if (_dataGridCarregada.length <= (_indexGuiaAberto + 1))
            RecarregarGrid(ProximaImagem);
        else
            ProximaImagem();
    } else {
        _gridGuia.CarregarGrid();
        LimparCamposGuia();
        Global.fecharModal("divModalVincularGuia");
    }
}

function RetornoBuscaGuia(data) {
    console.log(data)
    _guia.Guia.codEntity(data.Codigo);
    _guia.Guia.val(data.NumeroGuia);
}