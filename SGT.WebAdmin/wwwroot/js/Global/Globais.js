$(document).on('show.bs.modal', '.modal', function () {
    var zIndex = 1040 + (10 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});

$(document).on('hidden.bs.modal', '.modal', function () {
    $('.modal:visible').length && $(document.body).addClass('modal-open');
});

function InserirTag(id, text) {
    if (id != null && id.trim() != "") {
        var txtarea = document.getElementById(id);
        var scrollPos = txtarea.scrollTop;
        var strPos = 0;
        var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ? "ff" : (document.selection ? "ie" : false));
        if (br == "ie") {
            txtarea.focus();
            var range = document.selection.createRange();
            range.moveStart('character', -txtarea.value.length);
            strPos = range.text.length;
        } else if (br == "ff") {
            strPos = txtarea.selectionStart;
        }
        var front = (txtarea.value).substring(0, strPos);
        var back = (txtarea.value).substring(strPos, txtarea.value.length);
        txtarea.value = front + text + back;
        strPos = strPos + text.length;
        if (br == "ie") {
            txtarea.focus();
            var range = document.selection.createRange();
            range.moveStart('character', -txtarea.value.length);
            range.moveStart('character', strPos);
            range.moveEnd('character', 0);
            range.select();
        } else if (br == "ff") {
            txtarea.selectionStart = strPos;
            txtarea.selectionEnd = strPos;
            txtarea.focus();
        }
        txtarea.scrollTop = scrollPos;
    }
    $("#" + id).change();
}

var Global = {
    _resetarElementoNav(idContainerNavegacao, nomeClasseNavegacao, nomeElementoNavegacao) {
        let linksNavegacao = this._obterLinksNavegacaoDasPrimeirasAbasVisiveis(idContainerNavegacao, nomeClasseNavegacao, nomeElementoNavegacao);

        for (let i = 0; i < linksNavegacao.length; i++)
            bootstrap.Tab.getOrCreateInstance(linksNavegacao[0]).show();
    },
    _obterLinksNavegacaoDasPrimeirasAbasVisiveis(idContainerNavegacao, nomeClasseNavegacao, nomeElementoNavegacao) {
        let listaContainerNavegacao = [];
        let linksNavegacaoVisiveis = [];

        if (idContainerNavegacao) {
            let container = document.getElementById(idContainerNavegacao);
            listaContainerNavegacao = container.getElementsByClassName(nomeClasseNavegacao);
        }
        else
            listaContainerNavegacao = document.getElementsByClassName(nomeClasseNavegacao);

        for (let i = 0; i < listaContainerNavegacao.length; i++) {
            let itensNavegacao = listaContainerNavegacao[i].getElementsByTagName("li");

            for (let j = 0; j < itensNavegacao.length; j++) {
                let itemNavegacao = itensNavegacao[j];
                let $itemNavegacao = $(itemNavegacao);
                let itemOcultado = ($itemNavegacao.css('display') == 'none') || $itemNavegacao.hasClass("d-none");

                if (itemOcultado)
                    continue;

                let linksNavegacao = itemNavegacao.getElementsByTagName(nomeElementoNavegacao);

                if (linksNavegacao.length = 0)
                    continue;

                bootstrap.Tab.getOrCreateInstance(linksNavegacao[0]).show();
                break;
            }
        }

        return linksNavegacaoVisiveis;
    },
    _fallbackCopyTextToClipboard: function (text) {
        let textArea = document.createElement("textarea");
        textArea.value = text;

        // Avoid scrolling to bottom
        textArea.style.top = "0";
        textArea.style.left = "0";
        textArea.style.position = "fixed";

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        try {
            document.execCommand('copy');
        } catch (err) {
            console.error('Fallback: Oops, unable to copy', err);
        }

        document.body.removeChild(textArea);
    },
    criarData: function (data) {
        if (!data)
            return undefined;

        var dia = parseInt(data.substr(0, 2));
        var mes = parseInt(data.substr(3, 2)) - 1;
        var ano = parseInt(data.substr(6, 4));
        var horas = parseInt(data.substr(11, 2));
        var minutos = parseInt(data.substr(14, 2));
        var segundos = parseInt(data.substr(17, 2));

        horas = isNaN(horas) ? 0 : horas;
        minutos = isNaN(minutos) ? 0 : minutos;
        segundos = isNaN(segundos) ? 0 : segundos;

        return new Date(ano, mes, dia, horas, minutos, segundos);
    },
    DataAtual: function () {
        return moment().format("DD/MM/YYYY");
    },
    DataHoraAtual: function () {
        return moment().format("DD/MM/YYYY HH:mm");
    },
    DataHoraSegundoAtual: function () {
        return moment().format("DD/MM/YYYY HH:mm:ss");
    },
    Data: function (tipoOperacao, quantidade, objeto) {
        if (tipoOperacao == EnumTipoOperacaoDate.Add)
            return moment().add(quantidade, objeto).format("DD/MM/YYYY");
        else if (tipoOperacao == EnumTipoOperacaoDate.Subtract)
            return moment().subtract(quantidade, objeto).format("DD/MM/YYYY");
    },
    DataHora: function (tipoOperacao, quantidade, objeto) {
        if (tipoOperacao == EnumTipoOperacaoDate.Add)
            return moment().add(quantidade, objeto).format("DD/MM/YYYY HH:mm");
        else if (tipoOperacao == EnumTipoOperacaoDate.Subtract)
            return moment().subtract(quantidade, objeto).format("DD/MM/YYYY HH:mm");
    },
    PrimeiraDataDoMesAtual: function () {
        var date = new Date(), y = date.getFullYear(), m = date.getMonth();
        return moment(new Date(y, m, 1)).format("DD/MM/YYYY");
    },
    UltimaDataDoMesAtual: function () {
        var date = new Date(), y = date.getFullYear(), m = date.getMonth();
        return moment(new Date(y, m + 1, 0)).format("DD/MM/YYYY");
    },
    PrimeiraDataDoMesAnterior: function () {
        var date = new Date(), y = date.getFullYear(), m = date.getMonth();
        return moment(new Date(y, m - 1, 1)).format("DD/MM/YYYY");
    },
    ObterDiasEntreDatas: function (dataInicial, dataFinal) {
        var inicio = this.ParseDate(dataInicial);
        var fim = this.ParseDate(dataFinal);

        return Math.round((fim - inicio) / (1000 * 60 * 60 * 24));
    },
    ParseDate: function (data) {
        var expandido = data.split('/');
        return new Date(parseInt(expandido[2]), parseInt(expandido[1]) - 1, parseInt(expandido[0]));
    },
    ExibirAba: function (idTab) {
        let tab = bootstrap.Tab.getOrCreateInstance(document.querySelector('a[href="#' + idTab + '"]'));

        if (tab)
            tab.show();
    },
    ExibirStep: function (idStep) {
        let elemento = document.getElementById(idStep);

        if (elemento) {
            let tab = bootstrap.Tab.getOrCreateInstance(elemento);

            if (tab)
                tab.show();
        }
    },
    ResetarStep: function (idTabContent) {
        this._resetarElementoNav(idTabContent, "nav-steps", "button");
    },
    ResetarSteps: function () {
        this._resetarElementoNav(null, "nav-steps", "button");
    },
    ResetarAba: function (idTabContent) {
        this._resetarElementoNav(idTabContent, "nav-tabs", "a");
    },
    ResetarAbas: function () {
        this._resetarElementoNav(null, "nav-tabs", "a");
    },
    PossuiAbasVisiveis: function (idTabContent) {
        return (this._obterLinksNavegacaoDasPrimeirasAbasVisiveis(idTabContent, "nav-tabs", "a").length > 0);
    },
    ResetarMultiplasAbas: function () {
        this._resetarElementoNav(null, "nav-tabs", "a");
    },
    ObterClasseDinamica: function (cores) {
        var estiloCoresDinamicas = document.getElementById("dynamic-color-style");

        if (!estiloCoresDinamicas) {
            estiloCoresDinamicas = document.createElement("style");
            estiloCoresDinamicas.id = "dynamic-color-style";
            estiloCoresDinamicas.type = "text/css";

            document.getElementsByTagName('head')[0].appendChild(estiloCoresDinamicas);
        }

        var styleSheet = estiloCoresDinamicas.sheet;
        var totalRegrasAdicionadas = styleSheet.cssRules.length;
        var nomeClasse = 'dynamic-color-' + cores.Fundo.replace('#', '');

        if (cores.Borda)
            nomeClasse += '-' + cores.Borda.replace('#', '');

        var seletorClasse = '.' + nomeClasse;

        for (var i = 0; i < totalRegrasAdicionadas; i++) {
            var regra = styleSheet.cssRules[i];

            if (regra.selectorText == seletorClasse)
                return nomeClasse;
        }

        var regras = '';

        if (cores.Fundo)
            regras += 'background-color: ' + cores.Fundo + ' !important;';

        if (cores.Borda)
            regras += 'border: 1px solid ' + cores.Borda + ' !important;';

        if (cores.Fonte)
            regras += 'color: ' + cores.Fonte + ' !important;';

        var classe = seletorClasse + ' { ' + regras + ' }';

        styleSheet.insertRule(classe, totalRegrasAdicionadas);
        estiloCoresDinamicas.innerHTML += styleSheet.cssRules[totalRegrasAdicionadas].cssText;

        return nomeClasse;
    },
    ObterOpcoesPesquisaBooleano: function (descTrue, descFalse) {
        return [
            { text: Localization.Resources.Gerais.Geral.Todos, value: "" },
            { text: descTrue, value: true },
            { text: descFalse, value: false }
        ];
    },
    ObterOpcoesNaoSelecionadoBooleano: function (descTrue, descFalse) {
        return [
            { text: Localization.Resources.Gerais.Geral.NaoSelecionado, value: null },
            { text: descTrue, value: true },
            { text: descFalse, value: false }
        ];
    },
    ObterOpcoesBooleano: function (descTrue, descFalse) {
        return [
            { text: descTrue, value: true },
            { text: descFalse, value: false }
        ];
    },
    ObterOpcoesInteiro: function (inicio, fim, pesquisa) {
        var opcoes = [];

        if (pesquisa === true)
            opcoes.push({ text: "Todos", value: "" });

        while (inicio <= fim) {
            opcoes.push({ text: inicio, value: inicio });

            inicio++;
        }

        return opcoes;
    },
    ObterOpcoesSimNaoPesquisaPersonalizado: function (descSim, descNao) {
        return [
            { text: Localization.Resources.Gerais.Geral.Todos, value: 9 },
            { text: descSim, value: 1 },
            { text: descNao, value: 0 }
        ];
    },
    HumanFileSize: function (size) {
        if (size == 0)
            return "0.00b"
        var i = Math.floor(Math.log(size) / Math.log(1024));
        return Globalize.format((size / Math.pow(1024, i)) * 1, "n2") + " " + ["b", "kb", "mb", "gb", "tb"][i];
    },
    roundNumber: function (num, scale) {
        if (!("" + num).includes("e")) {
            return +(Math.round(num + "e+" + scale) + "e-" + scale);
        } else {
            var arr = ("" + num).split("e");
            var sig = ""
            if (+arr[1] + scale > 0) {
                sig = "+";
            }
            var i = +arr[0] + "e" + sig + (+arr[1] + scale);
            var j = Math.round(i);
            var k = +(j + "e-" + scale);
            return k;
        }
    },
    convertMinsToHrsMins: function (minutes) {
        var h = Math.floor(minutes / 60);
        var m = minutes % 60;
        h = h < 10 ? '0' + h : h;
        m = m < 10 ? '0' + m : m;
        return h + ':' + m;
    },
    contemModalAberto: function (idModal) {
        let idElemento = (idModal);

        if (string.IsNullOrWhiteSpace(idElemento) || idElemento.length == 0)
            return false;

        if (idElemento.substr(0, 1) == "#")
            idElemento = idElemento.substr(1, idElemento.length - 1);

        let element = document.getElementById(idElemento);

        if (!element)
            return false;

        let bsModal = bootstrap.Modal.getOrCreateInstance(element, { backdrop: 'static', keyboard: true });

        if (!bsModal)
            return false;

        return true;
    },
    abrirModal: function (idModal) {
        let idElemento = (idModal);

        if (string.IsNullOrWhiteSpace(idElemento) || idElemento.length == 0)
            throw "Elemento vazio não pode ser encontrado para interagir com o modal.";

        if (idElemento.substr(0, 1) == "#")
            idElemento = idElemento.substr(1, idElemento.length - 1);

        let element = document.getElementById(idElemento);

        if (!element)
            throw "Elemento " + idModal + " não encontrado.";

        let bsModal = bootstrap.Modal.getOrCreateInstance(element, { backdrop: 'static', keyboard: true, focus: true });

        if (!bsModal)
            throw "Não foi possível obter/criar o modal para o elemento " + idElemento + ".";

        element.addEventListener('shown.bs.modal', () => {
            $(element).focus();
        });

        bsModal.show();
    },
    fecharModal: function (idModal) {
        let idElemento = (idModal);

        if (string.IsNullOrWhiteSpace(idElemento) || idElemento.length == 0)
            throw "Elemento vazio não pode ser encontrado para interagir com o modal.";

        if (idElemento.substr(0, 1) == "#")
            idElemento = idElemento.substr(1, idElemento.length - 1);

        let element = document.getElementById(idElemento);

        if (!element)
            throw "Elemento " + idModal + " não encontrado.";

        let bsModal = bootstrap.Modal.getOrCreateInstance(element, { backdrop: 'static', keyboard: true });

        if (!bsModal)
            throw "Não foi possível obter/criar o modal para o elemento " + idElemento + ".";

        bsModal.hide();
    },
    setarFocoProximoCampo: function (idCampoAtual) {
        $("#" + idCampoAtual).focusNextInputField();
    },
    copyTextToClipboard: function (text, callback) {
        if (!navigator.clipboard) {
            this._fallbackCopyTextToClipboard(text);
            return;
        }
        navigator.clipboard.writeText(text).then(function () {
            if (callback)
                callback()
        }, function (err) {
            console.error('Async: Could not copy text: ', err);
        });
    }
};

var string = {
    IsNullOrWhiteSpace: function (str) {
        if (typeof str === 'undefined' || str == null)
            return true;

        return str.toString().replace(/\s/g, '').length < 1;
    },
    Left: function (str, n) {
        if (n <= 0)
            return "";
        else if (n > String(str).length)
            return str;
        else
            return String(str).substring(0, n);
    },
    Right: function (str, n) {
        if (n <= 0)
            return "";
        else if (n > String(str).length)
            return str;
        else {
            var iLen = String(str).length;
            return String(str).substring(iLen, iLen - n);
        }
    },
    OnlyNumbers: function (str) {
        if (str == null)
            return str;

        return str.replace(/\D+/g, '');
    },
    ParseFloat: function (str) {
        return parseFloat(String(str).replace(/\./g, '').replace(',', '.'))
    }
};

var EnumTipoOperacaoDate = {
    Add: 0,
    Subtract: 1
};

var EnumTipoOperacaoObjetoDate = {
    Milliseconds: 'milliseconds',
    Seconds: 'seconds',
    Minutes: 'minutes',
    Hours: 'hours',
    Days: 'days',
    Weeks: 'weeks',
    Months: 'months',
    Years: 'years'
};

function NavegadorIEInferiorVersao12() {
    var isIE = /*@cc_on!@*/false || !!document.documentMode;
    if (isIE && document.documentMode < 12)
        return true;
    else
        return false;
};

function IsMobile() {
    return $(window).width() <= 600;
};

function IsTouchDevice() {
    return window.matchMedia("(pointer: coarse)").matches;
}

var CODIGO_OCORRENCIA_VIA_TOKEN_ACESSO_AUTORIZACAO_OCORRENCIA = { val: ko.observable("") };

var CODIGO_CARGA_VIA_TOKEN_ACESSO_AUTORIZACAO_CARGA = { val: ko.observable("") };

var CODIGO_TABELA_FRETE_VIA_TOKEN_ACESSO_AUTORIZACAO_TABELA_FRETE = { val: ko.observable("") };

var CODIGO_CARREGAMENTO_VIA_TOKEN_ACESSO_AUTORIZACAO_CARREGAMENTO = { val: ko.observable("") };

var CODIGO_MDFE_AQUAVIARIO_PARA_CANCELAMENTO_TELA_CARGA = { val: ko.observable(""), codEntity: ko.observable(0) };

var CODIGO_CARGA_PARA_CANCELAMENTO_TELA_CARGA = 0;

var CODIGO_CARGA_PESQUISA_TELA_CARGA = 0;

var DADOS_CARGA_PEDIDOS_GESTAO_PEDIDOS = {
    Pedidos: [],
    CodigosAgrupadores: [],
    Filial: null,
    DataInicio: null,
    DataFim: null,
    SessaoRoteirizador: 0
};

var CODIGO_ORDEM_SERVICO_PARA_TELA_ORDEM_SERVICO = 0;



//Recebe uma cor em Hexadecimal (#58594f) e uma Opacidade.
//Retorna no formato RGBA: rgba(88,89,79,0.5).
function hexToRgbA(hex, op) {
    var c;
    if (/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
        c = hex.substring(1).split('');
        if (c.length == 3) {
            c = [c[0], c[0], c[1], c[1], c[2], c[2]];
        }
        c = '0x' + c.join('');
        return 'rgba(' + [(c >> 16) & 255, (c >> 8) & 255, c & 255].join(',') + ',' + op + ')';
    }
    return '';
}

//Recebe uma cor em Hexadecimal e uma porcentagem de intensidade
//Retorna a cor mais clara em Hexadecimal na intensidade recebida
function lightenHexColor(hex, amount) {

    if (/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {

        let c = hex.substring(1).split('');
        if (c.length == 3) {
            c = [c[0], c[0], c[1], c[1], c[2], c[2]];
        }
        let r = parseInt(c[0] + c[1], 16);
        let g = parseInt(c[2] + c[3], 16);
        let b = parseInt(c[4] + c[5], 16);

        // Aumenta a intensidade de cada canal de cor em uma porcentagem definida por 'amount'
        r = Math.min(255, Math.round(r + (255 - r) * amount));
        g = Math.min(255, Math.round(g + (255 - g) * amount));
        b = Math.min(255, Math.round(b + (255 - b) * amount));

        return `#${((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1)}`;
    }
    return '';
}

//Seleciona todos os checkboxes de uma classe knockout se colocado dentro do subscribe de um determinado campo (checkbox)
function selecionarTodosCheckboxesDaClasse(knockout, val) {
    for (const prop in knockout) {
        if (knockout[prop]?.val && typeof knockout[prop].val === "function" && knockout[prop].getType === "bool") {
            knockout[prop].val(val);
        }
    }
}

var TRACKING_RASTREADOR_SEM_POSICAO = "#a3b6bf";
var TRACKING_RASTREADOR_COM_POSICAO = "#1c75ba";
var TRACKING_RASTREADOR_COR_ONLINE = "#33cc33";
var TRACKING_RASTREADOR_COR_OFFLINE = "#e74c3c";

//Função para unificar o ícone de rastreador - TrackingIconAcompanhamento
function ObterIconeStatusTracking(status, size) {
    let color = "";
    switch (status) {
        //case 0: color = "purple"; break;
        case 1: color = TRACKING_RASTREADOR_SEM_POSICAO; break;
        case 2: color = TRACKING_RASTREADOR_COM_POSICAO; break;
        case 3: color = TRACKING_RASTREADOR_COR_ONLINE; break;
        case 4: color = TRACKING_RASTREADOR_COR_OFFLINE; break;
        default: break;
    }
    return TrackingIconWifi(color, size);
}
function TrackingIconWifi(color, size) {
    var icon =
        '<svg aria-hidden="true" focusable="false" data-prefix="fas" data-icon="wifi" class="svg-inline--fa fa-wifi fa-w-20" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512" x="0px" y="0px" width="' + size + '" height="' + size + '">' +
        '<path fill="' + color + '" d="M634.91 154.88C457.74-8.99 182.19-8.93 5.09 154.88c-6.66 6.16-6.79 16.59-.35 22.98l34.24 33.97c6.14 6.1 16.02 6.23 22.4.38 145.92-133.68 371.3-133.71 517.25 0 6.38 5.85 16.26 5.71 22.4-.38l34.24-33.97c6.43-6.39 6.3-16.82-.36-22.98zM320 352c-35.35 0-64 28.65-64 64s28.65 64 64 64 64-28.65 64-64-28.65-64-64-64zm202.67-83.59c-115.26-101.93-290.21-101.82-405.34 0-6.9 6.1-7.12 16.69-.57 23.15l34.44 33.99c6 5.92 15.66 6.32 22.05.8 83.95-72.57 209.74-72.41 293.49 0 6.39 5.52 16.05 5.13 22.05-.8l34.44-33.99c6.56-6.46 6.33-17.06-.56-23.15z"></path></svg>';
    return icon;
}