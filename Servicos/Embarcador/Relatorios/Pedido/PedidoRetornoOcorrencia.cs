using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pedido
{
    public class PedidoRetornoOcorrencia : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoRetornoOcorrencia, Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoRetornoOcorrencia>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.Pedido _repPedido;

        #endregion

        #region Construtores

        public PedidoRetornoOcorrencia(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoRetornoOcorrencia> ConsultarRegistros(FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repPedido.ConsultarRelatorioPedidoRetornoOcorrencia(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repPedido.ContarConsultaRelatorioPedidoRetornoOcorrencia(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pedidos/PedidoRetornoOcorrencia";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPedidoRetornoOcorrencia filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("NumeroPedido", filtrosPesquisa.NumeroPedido));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            
            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "SituacaoEntregaDescricao")
                return "SituacaoEntrega";

            return propriedadeOrdenarOuAgrupar;
        }
    }
}
