using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TipoPagamentoRecebimento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento>
    {
        public TipoPagamentoRecebimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento BuscarFormaAVista()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento>();
            var result = from obj in query where obj.Descricao.Contains("VISTA") select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento BuscarPorPlanoContas(int codigoPlano, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento>();
            var result = from obj in query where obj.PlanoConta.Codigo == codigoPlano select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento> Consultar(int codigoEmpresa, string descricao, int codigoPlanoConta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);
            
            if (codigoPlanoConta > 0)
                result = result.Where(obj => obj.PlanoConta.Codigo == codigoPlanoConta);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoEmpresa, string descricao, int codigoPlanoConta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (codigoPlanoConta > 0)
                result = result.Where(obj => obj.PlanoConta.Codigo == codigoPlanoConta);

            return result.Count();
        }
    }
}
