using System;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPreviaDocumentoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPreviaDocumentoDocumento>
    {
        public CargaPreviaDocumentoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void DeletarPorCarga(int codigo)
        {
            throw new NotImplementedException();
        }
    }
}
