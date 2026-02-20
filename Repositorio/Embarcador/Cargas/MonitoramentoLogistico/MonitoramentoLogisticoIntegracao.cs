using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.MonitoramentoLogistico
{
    public class MonitoramentoLogisticoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>
    {
        #region Construtores

        public MonitoramentoLogisticoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.MonitoramentoLogistico.FiltroPesquisaCargaMonitoramentoLogisticoIntegracao filtrosPesquisa)
        {
            var consultaCargaMonitoramentoLogisticoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(x => x.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML);

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(o => o.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value);

            if (filtrosPesquisa.CodigoFilial.Count > 0)
                consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(o => filtrosPesquisa.CodigoFilial.Contains(o.Carga.Filial.Codigo));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo)) ||
                    (o.Carga.CargaAgrupamento != null && o.Carga.CargaAgrupamento.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.CargaAgrupamento.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo))
                );

            if (filtrosPesquisa.DataInicialAgendamento.HasValue || filtrosPesquisa.DataLimiteAgendamento.HasValue)
            {
                var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                    .Where(o => (((TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == TipoCargaJanelaCarregamento.Carregamento));

                if (filtrosPesquisa.DataInicialAgendamento.HasValue)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento >= filtrosPesquisa.DataInicialAgendamento.Value.Date);

                if (filtrosPesquisa.DataLimiteAgendamento.HasValue)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento <= filtrosPesquisa.DataLimiteAgendamento.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

                consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && consultaCargaJanelaCarregamento.Any(j => j.Carga.Codigo == o.Carga.Codigo)) ||
                    (o.Carga.CargaAgrupamento != null && consultaCargaJanelaCarregamento.Any(j => j.Carga.Codigo == o.Carga.CargaAgrupamento.Codigo))
                );
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador) ||
                    (o.Carga.CargaAgrupamento != null && o.Carga.CargaAgrupamento.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador)
                );

            if (!string.IsNullOrEmpty(filtrosPesquisa.Protocolo))
            {   
                consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(o => o.Protocolo == filtrosPesquisa.Protocolo);
            }

            if (filtrosPesquisa.Transportadores.Count > 0)
                consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && filtrosPesquisa.Transportadores.Contains(o.Carga.Empresa.Codigo)) ||
                    (o.Carga.CargaAgrupamento != null && filtrosPesquisa.Transportadores.Contains(o.Carga.CargaAgrupamento.Empresa.Codigo))
                );

            return consultaCargaMonitoramentoLogisticoIntegracao;
        }

        #endregion

        #region Métodos Públicos
        

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaMonitoramentoLogisticoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>()
                .Where(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML && (o.Carga.Codigo == codigoCarga || o.Carga.CargaAgrupamento.Codigo == codigoCarga));

            
            return consultaCargaMonitoramentoLogisticoIntegracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarPorCargas(List<int> codigosCarga)
        {
            var consultaCargaMonitoramentoLogisticoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>()
                .Where(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskSML 
                && (codigosCarga.Contains(o.Carga.Codigo) || codigosCarga.Contains(o.Carga.CargaAgrupamento.Codigo)));

            return consultaCargaMonitoramentoLogisticoIntegracao.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaCargaMonitoramentoLogisticoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaCargaMonitoramentoLogisticoIntegracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.MonitoramentoLogistico.FiltroPesquisaCargaMonitoramentoLogisticoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaMonitoramentoLogisticoIntegracao = Consultar(filtrosPesquisa);

            consultaCargaMonitoramentoLogisticoIntegracao = consultaCargaMonitoramentoLogisticoIntegracao
                .Fetch(o => o.Carga).ThenFetch(o => o.Filial)
                .Fetch(o => o.Carga).ThenFetch(o => o.Motoristas)
                .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                .Fetch(o => o.Carga).ThenFetch(o => o.CargaAgrupamento).ThenFetch(o => o.Veiculo)
                .Fetch(o => o.TipoIntegracao);

            return ObterLista(consultaCargaMonitoramentoLogisticoIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.MonitoramentoLogistico.FiltroPesquisaCargaMonitoramentoLogisticoIntegracao filtrosPesquisa)
        {
            var consultaCargaMonitoramentoLogisticoIntegracao = Consultar(filtrosPesquisa);

            return consultaCargaMonitoramentoLogisticoIntegracao.Count();
        }

        #endregion
    }
}
