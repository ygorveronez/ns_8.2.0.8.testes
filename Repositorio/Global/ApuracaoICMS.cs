using System;
using System.Linq;

namespace Repositorio
{
    public class ApuracaoICMS: RepositorioBase<Dominio.Entidades.ApuracaoICMS>, Dominio.Interfaces.Repositorios.AliquotaDeICMS
    {
        public ApuracaoICMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ApuracaoICMS BuscarPorPeriodo(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ApuracaoICMS>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.DataInicial == dataInicial && obj.DataFinal == dataFinal select obj;

            return result.FirstOrDefault();
        }
    }
}
