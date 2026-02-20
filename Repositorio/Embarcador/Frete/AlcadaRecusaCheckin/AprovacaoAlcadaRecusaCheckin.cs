using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete.AlcadaRecusaCheckin
{
    public class AprovacaoAlcadaRecusaCheckin : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin,
        Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin,
        Dominio.Entidades.Embarcador.Cargas.CargaCTe
    >
    {
        #region Construtores

        public AprovacaoAlcadaRecusaCheckin(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRecusaCheckinAprovacao filtrosPesquisa)
        {
            var consultaCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (filtrosPesquisa.NumeroCTe > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Numero == filtrosPesquisa.NumeroCTe);

            if (filtrosPesquisa.SituacaoCheckin.HasValue)
            {
                consultaCargaCTe = consultaCargaCTe.Where(o => o.SituacaoCheckin == filtrosPesquisa.SituacaoCheckin.Value);

                if (filtrosPesquisa.SituacaoCheckin.Value == SituacaoCheckin.SemRegraAprovacao)
                    return consultaCargaCTe;
            }
            else
                consultaCargaCTe = consultaCargaCTe.Where(o => SituacaoCheckinHelper.ObterSituacoesAprovacao().Contains(o.SituacaoCheckin));

            var consultaAprovacaoCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAprovacaoCargaCTe = consultaAprovacaoCargaCTe.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoCheckin == SituacaoCheckin.AguardandoAprovacao)
                consultaAprovacaoCargaCTe = consultaAprovacaoCargaCTe.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SerieCte > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Serie.Numero == filtrosPesquisa.SerieCte);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCarga))
                consultaCargaCTe = consultaCargaCTe.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCarga);

            if (filtrosPesquisa.DataCriacaoCarga.HasValue)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.Carga.DataCriacaoCarga == filtrosPesquisa.DataCriacaoCarga.Value);

            if (filtrosPesquisa.Filial > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.Carga.Filial.Codigo == filtrosPesquisa.Filial);

            if (filtrosPesquisa.Transportador > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.Carga.Empresa.Codigo == filtrosPesquisa.Transportador);
            
            if (filtrosPesquisa.TipoOperacao > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.Carga.TipoOperacao.Codigo == filtrosPesquisa.TipoOperacao);

            if (filtrosPesquisa.SituacaoCheckin.HasValue)
                return consultaCargaCTe.Where(o => consultaAprovacaoCargaCTe.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaCargaCTe.Where(o =>
                o.SituacaoCheckin == SituacaoCheckin.SemRegraAprovacao ||
                consultaAprovacaoCargaCTe.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarSemRegraAprovacaoPorCodigos(List<int> codigos)
        {
            var consultaCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => codigos.Contains(o.Codigo) && o.SituacaoCheckin == SituacaoCheckin.SemRegraAprovacao);

            return consultaCargaCTe.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRecusaCheckinAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaCTe = Consultar(filtrosPesquisa);

            return ObterLista(consultaCargaCTe, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRecusaCheckinAprovacao filtrosPesquisa)
        {
            var consultaCargaCTe = Consultar(filtrosPesquisa);

            return consultaCargaCTe.Count();
        }

        #endregion Métodos Públicos
    }
}
