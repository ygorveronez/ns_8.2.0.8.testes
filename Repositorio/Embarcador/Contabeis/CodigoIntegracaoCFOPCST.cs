using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Contabeis
{
    public class CodigoIntegracaoCFOPCST : RepositorioBase<Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST>
    {
        public CodigoIntegracaoCFOPCST(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST BuscarCodigoIntegracaoCFOPCST(string cst, string cfop)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST>();
            query = query.Where(obj =>obj.CST == cst && obj.CFOP == cfop);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST> Consultar(string codigoIntegracao, string cst, string cfop, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(codigoIntegracao, cst, cfop);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(string codigoIntegracao, string cst, string cfop)
        {
            var result = Consultar(codigoIntegracao, cst, cfop);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST> Consultar(string codigoIntegracao, string cst, string cfop)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));
            if (!string.IsNullOrWhiteSpace(cst))
                result = result.Where(obj => obj.CST.Contains(cst));
            if (!string.IsNullOrWhiteSpace(cfop))
                result = result.Where(obj => obj.CFOP.Contains(cfop));

            return result;
        }

        #endregion
    }
}
