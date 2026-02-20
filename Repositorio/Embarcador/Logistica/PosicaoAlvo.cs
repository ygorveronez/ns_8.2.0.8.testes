using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PosicaoAlvo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>
    {
        public string _padraoData = "yyyy-MM-dd HH:mm:ss";

        #region Métodos públicos
        public PosicaoAlvo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> BuscarPorPosicao(long codigoPosicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();
            var result = query.Where(ent => ent.Posicao.Codigo == codigoPosicao);
            return result.Fetch(obj => obj.Cliente).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> BuscarPorPosicao(List<long> codigosPosicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();
            var result = query.Where(ent => codigosPosicao.Contains(ent.Posicao.Codigo));
            return result.Fetch(obj => obj.Cliente).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> BuscarPorPosicaoEClientes(List<long> codigosPosicao, List<double> codigosClientes)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> retorno = new();

            int take = 500;
            int start = 0;
            while (start < codigosPosicao.Count)
            {
                List<long> codigosPosicaoTmp = codigosPosicao.Skip(start).Take(take).ToList();
                List<double> codigosClientesTmp = codigosClientes.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();
                query = query.Where(ent => codigosPosicaoTmp.Contains(ent.Posicao.Codigo) && codigosClientesTmp.Contains(ent.Cliente.CPF_CNPJ));

                retorno.AddRange(query.Fetch(obj => obj.Cliente).ToList());

                start += take;
            }
            return retorno;
        }

        public List<double> BuscarCodigosClientes(long codigoPosicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo>();
            var result = query.Where(ent => ent.Posicao.Codigo == codigoPosicao).Select(obj => obj.Cliente.CPF_CNPJ);
            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> BuscarObjetoDeValorPorPosicoes(List<long> codigosPosicoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> result = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo>();

            int take = 600;
            int start = 0;
            while (start < codigosPosicoes.Count)
            {
                List<long> tmp = codigosPosicoes.Skip(start).Take(take).ToList();

                string sql = $@"
                    select
                        PosicaoAlvo.POS_CODIGO CodigoPosicao,
                        PosicaoAlvo.CLI_CGCCPF CodigoCliente
                    from
	                    T_POSICAO_ALVO PosicaoAlvo
                    where
	                    PosicaoAlvo.POS_CODIGO IN ({string.Join(",", tmp)})
                    order by
                        PosicaoAlvo.POS_CODIGO";

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);

                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo)));
                query.SetTimeout(600);

                result.AddRange(query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo>());

                start += take;
            }

            return result;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> BuscarPorTuplaClienteVeiculo(List<Tuple<int, string, DateTime, DateTime>> tuplaClienteVeiculo)
        {
            string sql = $@"
                select
                    PosicaoAlvo.CLI_CGCCPF CodigoCliente,
                    Posicao.VEI_CODIGO CodigoVeiculo,
                    Posicao.POS_CODIGO CodigoPosicao,
					Posicao.POS_DATA_VEICULO DataVeiculo,
					Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude
                from
                    T_POSICAO_ALVO PosicaoAlvo
                join
	                T_POSICAO Posicao
                        on Posicao.POS_CODIGO = PosicaoAlvo.POS_CODIGO
                where Posicao.POS_PROCESSAR = 2 and (";

            sql += string.Join(" or ", tuplaClienteVeiculo.Select(m => ObterWhereTuplaClienteVeiculo(m)).ToList());

            sql += ") order by Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo>();
        }
        private string ObterWhereTuplaClienteVeiculo(Tuple<int, string, DateTime, DateTime> tupla)
        {
            string whereIn = $"({string.Join(",", tupla.Item2.TrimStart(',').Split(',').Select(val => $"'{val}'"))})";
            return @$"( Posicao.VEI_CODIGO = {tupla.Item1} 
                        and Posicao.POS_DATA_VEICULO >= '{tupla.Item3.ToString(_padraoData)}'
                        and Posicao.POS_DATA_VEICULO <= '{tupla.Item4.ToString(_padraoData)}'
                        and PosicaoAlvo.CLI_CGCCPF in {whereIn} )";
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo> BuscarPorTuplaClienteSubareaVeiculo(List<Tuple<int, string, DateTime, DateTime>> tuplaClienteVeiculo)
        {
            string sql = $@"
                select
                    PosicaoAlvoSubarea.PAS_CODIGO CodigoClienteSubArea,
                    Posicao.VEI_CODIGO CodigoVeiculo,
                    Posicao.POS_CODIGO CodigoPosicao,
					Posicao.POS_DATA_VEICULO DataVeiculo,
					Posicao.POS_LATITUDE Latitude,
	                Posicao.POS_LONGITUDE Longitude
                from
                    T_POSICAO_ALVO_SUBAREA PosicaoAlvoSubarea
                join
	                T_POSICAO_ALVO PosicaoAlvo
                        on PosicaoAlvo.POA_CODIGO = PosicaoAlvoSubarea.POA_CODIGO
                join
	                T_POSICAO Posicao
                        on Posicao.POS_CODIGO = PosicaoAlvo.POS_CODIGO
                where Posicao.POS_PROCESSAR = 2 and (";

            sql += string.Join(" or ", tuplaClienteVeiculo.Select(m => ObterWhereTuplaClienteSubAreaVeiculo(m)).ToList());

            sql += ") order by Posicao.POS_DATA_VEICULO asc";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo)));
            query.SetTimeout(600);
            return query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlvo>();
        }
        private string ObterWhereTuplaClienteSubAreaVeiculo(Tuple<int, string, DateTime, DateTime> tupla)
        {
            string whereIn = $"({string.Join(",", tupla.Item2.TrimStart(',').Split(',').Select(val => $"'{val}'"))})";
            return @$"( Posicao.VEI_CODIGO = {tupla.Item1}
                        and Posicao.POS_DATA_VEICULO >= '{tupla.Item3.ToString(_padraoData)}'
                        and Posicao.POS_DATA_VEICULO <= '{tupla.Item4.ToString(_padraoData)}'
                        and PosicaoAlvoSubarea.SAC_CODIGO in {whereIn} )";
        }

        #endregion

    }

}
