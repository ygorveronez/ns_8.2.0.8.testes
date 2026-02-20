using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class PedidoProduto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto, Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoProduto>
    {
        #region Atributos
        private readonly Repositorio.Embarcador.Pedidos.PedidoProduto _repositorioPedidoProduto;

        #endregion

        #region Construtores
        public PedidoProduto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoProduto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPedidoProduto.ConsultarRelatorioPedidoProduto(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPedidoProduto.ContarConsultaRelatorioPedidoProduto(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/PedidoProduto";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoProduto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            if (filtrosPesquisa.DataInicial != null || filtrosPesquisa.DataFinal != null)
            {
                string data = "";
                data += filtrosPesquisa.DataInicial != null ? filtrosPesquisa.DataInicial.Value.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinal != null ? "até " + filtrosPesquisa.DataFinal.Value.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Parametro("Data", data, true));
            }
            else
                parametros.Add(new Parametro("Data", false));

            if (filtrosPesquisa.Remetente > 0)
            {
                Dominio.Entidades.Cliente _pessoa = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Remetente);
                parametros.Add(new Parametro("Remetente", _pessoa.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Remetente", false));

            if (filtrosPesquisa.Destinatario > 0)
            {
                Dominio.Entidades.Cliente _pessoa = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Destinatario);
                parametros.Add(new Parametro("Destinatario", _pessoa.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Destinatario", false));

            if (filtrosPesquisa.CodigosProduto != null && filtrosPesquisa.CodigosProduto.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtos = filtrosPesquisa.CodigosProduto.Count > 0 ? repProduto.BuscarPorCodigo(filtrosPesquisa.CodigosProduto.ToArray()) : new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
                parametros.Add(new Parametro("Produto", string.Join(", ", from obj in produtos select obj.Descricao), true));
            }
            else
                parametros.Add(new Parametro("Produto", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
                parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar, true));
            else
                parametros.Add(new Parametro("Agrupamento", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataPedido")
                return "DataPedido";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
