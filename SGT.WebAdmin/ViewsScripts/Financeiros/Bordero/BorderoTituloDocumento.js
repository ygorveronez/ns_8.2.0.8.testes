var _borderoTituloDocumento, _gridBorderoTituloDocumento;

var BorderoTituloDocumento = function () {
    this.Grid = PropertyEntity({ type: types.local, idGrid: guid() });
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.Pesquisar = PropertyEntity({ eventClick: function () { _gridBorderoTituloDocumento.CarregarGrid(); }, type: types.event, text: "Recarregar", icon: "fa fa-refresh", visible: ko.observable(true) });
    this.Fechar = PropertyEntity({ eventClick: FecharTelaBorderoTituloDocumento, type: types.event, text: "Fechar", icon: "fa fa-window-close", visible: ko.observable(true) });
}

////*******EVENTOS*******

function LoadBorderoTituloDocumento() {

    _borderoTituloDocumento = new BorderoTituloDocumento();
    KoBindings(_borderoTituloDocumento, "knockoutBorderoTituloDocumento");

    CarregarGridBorderoTituloDocumento();

}

////*******METODOS*******

function CarregarGridBorderoTituloDocumento() {

    var editarColuna = {
        permite: true,
        callback: EditarValorACobrarBorderoTituloDocumento,
        atualizarRow: false
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [
            { descricao: "Acréscimo/Desconto", id: guid(), evento: "onclick", metodo: AbrirTelaAcrescimoDescontoBorderoTituloDocumento, tamanho: "15", icone: "" },
            { descricao: "Baixar DACTE", id: guid(), metodo: DownloadDACTEBorderoTituloDocumento, visibilidade: VisibilidadeDownloadDACTE },
            { descricao: "Baixar XML", id: guid(), metodo: DownloadXMLBorderoTituloDocumento, visibilidade: VisibilidadeDownloadXMLCTe },
            { descricao: "Auditar", id: guid(), metodo: OpcaoAuditoria("BorderoTituloDocumento"), visibilidade: VisibilidadeOpcaoAuditoria }
        ],
        descricao: "Opções"
    };

    _gridBorderoTituloDocumento = new GridView(_borderoTituloDocumento.Grid.idGrid, "BorderoTituloDocumento/Pesquisa", _borderoTituloDocumento, menuOpcoes, { column: 0, dir: orderDir.asc }, 10, null, null, null, null, null, editarColuna, null);

}

function AbrirTelaBorderoTituloDocumento(dadosGrid) {
    _borderoTituloDocumento.Codigo.val(dadosGrid.Codigo);
    _gridBorderoTituloDocumento.CarregarGrid();
    Global.abrirModal('knockoutBorderoTituloDocumento');
}

function FecharTelaBorderoTituloDocumento() {
    Global.fecharModal('knockoutBorderoTituloDocumento');
}

function DownloadDACTEBorderoTituloDocumento(e, sender) {
    var data = { CodigoCTe: e.CodigoCTe };
    executarDownload("CargaCTe/DownloadDacte", data);
}

function DownloadXMLBorderoTituloDocumento(e, sender) {
    var data = { CodigoCTe: e.CodigoCTe };
    executarDownload("CargaCTe/DownloadXML", data);
}

function VisibilidadeDownloadXMLCTe(e) {
    if (e.TipoDocumento == EnumTipoDocumentoTitulo.CTe && (e.Modelo == "57" || e.Modelo == "39"))
        return true;
    else
        return false;
}

function VisibilidadeDownloadDACTE(e) {
    if (e.TipoDocumento == EnumTipoDocumentoTitulo.CTe)
        return true;
    else
        return false;
}

function EditarValorACobrarBorderoTituloDocumento(dataRow, row, head, callbackTabPress) {
    var data = { Codigo: dataRow.Codigo, ValorACobrar: dataRow.ValorACobrar };
    executarReST("BorderoTituloDocumento/AlterarDadosDocumento", data, function (r) {
        if (r.Success) {
            if (r.Data !== false) {
                PreecherCamposEdicaoBordero(r.Data.Bordero);
                CompararEAtualizarGridEditableDataRow(dataRow, r.Data.BorderoTituloDocumento)
                _gridBorderoTituloDocumento.AtualizarDataRow(row, dataRow, callbackTabPress);
                _gridBorderoTitulo.CarregarGrid();
            } else {
                _gridBorderoTituloDocumento.DesfazerAlteracaoDataRow(row);
                exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
            }
        } else {
            _gridBorderoTituloDocumento.DesfazerAlteracaoDataRow(row);
            exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
        }
    });
}