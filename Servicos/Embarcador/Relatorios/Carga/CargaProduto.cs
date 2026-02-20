using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaProduto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaPedidoProduto _repositorioCargaProduto;

        #endregion

        #region Construtores

        public CargaProduto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaProduto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaProduto.ConsultarRelatorioCargasProdutos(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaProduto.ContarConsultaRelatorioCargasProdutos(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargaProduto";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaProduto filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            if (filtrosPesquisa.DataInicial.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataInicial.HasValue ? $"{filtrosPesquisa.DataInicial.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataLimite.HasValue ? $"até {filtrosPesquisa.DataLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("Periodo", periodo, true));
            }
            else
                parametros.Add(new Parametro("Periodo", false));

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga);

                parametros.Add(new Parametro("Carga", carga.CodigoCargaEmbarcador, true));
            }
            else
                parametros.Add(new Parametro("Carga", false));

            if (filtrosPesquisa.CodigoPedido > 0)
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(filtrosPesquisa.CodigoPedido);

                parametros.Add(new Parametro("Pedido", pedido.NumeroPedidoEmbarcador, true));
            }
            else
                parametros.Add(new Parametro("Pedido", false));

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repositorioProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto);

                parametros.Add(new Parametro("Produto", produto.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Produto", false));

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);

                parametros.Add(new Parametro("Transportador", transportador.RazaoSocial, true));
            }
            else
                parametros.Add(new Parametro("Transportador", false));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente destinatario = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario);

                parametros.Add(new Parametro("Destinatario", destinatario.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Destinatario", false));

            if (filtrosPesquisa.ExibirCodigoBarras.HasValue && filtrosPesquisa.ExibirCodigoBarras.Value)
                parametros.Add(new Parametro("ExibirCodigoBarras", filtrosPesquisa.ExibirCodigoBarras));
            else
                parametros.Add(new Parametro("ExibirCodigoBarras", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }
    }
}
