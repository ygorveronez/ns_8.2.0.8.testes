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
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="Canhoto.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var PaginaOrigemAvulso = { enviarCanhoto: "enviarCanhoto", encerrarCarga: "EncerrarCarga" };

var _knoutArquivoAvulso;
var _knouJustificativaAvulso;
var _knoutPesquisarAvulso;
var _gridCanhotosAvulsos;


var PesquisaCanhotoAvulso = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.getFieldDescription(), val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.SomentePendetes = PropertyEntity({ getType: typesKnockout.bool, visible: false, def: false, val: ko.observable(false) });

    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.Transportador, idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.Filial, idBtnSearch: guid() });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.Recebedor.getFieldDescription(), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridCanhotosAvulsos.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.NumerodaCarga.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });

}


var CanhotoAvulso = function () {
    this.Carga = PropertyEntity({ getType: typesKnockout.int });
    this.CargaCanhotoAvulso = PropertyEntity({ getType: typesKnockout.int });
    this.NumeroNota = PropertyEntity({ getType: typesKnockout.int });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false });
    this.Observacao = PropertyEntity({ getType: typesKnockout.string, text: Localization.Resources.Cargas.EncerramentoCarga.Justificativa.getFieldDescription(), maxlength: 300, required: true });

    this.Justificar = PropertyEntity({ eventClick: enviarJustificativaAvulsoClick, enable: ko.observable(false), type: types.event, text: Localization.Resources.Cargas.EncerramentoCarga.Justificar, visible: ko.observable(true) });
}

//*******EVENTOS*******

var _paginaOrigemAvulso;
function loadCanhotoAvulso(paginaOrigem) {
    _paginaOrigemAvulso = paginaOrigem;
    _knoutArquivoAvulso = new CanhotoAvulso();
    KoBindings(_knoutArquivoAvulso, "knoutEnviarArquivoAvulso");

    _knouJustificativaAvulso = new CanhotoAvulso();
    KoBindings(_knouJustificativaAvulso, "knoutJustificarCanhotoAvulso");

    $("#" + _knoutArquivoAvulso.Arquivo.id).on("change", enviarCanhotoAvulsoClick);

    _knoutPesquisarAvulso = new PesquisaCanhotoAvulso();
    if (paginaOrigem == PaginaOrigemAvulso.enviarCanhoto) {
        KoBindings(_knoutPesquisarAvulso, "knockoutPesquisaCanhotosAvulsos", false, _knoutPesquisarAvulso.Pesquisar.id);
        new BuscarTransportadores(_knoutPesquisarAvulso.Empresa);
        new BuscarFilial(_knoutPesquisarAvulso.Filial);
        new BuscarClientes(_knoutPesquisarAvulso.Recebedor);
        _knoutPesquisarAvulso.SomentePendetes.val(true);
    } else {
        //_knoutPesquisarAvulso.Carga.val(_carga.Codigo.val());
    }

    buscarCanhotosAvulsos();
}

function enviarCanhotoAvulsoClick() {

    var file = document.getElementById(_knoutArquivoAvulso.Arquivo.id);
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.EncerramentoCarga.RealmenteEnviarArquivo + file.files[0].name + Localization.Resources.Cargas.EncerramentoCarga.DeCanhotoAvulso, function () {

        var formData = new FormData();
        formData.append("upload", file.files[0]);
        var data = {
            CargaCanhotoAvulso: _knoutArquivoAvulso.CargaCanhotoAvulso.val(),
            encerrarCargaAutomaticamente: _paginaOrigemAvulso == PaginaOrigemAvulso.enviarCanhoto ? true : false
        };

        enviarArquivo("CargaCanhotoAvulso/EnviarCanhoto?callback=?", data, formData, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoAvulsoEnviadoSucesso);
                    buscarCanhotosAvulsos();
                    VerificarEncerramentoLiberado();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

function enviarJustificativaAvulsoClick() {

    if (ValidarCamposObrigatorios(_knouJustificativaAvulso)) {
        var dados = {
            CargaCanhotoAvulso: _knouJustificativaAvulso.CargaCanhotoAvulso.val(),
            Observacao: _knouJustificativaAvulso.Observacao.val(),
            encerrarCargaAutomaticamente: _paginaOrigemAvulso == PaginaOrigemAvulso.enviarCanhoto ? true : false
        };
        executarReST("CargaCanhotoAvulso/Justificar", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.JustificativaEnviadaSucesso);
                    Global.fecharModal('divModalJustificarCanhotoAvulso');
                    buscarCanhotosAvulsos();
                    VerificarEncerramentoLiberado();
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.GeraisInformeJustificativa);
    }
}

function abrirModalEnviarCanhotoAvulsoClick(e) {
    _knoutArquivoAvulso.CargaCanhotoAvulso.val(e.Codigo);
    _knoutArquivoAvulso.Codigo.val(e.Codigo);
    $("#" + _knoutArquivoAvulso.Arquivo.id).trigger("click");
}

function abrirModalJustificativaAvulsoClick(e) {
    _knouJustificativaAvulso.CargaCanhotoAvulso.val(e.Codigo);
    _knouJustificativaAvulso.NumeroNota.val(e.Numero);
    _knouJustificativaAvulso.Codigo.val(e.Codigo);
    _knouJustificativaAvulso.Observacao.val(e.Observacao);    
    Global.abrirModal("divModalJustificarCanhotoAvulso");
}

function downloadCanhotoAvulsoClick(e) {
    if (e.CodigoCanhoto > 0 && e.GuidNomeArquivo != "") {
        var dados = {
            CargaCanhotoAvulso: e.Codigo
        }
        executarDownload("CargaCanhotoAvulso/DownloadCanhoto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoNaoEnviado, Localization.Resources.Cargas.EncerramentoCarga.CanhotoAvulsoNãoEnviado);
    }
}

function excluirCanhotoAvulsoClick(e) {
    if (e.CodigoCanhoto > 0) {
        exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.EncerramentoCarga.RealmenteExcluirCanhoto, function () {
            var dados = {
                CargaCanhotoAvulso: e.Codigo
            }
            executarReST("CargaCanhotoAvulso/ExcluirCanhoto", dados, function (arg) {
                if (arg.Success) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoAvulsoExcluídoSucesso);
                    buscarCanhotosAvulsos();
                    VerificarEncerramentoLiberado();
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            })
        });
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.EncerramentoCarga.CanhotoNaoEnviado, Localization.Resources.Cargas.EncerramentoCarga.CanhotoAvulsoNãoEnviado);
    }
}

function buscarCanhotosAvulsos() {
    var EnviarArquivo = { descricao: Localization.Resources.Gerais.Geral.Enviar, id: guid(), tamanho: 9, metodo: abrirModalEnviarCanhotoAvulsoClick };
    var Justificar = { descricao: Localization.Resources.Gerais.Geral.Justificar, id: guid(), tamanho: 9, metodo: abrirModalJustificativaAvulsoClick };
    var Download = { descricao: Localization.Resources.Cargas.EncerramentoCarga.DownloadCanhotoAvulso, id: guid(), metodo: downloadCanhotoAvulsoClick };
    var ExcluirCanhoto = { descricao: Localization.Resources.Gerais.Geral.ExcluirCanhoto, id: guid(), metodo: excluirCanhotoAvulsoClick };
    var menuOpcoes;

    if (_paginaOrigemAvulso == PaginaOrigemAvulso.encerrarCarga) {
        var idGrid = _carga.CanhotoAvulso.idGrid;
        menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 7, opcoes: [EnviarArquivo, Justificar, ExcluirCanhoto, Download] };
    } else {
        var idGrid = _knoutPesquisarAvulso.Pesquisar.idGrid;
        menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 18, opcoes: [Justificar, EnviarArquivo] };
    }
    _gridCanhotosAvulsos = new GridView(idGrid, "CargaCanhotoAvulso/BuscarCanhotos", _knoutPesquisarAvulso, menuOpcoes);
    _gridCanhotosAvulsos.CarregarGrid();
}


