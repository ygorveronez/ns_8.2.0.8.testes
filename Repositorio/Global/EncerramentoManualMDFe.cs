using System;
using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class EncerramentoManualMDFe : RepositorioBase<Dominio.Entidades.EncerramentoManualMDFe>, Dominio.Interfaces.Repositorios.EncerramentoManualMDFe
    {
        public EncerramentoManualMDFe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EncerramentoManualMDFe BuscaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EncerramentoManualMDFe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.EncerramentoManualMDFe BuscaPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EncerramentoManualMDFe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.EncerramentoManualMDFe> Consultar(int codigoEmpresa, int codigoUsuario, int codigoLocalidade, string nomeDaEmpresa, string CNPJ, string chaveMDFe, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EncerramentoManualMDFe>();

            var result = from obj in query where 1 == 1 select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (codigoUsuario > 0)
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);

            if (codigoLocalidade > 0)
                result = result.Where(o => o.Localidade.Codigo == codigoLocalidade);

            if (!string.IsNullOrWhiteSpace(nomeDaEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeDaEmpresa));

            if (!string.IsNullOrWhiteSpace(CNPJ))
                result = result.Where(o => o.Empresa.CNPJ.Equals(CNPJ));

            if (!string.IsNullOrWhiteSpace(chaveMDFe))
                result = result.Where(o => o.ChaveMDFe.Equals(chaveMDFe));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataHoraEncerramento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataHoraEncerramento <= dataFinal.Date);


            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoUsuario, int codigoLocalidade, string nomeDaEmpresa, string CNPJ, string chaveMDFe, DateTime dataInicial, DateTime dataFinal, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EncerramentoManualMDFe>();

            var result = from obj in query where 1 == 1 select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (codigoUsuario > 0)
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);

            if (codigoLocalidade > 0)
                result = result.Where(o => o.Localidade.Codigo == codigoLocalidade);

            if (!string.IsNullOrWhiteSpace(nomeDaEmpresa))
                result = result.Where(o => o.Empresa.NomeFantasia.Contains(nomeDaEmpresa));

            if (!string.IsNullOrWhiteSpace(CNPJ))
                result = result.Where(o => o.Empresa.CNPJ.Equals(CNPJ));

            if (!string.IsNullOrWhiteSpace(chaveMDFe))
                result = result.Where(o => o.ChaveMDFe.Equals(chaveMDFe));

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataHoraEncerramento >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataHoraEncerramento <= dataFinal.Date);


            return result.Count();
        }
    }
}
