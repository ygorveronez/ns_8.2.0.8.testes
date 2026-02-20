using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    public class ImpressaoIntegracaoCTe : RepositorioBase<Dominio.Entidades.ImpressaoIntegracaoCTe>, Dominio.Interfaces.Repositorios.ImpressaoIntegracaoCTe
    {
        public ImpressaoIntegracaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ImpressaoIntegracaoCTe Buscar(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpressaoIntegracaoCTe>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ImpressaoIntegracaoCTe Buscar(int codigo, int codigoEmpresaPai)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpressaoIntegracaoCTe>();

            var result = from obj in query where obj.Codigo == codigo && obj.CTe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ImpressaoIntegracaoCTe> Buscar(int codigoEmpresaPai, Dominio.Enumeradores.StatusImpressaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpressaoIntegracaoCTe>();

            var result = from obj in query where obj.Status == status && obj.CTe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ImpressaoIntegracaoCTe> Buscar(int codigoEmpresaPai, int numeroUnidade, Dominio.Enumeradores.StatusImpressaoCTe status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ImpressaoIntegracaoCTe>();
            var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.IntegracaoCTe>();

            var result = from obj in query
                         where
                         obj.Status == status &&
                         obj.CTe.Empresa.EmpresaPai.Codigo == codigoEmpresaPai &&
                         (from integracao in queryIntegracao where integracao.NumeroDaUnidade == numeroUnidade select integracao.CTe.Codigo).Distinct().Contains(obj.CTe.Codigo)
                         orderby obj.CTe.Numero
                         select obj;

            return result.Take(1).ToList();
        }
    }
}
