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
/// <reference path="../../Consultas/Banco.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="Pessoa.js" />
/// <reference path="../../Enumeradores/EnumTipoChavePix.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridDadoBancario;
var _dadoBancario;
var _gridPessoaContaBancaria
var _pessoaContaBancaria;
var DadoBancario = function () {
    this.ClientePortadorConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.PortadorDaConta.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.Banco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Pessoas.Pessoa.Banco.getFieldDescription(), issue: 49, idBtnSearch: guid(), required: false });
    this.Agencia = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Agencia.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 10 });
    this.Digito = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Digito.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 1 });
    this.NumeroConta = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.NumeroConta.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 12 });
    this.CnpjIpef = PropertyEntity({ text: "CNPJ IPEF", maxlength: 18, visible: ko.observable(true), getType: typesKnockout.cpfCnpj });
    this.TipoConta = PropertyEntity({ val: ko.observable(EnumTipoConta.Corrente), options: EnumTipoConta.obterOpcoes(), def: EnumTipoConta.Corrente, text: Localization.Resources.Pessoas.Pessoa.Tipo.getFieldDescription(), required: false });
    this.TipoChavePix = PropertyEntity({ val: ko.observable(""), options: EnumTipoChavePix.obterOpcoes(), def: "", text: Localization.Resources.Pessoas.Pessoa.TipoChavePix.getFieldDescription(), required: false });   
    this.ChavePix = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.ChavePix.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 36 });
    this.CodigoIntegracaoDadosBancarios = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.CodigoIntegracaoDadosBancarios.getFieldDescription()), required: false, visible: ko.observable(true), maxlength: 200 });


    this.UtilizarCadastroContaBancaria = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Pessoas.Pessoa.UtilizarCadastroContaBancaria, def: false, visible: ko.observable(true) });

    this.ContaBancaria = PropertyEntity({ type: types.event, text: "Adicionar Conta Bancária", idBtnSearch: guid(), enable: ko.observable(true) });
    this.Grid = PropertyEntity({ type: types.local });
    this.ContasBancarias = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

};

//*******EVENTOS*******

function loadDadoBancario() {
    $("#liTabDadosBancarios").show();
    _dadoBancario = new DadoBancario();
    KoBindings(_dadoBancario, "knockoutDadosBancarios");

    new BuscarBanco(_dadoBancario.Banco);
    new BuscarClientes(_dadoBancario.ClientePortadorConta, retornoClientePortadorConta);

    LoadPessoaContaBancaria();
}
function retornoClientePortadorConta(dado) {
    _dadoBancario.ClientePortadorConta.codEntity(dado.Codigo);
    _dadoBancario.ClientePortadorConta.val(dado.Nome + " - " + dado.CPF_CNPJ);
}
function validarCamposPessoaDadoBancario() {
    var tudoCerto = ValidarCamposObrigatorios(_dadoBancario);
    if (!tudoCerto) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        $("#liTabDadosBancarios a").tab("show");
    }

    return tudoCerto;
}

function limparCamposDadoBancario() {
    LimparCampos(_dadoBancario);
}

function LoadPessoaContaBancaria() {

    var menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [{
            descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: function (data) {
                ExcluirPessoaContaBancariaClick(data)
            }
        }]
    };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "15%" },
    ];

    _gridPessoaContaBancaria = new BasicDataTable(_dadoBancario.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    new BuscarContasBancarias(_dadoBancario.ContaBancaria, null, _gridPessoaContaBancaria);

    _dadoBancario.ContaBancaria.basicTable = _gridPessoaContaBancaria;
    _dadoBancario.ContaBancaria.basicTable.CarregarGrid(new Array());

}

function RecarregarGridPessoaContaBancaria(dados) {

    _dadoBancario.ContasBancarias.val(dados);
   // _dadoBancario.ContaBancaria.basicTable.CarregarGrid(dados);

    console.log(dados);


    var data = new Array();

    if (_dadoBancario.ContasBancarias.val() != "") {
        $.each(_dadoBancario.ContasBancarias.val(), function (i, conta) {
            var PessoaContaBancariaGrid = new Object();

            PessoaContaBancariaGrid.Codigo = conta.Codigo;
            PessoaContaBancariaGrid.Descricao = conta.Descricao;

            data.push(PessoaContaBancariaGrid);
        });
    }
    _gridPessoaContaBancaria.CarregarGrid(data);
    _dadoBancario.ContaBancaria.basicTable.CarregarGrid(data);
}

function ExcluirPessoaContaBancariaClick(data) {
    var PessoaContaBancariaGrid = _dadoBancario.ContaBancaria.basicTable.BuscarRegistros();
    for (var i = 0; i < PessoaContaBancariaGrid.length; i++) {
        if (data.Codigo == PessoaContaBancariaGrid[i].Codigo) {
            PessoaContaBancariaGrid.splice(i, 1);
            break;
        }
    }

    _dadoBancario.ContaBancaria.basicTable.CarregarGrid(PessoaContaBancariaGrid);
}

function LimparCamposPessoaContaBancaria() {
    _dadoBancario.ContaBancaria.basicTable.CarregarGrid(new Array());
}