using System;
using System.Collections.Generic;
using System.Text;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.GestaoPallet
{
    public class AgendamentoPallet
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion Atributos

        #region Construtores

        public AgendamentoPallet(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AgendamentoPallet(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.Carga AdicionarCarga(Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamento, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = agendamento.Filial;

            if (filial == null && agendamento.Destinatario != null)
                filial = repositorioFilial.BuscarPorCNPJ(agendamento.Destinatario.CPF_CNPJ_SemFormato);

            if (filial == null)
                throw new ControllerException($"Não foi possível localizar a filial.");

            return GerarPedidoECarga(agendamento, filial, ClienteMultisoftware);
        }

        public byte[] ResumoAgendamentoPallet(int codigoAgendamento)
        {
            return ReportRequest.WithType(ReportType.ResumoAgendamentoPallet)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoAgendamento", codigoAgendamento.ToString())
                .CallReport()
                .GetContentFile();
        }

        public string ObterSenhaAgendamentoPallet(Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet)
        {
            Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(_unitOfWork);

            if (!string.IsNullOrWhiteSpace(agendamentoPallet.Senha))
                return agendamentoPallet.Senha;

            if (string.IsNullOrWhiteSpace(agendamentoPallet.Senha))
                return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();

            int senhaSequencial = repositorioAgendamentoPallet.ObterProximaSenhaSequencial();

            agendamentoPallet.SenhaSequencial = senhaSequencial;

            return $"M{senhaSequencial}";
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.Carga GerarPedidoECarga(Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamento, Dominio.Entidades.Embarcador.Filiais.Filial filial, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = PreencherObjetoCargaIntegracao(agendamento, filial);

            Servicos.WebService.Carga.Pedido servicoPedidoWS = new Servicos.WebService.Carga.Pedido(_unitOfWork);
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.Carga servicoCargaWS = new Servicos.WebService.Carga.Carga(_unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido servicoProdutoPedido = new Servicos.WebService.Carga.ProdutosPedido(_unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Servicos.Embarcador.Carga.RateioFrete(_unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            int codigoCargaExistente = 0;
            int protocoloPedidoExistente = 0;

            StringBuilder mensagemErro = new StringBuilder();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = servicoPedidoWS.CriarPedido(cargaIntegracao, filial, agendamento.TipoOperacao, ref mensagemErro, _tipoServicoMultisoftware, ref protocoloPedidoExistente, ref codigoCargaExistente, buscarCargaPorTransportador: false, ignorarPedidosInseridosManualmente: true, configuracaoTMS: _configuracaoEmbarcador);

            if (mensagemErro.Length > 0)
                throw new ControllerException(mensagemErro.ToString());

            servicoProdutoPedido.AdicionarProdutosPedido(pedido, _configuracaoEmbarcador, cargaIntegracao, ref mensagemErro, _unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = servicoCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref mensagemErro, ref codigoCargaExistente, _unitOfWork, _tipoServicoMultisoftware, false, false, null, _configuracaoEmbarcador, null, "", filial, agendamento.TipoOperacao);

            if (cargaPedido != null)
            {
                servicoRateioFrete.GerarComponenteICMS(cargaPedido, false, _unitOfWork);

                if (cargaPedido.CargaPedidoFilialEmissora)
                    servicoRateioFrete.GerarComponenteICMS(cargaPedido, true, _unitOfWork);

                servicoRateioFrete.GerarComponenteISS(cargaPedido, false, _unitOfWork);
                servicoCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref mensagemErro, _unitOfWork, false);

                carga = cargaPedido.Carga;
            }

            if (mensagemErro.Length > 0)
                throw new ControllerException(mensagemErro.ToString());

            servicoCarga.FecharCarga(carga, _unitOfWork, _tipoServicoMultisoftware, ClienteMultisoftware, recriarRotas: false, adicionarJanelaDescarregamento: false, adicionarJanelaCarregamento: true, validarDados: false, gerarAgendamentoColeta: false);

            carga.DadosSumarizados.QuantidadeVolumes = agendamento.QuantidadePallets;
            carga.CargaFechada = true;
            carga.NumeroSequenciaCarga = cargaIntegracao.NumeroCarga.ToInt();
            carga.ModeloVeicularCarga = agendamento.ModeloVeicular;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            bool avancarEtapaCarga = (agendamento.TipoOperacao?.ConfiguracaoAgendamentoColetaEntrega?.ObrigarInformarCTePortalFornecedor ?? false) || (configuracaoJanelaCarregamento.ExibirOpcaoMultiModalAgendamentoColeta && agendamento.VeiculoSelecionado != null && agendamento.MotoristaSelecionado != null && agendamento.Transportador != null && carga.ModeloVeicularCarga != null);

            if (avancarEtapaCarga && carga.ExigeNotaFiscalParaCalcularFrete && (carga.SituacaoCarga == SituacaoCarga.AgTransportador || carga.SituacaoCarga == SituacaoCarga.Nova))
                carga.SituacaoCarga = SituacaoCarga.AgNFe;

            repositorioCarga.Atualizar(carga);
            repositorioPedido.Atualizar(pedido);

            return carga;
        }

        private Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao PreencherObjetoCargaIntegracao(Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamento, Dominio.Entidades.Embarcador.Filiais.Filial filial)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Servicos.WebService.Carga.TipoOperacao serWSTipoOperacao = new Servicos.WebService.Carga.TipoOperacao(_unitOfWork.StringConexao);
            Carga.Carga servicoCarga = new (_unitOfWork);

            string numeroCarga = servicoCarga.ObterNumeroCargaAdicionar(filial);
            string codigoFilialEmbarcador = filial.CodigoFilialEmbarcador;
            int numeroPedidoEmbarcador = _configuracaoEmbarcador.UtilizarNumeroPreCargaPorFilial ? repositorioPedido.ObterProximoCodigo(filial) : repositorioPedido.ObterProximoCodigo();

            Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao
            {
                Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = codigoFilialEmbarcador },
                NumeroCarga = numeroCarga,
                NumeroPedidoEmbarcador = numeroPedidoEmbarcador.ToString(),
                DataPrevisaoChegadaDestinatario = agendamento.DataEntrega?.ToString("dd/MM/yyyy HH:mm:ss"),
                ModeloVeicular = agendamento.ModeloVeicular != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = agendamento.ModeloVeicular.CodigoIntegracao } : null,
                Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = agendamento.Destinatario.CPF_CNPJ_SemFormato },
                Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = agendamento.Remetente?.CPF_CNPJ_SemFormato },
                TipoCargaEmbarcador = agendamento.TipoCarga != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = agendamento.TipoCarga.CodigoTipoCargaEmbarcador } : agendamento.TipoOperacao?.TipoDeCargaPadraoOperacao != null ? new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = agendamento.TipoOperacao.TipoDeCargaPadraoOperacao.CodigoTipoCargaEmbarcador } : null,
                ProdutoPredominante = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto() { DescricaoProduto = "Diversos", CodigoProduto = "Diversos" },
                DataColeta = agendamento.DataEntrega.HasValue ? $"{agendamento.DataEntrega:dd/MM/yyyy} 00:00:00" : string.Empty,
                DataPrevisaoEntrega = agendamento.DataEntrega.ToDateTimeString(true) ?? string.Empty,
                QuantidadeVolumes = agendamento.QuantidadePallets,
                TransportadoraEmitente = agendamento.Transportador != null ? new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = agendamento.Transportador.CNPJ } : null,
                DataInicioCarregamento = agendamento.DataEntrega?.ToDateTimeString(true),
                TipoOperacao = serWSTipoOperacao.ConverterObjetoTipoOperacao(agendamento.TipoOperacao),
                Veiculo = agendamento.VeiculoSelecionado != null ? (new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo() { Placa = agendamento.VeiculoSelecionado.Placa }) : null
            };

            if (agendamento.MotoristaSelecionado != null)
            {
                cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>() { new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista() { CPF = agendamento.MotoristaSelecionado.CPF } };
            }

            cargaIntegracao.Recebedor = null;

            return cargaIntegracao;
        }

        #endregion Métodos Privados
    }
}
