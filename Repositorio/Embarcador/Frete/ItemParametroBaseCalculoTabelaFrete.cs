using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frete
{
    public class ItemParametroBaseCalculoTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>
    {
        #region Construtores

        public ItemParametroBaseCalculoTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public TipoParametroBaseTabelaFrete? BuscarTipoParametroBaseComValorInformadoPorParametrosTabelaFreteDaCarga(int codigoCarga)
        {
            string sql = $@"
                select top(1) ItemParametroBase.TPI_TIPO_OBJETO Tipo
                  from T_CARGA_TABELA_FRETE_CLIENTE TabelaCarga
                  join T_TABELA_FRETE_CLIENTE TabelaFreteCliente on TabelaFreteCliente.TFC_CODIGO = TabelaCarga.TFC_CODIGO
                  join T_TABELA_FRETE TabelaFrete on TabelaFrete.TBF_CODIGO = TabelaFreteCliente.TBF_CODIGO
                  join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO ParametroBase on ParametroBase.TFC_CODIGO = TabelaFreteCliente.TFC_CODIGO
                  join T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ItemParametroBase on ItemParametroBase.TBC_CODIGO = ParametroBase.TBC_CODIGO
                 where TabelaCarga.CAR_CODIGO = {codigoCarga}
                   and TabelaFrete.TBF_PARAMETRO_BASE = {(int)TipoParametroBaseTabelaFrete.ModeloReboque}
                   and ItemParametroBase.TPI_VALOR > 0";

            var consultaTipoParametroBaseTabelaFrete = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaTipoParametroBaseTabelaFrete.AddScalar("Tipo", NHibernate.NHibernateUtil.Enum(typeof(TipoParametroBaseTabelaFrete)));

            return consultaTipoParametroBaseTabelaFrete.UniqueResult<TipoParametroBaseTabelaFrete?>();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarComValorInformadoPorParametrosTabelaFrete(int codigoTabelaFrete)
        {
            var consultaParametroBase = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>()
                .Where(parametroBase => parametroBase.TabelaFrete.Codigo == codigoTabelaFrete);

            var consultaValores = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                .Where(valor => consultaParametroBase.Any(parametro => parametro.Codigo == valor.ParametroBaseCalculo.Codigo) && valor.Valor > 0m);

            return consultaValores.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarPorCodigos(List<int> codigos)
        {
            int take = 1000;
            int start = 0;

            List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> retorno = new List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();
            while (start < codigos?.Count)
            {
                List<int> tmp2 = codigos.Skip(start).Take(take).ToList();

                var consultaItem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                                    .Where(o => tmp2.Contains(o.Codigo));

                consultaItem = consultaItem.Fetch(o => o.ParametroBaseCalculo).ThenFetch(o => o.TabelaFrete);
                retorno.AddRange(consultaItem.ToList());
                start += take;
            }

            return retorno;
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarPorParametro(int codigoParametro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.ParametroBaseCalculo.Codigo == codigoParametro select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete Buscar(int codigoTabelaFrete, int codigoItemBase, int codigoItem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoObjeto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.CodigoObjeto == codigoItem && obj.TipoObjeto == tipoObjeto select obj;

            if (codigoItemBase > 0)
                result = result.Where(o => o.ParametroBaseCalculo.CodigoObjeto == codigoItemBase && o.ParametroBaseCalculo.TabelaFrete.Codigo == codigoTabelaFrete);
            else
                result = result.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarPorCodigosItens(List<int> codigosItens, int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where codigosItens.Contains(obj.CodigoObjeto) && obj.ParametroBaseCalculo.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.Fetch(o => o.ParametroBaseCalculo).ThenFetch(o => o.TabelaFrete).
                          Fetch(o => o.TabelaFrete).
                          ToList();
        }

        public List<int> BuscarPorCodigosItensPorTabelaFrete(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                .Where(obj => obj.ParametroBaseCalculo.TabelaFrete.Codigo == codigoTabelaFrete);

            return query
                .Select(obj => obj.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarItensComParametroPorCodigos(List<int> codigosObjeto, int codigoTabelaFrete)
        {
            return SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                .Where(i => codigosObjeto.Contains(i.Codigo) && i.ParametroBaseCalculo.TabelaFrete.Codigo == codigoTabelaFrete)
                .Fetch(i => i.ParametroBaseCalculo).ThenFetch(p => p.TabelaFrete)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarPorCodigosItensPorTabelaFrete(List<int> codigosItens, int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where codigosItens.Contains(obj.CodigoObjeto) && obj.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.Fetch(o => o.ParametroBaseCalculo).ThenFetch(o => o.TabelaFrete).
                          Fetch(o => o.TabelaFrete).
                          ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarPorCodigosItensComParametroBaseCalculo(List<int> codigosItens, int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where codigosItens.Contains(obj.CodigoObjeto) && obj.ParametroBaseCalculo.TabelaFrete.Codigo == codigoTabelaFrete select obj;

            return result.Fetch(o => o.ParametroBaseCalculo).ThenFetch(o => o.TabelaFrete).
                          Fetch(o => o.TabelaFrete).
                          ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete BuscarPorCodigoObjetoETipoItem(int codigoTabelaFrete, int codigoParametro, int codigoObjetoItem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoObjetoItem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.CodigoObjeto == codigoObjetoItem && obj.TipoObjeto == tipoObjetoItem select obj;

            if (codigoParametro > 0)
                result = result.Where(o => o.ParametroBaseCalculo.Codigo == codigoParametro);
            else
                result = result.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> Buscar(int codigoParametro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.ParametroBaseCalculo.Codigo == codigoParametro select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarDiff(int codigoTabelaFrete, int[] codigosItens, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete tipoParametroBaseTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && !codigosItens.Contains(obj.Codigo) && obj.TipoObjeto == tipoParametroBaseTabelaFrete select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> BuscarPorCodigoObjeto(List<int> codigosObjetos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                .Where(obj => codigosObjetos.Contains(obj.CodigoObjeto));

            return query.ToList();
        }

        public void ExecutarInsercaoDeItens(List<string> listaValoresSql, bool comParametro = false)
        {
            for (int i = 0; i < listaValoresSql.Count; i += 1000)
            {
                string sqlParaExecutar = "";

                if (!comParametro)
                    sqlParaExecutar = "INSERT INTO T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM (TFC_CODIGO, TPI_TIPO_OBJETO, TPI_TIPO_VALOR, TPI_VALOR, TPI_CODIGO_OBJETO) VALUES ";
                else
                    sqlParaExecutar = "INSERT INTO T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM (TBC_CODIGO, TPI_TIPO_OBJETO, TPI_TIPO_VALOR, TPI_VALOR, TPI_CODIGO_OBJETO) VALUES ";

                List<string> valoresInserir = listaValoresSql.Skip(i).Take(1000).ToList();
                sqlParaExecutar += string.Join(", ", valoresInserir);

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlParaExecutar);

                query.ExecuteUpdate();
            }
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Frete.CodigoModeloVeicularCargaItem> BuscarCodigosModeloVeicularItemPorTabelasFreteCliente(int codigoComponentePedagio, List<int> codigosTabelaFreteCliente)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Frete.CodigoModeloVeicularCargaItem> codigosModeloVeicularCargaItem = new List<Dominio.ObjetosDeValor.Embarcador.Frete.CodigoModeloVeicularCargaItem>();
            if (codigoComponentePedagio > 0 && codigosTabelaFreteCliente != null && codigosTabelaFreteCliente.Count > 0)
            {
                string sql = $@"
                    SELECT
                        distinct 
                        PModeloReboque.MVC_CODIGO as CodigoModeloVeicularCarga,
                        ValorComponente1.TPI_CODIGO as CodigoItemParametroBaseCalculoTabelaFrete
                    FROM 
                        T_TABELA_FRETE_PARAMETRO_BASE_CALCULO Parametro 
                    JOIN
                        T_TABELA_FRETE_MODELO_REBOQUE PModeloReboque 
                            on Parametro.TBC_CODIGO_OBJETO = PModeloReboque.MVC_CODIGO
                    JOIN
                        T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM ValorComponente1 
                            on ValorComponente1.TBC_CODIGO = Parametro.TBC_CODIGO 
                            and ValorComponente1.TPI_TIPO_OBJETO = 4 
                            and ValorComponente1.TPI_CODIGO_OBJETO = {codigoComponentePedagio}
                    WHERE 
                        Parametro.TFC_CODIGO in ({string.Join(",", codigosTabelaFreteCliente)})";

                var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
                consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frete.CodigoModeloVeicularCargaItem)));
                codigosModeloVeicularCargaItem = consulta.List<Dominio.ObjetosDeValor.Embarcador.Frete.CodigoModeloVeicularCargaItem>();
            }
            return codigosModeloVeicularCargaItem;
        }

        public bool TodosItensIntegradosPorTabelaFreteCliente(int codigoTabelaFrete)
        {
            List<SituacaoItemParametroBaseCalculoTabelaFrete> situacoesItemIntegrado = SituacaoItemParametroBaseCalculoTabelaFreteHelper.ObterSituacoesIntegrado();

            var consultaParametroBase = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>()
                .Where(parametroBase => parametroBase.TabelaFrete.Codigo == codigoTabelaFrete);

            var consultaValores = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                .Where(valor =>
                    consultaParametroBase.Any(parametro => parametro.Codigo == valor.ParametroBaseCalculo.Codigo) &&
                    situacoesItemIntegrado.Contains(((SituacaoItemParametroBaseCalculoTabelaFrete?)valor.Situacao ?? SituacaoItemParametroBaseCalculoTabelaFrete.Ativo))
                );

            return consultaValores.Count() == 0;
        }

        public bool ExistePendenteAprovacaoPorParametrosTabelaFrete(int codigoTabelaFrete)
        {
            var consultaParametroBase = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete>()
                .Where(parametroBase => parametroBase.TabelaFrete.Codigo == codigoTabelaFrete);

            var consultaValores = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                .Where(valor =>
                    consultaParametroBase.Any(parametro => parametro.Codigo == valor.ParametroBaseCalculo.Codigo) &&
                    valor.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.Aprovacao
                );

            return consultaValores.Count() > 0;
        }

        public bool ExistePendenteAprovacaoPorTabelaFrete(int codigoTabelaFrete)
        {
            var consultaValores = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete>()
                .Where(valor =>
                    valor.TabelaFrete.Codigo == codigoTabelaFrete &&
                    valor.Situacao == SituacaoItemParametroBaseCalculoTabelaFrete.Aprovacao
                );

            return consultaValores.Count() > 0;
        }

        public void RemoverPendenciaIntegracaoPorParamentroBase(int codigoParametroBaseCalculo)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery(@"
                    update T_TABELA_FRETE_PARAMETRO_BASE_CALCULO_ITEM
                       set TPI_PENDENTE_INTEGRACAO = :PendenteIntegracao,
                           TPI_SITUACAO = :NovaSituacao
                     where TBC_CODIGO = :CodigoParametroBaseCalculo
                       and TPI_TIPO_OBJETO in (:TiposObjeto)
                       and TPI_SITUACAO in (:SituacoesAtual)"
                )
                .SetBoolean("PendenteIntegracao", false)
                .SetEnum("NovaSituacao", SituacaoItemParametroBaseCalculoTabelaFrete.Ativo)
                .SetInt32("CodigoParametroBaseCalculo", codigoParametroBaseCalculo)
                .SetParameterList("TiposObjeto", new List<TipoParametroBaseTabelaFrete>() { TipoParametroBaseTabelaFrete.ComponenteFrete, TipoParametroBaseTabelaFrete.NumeroEntrega })
                .SetParameterList("SituacoesAtual", SituacaoItemParametroBaseCalculoTabelaFreteHelper.ObterSituacoesAguardandoIntegracao())
                .ExecuteUpdate();
        }

        #endregion Métodos Públicos
    }
}
