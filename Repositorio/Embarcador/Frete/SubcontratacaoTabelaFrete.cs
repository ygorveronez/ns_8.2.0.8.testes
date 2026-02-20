using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class SubcontratacaoTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete>
    {
        public SubcontratacaoTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete> BuscarPorTabelaFrete(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == tabelaFrete select obj;

            return result.Fetch(obj => obj.Pessoa).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete BuscarPorTabelaETerceiro(int codigoTabelaFrete, double cnpjTerceiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete>();

            var result = from obj in query where obj.TabelaFrete.Codigo == codigoTabelaFrete && obj.Pessoa.CPF_CNPJ == cnpjTerceiro select obj;

            return result.FirstOrDefault();
        }
    }
}
