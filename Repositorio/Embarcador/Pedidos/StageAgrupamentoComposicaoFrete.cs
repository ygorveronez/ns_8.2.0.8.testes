using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class StageAgrupamentoComposicaoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete>
    {
        public StageAgrupamentoComposicaoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete> BuscarPorAgrupamentoEComponente(int codigoAgrupamento, int codigoComponente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete>();

            var result = from obj in query where obj.StageAgrupamento.Codigo == codigoAgrupamento select obj;

            if (codigoComponente > 0)
                result = result.Where(x => x.ComponenteFrete.Codigo == codigoComponente);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete> BuscarPorAgrupamento(int CodigoAgrupamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete>();

            var result = from obj in query where obj.StageAgrupamento.Codigo == CodigoAgrupamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete> BuscarPorCodigosAgrupamentos(List<int> CodigosAgrupamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComposicaoFrete>();

            var result = from obj in query where CodigosAgrupamentos.Contains(obj.StageAgrupamento.Codigo) select obj;

            return result.ToList();
        }

        public void ExecuteDeletarPorAgrupamento(int codigoAgrupamentoStage)
        {
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_STAGE_AGRUPAMENTO_COMPOSICAO_FRETE WHERE STG_CODIGO = :codigoAgrupamentoStage;").SetInt32("codigoAgrupamentoStage", codigoAgrupamentoStage).ExecuteUpdate();
        }

    }
}
