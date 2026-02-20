var _importarCteEmbarcador = null;

var ImportarCteEmbarcador = function () {
    this.CTeEmbarcador = PropertyEntity({ type: types.local, idGrid: guid() });
    this.ArquivoCTe = PropertyEntity({ eventClick: ArquivoCTeClick, type: types.event, text: "Importar CTe", visible: ko.observable(true) });
    this.AdicionarCTe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Adicionar CT-e", visible: ko.observable(true), idBtnSearch: guid(), enable: ko.observable(true) });
    this.RemoverTodosCTes = PropertyEntity({ eventClick: RemoverTodosCTesClick, type: types.event, text: "Remover Todos", visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: confimarImportacaoCTeEmbarcador, type: types.event, text: "Confirmar Vínculo", visible: ko.observable(true), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarImportacaoCTeEmnarcadorClick, type: types.event, text: "Cancelar", visible: ko.observable(true) });
};

function LoadImportarCteEmbarcador() {
    _importarCteEmbarcador = new ImportarCteEmbarcador();
    KoBindings(_importarCteEmbarcador, "knockoutModalImportarCTeEmbarcador");

    menuOpcoes = {
        tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{
            descricao: "Excluir", id: guid(), metodo: function (data) {
                excluirImpoeracaoCTeEmbarcador(data);
            }
        }]
    };

    var header = [
        { data: "Codigo", title: "", visible: false },
        { data: "Numero", title: "Número" },
        { data: "DataEmissao", title: "Emissão" },
        { data: "NotasFiscais", title: "Nº NF" },
        { data: "GrupoPessoas", title: "Grupo de Pessoas" },
        { data: "Origem", title: "Origem" },
        { data: "Destino", title: "Destino" },
        { data: "ValorFrete", title: "Valor a Receber" },
    ];


    let gridCTesSemCargaSelecao = new BasicDataTable(_importarCteEmbarcador.CTeEmbarcador.idGrid, header, menuOpcoes, { column: 0, dir: orderDir.asc });

    BuscarCTesSemCarga(_importarCteEmbarcador.AdicionarCTe, null, gridCTesSemCargaSelecao);

    _importarCteEmbarcador.CTeEmbarcador.basicTable = gridCTesSemCargaSelecao;
    gridCTesSemCargaSelecao.CarregarGrid([]);
}

function excluirImpoeracaoCTeEmbarcador(cte){
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja excluir o CT-e?",
        function () {
            let CTesEmbarcador = _importarCteEmbarcador.CTeEmbarcador.basicTable.BuscarRegistros();

            for (var i = 0; i < CTesEmbarcador.length; i++) {
                if (cte.Codigo == CTesEmbarcador[i].Codigo) {
                    CTesEmbarcador.splice(i, 1);
                    break;
                }
            }
            _importarCteEmbarcador.CTeEmbarcador.basicTable.CarregarGrid(CTesEmbarcador);
        }
    );
}

function confimarImportacaoCTeEmbarcador() {
    let CTesEmbarcador = _importarCteEmbarcador.CTeEmbarcador.basicTable.BuscarRegistros();
    if (CTesEmbarcador.length > 0) {
        let dados = { 
            Carga: _carga.Codigo.val(),
            CTesEmbarcador: JSON.stringify(CTesEmbarcador)
        }
        executarReST("CargaCTeManual/VincularCTeEmbarcadorCarga", dados, function (e) {
            if (e.Success) {               

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "CT-e(s) vinculado(s) a carga com sucesso");
                _gridCargaCTe.CarregarGrid();
                Global.fecharModal("divModalImportarCTeEmbarcador");

            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, e.Msg);
            }
        });

    } else {
        exibirMensagem(tipoMensagem.aviso, "Atenção", "Nenhum CTe informado");
    }
}

function cancelarImportacaoCTeEmnarcadorClick() {
    limparCTeEmbarcador();
    Global.fecharModal("divModalImportarCTeEmbarcador");
}

function ImportarEmbarcadorClick(e, sender) {
    limparCTeEmbarcador();
    Global.abrirModal("divModalImportarCTeEmbarcador");
}

function RemoverTodosCTesClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, "Realmente deseja remover todos?",
        function () {
            limparCTeEmbarcador();
        }
    );
    
}

function limparCTeEmbarcador(){
    var data = new Array();
    _importarCteEmbarcador.CTeEmbarcador.basicTable.CarregarGrid(data);
}

function ArquivoCTeClick(e, sender) {
    let input = document.createElement("input");
    input.type = "file";
    input.setAttribute("accept", ".xml");
    input.onchange = function (event) {
        if (this.files.length > 0) {

            let formData = new FormData();
            formData.append("xml", this.files[0]);
            enviarArquivo("CargaCTeManual/ImportarCTeEmbarcador", {}, formData, function (resul) {
                if (resul.Success && resul.Data) {
                    let CTesEmbarcador = _importarCteEmbarcador.CTeEmbarcador.basicTable.BuscarRegistros();
                    let exists = CTesEmbarcador.find(el => el.Codigo === resul.Data.Codigo);
                    if (!exists) {
                        CTesEmbarcador.push(resul.Data);
                        _importarCteEmbarcador.CTeEmbarcador.basicTable.CarregarGrid(CTesEmbarcador);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", resul.Msg);
                }
            });
        }

    };
    input.click();
}