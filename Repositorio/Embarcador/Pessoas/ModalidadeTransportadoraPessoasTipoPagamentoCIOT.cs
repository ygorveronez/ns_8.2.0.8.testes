using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class ModalidadeTransportadoraPessoasTipoPagamentoCIOT : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT>
    {
        public ModalidadeTransportadoraPessoasTipoPagamentoCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT> BuscarPorModalidadeTransportador(int codigo)
        {
            var consultaTipoPagamentoCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT>()
                .Where(tipoPagamentoCIOT => tipoPagamentoCIOT.ModalidadeTransportadoraPessoas.Codigo == codigo);

            return consultaTipoPagamentoCIOT.ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? BuscarTipoPagamentoPorOperadora(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT operadora)
        {
            var consultaTipoPagamentoCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT>()
                .Where(tipoPagamentoCIOT => tipoPagamentoCIOT.ModalidadeTransportadoraPessoas.Codigo == codigo && tipoPagamentoCIOT.Operadora == operadora);

            return consultaTipoPagamentoCIOT.FirstOrDefault()?.TipoPagamentoCIOT ?? null;
        }

        public bool ExistePorModalidadeTransportador(int codigo)
        {
            var consultaTipoPagamentoCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT>()
                .Where(tipoPagamentoCIOT => tipoPagamentoCIOT.ModalidadeTransportadoraPessoas.Codigo == codigo);

            return consultaTipoPagamentoCIOT.Any();
        }
    }
}
