
//*******MAPEAMENTO KNOUCKOUT*******
var _lacreAgendamento;
var _gridLacreAgendamento;

var LacreAgendamento = function () {
    this.ApenasGerarPedido = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool, type: types.map });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número:", maxlength: 60, required: true });

    this.Lacres = PropertyEntity({ type: types.event });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarLacreClick, type: types.event, text: "Adicionar Lacre", visible: ko.observable(true), enable: ko.observable(true) });
}

//*******EVENTOS*******
function LoadEtapaLacre() {
    _lacreAgendamento = new LacreAgendamento();
    KoBindings(_lacreAgendamento, "knockoutLacresAgendamento");
}

function LoadGridLacreAgendamento() {
    var opcaoInformacoes = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: ExcluirLacreClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [] };
    if ((_agendamentoColeta.Etapa.val() == EnumEtapaAgendamentoColeta.DadosTransporte || _agendamentoColeta.Etapa.val() == EnumEtapaAgendamentoColeta.NFe) && (_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe))
        menuOpcoes.opcoes.push(opcaoInformacoes);
    if (_gridLacreAgendamento) _gridLacreAgendamento.Destroy();
    _gridLacreAgendamento = new GridView(_lacreAgendamento.Lacres.id, "AgendamentoColeta/PesquisaLacre", { Codigo: _agendamentoColeta.CodigoAgendamento }, menuOpcoes);
    _gridLacreAgendamento.CarregarGrid();
}

function ExcluirLacreClick(lacre) {
    exibirConfirmacao("Excluir Lacre?", "Tem certeza que deseja excluir o lacre?", function () {
        executarReST("AgendamentoColeta/ExcluirLacre", {
            Agendamento: _agendamentoColeta.CodigoAgendamento.val(),
            Codigo: lacre.Codigo
        }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridLacreAgendamento.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function AdicionarLacreClick(e, sender) {
    var dados = {
        Codigo: _agendamentoColeta.CodigoAgendamento.val(),
        Numero: _lacreAgendamento.Numero.val()
    };
    executarReST("AgendamentoColeta/AdicionarLacre", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Lacre adicionado com sucesso.");
                _gridLacreAgendamento.CarregarGrid();
                _lacreAgendamento.Numero.val(_lacreAgendamento.Numero.def);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}



//*******MÉTODOS*******