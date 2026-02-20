using System;
using System.Linq;

namespace Repositorio
{
    public class TokenSigaFacil : RepositorioBase<Dominio.Entidades.TokenSigaFacil>, Dominio.Interfaces.Repositorios.TokenSigaFacil
    {
        public TokenSigaFacil(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.TokenSigaFacil BuscarPorData(int codigoEmpresa, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TokenSigaFacil>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Data == data.Date select obj;

            return result.FirstOrDefault();
        }
    }
}
