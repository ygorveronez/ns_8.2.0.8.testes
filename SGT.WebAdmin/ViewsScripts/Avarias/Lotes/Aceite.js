/// <reference path="EtapasLote.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLote.js" />
/// <reference path="../../Enumeradores/EnumResponsavelAvaria.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _aceite;
var _gridAnexosAceite;

var Aceite = function () {
    this.Codigo = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Lote = PropertyEntity({ type: types.map, getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    // Mensagem
    this.Mensagem = PropertyEntity({ text: 'Aguardando a aceitação do transportador', visible: ko.observable(true) });

    // Detalhes
    this.Solicitante = PropertyEntity({ text: "Solicitante:", val: ko.observable("") });
    this.DataSolicitacao = PropertyEntity({ text: "Data da Solicitação:", val: ko.observable("") });
    this.Responsavel = PropertyEntity({ text: "Responsável:", val: ko.observable("") });
    this.DataRetorno = PropertyEntity({ text: "Data do Retorno:", val: ko.observable("") });
    this.SituacaoRetorno = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", val: ko.observable("") });
    this.ResponsavelAvaria = PropertyEntity({ val: ko.observable(EnumResponsavelAvaria.Transportador), def: EnumResponsavelAvaria.Transportador });

    // Grid Anexos
    this.Anexos = PropertyEntity({ type: types.local, text: "Anexos Enviados Pelo Transportador", val: ko.observable(""), idGrid: guid(), visible: ko.observable(true) });
};

//*******EVENTOS*******

function loadAceite() {
    _aceite = new Aceite();
    KoBindings(_aceite, "knockoutAceiteTransportador");

    GridAnexosAceite();
}

function downloadAnexoAceiteClick(dataRow) {
    var data = { Codigo: dataRow.Codigo };
    executarDownload("AceiteLoteAvaria/DownloadAnexo", data);
}

//*******MÉTODOS*******

function GridAnexosAceite() {
    //-- Grid Anexos
    // Opcoes
    var download = {
        descricao: "Download",
        id: guid(),
        metodo: downloadAnexoAceiteClick,
        tamanho: 5,
        icone: ""
    };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        descricao: "Download",
        opcoes: [download]
    };

    _gridAnexosAceite = new GridView(_aceite.Anexos.idGrid, "AceiteLoteAvaria/PesquisaAnexo", _aceite, menuOpcoes);
}

function AceiteTransportador() {
    var situacao = _lote.Situacao.val();
    if (situacao != EnumSituacaoLote.AgIntegracao &&
        situacao != EnumSituacaoLote.AgAprovacaoIntegracao &&
        situacao != EnumSituacaoLote.Reprovacao &&
        situacao != EnumSituacaoLote.IntegracaoReprovada &&
        situacao != EnumSituacaoLote.EmCorrecao &&
        situacao != EnumSituacaoLote.Finalizada &&
        situacao != EnumSituacaoLote.FalhaIntegracao &&
        situacao != EnumSituacaoLote.EmIntegracao
    )
        return;

    _aceite.Codigo.val(_lote.Codigo.val());
    _aceite.Lote.val(_lote.Codigo.val());
    _gridAnexosAceite.CarregarGrid();

    BuscarPorCodigo(_aceite, "AceiteLoteAvaria/DetalhesPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data == false)
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
            else {
                if (_aceite.ResponsavelAvaria.val() == EnumResponsavelAvaria.CarregamentoDescarregamento) {
                    _aceite.Anexos.visible(false);
                } else {
                    _aceite.Anexos.visible(true);
                }
            }
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    });
}