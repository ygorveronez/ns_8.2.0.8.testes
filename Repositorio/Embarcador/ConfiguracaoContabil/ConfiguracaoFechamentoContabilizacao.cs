using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoFechamentoContabilizacao : RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao>
    {
        public ConfiguracaoFechamentoContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos 

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> Consultar(int mesReferencia, int anoReferencia, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> query = ObterQueryConsulta(mesReferencia, anoReferencia, propOrdenar, dirOrdenar, inicio, limite);

            return query.ToList();
        }

        public int ContarConsulta(int mesReferencia, int anoReferencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> query = ObterQueryConsulta(mesReferencia, anoReferencia);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao BuscarConfiguracaoAtual(DateTime data)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao>();

            query = query.Where(o => o.UltimoDiaEnvio >= data.Date);

            return query.OrderBy(o => o.AnoReferencia).ThenBy(o => o.MesReferencia).FirstOrDefault();
        }

        public bool ExistePorMesEAnoReferencia(int codigo, int mesReferencia, int anoReferencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> query = ObterQueryConsulta(mesReferencia, anoReferencia);

            query = query.Where(o => o.AnoReferencia == anoReferencia && o.MesReferencia == mesReferencia && o.Codigo != codigo);

            return query.Select(o => o.Codigo).Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> ObterQueryConsulta(int mesReferencia, int anoReferencia, string propOrdenar = "", string dirOrdenar = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao>();

            if (mesReferencia > 0)
                query = query.Where(o => o.MesReferencia == mesReferencia);

            if (anoReferencia > 0)
                query = query.Where(o => o.AnoReferencia == anoReferencia);

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
