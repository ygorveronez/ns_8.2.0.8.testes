using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class XMLMDFe : RepositorioBase<Dominio.Entidades.XMLMDFe>, Dominio.Interfaces.Repositorios.XMLMDFe
    {
        public XMLMDFe(UnitOfWork unidadeDeTrabalho) : base(unidadeDeTrabalho) { }

        public XMLMDFe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.XMLMDFe BuscarPorMDFe(int codigoMDFe, Dominio.Enumeradores.TipoXMLMDFe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.XMLMDFe> BuscarPorMDFeAsync(int codigoMDFe, Dominio.Enumeradores.TipoXMLMDFe tipo, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo select obj;
            return result.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// REMOVER APÓS CONCLUIR ATUALIZACOES
        /// </summary>
        public List<int> BuscarMDFesAutorizadosSemXML()
        {
            var queryMDFes = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var queryXMLs = this.SessionNHiBernate.Query<Dominio.Entidades.XMLMDFe>();

            var result = from mdfe in queryMDFes where (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado) && !(from xml in queryXMLs where xml.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Autorizacao select xml.MDFe.Codigo).Distinct().Contains(mdfe.Codigo) select mdfe.Codigo;

            return result.ToList();
        }

        /// <summary>
        /// REMOVER APÓS CONCLUIR ATUALIZACOES
        /// </summary>
        public List<int> BuscarMDFesCanceladosSemXML()
        {
            var queryMDFes = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var queryXMLs = this.SessionNHiBernate.Query<Dominio.Entidades.XMLMDFe>();

            var result = from mdfe in queryMDFes where mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado && !(from xml in queryXMLs where xml.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Cancelamento select xml.MDFe.Codigo).Distinct().Contains(mdfe.Codigo) select mdfe.Codigo;

            return result.ToList();
        }

        /// <summary>
        /// REMOVER APÓS CONCLUIR ATUALIZACOES
        /// </summary>
        public List<int> BuscarMDFesEncerradosSemXML()
        {
            var queryMDFes = this.SessionNHiBernate.Query<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();
            var queryXMLs = this.SessionNHiBernate.Query<Dominio.Entidades.XMLMDFe>();

            var result = from mdfe in queryMDFes where mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado && !(from xml in queryXMLs where xml.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Encerramento select xml.MDFe.Codigo).Distinct().Contains(mdfe.Codigo) select mdfe.Codigo;

            return result.ToList();
        }

        public List<Dominio.Entidades.XMLMDFe> BuscarPorMDFe(List<int> codigosMDFes, int codigoEmpresa, bool retornarEncerramento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLMDFe>();

            var result = from obj in query where codigosMDFes.Contains(obj.MDFe.Codigo) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.MDFe.Empresa.Codigo == codigoEmpresa);

            if (retornarEncerramento)
                result = result.Where(obj => (obj.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Autorizacao || obj.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Cancelamento || obj.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Encerramento));
            else
                result = result.Where(obj => (obj.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Autorizacao || obj.Tipo == Dominio.Enumeradores.TipoXMLMDFe.Cancelamento));

            return result.ToList();
        }

        public List<Dominio.Entidades.XMLMDFe> BuscarPorMDFeeTipo(int codigoMDFe, Dominio.Enumeradores.TipoXMLMDFe tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.XMLMDFe>();
            var result = from obj in query where obj.MDFe.Codigo == codigoMDFe && obj.Tipo == tipo select obj;
            return result.ToList();
        }
    }
}
