using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Pedido;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pedido
{
    public class PedidoOcorrencia : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoOcorrencia, Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrencia>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega _repPedidoOcorrencia;

        #endregion

        #region Construtores

        public PedidoOcorrencia(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repPedidoOcorrencia = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoOcorrencia> ConsultarRegistros(FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repPedidoOcorrencia.ConsultarRelatorioPedidoOcorrencia(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repPedidoOcorrencia.ContarConsultaRelatorioPedidoOcorrencia(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pedidos/PedidoOcorrencia";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioPedidoOcorrencia filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repositorioRemetente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = filtrosPesquisa.CodigoTipoOcorrencia > 0 ? repositorioTipoOcorrencia.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOcorrencia) : null;
            Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CpfCnpjRemetente > 0d ? repositorioRemetente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjRemetente) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFiliais.Count > 0 ? repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais) : null;
            Dominio.Entidades.Localidade origem = filtrosPesquisa.CodigoOrigem > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoOrigem) : null;
            Dominio.Entidades.Localidade destino = filtrosPesquisa.CodigoDestino > 0 ? repositorioLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoDestino) : null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega situacaoEntrega = filtrosPesquisa.SituacaoEntrega != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega)filtrosPesquisa.SituacaoEntrega : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue;

            parametros.Add(new Parametro("NumeroPedido", filtrosPesquisa.NumeroPedido));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("DataOcorrencia", filtrosPesquisa.DataOcorrenciaInicial, filtrosPesquisa.DataOcorrenciaFinal));
            parametros.Add(new Parametro("TipoOcorrencia", tipoOcorrencia?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Transportador", transportador?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Remetente", remetente?.Descricao ?? string.Empty));
            parametros.Add(new Parametro("Filial", filiais != null ? string.Join(", ", filiais.Select(o => o.Descricao)) : null));
            parametros.Add(new Parametro("Origem", origem?.DescricaoCidadeEstado ?? string.Empty));
            parametros.Add(new Parametro("Destino", destino?.DescricaoCidadeEstado ?? string.Empty));
            parametros.Add(new Parametro("NumeroNotaFiscal", filtrosPesquisa.NumeroNotaFiscal > 0 ? filtrosPesquisa.NumeroNotaFiscal.ToString() : string.Empty));
            parametros.Add(new Parametro("SituacaoEntrega", filtrosPesquisa.SituacaoEntrega != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterDescricao(situacaoEntrega) : string.Empty));

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
