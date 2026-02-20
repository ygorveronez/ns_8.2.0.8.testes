using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Documentos
{
    public class GestaoDocumentoHistoricoCTe : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.GestaoDocumentoHistoricoCTe>
	{
		public GestaoDocumentoHistoricoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

		public List<Dominio.Entidades.Embarcador.Documentos.GestaoDocumentoHistoricoCTe> BuscarPorGestaoDocumento(int codigoGestaoDocumento)
		{
			var consultaGestaoDocumentoHistoricoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.GestaoDocumentoHistoricoCTe>()
				.Where(o => o.GestaoDocumento.Codigo == codigoGestaoDocumento);

			return consultaGestaoDocumentoHistoricoCTe.ToList();
		}
	}
}