var _painelMotoristas;
var _gridMotoristas;

var PainelMotoristas = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Adicionar = PropertyEntity({ text: "Adicionar", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Motoristas = PropertyEntity({ idGrid: guid() });
}

function CarregarGridMotoristas() {
    var excluir = { descricao: "Excluir", evento: "onclick", metodo: excluirMotoristaClick, tamanho: "20" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [excluir] };

    var configuracaoExportacao = {
        url: "ManutencaoCentroResultado/ExportarMotoristas",
        titulo: "Motoristas"
    };

    _gridMotoristas = new GridView(_painelMotoristas.Motoristas.idGrid, "ManutencaoCentroResultado/BuscarMotoristas", _manutencaoCentroResultado, menuOpcoes, null, 25, null, null, null, null, null, null, configuracaoExportacao);
    _gridMotoristas.CarregarGrid();
}

function retornoAdicionarMotorista(item) {
    executarReST("ManutencaoCentroResultado/AdicionarMotorista", { Codigo: item.Codigo, CodigoCentroResultado: _manutencaoCentroResultado.CentroResultado.codEntity() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridMotoristas.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function excluirMotoristaClick(e) {
    _painelMotoristas.Codigo.val(e.Codigo);

    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_painelMotoristas, "ManutencaoCentroResultado/ExcluirMotoristaPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotoristas.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}