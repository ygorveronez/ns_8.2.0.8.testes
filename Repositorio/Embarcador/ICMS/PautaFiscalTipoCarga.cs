using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ICMS
{
    public class PautaFiscalTipoCarga : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga>
    {

        #region Construtores

        public PautaFiscalTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga> BuscarPorPautaFiscal(int codigoPautaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga>();
            var result = from obj in query where obj.PautaFiscal.Codigo == codigoPautaFiscal select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga> BuscarPorPautaFiscalETipoCargaDiferente(int codigoPautaFiscal, List<int> codigosTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga>();
            var result = from obj in query where obj.PautaFiscal.Codigo == codigoPautaFiscal && !codigosTipoCarga.Contains(obj.TipoCarga.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga BuscarPorPautaFiscalETipoCarga(int codigoPautaFiscal, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PautaFiscalTipoCarga>();
            var result = from obj in query where obj.PautaFiscal.Codigo == codigoPautaFiscal && obj.TipoCarga.Codigo == codigoTipoCarga select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}

