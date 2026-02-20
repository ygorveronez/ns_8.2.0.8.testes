/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/TipoCarga.js" />
/// <reference path="../../Consultas/TipoIntegracao.js" />
/// <reference path="GrupoPessoas.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoIntegracoes;
var _grupoPessoasTipoIntegracoes;

var TipoIntegracoes = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.CadastrarTipoIntegracoes = PropertyEntity({ type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.AdicionarTipoIntegracoes, idBtnSearch: guid() });
};

//*******EVENTOS*******

function LoadGrupoPessoasTipoIntegracoes() {
    _grupoPessoasTipoIntegracoes = new TipoIntegracoes();
    KoBindings(_grupoPessoasTipoIntegracoes, "knockoutGrupoPessoasIntegracao");

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), metodo: function (data) {
                ExcluirTipoIntegracoesClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Pessoas.GrupoPessoas.TipoIntegracoes, width: "70%" },
    ];

    _gridTipoIntegracoes = new BasicDataTable(_grupoPessoasTipoIntegracoes.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarTipoIntegracao(_grupoPessoasTipoIntegracoes.CadastrarTipoIntegracoes, null, null, null, null, null, _gridTipoIntegracoes);

    RecarregarTipoIntegracoes();
}

function RecarregarTipoIntegracoes() {
    var data = new Array();
    if (_grupoPessoas.TipoIntegracoes.val() != "") {
        $.each(_grupoPessoas.TipoIntegracoes.val(), function (i, TipoIntegracoes) {
            var tipoIntegracoesGrid = new Object();

            tipoIntegracoesGrid.Codigo = TipoIntegracoes.Codigo;
            tipoIntegracoesGrid.Descricao = TipoIntegracoes.Descricao;

            data.push(tipoIntegracoesGrid);
        });
    }
    _gridTipoIntegracoes.CarregarGrid(data);
}

function ExcluirTipoIntegracoesClick(data) {
    var tipoIntegracoesGrid = _gridTipoIntegracoes.BuscarRegistros();

    for (var i = 0; i < tipoIntegracoesGrid.length; i++) {
        if (data.Codigo == tipoIntegracoesGrid[i].Codigo) {
            tipoIntegracoesGrid.splice(i, 1);
            break;
        }
    }

    _gridTipoIntegracoes.CarregarGrid(tipoIntegracoesGrid);
}

function LimparCamposTipoIntegracoes() {
    LimparCampos(_grupoPessoasTipoIntegracoes);
    _gridTipoIntegracoes.CarregarGrid(new Array());
}