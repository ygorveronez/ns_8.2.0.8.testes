using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto
{
    public sealed class AprovacaoAlcadaContratoFreteAcrescimoDesconto : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto,
        Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto,
        Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto
    >
    {
        #region Construtores

        public AprovacaoAlcadaContratoFreteAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao filtrosPesquisa)
        {
            var consultaContratoFreteAcrescimoDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();
            var consultaAlcadaContratoFreteAcrescimoDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.NumeroContrato > 0)
                consultaContratoFreteAcrescimoDesconto = consultaContratoFreteAcrescimoDesconto.Where(o => o.ContratoFrete.NumeroContrato == filtrosPesquisa.NumeroContrato);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                consultaContratoFreteAcrescimoDesconto = consultaContratoFreteAcrescimoDesconto.Where(o => o.Data.Date >= filtrosPesquisa.DataInicio.Date);

            if (filtrosPesquisa.DataLimite != DateTime.MinValue)
                consultaContratoFreteAcrescimoDesconto = consultaContratoFreteAcrescimoDesconto.Where(o => o.Data.Date <= filtrosPesquisa.DataLimite.Date);

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto.Todos)
                consultaContratoFreteAcrescimoDesconto = consultaContratoFreteAcrescimoDesconto.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoJustificativa > 0)
                consultaContratoFreteAcrescimoDesconto = consultaContratoFreteAcrescimoDesconto.Where(o => o.Justificativa.Codigo == filtrosPesquisa.CodigoJustificativa);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaContratoFreteAcrescimoDesconto = consultaAlcadaContratoFreteAcrescimoDesconto.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto.AgAprovacao)
                consultaAlcadaContratoFreteAcrescimoDesconto = consultaAlcadaContratoFreteAcrescimoDesconto.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaContratoFreteAcrescimoDesconto.Where(o => consultaAlcadaContratoFreteAcrescimoDesconto.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaContratoFreteAcrescimoDesconto = Consultar(filtrosPesquisa);

            return ObterLista(consultaContratoFreteAcrescimoDesconto, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao filtrosPesquisa)
        {
            var consultaContratoFreteAcrescimoDesconto = Consultar(filtrosPesquisa);

            return consultaContratoFreteAcrescimoDesconto.Count();
        }

        #endregion
    }
}
