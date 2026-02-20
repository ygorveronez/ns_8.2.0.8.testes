
var _gridNotasFiscaisDTNatura, _dtNatura, _notaFiscalDTNatura;

//*******MAPEAMENTO KNOUCKOUT*******

var DTNatura = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.NotasFiscais = PropertyEntity({ type: types.listEntity, list: new Array(), required: false, codEntity: ko.observable(0), idGrid: guid() });

    this.Salvar = PropertyEntity({ eventClick: SalvarAlteracaoDTNaturaClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarAlteracaoDTNaturaClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
}

var NotaFiscalDTNatura = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "*Número:", val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), required: true });
    this.Serie = PropertyEntity({ text: "*Série:", val: ko.observable(0), def: 0, getType: typesKnockout.int, visible: ko.observable(true), configInt: { precision: 0, allowZero: true }, required: true });
    this.DataEmissao = PropertyEntity({ text: "*Data de Emissão:", val: ko.observable(""), def: "", getType: typesKnockout.date, visible: ko.observable(true), required: true });
    this.Chave = PropertyEntity({ text: "*Chave:", val: ko.observable(""), def: "", getType: typesKnockout.string, visible: ko.observable(true), maxlength: 44, required: true });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarNotaFiscalDTNaturaClick, type: types.event, text: "Salvar", visible: ko.observable(true) });
}

//*******EVENTOS*******

function LoadAlteracaoDTNatura() {

    _dtNatura = new DTNatura();
    KoBindings(_dtNatura, "divCRUDAlteracaoDTNatura");

    _notaFiscalDTNatura = new NotaFiscalDTNatura();
    KoBindings(_notaFiscalDTNatura, "divAlteracaoNotasFiscaisDTNatura");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: "Excluir", id: guid(), metodo: ExcluirNotaFiscalDTNaturaClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "25%" },
        { data: "Serie", title: "Série", width: "25%" },
        { data: "Chave", title: "Chave", width: "40%" }
    ];

    _gridNotasFiscaisDTNatura = new BasicDataTable(_notaFiscalDTNatura.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    RecarregarGridNotasFiscaisDT();
}

function AbrirTelaAlteracaoDTNaturaClick(dtNatura) {
    LimparCampos(_dtNatura);
    LimparCamposNotaFiscalDTNatura();
    Global.abrirModal("divAlterarDTNatura");
    _dtNatura.Codigo.val(dtNatura.Codigo);

    BuscarPorCodigo(_dtNatura, "DocumentoTransporteNatura/BuscarPorCodigo", function (r) {
        if (r.Success) {
            if (r.Data) {
                RecarregarGridNotasFiscaisDT();
                Global.abrirModal('divAlterarDTNatura');
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function RecarregarGridNotasFiscaisDT() {

    var data = new Array();

    $.each(_dtNatura.NotasFiscais.list, function (i, notaFiscal) {
        var notaFiscalGrid = new Object();

        notaFiscalGrid.Codigo = notaFiscal.Codigo.val;
        notaFiscalGrid.Numero = notaFiscal.Numero.val;
        notaFiscalGrid.Serie = notaFiscal.Serie.val;
        notaFiscalGrid.Chave = notaFiscal.Chave.val;

        data.push(notaFiscalGrid);
    });

    _gridNotasFiscaisDTNatura.CarregarGrid(data);
}


function ExcluirNotaFiscalDTNaturaClick(data) {
    for (var i = 0; i < _dtNatura.NotasFiscais.list.length; i++) {
        if (data.Codigo == _dtNatura.NotasFiscais.list[i].Codigo.val) {
            _dtNatura.NotasFiscais.list.splice(i, 1);
            break;
        }
    }

    RecarregarGridNotasFiscaisDT();
}

function AdicionarNotaFiscalDTNaturaClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_notaFiscalDTNatura);

    if (valido) {

        for (var i = 0; i < _dtNatura.NotasFiscais.list.length; i++) {
            if (_dtNatura.NotasFiscais.list[i].Numero.val == _notaFiscalDTNatura.Numero.val() && _dtNatura.NotasFiscais.list[i].Serie.val == _notaFiscalDTNatura.Serie.val()) {
                exibirMensagem(tipoMensagem.aviso, "Nota Fiscal já existe", "A nota fiscal informada já existe neste DT.");
                return;
            }
        }

        _notaFiscalDTNatura.Codigo.val(guid());

        _dtNatura.NotasFiscais.list.push(SalvarListEntity(_notaFiscalDTNatura));

        RecarregarGridNotasFiscaisDT();

        LimparCamposNotaFiscalDTNatura();
    } else {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
    }
}

function LimparCamposNotaFiscalDTNatura() {
    LimparCampos(_notaFiscalDTNatura);
}

function SalvarAlteracaoDTNaturaClick(e) {
    Salvar(_dtNatura, "DocumentoTransporteNatura/Salvar", function (r) {
        if (r.Success) {
            if (r.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso!", "Dados salvos com sucesso!");
                CancelarAlteracaoDTNaturaClick();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        }
    });
}

function CancelarAlteracaoDTNaturaClick() {
    Global.fecharModal("divAlterarDTNatura");
    LimparCampos(_dtNatura);
    LimparCamposNotaFiscalDTNatura();
}