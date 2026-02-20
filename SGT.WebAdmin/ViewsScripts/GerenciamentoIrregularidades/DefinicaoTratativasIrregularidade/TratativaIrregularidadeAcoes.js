/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumAcaoTratativaIrregularidade.js" />

function GerarCheckboxsAcoes(descricaoIrregularidade) {

    _tratativaIrregularidade.TratativaIrregularidadeAcoes(EnumAcaoTratativaIrregularidade.obterOpcoesPorDescricao(descricaoIrregularidade));
}

function ObterAcoesSelecionadas() {
    let acoes = _tratativaIrregularidade.TratativaIrregularidadeAcoes()
    let acoesSelecionadas = acoes.filter(x => x.val());

    return acoesSelecionadas.map(x => x = x.ValorEnum);
}

function ConfigurarAcoesSelecionadas(tratativa) {

    let acoesPermitidasFront = _tratativaIrregularidade.TratativaIrregularidadeAcoes();

    acoesPermitidasFront.forEach(acaoPermitida => {
        tratativa.Acoes.forEach(acaoMarcada => {
            if (acaoMarcada === acaoPermitida.ValorEnum) {
                acaoPermitida.val(true);
            }
        })
    });
}