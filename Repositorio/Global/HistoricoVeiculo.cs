using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class HistoricoVeiculo : RepositorioBase<Dominio.Entidades.HistoricoVeiculo>, Dominio.Interfaces.Repositorios.HistoricoVeiculo
    {
        public HistoricoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.HistoricoVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.HistoricoVeiculo>();
            var result = from obj in query where obj.Codigo == codigo && obj.Veiculo.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.HistoricoVeiculo> Consultar(int codigoEmpresa, string placaVeiculo, string descricaoServico, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.HistoricoVeiculo>();
            criteria.CreateAlias("Veiculo", "veiculo");
            criteria.CreateAlias("Servico", "servico");
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("veiculo.Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("veiculo.Placa", placaVeiculo, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(descricaoServico))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("servico.Descricao", descricaoServico, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.HistoricoVeiculo>();
        }

        public int ContarConsulta(int codigoEmpresa, string placaVeiculo, string descricaoServico, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.HistoricoVeiculo>();
            criteria.CreateAlias("Veiculo", "veiculo");
            criteria.CreateAlias("Servico", "servico");
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("veiculo.Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("veiculo.Placa", placaVeiculo, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(descricaoServico))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("servico.Descricao", descricaoServico, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Relatorios.RelatorioServicosVeiculos> RelatorioServicos(int codigoEmpresa, int codigoVeiculo, int codigoServico, DateTime dataInicial, DateTime dataFinal)
        {
            string query =
@"SELECT 
    MAX(hist.his_data) Data,
    MAX(hist.his_km_veiculo) KMVeiculo, 
    hist.vei_codigo CodigoVeiculo, 
    veic.vei_placa Placa, 
    veic.vei_kmatual KMAtual, 
    serv.ser_km_troca KMTroca, 
    serv.ser_dias_troca DiasTroca, 
    serv.ser_codigo CodigoServico, 
    serv.ser_descricao DescricaoServico, 
    serv.ser_avisodias DiasAviso, 
    case when serv.ser_dias_troca > 0 then 
        MAX(hist.his_data) + serv.ser_dias_troca  
    else 
        MAX(hist.his_data) 
    end
        DataVcto
FROM 
        t_historico_veiculo hist 
INNER JOIN 
        t_servico_veiculo serv 
ON 
        hist.ser_codigo = serv.ser_codigo 
INNER JOIN
        t_veiculo veic 
ON 
        veic.vei_codigo = hist.vei_codigo 
WHERE 
        veic.VEI_ATIVO = 1 AND 
        hist.his_status = 'A' AND 
        veic.emp_codigo = " + codigoEmpresa.ToString();


            if (codigoServico > 0)
            {
                query += " AND hist.ser_codigo = " + codigoServico.ToString();
            }

            if (codigoVeiculo > 0)
            {
                query += " AND hist.vei_codigo = " + codigoVeiculo.ToString();
            }

            if (dataInicial != DateTime.MinValue)
            {
                query += " AND hist.his_data >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";
            }

            if (dataFinal != DateTime.MinValue)
            {
                query += " AND hist.his_data < '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";
            }

            query +=
                @"
GROUP BY 
	hist.vei_codigo, 
	veic.vei_placa, 
	veic.vei_kmatual, 
	serv.ser_km_troca, 
	serv.ser_dias_troca, 
	serv.ser_codigo, 
	serv.ser_descricao, 
	serv.ser_avisodias, 
	serv.ser_avisokm
HAVING 
	(
		(serv.ser_avisokm > 0 and ((veic.VEI_KMATUAL - max(hist.his_km_veiculo)) >= (serv.ser_km_troca - serv.ser_avisokm)))
		OR
        (serv.ser_avisodias > 0 AND (datediff(day, max(hist.his_data), '" + DateTime.Now.Date.ToString("MM/dd/yyyy") + @"') >= (serv.ser_dias_troca - serv.ser_avisodias)))
    )";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Relatorios.RelatorioServicosVeiculos)));

            return nhQuery.List<Dominio.ObjetosDeValor.Relatorios.RelatorioServicosVeiculos>();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosVeiculos> Relatorio(int codigoEmpresa, int codigoVeiculo, int codigoServico, DateTime dataInicial, DateTime dataFinal, double fornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.HistoricoVeiculo>();

            var result = from obj in query
                         where
                             obj.Veiculo.Empresa.Codigo == codigoEmpresa &&
                             obj.Status == "A"
                         select obj;

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);
            if (codigoServico > 0)
                result = result.Where(o => o.Servico.Codigo == codigoServico);
            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Data.Value >= dataInicial);
            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Data.Value < dataFinal.AddDays(1));

            if (fornecedor > 0)
                result = result.Where(o => o.Fornecedor.CPF_CNPJ == fornecedor);

            return result.Select( obj => new Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosVeiculos()
            {
                Codigo = obj.Codigo,
                CodigoServico = obj.Servico.Codigo,
                CodigoVeiculo = obj.Veiculo.Codigo,
                Data = obj.Data,
                DescricaoServico = obj.Servico.Descricao,
                Km = obj.KMVeiculo,
                Observacao = obj.Observacao,
                PlacaVeiculo = obj.Veiculo.Placa,
                Quantidade = obj.Quantidade,
                Valor = obj.Valor
            }).ToList();
        }
    }
}
