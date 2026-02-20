using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.ISS
{
    public class AliquotaISS : RepositorioBase<Dominio.Entidades.Embarcador.ISS.AliquotaISS>
    {
        #region Construtores

        public AliquotaISS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.ISS.AliquotaISS> Consultar(Dominio.ObjetosDeValor.Embarcador.ISS.FiltroPesquisaAliquotaISS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.ISS.FiltroPesquisaAliquotaISS filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.ISS.AliquotaISS BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ISS.AliquotaISS>()
                .Where(o => o.Descricao == descricao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.ISS.AliquotaISS BuscarPorLocalidadeEAtivo(int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ISS.AliquotaISS>()
                .Where(o => o.Localidade.Codigo == codigoLocalidade && o.Ativo);

            return query.FirstOrDefault();
        }

        public bool ExisteAtivo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ISS.AliquotaISS>()
                .Where(o => o.Ativo);

            return query.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ISS.AliquotaISS> Consultar(Dominio.ObjetosDeValor.Embarcador.ISS.FiltroPesquisaAliquotaISS filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ISS.AliquotaISS>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (filtrosPesquisa.DataInicio.HasValue)
                query = query.Where(o => o.DataInicio.Value.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataFim.HasValue)
                query = query.Where(o => o.DataFim.Value.Date <= filtrosPesquisa.DataFim.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigoLocalidade > 0)
                query = query.Where(o => o.Localidade.Codigo == filtrosPesquisa.CodigoLocalidade);

            return query;
        }

        #endregion
    }
}