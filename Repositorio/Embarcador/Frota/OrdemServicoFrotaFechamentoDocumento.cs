using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class OrdemServicoFrotaFechamentoDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>
    {
        public OrdemServicoFrotaFechamentoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento> BuscarPorOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>();

            query = query.Where(o => o.DocumentoEntrada.Codigo == codigoDocumentoEntrada);

            return query.ToList();
        }

        public int ContarConsulta(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.Count();
        }

        public bool ContemRegistroDuplicado(int codigoOrdemServico, int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico && o.DocumentoEntrada.Codigo == codigoDocumentoEntrada);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento> Consultar(int codigoOrdemServico, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>();

            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int QuantidadeDocumentosOrdemServico(int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoDocumento>();
            query = query.Where(o => o.OrdemServico.Codigo == codigoOrdemServico);
            return query.Count();
        }

        #endregion
    }
}
