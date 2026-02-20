/// <reference path="../../Consultas/Ocorrencia.js" />
/// <reference path="../../Consultas/Justificativa.js" />
/// <reference path="../../Consultas/Veiculo.js" />
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
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="CabecalhoAcertoViagem.js" />
/// <reference path="../../Consultas/Carga.js" />
/// <reference path="PedagioAcertoViagem.js" />
/// <reference path="AcertoViagem.js" />
/// <reference path="DespesaAcertoViagem.js" />
/// <reference path="AbastecimentoAcertoViagem.js" />
/// <reference path="FechamentoAcertoViagem.js" />
/// <reference path="CargaAcertoViagem.js" />
/// <reference path="EtapaAcertoViagem.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _ocorrenciaAcertoViagem;
var _gridOcorrencias;
var _HTMLOcorrenciaAcertoViagem;

var OcorrenciaAcertoViagem = function () {
    this.CodigoAcerto = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Ocorrencias = PropertyEntity({ type: types.map, required: false, text: "Adicionar Ocorrencias", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: guid(), enable: ko.observable(true) });
    this.BuscarOcorrencias = PropertyEntity({ type: types.map, required: false, text: "Buscar ocorrências do motorista", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: this.Ocorrencias.idGrid, enable: ko.observable(true) });
    this.BuscarTodasOcorrencias = PropertyEntity({ type: types.map, required: false, text: "Buscar todas ocorrências", getType: typesKnockout.dynamic, idBtnSearch: guid(), idGrid: this.Ocorrencias.idGrid, enable: ko.observable(true) });

    this.TotalOcorrencias = PropertyEntity({ text: "Total de Ocorrências: ", getType: typesKnockout.decimal, val: ko.observable(0), visible: false });

    this.ProximaEtapa = PropertyEntity({ eventClick: ProximaEtapaClick, type: types.event, text: "Salvar Ocorrências", visible: ko.observable(true), enable: ko.observable(true) });
}


//*******EVENTOS*******

function loadOcorrenciaAcertoViagem() {
    $("#contentOcorrenciaAcertoViagem").html("");
    var idDiv = guid();
    $("#contentOcorrenciaAcertoViagem").append(_HTMLOcorrenciaAcertoViagem.replace(/#ocorrenciaAcertoViagem/g, idDiv));
    _ocorrenciaAcertoViagem = new OcorrenciaAcertoViagem();
    KoBindings(_ocorrenciaAcertoViagem, idDiv);

    _ocorrenciaAcertoViagem.CodigoAcerto.val(_acertoViagem.Codigo.val());

    var remover = { descricao: "Excluir", id: "clasEditar", evento: "onclick", metodo: RemoverOcorrenciaClick, tamanho: "8", icone: "" };
    //var detalhe = { descricao: "Detalhe", id: "clasEditar", evento: "onclick", metodo: DetalheOcorrenciaClick, tamanho: "8", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    //menuOpcoes.opcoes.push(detalhe);
    menuOpcoes.opcoes.push(remover);

    _gridOcorrencias = new GridView(_ocorrenciaAcertoViagem.Ocorrencias.idGrid, "AcertoOcorrencia/PesquisarAcertoOcorrencia", _ocorrenciaAcertoViagem, menuOpcoes, null, null, null, null, null, null);
    _gridOcorrencias.CarregarGrid();

    new BuscarOcorrenciaSemAcertoDeViagem(_ocorrenciaAcertoViagem.BuscarOcorrencias, RetornoInserirOcorrenciaDoAcerto, null, _acertoViagem.Codigo);
    new BuscarOcorrenciaSemAcertoDeViagem(_ocorrenciaAcertoViagem.BuscarTodasOcorrencias, RetornoInserirOcorrenciaDoAcerto, null, _acertoViagem.Codigo, true);   
}

function CarregarOcorrenciasAcerto() {
    _ocorrenciaAcertoViagem.CodigoAcerto.val(_acertoViagem.Codigo.val());

    _gridOcorrencias.CarregarGrid();    
}

function RemoverOcorrenciaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a ocorrência selecionada?", function () {

        var data = {
            Codigo: e.Codigo,
            CodigoAcerto: _acertoViagem.Codigo.val()
        };
        executarReST("AcertoOcorrencia/RemoverOcorrencia", data, function (arg) {
            if (arg.Success) {
                _gridOcorrencias.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

//function DetalheOcorrenciaClick(e, sender) {

//    _gridOcorrencias.CarregarGrid();
//}

function RetornoInserirOcorrenciaDoAcerto(data) {

    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }

    var dataEnvio = {
        Codigo: data.Codigo,
        CodigoAcerto: _acertoViagem.Codigo.val()
    };
    executarReST("AcertoOcorrencia/InserirOcorrencia", dataEnvio, function (arg) {
        if (arg.Success) {
            _gridOcorrencias.CarregarGrid();
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });

}

function ProximaEtapaClick(e, sender) {
    if (_acertoViagem == null || _acertoViagem.Codigo == null || _acertoViagem.Codigo.val() == null || _acertoViagem.Codigo.val() == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Por favor inicie o acerto de viagem antes.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Fechado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem já se encontra finalizado.");
        return;
    }
    if (_acertoViagem.Situacao.val() == EnumSituacoesAcertoViagem.Cancelado) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Este acerto de viagem se encontra cancelado.");
        return;
    }
    preencherListasSelecao();
    _acertoViagem.Etapa.val(EnumEtapasAcertoViagem.Ocorrencias);
    Salvar(_acertoViagem, "AcertoOcorrencia/AtualizarOcorrencias", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Ocorrencias salvas com sucesso.");

                CarregarDadosAcertoViagem(arg.Data.Codigo, null, EnumEtapaAcertoViagem.Ocorrencias);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender, exibirCamposObrigatorio);
}


//*******MÉTODOS*******