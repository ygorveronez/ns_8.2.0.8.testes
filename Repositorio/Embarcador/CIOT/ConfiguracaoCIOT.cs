using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CIOT
{
    public class ConfiguracaoCIOT : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>
    {
        public ConfiguracaoCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> BuscarPorConsultarFaturas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>();
            var result = from obj in query where obj.ConsultarFaturas && obj.Ativo select obj;
            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT> BuscarOperadorasDisponiveis()
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>();

            query = query.Where(o => o.Ativo);

            return query.Select(o => o.OperadoraCIOT).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> BuscarAtivas()
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>();

            query = query.Where(o => o.Ativo);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT BuscarPrimeiroPorOperadora(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT operadoraCIOT)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>();

            query = query.Where(o => o.OperadoraCIOT == operadoraCIOT && o.Ativo);

            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT> BuscarOperadorasComConciliacaoFinanceiraHabilitada()
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>();

            query = query.Where(o => o.HabilitarConciliacaoFinanceira && o.Ativo);

            return query.Select(o => o.OperadoraCIOT).Distinct().ToList();
        }

        #region Consulta

        public List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> query = ObterQueryConsulta(descricao, status);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> query = ObterQueryConsulta(descricao, status);

            return query.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> ObterQueryConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query;
        }

        #endregion
    }
}
