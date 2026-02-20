using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class DocumentoDestinadoXml : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml>
    {
        #region Construtores

        public DocumentoDestinadoXml(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public bool ExistePorNSU(int codigoEmpresa, long nsu)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa && o.NumeroSequencialUnico == nsu);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml BuscaPorNSU(int codigoEmpresa, long nsu)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa && o.NumeroSequencialUnico == nsu);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml> BuscaPorSituacaoSincronizando()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoXml>();

            query = query.Where(o => o.SituacaoXml == SituacaoXml.Sincronizando);

            return query.ToList();
        }

        #endregion

        #region Métodos Privados        

        #endregion
    }
}
