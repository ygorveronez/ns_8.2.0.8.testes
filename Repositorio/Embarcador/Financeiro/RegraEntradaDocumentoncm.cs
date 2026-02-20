using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class RegraEntradaDocumentoNCM : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM>
    {
        public RegraEntradaDocumentoNCM(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public List<(int CodigoRegra, string Ncm)> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM>();
            query = query.Where(obj => obj.RegraEntradaDocumento.Ativo == true);

            return query
                .Select(obj => ValueTuple.Create(obj.RegraEntradaDocumento.Codigo, obj.NCM))
                .ToList();
        }
        public List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM>();
            var result = from obj in query where obj.RegraEntradaDocumento.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
