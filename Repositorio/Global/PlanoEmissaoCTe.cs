using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PlanoEmissaoCTe : RepositorioBase<Dominio.Entidades.PlanoEmissaoCTe>, Dominio.Interfaces.Repositorios.PlanoEmissaoCTe
    {
        public PlanoEmissaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PlanoEmissaoCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoEmissaoCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.PlanoEmissaoCTe BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            var result = from obj in query where obj.Codigo == codigoEmpresa select obj.PlanoEmissaoCTe;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PlanoEmissaoCTe> BuscarAtivos(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoEmissaoCTe>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.PlanoEmissaoCTe> Consultar(int codigoEmpresa, string descricao, string status, int incioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoEmissaoCTe>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            
            return result.Skip(incioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoEmissaoCTe>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioCobrancas2> RelatorioDeCobrancas2()
        {
            throw new NotImplementedException("Método não implementado.");
        }
    }
}
