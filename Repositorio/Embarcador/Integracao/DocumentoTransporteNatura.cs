using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class DocumentoTransporteNatura : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.DTNatura>
    {
        public DocumentoTransporteNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.DTNatura BuscarPorNumero(int codigoEmpresa, long numero, bool? geradoPorNOTFIS)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.DTNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa && obj.Numero == numero && !obj.GeradoPorNOTFIS);

            if (geradoPorNOTFIS.HasValue)
                query = query.Where(o => o.GeradoPorNOTFIS == geradoPorNOTFIS);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.DTNatura BuscarPorNumero(long numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            var result = from obj in query where obj.Numero == numero select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.DTNatura> BuscarTodosPorNumero(long numero, bool? geradoPorNOTFIS)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.DTNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            query = query.Where(obj => obj.Numero == numero);

            if (geradoPorNOTFIS.HasValue)
                query = query.Where(obj => obj.GeradoPorNOTFIS == geradoPorNOTFIS);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.DTNatura> Consultar(long numeroDocumentoTransporte, int numeroNotaFiscal, DateTime dataInicial, DateTime dataFinal, bool semCarga, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.DTNatura> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            if (numeroDocumentoTransporte > 0L)
                query = query.Where(o => o.Numero == numeroDocumentoTransporte);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.Data >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.Data <= dataFinal.AddDays(1).Date);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.Numero == numeroNotaFiscal));

            if (semCarga)
                query = query.Where(o => !o.Cargas.Any(c => c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                                            c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada));

            if (!string.IsNullOrWhiteSpace(propOrdena) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propOrdena + " " + dirOrdena);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.Fetch(o => o.Recebedor).Fetch(o => o.Empresa).ToList();
        }

        public Dominio.Entidades.Embarcador.Integracao.DTNatura BuscarPorCodigo(int codigoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            var result = from obj in query where obj.Codigo == codigoDT select obj;

            return result.FirstOrDefault();
        }

        public int ContarConsulta(long numeroDocumentoTransporte, int numeroNotaFiscal, DateTime dataInicial, DateTime dataFinal, bool semCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            if (numeroDocumentoTransporte > 0L)
                query = query.Where(o => o.Numero == numeroDocumentoTransporte);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.Data >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.Data <= dataFinal.AddDays(1).Date);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.Numero == numeroNotaFiscal));

            if (semCarga)
                query = query.Where(o => !o.Cargas.Any(c => c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                                            c.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada));

            return query.Count();
        }

        public decimal BuscarValorFrete(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            query = query.Where(o => o.Cargas.Any(c => c.Carga.Codigo == codigoCarga));

            return query.Sum(o => (decimal?)o.ValorFrete) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.DTNatura> Consultar(long numeroDocumentoTransporte, int numeroNF, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada };

            query = query.Where(o => !o.Cargas.Any(c => !situacoesPermitidas.Contains(c.Carga.SituacaoCarga)) && o.Status);

            if (numeroDocumentoTransporte > 0L)
                query = query.Where(o => o.Numero == numeroDocumentoTransporte);

            if (numeroNF > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.Numero == numeroNF));

            return query.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(long numeroDocumentoTransporte, int numeroNF)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.DTNatura>();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada };

            query = query.Where(o => !o.Cargas.Any(c => !situacoesPermitidas.Contains(c.Carga.SituacaoCarga)) && o.Status);

            if (numeroDocumentoTransporte > 0L)
                query = query.Where(o => o.Numero == numeroDocumentoTransporte);

            if (numeroNF > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.Numero == numeroNF));

            return query.Count();
        }
    }
}
