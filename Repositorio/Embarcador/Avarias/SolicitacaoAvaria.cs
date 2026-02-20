using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Avarias
{
    public class SolicitacaoAvaria : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>
    {
        public SolicitacaoAvaria(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public SolicitacaoAvaria(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            int? retorno = query.Max(o => (int?)o.NumeroAvaria);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> _Consultar(int solicitante, int numeroAvaria, DateTime dataInicio, DateTime dataFim, int transportadora, int motivoAvaria, string carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacao, int numeroNota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            var result = from obj in query select obj;

            if (solicitante > 0)
                result = result.Where(o => o.Solicitante.Codigo == solicitante);

            if (numeroAvaria > 0)
                result = result.Where(o => o.NumeroAvaria == numeroAvaria);

            if (!string.IsNullOrWhiteSpace(carga) && carga != "0")
                result = result.Where(o => o.Carga.CodigoCargaEmbarcador == carga);

            if (dataInicio != DateTime.MinValue && dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataAvaria >= dataInicio && o.DataAvaria < dataFim.AddDays(1));
            else if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataAvaria >= dataInicio);
            else if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataAvaria < dataFim.AddDays(1));

            if (transportadora > 0)
                result = result.Where(o => o.Transportador.Codigo == transportadora);

            if (motivoAvaria > 0)
                result = result.Where(o => o.MotivoAvaria.Codigo == motivoAvaria);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Todas)
                result = result.Where(o => o.Situacao == situacao);

            if (numeroNota > 0)
            {
                result = result.Where(o => o.Carga.Pedidos.Any(pedido => pedido.NotasFiscais.Any(nota => nota.XMLNotaFiscal.Numero == numeroNota)));
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> Consultar(int solicitante, int numeroAvaria, DateTime dataInicio, DateTime dataFim, int transportadora, int motivoAvaria, string carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros, int numeroNota)
        {
            var result = _Consultar(solicitante, numeroAvaria, dataInicio, dataFim, transportadora, motivoAvaria, carga, situacao, numeroNota);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Carga)
                .Fetch(obj => obj.Solicitante)
                .Fetch(obj => obj.MotivoAvaria)
                .ToList();
        }

        public int ContarConsulta(int solicitante, int numeroAvaria, DateTime dataInicio, DateTime dataFim, int transportadora, int motivoAvaria, string carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria situacao, int numeroNota)
        {
            var result = _Consultar(solicitante, numeroAvaria, dataInicio, dataFim, transportadora, motivoAvaria, carga, situacao, numeroNota);

            return result.Count();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> _ConsultarAvariasDisponiveis(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaSolicitacaoAvaria filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            var result = from obj in query
                         where
                            obj.Lote == null &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.AgLote
                         select obj;

            if (filtrosPesquisa.Transportadora > 0)
                result = result.Where(o => o.Transportador.Codigo == filtrosPesquisa.Transportadora);

            if (filtrosPesquisa.Filial > 0)
                result = result.Where(o => o.Carga.Filial.Codigo == filtrosPesquisa.Filial);

            if (filtrosPesquisa.Motivo > 0)
                result = result.Where(o => o.MotivoAvaria.Codigo == filtrosPesquisa.Motivo);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue && filtrosPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(o => o.DataAvaria >= filtrosPesquisa.DataInicio && o.DataAvaria < filtrosPesquisa.DataFim.AddDays(1));
            else if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataAvaria >= filtrosPesquisa.DataInicio);
            else if (filtrosPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(o => o.DataAvaria < filtrosPesquisa.DataFim.AddDays(1));


            if (filtrosPesquisa.TiposOperacao.Count > 0)
                result = result.Where(o => filtrosPesquisa.TiposOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.CodigoUsuario > 0)
            {
                var queryResp = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria>();
                var resultResp = from obj in queryResp where obj.Usuario.Codigo == filtrosPesquisa.CodigoUsuario select obj.SolicitacaoAvaria;

                result = result.Where(o => resultResp.Contains(o));
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> ConsultarAvariasDisponiveis(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaSolicitacaoAvaria filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarAvariasDisponiveis(filtrosPesquisa);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultaAvariasDisponiveis(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaSolicitacaoAvaria filtrosPesquisa)
        {
            var result = _ConsultarAvariasDisponiveis(filtrosPesquisa);

            return result.Count();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> _ConsultarPorLote(int lote, int numeroAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            var result = from obj in query where obj.Lote.Codigo == lote select obj;

            if (numeroAvaria > 0)
                result = result.Where(o => o.NumeroAvaria == numeroAvaria);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> BuscarPorLote(int lote)
        {
            int numeroAvaria = 0;
            var result = _ConsultarPorLote(lote, numeroAvaria);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria> ConsultarPorLote(int lote, int numeroAvaria, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarPorLote(lote, numeroAvaria);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.MotivoDesconto)
                .ToList();
        }

        public int ContarConsultaPorLote(int lote, int numeroAvaria)
        {
            var result = _ConsultarPorLote(lote, numeroAvaria);

            return result.Count();
        }

        public List<Dominio.Entidades.Usuario> ResponsavelSolicitacao(int avaria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();

            var resultAutorizacao = (from obj in queryAutorizacao
                                     where obj.SolicitacaoAvaria.Codigo == avaria && obj.EtapaAutorizacaoAvaria == etapa
                                     group obj by obj.Usuario.Codigo into g
                                     select g.Key).ToList();

            var result = from obj in query where resultAutorizacao.Contains(obj.Codigo) select obj;

            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            // Validação também é feita em Repositorio.Embarcador.Carga.Cargas.Carga_ConsultarAvarias
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria[] sitaucaoAvariaExceto = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria[]
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Cancelada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.RejeitadaAutorizacao
            };

            var result = from obj in query
                         where
                            obj.Carga.Codigo == carga &&
                            !sitaucaoAvariaExceto.Contains(obj.Situacao)
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria BuscarPorCargaEMotivo(int carga, int motivoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria>();

            // Validação também é feita em Repositorio.Embarcador.Carga.Cargas.Carga_ConsultarAvarias
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria[] sitaucaoAvariaExceto = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria[]
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.Cancelada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvaria.RejeitadaAutorizacao
            };

            var result = from obj in query
                         where
                            obj.Carga.Codigo == carga &&
                            obj.MotivoAvaria.Codigo == motivoAvaria &&
                            !sitaucaoAvariaExceto.Contains(obj.Situacao)
                         select obj;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> _ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            if (filtroPesquisa.NumeroAvaria > 0)
                query = query.Where(o => o.SolicitacaoAvaria.NumeroAvaria == filtroPesquisa.NumeroAvaria);

            if (filtroPesquisa.CodigoSolicitante > 0)
                query = query.Where(o => o.SolicitacaoAvaria.Solicitante.Codigo == filtroPesquisa.CodigoSolicitante);

            if (filtroPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.SolicitacaoAvaria.Transportador.Codigo == filtroPesquisa.CodigoTransportador);

            if (filtroPesquisa.SituacaoAvaria?.Count > 0)
                query = query.Where(o => filtroPesquisa.SituacaoAvaria.Contains(o.SolicitacaoAvaria.Situacao));

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.CodigoCargaEmbarcador))
                query = query.Where(o => o.SolicitacaoAvaria.Carga.CodigoCargaEmbarcador.Contains(filtroPesquisa.CodigoCargaEmbarcador));

            if (filtroPesquisa.DataSolicitacaoInicial != DateTime.MinValue && filtroPesquisa.DataSolicitacaoFinal != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.DataSolicitacao >= filtroPesquisa.DataSolicitacaoInicial && o.SolicitacaoAvaria.DataSolicitacao < filtroPesquisa.DataSolicitacaoFinal.AddDays(1));
            else if (filtroPesquisa.DataSolicitacaoInicial != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.DataSolicitacao >= filtroPesquisa.DataSolicitacaoInicial);
            else if (filtroPesquisa.DataSolicitacaoFinal != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.DataSolicitacao < filtroPesquisa.DataSolicitacaoFinal.AddDays(1));

            if (filtroPesquisa.DataGeracaoLoteInicial != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.Lote.DataGeracao >= filtroPesquisa.DataGeracaoLoteInicial);

            if (filtroPesquisa.DataGeracaoLoteFinal != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.Lote.DataGeracao <= filtroPesquisa.DataGeracaoLoteFinal.Date.AddDays(1).AddSeconds(-1));

            if (filtroPesquisa.Etapa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Todas)
                query = query.Where(o => o.SolicitacaoAvaria.Lote.Etapa == filtroPesquisa.Etapa);

            if (filtroPesquisa.DataIntegracaoLoteInicial != DateTime.MinValue || filtroPesquisa.DataIntegracaoLoteFinal != DateTime.MinValue)
            {
                var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

                if (filtroPesquisa.DataIntegracaoLoteInicial != DateTime.MinValue)
                    queryIntegracao = queryIntegracao.Where(o => o.DataIntegracao >= filtroPesquisa.DataIntegracaoLoteInicial);

                if (filtroPesquisa.DataIntegracaoLoteFinal != DateTime.MinValue)
                    queryIntegracao = queryIntegracao.Where(o => o.DataIntegracao <= filtroPesquisa.DataIntegracaoLoteFinal.Date.AddDays(1).AddSeconds(-1));

                query = query.Where(o => queryIntegracao.Any(obj => obj.Lote.Codigo == o.SolicitacaoAvaria.Lote.Codigo));
            }

            return query;
        }

        public async Task<IQueryable<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>> _ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtroPesquisa)
        {
            var queryAsync = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>().ToListAsync();
            var query = queryAsync.AsEnumerable();

            if (filtroPesquisa.NumeroAvaria > 0)
                query = query.Where(o => o.SolicitacaoAvaria.NumeroAvaria == filtroPesquisa.NumeroAvaria);

            if (filtroPesquisa.CodigoSolicitante > 0)
                query = query.Where(o => o.SolicitacaoAvaria.Solicitante.Codigo == filtroPesquisa.CodigoSolicitante);

            if (filtroPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.SolicitacaoAvaria.Transportador.Codigo == filtroPesquisa.CodigoTransportador);

            if (filtroPesquisa.SituacaoAvaria?.Count > 0)
                query = query.Where(o => filtroPesquisa.SituacaoAvaria.Contains(o.SolicitacaoAvaria.Situacao));

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.CodigoCargaEmbarcador))
                query = query.Where(o => o.SolicitacaoAvaria.Carga.CodigoCargaEmbarcador.Contains(filtroPesquisa.CodigoCargaEmbarcador));

            if (filtroPesquisa.DataSolicitacaoInicial != DateTime.MinValue && filtroPesquisa.DataSolicitacaoFinal != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.DataSolicitacao >= filtroPesquisa.DataSolicitacaoInicial && o.SolicitacaoAvaria.DataSolicitacao < filtroPesquisa.DataSolicitacaoFinal.AddDays(1));
            else if (filtroPesquisa.DataSolicitacaoInicial != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.DataSolicitacao >= filtroPesquisa.DataSolicitacaoInicial);
            else if (filtroPesquisa.DataSolicitacaoFinal != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.DataSolicitacao < filtroPesquisa.DataSolicitacaoFinal.AddDays(1));

            if (filtroPesquisa.DataGeracaoLoteInicial != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.Lote.DataGeracao >= filtroPesquisa.DataGeracaoLoteInicial);

            if (filtroPesquisa.DataGeracaoLoteFinal != DateTime.MinValue)
                query = query.Where(o => o.SolicitacaoAvaria.Lote.DataGeracao <= filtroPesquisa.DataGeracaoLoteFinal.Date.AddDays(1).AddSeconds(-1));

            if (filtroPesquisa.Etapa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Todas)
                query = query.Where(o => o.SolicitacaoAvaria.Lote.Etapa == filtroPesquisa.Etapa);

            if (filtroPesquisa.DataIntegracaoLoteInicial != DateTime.MinValue || filtroPesquisa.DataIntegracaoLoteFinal != DateTime.MinValue)
            {
                var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

                if (filtroPesquisa.DataIntegracaoLoteInicial != DateTime.MinValue)
                    queryIntegracao = queryIntegracao.Where(o => o.DataIntegracao >= filtroPesquisa.DataIntegracaoLoteInicial);

                if (filtroPesquisa.DataIntegracaoLoteFinal != DateTime.MinValue)
                    queryIntegracao = queryIntegracao.Where(o => o.DataIntegracao <= filtroPesquisa.DataIntegracaoLoteFinal.Date.AddDays(1).AddSeconds(-1));

                query = query.Where(o => queryIntegracao.Any(obj => obj.Lote.Codigo == o.SolicitacaoAvaria.Lote.Codigo));
            }

            return query.AsQueryable();
        }
        public async Task<List<Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria>> ConsultarRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = await _ConsultarRelatorioAsync(filtrosPesquisa);

            if (parametrosConsulta.InicioRegistros > 0)
                query = query.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                query = query.Take(parametrosConsulta.LimiteRegistros);

            //if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
            //query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            var list = new List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            if (query.Count() > 0)
            list = query
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Lote)
                .Fetch(obj => obj.ProdutoEmbarcador)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Solicitante)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.MotivoAvaria)
                .ToList();

            List<int> codigosLotes = list
                       .Where(x => x.SolicitacaoAvaria.Lote != null)
                       .Select(x => x.SolicitacaoAvaria.Lote.Codigo)
                       .Distinct()
                       .ToList();

            List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> integracoesRetornar = new List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();
            IQueryable<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> integracoesConsulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosLotes.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
                integracoesRetornar.AddRange(integracoesConsulta.Where(o => codigosLotes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Lote.Codigo)).ToList());

            var result = from o in list
                         select new Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria()
                         {
                             Codigo = o.Codigo,
                             Avaria = o.SolicitacaoAvaria?.NumeroAvaria ?? 0,
                             Lote = o.SolicitacaoAvaria?.Lote?.Numero ?? 0,
                             CodigoProduto = o.ProdutoEmbarcador?.CodigoProdutoEmbarcador ?? string.Empty,
                             DescricaoProduto = o.ProdutoEmbarcador?.Descricao ?? string.Empty,
                             QuantidadeCaixas = o.CaixasAvariadas,
                             QuantidadeUnidades = o.UnidadesAvariadas,
                             ValorUnitario = o.CustoPrimario,
                             ValorAvaria = o.RemovidoLote ? 0 : o.ValorAvaria,
                             ValorDescontoAvaria = o.SolicitacaoAvaria.ValorDesconto,
                             SituacaoAvaria = o.SolicitacaoAvaria?.DescricaoSituacao ?? string.Empty,
                             DataSolicitacao = o.SolicitacaoAvaria?.DataSolicitacao.ToString("dd/MM/yyyy") ?? string.Empty,
                             DataCriacao = o.SolicitacaoAvaria?.Lote?.DataGeracao.ToString("dd/MM/yyyy") ?? string.Empty,
                             DataAprovacao = o.SolicitacaoAvaria?.SolicitacaoAvariaAutorizacoes.Where(autorizacao => autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada).Max(autorizacao => autorizacao.Data)?.ToDateTimeString() ?? string.Empty,
                             Responsavel = o.SolicitacaoAvaria?.Responsaveis ?? string.Empty,
                             EtapaLote = o.SolicitacaoAvaria?.Lote?.DescricaoEtapa ?? string.Empty,
                             Filial = o.SolicitacaoAvaria?.Carga?.Filial?.Descricao ?? string.Empty,
                             Transportadora = o.SolicitacaoAvaria?.Transportador?.Descricao ?? string.Empty,
                             Criador = o.SolicitacaoAvaria?.Solicitante?.Nome ?? string.Empty,
                             DataIntegracao = integracoesRetornar.Exists(x => x.Lote.Codigo == o.SolicitacaoAvaria.Lote?.Codigo) ? integracoesRetornar.Where(b => b.Lote.Codigo == o.SolicitacaoAvaria.Lote.Codigo).Select(a => a.DataIntegracao).FirstOrDefault().ToDateString() : "",
                             DataAvaria = o.SolicitacaoAvaria?.DataAvaria.ToString("dd/MM/yyyy") ?? string.Empty,
                             CTe = o.SolicitacaoAvaria?.Carga?.NumerosCTes ?? string.Empty,
                             NotasFiscais = o.NotaFiscal,
                             Viagem = o.SolicitacaoAvaria?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                             TipoOperacao = o.SolicitacaoAvaria?.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                             Origem = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Origens ?? string.Empty,
                             Destino = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Destinos ?? string.Empty,
                             MotivoAvaria = o.SolicitacaoAvaria?.MotivoAvaria?.Descricao ?? string.Empty,
                             Motorista = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Motoristas ?? string.Empty,
                             //RGMotorista = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.RGMotoristas ?? string.Empty,  //ao refazer retornar novamente esse campo
                             Placa = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Veiculos ?? string.Empty,
                             Recebedor = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Recebedores ?? string.Empty,
                             Expedidor = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Expedidores ?? string.Empty,
                             DataViagem = o.SolicitacaoAvaria?.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy") ?? string.Empty,
                         };

            return result.ToList();
        }


        public List<Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = _ConsultarRelatorio(filtrosPesquisa);

            if (parametrosConsulta.InicioRegistros > 0)
                query = query.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                query = query.Take(parametrosConsulta.LimiteRegistros);

            //if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
            //query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            var list = query
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Lote)
                .Fetch(obj => obj.ProdutoEmbarcador)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Solicitante)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.Carga)
                .ThenFetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.SolicitacaoAvaria)
                .ThenFetch(obj => obj.MotivoAvaria)
                .ToList();

            List<int> codigosLotes = list
                       .Where(x => x.SolicitacaoAvaria.Lote != null)
                       .Select(x => x.SolicitacaoAvaria.Lote.Codigo)
                       .Distinct()
                       .ToList();

            List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> integracoesRetornar = new List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();
            IQueryable<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> integracoesConsulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosLotes.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
                integracoesRetornar.AddRange(integracoesConsulta.Where(o => codigosLotes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.Lote.Codigo)).ToList());

            var result = from o in list
                         select new Dominio.Relatorios.Embarcador.DataSource.Avarias.Avaria.ReportAvaria()
                         {
                             Codigo = o.Codigo,
                             Avaria = o.SolicitacaoAvaria?.NumeroAvaria ?? 0,
                             Lote = o.SolicitacaoAvaria?.Lote?.Numero ?? 0,
                             CodigoProduto = o.ProdutoEmbarcador?.CodigoProdutoEmbarcador ?? string.Empty,
                             DescricaoProduto = o.ProdutoEmbarcador?.Descricao ?? string.Empty,
                             QuantidadeCaixas = o.CaixasAvariadas,
                             QuantidadeUnidades = o.UnidadesAvariadas,
                             ValorUnitario = o.CustoPrimario,
                             ValorAvaria = o.RemovidoLote ? 0 : o.ValorAvaria,
                             ValorDescontoAvaria = o.SolicitacaoAvaria.ValorDesconto,
                             SituacaoAvaria = o.SolicitacaoAvaria?.DescricaoSituacao ?? string.Empty,
                             DataSolicitacao = o.SolicitacaoAvaria?.DataSolicitacao.ToString("dd/MM/yyyy") ?? string.Empty,
                             DataCriacao = o.SolicitacaoAvaria?.Lote?.DataGeracao.ToString("dd/MM/yyyy") ?? string.Empty,
                             DataAprovacao = o.SolicitacaoAvaria?.SolicitacaoAvariaAutorizacoes.Where(autorizacao => autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada).Max(autorizacao => autorizacao.Data)?.ToDateTimeString() ?? string.Empty,
                             Responsavel = o.SolicitacaoAvaria?.Responsaveis ?? string.Empty,
                             EtapaLote = o.SolicitacaoAvaria?.Lote?.DescricaoEtapa ?? string.Empty,
                             Filial = o.SolicitacaoAvaria?.Carga?.Filial?.Descricao ?? string.Empty,
                             Transportadora = o.SolicitacaoAvaria?.Transportador?.Descricao ?? string.Empty,
                             Criador = o.SolicitacaoAvaria?.Solicitante?.Nome ?? string.Empty,
                             DataIntegracao = integracoesRetornar.Exists(x => x.Lote.Codigo == o.SolicitacaoAvaria.Lote?.Codigo) ? integracoesRetornar.Where(b => b.Lote.Codigo == o.SolicitacaoAvaria.Lote.Codigo).Select(a => a.DataIntegracao).FirstOrDefault().ToDateString() : "",
                             DataAvaria = o.SolicitacaoAvaria?.DataAvaria.ToString("dd/MM/yyyy") ?? string.Empty,
                             CTe = o.SolicitacaoAvaria?.Carga?.NumerosCTes ?? string.Empty,
                             NotasFiscais = o.NotaFiscal,
                             Viagem = o.SolicitacaoAvaria?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                             TipoOperacao = o.SolicitacaoAvaria?.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                             Origem = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Origens ?? string.Empty,
                             Destino = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Destinos ?? string.Empty,
                             MotivoAvaria = o.SolicitacaoAvaria?.MotivoAvaria?.Descricao ?? string.Empty,
                             Motorista = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Motoristas ?? string.Empty,
                             //RGMotorista = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.RGMotoristas ?? string.Empty,  //ao refazer retornar novamente esse campo
                             Placa = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Veiculos ?? string.Empty,
                             Recebedor = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Recebedores ?? string.Empty,
                             Expedidor = o.SolicitacaoAvaria?.Carga?.DadosSumarizados?.Expedidores ?? string.Empty,
                             DataViagem = o.SolicitacaoAvaria?.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy") ?? string.Empty,
                             DataCarga = o.SolicitacaoAvaria?.Carga.DataCarregamentoCarga?.ToDateString() ?? string.Empty,
                         };

            return result.ToList();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaRelatorioAvarias filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            var query = _ConsultarRelatorio(filtrosPesquisa);

            return query.Count();
        }

        #endregion
    }
}