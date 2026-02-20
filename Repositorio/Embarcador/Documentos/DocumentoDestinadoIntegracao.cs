using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Documentos
{
    public class DocumentoDestinadoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>
    {
        #region Constructor
        public DocumentoDestinadoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> BuscarIntegracoesPendentes(int quantideRegistro, int numeroTentativas, int minutosACadaTentativa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();

            query = from obj in query
                    where obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa) || ((obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas <= numeroTentativas)
                    || (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && (obj.ExisteXmlCompletoDocumentoDestinado == null || obj.ExisteXmlCompletoDocumentoDestinado == false) && obj.DataIntegracao <= DateTime.Now.AddMinutes(-120) && obj.NumeroTentativas <= (numeroTentativas + numeroTentativas)))

                    select obj;

            return query.Take(quantideRegistro).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> ObterIntegracoesPorDocumento(int codigoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();

            query = from obj in query
                    where obj.DocumentoDestinadoEmpresa.Codigo == codigoDocumento
                    select obj;

            return query.ToList();

        }

 
        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao BuscarPorChaveTipoDocumentoDestinado(string chave, TipoDocumentoDestinadoEmpresa tipoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();

            query = from obj in query
                    where obj.DocumentoDestinadoEmpresa.Chave == chave && obj.DocumentoDestinadoEmpresa.TipoDocumento == tipoDocumento
                    select obj;

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao BuscarPorCodigoETipoDocumentoDestinado(int codigo, TipoDocumentoDestinadoEmpresa tipoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();

            TipoIntegracao tipoIntegracao = tipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoNFe || tipoDocumento == TipoDocumentoDestinadoEmpresa.CancelamentoCTe ? TipoIntegracao.UnileverStatus : TipoIntegracao.UnileverXml;

            query = from obj in query
                    where obj.DocumentoDestinadoEmpresa.Codigo == codigo && obj.DocumentoDestinadoEmpresa.TipoDocumento == tipoDocumento && obj.TipoIntegracao.Tipo == tipoIntegracao
                    select obj;

            return query.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> BuscarPorChaveDocumentoDestinado(string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();

            query = from obj in query
                    where obj.DocumentoDestinadoEmpresa.Chave == chave
                    select obj;

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao BuscarPorCodigoDocumentoDestinado(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();

            query = from obj in query
                    where obj.DocumentoDestinadoEmpresa.Codigo == codigo
                    select obj;

            return query.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> BuscarPorCodigoDocumentoDestinadoEmpresa(List<long> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>();

            query = from obj in query
                    where codigos.Contains(obj.DocumentoDestinadoEmpresa.Codigo)
                    select obj;

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao>()
                .Where(o => o.ArquivosTransacao.Any(a => a.Codigo == codigoArquivo));
            return query.FirstOrDefault();
        }

        #endregion
    }
}
