var _painelReboques;
var _gridReboques;

var PainelReboques = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Adicionar = PropertyEntity({ text: "Adicionar", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Reboques = PropertyEntity({ idGrid: guid() });
}

function CarregarGridReboques() {
    var excluir = { descricao: "Excluir", evento: "onclick", metodo: excluirReboqueClick, tamanho: "20" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var configuracaoExportacao = {
        url: "ManutencaoCentroResultado/ExportarReboques",
        titulo: "Reboques"
    };

    _gridReboques = new GridView(_painelReboques.Reboques.idGrid, "ManutencaoCentroResultado/BuscarReboques", _manutencaoCentroResultado, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridReboques.CarregarGrid();
}

function retornoAdicionarReboque(item) {
    executarReST("ManutencaoCentroResultado/AdicionarReboque", { Codigo: item.Codigo, CodigoCentroResultado: _manutencaoCentroResultado.CentroResultado.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridReboques.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function excluirReboqueClick(e) {
    _painelReboques.Codigo.val(e.Codigo);

    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_painelReboques, "ManutencaoCentroResultado/ExcluirReboquePorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridReboques.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}