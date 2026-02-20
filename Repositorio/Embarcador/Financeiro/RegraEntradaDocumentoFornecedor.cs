using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class RegraEntradaDocumentoFornecedor : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor>
    {
        public RegraEntradaDocumentoFornecedor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<(int CodigoRegra, double CpfCnpjFornecedor)> BuscarAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor>();
            query = query.Where(obj => obj.RegraEntradaDocumento.Ativo == true);

            return query
                .Select(o => ValueTuple.Create(o.RegraEntradaDocumento.Codigo, o.Pessoa.CPF_CNPJ))
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor>();
            var result = from obj in query where obj.RegraEntradaDocumento.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
