using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class VencimentoCertificadoHistorico : RepositorioBase<Dominio.Entidades.VencimentoCertificadoHistorico>
    {
        public VencimentoCertificadoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IList<Dominio.ObjetosDeValor.ConsultaVencimentoCertificadoDetalhes> Consultar(string cnpj, DateTime vencimento, int inicioRegistros, int maximoRegistros)
        {
            string select = @"DISTINCT Historico.VCH_DATA_LANCAMENTO DataLancamento, 
                              Historico.VCH_TIPO Tipo,
                              Historico.VCH_STATUS_VENDA StatusVenda, 
                              Historico.VCH_SATISFACAO Satisfacao, 
                              Usuario.FUN_NOME Usuario, 
                              Historico.VCH_DETALHES Detalhes";

            string query = ObterSQLHistoricoVencimento(select, cnpj, vencimento);

            query += @" ORDER BY Historico.VCH_DATA_LANCAMENTO DESC
                        OFFSET " + inicioRegistros.ToString() + " ROWS FETCH NEXT " + maximoRegistros.ToString() + " ROWS ONLY;";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query)
                              .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.ConsultaVencimentoCertificadoDetalhes)));

            return nhQuery.List<Dominio.ObjetosDeValor.ConsultaVencimentoCertificadoDetalhes>();
        }

        public int ContarConsulta(string cnpj, DateTime vencimento)
        {
            string select = "DISTINCT(COUNT(0) OVER ())";
            string query = ObterSQLHistoricoVencimento(select, cnpj, vencimento);

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);
            return nhQuery.UniqueResult<int>();
        }

        private string ObterSQLHistoricoVencimento(string select, string cnpj, DateTime vencimento)
        {
            string query = @"SELECT 
                                " + select + @"
                            FROM 
                                T_VENCIMENTO_CERTIFICADO_HISTORICO Historico
                            JOIN 
                                T_VENCIMENTO_CERTIFICADO Vencimento
                                    ON Historico.VC_CODIGO = Vencimento.VC_CODIGO
                            JOIN 
                                T_FUNCIONARIO Usuario 
                                    ON Historico.FUN_CODIGO = Usuario.FUN_CODIGO
                            WHERE
                                Vencimento.VC_CNPJ = '" + cnpj + @"' AND 
                                Vencimento.VC_DATA_VENCIMENTO = '" + vencimento.ToString("yyyy-MM-dd") + @"'
                            GROUP BY
	                            Historico.VCH_DATA_LANCAMENTO,
                                Historico.VCH_TIPO,
                                Historico.VCH_STATUS_VENDA,
                                Historico.VCH_SATISFACAO,
                                Usuario.FUN_NOME, 
                                Historico.VCH_DETALHES";

            return query;
        }
    }
}
