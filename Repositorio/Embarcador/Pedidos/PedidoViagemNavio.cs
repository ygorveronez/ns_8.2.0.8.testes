using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoViagemNavio : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>
    {
        public PedidoViagemNavio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio> ConsultarPentendeIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPentendeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.Count();
        }
        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();
            var result = from obj in query where obj.CodigoIntegracao == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio BuscarPorCodigoImo(string codigoImo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal direcao, int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();
            var result = from obj in query where obj.Navio.CodigoIMO == codigoImo && obj.DirecaoViagemMultimodal == direcao
                         && obj.NumeroViagem == numero select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio BuscarPorNumeroViagemDirecaoNavio(int numeroViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal direcao, string codigoIMO, string irin)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();

            var result = from obj in query
                         where obj.NumeroViagem == numeroViagem &&
                               obj.DirecaoViagemMultimodal == direcao &&
                               obj.Navio.CodigoIMO == codigoIMO &&
                               obj.Navio.Irin == irin
                         select obj;

            return result.FirstOrDefault();
        }
        public bool ExistePorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio> Consultar(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(descricao, codigoIntegracao, status);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = Consultar(descricao, codigoIntegracao, status);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio> Consultar(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(codigoIntegracao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);


            return result;
        }
    }
}
