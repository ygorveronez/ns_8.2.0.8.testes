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
/// <reference path="../../Enumeradores/EnumSituacaoOcorrencia.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _aceiteDebito;
var _gridAnexosAceiteDebito;

var AceiteDebito = function () {
    this.AceitePendente = PropertyEntity({ val: ko.observable(false) });
    this.AceiteRejeitado = PropertyEntity({ val: ko.observable(false) });
    this.AceiteAprovado = PropertyEntity({ val: ko.observable(false) });

    this.Observacao = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.Observacao.getFieldDescription() });
    this.Usuario = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.Usuario.getFieldDescription()  });
    this.DataRetorno = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Ocorrencias.Ocorrencia.DataRetorno.getFieldDescription()  });

    this.Anexos = PropertyEntity({ type: types.local, idGrid: guid() });
    
}

//*******EVENTOS*******

function loadAceiteDebito() {
    _aceiteDebito = new AceiteDebito();
    KoBindings(_aceiteDebito, "knockoutAceiteDebito");

    GridAnexosAceiteDebito();
}

function downloadAnexoAceiteClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("AceiteDebitoAnexo/DownloadAnexo", data);
}



//*******MÉTODOS*******
function EditarAceiteDebito() {
    var situacao = _ocorrencia.SituacaoOcorrencia.val();

    _aceiteDebito.AceitePendente.val(false);
    _aceiteDebito.AceiteRejeitado.val(false);
    _aceiteDebito.AceiteAprovado.val(false);

    var buscarDetalhes = true;

    if (situacao == EnumSituacaoOcorrencia.AgAceiteTransportador) {
        _aceiteDebito.AceitePendente.val(true);
        buscarDetalhes = false;
    } else if (situacao == EnumSituacaoOcorrencia.DebitoRejeitadoTransportador) {
        _aceiteDebito.AceiteRejeitado.val(true);
    } else {
        _aceiteDebito.AceiteAprovado.val(true);
    }

    if (buscarDetalhes) {
        executarReST("AceiteDebito/DetalhesAceite", { Codigo: _ocorrencia.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    PreencherObjetoKnout(_aceiteDebito, arg);
                    _gridAnexosAceiteDebito.CarregarGrid();
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    }
}

function GridAnexosAceiteDebito() {
    //-- Grid Anexos
    // Opcoes
    var download = { descricao: Localization.Resources.Ocorrencias.Ocorrencia.Download, id: guid(), tamanho: 3, metodo: downloadAnexoAceiteClick, icone: "" };

    // Menu
    var menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Ocorrencias.Ocorrencia.Download, opcoes: [download] };

    var ko_ocorrencia = {
        Codigo: _ocorrencia.Codigo
    };

    _gridAnexosAceiteDebito = new GridView(_aceiteDebito.Anexos.idGrid, "AceiteDebitoAnexo/Pesquisa", ko_ocorrencia, menuOpcoes);
}