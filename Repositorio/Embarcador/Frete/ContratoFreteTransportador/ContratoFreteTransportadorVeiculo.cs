using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>
    {
        #region Construtores

        public ContratoFreteTransportadorVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool VeiculoPossuiContratoValido(int veiculo, DateTime dataVigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>();
            var result = from obj in query
                         where 
                            obj.Veiculo.Codigo == veiculo
                            && obj.ContratoFrete.Ativo
                            && obj.ContratoFrete.DataInicial.Date <= dataVigencia.Date
                            && obj.ContratoFrete.DataFinal.Date >= dataVigencia.Date
                         select obj.ContratoFrete;
            return result.Count() > 0;
        }

        public List<int> BuscarCodigosNaoPresentesNaLista(int contrato, List<int> codigosVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && !codigosVeiculo.Contains(obj.Veiculo.Codigo)
                         select obj.Veiculo.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo BuscarPorContratoEVeiculo(int contrato, int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && obj.Veiculo.Codigo == veiculo
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo> BuscarPorContrato(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                         select obj;

            return result.ToList();
        }

        public List<(int Transportador, int Veiculo)> BuscarPorCargasComContratoAtivoParaTransportadorEVeiculo(List<int> codigosCarga)
        {
            DateTime vigencia = DateTime.Now.Date;

            var consultaContratoFreteTransportadorVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>()
                .Where(o =>
                    o.ContratoFrete.DataInicial.Date <= vigencia &&
                    o.ContratoFrete.DataFinal.Date >= vigencia.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.ContratoFrete.Ativo
                );

            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(o => codigosCarga.Contains(o.Codigo));

            consultaContratoFreteTransportadorVeiculo = consultaContratoFreteTransportadorVeiculo.Where(o => consultaCarga.Any(c => c.Empresa.Codigo == o.ContratoFrete.Transportador.Codigo && c.Veiculo.Codigo == o.Veiculo.Codigo));

            return consultaContratoFreteTransportadorVeiculo
                .Select(o => ValueTuple.Create(o.ContratoFrete.Transportador.Codigo, o.Veiculo.Codigo))
                .ToList();
        }

        public List<(int CodigoVeiculo, string NumeroContrato)> BuscarContratosAtivosPorVeiculos(List<int> codigosVeiculo)
        {
            var consultaContratoFreteTransportadorVeiculo = ConsultarContratosAtivosPorVeiculos(codigosVeiculo, dataVigencia: DateTime.Today);

            return consultaContratoFreteTransportadorVeiculo
                .Select(veiculoContrato => ValueTuple.Create(veiculoContrato.Veiculo.Codigo, veiculoContrato.ContratoFrete.NumeroEmbarcador))
                .ToList();
        }

        public bool ExiteContratosAtivosPorVeiculos(List<int> codigosVeiculo, DateTime dataVigencia)
        {
            var consultaContratoFreteTransportadorVeiculo = ConsultarContratosAtivosPorVeiculos(codigosVeiculo, dataVigencia);

            return consultaContratoFreteTransportadorVeiculo.Count() > 0;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo> Consultar(int contrato, DateTime periodoInicial, DateTime periodoFinal, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            string sql = ObterSelectConsultaVeiculos(false, contrato, periodoInicial, periodoFinal, propOrdena, dirOrdena, inicioRegistros, maximoRegistros);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo)));

            return query.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo>();
        }

        //public int ContarConsulta(int contrato)
        //{
        //    var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaVeiculos(true, contrato, string.Empty, string.Empty, 0, 0));

        //    return query.UniqueResult<int>();
        //}

        public void DeletarPorContrato(int codigoContrato)
        {
            try
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete ContratoFreteTransportadorVeiculo VeiculoContrato where VeiculoContrato.ContratoFrete.Codigo = :codigoContrato")
                    .SetInt32("codigoContrato", codigoContrato)
                    .ExecuteUpdate();
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecao);
                }

                throw;
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo> ConsultarContratosAtivosPorVeiculos(List<int> codigosVeiculo, DateTime dataVigencia)
        {
            List<SituacaoContratoFreteTransportador> situacoesContratoAtivo = new List<SituacaoContratoFreteTransportador>()
            {
                SituacaoContratoFreteTransportador.AgAprovacao,
                SituacaoContratoFreteTransportador.Aprovado
            };

            var consultaContratoFreteTransportadorVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo>()
                .Where(veiculoContrato =>
                    veiculoContrato.ContratoFrete.Ativo == true &&
                    veiculoContrato.ContratoFrete.DataInicial.Date <= dataVigencia.Date &&
                    veiculoContrato.ContratoFrete.DataFinal.Date >= dataVigencia.Date &&
                    situacoesContratoAtivo.Contains(veiculoContrato.ContratoFrete.Situacao) &&
                    codigosVeiculo.Contains(veiculoContrato.Veiculo.Codigo)
                );

            return consultaContratoFreteTransportadorVeiculo;
        }

        private string ObterSelectConsultaVeiculos(bool count, int contrato, DateTime periodoInicial, DateTime periodoFinal, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string diaria = Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Diaria.ToString("D");
            string quinzena = Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Quinzena.ToString("D");

            string select = @"ContratoVeiculo.CFV_CODIGO Codigo,
	                        Veiculo.VEI_CODIGO CodigoVeiculo,
	                        COUNT(CTe.CON_CODIGO) QuantidadeDocumentos,
	                        SUM(CTe.CON_VALOR_RECEBER) ValorDocumentos,
	                        Veiculo.VEI_PLACA Veiculo,
	
	                        CASE WHEN ContratoVeiculo.CFV_TIPO_PAGAMENTO_CONTRATO_FRETE = " + diaria + @" THEN (
		                        CASE 
		                        WHEN ContratoModelo.CFM_VALOR_DIARIA > 0 THEN ContratoModelo.CFM_VALOR_DIARIA
		                        ELSE Contrato.CFT_VALOR_DIARIA_POR_VEICULO END
	                        ) ELSE 0 END ValorDiaria,
	
	                        CASE WHEN ContratoVeiculo.CFV_TIPO_PAGAMENTO_CONTRATO_FRETE = " + quinzena + @" THEN (
		                        CASE 
		                        WHEN ContratoModelo.CFM_VALOR_QUINZENA > 0 THEN ContratoModelo.CFM_VALOR_QUINZENA
		                        ELSE Contrato.CFT_VALOR_QUINZENA_POR_VEICULO END
	                        ) ELSE 0 END ValorQuinzena,

	                        CASE WHEN ContratoVeiculo.CFV_TIPO_PAGAMENTO_CONTRATO_FRETE = " + quinzena + @" THEN (
		                        CASE 
		                        WHEN ContratoModelo.CFM_VALOR_QUINZENA > 0 THEN ContratoModelo.CFM_VALOR_QUINZENA
		                        ELSE Contrato.CFT_VALOR_QUINZENA_POR_VEICULO END
	                        ) ELSE 0
	                        END Total";

            string joins = @"JOIN T_VEICULO Veiculo
                                ON Veiculo.VEI_CODIGO = ContratoVeiculo.VEI_CODIGO
                            LEFT JOIN T_CONTRATO_FRETE_TRANSPORTADOR_MODELO_VEICULAR ContratoModelo
                                ON ContratoModelo.MVC_CODIGO = Veiculo.MVC_CODIGO AND ContratoModelo.CFT_CODIGO = " + contrato.ToString() + @"
                            LEFT JOIN T_CONTRATO_FRETE_TRANSPORTADOR Contrato
                                ON ContratoVeiculo.CFT_CODIGO = Contrato.CFT_CODIGO
                            LEFT JOIN T_CARGA Cargas
	                            ON Veiculo.VEI_CODIGO = Cargas.CAR_VEICULO AND Cargas.CAR_DATA_CRIACAO BETWEEN '" + periodoInicial.ToString("yyyy-MM-dd") + "' AND '" + periodoFinal.ToString("yyyy-MM-dd") + @" 23:59:59'
                            LEFT JOIN T_CARGA_CTE CargaCTe
	                            ON Cargas.CAR_CODIGO = CargaCTe.CAR_CODIGO
                            LEFT JOIN T_CTE Cte
	                            ON CargaCTe.CON_CODIGO = Cte.CON_CODIGO AND Cte.CON_STATUS = 'A'";

            string where = RetornaWhereConsultaVeiculos(contrato);
            string groupby = @"ContratoVeiculo.CFV_CODIGO, 
	                           Veiculo.VEI_CODIGO,
	                           Veiculo.VEI_PLACA,
	                           ContratoVeiculo.CFV_TIPO_PAGAMENTO_CONTRATO_FRETE,
	                           ContratoModelo.CFM_VALOR_DIARIA,
	                           Contrato.CFT_VALOR_DIARIA_POR_VEICULO,
	                           ContratoModelo.CFM_VALOR_QUINZENA,
	                           Contrato.CFT_VALOR_QUINZENA_POR_VEICULO";
            string orderBy = string.Empty;

            string query = @"
                            SELECT " + (count ? " DISTINCT(COUNT(0) OVER ())" : select) + @" 
                            FROM T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoVeiculo
                            " + joins + @"
                            WHERE " + where + " GROUP BY " + groupby;

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propOrdena) && select.Contains(propOrdena))
                    orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                query += " ORDER BY " + (orderBy.Length > 0 ? orderBy : "1 ASC ");

                if (inicio > 0 || limite > 0)
                    query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;";
            }

            return query;
        }

        private string RetornaWhereConsultaVeiculos(int contrato)
        {
            string where = " Contrato.CFT_CODIGO = " + contrato.ToString();

            return where;
        }

        #endregion Métodos Privados
    }
}
