using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class PagamentoMotoristaTipo : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>
    {
        public PagamentoMotoristaTipo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();
            var result = from obj in query where obj.CodigoIntegracaoImportacao.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo BuscarPrimeiroPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();
            var result = from obj in query where obj.Ativo == true && obj.TipoPagamentoMotorista == tipo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo BuscarPrimeiroParaGeracaoSaldo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();
            var result = from obj in query where obj.Ativo == true && obj.UtilizarEstePagamentoParaGeracaoPagamentoValorSaldo == true select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string descricao, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);

            return result.Count();
        }
    }
}
