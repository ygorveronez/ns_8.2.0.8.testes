using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteRotaTipoCargaModeloVeicularCarga : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga>
    {
         public TabelaFreteRotaTipoCargaModeloVeicularCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

         public Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga BuscarPorCodigo(int codigo)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga>();
             var result = from obj in query where obj.Codigo == codigo select obj;
             return result.FirstOrDefault();
         }

         public Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga BuscarPorTipoCargaModeloVeicular(int codigoTabelaFreteRotaTipoCarga, int codigoModeloVeicularCarga)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga>();

             var result = from obj in query select obj;

             result = result.Where(ttm => ttm.TabelaFreteRotaTipoCarga.Codigo == codigoTabelaFreteRotaTipoCarga && ttm.ModeloVeicularCarga.Codigo == codigoModeloVeicularCarga);

             return result.FirstOrDefault();

         }


         public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga> Consultar(int codigoTabelaFreteRotaTipoCarga)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga>();

             var result = from obj in query select obj;

             result = result.Where(ttm => ttm.TabelaFreteRotaTipoCarga.Codigo == codigoTabelaFreteRotaTipoCarga);

             return result.ToList();

         }

         public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga> Consultar(int codigoTabelaFreteRotaTipoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga>();

             var result = from obj in query select obj;

             result = result.Where(ttm => ttm.TabelaFreteRotaTipoCarga.Codigo == codigoTabelaFreteRotaTipoCarga);


             return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

         }

         public int ContarConsulta(int codigoTabelaFreteRotaTipoCarga)
         {
             var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteRotaTipoCargaModeloVeicularCarga>();

             var result = from obj in query select obj;

             result = result.Where(ttm => ttm.TabelaFreteRotaTipoCarga.Codigo == codigoTabelaFreteRotaTipoCarga);

             return result.Count();
         }


         public void DeletarPorTabelaFreteRotaTipoCarga(int codigoTabelaFreteRotaTipoCarga)
         {
             try
             {
                 if (UnitOfWork.IsActiveTransaction())
                 {
                     UnitOfWork.Sessao.CreateQuery("DELETE TabelaFreteRotaTipoCargaModeloVeicularCarga obj WHERE obj.TabelaFreteRotaTipoCarga.Codigo = :codigoTabelaFreteRotaTipoCarga")
                                      .SetInt32("codigoTabelaFreteRotaTipoCarga", codigoTabelaFreteRotaTipoCarga)
                                      .ExecuteUpdate();
                 }
                 else
                 {
                     try
                     {
                         UnitOfWork.Start();
                     
                         UnitOfWork.Sessao.CreateQuery("DELETE TabelaFreteRotaTipoCargaModeloVeicularCarga obj WHERE obj.TabelaFreteRotaTipoCarga.Codigo = :codigoTabelaFreteRotaTipoCarga")
                         .SetInt32("codigoTabelaFreteRotaTipoCarga", codigoTabelaFreteRotaTipoCarga)
                         .ExecuteUpdate();
                     
                         UnitOfWork.CommitChanges();
                     }
                     catch
                     {
                         UnitOfWork.Rollback();
                         throw;
                     }
                 }
             }
             catch (NHibernate.Exceptions.GenericADOException ex)
             {
                 if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                 {
                     System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                     if (excecao.Number == 547)
                     {
                         throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                     }
                 }
                 throw;
             }
         }


        private NHibernate.ISQLQuery _QueryConsultarRelatorio(int codOrigem, int codDestino, int codTipoCarga, int codModeloVeicularCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool contagem, string propOrdenacao = "", string dirOrdenacao = "", int inicioRegistros = 0, int maximoRegistros = 0)
        {
            // Codigo de atividade é fixo
            int codAtividade = 3;

            string sqlQuery = "SELECT ";

            string select = @"
	            TabelaFreteRotaTipoCargaModeloVeicularCarga.TTM_CODIGO Codigo,

	            LocalidadeOrigem.LOC_DESCRICAO Origem,
	            LocalidadeDestino.LOC_DESCRICAO Destino,
	            TabelaFreteRota.TBF_DESCRICAO_DESTINOS Descricao,
	            TabelaFreteRota.TFR_CODIGO_EMBARCADOR CodigoEmbarcador,

	            TipoDeCarga.TCG_DESCRICAO TipoCarga,

	            ModeloVeicularCarga.MVC_DESCRICAO Veiculo,
	            TabelaFreteRotaTipoCargaModeloVeicularCarga.TTM_VALOR_FRETE ValorFrete,
	            TabelaFreteRotaTipoCargaModeloVeicularCarga.TTM_VALOR_PEDAGIO Pedagio,

	            Rota.ROT_DISTANCIA_KM Km,
	            Aliquota.ALI_ALIQUOTA Aliquota,
	            Imposto = 0.0,
	            FreteBruto = 0.0,
	            ValorPorKmLiquido = 0.0,
	            ValorPorKmBruto = 0.0";

            string count = "distinct(count(0) over ())";

            if (contagem)
                sqlQuery += count;
            else
                sqlQuery += select;

            List<string> sqlWhere = new List<string>();
            sqlWhere.Add("1 = 1");
            
            if(ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                sqlWhere.Add("TabelaFreteRota.TFR_ATIVO = 1");

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                sqlWhere.Add("TabelaFreteRota.TFR_ATIVO = 0");

            if (codOrigem > 0)
                sqlWhere.Add("LocalidadeOrigem.LOC_CODIGO = " + (codOrigem.ToString()));

            if (codDestino > 0)
                sqlWhere.Add("LocalidadeDestino.LOC_CODIGO = " + (codDestino.ToString()));

            if (codTipoCarga > 0)
                sqlWhere.Add("TipoDeCarga.TCG_CODIGO = " + (codTipoCarga.ToString()));

            if (codModeloVeicularCarga > 0)
                sqlWhere.Add("ModeloVeicularCarga.MVC_CODIGO = " + (codModeloVeicularCarga.ToString()));

            sqlQuery += @"
                FROM 
	                T_TABELA_FRETE_ROTA_TIPO_CARGA_MODELO_VEICULAR_CARGA TabelaFreteRotaTipoCargaModeloVeicularCarga

                -- Tabelas por hierarquia
                LEFT JOIN 
	                T_TABELA_FRETE_ROTA_TIPO_CARGA TabelaFreteRotaTipoCarga
	                ON TabelaFreteRotaTipoCarga.TTC_CODIGO = TabelaFreteRotaTipoCargaModeloVeicularCarga.TTC_CODIGO
                LEFT JOIN 
	                T_TABELA_FRETE_ROTA TabelaFreteRota
	                ON TabelaFreteRota.TFR_CODIGO = TabelaFreteRotaTipoCarga.TFR_CODIGO
	
                -- Campos
                LEFT JOIN 
	                T_MODELO_VEICULAR_CARGA ModeloVeicularCarga
	                ON ModeloVeicularCarga.MVC_CODIGO = TabelaFreteRotaTipoCargaModeloVeicularCarga.MVC_CODIGO
	
                LEFT JOIN 
	                T_TIPO_DE_CARGA TipoDeCarga
	                ON TipoDeCarga.TCG_CODIGO = TabelaFreteRotaTipoCarga.TCG_CODIGO
	
                LEFT JOIN 
	                T_LOCALIDADES LocalidadeOrigem
	                ON LocalidadeOrigem.LOC_CODIGO = TabelaFreteRota.LOC_CODIGO_ORIGEM
	
                LEFT JOIN 
	                T_LOCALIDADES LocalidadeDestino
	                ON LocalidadeDestino.LOC_CODIGO = TabelaFreteRota.LOC_CODIGO_DESTINO
	
                -- Distancia
                LEFT JOIN 
	                T_ROTA Rota
	                ON Rota.LOC_CODIGO_ORIGEM = TabelaFreteRota.LOC_CODIGO_ORIGEM 
	                AND Rota.LOC_CODIGO_DESTINO = TabelaFreteRota.LOC_CODIGO_DESTINO 
	
                -- Aliquota
                LEFT JOIN 
	                T_ALIQUOTA Aliquota
	                ON Aliquota.ALI_UF_ORIGEM = LocalidadeOrigem.UF_SIGLA
	                AND Aliquota.ALI_UF_DESTINO = LocalidadeDestino.UF_SIGLA
	                AND Aliquota.ALI_UF_EMPRESA = LocalidadeOrigem.UF_SIGLA
	                AND Aliquota.ATI_CODIGO = " + codAtividade.ToString() + @"

                WHERE " + (String.Join("\r\n\tAND ", sqlWhere));

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                sqlQuery += "\r\n\tORDER BY " + propOrdenacao + " " + (dirOrdenacao == "asc" ? " ASC" : " DESC");

            if (maximoRegistros > 0)
                sqlQuery += "\r\n\tOFFSET " + (inicioRegistros.ToString()) + " ROWS FETCH NEXT " + (maximoRegistros.ToString()) + " ROWS ONLY";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            /*if (!string.IsNullOrWhiteSpace(descricao))
                query.SetString("descricao", "%" + descricao + "%");

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                query.SetString("codigoEmbarcador", "%" + codigoEmbarcador + "%");*/

            return query;
        }

        private Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota RegraDeCalculo(Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota obj)
        {
            decimal pAliquota = Math.Round(((decimal)obj.Aliquota / 100), 2, MidpointRounding.ToEven);
            decimal BC = Math.Round((obj.ValorFrete / (1 - pAliquota)), 2 , MidpointRounding.ToEven);

            decimal imposto = Math.Round((BC * pAliquota), 2, MidpointRounding.ToEven);
            decimal freteBruto = (obj.ValorFrete + obj.Pedagio + imposto);

            var x = new Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota()
            {
                Codigo = obj.Codigo,

                Origem = obj.Origem,
                Destino = obj.Destino,
                Descricao = obj.Descricao,
                CodigoEmbarcador = obj.CodigoEmbarcador,

                TipoCarga = obj.TipoCarga,

                Veiculo = obj.Veiculo,
                ValorFrete = obj.ValorFrete,
                Pedagio = obj.Pedagio,

                Km = obj.Km,

                Imposto = imposto,
                FreteBruto = freteBruto,
                ValorPorKmLiquido = obj.Km > 0 ? (obj.ValorFrete / obj.Km) : 0,
                ValorPorKmBruto = obj.Km > 0 ? (freteBruto / obj.Km) : 0
            };

            return x;
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota> ConsultarRelatorio(int codOrigem, int codDestino, int codTipoCarga, int codModeloVeicularCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propAgrupa, string dirAgrupa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _QueryConsultarRelatorio(codOrigem, codDestino, codTipoCarga, codModeloVeicularCarga, ativo, false, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota)));

            var data = result.List<Dominio.Relatorios.Embarcador.DataSource.Fretes.TabelaFreteRota>();
            
            return (from o in data select RegraDeCalculo(o)).ToList();
        }

        public int ContarConsultaRelatorio(int codOrigem, int codDestino, int codTipoCarga, int codModeloVeicularCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = _QueryConsultarRelatorio(codOrigem, codDestino, codTipoCarga, codModeloVeicularCarga, ativo, true);

            return result.UniqueResult<int>();
        }
    }
}
