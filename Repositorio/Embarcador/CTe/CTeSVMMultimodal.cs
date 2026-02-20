using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    public class CTeSVMMultimodal : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>
    {
        public CTeSVMMultimodal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal> BuscarPorCTeMultiModal(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
            var result = from obj in query where obj.CTeMultimodal.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal> BuscarPorCTeSVM(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
            var result = from obj in query where obj.CTeSVM.Codigo == codigo select obj;
            return result.ToList();
        }

        public bool PossuiCTePendenteSVM(int codigoCarga)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.Carga.Codigo == codigoCarga && c.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            if (queryCargaCTe.Any())
            {
                var querySVM = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
                queryCargaCTe = queryCargaCTe.Where(c => !querySVM.Any(s => s.CTeSVM.Status == "A" && s.CTeMultimodal.Codigo == c.CTe.Codigo) || !querySVM.Any(s => s.CTeMultimodal.Codigo == c.CTe.Codigo));
                return queryCargaCTe.Any();
            }
            else
                return true;
        }

        public bool PossuiSVMPendenteAutorizacao(int codigoCarga)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.Carga.Codigo == codigoCarga);

            var querySVM = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
            queryCargaCTe = queryCargaCTe.Where(c => querySVM.Any(s => s.CTeSVM.Status == "R" && s.CTeMultimodal.Codigo == c.CTe.Codigo));
            return queryCargaCTe.Any();
        }

        public List<int> BuscarCodigosCTesAutorizadosPorCarga(int codigoCarga)
        {
            Dominio.Enumeradores.TipoDocumento[] tiposDocumentosAutorizados = new Dominio.Enumeradores.TipoDocumento[]
            {
                 Dominio.Enumeradores.TipoDocumento.CTe,
                  Dominio.Enumeradores.TipoDocumento.Outros
            };

            string[] statusPermitido = new string[] { "A", "C", "Z" };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();

            query = query.Where(o => o.CargaMultimodal.Codigo == codigoCarga && tiposDocumentosAutorizados.Contains(o.CTeSVM.ModeloDocumentoFiscal.TipoDocumentoEmissao) && statusPermitido.Contains(o.CTeSVM.Status));

            query = query.OrderBy("CTeSVM.Numero");
            return query.Select(o => o.CTeSVM.Codigo).ToList();
        }

        public List<int> BuscarCodigosNFSePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();

            query = query.Where(o => o.CargaMultimodal.Codigo == codigoCarga && o.CTeSVM.ModeloDocumentoFiscal.Numero == "39");

            return query.Select(o => o.CTeSVM.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal> ConsultarSVM(int carga, string statusCTe, int numeroNF, int numeroDocumento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.Carga.Codigo == carga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
            query = query.Where(c => queryCargaCTe.Any(e => e.CTe.Codigo == c.CTeMultimodal.Codigo));

            if (!string.IsNullOrWhiteSpace(statusCTe))
                query = query.Where(c => c.CTeSVM.Status == statusCTe);

            if (numeroDocumento > 0)
                query = query.Where(o => o.CTeSVM.Numero == numeroDocumento);

            if (numeroNF > 0)
                query = query.Where(obj => obj.CTeSVM.Documentos.Any(doc => doc.Numero == numeroNF.ToString()));

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarSVM(int carga, string statusCTe, int numeroNF, int numeroDocumento)
        {
            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(c => c.Carga.Codigo == carga);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeSVMMultimodal>();
            query = query.Where(c => queryCargaCTe.Any(e => e.CTe.Codigo == c.CTeMultimodal.Codigo));

            if (!string.IsNullOrWhiteSpace(statusCTe))
                query = query.Where(c => c.CTeSVM.Status == statusCTe);

            if (numeroDocumento > 0)
                query = query.Where(o => o.CTeSVM.Numero == numeroDocumento);

            if (numeroNF > 0)
                query = query.Where(obj => obj.CTeSVM.Documentos.Any(doc => doc.Numero == numeroNF.ToString()));

            return query.Count();
        }
    }
}
