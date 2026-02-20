using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Repositorio.Embarcador.Consulta;

namespace Repositorio
{
    public class VencimentoCertificado : RepositorioBase<Dominio.Entidades.VencimentoCertificado>, Dominio.Interfaces.Repositorios.VencimentoCertificado
    {
        public VencimentoCertificado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.VencimentoCertificado BuscarPorCodigoEAcertoDeViagem(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VencimentoCertificado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.VencimentoCertificado BuscarPorCNPJeVencimentoAmbiente(string cnpj, DateTime dataVencimento, string ambiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VencimentoCertificado>();
            var result = from obj in query where obj.CNPJ.Equals(cnpj) && obj.DataVencimento == dataVencimento && obj.Ambiente.Equals(ambiente) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.VencimentoCertificado> BuscarPorCNPJeVencimento(string cnpj, DateTime dataVencimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VencimentoCertificado>();
            var result = from obj in query where obj.CNPJ.Equals(cnpj) && obj.DataVencimento == dataVencimento select obj;
            return result.ToList();
        }

        public Dominio.Entidades.VencimentoCertificado BuscarUltimoPorCNPJeAmbiente(string cnpj, string ambiente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.VencimentoCertificado>();
            var result = from obj in query where obj.CNPJ.Equals(cnpj) && obj.Ambiente.Equals(ambiente) select obj;
            return result.OrderByDescending(o => o.Codigo).FirstOrDefault();
        }
        
        public IList<Dominio.ObjetosDeValor.ConsultaVencimentoCertificado> Consultar(string cnpj, string nome, string ambiente, string status, string cidade, Dominio.Enumeradores.StatusVendaCertificado? statusVenda, bool comContato, int diasVencimento, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao, int inicioRegistros, int maximoRegistros)
        {
            var query = ObterSQLConsultaCertificados(cnpj, nome, ambiente, status, cidade, statusVenda, comContato, diasVencimento, dataInicio, dataFim, satisfacao);
            query.StringQuery += " ORDER BY VC.VC_DATA_VENCIMENTO";

            if (inicioRegistros > 0)
                query.StringQuery += " OFFSET " + inicioRegistros.ToString() + " ROWS";
            if (inicioRegistros > 0 && maximoRegistros > 0)
                query.StringQuery += " FETCH NEXT " + maximoRegistros.ToString() + " ROWS ONLY;";

            var nhQuery = query.CriarQuery(this.SessionNHiBernate);
            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.ConsultaVencimentoCertificado)));

            return nhQuery.List<Dominio.ObjetosDeValor.ConsultaVencimentoCertificado>();
        }

        public int ContarConsulta(string cnpj, string nome, string ambiente, string status, string cidade, Dominio.Enumeradores.StatusVendaCertificado? statusVenda, bool comContato, int diasVencimento, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao)
        {
            var query = ObterSQLConsultaCertificados(cnpj, nome, ambiente, status, cidade, statusVenda, comContato, diasVencimento, dataInicio, dataFim, satisfacao);
            query.StringQuery = "SELECT COUNT(*) FROM ( " + query.StringQuery + " ) AS DadosVencimento";

            var nhQuery = query.CriarQuery(this.SessionNHiBernate);
            return nhQuery.UniqueResult<int>();
        }

        private SQLDinamico ObterSQLConsultaCertificados(string cnpj, string nome, string ambiente, string status, string cidade, Dominio.Enumeradores.StatusVendaCertificado? statusVenda, bool comContato, int diasVencimento, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao)
        {
            var where = SetarWhereConsulta(cnpj, nome, ambiente, status, cidade, statusVenda, comContato, diasVencimento, dataInicio, dataFim, satisfacao);
            string query = @"SELECT	
                             VC.VC_CNPJ Cnpj, 
		                     substring((select DISTINCT ', ' + VC_NOME from T_VENCIMENTO_CERTIFICADO VC2 where VC2.VC_CNPJ = VC.VC_CNPJ and VC2.VC_DATA_VENCIMENTO = VC.VC_DATA_VENCIMENTO for XML PATH('')), 3, 1000) Nome,
                             VC.VC_DATA_VENCIMENTO Vencimento, 
                             VC.VC_STATUS_VENDA StatusVenda,
                             VC.VC_SATISFACAO Satisfacao,
                             CONCAT(_localidades.LOC_DESCRICAO, '/', _localidades.UF_SIGLA) Localidade,
                             (select _pai.EMP_RAZAO from T_EMPRESA _empresa JOIN T_EMPRESA _pai ON _empresa.EMP_EMPRESA = _pai.EMP_CODIGO where _empresa.EMP_CNPJ = VC.VC_CNPJ) EmpresaAdmin,
		                     substring((select DISTINCT ', ' + VC_EMAIL from T_VENCIMENTO_CERTIFICADO VC2 where VC2.VC_CNPJ = VC.VC_CNPJ and VC2.VC_DATA_VENCIMENTO = VC.VC_DATA_VENCIMENTO for XML PATH('')), 3, 1000) Email,
		                     substring((select DISTINCT ', ' + VC_TELEFONE from T_VENCIMENTO_CERTIFICADO VC2 where VC2.VC_CNPJ = VC.VC_CNPJ and VC2.VC_DATA_VENCIMENTO = VC.VC_DATA_VENCIMENTO for XML PATH('')), 3, 1000) Telefone,
		                     substring((select ', ' + VC_AMBIENTE + CASE WHEN VC_HOMOLOGACAO = 'S' THEN ' Homologação' else '' END
                                                                       +(CASE WHEN VC_STATUS = 'P' AND VC2.VC_DATA_VENCIMENTO < GETDATE() THEN ' (VENCIDO)' ELSE 
													                     CASE WHEN VC_STATUS = 'A' THEN ' (ATUALIZADO)' ELSE 
													                     CASE WHEN VC_STATUS = 'C' THEN ' (ATUALIZAÇÃO CONFIRMADA)' ELSE 
 													                     CASE WHEN VC_STATUS = 'I' THEN ' (INATIVO)' ELSE '' 
                    													END END END END) from T_VENCIMENTO_CERTIFICADO VC2 where VC2.VC_CNPJ = VC.VC_CNPJ and VC2.VC_DATA_VENCIMENTO = VC.VC_DATA_VENCIMENTO for XML PATH('')), 3, 1000) Ambiente
                             FROM T_VENCIMENTO_CERTIFICADO VC
                             LEFT JOIN T_EMPRESA _empresa ON _empresa.EMP_CNPJ = VC.VC_CNPJ
                             LEFT JOIN T_LOCALIDADES _localidades on _localidades.LOC_CODIGO = _empresa.LOC_CODIGO
                             WHERE 1 = 1 " + where.WhereClause;
            query += " group by VC_CNPJ, VC_DATA_VENCIMENTO, VC.VC_SATISFACAO, VC.VC_STATUS_VENDA, _localidades.LOC_DESCRICAO, _localidades.UF_SIGLA";

            return new SQLDinamico(query, where.Parametros);
        }

        private (string WhereClause, List<ParametroSQL> Parametros) SetarWhereConsulta(string cnpj, string nome, string ambiente, string status, string cidade, Dominio.Enumeradores.StatusVendaCertificado? statusVenda, bool comContato, int diasVencimento, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao? satisfacao)
        {
            string where = "";
            var parametros = new List<ParametroSQL>();

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "P")
                    where += " AND VC.VC_STATUS = 'P'";
                else if (status == "V")
                    where += " AND VC.VC_STATUS = 'P' AND VC.VC_DATA_VENCIMENTO <= GETDATE()";
                else if (status == "N")
                    where += " AND VC.VC_STATUS = 'P' AND VC.VC_DATA_VENCIMENTO > GETDATE()";
                else if (status == "A")
                    where += " AND VC.VC_STATUS = 'A'";
                else if (status == "C")
                    where += " AND VC.VC_STATUS = 'C'";
                else if (status == "I")
                    where += " AND VC.VC_STATUS = 'I'";
            }

            if (dataInicio != DateTime.MinValue || dataFim != DateTime.MinValue)
            {
                if (dataInicio != DateTime.MinValue)
                    where += " AND VC.VC_DATA_VENCIMENTO >= '" + dataInicio.ToString("yyyy-MM-dd") + "'";

                if (dataFim != DateTime.MinValue)
                    where += " AND VC.VC_DATA_VENCIMENTO <= '" + dataFim.ToString("yyyy-MM-dd") + "'";
            }
            else if (diasVencimento > 0)
                where += " AND VC.VC_DATA_VENCIMENTO < '" + DateTime.Today.AddDays(diasVencimento).ToString("yyyy-MM-dd") + "'";

            if (!string.IsNullOrWhiteSpace(cnpj))
            {
                //where += " AND VC.VC_CNPJ like '" + cnpj + "%'"; 
                where += " AND VC.VC_CNPJ like :VC_VC_CNPJ";
                parametros.Add(new ParametroSQL("VC_VC_CNPJ", cnpj + "%")); 
            }

            if (!string.IsNullOrWhiteSpace(nome))
            {
                //where += " AND VC.VC_NOME like '%" + nome + "%'";
                where += " AND VC.VC_NOME like :VC_VC_NOME";
                parametros.Add(new ParametroSQL("VC_VC_NOME", "%" + nome + "%"));
            }

            if (!string.IsNullOrWhiteSpace(ambiente))
            {
                //where += " AND VC.VC_AMBIENTE like '%" + ambiente + "%'";
                where += " AND VC.VC_AMBIENTE like :VC_VC_AMBIENTE";
                parametros.Add(new ParametroSQL("VC_VC_AMBIENTE", "%" + ambiente + "%"));
            }

            if (!string.IsNullOrWhiteSpace(cidade))
            {
                //where += " AND _localidades.LOC_DESCRICAO like '%" + cidade + "%'";
                where += " AND _localidades.LOC_DESCRICAO like :LOCALIDADES_LOC_DESCRICAO";
                parametros.Add(new ParametroSQL("LOCALIDADES_LOC_DESCRICAO", "%" + cidade + "%"));
            }

            if (statusVenda.HasValue)
                where += " AND VC.VC_STATUS_VENDA = " + statusVenda.Value.ToString("d");

            if (comContato)
                where += " AND VC.VC_STATUS_VENDA <> " + Dominio.Enumeradores.StatusVendaCertificado.SemContato.ToString("d");

            if(satisfacao.HasValue)
                where += " AND VC.VC_SATISFACAO = " + satisfacao.Value.ToString("d");

            return (where, parametros);

        }
    }
}
