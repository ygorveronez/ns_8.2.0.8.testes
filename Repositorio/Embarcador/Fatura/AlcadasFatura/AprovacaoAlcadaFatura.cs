using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Fatura.AlcadasFatura
{
    public sealed class AprovacaoAlcadaFatura : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura,
        Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura,
        Dominio.Entidades.Embarcador.Fatura.Fatura
    >
    {
        #region Construtores

        public AprovacaoAlcadaFatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Fatura.Fatura> Consultar(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFaturaAprovacao filtrosPesquisa)
        {
            var consultaFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Fatura>();
            var consultaAlcadaFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Numero > 0)
                consultaFatura = consultaFatura.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                consultaFatura = consultaFatura.Where(o => o.DataFatura.Date >= filtrosPesquisa.DataInicio.Date);

            if (filtrosPesquisa.DataLimite != DateTime.MinValue)
                consultaFatura = consultaFatura.Where(o => o.DataFatura.Date <= filtrosPesquisa.DataLimite.Date);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaFatura = consultaFatura.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaFatura = consultaFatura.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaFatura = consultaAlcadaFatura.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.AguardandoAprovacao)
                consultaAlcadaFatura = consultaAlcadaFatura.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaFatura.Where(o => consultaAlcadaFatura.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Fatura.Fatura> Consultar(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFaturaAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFatura = Consultar(filtrosPesquisa);

            return ObterLista(consultaFatura, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFaturaAprovacao filtrosPesquisa)
        {
            var consultaFatura = Consultar(filtrosPesquisa);

            return consultaFatura.Count();
        }

        public bool PossuiRegrasCadastradas()
        {
            var consultaAlcadaFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura>()
                .Where(o => o.Ativo);

            return consultaAlcadaFatura.Any();
        }

        #endregion
    }
}
