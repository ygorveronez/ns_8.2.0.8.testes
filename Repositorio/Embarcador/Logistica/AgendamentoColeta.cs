using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AgendamentoColeta : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>
    {
        #region Construtores

        public AgendamentoColeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public AgendamentoColeta(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            var result = query.Where(o => o.Codigo == codigo);

            return result
                .Fetch(o => o.Carga)
                .ThenFetch(o => o.Veiculo)
                .ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Carga)
                .ThenFetch(o => o.Empresa)
                .Fetch(o => o.Carga)
                .Fetch(o => o.Filial)
                .Fetch(o => o.Remetente)
                .Fetch(o => o.Destinatario)
                .Fetch(o => o.Recebedor)
                .Fetch(o => o.Transportador)
                .Fetch(o => o.TipoCarga)
                .Fetch(o => o.ModeloVeicular)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            var result = query.Where(o => o.Carga.Codigo == codigoCarga);

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            var result = query.Where(o => o.Carga.Codigo == codigoCarga);

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> BuscarPorPedidos(List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> result = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
                query = query.Where(o => tmp.Contains(o.Pedido.Codigo));
                result.AddRange(query.ToList());
                start += take;
            }
            return result.Distinct().ToList();

        }

        public async Task<List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>> BuscarPorPedidosAsync(List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> result = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            int take = 1000;
            int start = 0;

            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
                query = query.Where(o => tmp.Contains(o.Pedido.Codigo));
                result.AddRange(await query.ToListAsync());
                start += take;
            }

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta BuscarFirstOrDefaultPorPedidos(List<int> codigosPedidos)
        {

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> result = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
                query = query.Where(o => tmp.Contains(o.Pedido.Codigo));

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = query.FirstOrDefault();
                if (agendamento != null)
                    return agendamento;

                start += take;
            }
            return null;
        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta BuscarAgendamentoAbertoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);
            query = query.Where(o => o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Cancelado &&
                                     o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.CanceladoEmbarcador);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta BuscarAgendamentoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public string BuscarSolicitantePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>()
                .Where(obj => obj.Carga.Codigo == codigoCarga).Select(obj => obj.EmailSolicitante);

            return query.FirstOrDefault();
        }

        public bool ExisteAgendamentoPorNumeroCargaEFilial(string numeroCarga, int filial, List<SituacaoAgendamentoColeta> situacoesIgnorar)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            var result = query.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga && o.Carga.Filial.Codigo == filial && (!o.Situacao.HasValue || !situacoesIgnorar.Contains(o.Situacao.Value)));

            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> BuscarPorCargas(List<int> codigosCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCarga.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentoColetaRetornar = new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                agendamentoColetaRetornar.AddRange(query.Where(o => codigosCarga.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Carga.Codigo)).ToList());

            return agendamentoColetaRetornar;
        }

        public bool BuscarSePossuiAgendamentoPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();

            query = query.Where(obj => obj.Pedido.Codigo == codigoPedido
                                && obj.AgendamentoColeta.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Cancelado
                                && obj.AgendamentoColeta.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.CanceladoEmbarcador);

            return query.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            result = result
                .Fetch(o => o.TipoCarga)
                .Fetch(o => o.Destinatario)
                .Fetch(o => o.Remetente)
                .Fetch(o => o.Pedido)
                .Fetch(o => o.Carga);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

        }

        public List<int> BuscarCodigosTiposDeCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            query = query.Where(o => o.AgendamentoColeta.Carga.Codigo == codigoCarga);

            return query
                .Select(o => o.Pedido.TipoDeCarga.Codigo)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarTiposDeCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido>();
            query = query.Where(o => o.AgendamentoColeta.Carga.Codigo == codigoCarga);

            return query
                .Select(o => o.Pedido.TipoDeCarga)
                .Distinct()
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public int ObterProximoNumeroSequencial()
        {
            var consultaAgendamentoColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
            int? ultimoNumeroSequencial = consultaAgendamentoColeta.Max(o => (int?)o.Sequencia);

            return ultimoNumeroSequencial.HasValue ? (ultimoNumeroSequencial.Value + 1) : 1;
        }

        public int ObterProximaSenhaSequencial()
        {
            var consultaAgendamentoColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();
            int? ultimaSenhaSequencial = consultaAgendamentoColeta.Max(o => o.SenhaSequencial);

            return ultimaSenhaSequencial.HasValue ? (ultimaSenhaSequencial.Value + 1) : 1;
        }

        public bool BuscarSePossuiCargaPerigosaPorPedido(int codigoPedido, string numeroPedidoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>()
                .Where(obj => obj.Pedido.Codigo == codigoPedido && obj.Pedido.NumeroPedidoEmbarcador.Equals(numeroPedidoEmbarcador) && obj.CargaPerigosa == true);

            return query.Count() > 0;
        }

        public bool ExisteAgendamentoAbertoPorCarga(int codigoCarga)
        {
            var consultaAgendamentoColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Cancelado &&
                    o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.CanceladoEmbarcador
                );

            return consultaAgendamentoColeta.Any();
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AgendamentoColeta>> BuscarPorAgendamentoAsync(List<int> codigoCarga)
        {
            string sql = @$"SELECT coleta.CAR_CODIGO as CodigoCarga,
                                   coleta.ACO_CODIGO as Codigo,
                                   coleta.ACO_DATA_ENTREGA as DataEntrega,
                                   coleta.ACO_TRANSPORTADOR_MANUAL as TransportadorManual
                              FROM T_AGENDAMENTO_COLETA coleta
                             WHERE coleta.CAR_CODIGO IN ({string.Join(",", codigoCarga)})"; // SQL-INJECTION-SAFE

            var queryResult = this.SessionNHiBernate.CreateSQLQuery(sql);

            return queryResult.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.AgendamentoColeta)))
                .ListAsync<Dominio.ObjetosDeValor.Embarcador.Logistica.AgendamentoColeta>(CancellationToken);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoColeta filtrosPesquisa)
        {
            var consultaAgendamentoColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

            consultaAgendamentoColeta = consultaAgendamentoColeta.Where(obj => obj.AgendamentoPai.Value == false || obj.AgendamentoPai.HasValue == false);

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Remetente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoas);

            if (filtrosPesquisa.CodigoRemetente > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Remetente.CPF_CNPJ == filtrosPesquisa.CodigoRemetente);

            if (filtrosPesquisa.Situacao.HasValue)
            {
                var consultaCargaJanelaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>()
                    .Where(o => o.Situacao == filtrosPesquisa.Situacao.Value && (((Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamento?)o.Tipo).HasValue == false || o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaJanelaCarregamento.Carregamento));

                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => consultaCargaJanelaCarregamento.Any(j => j.Carga.Codigo == o.Carga.Codigo));
            }

            if (filtrosPesquisa.Etapa.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Carga.SituacaoCarga == filtrosPesquisa.Etapa);

            if (filtrosPesquisa.DataColeta.HasValue)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.DataColeta.Value.Date == filtrosPesquisa.DataColeta.Value.Date);

            if (filtrosPesquisa.DataEntrega.HasValue)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.DataEntrega.Value.Date == filtrosPesquisa.DataEntrega.Value.Date);

            if (filtrosPesquisa.DataCriacao.HasValue)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.DataCriacao.Value.Date == filtrosPesquisa.DataCriacao);

            if (filtrosPesquisa.DataAgendamento.HasValue)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.DataAgendamento == filtrosPesquisa.DataAgendamento);

            if (filtrosPesquisa.TipoCargas != null && filtrosPesquisa.TipoCargas.Count > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.TipoCarga == null || filtrosPesquisa.TipoCargas.Contains(o.TipoCarga.Codigo));

            if (filtrosPesquisa.TipoOperacoes != null && filtrosPesquisa.TipoOperacoes.Count > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => filtrosPesquisa.TipoOperacoes.Contains(o.TipoOperacao.Codigo));

            if (filtrosPesquisa.TipoCarga > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.TipoCarga.Codigo == filtrosPesquisa.TipoCarga);

            if (filtrosPesquisa.Transportadores != null && filtrosPesquisa.Transportadores.Count > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Transportador == null || filtrosPesquisa.Transportadores.Contains(o.Transportador.Codigo));

            if (filtrosPesquisa.CodigoEmpresaLogada > 0)
            {
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoEmpresaLogada);
            }

            if (filtrosPesquisa.ModelosVeiculares != null && filtrosPesquisa.ModelosVeiculares.Count > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.ModeloVeicular == null || filtrosPesquisa.ModelosVeiculares.Contains(o.ModeloVeicular.Codigo));

            if (filtrosPesquisa.Destinatario > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Destinatario.CPF_CNPJ == filtrosPesquisa.Destinatario);

            if (filtrosPesquisa.Recebedor > 0)
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Recebedor.CPF_CNPJ == filtrosPesquisa.Recebedor);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.Carga);

            if (filtrosPesquisa.SituacaoJanelaDescarregamento.HasValue)
            {
                var queryJanelaDescarregamento = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                    .Where(o =>
                        o.Situacao == filtrosPesquisa.SituacaoJanelaDescarregamento &&
                        ((bool?)o.Cancelada ?? false) == false
                    );

                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => queryJanelaDescarregamento.Any(j => j.Carga.Codigo == o.Carga.Codigo));
            }

            if (filtrosPesquisa.OcultarDescargaCancelada)
            {
                var queryJanelaDescarregamento = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                    .Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamento.Cancelado);

                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(o => !queryJanelaDescarregamento.Any(j => j.Carga.Codigo == o.Carga.Codigo));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Senha))
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(obj => obj.Senha == filtrosPesquisa.Senha);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Pedido))
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(obj => obj.Pedidos.Any(x => x.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.Pedido));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PedidoEmbarcador))
                consultaAgendamentoColeta = consultaAgendamentoColeta.Where(obj => obj.Carga.Pedidos.Any(x => x.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.PedidoEmbarcador) || obj.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.PedidoEmbarcador);

            return consultaAgendamentoColeta;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaAgendamento> ConsultarRelatorioJanelaAgendamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaJanelaAgendamento = new Consulta.ConsultaJanelaAgendamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaJanelaAgendamento.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaAgendamento)));

            return consultaJanelaAgendamento.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaAgendamento>();
        }

        public int ContarConsultaRelatorioJanelaAgendamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaJanelaAgendamento = new Consulta.ConsultaJanelaAgendamento().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaJanelaAgendamento.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.AgendaCancelada> ConsultarRelatorioAgendaCancelada(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAgendaCancelada = new Consulta.ConsultaAgendaCancelada().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaAgendaCancelada.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.AgendaCancelada)));

            return consultaAgendaCancelada.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AgendaCancelada>();
        }

        public int ContarConsultaRelatorioAgendaCancelada(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaAgendaCancelada = new Consulta.ConsultaAgendaCancelada().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaAgendaCancelada.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
