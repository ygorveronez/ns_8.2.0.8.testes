using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Repositorio.Embarcador.Financeiro.Consulta;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContratoFinanciamento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento>
    {
        public ContratoFinanciamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento BuscarPorNumeroDocumento(string numeroDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento>();
            var result = from obj in query where obj.NumeroDocumento == numeroDocumento select obj;
            return result.FirstOrDefault();
        }
        public int ProximoNumeroContrato(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento> Consultar(int codigoEmpresa, int numero, string numeroDocumento, double fornecedor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento situacao, string placaVeiculo, int numeroDocumentoEntrada, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento>();

            var result = from obj in query select obj;

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(obj => obj.NumeroDocumento.Contains(numeroDocumento));

            if (fornecedor > 0)
                result = result.Where(obj => obj.Fornecedor.CPF_CNPJ == fornecedor);

            if ((int)situacao > 0)
                result = result.Where(obj => obj.Situacao == situacao);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(obj => obj.Veiculos.Any(v => v.Veiculo.Placa == placaVeiculo));

            if (numeroDocumentoEntrada > 0)
                result = result.Where(obj => obj.DocumentosEntrada.Any(d => d.DocumentoEntradaTMS.Numero == numeroDocumentoEntrada));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int numero, string numeroDocumento, double fornecedor, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento situacao, string placaVeiculo, int numeroDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamento>();

            var result = from obj in query select obj;

            if (numero > 0)
                result = result.Where(obj => obj.Numero == numero);

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(obj => obj.NumeroDocumento.Contains(numeroDocumento));

            if (fornecedor > 0)
                result = result.Where(obj => obj.Fornecedor.CPF_CNPJ == fornecedor);

            if ((int)situacao > 0)
                result = result.Where(obj => obj.Situacao == situacao);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(obj => obj.Veiculos.Any(v => v.Veiculo.Placa == placaVeiculo));

            if (numeroDocumentoEntrada > 0)
                result = result.Where(obj => obj.DocumentosEntrada.Any(d => d.DocumentoEntradaTMS.Numero == numeroDocumentoEntrada));

            return result.Count();
        }
    
        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ContratoFinanceiro> ConsultarRelatorioContratoFinanceiro(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioContratoFinanceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaContratoFinanceiro = new ConsultaContratoFinanceiro().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaContratoFinanceiro.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ContratoFinanceiro)));

            return consultaContratoFinanceiro.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ContratoFinanceiro>();
        }

        public int ContarConsultaRelatorioContratoFinanceiro(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioContratoFinanceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaContratoFinanceiro = new ConsultaContratoFinanceiro().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaContratoFinanceiro.SetTimeout(600).UniqueResult<int>();
        }
    }
}
