/// <reference path="ContatoGrupoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContatoGrupoPessoaDado;
var _contatoGrupoPessoaDado;

var ContatoGrupoPessoaDado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.Nome = PropertyEntity({ text: "*Nome: ", required: ko.observable(true), maxlength: 80 });
    this.Email = PropertyEntity({ text: "*Email: ", getType: typesKnockout.multiplesEmails, required: ko.observable(true), maxlength: 1000 });
    this.Telefone = PropertyEntity({ text: "Telefone: ", getType: typesKnockout.phone });
    this.Celular = PropertyEntity({ text: "Celular: ", getType: typesKnockout.phone });

    this.SalvarContato = PropertyEntity({ eventClick: SalvarContatoClick, type: types.event, text: "Salvar Contato" });
};

//*******EVENTOS*******

function loadContatoGrupoPessoaDado() {
    _contatoGrupoPessoaDado = new ContatoGrupoPessoaDado();
    KoBindings(_contatoGrupoPessoaDado, "knockoutCadastroContatoGrupoPessoaDado");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirContatoClick, tamanho: 10 }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Nome", title: "Nome", width: "40%" },
        { data: "Email", title: "E-mail", width: "20%" },
        { data: "Celular", title: "Celular", width: "15%" },
        { data: "Telefone", title: "Telefone", width: "15%" }
    ];
    _gridContatoGrupoPessoaDado = new BasicDataTable(_contatoGrupoPessoaDado.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridContato();
}

function RecarregarGridContato() {

    var data = new Array();
    $.each(_contatoGrupoPessoa.Contatos.list, function (i, contato) {
        var contatoGrid = new Object();

        contatoGrid.Codigo = contato.Codigo.val;
        contatoGrid.Nome = contato.Nome.val;
        contatoGrid.Email = contato.Email.val;
        contatoGrid.Telefone = contato.Telefone.val;
        contatoGrid.Celular = contato.Celular.val;

        data.push(contatoGrid);
    });

    _gridContatoGrupoPessoaDado.CarregarGrid(data);
}

function SalvarContatoClick() {

    var valido = ValidarCamposObrigatorios(_contatoGrupoPessoaDado);
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
        return;
    }

    valido = ValidarMultiplosEmails(_contatoGrupoPessoaDado.Email.val());
    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "E-mail Inválido", "Por favor, informe um ou mais e-mails válidos separados por ponto e vírgula ( ; ).");
        return;
    }

    _contatoGrupoPessoaDado.Codigo.val(guid());
    _contatoGrupoPessoa.Contatos.list.push(SalvarListEntity(_contatoGrupoPessoaDado));

    LimparContatoGrupoPessoaDados();
}

function ExcluirContatoClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente excluir o Contato?", function () {
        for (var i = 0; i < _contatoGrupoPessoa.Contatos.list.length; i++) {
            if (e.Codigo === _contatoGrupoPessoa.Contatos.list[i].Codigo.val) {
                _contatoGrupoPessoa.Contatos.list.splice(i, 1);
                break;
            }
        }

        LimparContatoGrupoPessoaDados();
    });
}

function LimparContatoGrupoPessoaDados() {
    LimparCampos(_contatoGrupoPessoaDado);
    RecarregarGridContato();
}