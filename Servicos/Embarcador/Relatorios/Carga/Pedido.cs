using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class Pedido : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido, Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedido>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.Pedido _repositorioPedido;

        #endregion

        #region Construtores

        public Pedido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo async
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedido> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPedido.ConsultarRelatorioPedido(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPedido.ContarConsultaRelatorioPedido(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/Pedido";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCustoViagem repCentroDeCustoViagem = new Repositorio.Embarcador.Logistica.CentroCustoViagem(_unitOfWork);

            List<Dominio.Entidades.Cliente> remetente = repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsRemetente);
            List<Dominio.Entidades.Cliente> destinatario = repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsDestinatario);
            List<Dominio.Entidades.Cliente> expedidores = filtrosPesquisa.CpfCnpjsExpedidor?.Count > 0 ? repositorioCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjsExpedidor) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Localidade> origem = repositorioLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosOrigem);
            List<Dominio.Entidades.Localidade> destino = repositorioLocalidade.BuscarPorCodigos(filtrosPesquisa.CodigosDestino);
            List<Dominio.Entidades.Estado> UForigem = repEstado.BuscarPorSiglas(filtrosPesquisa.SiglasOrigem);
            List<Dominio.Entidades.Estado> UFDestino = repEstado.BuscarPorSiglas(filtrosPesquisa.SiglasDestino);
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repositorioVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> restricoesEntregas = repRestricaoEntrega.BuscarPorCodigos(filtrosPesquisa.CodigosRestricoes);
            List<Dominio.Entidades.RotaFrete> rotaFretes = repRotaFrete.BuscarPorCodigos(filtrosPesquisa.CodigosRotaFrete);
            List<Dominio.Entidades.Empresa> empresas = filtrosPesquisa.CodigosTransportadores?.Count > 0 ? repositorioEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportadores) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFilial?.Count > 0 ? repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : null;
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = filtrosPesquisa.CodigosGruposPessoas?.Count > 0 ? repositorioGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigosGruposPessoas.ToArray()) : null;
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = filtrosPesquisa.CodigosTipoCarga?.Count > 0 ? repositorioTipoCarga.BuscarPorCodigos(filtrosPesquisa.CodigosTipoCarga) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao?.Count > 0 ? repositorioTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacao) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorCodigos(filtrosPesquisa.CodigosPedido);
            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeiculares = filtrosPesquisa.CodigosModelosVeiculos?.Count > 0 ? repositorioModeloVeiculo.BuscarPorCodigos(filtrosPesquisa.CodigosModelosVeiculos) : null;
            Dominio.Entidades.Usuario gerente = filtrosPesquisa.CodigoGerente > 0 ? repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoGerente) : null;
            Dominio.Entidades.Usuario vendedor = filtrosPesquisa.CodigoVendedor > 0 ? repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoVendedor) : null;
            Dominio.Entidades.Usuario supervisor = filtrosPesquisa.CodigoSupervisor > 0 ? repositorioUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoSupervisor) : null;
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio numeroViagemNavio = filtrosPesquisa.CodigoNumeroViagemNavio > 0 ? repPedidoViagemNavio.BuscarPorCodigo(filtrosPesquisa.CodigoNumeroViagemNavio) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoOrigem = filtrosPesquisa.CodigoPortoOrigem > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoOrigem) : null;
            Dominio.Entidades.Embarcador.Pedidos.Porto portoDestino = filtrosPesquisa.CodigoPortoDestino > 0 ? repPorto.BuscarPorCodigo(filtrosPesquisa.CodigoPortoDestino) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> operadoresPedido = filtrosPesquisa.CodigoOperadorPedido?.Count > 0 ? repPedido.BuscarPorCodigos(filtrosPesquisa.CodigoOperadorPedido) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centrosResultado = filtrosPesquisa.CodigoCentroResultado?.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.CodigoCentroResultado) : new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            Dominio.Entidades.Embarcador.Logistica.CentroCustoViagem centroCustoViagem = filtrosPesquisa.CentroDeCustoViagemCodigo > 0 ? repCentroDeCustoViagem.BuscarPorCodigo(filtrosPesquisa.CentroDeCustoViagemCodigo) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCarregamentoInicial", filtrosPesquisa.DataCarregamentoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCarregamentoFinal", filtrosPesquisa.DataCarregamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCriacaoPedidoInicial", filtrosPesquisa.DataCriacaoPedidoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCriacaoPedidoFinal", filtrosPesquisa.DataCriacaoPedidoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacoesPedido", string.Join(", ", filtrosPesquisa.SituacoesPedido.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal));

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 1)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.SituacoesCargaMercante != null ? string.Join(",", filtrosPesquisa.SituacoesCargaMercante.Select(o => o.ObterDescricao())) : null));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacoes != null ? string.Join(",", filtrosPesquisa.Situacoes.Select(o => o.ObterDescricao())) : null));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Restricao", string.Join(", ", restricoesEntregas.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", string.Join(", ", remetente.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", string.Join(", ", destinatario.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", string.Join(", ", origem.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", string.Join(", ", destino.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UFOrigem", string.Join(", ", UForigem.Select(o => o.Nome))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UFDestino", string.Join(", ", UFDestino.Select(o => o.Nome))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", string.Join(", ", pedidos.Select(o => o.NumeroPedidoEmbarcador))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresas != null ? string.Join(", ", empresas.Select(o => o.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RotaFrete", string.Join(", ", rotaFretes.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filiais != null ? string.Join(", ", filiais.Select(o => o.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", gruposPessoas != null ? string.Join(", ", gruposPessoas.Select(o => o.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tiposCarga != null ? string.Join(", ", tiposCarga.Select(o => o.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tiposOperacao != null ? string.Join(", ", tiposOperacao.Select(o => o.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modelosVeiculares != null ? string.Join(", ", modelosVeiculares.Select(o => o.Descricao)) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoLocalPrestacao", filtrosPesquisa.TipoLocalPrestacao != TipoLocalPrestacao.todos ? filtrosPesquisa.TipoLocalPrestacao.ObterDescricao() : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirProdutos", filtrosPesquisa.ExibirProdutos ? "Sim" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PedidosSemCarga", filtrosPesquisa.PedidosSemCarga ? "Sim" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DeliveryTerm", filtrosPesquisa.DeliveryTerm));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("IdAutorizacao", filtrosPesquisa.IdAutorizacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInclusaoBookingInicial", filtrosPesquisa.DataInclusaoBookingInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInclusaoBookingLimite", filtrosPesquisa.DataInclusaoBookingLimite));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInclusaoPCPInicial", filtrosPesquisa.DataInclusaoPCPInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInclusaoPCPLimite", filtrosPesquisa.DataInclusaoPCPLimite));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Gerente", gerente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vendedor", vendedor?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Supervisor", supervisor?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicioViagem", filtrosPesquisa.DataInicioViagemInicial, filtrosPesquisa.DataInicioViagemFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntrega", filtrosPesquisa.DataEntregaInicial, filtrosPesquisa.DataEntregaFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPrevisaoEntrega", filtrosPesquisa.PrevisaoDataInicial, filtrosPesquisa.PrevisaoDataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomentePedidosCanceladosAposVincularCarga", filtrosPesquisa.SomentePedidosCanceladosAposVincularCarga ? "Sim" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedido", filtrosPesquisa.NumeroPedido));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoEntrega", filtrosPesquisa.SituacoesEntrega?.Count > 0 ? string.Join(", ", filtrosPesquisa.SituacoesEntrega.Select(obj => obj.ObterDescricao())) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Expedidor", string.Join(", ", expedidores.Select(obj => obj.Nome))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiExpedidor", filtrosPesquisa.PossuiExpedidor));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiRecebedor", filtrosPesquisa.PossuiRecebedor));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirCargasAgrupadas", filtrosPesquisa.ExibirCargasAgrupadas ? "Sim" : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroViagem", numeroViagemNavio?.NumeroViagem));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoOrigem", portoOrigem?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PortoDestino", portoDestino?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPropostaMultiModal", filtrosPesquisa.TipoPropostaMultiModal?.Count > 0 ? string.Join(", ", filtrosPesquisa.TipoPropostaMultiModal.Select(obj => obj.ObterDescricao())) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataETAPortoOrigem", filtrosPesquisa.DataETAPortoOrigemInicial, filtrosPesquisa.DataETAPortoOrigemFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataETSPortoOrigem", filtrosPesquisa.DataETSPortoOrigemInicial, filtrosPesquisa.DataETSPortoOrigemFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataETAPortoDestino", filtrosPesquisa.DataETAPortoDestinoInicial, filtrosPesquisa.DataETAPortoDestinoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataETSPortoDestino", filtrosPesquisa.DataETSPortoDestinoInicial, filtrosPesquisa.DataETSPortoDestinoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInclusaoPedido", filtrosPesquisa.DataInclusaoPedidoInicial, filtrosPesquisa.DataInclusaoPedidoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInclusaoPedido", filtrosPesquisa.DataInclusaoPedidoInicial, filtrosPesquisa.DataInclusaoPedidoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("OperadorPedido", string.Join(", ", operadoresPedido.Select(obj => obj.Autor.Nome))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", string.Join(", ", centrosResultado.Select(obj => obj.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicioJanela", filtrosPesquisa.DataInicioJanela));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AguardandoIntegracao", filtrosPesquisa.AguardandoIntegracao.HasValue ? filtrosPesquisa.AguardandoIntegracao?.ObterDescricao() : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SomentePedidosDeIntegracao", filtrosPesquisa.SomentePedidosDeIntegracao ? "Sim" : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroDeCustoViagemCodigo", centroCustoViagem?.Descricao, Localization.Resources.Relatorios.Cargas.Carga.CentroDeCustoViagem));

            return parametros;
        }

        protected override List<KeyValuePair<string, dynamic>> ObterSubReportDataSources(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedidoProduto> listaPedidoProdutos = new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedidoProduto>();

            if (filtrosPesquisa.ExibirProdutos)
                listaPedidoProdutos = _repositorioPedido.ConsultarRelatorioPedidoProdutos(filtrosPesquisa);

            return new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("PedidosProdutos.rpt", listaPedidoProdutos) };
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoPedido")
                return "SituacaoPedido";

            if (propriedadeOrdenarOuAgrupar == "PrevisaoSaidaAnteriorResponsavelDescricao")
                return "PrevisaoSaidaAnteriorResponsavel";

            if (propriedadeOrdenarOuAgrupar == "PrevisaoEntregaAnteriorResponsavelDescricao")
                return "PrevisaoEntregaAnteriorResponsavel";

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.EndsWith("String"))
                return propriedadeOrdenarOuAgrupar.Replace("String", "");

            if (propriedadeOrdenarOuAgrupar == "Tomador")
                return "TipoTomadorPagador";

            if (propriedadeOrdenarOuAgrupar == "DataAlocacaoPedidoFormatada")
                return "DataAlocacaoPedido";

            if (propriedadeOrdenarOuAgrupar == "AguardandoIntegracaoDescricao")
                return "AguardandoIntegracao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}