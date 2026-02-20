using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoMiroDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento>
    {
        #region Contructores
        public LoteEscrituracaoMiroDocumento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento> BuscarLoteEscrituracaoMiroDocumentosPorLote(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento>()
                .Where(x => x.LoteEscrituracaoMiro.Codigo == codigoLote);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento> BuscarLoteDocumentosPorDatas(DateTime? datainicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento>()
                .Where(x => x.LoteEscrituracaoMiro.DataGeracaoLote >= datainicial && x.LoteEscrituracaoMiro.DataGeracaoLote <= dataFinal && string.IsNullOrEmpty(x.NumeroFolha));
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento BuscarLoteDocumentosPorDocumento(int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento>()
                .Where(x => x.ControleDocumento.Codigo == codigoDocumento);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico ObterCteVinculadoAoDocumento(string numeroMiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiroDocumento>()
                .Where(x => x.NumeroMiro == numeroMiro);
            return query.Select(x => x.ControleDocumento.CTe).FirstOrDefault();
        }


        #endregion

    }
}
