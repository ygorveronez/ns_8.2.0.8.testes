using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Fatura
{
    public class FaturaCargaDocumentoExcluido : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumentoExcluido>
    {
        public FaturaCargaDocumentoExcluido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumentoExcluido BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumentoExcluido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool CTeExcluidoEmFatura(int codigo, int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>();
            var result = from obj in query where obj.ConhecimentoDeTransporteEletronico.Codigo == codigo && obj.Fatura.Codigo == codigoFatura && obj.StatusDocumentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura.Excluido select obj;
            return result.Count() > 0;
        }
    }
}
