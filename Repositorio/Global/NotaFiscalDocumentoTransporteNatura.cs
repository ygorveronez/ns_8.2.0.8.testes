using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class NotaFiscalDocumentoTransporteNatura: RepositorioBase<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>, Dominio.Interfaces.Repositorios.NotaFiscalDocumentoTransporteNatura
    {
        public NotaFiscalDocumentoTransporteNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> BuscarPorDocumentoTransporte(int codigoDocumentoTransporte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.Codigo == codigoDocumentoTransporte select obj;

            return result.ToList();
        }

        public string BuscarNotasPorDocumentoTransporte(int codigoDocumentoTransporte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.Codigo == codigoDocumentoTransporte select obj;

            return string.Join(", ", (from obj in result select obj.Numero));
        }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura Buscar(int codigoEmpresa, string chaveCTe, long numeroDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.NumeroDT == numeroDT && obj.CTe.Chave == chaveCTe select obj; //Retirado filtro para voltar todos CTes independente da Empresa:  && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura Buscar(int codigoEmpresa, int numeroNFSe, int serieNFSe, long numeroDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.NumeroDT == numeroDT && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa && obj.NFSe.Numero == numeroNFSe && obj.NFSe.Serie.Numero == serieNFSe select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura BuscarNumeroSerieCTe(int codigoEmpresa, int serie, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa && obj.CTe.Numero == numero select obj;
            if (serie > 0)
            {
                result = result.Where(obj => obj.CTe.Serie.Numero == serie);
            }

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura BuscarPorChaveNFe(int codigoEmpresa, string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.Chave == chaveNFe && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura BuscarPorDTeChaveNFe(int codigoEmpresa, long numeroDT, string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.NumeroDT == numeroDT && obj.Chave == chaveNFe && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa && obj.DocumentoTransporte.Status != Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Cancelado select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura BuscarPorCTe(int codigoEmpresa, int Cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.CTe.Codigo == Cte && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> BuscarListaNotasPorCTe(int codigoEmpresa, int Cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.CTe.Codigo == Cte && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> BuscarListaNotasPorNFSe(int codigoEmpresa, int nfse)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.NFSe.Codigo == nfse && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa select obj;

            return result.ToList();
        }

        public Dominio.Entidades.NotaFiscalDocumentoTransporteNatura BuscarPorChaveCTe(int codigoEmpresa, string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.CTe.Chave == chaveCTe && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public int VerificarNumeroNotasSemCTe(int codigoEmpresa, int codigoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();

            var result = from obj in query where obj.DocumentoTransporte.Codigo == codigoDT && obj.DocumentoTransporte.Empresa.Codigo == codigoEmpresa && obj.CTe == null select obj;

            return result.Count();
        }


    }
}
