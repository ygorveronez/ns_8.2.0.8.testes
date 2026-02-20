using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class Paradas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioParadas, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.Paradas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega _repCargaEntrega;

        #endregion

        #region Construtores

        public Paradas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.Paradas> ConsultarRegistros(FiltroPesquisaRelatorioParadas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repCargaEntrega.ConsultarRelatorioParadas(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioParadas filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repCargaEntrega.ContarConsultaRelatorioParadas(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/Paradas";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioParadas filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFiliais.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais) : null;
            List<Dominio.Entidades.Usuario> motoristas = filtrosPesquisa.CodigosMotoristas.Count > 0 ? repUsuario.BuscarUsuariosPorCodigos(filtrosPesquisa.CodigosMotoristas.ToArray(), "") : null;
            List<Dominio.Entidades.Cliente> remetentes = filtrosPesquisa.CpfsCnpjsRemetentes.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfsCnpjsRemetentes) : null;
            List<Dominio.Entidades.Cliente> destinatarios = filtrosPesquisa.CpfsCnpjsDestinatarios.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfsCnpjsDestinatarios) : null;
            List<Dominio.Entidades.Empresa> transportadores = filtrosPesquisa.CodigosTransportadores.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportadores) : null;
            List<Dominio.Entidades.Veiculo> veiculos = filtrosPesquisa.CodigosVeiculos.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculos) : null;
            Dominio.Entidades.Localidade origem = filtrosPesquisa.CodigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoOrigem) : null;
            Dominio.Entidades.Localidade destino = filtrosPesquisa.CodigoDestino > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoDestino) : null;
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tipoCargas = filtrosPesquisa.CodigosTipoCargas.Count > 0 ? repTipoDeCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTipoCargas) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tipoOperacoes = filtrosPesquisa.CodigosTipoOperacoes.Count > 0 ? repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacoes) : null;

            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("DataEntregaInicial", filtrosPesquisa.DataEntregaInicial, filtrosPesquisa.DataEntregaInicial));
            parametros.Add(new Parametro("DataEntregaFinal", filtrosPesquisa.DataEntregaFinal, filtrosPesquisa.DataEntregaFinal));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCargas));
            parametros.Add(new Parametro("TipoCarga", tipoCargas != null ? (from o in tipoCargas select o.Descricao) : null));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacoes != null ? (from o in tipoOperacoes select o.Descricao) : null));
            parametros.Add(new Parametro("Transportador", transportadores != null ? (from o in transportadores select o.Descricao) : null));
            parametros.Add(new Parametro("Filial", filiais != null ? (from o in filiais select o.Descricao) : null));
            parametros.Add(new Parametro("Veiculo", veiculos != null ? (from o in veiculos select o.Descricao) : null));
            parametros.Add(new Parametro("Motorista", motoristas != null ? (from o in motoristas select o.Descricao) : null));
            parametros.Add(new Parametro("Remetente", remetentes != null ? (from o in remetentes select o.Descricao) : null));
            parametros.Add(new Parametro("Destinatario", destinatarios != null ? (from o in destinatarios select o.Descricao) : null));
            parametros.Add(new Parametro("Origem", origem?.Descricao));
            parametros.Add(new Parametro("Destino", destino?.Descricao));
            parametros.Add(new Parametro("TipoParada", filtrosPesquisa.TipoParada.HasValue ? filtrosPesquisa.TipoParada.Value ? "Coleta" : "Entrega" : string.Empty));
            parametros.Add(new Parametro("ProtocoloIntegracaoSM", filtrosPesquisa.ProtocoloIntegracaoSM));
            parametros.Add(new Parametro("EscritorioVendas", filtrosPesquisa.EscritorioVendas));
            parametros.Add(new Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente));
            parametros.Add(new Parametro("MonitoramentoStatus", string.Join(", ", filtrosPesquisa.MonitoramentoStatus.Select(O => O.ObterDescricao()))));
            parametros.Add(new Parametro("DataEntregaPlanejadaInicio", filtrosPesquisa.DataEntregaPlanejadaInicio));
            parametros.Add(new Parametro("DataEntregaPlanejadaFinal", filtrosPesquisa.DataEntregaPlanejadaFinal));
            parametros.Add(new Parametro("Transbordo", filtrosPesquisa.Transbordo.HasValue ? filtrosPesquisa.Transbordo.Value ? "Sim" : "N�o" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar == "SituacaoEntregaDescricao")
                return "SituacaoEntrega";

            return propriedadeOrdenarOuAgrupar;
        }
    }
}
