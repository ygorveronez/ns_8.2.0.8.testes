using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios.Comissao
{
    public class FuncionarioMeta : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta>
    {
        public FuncionarioMeta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta> Consultar(int codigoEmpresa, int codigoFuncionario, DateTime dataVigencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta>();

            var result = from obj in query select obj;

            if (dataVigencia > DateTime.MinValue)
                result = result.Where(obj => obj.DataVigencia == dataVigencia);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == codigoFuncionario);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoFuncionario, DateTime dataVigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta>();

            var result = from obj in query select obj;

            if (dataVigencia > DateTime.MinValue)
                result = result.Where(obj => obj.DataVigencia == dataVigencia);

            if (codigoFuncionario > 0)
                result = result.Where(obj => obj.Funcionario.Codigo == codigoFuncionario);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta BuscarPorFuncionario(int codigoFuncionario, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta>();
            var result = from obj in query where obj.Funcionario.Codigo == codigoFuncionario && obj.DataVigencia >= DateTime.Now select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (result.Count() == 0)
            {
                var resultMeta = from obj in query where obj.Funcionario == null && obj.DataVigencia.Date >= DateTime.Now.Date select obj;

                if (codigoEmpresa > 0)
                    resultMeta = resultMeta.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

                return resultMeta.FirstOrDefault();
            }
            else
                return result.FirstOrDefault();
        }
    }
}
