//*******MAPEAMENTO KNOUCKOUT*******

var _gridMDFeEncerramentoTMS;
var _pesquisaMDFeEncerramentoTMS;

var PesquisaMDFeEncerramentoTMS = function () {

    this.Placa = PropertyEntity({ text: "Placa:", getType: typesKnockout.string, maxlength: 7 });
    
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMDFeEncerramentoTMS.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
}

//*******EVENTOS*******

function LoadEncerramentoTMS() {

    _pesquisaMDFeEncerramentoTMS = new PesquisaMDFeEncerramentoTMS();
    KoBindings(_pesquisaMDFeEncerramentoTMS, "knockoutPesquisaMDFeEncerramentoTMS", _pesquisaMDFeEncerramentoTMS.Pesquisar.id);

    HeaderAuditoria("ManifestoEletronicoDeDocumentosFiscais");

    BuscarMDFesEncerramentoTMS();
}

//*******MÉTODOS*******

function EncerrarMDFe(mdfeGrid) {
    exibirConfirmacao("Atenção!", "Deseja realmente encerrar o MDF-e " + mdfeGrid.Numero + "?", function () {
        executarReST("EncerramentoTMS/Encerrar", { MDFe: mdfeGrid.Codigo }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "MDF-e enviado para encerramento com sucesso!");
                    _gridMDFeEncerramentoTMS.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Atenção!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function BaixarDAMDFE(mdfeGrid) {
    executarDownload("EncerramentoTMS/DownloadDAMDFE", { MDFe: mdfeGrid.Codigo });
}

function BaixarXMLAutorizacao(mdfeGrid) {
    executarDownload("EncerramentoTMS/DownloadXML", { MDFe: mdfeGrid.Codigo, Tipo: EnumTipoXMLMDFe.Autorizacao });
}

function BuscarMDFesEncerramentoTMS() {
    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [
            { descricao: "Encerrar", id: guid(), evento: "onclick", metodo: EncerrarMDFe, tamanho: "10", icone: "" },
            { descricao: "DAMDFE", id: guid(), evento: "onclick", metodo: BaixarDAMDFE, tamanho: "10", icone: "" },
            { descricao: "XML de Autorização", id: guid(), evento: "onclick", metodo: BaixarXMLAutorizacao, tamanho: "10", icone: "" },
            { descricao: "Auditoria", id: guid(), evento: "onclick", metodo: OpcaoAuditoria("ManifestoEletronicoDeDocumentosFiscais", "Codigo"), tamanho: "20", icone: "", visibilidade: VisibilidadeOpcaoAuditoria }
        ],
        tamanho: 10
    };

    _gridMDFeEncerramentoTMS = new GridView(_pesquisaMDFeEncerramentoTMS.Pesquisar.idGrid, "EncerramentoTMS/Pesquisa", _pesquisaMDFeEncerramentoTMS, menuOpcoes, { column: 2, dir: orderDir.desc }, 10);
    _gridMDFeEncerramentoTMS.CarregarGrid();
}