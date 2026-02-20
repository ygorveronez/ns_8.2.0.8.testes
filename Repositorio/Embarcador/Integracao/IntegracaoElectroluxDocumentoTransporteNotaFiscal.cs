using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoElectroluxDocumentoTransporteNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal>
    {
        public IntegracaoElectroluxDocumentoTransporteNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal> BuscarPorDT(int codigoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal>();

            query = query.Where(o => o.DocumentoTransporte.Codigo == codigoDT);

            return query.ToList();
        }

        public List<int> BuscarNumerosPorDT(int codigoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal>();

            query = query.Where(o => o.DocumentoTransporte.Codigo == codigoDT);

            return query.Select(o => o.Numero).ToList();
        }

        public decimal BuscarPesoPorDT(IEnumerable<int> codigosDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal>();

            query = query.Where(o => codigosDT.Contains(o.DocumentoTransporte.Codigo));

            return query.Sum(o => (decimal?)o.Peso) ?? 0m;
        }

        public int BuscarVolumesPorDT(IEnumerable<int> codigosDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal>();

            query = query.Where(o => codigosDT.Contains(o.DocumentoTransporte.Codigo));

            return query.Sum(o => (int?)o.Quantidade) ?? 0;
        }


    }
}
