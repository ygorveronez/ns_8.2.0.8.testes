//*******MAPEAMENTO KNOUCKOUT*******

var _gridBorderoTitulo, _pesquisaBorderoTitulo, _gridSelecaoTitulosBordero;

var PesquisaBorderoTitulo = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.AdicionarTitulo = PropertyEntity({ type: types.event, text: "Adicionar Título", idBtnSearch: guid(), visible: ko.observable(true), icon: "fal fa-plus" });

    this.Codigo = PropertyEntity({ getType: typesKnockout.int, def: 0, val: ko.observable(0) });
    this.Carga = PropertyEntity({ text: "Nº Carga:", maxlength: 15 });
    this.CTe = PropertyEntity({ text: "Nº CT-e:", val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int });
    this.Titulo = PropertyEntity({ text: "Nº Título:", val: ko.observable(""), def: "", maxlength: 15, getType: typesKnockout.int });


    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridBorderoTitulo.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true), icon: "fa fa-refresh"
    });


}

function LoadBorderoTitulo() {

    _pesquisaBorderoTitulo = new PesquisaBorderoTitulo();
    KoBindings(_pesquisaBorderoTitulo, "knockoutTitulos");


    var header = [
        { data: "Codigo", visible: false },
        { data: "Documento", title: "Documento", width: "80%" }
    ];

    _gridSelecaoTitulosBordero = new BasicDataTable(_pesquisaBorderoTitulo.Grid.id, header, null, { column: 0, dir: orderDir.asc });

    new BuscarTitulosParaBordero(_pesquisaBorderoTitulo.AdicionarTitulo, AdicionarTitulosBordero, _gridSelecaoTitulosBordero, _bordero);
    _pesquisaBorderoTitulo.AdicionarTitulo.basicTable = _gridSelecaoTitulosBordero;
    _gridSelecaoTitulosBordero.CarregarGrid([]);

    BuscarBorderoTitulos();

}

function AdicionarTitulosBordero(titulos) {
    var codigosTitulos = new Array();

    for (var i = 0; i < titulos.length; i++)
        codigosTitulos.push(titulos[i].Codigo);

    executarReST("BorderoTitulo/AdicionarTitulos", { Bordero: _bordero.Codigo.val(), Titulos: JSON.stringify(codigosTitulos) }, function (r) {
        if (r.Success) {
            if (r.Data) {
                PreecherCamposEdicaoBordero(r.Data);
                _gridBorderoTitulo.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function BuscarBorderoTitulos() {
    var detalhes = { descricao: "Detalhes", id: guid(), evento: "onclick", metodo: DetalhesBorderoTitulo, tamanho: "15", icone: "" };
    var remover = { descricao: "Remover", id: guid(), evento: "onclick", metodo: RemoverBorderoTitulo, tamanho: "15", icone: "", visibilidade: VisibilidadeOpcaoRemoverBorderoTitulo };
    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [detalhes, remover], tamanho: 7, descricao: "Opções" };

    _gridBorderoTitulo = new GridView(_pesquisaBorderoTitulo.Pesquisar.idGrid, "BorderoTitulo/Pesquisa", _pesquisaBorderoTitulo, menuOpcoes, { column: 1, dir: orderDir.asc }, null);
}

function DetalhesBorderoTitulo(borderoTituloGrid) {
    AbrirTelaBorderoTituloDocumento(borderoTituloGrid);
}

function RemoverBorderoTitulo(borderoTituloGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente remover este título do borderô?", function () {
        executarReST("BorderoTitulo/RemoverTitulo", { Codigo: borderoTituloGrid.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Título removido com sucesso.");
                    PreecherCamposEdicaoBordero(r.Data);
                    _gridBorderoTitulo.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
            }
        });
    });
}

function ExibirTitulosBordero() {
    _pesquisaBorderoTitulo.Codigo.val(_bordero.Codigo.val());
    _gridBorderoTitulo.CarregarGrid();

    _pesquisaBorderoTitulo.AdicionarTitulo.visible((_bordero.Situacao.val() == EnumSituacaoBordero.EmAndamento));
}

function VisibilidadeOpcaoRemoverBorderoTitulo() {
    return (_bordero.Situacao.val() == EnumSituacaoBordero.EmAndamento);
}