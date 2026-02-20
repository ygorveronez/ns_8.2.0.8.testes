using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContratoFinanciamentoDocumentoEntrada : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada>
    {
        public ContratoFinanciamentoDocumentoEntrada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada BuscarPorDocumentoEntradaEContrato(int codigoDocumentoEntrada, int codigoContratoFinanciamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada>();
            var result = from obj in query where obj.DocumentoEntradaTMS.Codigo == codigoDocumentoEntrada && obj.ContratoFinanciamento.Codigo == codigoContratoFinanciamento select obj;
            return result.FirstOrDefault();
        }
    }
}
