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
/// <reference path="../../Enumeradores/EnumTipoContatoAtendimento.js" />
/// <reference path="../../Enumeradores/EnumSituacaoProspeccao.js" />
/// <reference path="../../Enumeradores/EnumNivelSatisfacao.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _historico;
var _gridHistorico;
var _modalHistorico;
var Historico = function () {
    // Codigo da entidade
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataLancamento = PropertyEntity({ text: "Data Lançamento:", def: "" });
    this.Nome = PropertyEntity({ text: "Nome:", def: "" });
    this.Produto = PropertyEntity({ text: "Produto:", def: "" });
    this.CNPJ = PropertyEntity({ text: "CNPJ:", def: "" });
    this.Contato = PropertyEntity({ text: "Contato:", def: "" });
    this.Email = PropertyEntity({ text: "Email:", def: "" });
    this.Telefone = PropertyEntity({ text: "Telefone:", def: "" });
    this.Cidade = PropertyEntity({ text: "Cidade:", def: "" });
    this.TipoContato = PropertyEntity({ text: "Tipo do Contato:", def: "" });
    this.OrigemContato = PropertyEntity({ text: "Origem do Contato:", def: "" });
    this.Valor = PropertyEntity({ text: "Valor:", def: "" });
    this.Situacao = PropertyEntity({ text: "Situação:", def: "" });
    this.Satisfacao = PropertyEntity({ text: "Satisfação:", def: "" });
    this.DataRetorno = PropertyEntity({ text: "Data Retorno:", def: "" });
    this.Faturado = PropertyEntity({ text: "Faturado:", def: "" });
    this.Observacao = PropertyEntity({ text: "Observação:", def: "" });
}


//*******EVENTOS*******
function loadHistorico() {
    _historico = new Historico();
    KoBindings(_historico, "knockoutHistorico");
    
    GridHistorico();

    _prospeccao.Cliente.codEntity.subscribe(function (val) {
        if (val > 0) {
            _prospeccao.Historico.visible(true);
            _gridHistorico.CarregarGrid();
        } else {
            _prospeccao.Historico.visible(false);
        }
    });
    _modalHistorico = new bootstrap.Modal(document.getElementById("divModalHistorico"), { backdrop: true, keyboard: true });
}

function exibirDetalhesHistorico(data) {
    ExibirDetalhesHistorico(data.Codigo);
}


//*******MÉTODOS*******
function GridHistorico() {
    //-- Grid
    // Opcoes
    var detalhar = { descricao: "Detalhar", id: "clasEditar", evento: "onclick", metodo: exibirDetalhesHistorico, tamanho: "10", icone: "" };

    // Menu
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhar]
    };

    // Inicia Grid de busca
    _gridHistorico = new GridView(_prospeccao.Historico.idGrid, "Prospeccao/HistoricoCliente", _prospeccao, menuOpcoes, null);
    _gridHistorico.CarregarGrid();
}

function ExibirDetalhesHistorico(codigo) {
    _historico.Codigo.val(codigo);

    // Busca informacoes para edicao
    BuscarPorCodigo(_historico, "Prospeccao/DetalheHistorico", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _modalHistorico.show();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}