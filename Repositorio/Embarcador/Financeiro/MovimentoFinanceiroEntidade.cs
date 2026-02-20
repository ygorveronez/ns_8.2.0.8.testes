using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class MovimentoFinanceiroEntidade : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade>
    {
        public MovimentoFinanceiroEntidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade BuscarPorMovimento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade>();
            var result = from obj in query where obj.MovimentoFinanceiro.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade> BuscarPorMotoristaAcerto(int codigo, int numeroAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade>();
            var result = from obj in query where obj.Motorista.Codigo == codigo && obj.MovimentoFinanceiro.Documento == numeroAcerto.ToString() && obj.MovimentoFinanceiro.TipoDocumentoMovimento == tipo select obj;
            return result.ToList();
        }

        public bool BuscarPorMotoristaAcertoRevertido(int codigo, int numeroAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade>();
            var result = from obj in query where obj.MovimentoFinanceiro.Observacao.Contains("REVERS") && obj.Motorista.Codigo == codigo && obj.MovimentoFinanceiro.Documento == numeroAcerto.ToString() && obj.MovimentoFinanceiro.TipoDocumentoMovimento == tipo select obj;
            return result.Any();
        }
    }
}
