using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class DocumentoManifestoAvon : RepositorioBase<Dominio.Entidades.DocumentoManifestoAvon>, Dominio.Interfaces.Repositorios.DocumentoManifestoAvon
    {
        public DocumentoManifestoAvon(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.DocumentoManifestoAvon> ConsultarRetornos(int codManifesto, Dominio.Enumeradores.StatusDocumentoManifestoAvon statusDocumentoManifestoAvon, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Manifesto.Codigo == codManifesto);

            if (statusDocumentoManifestoAvon != Dominio.Enumeradores.StatusDocumentoManifestoAvon.Todas)
                result = result.Where(obj => obj.Status == statusDocumentoManifestoAvon);


            string propOrdenacao = "Status";
            string dirOrdenacao = "desc";

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            return result.OrderBy("Codigo descending").Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsultaRetornos(int codManifesto, Dominio.Enumeradores.StatusDocumentoManifestoAvon statusDocumentoManifestoAvon)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.Manifesto.Codigo == codManifesto);

            if (statusDocumentoManifestoAvon != Dominio.Enumeradores.StatusDocumentoManifestoAvon.Todas)
                result = result.Where(obj => obj.Status == statusDocumentoManifestoAvon);

            return result.Count();
        }


        public List<Dominio.Entidades.DocumentoManifestoAvon> BuscarPorManifesto(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto select obj;

            return result.Fetch(o => o.CTe).ToList();
        }


        public List<Dominio.Entidades.DocumentoManifestoAvon> ConsultarDocumentosParaRetorno(int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query select obj;
            //result = result.Where(obj => obj.CTe.Status == "A" && obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido || (obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno && obj.NumeroTentativas < 3));
            result = result.Where(obj => obj.CTe.Status == "A" && obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.Emitido);

            return result.OrderBy("Codigo ascending").Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorStatus(int codigoManifesto, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto && obj.CTe.Status.Equals(statusCTe) select obj.CTe;

            return result.ToList();
        }

        public Dominio.Entidades.DocumentoManifestoAvon BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentoManifestoAvon> BuscarPorManifesto(int codigoManifesto, Dominio.Enumeradores.StatusDocumentoManifestoAvon? status = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto && obj.Status == status select obj;

            if (status != null)
            {
                result = result.Where(obj => obj.Status == status);
            }


            return result.ToList();
        }

        public int ContarPorManifesto(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto select obj;

            return result.Count();
        }

        public decimal ObterValorDosCTesPorManifesto(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto select obj.CTe.ValorPrestacaoServico;

            return result.Sum();
        }

        public decimal ObterValorDosCTesPorManifesto(int[] codigosManifestos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where codigosManifestos.Contains(obj.Manifesto.Codigo) select obj.CTe.ValorPrestacaoServico;

            return result.Sum();
        }

        public int ContarPorManifesto(int codigoManifesto, Dominio.Enumeradores.StatusDocumentoManifestoAvon status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto && obj.Status == status select obj;

            return result.Count();
        }

        public int ContarPorManifesto(int[] codigosManifestos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where codigosManifestos.Contains(obj.Manifesto.Codigo) select obj;

            return result.Count();
        }

        public Dominio.Entidades.DocumentoManifestoAvon BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.DocumentoManifestoAvon> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public int ContarPorManifestoEStatusDiff(int codigoManifesto, Dominio.Enumeradores.StatusDocumentoManifestoAvon statusDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto && obj.Status != statusDiff select obj;

            return result.Count();
        }

        public int ContarPorManifestoEStatusProblema(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            //var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto && obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno && obj.NumeroTentativas > 2 select obj;
            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto && obj.Status == Dominio.Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno select obj;
            return result.Count();
        }



        public int ContarPorManifestoEStatusDiff(int[] codigosManifestos, Dominio.Enumeradores.StatusDocumentoManifestoAvon statusDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where codigosManifestos.Contains(obj.Manifesto.Codigo) && obj.Status != statusDiff select obj;

            return result.Count();
        }

        public string ObterCNPJRemetente(int codigoManifesto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where obj.Manifesto.Codigo == codigoManifesto select obj.CTe.Remetente.CPF_CNPJ;

            return result.FirstOrDefault();
        }

        public List<string> ObterChaveDosCTes(int[] codigosManifestos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();

            var result = from obj in query where codigosManifestos.Contains(obj.Manifesto.Codigo) select obj.CTe.Chave;

#if DEBUG
            result = result.Where(o => o != string.Empty && o != null);
#endif

            return result.ToList();
        }

        public List<Dominio.Entidades.XMLCTe> ObterXMLCTes(int codigoFatura)
        {
            var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();
            var queryXMLCTe = this.SessionNHiBernate.Query<Dominio.Entidades.XMLCTe>();

            var result = from xml in queryXMLCTe where (from doc in queryDocumentos where doc.Manifesto.Faturas.Contains(new Dominio.Entidades.FaturaAvon() { Codigo = codigoFatura }) select doc.CTe.Codigo).Contains(xml.CTe.Codigo) select xml;

            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ObterCTes(int codigoFatura)
        {
            var queryDocumentos = this.SessionNHiBernate.Query<Dominio.Entidades.DocumentoManifestoAvon>();
        var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

        var result = from cte in queryCTe where (from doc in queryDocumentos where doc.Manifesto.Faturas.Contains(new Dominio.Entidades.FaturaAvon() { Codigo = codigoFatura }) select doc.CTe.Codigo).Contains(cte.Codigo) select cte;

            return result.ToList();
        }
}
}
