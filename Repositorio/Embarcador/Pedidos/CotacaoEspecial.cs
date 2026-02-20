using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Pedidos
{
    public class CotacaoEspecial : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial>
    {
        public CotacaoEspecial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial> ConsultarParaCotacaoEspecial(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaCotacaoEspecial filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial> result = query;

            if (filtrosPesquisa.CodigoFornecedor > 0)
                result = result.Where(o => o.Usuario.Cliente.CPF_CNPJ == filtrosPesquisa.CodigoFornecedor);

            if (filtrosPesquisa.DataCotacaoInicial != DateTime.MinValue || filtrosPesquisa.DataCotacaoFinal != DateTime.MinValue)
            {
                result = result.Where(o => o.DataSolicitacao.HasValue && (filtrosPesquisa.DataCotacaoInicial == DateTime.MinValue || o.DataSolicitacao.Value.Date >= filtrosPesquisa.DataCotacaoInicial.Value) &&
                    (filtrosPesquisa.DataCotacaoFinal == DateTime.MinValue || o.DataSolicitacao.Value.Date <= filtrosPesquisa.DataCotacaoFinal.Value));
            }

            if (filtrosPesquisa.NumeroCotacao > 0)
                result = result.Where(o => o.Codigo == filtrosPesquisa.NumeroCotacao);

            if (filtrosPesquisa.StatusCotacaoEspecial.HasValue && filtrosPesquisa.StatusCotacaoEspecial != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoEspecial.Todos)
                result = result.Where(o => o.StatusCotacaoEspecial == filtrosPesquisa.StatusCotacaoEspecial.Value);

            if (filtrosPesquisa.TipoModal.HasValue && filtrosPesquisa.TipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalCotacaoEspecial.Todos)
                result = result.Where(o => o.TipoModal == filtrosPesquisa.TipoModal.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                result = result.Where(o => o.Pedidos.Any(p => p.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedido));

            return result;
        }


        public int ContarConsultaParaCotacaoEspecial(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaCotacaoEspecial filtrosPesquisa)
        {
            var result = ConsultarParaCotacaoEspecial(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.CotacaoEspecial> ConsultarParaCotacaoEspecial(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaCotacaoEspecial filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = ConsultarParaCotacaoEspecial(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

    }
}
