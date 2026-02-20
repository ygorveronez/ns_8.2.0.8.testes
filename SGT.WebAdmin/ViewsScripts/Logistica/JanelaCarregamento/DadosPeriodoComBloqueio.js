var DadosPeriodoComBloqueio = function () {
}

DadosPeriodoComBloqueio.prototype = {
    ObterDadosPeriodoComBloqueio: function (bloqueio) {
        var evento = {};

        evento.className = "backgroundPeriodoComBloqueioCalendario";
        evento.constraint = "horarioDisponivel";
        evento.id = "horarioDisponivel";
        evento.start = moment(bloqueio.DataInicio, "DD/MM/YYYY HH:mm:ss");
        evento.end = moment(bloqueio.DataTermino, "DD/MM/YYYY HH:mm:ss");
        evento.title = bloqueio.NumeroCargasPossiveis > 0 ? Localization.Resources.Cargas.Carga.BloqueioMaximoDeCargasSimultaneas.format(bloqueio.NumeroCargasPossiveis) : Localization.Resources.Cargas.Carga.BloqueioTotal;
        evento.rendering = 'block';
        
        return evento;     
    }
}