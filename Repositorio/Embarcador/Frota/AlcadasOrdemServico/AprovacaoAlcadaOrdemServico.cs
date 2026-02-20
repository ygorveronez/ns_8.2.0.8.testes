using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota.AlcadasOrdemServico
{
    public sealed class AprovacaoAlcadaOrdemServico : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico,
        Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico,
        Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota
    >
    {
        #region Construtores

        public AprovacaoAlcadaOrdemServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServicoAprovacao filtrosPesquisa)
        {
            var consultaOrdemServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();
            var consultaAlcadaOrdemServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Numero > 0)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.DataProgramada.Date >= filtrosPesquisa.DataInicio.Date);

            if (filtrosPesquisa.DataLimite != DateTime.MinValue)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.DataProgramada.Date <= filtrosPesquisa.DataLimite.Date);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaOrdemServico = consultaOrdemServico.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaOrdemServico = consultaAlcadaOrdemServico.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.AguardandoAprovacao)
                consultaAlcadaOrdemServico = consultaAlcadaOrdemServico.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaOrdemServico.Where(o => consultaAlcadaOrdemServico.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServicoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOrdemServico = Consultar(filtrosPesquisa);

            return ObterLista(consultaOrdemServico, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServicoAprovacao filtrosPesquisa)
        {
            var consultaOrdemServico = Consultar(filtrosPesquisa);

            return consultaOrdemServico.Count();
        }

        #endregion
    }
}
