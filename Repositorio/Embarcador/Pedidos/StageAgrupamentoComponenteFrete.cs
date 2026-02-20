using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class StageAgrupamentoComponenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete>
    {
        public StageAgrupamentoComponenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete> BuscarPorAgrupamentoEComponente(int CodigoAgrupamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponente, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete>();

            var result = from obj in query where obj.StageAgrupamento.Codigo == CodigoAgrupamento && obj.TipoComponenteFrete == tipoComponente select obj;
            if (componente != null)
                result = result.Where(obj => obj.ComponenteFrete.Codigo == componente.Codigo);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete> BuscarPorAgrupamento(int CodigoAgrupamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete>();

            var result = from obj in query where obj.StageAgrupamento.Codigo == CodigoAgrupamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete> BuscarPorCodigosAgrupamentos(List<int> CodigosAgrupamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete>();

            var result = from obj in query where CodigosAgrupamentos.Contains(obj.StageAgrupamento.Codigo) select obj;

            return result.ToList();
        }


        public void ExecuteDeletarPorAgrupamento(int codigoAgrupamentoStage)
        {
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_STAGE_AGRUPAMENTO_COMPONENTE_FRETE WHERE STG_CODIGO = :codigoAgrupamentoStage;").SetInt32("codigoAgrupamentoStage", codigoAgrupamentoStage).ExecuteUpdate();
        }
    }
}
