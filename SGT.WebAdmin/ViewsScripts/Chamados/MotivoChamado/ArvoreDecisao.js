/// <reference path="../../Enumeradores/EnumStatusFinalizacaoAtendimento.js" /> 

var _arvoreDecisao;

function ArvoreDecisao() {
    this.Codigo = PropertyEntity({ val: ko.observable(0) });
    this.Perguntas = PropertyEntity({ val: ko.observable([]) });
    this.Key = PropertyEntity({ val: ko.observable("") });

    this.Pergunta = PropertyEntity({ val: ko.observable(""), text: Localization.Resources.Chamado.MotivoChamado.Pergunta.getFieldDescription() })
    this.PerguntaPai = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.PerguntaPai.getFieldDescription(), val: ko.observable([{ text: "", value: "" }]), def: { text: "", value: "" }, options: ko.observable([]), enable: ko.observable(false) });
    this.Resposta = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.Resposta.getFieldDescription(), val: ko.observable(EnumTipoPerguntas.SemResposta), def: EnumTipoPerguntas.SemResposta, options: EnumTipoPerguntas.obterOpcoes() });
    this.StatusFinalizador = PropertyEntity({ text: Localization.Resources.Chamado.MotivoChamado.StatusFinalizacao.getFieldDescription(), val: ko.observable(EnumStatusFinalizacaoAtendimento.SemStatus), def: EnumStatusFinalizacaoAtendimento.SemStatus, options: EnumStatusFinalizacaoAtendimento.obterOpcoes() });
    this.Adicionar = PropertyEntity({ eventClick: AdicionarPergunta, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarPergunta, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarEdicao, type: types.event, text: Localization.Resources.Chamado.MotivoChamado.Cancelar, visible: ko.observable(false) });
}

function LoadArvoreDecisao() {

    if (_CONFIGURACAO_TMS.HabilitarArvoreDecisaoEscalationList)
        $("#liTabArvoreDesicao").show();

    _arvoreDecisao = new ArvoreDecisao();
    KoBindings(_arvoreDecisao, "knockoutArvoreDecisao");
}

function AdicionarPergunta() {
    const key = guid();
    const objetoPergunta = new Object();
    objetoPergunta.key = key
    objetoPergunta.pergunta = _arvoreDecisao.Pergunta.val();
    objetoPergunta.resposta = _arvoreDecisao.Resposta.val();
    objetoPergunta.pai = _arvoreDecisao.PerguntaPai.val();
    objetoPergunta.statusFinalizador = _arvoreDecisao.StatusFinalizador.val();

    AdicionarNalistaPerguntas(objetoPergunta)
    AdicionarPerguntaPai(_arvoreDecisao.Pergunta.val(), key)

    LimparCadastro();
}

function AtualizarPergunta(e) {
    const listaPerguntas = _arvoreDecisao.Perguntas.val()
    for (let indicePergunta in listaPerguntas) {
        const perguntaAtual = listaPerguntas[indicePergunta];

        if (perguntaAtual.key != e.Key.val())
            continue;

        perguntaAtual.pergunta = e.Pergunta.val();
        perguntaAtual.resposta = e.Resposta.val();
        perguntaAtual.pai = e.PerguntaPai.val();
        perguntaAtual.statusFinalizador = e.StatusFinalizador.val();
        $("#" + perguntaAtual.key).text(perguntaAtual.pergunta);
        break;
    }
    _arvoreDecisao.Perguntas.val(listaPerguntas);
    LimparCadastro();
}
function CancelarEdicao() {
    LimparCadastro();
    VisibilidadeBotoesCRUD(false);
}
function DeletaPergunta(id) {

    const listaPerguntas = _arvoreDecisao.Perguntas.val();
    const listaPerguntasPais = _arvoreDecisao.PerguntaPai.options();
    const novalista = listaPerguntas.filter(pergunta => pergunta.key != id);
    const novalistaPais = listaPerguntasPais.filter(pergunta => pergunta.value != id);

    _arvoreDecisao.Perguntas.val(novalista);
    _arvoreDecisao.PerguntaPai.options(novalistaPais);

    exibirMensagem(tipoMensagem.ok, Localization.Resources.Chamado.MotivoChamado.Sucesso, Localization.Resources.Chamado.MotivoChamado.PerguntaRemovidaComSucesso);
}

function LimparCadastro() {
    LimparCampo(_arvoreDecisao.Pergunta);
    LimparCampo(_arvoreDecisao.Key);
    LimparCampo(_arvoreDecisao.Resposta);
    const options = _arvoreDecisao.PerguntaPai.options();
    if (options.length > 0)
        _arvoreDecisao.PerguntaPai.val(options[0].value);

    VisibilidadeBotoesCRUD(false);
}

function AdicionarPerguntaPai(text, value) {

    if (!_arvoreDecisao.PerguntaPai.enable())
        _arvoreDecisao.PerguntaPai.enable(true);

    const listaPerguntasPais = _arvoreDecisao.PerguntaPai.options();
    listaPerguntasPais.push({ text, value });
    _arvoreDecisao.PerguntaPai.options(listaPerguntasPais);
}

function AdicionarNalistaPerguntas(objetoPergunta) {
    const listaPerguntas = _arvoreDecisao.Perguntas.val();
    const [existeRepostaCadastradaNo] = listaPerguntas.filter(pergunta => pergunta.pai == objetoPergunta.pai && pergunta.resposta == objetoPergunta.resposta)

    if (existeRepostaCadastradaNo)
        return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Chamado.MotivoChamado.Alerta, Localization.Resources.Chamado.MotivoChamado.JaExisteRespostaParaPai.format(objetoPergunta.pai.substring(0, 4)))

    listaPerguntas.push(objetoPergunta);
    _arvoreDecisao.Perguntas.val(listaPerguntas);
}

function ObterPai(objetoPergunta) {
    if (!objetoPergunta.pai)
        return `ID:${objetoPergunta.key.substring(0, 4)} `;

    return `ID: ${objetoPergunta.key.substring(0, 4)} / Pai: ${objetoPergunta.pai.substring(0, 4)} / Resposta: ${EnumTipoPerguntas.obterResposta(objetoPergunta.resposta)} / ${EnumStatusFinalizacaoAtendimento.obterDescricao(objetoPergunta.statusFinalizador)} `;
}

function ObterArvore() {
    const list = _arvoreDecisao.Perguntas.val();
    const listaFormatada = new Array();
    for (var i = 0; i < list.length; i++) {
        let pergunta = list[i];
        const data = new Object();
        data.key = pergunta.key;
        data.pergunta = pergunta.pergunta;
        data.resposta = pergunta.resposta;
        data.pai = pergunta.pai || "";
        data.statusFinalizador = pergunta.statusFinalizador || EnumStatusFinalizacaoAtendimento.SemStatus;

        listaFormatada.push(data);
    }

    const arvore = JSON.stringify(listaFormatada);
    return arvore;
}

function LimparTodosCamposArvore() {
    LimparCampos(_arvoreDecisao);
    _arvoreDecisao.PerguntaPai.options([]);
    _arvoreDecisao.Perguntas.val([]);
}

function ListarArvore() {
    const listArvore = _motivoChamado.Arvore.val();

    for (var i = 0; i < listArvore.length; i++) {
        let pergunta = listArvore[i];

        const objetoPergunta = new Object();
        objetoPergunta.key = pergunta.key
        objetoPergunta.pergunta = pergunta.Pergunta;
        objetoPergunta.resposta = pergunta.Resposta;
        objetoPergunta.pai = pergunta.Pai;
        objetoPergunta.statusFinalizador = pergunta.StatusFinalizacaoAtendimento;

        AdicionarNalistaPerguntas(objetoPergunta)
        AdicionarPerguntaPai(objetoPergunta.pergunta, pergunta.key)
    }
}

function EditaPerguntaArvore(itemPergunta) {
    LimparCadastro();
    VisibilidadeBotoesCRUD(true);

    _arvoreDecisao.Pergunta.val(itemPergunta.pergunta);
    _arvoreDecisao.Resposta.val(itemPergunta.resposta)
    _arvoreDecisao.PerguntaPai.val(itemPergunta.pai);
    _arvoreDecisao.StatusFinalizador.val(itemPergunta.statusFinalizador);
    _arvoreDecisao.Key.val(itemPergunta.key);
}

function VisibilidadeBotoesCRUD(visible) {
    _arvoreDecisao.Atualizar.visible(visible);
    _arvoreDecisao.Cancelar.visible(visible);
    _arvoreDecisao.Adicionar.visible(!visible);
}