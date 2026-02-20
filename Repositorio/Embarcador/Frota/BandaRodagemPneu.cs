using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frota
{
    public class BandaRodagemPneu : RepositorioBase<Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu>
    {
        #region Construtores

        public BandaRodagemPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBandaRodagemPneu filtrosPesquisa)
        {
            var consultaBandaRodagemPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu>();

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaBandaRodagemPneu = consultaBandaRodagemPneu.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoMarca > 0)
                consultaBandaRodagemPneu = consultaBandaRodagemPneu.Where(o => o.Marca.Codigo == filtrosPesquisa.CodigoMarca);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaBandaRodagemPneu = consultaBandaRodagemPneu.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaBandaRodagemPneu = consultaBandaRodagemPneu.Where(o => o.Ativo == ativo);
            }

            if (filtrosPesquisa.Tipo.HasValue)
                consultaBandaRodagemPneu = consultaBandaRodagemPneu.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            return consultaBandaRodagemPneu;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu BuscarPorCodigo(int codigo)
        {
            var bandaRodagemPneu = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return bandaRodagemPneu;
        }

        public List<Dominio.Entidades.Embarcador.Frota.BandaRodagemPneu> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBandaRodagemPneu filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaBandaRodagemPneu = Consultar(filtrosPesquisa);

            return ObterLista(consultaBandaRodagemPneu, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBandaRodagemPneu filtrosPesquisa)
        {
            var consultaBandaRodagemPneu = Consultar(filtrosPesquisa);

            return consultaBandaRodagemPneu.Count();
        }

        #endregion
    }
}
