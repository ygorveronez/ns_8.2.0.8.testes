using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
    public class BemTransferencia : RepositorioBase<Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia>
    {
        public BemTransferencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia> Consultar(int codigoEmpresa, int codigoFuncionario, DateTime dataEnvio, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia>();

            var result = from obj in query select obj;

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == codigoFuncionario);

            if (dataEnvio > DateTime.MinValue)
                result = result.Where(obj => obj.DataEnvio == dataEnvio);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoFuncionario, DateTime dataEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemTransferencia>();

            var result = from obj in query select obj;

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == codigoFuncionario);

            if (dataEnvio > DateTime.MinValue)
                result = result.Where(obj => obj.DataEnvio == dataEnvio);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }
    }
}
