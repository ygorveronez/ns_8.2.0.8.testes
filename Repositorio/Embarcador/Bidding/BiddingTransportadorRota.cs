using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingTransportadorRota : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>
    {
        public BiddingTransportadorRota(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public BiddingTransportadorRota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Publicos

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BuscarPorOferta(Dominio.Entidades.Embarcador.Bidding.BiddingOferta oferta, int codigoTransportador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta == oferta && o.Transportador.Codigo == codigoTransportador && o.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBiddingRota.NovaRodada);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador)
                .ToList();

        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BuscarPorBiddingSomenteOfertadas(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta.BiddingConvite == biddingConvite && o.Status != StatusBiddingRota.NovaRodada && o.Status != StatusBiddingRota.Aguardando && o.Status != StatusBiddingRota.Rejeitada);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador)
                .ToList();

        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaDados>> BuscarTransportadorRotaPorBiddingSomenteOfertadasAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta.BiddingConvite == biddingConvite &&
                o.Status != StatusBiddingRota.NovaRodada &&
                o.Status != StatusBiddingRota.Aguardando &&
                o.Status != StatusBiddingRota.Rejeitada &&
                !o.TransportadorRejeitado)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaDados
                {
                    Codigo = o.Codigo,
                    Transportador = o.Transportador.NomeFantasia,
                    TransportadorCodigo = o.Transportador.Codigo,
                    RotaCodigo = o.Rota.Codigo,
                    RotaDescricao = o.Rota.Descricao,
                    Status = o.Status,
                    Ranking = o.Ranking,
                    Rodada = o.Rodada,
                    Target = o.Target,
                    DataRetorno = o.DataRetorno,
                    Origem = o.Rota.DescricaoOrigem,
                    Destino = o.Rota.DescricaoDestino
                })
                .ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BuscarPorBidding(Dominio.Entidades.Embarcador.Bidding.BiddingOferta biddingOferta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta == biddingOferta);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador)
                .ToList();

        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BuscarPorBiddingRejeitados(Dominio.Entidades.Embarcador.Bidding.BiddingOferta biddingOferta, List<int> codigosVencedores)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => !codigosVencedores.Contains(o.Codigo) && o.Rota.BiddingOferta == biddingOferta && o.Status != StatusBiddingRota.NovaRodada);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador)
                .ToList();

        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BuscarPorBiddingStatus(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, List<StatusBiddingRota> status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta.BiddingConvite == biddingConvite && status.Contains(o.Status));

            return query
                .ToList();
        }

        public int ContarPorBiddingRotas(int biddingConvite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta.BiddingConvite.Codigo == biddingConvite);

            return query.Count();
        }

        public async Task<bool> ExisteRotaVinculoTransportadorAsync(List<int> codigosRotas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => codigosRotas.Contains(o.Rota.Codigo));

            return await query.AnyAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota BuscarPorRotaETransportador(int codigoRota, int codigoTransportador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.Codigo == codigoRota && o.Status != StatusBiddingRota.NovaRodada && o.Status != StatusBiddingRota.Rejeitada);

            if (codigoTransportador > 0)
                query = query.Where(o => o.Transportador.Codigo == codigoTransportador);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota BuscarPorRota(int codigoRota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.Codigo == codigoRota);

            return query.FirstOrDefault();
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingRankingTransportadorRota>> BuscarPorRotasETransportadoresAsync(List<int> codigosRota, List<int?> codigosTransportador)
        {
            const int loteMaximo = 1000;
            var resultados = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingRankingTransportadorRota>();

            var lotesRotas = PaginarLista(codigosRota, loteMaximo);
            var lotesTransportadores = PaginarLista(codigosTransportador.Where(x => x.HasValue).Select(x => x.Value).ToList(), loteMaximo);

            var tasks = new List<Task<List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingRankingTransportadorRota>>>();

            foreach (var loteRota in lotesRotas)
            {
                foreach (var loteTransportador in lotesTransportadores)
                {
                    tasks.Add(BuscarLoteAsync(loteRota, loteTransportador));
                }
            }

            var resultadosLotes = await Task.WhenAll(tasks);

            foreach (var resultado in resultadosLotes)
            {
                resultados.AddRange(resultado);
            }

            return resultados;
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingRankingTransportadorRota>> BuscarLoteAsync(List<int> codigosRota, List<int> codigosTransportador)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => codigosRota.Contains(o.Rota.Codigo)
                         && codigosTransportador.Contains(o.Transportador.Codigo)
                         && o.Status != StatusBiddingRota.NovaRodada
                         && o.Status != StatusBiddingRota.Rejeitada)
                .Select(o => new Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingRankingTransportadorRota
                {
                    CodigoRota = o.Rota.Codigo,
                    CodigoTransportador = o.Transportador.Codigo,
                    RotaDescricao = o.Rota.Descricao,
                    Origem = o.Rota.DescricaoOrigem,
                    Destino = o.Rota.DescricaoDestino
                })
                .ToListAsync();
        }

        private List<List<T>> PaginarLista<T>(List<T> lista, int tamanhoMaximo)
        {
            var resultado = new List<List<T>>();
            for (int i = 0; i < lista.Count; i += tamanhoMaximo)
            {
                resultado.Add(lista.Skip(i).Take(tamanhoMaximo).ToList());
            }
            return resultado;
        }


        public Task<List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>> BuscarPorRotasAsync(List<int> codigosRota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => codigosRota.Contains(o.Rota.Codigo) && o.Status != StatusBiddingRota.Aguardando && !o.TransportadorRejeitado)
                .OrderByDescending(o => o.Codigo);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BuscarPorBiddingRotas(int biddingConvite, int codigoEmpresa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta.BiddingConvite.Codigo == biddingConvite && o.Status != StatusBiddingRota.NovaRodada && !o.TransportadorRejeitado);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Transportador.Codigo == codigoEmpresa);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador)
                .Fetch(x => x.Rota)
                .ThenFetch(x => x.Origens)
                .Fetch(x => x.Rota)
                .ThenFetch(x => x.Destinos)
                .Fetch(x => x.Rota)
                .ThenFetch(x => x.ModeloCarroceria)
                .OrderBy(o => o.Ranking).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> BuscarPorBiddingConviteETransportador(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite, int codigoTransportador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                .Where(o => o.Rota.BiddingOferta.BiddingConvite == biddingConvite);

            if (codigoTransportador > 0)
                query = query.Where(o => o.Transportador.Codigo == codigoTransportador);

            return query
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.TiposCarga)
                .Fetch(o => o.Rota)
                .ThenFetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Transportador)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota BuscarPorProtocoloImportacao(int protocoloImportacao, Dominio.Entidades.Empresa transportador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                          .Where(obj => obj.Rota.ProtocoloImportacao == protocoloImportacao && obj.Transportador == transportador)
                          .OrderByDescending(obj => obj.Codigo);

            return query.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>> BuscarPorProtocolosImportacaoAsync(List<int> protocolosImportacao, Dominio.Entidades.Empresa transportador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                          .Where(obj => protocolosImportacao.Contains(obj.Rota.ProtocoloImportacao) && obj.Transportador == transportador)
                          .OrderByDescending(obj => obj.Codigo);

            return query
                .Fetch(x => x.Rota)
                    .ThenFetch(x => x.ModelosVeiculares)
                .ToListAsync();
        }

        #region BuscasFiltroPesquisa

        public Task<List<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>> BuscarPorBiddingRotaOfertasAsync(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas filtroPesquisaBiddingOfertas, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, bool somenteOfertadas = false, bool excetoNovaRodada = false)
        {
            var consulta = ConsultarBiddingRotaOfertas(filtroPesquisaBiddingOfertas, somenteOfertadas, excetoNovaRodada);

            if (parametroConsulta.InicioRegistros > 0)
                consulta = consulta.Skip(parametroConsulta.InicioRegistros);

            if (parametroConsulta.LimiteRegistros > 0)
                consulta = consulta.Take(parametroConsulta.LimiteRegistros);

            return consulta
                .Fetch(o => o.ModelosVeiculares)
                .Fetch(o => o.Origens)
                .Fetch(o => o.Destinos)
                .Fetch(o => o.TiposCarga)
                .ToListAsync();
        }

        public Task<int> ContarPorBiddingRotaOfertasAsync(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas filtroPesquisaBiddingOfertas, bool somenteOfertadas = false)
        {
            return ConsultarBiddingRotaOfertas(filtroPesquisaBiddingOfertas, somenteOfertadas, false).CountAsync(CancellationToken);
        }

        #endregion

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> ConsultarBiddingRotaOfertas(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingOfertas filtroPesquisaBiddingOfertas, bool somenteOfertadas = false, bool excetoNovaRodada = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>()
                                                                            .Where(o => o.BiddingOferta.BiddingConvite.Codigo == filtroPesquisaBiddingOfertas.Codigo);

            IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota> subQueryTranportadoresRota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRota>()
                                                                                        .Where(o => o.Rota.BiddingOferta.BiddingConvite.Codigo == filtroPesquisaBiddingOfertas.Codigo);
            if (somenteOfertadas)
                subQueryTranportadoresRota = subQueryTranportadoresRota.Where(o => o.Status != StatusBiddingRota.NovaRodada && o.Status != StatusBiddingRota.Rejeitada);

            if (excetoNovaRodada)
                subQueryTranportadoresRota = subQueryTranportadoresRota.Where(o => o.Status != StatusBiddingRota.NovaRodada);

            if (filtroPesquisaBiddingOfertas.CodigoRota > 0)
                query = query.Where(o => o.Codigo == filtroPesquisaBiddingOfertas.CodigoRota);

            if (filtroPesquisaBiddingOfertas.CodigoModeloVeicular > 0)
                query = query.Where(o => o.ModelosVeiculares.Any(obj => obj.Codigo == filtroPesquisaBiddingOfertas.CodigoModeloVeicular));

            if (filtroPesquisaBiddingOfertas.CodigoFilialParticipante > 0)
                query = query.Where(o => o.FiliaisParticipante.Any(f => f.Codigo == filtroPesquisaBiddingOfertas.CodigoFilialParticipante));

            if (filtroPesquisaBiddingOfertas.CodigoOrigem > 0) // Na verdade isso representa a cidade origem
                query = query.Where(o => o.Origens.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoOrigem));

            if (filtroPesquisaBiddingOfertas.CodigoDestino > 0) // Na verdade isso representa a cidade destino
                query = query.Where(o => o.Destinos.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoDestino));

            if (filtroPesquisaBiddingOfertas.CodigoMesorregiaoDestino > 0)
                query = query.Where(o => o.RegioesDestino.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoMesorregiaoDestino));

            if (filtroPesquisaBiddingOfertas.CodigoMesorregiaoOrigem > 0)
                query = query.Where(o => o.RegioesOrigem.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoMesorregiaoOrigem));

            if (filtroPesquisaBiddingOfertas.CodigoClienteDestino > 0)
                query = query.Where(o => o.ClientesDestino.Any(x => x.CPF_CNPJ == filtroPesquisaBiddingOfertas.CodigoClienteDestino));

            if (filtroPesquisaBiddingOfertas.CodigoClienteOrigem > 0)
                query = query.Where(o => o.ClientesOrigem.Any(x => x.CPF_CNPJ == filtroPesquisaBiddingOfertas.CodigoClienteOrigem));

            if (filtroPesquisaBiddingOfertas.CodigoRotaDestino > 0)
                query = query.Where(o => o.RotasDestino.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoRotaDestino));

            if (filtroPesquisaBiddingOfertas.CodigoRotaOrigem > 0)
                query = query.Where(o => o.RotasOrigem.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoRotaOrigem));

            if (filtroPesquisaBiddingOfertas.CodigoEstadoDestino > 0)
                query = query.Where(o => o.EstadosDestino.Any(x => x.CodigoIBGE == filtroPesquisaBiddingOfertas.CodigoEstadoDestino));

            if (filtroPesquisaBiddingOfertas.CodigoEstadoOrigem > 0)
                query = query.Where(o => o.EstadosOrigem.Any(x => x.CodigoIBGE == filtroPesquisaBiddingOfertas.CodigoEstadoOrigem));

            if (filtroPesquisaBiddingOfertas.CodigoPaisDestino > 0)
                query = query.Where(o => o.PaisesDestino.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoPaisDestino));

            if (filtroPesquisaBiddingOfertas.CodigoPaisOrigem > 0)
                query = query.Where(o => o.PaisesOrigem.Any(x => x.Codigo == filtroPesquisaBiddingOfertas.CodigoPaisOrigem));

            if (filtroPesquisaBiddingOfertas.CEPDestino > 0)
                query = query.Where(o => o.CEPsDestino.Any(x => x.CEPInicial <= filtroPesquisaBiddingOfertas.CEPDestino && x.CEPFinal >= filtroPesquisaBiddingOfertas.CEPDestino));

            if (filtroPesquisaBiddingOfertas.CEPOrigem > 0)
                query = query.Where(o => o.CEPsOrigem.Any(x => x.CEPInicial <= filtroPesquisaBiddingOfertas.CEPOrigem && x.CEPFinal >= filtroPesquisaBiddingOfertas.CEPOrigem));

            if (filtroPesquisaBiddingOfertas.CodigoRegiaoDestino > 0)
            {
                IQueryable<int> codigosTranportadoresRotaCidade = query.Where(o => o.Destinos.Any(x => x.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino)).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaEstado = query.Where(o => o.EstadosDestino.Any(x => x.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino)).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaMesoregiao = query.Where(o => o.RegioesDestino.Any(x => x.Localidades.Any(l => l.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino))).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaCliente = query.Where(o => o.ClientesDestino.Any(x => x.Localidade.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino)).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaRota = query.Where(o => o.RotasDestino.Any(x => x.LocalidadesOrigem.Any(l => l.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino))).Select(x => x.Codigo);

                query = query.Where(o => codigosTranportadoresRotaCidade.Contains(o.Codigo) || codigosTranportadoresRotaEstado.Contains(o.Codigo) || codigosTranportadoresRotaMesoregiao.Contains(o.Codigo) || codigosTranportadoresRotaCliente.Contains(o.Codigo) || codigosTranportadoresRotaRota.Contains(o.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem > 0)
            {
                IQueryable<int> codigosTranportadoresRotaCidade = query.Where(o => o.Origens.Any(x => x.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoOrigem)).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaEstado = query.Where(o => o.EstadosOrigem.Any(x => x.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino)).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaMesoregiao = query.Where(o => o.RegioesOrigem.Any(x => x.Localidades.Any(l => l.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino))).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaCliente = query.Where(o => o.ClientesOrigem.Any(x => x.Localidade.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino)).Select(x => x.Codigo);

                IQueryable<int> codigosTranportadoresRotaRota = query.Where(o => o.RotasOrigem.Any(x => x.LocalidadesOrigem.Any(l => l.Estado.RegiaoBrasil.Codigo == filtroPesquisaBiddingOfertas.CodigoRegiaoDestino))).Select(x => x.Codigo);

                query = query.Where(o => codigosTranportadoresRotaCidade.Contains(o.Codigo) || codigosTranportadoresRotaEstado.Contains(o.Codigo) || codigosTranportadoresRotaMesoregiao.Contains(o.Codigo) || codigosTranportadoresRotaCliente.Contains(o.Codigo) || codigosTranportadoresRotaRota.Contains(o.Codigo));
            }

            if (filtroPesquisaBiddingOfertas.QuantidadeEntregas > 0)
                query = query.Where(o => o.NumeroEntrega == filtroPesquisaBiddingOfertas.QuantidadeEntregas);

            if (filtroPesquisaBiddingOfertas.QuantidadeAjudantes > 0)
                query = query.Where(o => o.QuantidadeAjudantePorVeiculo == filtroPesquisaBiddingOfertas.QuantidadeAjudantes);

            if (filtroPesquisaBiddingOfertas.QuantidadeViagensAno > 0)
                query = query.Where(o => o.QuantidadeViagensPorAno == filtroPesquisaBiddingOfertas.QuantidadeViagensAno);

            IQueryable<int> result = subQueryTranportadoresRota.Select(obj => obj.Rota.Codigo);

            query = query.Where(o => result.Contains(o.Codigo));

            return query;
        }

        #endregion

    }

}

