var _gridCanhotos = null;
var _listaCanhotosAtual = [];
var _cargaEntrega = null;
// Entidades knockout

var CargaEntrega = function () {
    this.ListaCanhotos = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });
    this.AplicarDemais = PropertyEntity({ eventClick: AplicarDemaisClick, type: types.event, text: ko.observable("Aplicar imagem aos sem imagem"), visible: ko.observable(true), enable: ko.observable(true) });

    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), required: false });

    this.onChooseFile = () => {
        onChooseImageEnviarDemais();
    }
}

// Load

function loadCanhoto() {
    _cargaEntrega = new CargaEntrega();
    KoBindings(_cargaEntrega, "knockoutListaCanhoto");

    _cargaEntrega.AplicarDemais.enable(podeEditarLote())

    var ordenacao = { column: 0, dir: orderDir.asc };

    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.list;
    menuOpcoes.opcoes = [
        { descricao: "Imagem Comprovante", id: guid(), evento: "onclick", metodo: onClickAlterarImagemCanhoto, tamanho: "7", icone: "" },
    ];

    var linhasPorPaginas = 5;

    var header = [
        { data: "Codigo", title: "Código" },
        { data: "Tipo", title: "Tipo" },
        { data: "Imagem", title: "Imagem" },
    ];

    _gridCanhotos = new BasicDataTable(_cargaEntrega.ListaCanhotos.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridCanhotos.CarregarGrid([]);
}

async function exibirModalListaCanhotos(cargaEntrega) {
    loadCanhoto();

    // Buscar canhotos dessa CargaEntrega e popular a variavel
    _listaCanhotosAtual = cargaEntrega.Canhotos;

    recarregarGridCanhotos();

    Global.abrirModal('divModalListaCanhoto');
    $("#divModalListaCanhoto").on("hidden.bs.modal", () => {
        recarregarGridCargaEntrega();
    });
}

function recarregarGridCanhotos() {
    let dados = _listaCanhotosAtual.map((canhoto) => {
        return {
            Codigo: canhoto.Codigo,
            Imagem: canhoto.Imagem || canhoto.ImagemFile ? "Sim" : "Não",
            Tipo: canhoto.Tipo
        }
    });
    _gridCanhotos.CarregarGrid(dados);
}

function onClickAlterarImagemCanhoto(canhoto) {
    let index = _gridCanhotos.BuscarRegistros().indexOf(canhoto);
    let canhotoDaEntidade = _listaCanhotosAtual[index];
    exibirModalImagemCanhoto(canhotoDaEntidade);
}

/*
 *  Aplica a imagem atual a todos ao canhotos da lista que não têm uma imagem
 */
async function AplicarDemaisClick() {
    // Reseta o input
    var imageInput = document.getElementById(_cargaEntrega.Arquivo.id);
    imageInput.value = null;

    $("#" + _cargaEntrega.Arquivo.id).trigger("click");
}

function onChooseImageEnviarDemais() {
    var imageInput = document.getElementById(_cargaEntrega.Arquivo.id);

    if (!imageInput.files[0]) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Carregue uma imagem primeiro");
        return;
    }

    exibirConfirmacao("Aplicar imagem", "Tem certeza que deseja aplicar essa imagem aos canhotos sem imagem?", async function () {
        for (let cargaEntrega of _dadosComprovanteEtapa.ListaCargaEntrega.val()) {
            for (let canhoto of cargaEntrega.Canhotos) {
                if (!canhoto.Imagem) {
                    canhoto.Imagem = "generico.png";
                    canhoto.ImagemFile = imageInput.files[0];
                    canhoto.ImagemBase64 = await toBase64(imageInput.files[0]);
                }
            }
        }

        recarregarGridCanhotos();
        recarregarGridCargaEntrega();
    });
}
