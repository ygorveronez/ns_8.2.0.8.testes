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
/// <reference path="../../../js/libs/jquery.twbsPagination.js" />
/// <reference path="../../Consultas/Localidade.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/Motorista.js" />
/// <reference path="../../Consultas/Veiculo.js" />
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="Carga.js" />
/// <reference path="CargaCTe.js" />
/// <reference path="../../Enumeradores/EnumSituacoesCarga.js" />
/// <reference path="EncerramentoCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Consultas/Filial.js" />

//*******MAPEAMENTO KNOUCKOUT*******

//var PesquisaCanhoto = function () {
//    this.Carga = PropertyEntity({ getType: typesKnockout.int });
//}   

var PaginaOrigem = { enviarCanhoto: "enviarCanhoto", encerrarCarga: "EncerrarCarga" };

var _knoutArquivo;
var _knouJustificativa;
var _knoutPesquisar;
var _gridCanhotos;



var PesquisaCanhoto = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.Chave = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.ChaveNF-e.getFieldDescription(), maxlength: 44 });
    this.SomentePendetes = PropertyEntity({ getType: typesKnockout.bool, visible: false, def: false, val: ko.observable(false) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Filial.getFieldDescription(), idBtnSearch: guid() });
    this.NumeroNF = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.NumeroNF-e.getFieldDescription(), getType: typesKnockout.int, maxlength: 9, configInt: { precision: 0, allowZero: false, thousands: "" } });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.Transportador.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCanhotos.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

}


var Canhoto = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int });
    this.XMLNotaFiscal = PropertyEntity({ getType: typesKnockout.int });
    this.NumeroNota = PropertyEntity({ getType: typesKnockout.int });
    this.CodigoCanhoto = PropertyEntity({ getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.EncerramentoCarga.Justificativa.getFieldDescription() , maxlength: 300, required: true });

    this.Justificar = PropertyEntity({ eventClick: enviarJustificativaClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.Justificar, visible: ko.observable(true) });
}

//*******EVENTOS*******

var _paginaOrigem;
function loadCanhoto(paginaOrigem) {
    _paginaOrigem = paginaOrigem;
    _knoutArquivo = new Canhoto();
    KoBindings(_knoutArquivo, "knoutEnviarArquivo");

    _knouJustificativa = new Canhoto();
    KoBindings(_knouJustificativa, "knoutJustificarCanhoto");

    $("#" + _knoutArquivo.Arquivo.id).on("change", enviarCanhotoClick);

    _knoutPesquisar = new PesquisaCanhoto();
    if (paginaOrigem == PaginaOrigem.enviarCanhoto) {
        KoBindings(_knoutPesquisar, "knockoutPesquisaCanhotos", false, _knoutPesquisar.Pesquisar.id);
        new BuscarTransportadores(_knoutPesquisar.Empresa);
        new BuscarFilial(_knoutPesquisar.Filial);
        _knoutPesquisar.SomentePendetes.val(true);
    } else {
        _knoutPesquisar.Carga.val(_carga.Codigo.val());
    }

    buscarCanhotos();
}

function enviarCanhotoClick() {
    

    var file = document.getElementById(_knoutArquivo.Arquivo.id);
    exibirConfirmacao(Localization.Resources.Cargas.EncerramentoCarga.Confirmarçao, Localization.Resources.Cargas.EncerramentoCarga.RealmenteDesejaEnviarArquivoCanhotoNotaFiscal.format(file.files[0].name, _knoutArquivo.NumeroNota.val()), function () {

        var formData = new FormData();
        formData.append("upload", file.files[0]);
        var data = {
            XMLNotaFiscal: _knoutArquivo.XMLNotaFiscal.val(),
            encerrarCargaAutomaticamente: _paginaOrigem == PaginaOrigem.enviarCanhoto ? true : false
        };

        enviarArquivo("CargaCanhoto/EnviarCanhoto?callback=?", data, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoEnviadoSucesso);
                    buscarCanhotos();
                    VerificarEncerramentoLiberado();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, F, arg.Msg);
            }
        });
    });
}

function enviarJustificativaClick() {

    if (ValidarCamposObrigatorios(_knouJustificativa)) {
        var dados = {
            XMLNotaFiscal: _knouJustificativa.XMLNotaFiscal.val(),
            Observacao: _knouJustificativa.Observacao.val(),
            encerrarCargaAutomaticamente: _paginaOrigem == PaginaOrigem.enviarCanhoto ? true : false
        };
        executarReST("CargaCanhoto/Justificar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.JustificativaEnviadaSucesso);
                    Global.fecharModal('divModalJustificarCanhoto');
                    buscarCanhotos();
                    VerificarEncerramentoLiberado();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoObrigatorio, Localization.Resources.Gerais.Geral.CampoObrigatorio , Localization.Resources.Gerais.Geral.InformeJustificativas);
    }
}

function abrirModalEnviarCanhotoClick(e) {
    _knoutArquivo.XMLNotaFiscal.val(e.Codigo);
    _knoutArquivo.NumeroNota.val(e.Numero);
    _knoutArquivo.CodigoCanhoto.val(e.CodigoCanhoto);
    $("#" + _knoutArquivo.Arquivo.id).trigger("click");
}

function abrirModalJustificativaClick(e) {
    _knouJustificativa.XMLNotaFiscal.val(e.Codigo);
    _knouJustificativa.NumeroNota.val(e.Numero);
    _knouJustificativa.CodigoCanhoto.val(e.CodigoCanhoto);
    _knouJustificativa.Observacao.val(e.Observacao);    
    Global.abrirModal("divModalJustificarCanhoto");
}

function RemoverArquivoClick(id) {
    uploader.removeFile(uploader.getFile(id));
    $("#" + id).remove();
}

function downloadCanhotoClick(e) {
    if (e.CodigoCanhoto > 0 && e.GuidNomeArquivo != "") {
        var dados = {
            Codigo: e.Codigo
        }
        executarDownload("CargaCanhoto/DownloadCanhoto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoNaoEnviado, Localization.Resources.Cargas.EncerramentoCarga.NaoEnviadoCanhotoNota);
    }
}
 
function excluirCanhotoClick(e) {
    if (e.CodigoCanhoto > 0) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.EncerramentoCarga.RealmenteExcluirCanhoto, function () {
            var dados = {
                Codigo: e.Codigo
            }
            executarReST("CargaCanhoto/ExcluirCanhoto", dados, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoExcluidoSucesso);
                    buscarCanhotos();
                    VerificarEncerramentoLiberado();
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            })
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoNaoEnviado, Localization.Resources.Cargas.EncerramentoCarga.NaoEnviadoCanhotoNota);
    }
}

function buscarCanhotos() {
    var EnviarArquivo = { descricao: Localization.Resources.Cargas.EncerramentoCarga.EnviarCanhoto, id: guid(), tamanho: 9, metodo: abrirModalEnviarCanhotoClick };
    var Justificar = { descricao: Localization.Resources.Cargas.EncerramentoCarga.Justificar, id: guid(), tamanho: 9, metodo: abrirModalJustificativaClick };
    var Download = { descricao: Localization.Resources.Cargas.EncerramentoCarga.DownloadCanhoto, id: guid(), metodo: downloadCanhotoClick };
    var ExcluirCanhoto = { descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirCanhotoClick };
    var menuOpcoes;

    if (_paginaOrigem == PaginaOrigem.encerrarCarga) {
        var idGrid = _carga.Canhoto.idGrid;
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [EnviarArquivo, Justificar, ExcluirCanhoto, Download] };
    } else {
        var idGrid = _knoutPesquisar.Pesquisar.idGrid;
        menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 18, opcoes: [Justificar, EnviarArquivo] };
    }
    _gridCanhotos = new GridView(idGrid, "CargaCanhoto/BuscarCanhotos", _knoutPesquisar, menuOpcoes);
    _gridCanhotos.CarregarGrid();
}


