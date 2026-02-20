var _gridDocumentoCargaCTeAgrupado = null, _pesquisaDocumentoCargaCTeAgrupado = null;

//*******MAPEAMENTO KNOUCKOUT*******
var PesquisaDocumentoCargaCTeAgrupado = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(""), def: "", options: EnumSituacaoCargaCTeAgrupado.ObterOpcoesPesquisa(), text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentoCargaCTeAgrupado.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
};



//*******EVENTOS*******

function LoadDocumentosCargaCTeAgrupado() {
    _pesquisaDocumentoCargaCTeAgrupado = new PesquisaDocumentoCargaCTeAgrupado();
    KoBindings(_pesquisaDocumentoCargaCTeAgrupado, "knockoutDocumentosEmitidos", false, _pesquisaDocumentoCargaCTeAgrupado.Pesquisar.id);
}

//*******MÉTODOS*******

function BuscarDocumentosCargaCTeAgrupado() {
    _pesquisaDocumentoCargaCTeAgrupado.Codigo.val(_cargaCTeAgrupado.Codigo.val());

    var baixarDACTE = { descricao: "Baixar DACTE", id: guid(), metodo: BaixarDACTEClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var baixarXML = { descricao: "Baixar XML", id: guid(), metodo: BaixarXMLCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };
    var emitir = { descricao: "Emitir", id: guid(), metodo: EmitirCTeClick, icone: "", visibilidade: VisibilidadeRejeicao };
    var editar = { descricao: "Editar", id: guid(), metodo: EditarCTeClick, icone: "", visibilidade: VisibilidadeRejeicao };
    var visualizar = { descricao: "Detalhes", id: guid(), metodo: DetalhesCTeClick, icone: "", visibilidade: VisibilidadeOpcaoDownload };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        opcoes: [baixarDACTE, baixarXML, emitir, visualizar, editar],
        tamanho: "6"
    };

    _gridDocumentoCargaCTeAgrupado = new GridView(_pesquisaDocumentoCargaCTeAgrupado.Pesquisar.idGrid, "CargaCTeAgrupadoDocumento/Pesquisa", _pesquisaDocumentoCargaCTeAgrupado, menuOpcoes);
    _gridDocumentoCargaCTeAgrupado.CarregarGrid();
}

function BaixarDACTEClick(e) {
    executarDownload("CargaCTe/DownloadDacte", { CodigoCTe: e.CodigoCTe, CodigoEmpresa: e.CodigoEmpresa });
}

function BaixarXMLCTeClick(e) {
    executarDownload("CargaCTe/DownloadXML", { CodigoCTe: e.CodigoCTe, CodigoEmpresa: e.CodigoEmpresa });
}

function EmitirCTeClick(e) {
    if (e.Situacao !== EnumStatusCTe.REJEICAO && e.Situacao !== EnumStatusCTe.FSDA) {
        exibirMensagem(tipoMensagem.atencao, "Situação não permite emissão", "A atual situação do CT-e (" + e.DescricaoSituacao + ") não permite que ele seja emitido.");
        return;
    }

    executarReST("CargaCTe/EmitirNovamente", { CodigoCTe: e.CodigoCTe, CodigoEmpresa: e.CodigoEmpresa }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _gridDocumentoCargaCTeAgrupado.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function DetalhesCTeClick(e) {
    var instancia = new EmissaoCTe(e.CodigoCTe, function () {
        instancia.CRUDCTe.Emitir.visible(false);
        instancia.CRUDCTe.Salvar.visible(false);
        instancia.CRUDCTe.Salvar.eventClick = function () { }
    }, [EnumPermissoesEdicaoCTe.Nenhuma]);
}

function EditarCTeClick(e) {

    var permissoesEdicaoCTe = [EnumPermissoesEdicaoCTe.AlterarIEParticipanete, EnumPermissoesEdicaoCTe.AlterarCFOP];

    var codigo = parseInt(e.CodigoCTe);

    if (e.Situacao === EnumStatusCTe.REJEICAO) {

        iniciarControleManualRequisicao();

        var instancia = new EmissaoCTe(codigo, function () {

            finalizarControleManualRequisicao();

            instancia.CRUDCTe.Salvar.visible(false);
            instancia.CRUDCTe.Emitir.visible(true);

            instancia.CRUDCTe.Emitir.eventClick = function () {
                var objetoCTe = ObterObjetoCTe(instancia);
                EmitirCTeRejeitado(objetoCTe, e.Codigo, instancia);
            };

        }, permissoesEdicaoCTe);

    }
}

function EmitirCTeRejeitado(cte, codigoCargaCTeAgrupadoCTe, instancia) {
    var dados = { CTe: cte, Codigo: codigoCargaCTeAgrupadoCTe };
    executarReST("CargaCTeAgrupadoDocumento/EmitirCTeRejeitado", dados, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                instancia.FecharModal();
                _gridDocumentoCargaCTeAgrupado.CarregarGrid();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "CT-e emitido com sucesso.");
            } else {
                exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg, 60000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}

function VisibilidadeOpcaoDownload(data) {
    if ((data.Situacao === EnumStatusCTe.AUTORIZADO || data.Situacao === EnumStatusCTe.ANULADO || data.Situacao === EnumStatusCTe.CANCELADO || data.Situacao === EnumStatusCTe.FSDA) && data.TipoDocumentoEmissao === EnumTipoDocumentoEmissao.CTe) {
        return true;
    } else {
        return false;
    }
}

function VisibilidadeRejeicao(data) {
    if (data.Situacao === EnumStatusCTe.REJEICAO || data.Situacao === EnumStatusCTe.FSDA) {
        return true;
    } else {
        return false;
    }
}