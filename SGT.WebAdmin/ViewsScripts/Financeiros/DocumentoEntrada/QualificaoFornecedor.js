/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/CFOP.js" />
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

//*******MAPEAMENTO KNOUCKOUT*******

var _qualificacao;

var _critarios = [
    'CriterioPrazoEntregaPontualidade',
    'CriterioCaracteristicaEspecificacoes',
    'CriterioQuantidadeRecebida',
    'CriterioIntegridadeFisica',
    'CriterioAtendimento'
];

var Qualificacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, estrelas: [5, 4, 3, 2, 1] });

    this.CriterioPrazoEntregaPontualidade = PropertyEntity({ name: "CriterioPrazoEntregaPontualidade", text: "1 - Prazo de entrega ou pontualidade", type: types.map, val: ko.observable(0), def: 0 });
    this.CriterioCaracteristicaEspecificacoes = PropertyEntity({ name: "CriterioCaracteristicaEspecificacoes", text: "2 - Característica/especificações", type: types.map, val: ko.observable(0), def: 0 });
    this.CriterioQuantidadeRecebida = PropertyEntity({ name: "CriterioQuantidadeRecebida", text: "3 - Quantidade recebida ou itens atendidos (quando serviço)", type: types.map, val: ko.observable(0), def: 0 });
    this.CriterioIntegridadeFisica = PropertyEntity({ name: "CriterioIntegridadeFisica", text: "4 - Integridade física ou quantidade (quando serviço)", type: types.map, val: ko.observable(0), def: 0 });
    this.CriterioAtendimento = PropertyEntity({ name: "CriterioAtendimento", text: "5 - Atendimento (facilidade de comunicação e rapidez de resposta)", type: types.map, val: ko.observable(0), def: 0 });

    this.Salvar = PropertyEntity({ eventClick: SalvarClick, type: types.event, text: "Salvar", enable: ko.observable(true) });
    this.LimparQualificacao = PropertyEntity({ eventClick: LimparQualificaoFornecedor, type: types.event, text: "Limpar Qualificação", enable: ko.observable(true) });
}

//*******EVENTOS*******

function LoadQualificacao() {
    _qualificacao = new Qualificacao();
    KoBindings(_qualificacao, "knockoutQualificacao");

}

function SalvarClick() {
    var data = {
        Codigo: _qualificacao.Codigo.val()
    };

    for (var i in _critarios) {
        data[_critarios[i]] = GetQualificao(_critarios[i]).val() || 0;
    }

    executarReST("QualificaoFornecedor/Qualificar", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Qualificado com sucesso.");
                OcultarQualificaoFornecedor();
                LimparCamposQualificao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

//*******METODOS*******
function ExibirQualificaoFornecedor(codigo) {
    if (codigo <= 0) return;

    LimparCamposQualificao()
    _qualificacao.Codigo.val(codigo);    
    Global.abrirModal('divModalQualificaoFornecedor');
}

function OcultarQualificaoFornecedor() {
    Global.fecharModal('divModalQualificaoFornecedor');
}

function LimparCamposQualificao() {
    LimparCampos(_qualificacao);
    LimparQualificaoFornecedor();
}

function LimparQualificaoFornecedor() {
    for (var i in _critarios) {
        GetQualificao(_critarios[i]).prop('checked', false);
    }
}

function GetQualificao(name) {
    return $("#knockoutQualificacao input[name='" + name + "']:checked");
}