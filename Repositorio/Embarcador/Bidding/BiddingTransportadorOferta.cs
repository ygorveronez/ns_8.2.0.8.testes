using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingTransportadorOferta : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>
    {
        public BiddingTransportadorOferta(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public BiddingTransportadorOferta(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> BuscarPorTransportadorOferta(int codigoTransportador, int codigoOferta, int codigoRota, int situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => o.TransportadorRota.Rota.BiddingOferta.Codigo == codigoOferta && o.TransportadorRota.Transportador.Codigo == codigoTransportador);

            if (codigoRota > 0)
                query = query.Where(o => o.TransportadorRota.Rota.Codigo == codigoRota);

            if (situacao > 0)
            {
                if ((StatusBiddingRota)situacao == StatusBiddingRota.Aprovada)
                    query = query.Where(o => o.Aceito == true);
                else if ((StatusBiddingRota)situacao == StatusBiddingRota.Rejeitada)
                    query = query.Where(o => o.Aceito == false);
            }

            return query
                .Fetch(o => o.ModeloVeicular)
                .Fetch(o => o.TransportadorRota)
                .ThenFetch(o => o.Rota)
                .OrderByDescending(o => o.TransportadorRota.Rodada)
                    .ThenBy(o => o.TransportadorRota.Rota.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> BuscarPorCodigoTransportadorRota(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => o.TransportadorRota.Codigo == codigo);

            return query
                .Fetch(o => o.ModeloVeicular)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> BuscarPorCodigosTransportadorRota(List<int> codigos)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            const int quantidadeParametrosWhere = 1000;
            var resultadoFinal = new List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            for (int i = 0; i < codigos.Count; i += quantidadeParametrosWhere)
            {
                var paginaCodigos = codigos.Skip(i).Take(quantidadeParametrosWhere).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>()
                    .Where(o => paginaCodigos.Contains(o.TransportadorRota.Codigo));

                resultadoFinal.AddRange(query.ToList());
            }

            return resultadoFinal;
        } 
        
        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> BuscarPorCodigosBiddingTransportadorRota(List<int> codigos)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            const int quantidadeParametrosWhere = 2000;
            var resultadoFinal = new List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            for (int i = 0; i < codigos.Count; i += quantidadeParametrosWhere)
            {
                var paginaCodigos = codigos.Skip(i).Take(quantidadeParametrosWhere).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>()
                    .Where(o => paginaCodigos.Contains(o.TransportadorRota.Rota.Codigo));

                resultadoFinal.AddRange(query.Fetch(x=>x.TransportadorRota).ToList());
            }

            return resultadoFinal;
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaOfertaDados>> BuscarPorCodigosTransportadorRotaOfertaAsync(List<int> codigos)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaOfertaDados>();

            const int quantidadeParametrosWhere = 1000;
            var resultadoFinal = new List<Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaOfertaDados>();

            for (int i = 0; i < codigos.Count; i += quantidadeParametrosWhere)
            {
                var paginaCodigos = codigos.Skip(i).Take(quantidadeParametrosWhere);

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>()
                    .Where(o => paginaCodigos.Contains(o.TransportadorRota.Codigo))
                    .Select(o => new Dominio.ObjetosDeValor.Embarcador.Bidding.BiddingTransportadorRotaOfertaDados
                    {
                        CodigoTransportadorRota = o.TransportadorRota.Codigo,
                        CustoEstimado = o.CustoEstimado,
                        NaoOfertar = o.NaoOfertar,
                        TipoLance = o.TipoOferta,
                        FreteTonelada = o.FreteTonelada,
                        AdicionalPorEntrega = o.AdicionalPorEntrega,
                        Ajudante = o.Ajudante,
                        PedagioParaEixo = o.PedagioParaEixo,
                        VeiculosVerdes = o.VeiculosVerdes
                    });

                resultadoFinal.AddRange(await query.ToListAsync(CancellationToken)); 
            }

            return resultadoFinal;
        }


        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> BuscarVencedores(int codigo, int codigoRota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => o.TransportadorRota.Rota.BiddingOferta.BiddingConvite.Codigo == codigo && o.Aceito == true);

            if (codigoRota > 0)
                query = query.Where(o => o.TransportadorRota.Rota.Codigo == codigoRota);

            return query
                .Fetch(o => o.ModeloVeicular)
                .Fetch(o => o.TransportadorRota)
                .ThenFetch(o => o.Rota)
                .Fetch(o => o.TransportadorRota)
                .ThenFetch(o => o.Transportador)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> BuscarPorRota(int rota, int transportador = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => o.TransportadorRota.Rota.Codigo == rota && o.Status != StatusBiddingRota.NovaRodada);

            if (transportador > 0)
                query = query.Where(o => o.TransportadorRota.Transportador.Codigo == transportador);

            return query
                .Fetch(obj => obj.ModeloVeicular)
                .Fetch(obj => obj.TransportadorRota)
                .OrderByDescending(obj => obj.CustoEstimado)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>> BuscarPorRotaAsync(int rota, int transportador = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => o.TransportadorRota.Rota.Codigo == rota && o.Status != StatusBiddingRota.NovaRodada);

            if (transportador > 0)
                query = query.Where(o => o.TransportadorRota.Transportador.Codigo == transportador);

            return query
                .Fetch(obj => obj.ModeloVeicular)
                .Fetch(obj => obj.TransportadorRota)
                .OrderByDescending(obj => obj.CustoEstimado)
                .ToListAsync(CancellationToken);
        }

        public IList<(int codigoTransportador, int codigoRota, decimal custoTotal, bool naoOfertar)> BuscarValoresPorCodigosTransportadorRota(IList<int> codigos)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<(int, int, decimal, bool)>();

            const int quantidadeParametrosWhere = 1000;
            var resultadoFinal = new List<(int codigoTransportador, int codigoRota, decimal custoTotal, bool naoOfertar)>();

            for (int i = 0; i < codigos.Count; i += quantidadeParametrosWhere)
            {
                var paginaCodigos = codigos.Skip(i).Take(quantidadeParametrosWhere).ToList();

                string sql = $@"
                                SELECT
                                    biddingTransportadorRota.TTR_TRANSPORTADOR_CODIGO AS codigoTransportador,
                                    biddingTransportadorRota.TTR_ROTA_CODIGO AS codigoRota,
                                    COALESCE(MIN(biddingTransportadorRotaOferta.TRO_CUSTO_ESTIMADO), 0) AS custoTotal,
                                    CONVERT(BIT, COALESCE(biddingTransportadorRotaOferta.TRO_NAO_OFERTAR, 0)) AS naoOfertar
                                FROM
                                    T_BIDDING_TRANSPORTADOR_ROTA_OFERTA biddingTransportadorRotaOferta
                                LEFT JOIN
                                    T_BIDDING_TRANSPORTADOR_ROTA biddingTransportadorRota
                                    ON biddingTransportadorRotaOferta.TRO_TRANSPORTADOR_ROTA_CODIGO = biddingTransportadorRota.TTR_CODIGO
                                WHERE
                                    biddingTransportadorRotaOferta.TRO_TRANSPORTADOR_ROTA_CODIGO IN (:codigos)
                                GROUP BY
                                    biddingTransportadorRota.TTR_TRANSPORTADOR_CODIGO,
                                    biddingTransportadorRota.TTR_ROTA_CODIGO,
                                    COALESCE(biddingTransportadorRotaOferta.TRO_NAO_OFERTAR, 0);";

                var result = this.SessionNHiBernate
                    .CreateSQLQuery(sql)
                    .SetParameterList("codigos", paginaCodigos)
                    .SetResultTransformer(new NHibernate.Transform.AliasToBeanConstructorResultTransformer(
                        typeof((int codigoTransportador, int codigoRota, decimal custoTotal, bool naoOfertar)).GetConstructors().FirstOrDefault()))
                    .List<(int codigoTransportador, int codigoRota, decimal custoTotal, bool naoOfertar)>()
                    .Select(o => ValueTuple.Create(
                            o.codigoTransportador,
                            o.codigoRota,
                            o.custoTotal,
                            o.naoOfertar
                        ));

                resultadoFinal.AddRange(result);
            }

            return resultadoFinal;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Bidding.ListaBiddingOfertaAceitamento> BuscarValoresComponentesPorCodigosTransportadorRota(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => codigos.Contains(o.TransportadorRota.Codigo));

            return query.GroupBy(o => new
            {
                CodigoTransportador = o.TransportadorRota.Transportador.Codigo,
                CodigoRota = o.TransportadorRota.Rota.Codigo,
                NaoOfertar = o.NaoOfertar ?? false
            }).Select(o => new Dominio.ObjetosDeValor.Embarcador.Bidding.ListaBiddingOfertaAceitamento()
            {
                CodigoTransportador = o.Key.CodigoTransportador,
                CodigoRota = o.Key.CodigoRota,
                TipoOferta = o.Min(x => x.TipoOferta),
                CustoEstimado = o.Min(x => x.CustoEstimado),
                NaoOfertar = o.Key.NaoOfertar,
                Quilometragem = o.Min(x => x.Quilometragem),
                ValorFixo = o.Min(x => x.ValorFixo),
                ValorFranquia = o.Min(x => x.ValorFranquia),
                ValorFixoEquipamento = o.Min(x => x.ValorFixoEquipamento),
                ValorFixoMensal = o.Min(x => x.ValorFixoMensal),
                ValorKmRodado = o.Min(x => x.ValorKmRodado),
                Porcentagem = o.Min(x => x.Porcentagem),
                ValorViagem = o.Min(x => x.ValorViagem),
                ValorEntrega = o.Min(x => x.ValorEntrega),
                FreteTonelada = o.Min(x => x.FreteTonelada),
                PedagioParaEixo = o.Min(x => x.PedagioParaEixo),
                ICMSPorcentagem = o.Min(x => x.ICMSPorcentagem),
                AdicionalPorEntrega = o.Min(x => x.AdicionalPorEntrega),
                Ajudante = o.Min(x => x.Ajudante)
            }).ToList();
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta BuscarVencedoresRota(int codigoBidding, int codigoTransportador, int codigoRota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => o.TransportadorRota.Rota.BiddingOferta.BiddingConvite.Codigo == codigoBidding && o.TransportadorRota.Transportador.Codigo == codigoTransportador
             && o.TransportadorRota.Rota.Codigo == codigoRota);

            return query.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query
                .ToList();
        }

        public bool BuscarExisteRotasOfertadasPorCodigosTransportadorRota(List<int> codigos, int codigoTransportador = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingTransportadorRotaOferta>();

            query = query.Where(o => codigos.Contains(o.TransportadorRota.Codigo));

            if (codigoTransportador > 0)
                query = query.Where(o => o.TransportadorRota.Transportador.Codigo == codigoTransportador);

            return query.Any();
        }
    }
}
