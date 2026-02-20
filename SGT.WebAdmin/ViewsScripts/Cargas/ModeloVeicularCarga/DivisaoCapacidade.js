//*******MAPEAMENTO KNOUCKOUT*******
var _gridDivisaoCapacidade;
var _divisaoCapacidade;

var DivisaoCapacidade = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), maxlength: 150, required: true });
    this.Quantidade = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.Quantidade.getRequiredFieldDescription(), getType: typesKnockout.decimal, required: true });
    this.Coluna = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.Coluna.getFieldDescription(), getType: typesKnockout.int, required: false, requiredLabel: ko.observable(false) });
    this.Piso = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.Piso.getFieldDescription(), getType: typesKnockout.int, required: false, requiredLabel: ko.observable(false) });
    this.UnidadeMedida = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, text: Localization.Resources.Cargas.ModeloVeicularCarga.UnidadeDeMedida.getRequiredFieldDescription(), idBtnSearch: guid(), required: true, issue: 88 });
    this.ValidarCapacidadeMaximaNoApp = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.ValidarCapacidadeMaximaNoApp, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarDivisaoCapacidadeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarDivisaoCapacidadeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirDivisaoCapacidadeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarDivisaoCapacidadeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
};

//*******EVENTOS*******
function LoadDivisaoCapacidade() {
    _divisaoCapacidade = new DivisaoCapacidade();
    KoBindings(_divisaoCapacidade, "knockoutDivisaoCapacidade");

    new BuscarUnidadesMedida(_divisaoCapacidade.UnidadeMedida);

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: EditarDivisaoCapacidadeClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "40%" },
        { data: "Coluna", title: Localization.Resources.Cargas.ModeloVeicularCarga.Coluna, width: "10%" },
        { data: "Piso", title: Localization.Resources.Cargas.ModeloVeicularCarga.Piso, width: "10%" },
        { data: "Quantidade", title: Localization.Resources.Cargas.ModeloVeicularCarga.Quantidade, width: "10%" }
    ];

    var ordenacao = [
        { column: 3, dir: orderDir.asc },
        { column: 2, dir: orderDir.asc },
        { column: 1, dir: orderDir.asc },
    ];
    _gridDivisaoCapacidade = new BasicDataTable(_divisaoCapacidade.Grid.id, header, menuOpcoes, ordenacao);

    RecarregarGridDivisaoCapacidade();
}

function EditarDivisaoCapacidadeClick(data) {
    for (var i = 0; i < _modeloVeicularCarga.DivisoesCapacidade.list.length; i++) {

        var divisaoCapacidade = _modeloVeicularCarga.DivisoesCapacidade.list[i];

        if (divisaoCapacidade.Codigo.val == data.Codigo) {

            _divisaoCapacidade.Codigo.val(divisaoCapacidade.Codigo.val);
            _divisaoCapacidade.Descricao.val(divisaoCapacidade.Descricao.val);
            _divisaoCapacidade.Quantidade.val(divisaoCapacidade.Quantidade.val);
            _divisaoCapacidade.Piso.val(divisaoCapacidade.Piso.val);
            _divisaoCapacidade.Coluna.val(divisaoCapacidade.Coluna.val);
            _divisaoCapacidade.UnidadeMedida.val(divisaoCapacidade.UnidadeMedida.val);
            _divisaoCapacidade.UnidadeMedida.codEntity(divisaoCapacidade.UnidadeMedida.codEntity);

            _divisaoCapacidade.Atualizar.visible(true);
            _divisaoCapacidade.Cancelar.visible(true);
            _divisaoCapacidade.Excluir.visible(true);
            _divisaoCapacidade.Adicionar.visible(false);

            return;
        }
    }
}

function CancelarDivisaoCapacidadeClick(e) {
    LimparCamposDivisaoCapacidade();
}

function ExcluirDivisaoCapacidadeClick() {
    for (var i = 0; i < _modeloVeicularCarga.DivisoesCapacidade.list.length; i++) {

        var divisaoCapacidade = _modeloVeicularCarga.DivisoesCapacidade.list[i];

        if (_divisaoCapacidade.Codigo.val() == divisaoCapacidade.Codigo.val)
            _modeloVeicularCarga.DivisoesCapacidade.list.splice(i, 1);

    }

    LimparCamposDivisaoCapacidade();
    RecarregarGridDivisaoCapacidade();
}

function AdicionarDivisaoCapacidadeClick(e, sender) {
    if (ValidarCamposObrigatorios(_divisaoCapacidade)) {

        _divisaoCapacidade.Codigo.val(guid());

        _modeloVeicularCarga.DivisoesCapacidade.list.push(SalvarListEntity(_divisaoCapacidade));

        RecarregarGridDivisaoCapacidade();

        $("#" + _divisaoCapacidade.Descricao.id).focus();

        LimparCamposDivisaoCapacidade();

    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}

function AtualizarDivisaoCapacidadeClick(e, sender) {
    if (ValidarCamposObrigatorios(_divisaoCapacidade)) {
        $.each(_modeloVeicularCarga.DivisoesCapacidade.list, function (i, divisaoCapacidade) {
            if (divisaoCapacidade.Codigo.val == _divisaoCapacidade.Codigo.val()) {
                AtualizarListEntity(_divisaoCapacidade, divisaoCapacidade);
                return false;
            }
        });
        RecarregarGridDivisaoCapacidade();
        LimparCamposDivisaoCapacidade();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}


//*******METODOS*******
function RecarregarGridDivisaoCapacidade() {
    var data = [];
    var pisoObrigatorio = false;
    var colunaObrigatoria = false;

    $.each(_modeloVeicularCarga.DivisoesCapacidade.list, function (i, divisaoCapacidade) {
        data.push({
            Descricao: divisaoCapacidade.Descricao.val,
            Quantidade: divisaoCapacidade.Quantidade.val,
            Piso: divisaoCapacidade.Piso.val,
            Coluna: divisaoCapacidade.Coluna.val,
            Codigo: divisaoCapacidade.Codigo.val
        });

        if (divisaoCapacidade.Piso.val != "" && divisaoCapacidade.Piso.val != null)
            pisoObrigatorio = true;

        if (divisaoCapacidade.Coluna.val != "" && divisaoCapacidade.Coluna.val != null)
            colunaObrigatoria = true;
    });

    _divisaoCapacidade.Piso.required = pisoObrigatorio;
    _divisaoCapacidade.Piso.requiredLabel(pisoObrigatorio);
    _divisaoCapacidade.Coluna.required = colunaObrigatoria;
    _divisaoCapacidade.Coluna.requiredLabel(colunaObrigatoria);

    // Passa pra esse objeto o campo
    _divisaoCapacidade.ValidarCapacidadeMaximaNoApp.val(_modeloVeicularCarga.ValidarCapacidadeMaximaNoApp.val())

    _gridDivisaoCapacidade.CarregarGrid(data);
}

function LimparCamposDivisaoCapacidade() {
    _divisaoCapacidade.Atualizar.visible(false);
    _divisaoCapacidade.Excluir.visible(false);
    _divisaoCapacidade.Cancelar.visible(false);
    _divisaoCapacidade.Adicionar.visible(true);

    LimparCampos(_divisaoCapacidade);
}