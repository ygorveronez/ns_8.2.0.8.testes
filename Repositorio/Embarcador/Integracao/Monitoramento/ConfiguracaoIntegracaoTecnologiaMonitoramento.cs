using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.Integracao.Monitoramento
{
    public class ConfiguracaoIntegracaoTecnologiaMonitoramento : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento>
    {
        public ConfiguracaoIntegracaoTecnologiaMonitoramento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento>();

            query = query.Where(o => o.Tipo == tipo);

            return query.FirstOrDefault();
        }

        public bool ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento>();

            query = query.Where(o => o.Tipo == tipo);

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaConfiguracaoIntegracaoTecnologiaMonitoramento filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int limiteRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> query = ObterQueryConsulta(filtrosPesquisa, propOrdena, dirOrdena, inicioRegistros, limiteRegistros);

            return query.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaConfiguracaoIntegracaoTecnologiaMonitoramento filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> query = ObterQueryConsulta(filtrosPesquisa);

            return query.Select(o => o.Codigo).Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaConfiguracaoIntegracaoTecnologiaMonitoramento filtrosPesquisa, string propOrdena = null, string dirOrdena = null, int? inicioRegistros = null, int? limiteRegistros = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento>();

            if (filtrosPesquisa.Habilitada.HasValue)
                query = query.Where(o => o.Habilitada == filtrosPesquisa.Habilitada.Value);

            if (filtrosPesquisa.Tipo.HasValue)
                query = query.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            if (!string.IsNullOrWhiteSpace(propOrdena) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propOrdena + " " + dirOrdena);

            if (inicioRegistros.HasValue)
                query = query.Skip(inicioRegistros.Value);

            if (limiteRegistros.HasValue)
                query = query.Take(limiteRegistros.Value);

            return query;
        }
    }
}
