using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class ContainerTipo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>
    {
        public ContainerTipo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ContainerTipo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public int DeletarPorCodigo(int codigo)
        {
            var query = UnitOfWork.Sessao.CreateQuery("DELETE FROM ContainerTipo c WHERE c.Codigo = :codigo").SetInt32("codigo", codigo);

            return query.SetTimeout(240).ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo> ConsultarPentendeIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPentendeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.Count();
        }

        public List<string> BuscarDescricoes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();

            var result = from obj in query where obj.Status == true && obj.Descricao != null select obj;

            return result.Select(o => o.Descricao).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo> BuscarPorListaDeCodigo(List<int> Codigos)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>().Where(x => Codigos.Contains(x.Codigo)).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ContainerTipo BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();

            var result = from obj in query where obj.Descricao == descricao && obj.Status == true select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ContainerTipo BuscarTodosPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();
            var result = from obj in query where obj.CodigoIntegracao == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.ContainerTipo BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();
            var result = from obj in query where obj.CodigoIntegracao == codigo && obj.Status == true select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo> Consultar(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(descricao, codigoIntegracao, status);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = Consultar(descricao, codigoIntegracao, status);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo> Consultar(string descricao, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo>();

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

        public List<(int CodigoCarga, Dominio.Entidades.Embarcador.Pedidos.ContainerTipo TipoContainer)> BuscarPorCargasPedido(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return query
                .Select(obj => ValueTuple.Create(obj.Carga.Codigo, obj.Pedido.Container.ContainerTipo)).ToList();
        }
    }
}
