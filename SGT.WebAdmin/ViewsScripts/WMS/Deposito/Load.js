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
var _pesquisaNivelDeposito;

var PesquisaNivelDeposito = function () {
    this.Deposito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Depósito:", idBtnSearch: guid() });
    this.Rua = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rua:", idBtnSearch: guid() });
    this.Bloco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Bloco:", idBtnSearch: guid() });
    this.Posicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Posição:", idBtnSearch: guid() });

    this.Abreviacao = PropertyEntity({ text: "Abreviação: " });

    this.Pesquisar = PropertyEntity({ eventClick: BuscarPosicaoClick, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

//*******EVENTOS*******
function loadInitDeposito() {
    _pesquisaNivelDeposito = new PesquisaNivelDeposito();
    KoBindings(_pesquisaNivelDeposito, "knockoutPesquisaNivelDeposito", false, _pesquisaNivelDeposito.Pesquisar.id);

    BuscarDeposito(_pesquisaNivelDeposito.Deposito);
    BuscarDepositoRua(_pesquisaNivelDeposito.Rua, null, _pesquisaNivelDeposito.Deposito);
    BuscarDepositoBloco(_pesquisaNivelDeposito.Bloco, null, _pesquisaNivelDeposito.Deposito, _pesquisaNivelDeposito.Rua);
    BuscarDepositoPosicao(_pesquisaNivelDeposito.Posicao, null, _pesquisaNivelDeposito.Deposito, _pesquisaNivelDeposito.Rua, _pesquisaNivelDeposito.Bloco);

    loadNavegacao();

    loadDeposito();
    loadRua();
    loadBloco();
    loadPosicao();
}
function BuscarPosicaoClick(){
    BuscarPosicaoEFocar();
}


//*******MÉTODOS*******
function BuscarPosicaoEFocar() {
    var dados = RetornarObjetoPesquisa(_pesquisaNivelDeposito);
    executarReST("Deposito/FiltroBusca", dados, function (arg) {
        if (arg.Success) {
            var data = arg.Data;
            if (data != null) {
                etapa = 0;
                if (data.Deposito != null) etapa++;
                _gridDeposito.CarregarGrid();
                SetDeposito(data.Deposito);

                if (data.Rua != null) etapa++;
                _gridRua.CarregarGrid();
                SetRua(data.Rua);

                if (data.Bloco != null) etapa++;
                _gridBloco.CarregarGrid();
                SetBloco(data.Bloco);

                if (data.Posicao != null) {
                    etapa++;
                    editarPosicao(data.Posicao);
                }
                _gridPosicao.CarregarGrid();
                
                SetarEtapa(etapa);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Nada Encontrado", "Nenhum registro foi encontrado com esse filtro.");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}