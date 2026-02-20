using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Operacional.Canhoto
{
    public class OperadorCanhotoTipoCargaModeloVeicular : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular>
    {
        public OperadorCanhotoTipoCargaModeloVeicular(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular> BuscarPorOperadorTipoCarga(int codigoOperadorTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular>();
            var result = from obj in query where obj.OperadorCanhotoTipoCarga.Codigo == codigoOperadorTipoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular> BuscarPorOperadoresTiposCarga(List<int> codigosOperadorTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular>();
            var result = from obj in query where codigosOperadorTipoCarga.Contains(obj.OperadorCanhotoTipoCarga.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular BuscarPorOperadorTipoCargaEMVC(int codigoOperadorTipoCarga, int mvcCodigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.Canhoto.OperadorCanhotoTipoCargaModeloVeicular>();
            var result = from obj in query where obj.OperadorCanhotoTipoCarga.Codigo == codigoOperadorTipoCarga && obj.ModeloVeicularCarga.Codigo == mvcCodigo select obj;
            return result.FirstOrDefault();
        }
    }
}
