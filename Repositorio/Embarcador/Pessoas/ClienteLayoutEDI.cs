using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteLayoutEDI : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>
    {
        public ClienteLayoutEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> BuscarPorPessoa(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();

            var result = from obj in query where obj.Cliente.CPF_CNPJ == cpfCnpj select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> BuscarDisponiveisParaLeituraFTP()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();

            query = query.Where(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP && o.UtilizarLeituraArquivos && o.Cliente.Ativo);

            return query.ToList();
        }
    }
}
