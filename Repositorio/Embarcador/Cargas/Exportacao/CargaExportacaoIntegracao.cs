using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas.Exportacao
{
    public class CargaExportacaoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao>
    {
        #region Construtores

        public CargaExportacaoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao.FiltroPesquisaCargaExportacaoIntegracao filtrosPesquisa)
        {
            var consultaCargaExportacaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao>();

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao.Where(o => o.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value);

            if (filtrosPesquisa.CodigoFilial.Count > 0)
                consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao.Where(o => filtrosPesquisa.CodigoFilial.Contains(o.Carga.Filial.Codigo));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && o.Carga.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo)) ||
                    (o.Carga.CargaAgrupamento != null && o.Carga.CargaAgrupamento.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo || o.Carga.CargaAgrupamento.VeiculosVinculados.Any(v => v.Codigo == filtrosPesquisa.CodigoVeiculo))
                );

            if (filtrosPesquisa.DataInicialAgendamento.HasValue || filtrosPesquisa.DataLimiteAgendamento.HasValue)
            {
                var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                if (filtrosPesquisa.DataInicialAgendamento.HasValue)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento >= filtrosPesquisa.DataInicialAgendamento.Value.Date);

                if (filtrosPesquisa.DataLimiteAgendamento.HasValue)
                    consultaCargaJanelaCarregamento = consultaCargaJanelaCarregamento.Where(o => o.InicioCarregamento <= filtrosPesquisa.DataLimiteAgendamento.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

                consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && consultaCargaJanelaCarregamento.Any(j => j.Carga.Codigo == o.Carga.Codigo)) ||
                    (o.Carga.CargaAgrupamento != null && consultaCargaJanelaCarregamento.Any(j => j.Carga.Codigo == o.Carga.CargaAgrupamento.Codigo))
                );
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador) ||
                    (o.Carga.CargaAgrupamento != null && o.Carga.CargaAgrupamento.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador)
                );

            if (filtrosPesquisa.Transportadores.Count > 0)
                consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && filtrosPesquisa.Transportadores.Contains(o.Carga.Empresa.Codigo)) ||
                    (o.Carga.CargaAgrupamento != null && filtrosPesquisa.Transportadores.Contains(o.Carga.CargaAgrupamento.Empresa.Codigo))
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
            {
                var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                    .Where(o => o.Pedido.NumeroEXP == filtrosPesquisa.NumeroEXP);

                consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao.Where(o =>
                    (o.Carga.CargaAgrupamento == null && consultaCargaPedido.Any(cp => cp.Carga.Codigo == o.Carga.Codigo)) ||
                    (o.Carga.CargaAgrupamento != null && consultaCargaPedido.Any(cp => cp.Carga.Codigo == o.Carga.CargaAgrupamento.Codigo))
                );
            }

            return consultaCargaExportacaoIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double tempoProximaTentativaEmMinutos, int limiteRegistros)
        {
            var consultaCargaExportacaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao>()
                .Where(o =>
                    o.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                    (o.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < numeroTentativas && o.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaEmMinutos))
                );

            return consultaCargaExportacaoIntegracao
                .Fetch(o => o.Carga)
                .OrderBy(o => o.Codigo)
                .Skip(0)
                .Take(limiteRegistros)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var consultaCargaExportacaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));

            return consultaCargaExportacaoIntegracao.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaCargaExportacaoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao>();
            int? ultimoNumero = consultaCargaExportacaoIntegracao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Exportacao.CargaExportacaoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao.FiltroPesquisaCargaExportacaoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaExportacaoIntegracao = Consultar(filtrosPesquisa);

            consultaCargaExportacaoIntegracao = consultaCargaExportacaoIntegracao
                .Fetch(o => o.Carga).ThenFetch(o => o.Filial)
                .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                .Fetch(o => o.Carga).ThenFetch(o => o.CargaAgrupamento).ThenFetch(o => o.Veiculo);

            return ObterLista(consultaCargaExportacaoIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.Exportacao.FiltroPesquisaCargaExportacaoIntegracao filtrosPesquisa)
        {
            var consultaCargaExportacaoIntegracao = Consultar(filtrosPesquisa);

            return consultaCargaExportacaoIntegracao.Count();
        }

        #endregion Métodos Públicos
    }
}
