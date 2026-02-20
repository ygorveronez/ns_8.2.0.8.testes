
//#region Variaveis
var _modalResponsaveisAtendimentoNivel;
var _gridResponsaveisAtendimento;
//#endregion

//#region
var ModalResponsanveisAtendimento = function () {
    this.Responsaveis = PropertyEntity({ list: ko.observable(new Array()) });
    this.Grid = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, idGrid: guid(), idTab: guid() });
    this.TituloModalContatosResponsaveisPorNivel = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ContatosResponsaveisPorNivel) });
};
//#endregion

//#region
function LoadModalResponsaveisAtendimento() {
    _modalResponsaveisAtendimentoNivel = new ModalResponsanveisAtendimento();
    KoBindings(_modalResponsaveisAtendimentoNivel, "knockoutModalResponsaveisAtendimentoNivel");

    LoadGridResponsaveisAtendimento();

    RecarregarGridResponsaveisAtendimentpo();
}

function LoadGridResponsaveisAtendimento() {
    var header = [
        { data: "NomeCompleto", title: Localization.Resources.Chamado.ChamadoOcorrencia.Nome , width: "20%" },
        { data: "Cargo", title: Localization.Resources.Chamado.ChamadoOcorrencia.Cargo , width: "15%" },
        { data: "Nivel", title: Localization.Resources.Chamado.ChamadoOcorrencia.Nivel , width: "20%" },
        { data: "Email", title: Localization.Resources.Chamado.ChamadoOcorrencia.Email , width: "20%" },
        { data: "Celular", title: Localization.Resources.Chamado.ChamadoOcorrencia.Celular, width: "20%" }
    ];

    _gridResponsaveisAtendimento = new BasicDataTable(_modalResponsaveisAtendimentoNivel.Grid.idGrid, header, null);
}
//#endregion

//#region Fuções auxiliares
function RecarregarGridResponsaveisAtendimentpo() {
    const data = new Array();

    $.each(_gridResponsaveisAtendimento.BuscarRegistros, (_, responsavel) => {
        data.push(responsavel);
    });
    _gridResponsaveisAtendimento.CarregarGrid(data);
}

function PreecherResponsaveisAtendimentoNivel(listaReponsaveis) {
    if (listaReponsaveis != null && listaReponsaveis.length == 0)
        return;

    _gridResponsaveisAtendimento.CarregarGrid(listaReponsaveis);
}

function LimparModalResponsaveis() {
    LimparCampos(_modalResponsaveisAtendimentoNivel);
}
//#endregion

