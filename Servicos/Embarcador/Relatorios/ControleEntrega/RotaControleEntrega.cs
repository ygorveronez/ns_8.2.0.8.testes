using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.ControleEntrega
{
    public class RotaControleEntrega : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega, Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RotaControleEntrega>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.RotaControleEntrega _repositorioRotaControleEntrega;

        #endregion

        #region Construtores

        public RotaControleEntrega(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioRotaControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.RotaControleEntrega(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RotaControleEntrega> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioRotaControleEntrega.ConsultarRelatorioRotaControleEntrega(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioRotaControleEntrega.ContarConsultaRelatorioRotaControleEntrega(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/RotaControleEntrega";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Cliente> listaDestinatario = filtrosPesquisa.CpfCnpjDestinatarios?.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatarios) : null;
            List<Dominio.Entidades.Cliente> listaEmitente = filtrosPesquisa.CpfCnpjEmitentes?.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjEmitentes) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFiliais = filtrosPesquisa.CodigosFilial?.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : null;
            List<Dominio.Entidades.Empresa> listaTransportadores = filtrosPesquisa.CodigosTransportador?.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportador) : null;
            List<Dominio.Entidades.Veiculo> listaVeiculos = filtrosPesquisa.CodigosVeiculo?.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculo) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = filtrosPesquisa.NumerosPedido?.Count > 0 ? repPedido.BuscarPorCodigos(filtrosPesquisa.NumerosPedido) : null;
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> listaNotasFiscais = filtrosPesquisa.NumeroNotasFiscais?.Count > 0 ? repNotaFiscal.BuscarPorCodigo(filtrosPesquisa.NumeroNotasFiscais) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCarga", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntregaPedido", filtrosPesquisa.DataEntregaPedidoInicial, filtrosPesquisa.DataEntregaPedidoFinal, false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPrevisaoEntregaPedido", filtrosPesquisa.DataPrevisaoEntregaPedidoInicial, filtrosPesquisa.DataPrevisaoEntregaPedidoFinal, false));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa.CodigoCargaEmbarcador));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", listaFiliais != null ? string.Join(", ", listaFiliais?.Select(o => o.Descricao)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", listaVeiculos != null ? string.Join(", ", listaVeiculos?.Select(o => o.Placa)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", listaTransportadores != null ? string.Join(", ", listaTransportadores?.Select(o => o.RazaoSocial)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", listaDestinatario != null ? string.Join(", ", listaDestinatario?.Select(o => o.Descricao)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Emitente", listaEmitente != null ? string.Join(", ", listaEmitente?.Select(o => o.Descricao)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", listaPedidos != null ? string.Join(", ", listaPedidos?.Select(o => o.Numero)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NotaFiscal", listaNotasFiscais != null ? string.Join(", ", listaNotasFiscais?.Select(o => o.Numero)) : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CnpjTransportadorFormatado")
                return "CnpjTransportador";

            if (propriedadeOrdenarOuAgrupar == "DataAtualizacaoFormatada")
                return "DataAtualizacao";

            if (propriedadeOrdenarOuAgrupar == "DataValidadeGerenciadoraRiscoFormatada")
                return "DataValidadeGerenciadoraRisco";

            if (propriedadeOrdenarOuAgrupar == "DataAquisicaoFormatada")
                return "DataAquisicao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}