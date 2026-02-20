var _tipoOperacaoVendedores, _gridTipoOperacaoVendedores;
var TipoOperacaoVendedores = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: 'Funcionário:', idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.PercentualComissao = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), getType: typesKnockout.decimal, required: ko.observable(true), text: 'Percentual Comissão: ', configDecimal: { precision: 5 }, maxlength: 8, enable: ko.observable(true) });
    this.DataInicioVigencia = PropertyEntity({ text: 'Data Início Vigência:', getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFimVigencia = PropertyEntity({ text: 'Data Fim Vigência:', getType: typesKnockout.date, required: ko.observable(true), enable: ko.observable(true), visible: ko.observable(true) });


    this.GridTipoOperacaoVendedores = PropertyEntity({ type: types.map, idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.AdicionarVendedor = PropertyEntity({ eventClick: adicionarTipoOperacaoVendedorClick, type: types.event, text: 'Adicionar', visible: ko.observable(true), enable: !_FormularioSomenteLeitura });
    this.AtualizarVendedor = PropertyEntity({ eventClick: atualizarTipoOperacaoVendedorClick, type: types.event, text: 'Atualizar', visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.ExcluirVendedor = PropertyEntity({ eventClick: excluirTipoOperacaoVendedorClick, type: types.event, text: 'Excluir', visible: ko.observable(false), enable: !_FormularioSomenteLeitura });
    this.CancelarVendedor = PropertyEntity({ eventClick: LimparCamposTipoOperacaoVendedores, type: types.event, text: 'Cancelar', visible: ko.observable(false), enable: !_FormularioSomenteLeitura })
}

function isValidarTipoOperacaoVendedores() {
  

    var isValided = ValidarCamposObrigatorios(_tipoOperacaoVendedores);

    if (!isValided) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    return true;
}

function adicionarTipoOperacaoVendedorClick() {
    if (isValidarTipoOperacaoVendedores()) {
        var lista = _tipoOperacao.ListVendedores.val();
        lista.push({
            Codigo: guid(),
            Funcionario: { Codigo: _tipoOperacaoVendedores.Funcionario.codEntity(), Descricao: _tipoOperacaoVendedores.Funcionario.val() },
            PercentualComissao: _tipoOperacaoVendedores.PercentualComissao.val(),
            DataInicioVigencia: _tipoOperacaoVendedores.DataInicioVigencia.val(),
            DataFimVigencia: _tipoOperacaoVendedores.DataFimVigencia.val()

        });
        _tipoOperacao.ListVendedores.val(lista);
        RecarregarGridTipoOperacaoVendedores();
        limparTipoOperacaoVendedores();
    }
}

function atualizarTipoOperacaoVendedorClick() {
    if (isValidarTipoOperacaoVendedores()) {
        var lista = _tipoOperacao.ListVendedores.val();
        const vendedor = lista.find(el => el.Codigo == _tipoOperacaoVendedores.Codigo.val());        
        vendedor.Funcionario = { Codigo: _tipoOperacaoVendedores.Funcionario.codEntity(), Descricao: _tipoOperacaoVendedores.Funcionario.val() };
        vendedor.PercentualComissao = _tipoOperacaoVendedores.PercentualComissao.val();
        vendedor.DataInicioVigencia = _tipoOperacaoVendedores.DataInicioVigencia.val();
        vendedor.DataFimVigencia = _tipoOperacaoVendedores.DataFimVigencia.val();
        RecarregarGridTipoOperacaoVendedores();
        limparTipoOperacaoVendedores();
    }
}
function excluirTipoOperacaoVendedorClick() {
    excluirTipoOperacaoVendedores(_tipoOperacaoVendedores.Codigo.val());
}
function LimparCamposTipoOperacaoVendedores() {
    limparTipoOperacaoVendedores();
}

function excluirTipoOperacaoVendedoresGridClick(data) {
    excluirTipoOperacaoVendedores(data.Codigo);
}

function editarTipoOperacaoVendedoresGridClick(data) {
    var lista = _tipoOperacao.ListVendedores.val();
    const vendedor = lista.find(el => el.Codigo == data.Codigo);  
    _tipoOperacaoVendedores.Codigo.val(vendedor.Codigo);
    _tipoOperacaoVendedores.Funcionario.val(vendedor.Funcionario.Descricao);
    _tipoOperacaoVendedores.Funcionario.codEntity(vendedor.Funcionario.Codigo);
    _tipoOperacaoVendedores.PercentualComissao.val(vendedor.PercentualComissao);
    _tipoOperacaoVendedores.DataInicioVigencia.val(vendedor.DataInicioVigencia);
    _tipoOperacaoVendedores.DataFimVigencia.val(vendedor.DataFimVigencia);
    ControlarBotoes(true);
}

function excluirTipoOperacaoVendedores(Codigo) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir?",
        function () {
            var lista = _tipoOperacao.ListVendedores.val();
            for (var i = 0; i < lista.length; i++) {
                if (Codigo == lista[i].Codigo) {
                    lista.splice(i, 1);
                    break;
                }
            }
            _tipoOperacao.ListVendedores.val(lista);
            RecarregarGridTipoOperacaoVendedores();
            limparTipoOperacaoVendedores();
        }
    );
}

function LoadTipoOperacaoVendedores() {
    _tipoOperacaoVendedores = new TipoOperacaoVendedores();
    KoBindings(_tipoOperacaoVendedores, "knockoutTipoOperacaoVendedores");

    new BuscarFuncionario(_tipoOperacaoVendedores.Funcionario);

    ConfigurarGridTipoOperacaoVendedores();
}


function ConfigurarGridTipoOperacaoVendedores() {
    let excluir = { descricao: "Excluir", id: guid(), evento: "onclick", metodo: excluirTipoOperacaoVendedoresGridClick, visibilidade: true };
    let editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarTipoOperacaoVendedoresGridClick, visibilidade: true };
    menuOpcoes = new Object();

    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar, excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "Funcionario", title: "Funcionário", width: "25%" },
        { data: "PercentualComissao", title: "Percentual Comissão", width: "25%" },
        { data: "DataInicioVigencia", title: "Data Início Vigência", width: "25%" },
        { data: "DataFimVigencia", title: "Data Fim Vigência", width: "25%" },
    ];
    _gridTipoOperacaoVendedores = new BasicDataTable(_tipoOperacaoVendedores.GridTipoOperacaoVendedores.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    RecarregarGridTipoOperacaoVendedores();
}

function RecarregarGridTipoOperacaoVendedores() {
    var data = new Array();
    $.each(_tipoOperacao.ListVendedores.val(), function (i, vendedores) {
        data.push({
            Codigo: vendedores.Codigo,
            Funcionario: vendedores.Funcionario.Descricao,
            PercentualComissao: vendedores.PercentualComissao,
            DataInicioVigencia: vendedores.DataInicioVigencia,
            DataFimVigencia: vendedores.DataFimVigencia
        });
    });

    _gridTipoOperacaoVendedores.CarregarGrid(data);
}

function limparTipoOperacaoVendedores() {
   // LimparCampos(_tipoOperacao.ListVendedores);
    //LimparCampos(_tipoOperacao.Vendedores);
    LimparCampos(_tipoOperacaoVendedores);
    ControlarBotoes(false);
}

function ControlarBotoes(edit) {
    _tipoOperacaoVendedores.AdicionarVendedor.visible(!edit);
    _tipoOperacaoVendedores.AtualizarVendedor.visible(edit);
    _tipoOperacaoVendedores.ExcluirVendedor.visible(edit);
    _tipoOperacaoVendedores.CancelarVendedor.visible(edit);
}