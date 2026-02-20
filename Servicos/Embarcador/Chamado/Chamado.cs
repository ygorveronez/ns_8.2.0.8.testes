using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Chamado
{
    public class Chamado : ServicoBase
    {
        #region Variaveis Privadas

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Variaveis Privadas

        #region Construtores

        public Chamado(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public Chamado(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork)
        {
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
        }

        #endregion Construtores

        #region Métodos Públicos

        public dynamic ObterResumoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteComplementar repClienteComplementar = new Repositorio.Embarcador.Pessoas.ClienteComplementar(unitOfWork);

            Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = chamado.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = carga != null ? servicoCargaJanelaCarregamentoConsulta.ObterCargaJanelaCarregamentoPorCarga(carga.Codigo) : null;
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = chamado.MotivoChamado;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = carga != null && !configuracaoTMS.NaoExibirInfosAdicionaisGridPatio ? repCargaCTe.BuscarPrimeiroCTePorCarga(carga.Codigo) : null;
            Dominio.Entidades.Cliente cliente = chamado.Cliente;
            Dominio.ObjetosDeValor.Embarcador.Pessoas.ClienteComplementar clienteComplementar = repClienteComplementar.BuscarClienteComplementarPorClienteAsync(cliente.CPF_CNPJ).Result;
            Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = configuracaoTMS.VisualizarDatasRaioNoAtendimento ? repClienteDescarga.BuscarPorOrigemEDestino(cliente?.CPF_CNPJ ?? 0, chamado.Destinatario?.CPF_CNPJ ?? 0) : null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoDoCliente = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica = repDiariaAutomatica.BuscarPorChamado(chamado.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidosCarga = repositorioPedido.BuscarPorCarga(carga?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();

            List<int> notas = new List<int>();
            List<string> tomadores = new List<string>();
            List<string> vendedores = new List<string>();
            List<string> vendedoresEmail = new List<string>();
            List<string> vendedoresTelefone = new List<string>();
            List<string> listaNumeroOrdemPedido = new List<string>();
            List<string> listaNumeroPedidoEmbarcador = new List<string>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = null;

            if (carga != null)
            {
                if (cliente != null)
                    cargasPedidoDoCliente = (from o in carga.Pedidos where o.Pedido.Destinatario.Codigo == cliente.Codigo select o).ToList();
                else
                    cargasPedidoDoCliente = (from o in carga.Pedidos select o).ToList();
            }

            if (chamado.Tomador != null)
                tomadores.Add(chamado.Tomador.Descricao);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in cargasPedidoDoCliente)
            {
                notasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(pedido.Codigo);

                if (notasFiscais != null && notasFiscais.Count > 0)
                    notas.AddRange((from o in notasFiscais select o.XMLNotaFiscal.Numero).ToList());
                if (chamado.Tomador == null && pedido.ObterTomador() != null && !tomadores.Contains(pedido.ObterTomador()?.Descricao))
                    tomadores.Add(pedido.ObterTomador()?.Descricao);
                if (pedido.Pedido.FuncionarioVendedor != null)
                {
                    vendedores.Add(pedido.Pedido.FuncionarioVendedor.Nome);
                    vendedoresEmail.Add(pedido.Pedido.FuncionarioVendedor.Email);
                    vendedoresTelefone.Add(pedido.Pedido.FuncionarioVendedor.Telefone);
                }
                else if (!string.IsNullOrWhiteSpace(pedido.Pedido.Vendedor))
                    vendedores.Add(pedido.Pedido.Vendedor);

                listaNumeroOrdemPedido.Add(pedido.Pedido.NumeroOrdem);
                listaNumeroPedidoEmbarcador.Add(pedido.Pedido.NumeroPedidoEmbarcador);
            }

            notas = notas.Distinct().ToList();
            tomadores = tomadores.Distinct().ToList();
            vendedores = vendedores.Distinct().ToList();
            vendedoresEmail = vendedoresEmail.Distinct().ToList();
            vendedoresTelefone = vendedoresTelefone.Distinct().ToList();

            DateTime? dataPrevisaoEntrega = carga != null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? (from o in carga.Pedidos where o.Pedido.PrevisaoEntrega.HasValue select o.Pedido.PrevisaoEntrega).FirstOrDefault() : null;
            bool podeVisualizarRetencaoDaReentrega = motivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega && (repMotivoChamado.ExistePorTipoMotivoAtendimento(TipoMotivoAtendimento.Retencao) || repMotivoChamado.ExistePorTipoMotivoAtendimento(TipoMotivoAtendimento.RetencaoOrigem));

            var resumo = new
            {
                chamado.Codigo,
                chamado.Numero,
                Carga = carga?.CodigoCargaEmbarcador ?? string.Empty,
                CodigosAgrupadosCarga = carga != null ? string.Join(", ", carga.CodigosAgrupados) : string.Empty,
                Empresa = carga?.Empresa?.Descricao ?? string.Empty,
                Veiculo = carga?.RetornarPlacasComModelo ?? string.Empty,
                TipoVeiculo = carga?.Veiculo?.DescricaoTipoVeiculo ?? string.Empty,
                Motorista = carga?.RetornarMotoristas ?? string.Empty,
                MotoristaTelefone = carga?.RetornarTelefoneMotoristas ?? string.Empty,
                Origem = configuracaoChamado.PermitirGerarAtendimentoPorPedido ? carga?.DadosSumarizados?.Origens ?? chamado.Pedido.Origem.DescricaoCidadeEstado : carga?.DadosSumarizados?.Origens ?? string.Empty,
                Destino = configuracaoChamado.PermitirGerarAtendimentoPorPedido ? carga?.DadosSumarizados?.Destinos ?? chamado.Pedido.Destino.DescricaoCidadeEstado : carga?.DadosSumarizados?.Destinos ?? string.Empty,
                DataRegistroMotorista = chamado.DataRegistroMotorista.HasValue ? chamado.DataRegistroMotorista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                Analista = string.Join(", ", (from obj in chamado.Analistas select obj.Nome).ToList()),
                DataChamado = chamado.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                Situacao = chamado.DescricaoSituacao,
                Cliente = cliente?.Descricao ?? "",
                ClienteFantasia = cliente?.NomeFantasia ?? "",
                ClienteEndereco = cliente?.EnderecoCompleto ?? "",
                ObservacoesCarga = cargaJanelaCarregamento?.ObservacaoTransportador ?? string.Empty,
                ClienteCidade = cliente?.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                DataHoraFaturamento = carga?.DataEnvioUltimaNFe?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataHoraPrevisaoEntrega = dataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Atrazo = cargaJanelaCarregamento?.DiasAtraso ?? 0,
                Valor = chamado.Valor > 0 ? chamado.Valor.ToString("n2") : string.Empty,
                NotasFiscais = string.Join(", ", notas),
                Tomador = string.Join(", ", tomadores),
                Destinatario = !configuracaoTMS.NaoExibirInfosAdicionaisGridPatio ? chamado.Destinatario?.Descricao ?? string.Empty : string.Empty,
                DestinatarioFantasia = !configuracaoTMS.NaoExibirInfosAdicionaisGridPatio ? chamado.Destinatario?.NomeFantasia ?? string.Empty : string.Empty,
                FilialVenda = cargaEntrega != null ? string.Join(", ", cargaEntrega.Pedidos.Where(o => o.CargaPedido.Pedido.FilialVenda != null).Select(o => o.CargaPedido.Pedido.FilialVenda.Descricao).Distinct()) : carga != null ? string.Join(", ", carga.Pedidos.Where(o => o.Pedido.FilialVenda != null).Select(o => o.Pedido.FilialVenda.Descricao).Distinct()) : string.Empty,
                FuncionarioVendedor = string.Join(", ", vendedores) ?? string.Empty,
                FuncionarioVendedorEmail = string.Join(", ", vendedoresEmail) ?? string.Empty,
                FuncionarioVendedorTelefone = string.Join(", ", vendedoresTelefone) ?? string.Empty,
                DataInicioViagemCarga = carga?.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataEmissaoDocumentoFreteCarga = cte?.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataEntradaRaio = cargaEntrega?.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataSaidaRaio = cargaEntrega?.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataRetencaoInicio = podeVisualizarRetencaoDaReentrega ? chamado.DataRetencaoInicio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty : string.Empty,
                DataRetencaoFim = podeVisualizarRetencaoDaReentrega ? chamado.DataRetencaoFim?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty : string.Empty,
                PeriodoJanelaDescarga = clienteDescarga != null && !string.IsNullOrWhiteSpace(clienteDescarga.HoraInicioDescarga) && !string.IsNullOrWhiteSpace(clienteDescarga.HoraLimiteDescarga) ? $"{clienteDescarga.HoraInicioDescarga} - {clienteDescarga.HoraLimiteDescarga}" : string.Empty,
                FilialCarga = carga?.Filial?.Descricao ?? string.Empty,
                ModeloVeiculo = carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                NumeroOrdem = listaNumeroOrdemPedido?.Count > 0 ? string.Join(", ", listaNumeroOrdemPedido) : string.Empty,
                NumeroPedidoEmbarcador = listaNumeroPedidoEmbarcador?.Count > 0 ? string.Join(", ", listaNumeroPedidoEmbarcador) : string.Empty,
                CustoFrete = carga?.DadosSumarizados?.CustoFrete ?? string.Empty,
                Expedidor = carga?.DadosSumarizados?.Expedidores ?? string.Empty,
                Recebedor = carga?.DadosSumarizados?.Recebedores ?? string.Empty,
                NotasFiscaisSelecionadasAtendimento = string.Join(", ", chamado.XMLNotasFiscais.Select(p => p.Numero)),
                TipoOperacaoCarga = carga?.TipoOperacao?.Descricao ?? string.Empty,
                Genero = chamado.MotivoChamado?.Genero?.Descricao ?? string.Empty,
                AreaEnvolvida = chamado.MotivoChamado?.AreaEnvolvida?.Descricao ?? string.Empty,
                LeadTimeTransportador = (chamado.Pedido == null && (configuracaoChamado?.VincularPrimeiroPedidoDoClienteAoAbrirChamado ?? false)) ? repCargaPedido.BuscarCargaPedidoPorCargaEClienteDestinatario(chamado.Carga.Codigo, chamado.Cliente.Codigo)?.FirstOrDefault()?.Pedido?.DiasUteisPrazoTransportador : chamado.Pedido?.DiasUteisPrazoTransportador ?? 0,
                Parqueada = (chamado.Carga != null && chamado.Carga.Parqueada.HasValue) ? (chamado.Carga.Parqueada.Value ? "Sim" : "Não") : string.Empty,
                NumeroCarga = carga?.Numero ?? "-",
                DescricaoTipoVeiculo = carga?.Veiculo?.TipoDoVeiculo?.Descricao ?? "-",
                CNPJCliente = cliente?.CPF_CNPJ_Formatado ?? "-",
                GrupoCliente = cliente?.GrupoPessoas?.Descricao ?? "-",
                ClienteMatriz = cliente?.Empresa?.Matriz.FirstOrDefault().Descricao ?? "-",
                CodigoClienteMatriz = cliente?.Empresa?.Matriz.FirstOrDefault().CodigoEmpresa ?? "-",
                ClienteBairro = cliente?.Bairro ?? "-",
                ClienteCEP = cliente?.CEP ?? "-",
                ClienteUF = cliente?.Localidade?.Estado?.Sigla ?? "-",
                ClientePais = !string.IsNullOrWhiteSpace(cliente?.Pais?.Descricao) ? cliente.Pais.Descricao : !string.IsNullOrWhiteSpace(cliente?.Localidade?.Pais?.Descricao) ? cliente.Localidade.Pais.Descricao : "-",
                ClienteTelefone = !string.IsNullOrWhiteSpace(cliente?.Telefone1) ? cliente.Telefone1 : !string.IsNullOrWhiteSpace(cliente?.Telefone2) ? cliente.Telefone2 : "-",
                ClienteEmail = cliente?.Email ?? "-",
                ClienteEmailNFe = !string.IsNullOrEmpty(cliente?.Email) && cliente?.EmailStatus == "A" ? cliente.Email : "-",
                CodigoCliente = clienteComplementar?.Codigo ?? 0,
                RecebimentoSegundaFeira = clienteComplementar?.SegundaFeira ?? "-",
                RecebimentoTercaFeira = clienteComplementar?.TercaFeira ?? "-",
                RecebimentoQuartaFeira = clienteComplementar?.QuartaFeira ?? "-",
                RecebimentoQuintaFeira = clienteComplementar?.QuintaFeira ?? "-",
                RecebimentoSextaFeira = clienteComplementar?.SextaFeira ?? "-",
                RecebimentoSabado = clienteComplementar?.Sabado ?? "-",
                RecebimentoDomingo = clienteComplementar?.Domingo ?? "-",
                SegundaRemessa = clienteComplementar?.SegundaRemessa == true ? "Sim" : clienteComplementar?.SegundaRemessa == false ? "Não" : "-",
                ExclusividadeEntrega = clienteComplementar?.ExclusividadeEntrega == true ? "Sim" : clienteComplementar?.ExclusividadeEntrega == false ? "Não" : "-",
                Paletizacao = clienteComplementar?.Paletizacao ?? "-",
                ClienteStrechado = clienteComplementar?.ClienteStrechado == true ? "Sim" : clienteComplementar?.ClienteStrechado == false ? "Não" : "-",
                Agendamento = clienteComplementar?.Agendamento ?? "-",
                ClienteComMulta = clienteComplementar?.ClienteComMulta == true ? "Sim" : clienteComplementar?.ClienteComMulta == false ? "Não" : "-",
                CapacidadeRecebimento = clienteComplementar?.CapacidadeRecebimento ?? "-",
                CustoDescarga = clienteComplementar?.CustoDescarga ?? 0,
                TipoCusto = clienteComplementar?.TipoCusto ?? "-",
                Ajudantes = clienteComplementar?.Ajudantes ?? "-",
                PagamentoDescarga = clienteComplementar?.PagamentoDescarga ?? "-",
                DescricaoPagamentoDescarga = clienteComplementar?.DescricaoPagamentoDescarga ?? "-",
                AlturaRecebimento = clienteComplementar?.AlturaRecebimento ?? "-",
                DescricaoAlturaRecebimento = clienteComplementar?.DescricaoAlturaRecebimento ?? "-",
                RestricaoCarregamento = clienteComplementar?.RestricaoCarregamento ?? "-",
                DescricaoRestricaoCarregamento = clienteComplementar?.DescricaoRestricaoCarregamento ?? "-",
                ComposicaoPalete = clienteComplementar?.ComposicaoPalete ?? "-",
                DescricaoComposicaoPalete = clienteComplementar?.DescricaoComposicaoPalete ?? "-",
                MatrizReferencia = clienteComplementar?.MatrizReferencia ?? "-",
                ParticionamentoVeiculo = clienteComplementar?.ParticionamentoVeiculo ?? "-",
                DescricaoParticionamentoVeiculo = clienteComplementar?.DescricaoParticionamentoVeiculo ?? "-",

                // Dados da diária automática
                DiariaAutomaticaTempoCobrado = diariaAutomatica?.TempoTotal,
                DiariaAutomaticaLocalFreeTime = diariaAutomatica?.LocalFreeTime.ObterDescricao() ?? string.Empty,
                DiariaAutomaticaTempoFreeTime = diariaAutomatica?.TempoFreeTime,
                DiariaAutomaticaValorTotal = diariaAutomatica?.ValorDiaria,
                DiariaAutomaticaValorPorHora = diariaAutomatica != null ? diariaAutomatica?.ValorPorHora : 0,
                DiariaAutomaticaEntradasESaidas = ObterDiariaAutomaticaEntradasESaidas(diariaAutomatica, unitOfWork),
                ValorNFe = notasFiscais != null ? notasFiscais.Sum(n => n.XMLNotaFiscal?.Valor ?? 0).ToString("n2") : string.Empty,
                DataEntradaRaioEntrega = cargaEntrega?.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                DataSaidaRaioEntrega = cargaEntrega?.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Volume = cargasPedidoDoCliente.Sum(p => p.Pedido.QtVolumes).ToString("n2") ?? string.Empty,
                VolumeNotasSelecionadas = chamado.XMLNotasFiscais?.Sum(x => x.Volumes).ToString("n2") ?? string.Empty,
            };

            return resumo;
        }

        public static Dominio.Entidades.Embarcador.Chamados.Chamado AbrirChamado(Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            return new Chamado(unitOfWork, tipoServicoMultisoftware, auditado).AbrirChamadoAsync(objetoChamado, usuario).GetAwaiter().GetResult();
        }

        public async Task<Dominio.Entidades.Embarcador.Chamados.Chamado> AbrirChamadoAsync(Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoAnalise repositorioChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repositorioConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = await repositorioConfiguracaoChamado.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = new Dominio.Entidades.Embarcador.Chamados.Chamado();
            chamado.MotivoChamado = objetoChamado.MotivoChamado;
            chamado.Carga = objetoChamado.Carga;
            chamado.Pedido = objetoChamado.Pedido;

            chamado.Empresa = objetoChamado.Empresa;
            chamado.Cliente = objetoChamado.Cliente;
            chamado.Tomador = objetoChamado.Tomador;
            chamado.Destinatario = objetoChamado.Destinatario;
            chamado.Motorista = objetoChamado.Motorista;
            chamado.QuantidadeImagensEsperada = objetoChamado.QuantidadeImagens;
            chamado.ModeloVeicularCarga = objetoChamado.ModeloVeicularCarga;
            chamado.GrupoMotivoChamado = objetoChamado.GrupoMotivoChamado;
            chamado.NumeroEmbarcador = objetoChamado.NumeroEmbarcador;
            chamado.MotivoDaDevolucao = objetoChamado.MotivoDaDevolucao;

            if (objetoChamado.Pedido == null && (configuracaoChamado?.VincularPrimeiroPedidoDoClienteAoAbrirChamado ?? false))
                chamado.Pedido = await repositorioCargaPedido.BuscarPedidoPorCargaEClienteDestinatarioAsync(objetoChamado.Carga?.Codigo ?? 0, objetoChamado.Cliente?.Codigo ?? 0);

            if (chamado.MotivoChamado == null)
                throw new ServicoException("Motivo é obrigatório.");

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && chamado.Pedido == null)
                throw new ServicoException("Pedido é obrigatório.");

            if (chamado.Carga == null && !chamado.MotivoChamado.PermiteAtendimentoSemCarga && !configuracaoChamado.PermitirGerarAtendimentoPorPedido)
                throw new ServicoException("Carga é obrigatória.");

            if (chamado.Cliente == null && !chamado.MotivoChamado.ChamadoDeveSerAbertoPeloEmbarcador)
                throw new ServicoException("Cliente é obrigatório.");

            if (chamado.Tomador == null && _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPrimeiraPorCargaAsync(objetoChamado.Carga?.Codigo ?? 0);
                chamado.Tomador = cargaPedido?.ObterTomador();
                if (chamado.Tomador == null)
                    throw new ServicoException("Tomador é obrigatório.");
            }

            if (chamado.Destinatario == null && _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = await repositorioCargaPedido.BuscarPrimeiraPorCargaAsync(objetoChamado.Carga?.Codigo ?? 0);
                chamado.Destinatario = cargaPedido?.Pedido?.Destinatario;
                if (chamado.Destinatario == null)
                    throw new ServicoException("Destinatário é obrigatório.");
            }

            if (chamado.Empresa == null)
            {
                chamado.Empresa = chamado?.Empresa;
                if (chamado.Empresa == null)
                    throw new ServicoException("Empresa é obrigatória.");
            }

            if ((chamado?.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoChamado?.PermitirSelecionarApenasAlgunsMotivosAtendimento ?? false) && (chamado?.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoChamado?.Transportadores.Count > 0) && (_tipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe))
                if (!VerificarTransportadorPodeAbrirChamado(chamado, usuario))
                    throw new ServicoException("Transportador não possui permissão para abrir atendimentos para esse motivo de atendimento.");

            if (chamado.Carga != null)
            {
                if (chamado.MotivoChamado.ValidarDuplicidade)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamados = await repositorioChamado.BuscarPorCargaEMotivoAsync(chamado.Carga.Codigo, chamado.MotivoChamado.Codigo);
                    if (listaChamados.Exists(o => o.Situacao == SituacaoChamado.Aberto || o.Situacao == SituacaoChamado.EmTratativa || o.Situacao == SituacaoChamado.Finalizado || o.Situacao == SituacaoChamado.RecusadoPeloCliente ||
                                                 o.Situacao == SituacaoChamado.LiberadaOcorrencia || o.Situacao == SituacaoChamado.LiberadaValePallet || o.Situacao == SituacaoChamado.SemRegra))
                    {
                        Dominio.Entidades.Embarcador.Chamados.Chamado chamadoDuplicado = listaChamados.FirstOrDefault(o => o.Situacao == SituacaoChamado.Aberto || o.Situacao == SituacaoChamado.EmTratativa ||
                                                 o.Situacao == SituacaoChamado.Finalizado || o.Situacao == SituacaoChamado.RecusadoPeloCliente || o.Situacao == SituacaoChamado.LiberadaOcorrencia ||
                                                 o.Situacao == SituacaoChamado.LiberadaValePallet || o.Situacao == SituacaoChamado.SemRegra);

                        throw new ServicoException("Já existe atendimento " + chamadoDuplicado.Numero.ToString() + " (" + chamadoDuplicado.DescricaoSituacao + ") para Carga " + chamadoDuplicado.Carga.CodigoCargaEmbarcador + ", Motivo " + chamadoDuplicado.MotivoChamado.Descricao + " e Transportador " + chamadoDuplicado.Carga.Empresa?.Descricao + ".");
                    }
                }

                if (chamado.MotivoChamado.ValidarDuplicidadePorDestinatario && chamado.Destinatario != null)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamados = await repositorioChamado.BuscarPorCargaMotivoDestinatarioAsync(chamado.Carga.Codigo, chamado.MotivoChamado.Codigo, chamado.Destinatario.CPF_CNPJ);
                    if (listaChamados.Exists(o => o.Situacao == SituacaoChamado.Aberto || o.Situacao == SituacaoChamado.EmTratativa || o.Situacao == SituacaoChamado.Finalizado || o.Situacao == SituacaoChamado.RecusadoPeloCliente ||
                                                 o.Situacao == SituacaoChamado.LiberadaOcorrencia || o.Situacao == SituacaoChamado.LiberadaValePallet || o.Situacao == SituacaoChamado.SemRegra))
                    {
                        Dominio.Entidades.Embarcador.Chamados.Chamado chamadoDuplicado = listaChamados.FirstOrDefault(o => o.Situacao == SituacaoChamado.Aberto || o.Situacao == SituacaoChamado.EmTratativa ||
                                                 o.Situacao == SituacaoChamado.Finalizado || o.Situacao == SituacaoChamado.RecusadoPeloCliente || o.Situacao == SituacaoChamado.LiberadaOcorrencia ||
                                                 o.Situacao == SituacaoChamado.LiberadaValePallet || o.Situacao == SituacaoChamado.SemRegra);

                        throw new ServicoException("Já existe o atendimento " + chamadoDuplicado.Numero.ToString() + " (" + chamadoDuplicado.DescricaoSituacao + ") para Carga " + chamadoDuplicado.Carga.CodigoCargaEmbarcador + ", Motivo " + chamadoDuplicado.MotivoChamado.Descricao + " e Destinatário " + chamadoDuplicado.Destinatario.Descricao + ".");
                    }
                }

                if (chamado.MotivoChamado.TipoOcorrencia?.BloquearAberturaAtendimentoParaVeiculoEmContratoFrete ?? false)
                {
                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);
                    if (chamado.Carga.Veiculo != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = await repContratoFreteTransportador.BuscarContratosPorVeiculoAsync(DateTime.Now, chamado.Carga.Veiculo.Codigo);
                        if (contrato != null)
                            throw new ServicoException($"O veículo da carga está em um contrato de frete ({contrato.Descricao}), assim não é possível a abertura do atendimento.");
                    }
                }

                if (configuracaoChamado.BloquearAberturaChamadoRetencaoQuandoPossuirReentrega && objetoChamado.CargaEntrega != null && chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao
                    && await repositorioChamado.ContemChamadoDoTipoReentregaAsync(chamado.Carga.Codigo, objetoChamado.CargaEntrega.Codigo))
                    throw new ServicoException("Já existe um atendimento do Tipo de Reentrega para a entrega dessa carga, assim não é possível a abertura do atendimento do Tipo Retenção.");

                if (chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Devolucao || chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga)
                {

                    if (objetoChamado.CargaEntrega == null)
                        objetoChamado.CargaEntrega = await repositorioCargaEntrega.BuscarPorCargaEClienteAsync(chamado.Carga.Codigo, chamado.Destinatario.CPF_CNPJ);

                    if (objetoChamado.CargaEntrega == null)
                        objetoChamado.CargaEntrega = (chamado.Carga?.Pedidos?.Any(x => x.Recebedor != null) ?? true) ? await repositorioCargaEntrega.BuscarPorClienteRecebedorAsync(chamado.Carga.Codigo, chamado.Carga.Pedidos.Select(o => o.Recebedor.CPF_CNPJ).FirstOrDefault()) : null;

                    if (objetoChamado.CargaEntrega == null)
                        throw new ServicoException("Não localizada uma entrega para carga/cliente informado.");

                    if (objetoChamado.CargaEntrega.Situacao == SituacaoEntrega.Entregue && !configuracaoChamado.PermitirAbrirChamadoParaEntregaJaRealizada)
                        throw new ServicoException("Não é permitido abrir atendimento para entrega já realizada.");

                    List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosAnterior = await repositorioChamado.BuscarListaPorCargaEntregaAsync(objetoChamado.CargaEntrega.Codigo, chamado.MotivoChamado.TipoMotivoAtendimento);
                    foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamadoAnterior in chamadosAnterior)
                    {
                        if (!chamado.MotivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga && (chamadoAnterior.Situacao == SituacaoChamado.Aberto || chamadoAnterior.Situacao == SituacaoChamado.EmTratativa || chamadoAnterior.Situacao == SituacaoChamado.SemRegra ||
                            chamadoAnterior.Situacao == SituacaoChamado.LiberadaOcorrencia || chamadoAnterior.Situacao == SituacaoChamado.RecusadoPeloCliente))
                            throw new ServicoException($"Já existe o atendimento {chamadoAnterior.Numero} em aberto para mesma entrega.");
                    }
                }

                if (!await servicoCarga.VerificarSeCargaEstaNaLogisticaAsync(chamado.Carga, _tipoServicoMultisoftware))
                    chamado.VeiculoCarregado = true;
            }

            if (!chamado.MotivoChamado.PermitirLancarAtendimentoEmCargasComDocumentoEmitido && chamado.Carga != null &&
                chamado?.Carga?.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                chamado?.Carga?.SituacaoCarga != SituacaoCarga.EmTransporte &&
                chamado?.Carga?.SituacaoCarga != SituacaoCarga.LiberadoPagamento &&
                chamado?.Carga?.SituacaoCarga != SituacaoCarga.Encerrada &&
                chamado?.Carga?.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos &&
                !configuracao.FiltrarCargasSemDocumentosParaChamados)
            {
                if (_auditado?.OrigemAuditado == Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp)
                {
                    //nao gera atendimento.
                    chamado = new Dominio.Entidades.Embarcador.Chamados.Chamado();
                    return chamado;
                }
                else
                    throw new ServicoException("Carga sem documentação emitida. Para gerar atendimento com esse motivo é necessário ter os documentos emitidos na carga!");
            }

            if (configuracaoChamado.FazerGestaoCriticidade && chamado.Carga != null)
                await ValidarCriticidadeDoAtendimento(chamado);

            bool chamadoDuplicadoComCargaEMotivo = false;
            if (chamado.MotivoChamado.PermitirAbrirMaisAtendimentoComMesmoMotivoParaMesmaCarga)
                chamadoDuplicadoComCargaEMotivo = await repositorioChamado.JaExisteAtendimentoPorCargaMotivoAsync(chamado.Carga.Codigo, chamado.MotivoChamado.Codigo);

            chamado.Observacao = chamadoDuplicadoComCargaEMotivo ? "Chamado foi aberto pelo usuário, apesar de já existir outro semelhante com o mesmo motivo e carga." : Utilidades.String.RemoveAllSpecialCharactersNotCommon(objetoChamado.Observacao);

            if (chamado.MotivoChamado.AtendimentoPorLote)
                chamado.Situacao = SituacaoChamado.AgGeracaoLote;
            else
                chamado.Situacao = SituacaoChamado.Aberto;
            chamado.DataCriacao = DateTime.Now;

            if (objetoChamado.AtendimentoRegistradoPeloMotorista.HasValue && objetoChamado.AtendimentoRegistradoPeloMotorista.Value)
                chamado.DataRegistroMotorista = objetoChamado.DataRegistroMotorista;

            chamado.DataFinalizacao = null;
            chamado.AosCuidadosDo = ChamadoAosCuidadosDo.Embarcador;
            chamado.NumeroPallet = objetoChamado.NumeroPallet;
            chamado.QuantidadeItens = objetoChamado.QuantidadeItens;
            chamado.ResponsavelChamado = objetoChamado.ResponsavelChamado;
            chamado.Numero = (int)await repositorioChamado.BuscarProximoNumeroAsync();
            chamado.GerarCargaDevolucao = chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado;
            chamado.CargaEntrega = objetoChamado.CargaEntrega;
            chamado.Valor = objetoChamado.Valor;
            chamado.RetencaoBau = objetoChamado.RetencaoBau;
            chamado.TipoCliente = objetoChamado.TipoCliente;
            chamado.DataReentrega = objetoChamado.DataReentrega > DateTime.MinValue ? objetoChamado.DataReentrega : null;
            chamado.DataRetencaoInicio = objetoChamado.DataRetencaoInicio > DateTime.MinValue ? objetoChamado.DataRetencaoInicio : null;
            chamado.DataRetencaoFim = objetoChamado.DataRetencaoFim > DateTime.MinValue ? objetoChamado.DataRetencaoFim : null;
            chamado.TempoRetencao = objetoChamado.TempoRetencao;
            if (chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Reentrega || chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.Retencao || chamado.MotivoChamado.TipoMotivoAtendimento == TipoMotivoAtendimento.RetencaoOrigem)
                chamado.ClienteNovaEntrega = objetoChamado.ClienteDestino;

            if (objetoChamado.CargaEntrega != null && chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                chamado.GerarCargaDevolucao = true;

            if (objetoChamado?.TiposCausadoresOcorrencia != null)
                chamado.TiposCausadoresOcorrencia = objetoChamado.TiposCausadoresOcorrencia;

            if (objetoChamado?.CausasMotivoChamado != null)
                chamado.CausasMotivoChamado = objetoChamado.CausasMotivoChamado;

            chamado.Autor = usuario;
            chamado.ValorReferencia = objetoChamado.ValorReferencia;
            chamado.PlacaReboque = objetoChamado.PlacaReboque;
            chamado.ClienteResponsavel = objetoChamado.ClienteResponsavel;
            chamado.GrupoPessoasResponsavel = objetoChamado.GrupoPessoasResponsavel;
            chamado.TipoPessoaResponsavel = objetoChamado.TipoPessoaResponsavel;
            chamado.ValorDesconto = objetoChamado.ValorDesconto;
            chamado.PagoPeloMotorista = objetoChamado.PagoPeloMotorista;
            chamado.SaldoDescontadoMotorista = objetoChamado.SaldoDescontadoMotorista;
            chamado.Quantidade = objetoChamado.Quantidade;
            chamado.RealMotivo = objetoChamado.RealMotivo;

            // Persiste dados
            await repositorioChamado.InserirAsync(chamado, _auditado);

            //Cancela o atendimento anterior de acordo com a criticidade
            if (configuracaoChamado.FazerGestaoCriticidade && chamado.Carga != null)
                await CancelarAtendimentoAnteriorDeAcordoComACriticidadeAsync(chamado);

            //Cria um registro do nivel de atendimento para gerar uma notificação quando superar
            await SalvarNivelAtendimento(chamado);

            // Vincula e notifica usuarioss
            if (!await DefinirAnalistasChamado(chamado, usuario))
                chamado.Situacao = SituacaoChamado.SemRegra;

            await DefinirResponsavelChamado(chamado, usuario);

            if (objetoChamado.NotaFiscal != null)
            {
                if (chamado.XMLNotasFiscais == null)
                    chamado.XMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                chamado.XMLNotasFiscais.Add(objetoChamado.NotaFiscal);
            }

            if (objetoChamado.NotasFiscais?.Any() ?? false)
            {
                if (chamado.XMLNotasFiscais == null)
                    chamado.XMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                chamado.XMLNotasFiscais = objetoChamado.NotasFiscais;
            }

            await repositorioChamado.AtualizarAsync(chamado);

            Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise abertura = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise()
            {
                Chamado = chamado,
                Autor = chamado.Autor,
                DataCriacao = DateTime.Now,
                DataRetorno = null,
                Observacao = "Chamado aberto pelo usuario: {{autor}}, transportador: ({{empresa}}), dia {{dataabertura}} às {{horaabertura}}.\nMotivo: {{motivo}}.\nObservações: {{observacoes}}"
            };
            abertura.Observacao = abertura.Observacao
                .Replace("{{autor}}", abertura.Autor.Nome)
                .Replace("{{empresa}}", (chamado.Carga?.Empresa?.RazaoSocial ?? chamado.Empresa?.RazaoSocial ?? string.Empty))
                .Replace("{{dataabertura}}", chamado.DataCriacao.ToString("dd/MM/yyyy"))
                .Replace("{{horaabertura}}", chamado.DataCriacao.ToString("HH:mm"))
                .Replace("{{motivo}}", (chamado.MotivoChamado?.Descricao ?? string.Empty))
                .Replace("{{observacoes}}", (chamado.Observacao));

            await repositorioChamadoAnalise.InserirAsync(abertura);

            _unitOfWork.Flush();

            await AcoesPosCriacaoChamado(objetoChamado.CargaEntrega, _unitOfWork.StringConexao, objetoChamado.MotivoChamado);

            return chamado;
        }


        public void CancelarChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Pallets.ValePallet repValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);

            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repChamadoOcorrencia.BuscarOcorrenciasPorChamado(chamado.Codigo);
            if (ocorrencias.Exists(obj => obj.SituacaoOcorrencia != SituacaoOcorrencia.Cancelada && obj.SituacaoOcorrencia != SituacaoOcorrencia.Rejeitada))
                throw new ServicoException("Não é possível cancelar o atendimento pois ainda existem ocorrências não canceladas.");

            List<Dominio.Entidades.Embarcador.Pallets.ValePallet> valePallets = repValePallet.BuscarPorChamado(chamado.Codigo);
            if (valePallets.Exists(obj => obj.Situacao != SituacaoValePallet.Cancelado))
                throw new ServicoException("Não é possível cancelar o atendimento pois ainda existem vale pallets não cancelados.");

            if (chamado.CargaEntrega != null)
            {
                chamado.NotificacaoMotoristaMobile = true;
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                chamado.CargaEntrega.ChamadoEmAberto = false;
                chamado.CargaEntrega.MotivoRejeicao = null;

                if (!SituacaoEntregaHelper.ObterSituacaoEntregaFinalizada(chamado.CargaEntrega.Situacao))
                    chamado.CargaEntrega.Situacao = SituacaoEntrega.NaoEntregue;

                repCargaEntrega.Atualizar(chamado.CargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(chamado.CargaEntrega, repCargaEntrega, unitOfWork);

                ReverterQuantidadeDevolucao(chamado, unitOfWork);
            }

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorChamado(chamado.Codigo);

            if (pagamentoMotorista != null && pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.FinalizadoPagamento)
            {
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista servicoAutorizacaoPagamentoMotorista = new Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista(unitOfWork);
                servicoAutorizacaoPagamentoMotorista.CancelarIntegracaoPagamentoMotorista(pagamentoMotorista, usuario, auditado, tipoServicoMultisoftware);
                servicoAutorizacaoPagamentoMotorista.ReverterPagamentoMotorista(pagamentoMotorista, usuario, auditado, tipoServicoMultisoftware);
            }

            chamado.Situacao = SituacaoChamado.Cancelada;
            repChamado.Atualizar(chamado, auditado);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;

            if (cargaEntrega != null)
            {
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);

                if (cargaEvento != null)
                {
                    servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                    servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                }
            }
        }

        public static void NotificarChamadoAdicionadoOuAtualizado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Hubs.Chamado hubChamado = new Servicos.Embarcador.Hubs.Chamado();

            hubChamado.NotificarTodosChamadoAdicionadoOuAtualizado(chamado);

            Servicos.Embarcador.Carga.AlertaCarga.AtendimentoIniciado alertaAtendimento = new Servicos.Embarcador.Carga.AlertaCarga.AtendimentoIniciado(unitOfWork, unitOfWork.StringConexao);
            if (chamado.Situacao != SituacaoChamado.Finalizado)
                alertaAtendimento.ProcessarEvento(chamado.CargaEntrega, chamado);
        }

        public void NotificarChamadoPedidoPortalFornecedor(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, bool devolucaoParcial, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (chamado.CargaEntrega == null)
                return;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacaoPortal = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, clienteMultisoftware, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, string.Empty);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(chamado.CargaEntrega.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                Dominio.Entidades.Usuario usuarioDestinatario = repUsuario.BuscarPorCPF(pedido.Destinatario.CPF_CNPJ_SemFormato);
                Dominio.Entidades.Usuario usuarioRemetente = repUsuario.BuscarPorCPF(pedido.Remetente.CPF_CNPJ_SemFormato);

                string nota = string.Empty;
                if (chamado.TratativaDevolucao == SituacaoEntrega.Reentergue)
                    nota += string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.PedidoPertenceCarga, pedido.NumeroPedidoEmbarcador, chamado.Carga.CodigoCargaEmbarcador) + Localization.Resources.Chamado.ChamadoOcorrencia.EnviadoReentrega;

                if (chamado.TratativaDevolucao == SituacaoEntrega.Revertida)
                    nota += string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.PedidoPertenceCarga, pedido.NumeroPedidoEmbarcador, chamado.Carga.CodigoCargaEmbarcador) + Localization.Resources.Chamado.ChamadoOcorrencia.Revertido;

                if (chamado.TratativaDevolucao == SituacaoEntrega.NaoEntregue && devolucaoParcial)
                    nota += string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.PedidoPertenceCarga, pedido.NumeroPedidoEmbarcador, chamado.Carga.CodigoCargaEmbarcador) + Localization.Resources.Chamado.ChamadoOcorrencia.DevolvidoParcialmente;

                if (chamado.TratativaDevolucao == SituacaoEntrega.Rejeitado && !devolucaoParcial)
                    nota += string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.PedidoPertenceCarga, pedido.NumeroPedidoEmbarcador, chamado.Carga.CodigoCargaEmbarcador) + Localization.Resources.Chamado.ChamadoOcorrencia.DevolvidoTotalmente;

                if (usuarioDestinatario != null)
                    serNotificacaoPortal.GerarNotificacao(usuarioDestinatario, codigoClienteMultisoftware: clienteMultisoftware?.Codigo ?? 0, codigoObjeto: pedido.Codigo, URLPagina: "", nota: nota, icone: IconesNotificacao.ocorrenciaPedido, tipoNotificacao: TipoNotificacao.ocorrenciaPedido, tipoServicoMultisoftwareNotificar: AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, unitOfWork: unitOfWork);

                if (usuarioRemetente != null)
                    serNotificacaoPortal.GerarNotificacao(usuarioRemetente, codigoClienteMultisoftware: clienteMultisoftware?.Codigo ?? 0, codigoObjeto: pedido.Codigo, URLPagina: "", nota: nota, icone: IconesNotificacao.ocorrenciaPedido, tipoNotificacao: TipoNotificacao.ocorrenciaPedido, tipoServicoMultisoftwareNotificar: AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, unitOfWork: unitOfWork);
            }
        }

        public static void EnviarEmailCargaDevolucao(int codigoChamado, string stringConexao)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCodigo(codigoChamado);

                if (!string.IsNullOrWhiteSpace(chamado.MotivoChamado.Assunto))
                {
                    Dominio.Entidades.Cliente cliente = chamado.Carga.Pedidos.FirstOrDefault().Pedido.Remetente;

                    string subject = chamado.MotivoChamado.Assunto; //"Ocorrência de devolução. ";
                    string body = chamado.MotivoChamado.ConteudoEmail; //"Segue para conhecimento, estamos com uma ocorrência referente a carga (" + chamado.Carga.CodigoCargaEmbarcador + "), por gentileza autorizar esta devolução com urgência. <br/><br/>";

                    //body += "<h3>Quero ressaltar que á ocorrência em questão está gerando um custo diário para o cliente, no valor de aproximadamente R$ 700 reais por dia.</h3> <br/><br/>";

                    body += "Atenciosamente: <br/>";
                    body += "Backhaul <br/><br/>";

                    body += "<small>E-mail gerado automaticamente, favor não responder.</small> <br/>";
                    body += "<small>Para maiores informações entrar em contato através do telefone: (11) 3693-9610 </small>";


                    string emails = "";

                    if (cliente.EmailStatus == "A" && !string.IsNullOrEmpty(cliente.Email))
                    {
                        emails = cliente.Email;
                    }

#if DEBUG
                    emails = "rodrigo@multisoftware.com.br";
#endif

                    string[] splitEmail = emails.Split(';');
                    string email = splitEmail[0];
                    List<string> cc = new List<string>();

                    if (splitEmail.Length > 1)
                    {
                        for (int i = 1; i < splitEmail.Length; i++)
                        {
                            cc.Add(splitEmail[i]);
                        }
                    }

                    if (!Servicos.Email.EnviarEmailAutenticado(email, subject, body, unitOfWork, out string msg, "", null, null, cc.ToArray()))
                    {
                        Servicos.Log.TratarErro("Falha ao enviar notificação de devolução ao cliente: " + msg);
                    }
                    else
                    {
                        chamado.Notificado = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            //finally
            //{
            //    unitOfWork.Dispose();
            //}

        }

        public void EnviarEmailChamadoAberto(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            if (chamado == null || chamado.Situacao != SituacaoChamado.Aberto)
                return;

            string assunto = "Atendimento aberto";
            string mensagemCarga = chamado.Carga != null ? $" para a carga {chamado.Carga.CodigoCargaEmbarcador}" : string.Empty;
            string mensagem = $"O atendimento {chamado.Numero}{mensagemCarga} foi aberto";

            EnviarEmailChamado(chamado, assunto, mensagem, unitOfWork);
        }

        public void EnviarEmailChamadoAbertoDespesaMotorista(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;
            if (chamado.PerfisAcesso.Count == 0)
                return;

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            List<Dominio.Entidades.Usuario> usuarios = repositorioUsuario.BuscarPorPerfilEmbarcador(chamado.PerfisAcesso.Select(o => o.Codigo).ToList());
            bool despesaMotorista = chamado.MotivoChamado.PermiteAdicionarValorComoDespesaMotorista;

            if (usuarios.Count > 0)
            {
                string assunto = $"Atendimento de despesa de motorista";
                if (!despesaMotorista)
                    assunto = "Atendimento aberto";

                string mensagemCarga = chamado.Carga != null ? $" para a carga {chamado.Carga.CodigoCargaEmbarcador}" : string.Empty;
                string mensagem = $"O atendimento {chamado.Numero}{mensagemCarga} foi aberto";
                mensagem += despesaMotorista ? " referente a despesa de motorista:<br/>" : ":<br/>";
                mensagem += $"<br/>Cliente: {chamado.Cliente?.Nome}";
                mensagem += $"<br/>Motorista: {chamado.Motorista?.Nome}";
                mensagem += $"<br/>Motivo: {chamado.MotivoChamado.Descricao}";
                mensagem += $"<br/>Valor: {chamado.Valor.ToString("n2")}";
                if (despesaMotorista)
                    mensagem += $"<br/>Pago pelo Motorista: {(chamado.PagoPeloMotorista ? "Sim" : "Não")}";
                if (!string.IsNullOrWhiteSpace(chamado.Observacao))
                    mensagem += $"<br/>Observação: {chamado.Observacao}";

                EnviarEmailChamado(chamado, assunto, mensagem, unitOfWork, usuarios);
            }
        }

        public void EnviarEmailChamadoAssumido(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            string assunto = "Atendimento assumido";
            string mensagemCarga = chamado.Carga != null ? $" para a carga {chamado.Carga.CodigoCargaEmbarcador}" : string.Empty;
            string mensagem = $"O atendimento {chamado.Numero}{mensagemCarga} foi assumido pelo usuário {chamado.Responsavel.Descricao}";

            EnviarEmailChamado(chamado, assunto, mensagem, unitOfWork);
        }

        public void EnviarEmailChamadoDelegado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            string assunto = "Atendimento delegado";
            string mensagemCarga = chamado.Carga != null ? $" para a carga {chamado.Carga.CodigoCargaEmbarcador}" : string.Empty;
            string mensagem = $"O atendimento {chamado.Numero}{mensagemCarga} foi delegado para o usuário {chamado.Responsavel.Descricao}";

            EnviarEmailChamado(chamado, assunto, mensagem, unitOfWork);
        }

        public void EnviarEmailChamadoDelegadoParaSetor(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Setor setorFuncionario, Repositorio.UnitOfWork unitOfWork)
        {
            string assunto = "Atendimento delegado para setor";
            string mensagemCarga = chamado.Carga != null ? $" para a carga {chamado.Carga.CodigoCargaEmbarcador}" : string.Empty;
            string mensagem = $"O atendimento {chamado.Numero}{mensagemCarga} foi delegado para o setor {setorFuncionario.Descricao}";

            EnviarEmailChamado(chamado, assunto, mensagem, unitOfWork);
        }

        public void EnviarEmailChamadoFinalizado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            string assunto = "Atendimento finalizado";
            string mensagemCarga = chamado.Carga != null ? $" para a carga {chamado.Carga.CodigoCargaEmbarcador}" : string.Empty;
            string mensagem = $"O atendimento {chamado.Numero}{mensagemCarga} foi finalizado";

            EnviarEmailChamado(chamado, assunto, mensagem, unitOfWork);
        }

        public async Task<bool> DefinirAnalistasChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Usuario autor)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaRegras = await VerificarRegrasAutorizacaoOcorrenciaAsync(chamado);

            if (listaRegras.Count == 0)
                return false;

            List<Dominio.Entidades.Usuario> analistas = BuscarAnalistasChamado(listaRegras, chamado, autor, _tipoServicoMultisoftware, StringConexao, _unitOfWork);

            chamado.Analistas = analistas;
            chamado.RegrasAnalise = listaRegras;

            await repositorioChamado.AtualizarAsync(chamado);

            return true;
        }

        public async Task DefinirResponsavelChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Usuario autor)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaRegras = await VerificarRegrasAtendimentoChamadoAsync(chamado);

            if (listaRegras.Count == 0)
                return;

            List<Dominio.Entidades.Usuario> responsaveis = BuscarResponsaveisChamado(listaRegras);

            chamado.RegrasAtendimentoChamados = listaRegras.FirstOrDefault();
            chamado.Responsavel = responsaveis.FirstOrDefault();

            await repositorioChamado.AtualizarAsync(chamado);
        }

        public void DefinirAnalistasChamadoPorSetor(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Setor setorFuncionario, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            if (!PossuiAnaliseParaEfetuarOperacao(chamado, usuario.Codigo, unitOfWork))
                throw new ServicoException("Favor informar uma análise antes de efetuar a operação.");

            int codigoEmpresa = 0;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = usuario.Empresa.Codigo;

            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            List<Dominio.Entidades.Usuario> analistas = repositorioUsuario.BuscarUsuariosPorSetor(setorFuncionario.Codigo, codigoEmpresa, usuario.Codigo);

            if (analistas.Count == 0)
                throw new ServicoException("Não foi possível encontrar nenhum usuário para o setor informado.");

            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(StringConexao, null, tipoServicoMultisoftware, string.Empty);

            chamado.Responsavel = null;
            chamado.SetorResponsavel = setorFuncionario;
            chamado.Analistas = analistas;
            chamado.RegrasAnalise.Clear();

            repositorioChamado.Atualizar(chamado);
            Auditoria.Auditoria.Auditar(auditado, chamado, null, $"{usuario.Nome} delegou o chamado para o setor {setorFuncionario.Descricao}", unitOfWork);

            try
            {
                foreach (Dominio.Entidades.Usuario analista in analistas)
                {
                    string nota = string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.UsuarioDelegouChamadoAguardaPosicionamento, usuario.Nome, chamado.Numero);
                    servicoNotificacao.GerarNotificacao(analista, usuario, chamado.Codigo, "Chamados/ChamadoOcorrencia", nota, IconesNotificacao.agConfirmacao, TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro($"Falha ao gerar notificação para usuario: {ex.Message}");
            }
        }

        public static void VerificarDataRetornoENotificar(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            DateTime horaBase = DateTime.Now;
            horaBase = horaBase.AddSeconds(-horaBase.Second);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarChamadosPendentesDeRetorno(horaBase);

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
            {
                chamado.Notificado = true;
                repChamado.Atualizar(chamado);
                string nota = Localization.Resources.Chamado.ChamadoOcorrencia.ChamadoAguardandoResposta;
                serNotificacao.GerarNotificacao(chamado.Autor, chamado.Codigo, "Chamados/ChamadoOcorrencia", nota, IconesNotificacao.agConfirmacao, SmartAdminBgColor.yellow, TipoNotificacao.credito, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unitOfWork);
            }
        }

        public static void GerarCargaDevolucao(ref Dominio.Entidades.Embarcador.Chamados.Chamado chamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaChamado = chamado.Carga;

            int numeroSequencial = 0;
            if (configuracaoTMS.NumeroCargaSequencialUnico)
                numeroSequencial = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            else
                numeroSequencial = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, cargaChamado.Filial?.Codigo ?? 0);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaDevolucao = new Dominio.Entidades.Embarcador.Cargas.Carga()
            {
                AgConfirmacaoUtilizacaoCredito = cargaChamado.AgConfirmacaoUtilizacaoCredito,
                AgImportacaoCTe = cargaChamado.AgImportacaoCTe,
                AgImportacaoMDFe = cargaChamado.AgImportacaoMDFe,
                AguardandoEmissaoDocumentoAnterior = cargaChamado.AguardandoEmissaoDocumentoAnterior,
                AutorizouTodosCTes = cargaChamado.AutorizouTodosCTes,
                CargaCancelamento = cargaChamado.CargaCancelamento,
                CargaFechada = cargaChamado.CargaFechada,
                CargaIntegradaEmbarcador = cargaChamado.CargaIntegradaEmbarcador,
                CargaTransbordo = cargaChamado.CargaTransbordo,
                CodigoCargaEmbarcador = numeroSequencial.ToString(),
                NumeroSequenciaCarga = numeroSequencial,
                ControlaTempoParaEmissao = cargaChamado.ControlaTempoParaEmissao,
                //EmitindoCTes = cargaAntiga.EmitindoCTes,
                Empresa = cargaChamado.Empresa,
                ExigeNotaFiscalParaCalcularFrete = cargaChamado.ExigeNotaFiscalParaCalcularFrete,
                Filial = cargaChamado.Filial,
                FreteDeTerceiro = cargaChamado.FreteDeTerceiro,
                //GerandoIntegracoes = cargaAntiga.GerandoIntegracoes,
                GrupoPessoaPrincipal = cargaChamado.GrupoPessoaPrincipal,
                ModeloVeicularCarga = cargaChamado.ModeloVeicularCarga,
                MotivoPendencia = cargaChamado.MotivoPendencia,
                MotivoPendenciaFrete = cargaChamado.MotivoPendenciaFrete,
                NaoExigeVeiculoParaEmissao = cargaChamado.NaoExigeVeiculoParaEmissao,
                Operador = cargaChamado.Operador,
                PossuiPendencia = cargaChamado.PossuiPendencia,
                PrioridadeEnvioIntegracao = cargaChamado.PrioridadeEnvioIntegracao,
                problemaCTE = cargaChamado.problemaCTE,
                problemaAverbacaoCTe = cargaChamado.problemaAverbacaoCTe,
                problemaMDFe = cargaChamado.problemaMDFe,
                problemaNFS = cargaChamado.problemaNFS,
                Rota = cargaChamado.Rota,
                SegmentoGrupoPessoas = cargaChamado.SegmentoGrupoPessoas,
                SegmentoModeloVeicularCarga = cargaChamado.SegmentoModeloVeicularCarga,
                SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova,
                TabelaFrete = cargaChamado.TabelaFrete,
                //TipoContratacaoCarga = cargaAntiga.TipoContratacaoCarga,
                TipoDeCarga = cargaChamado.TipoDeCarga,
                TipoFreteEscolhido = cargaChamado.TipoFreteEscolhido,
                TipoOperacao = cargaChamado.TipoOperacao,
                ValorFrete = cargaChamado.ValorFrete,
                ValorFreteAPagar = cargaChamado.ValorFreteAPagar,
                ValorFreteEmbarcador = cargaChamado.ValorFreteEmbarcador,
                ValorFreteLeilao = cargaChamado.ValorFreteLeilao,
                ValorFreteLiquido = cargaChamado.ValorFreteLiquido,
                ValorFreteOperador = cargaChamado.ValorFreteOperador,
                ValorFreteTabelaFrete = cargaChamado.ValorFreteTabelaFrete,
                ValorICMS = cargaChamado.ValorICMS,
                ValorISS = cargaChamado.ValorISS,
                ValorRetencaoISS = cargaChamado.ValorRetencaoISS,
                ValorIBSEstadual = cargaChamado.ValorIBSEstadual,
                ValorIBSMunicipal = cargaChamado.ValorIBSMunicipal,
                ValorCBS = cargaChamado.ValorCBS,
                VeiculoIntegradoEmbarcador = cargaChamado.VeiculoIntegradoEmbarcador,
                Veiculo = cargaChamado.Veiculo,
                EmiteMDFeFilialEmissora = cargaChamado.EmiteMDFeFilialEmissora,
                EmpresaFilialEmissora = cargaChamado.EmpresaFilialEmissora,
                UtilizarCTesAnterioresComoCTeFilialEmissora = cargaChamado.UtilizarCTesAnterioresComoCTeFilialEmissora
            };
            cargaDevolucao.VeiculosVinculados = cargaChamado.VeiculosVinculados.ToList();
            cargaDevolucao.DadosSumarizados = null;
            repCarga.Inserir(cargaDevolucao);

            new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(cargaChamado?.Rota ?? null);

            // Replicar as fronteiras da carga antiga pra nova
            Repositorio.Embarcador.Cargas.CargaFronteira repCargaFronteira = new Repositorio.Embarcador.Cargas.CargaFronteira(unitOfWork);
            var serCargaFronteira = new Servicos.Embarcador.Carga.CargaFronteira(unitOfWork);
            var fronteirasCargaAntiga = serCargaFronteira.ObterFronteirasPorCarga(cargaChamado);
            repCargaFronteira.CopiarFronteirasParaCarga(fronteirasCargaAntiga, cargaDevolucao);

            if (cargaDevolucao.CargaFechada)
                Servicos.Log.TratarErro("Devolução - Fechou Carga (" + cargaDevolucao.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");


            cargaDevolucao.Protocolo = cargaDevolucao.Codigo;

            servicoCargaMotorista.AdicionarMotoristas(cargaChamado, cargaDevolucao);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaChamado.Pedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido.Clonar();
                cargaPedido.Pedido.ControleNumeracao = cargaPedido.Pedido.Codigo;
                repPedido.Atualizar(cargaPedido.Pedido);

                pedido.Veiculos = null;
                pedido.Motoristas = null;

                Dominio.Entidades.Cliente remetente = pedido.Destinatario;
                Dominio.Entidades.Cliente destinatario = pedido.Remetente;

                Dominio.Entidades.Cliente expedidor = pedido.Recebedor;
                Dominio.Entidades.Cliente recebedor = pedido.Expedidor;

                Dominio.Entidades.Localidade origem = pedido.Destino;
                Dominio.Entidades.Localidade destino = pedido.Origem;

                pedido.Remetente = remetente;
                pedido.Destinatario = destinatario;
                pedido.Expedidor = expedidor;
                pedido.Recebedor = recebedor;
                pedido.Origem = origem;
                pedido.Destino = destino;
                pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                Utilidades.Object.DefinirListasGenericasComoNulas(pedido);

                repPedido.Inserir(pedido);

                foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in cargaPedido.Pedido.Produtos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProdutoTransbordo = pedidoProduto.Clonar();
                    pedidoProdutoTransbordo.Pedido = pedido;
                    Utilidades.Object.DefinirListasGenericasComoNulas(pedidoProdutoTransbordo);
                    repPedidoProduto.Inserir(pedidoProdutoTransbordo);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoTransbordo = cargaPedido.Clonar();
                Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoTransbordo);
                cargaPedidoTransbordo.Carga = cargaDevolucao;
                cargaPedidoTransbordo.CargaOrigem = cargaDevolucao;
                cargaPedidoTransbordo.Pedido = pedido;

                Dominio.Entidades.Cliente expedidorCargaPedido = cargaPedido.Recebedor;
                Dominio.Entidades.Cliente recebedorCargaPedido = cargaPedido.Expedidor;

                Dominio.Entidades.Localidade origemCargaPedido = cargaPedido.Destino;
                Dominio.Entidades.Localidade destinoCargaPedido = cargaPedido.Origem;

                cargaPedidoTransbordo.Expedidor = expedidor;
                cargaPedidoTransbordo.Recebedor = recebedor;
                cargaPedidoTransbordo.Origem = origem;
                cargaPedidoTransbordo.Destino = destino;

                repCargaPedido.Inserir(cargaPedidoTransbordo);
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedido.Produtos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProdutoTransbordo = cargaPedidoProduto.Clonar();
                    cargaPedidoProdutoTransbordo.CargaPedido = cargaPedidoTransbordo;
                    Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoProdutoTransbordo);
                    repCargaPedidoProduto.Inserir(cargaPedidoProdutoTransbordo);
                }

                cargaPedidos.Add(cargaPedidoTransbordo);
            }

            serCarga.FecharCarga(cargaDevolucao, unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware, true);

            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref cargaDevolucao, cargaPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
            chamado.CargaDevolucao = cargaDevolucao;
        }

        public static void ResponderChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, tipoServicoMultisoftware, string.Empty);

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && !chamado.DataRetorno.HasValue)
                throw new ServicoException("O atendimento ainda não possui data de retorno.");

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                chamado.AosCuidadosDo = ChamadoAosCuidadosDo.Embarcador;

                if (chamado.Responsavel != null)
                {
                    string nota = string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.AtendimentoAguardandoResposta, chamado.Descricao);
                    serNotificacao.GerarNotificacao(chamado.Responsavel, chamado.Codigo, "Chamados/ChamadoOcorrencia", nota, IconesNotificacao.agConfirmacao, SmartAdminBgColor.yellow, TipoNotificacao.credito, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);
                }
            }
            else
            {
                if (!chamado.MotivoChamado.ChamadoDeveSerAbertoPeloEmbarcador)
                {
                    chamado.AosCuidadosDo = ChamadoAosCuidadosDo.Transporador;

                    string nota = string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.AtendimentoAguardandoRespostaPrazo, chamado.Descricao, chamado.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm"));
                    serNotificacao.GerarNotificacao(chamado.Autor, chamado.Codigo, "Chamados/ChamadoOcorrencia", nota, IconesNotificacao.agConfirmacao, SmartAdminBgColor.yellow, TipoNotificacao.credito, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unitOfWork);
                }
                else
                    chamado.AosCuidadosDo = ChamadoAosCuidadosDo.Embarcador;
            }

            repChamado.Atualizar(chamado);
        }

        public bool PossuiAnaliseParaEfetuarOperacao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, int codigoUsuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (chamado.MotivoChamado.ExigeAnaliseParaOperacao)
            {
                Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise ultimaAnalise = repChamadoAnalise.BuscarUltimaAnalisePorCodigo(chamado.Codigo);
                if (ultimaAnalise.Autor.Codigo != codigoUsuario)
                    return false;
            }

            return true;
        }

        public void ProcessarFinalizacaoAnaliseDevolucao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            GerarIntegracoes(chamado, unitOfWork, auditado, tipoServicoMultisoftware);
            if (chamado.Situacao != SituacaoChamado.AgIntegracao)
                FinalizarChamadoAnaliseDevolucao(chamado, unitOfWork, auditado, tipoServicoMultisoftware, clienteMultisoftware);
        }

        public void FinalizarChamadoAnaliseDevolucao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);

            if (chamado.Situacao == SituacaoChamado.Finalizado)
                return;

            if (!chamado.IsInitialized())
                chamado.Initialize();

            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            chamado.Situacao = SituacaoChamado.Finalizado;
            chamado.DataFinalizacao = DateTime.Now;
            chamado.ControleDuplicidade = chamado.Codigo;
            repChamado.Atualizar(chamado, auditado);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;
            if (cargaEntrega == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(cargaEntrega.Carga.Codigo, chamado.Codigo);
            if (cargaEvento != null)
            {
                servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
            }

            bool reentregaParcial = false;
            if (cargaEntrega.Situacao != SituacaoEntrega.Entregue)
            {
                cargaEntrega.Situacao = chamado.TratativaDevolucao;
                if (chamado.TratativaDevolucao == SituacaoEntrega.Reentergue && cargaEntrega.DevolucaoParcial)
                {
                    cargaEntrega.Situacao = SituacaoEntrega.NaoEntregue;
                    reentregaParcial = true;
                }

                if (cargaEntrega.PermitirEntregarMaisTarde && cargaEntrega.Situacao == SituacaoEntrega.Rejeitado && cargaEntrega.DataFim == null && cargaEntrega.DataInicio == null && cargaEntrega.DataConfirmacao == null)
                    AlterarSituacaoAgendamentoENotaFiscalEntrega(cargaEntrega, unitOfWork);
            }

            cargaEntrega.ChamadoEmAberto = false;
            repCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> listaCargaEntregaPedido = repositorioCargaEntregaPedido.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            if (cargaEntrega.Situacao == SituacaoEntrega.Rejeitado)
            {
                if (!cargaEntrega.DevolucaoParcial)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.LiberarPagamentoPorEntrega(cargaEntrega, true, configuracao, unitOfWork);

                    if (chamado.MotivoChamado.DisponibilizaParaReeentrega)
                    {
                        Servicos.Embarcador.Pedido.SeparacaoPedido.EncaminharPedidosParaReentrega(cargaEntrega, unitOfWork);

                        List<int> codigosPedidos = repositorioCargaEntregaPedido.BuscarCodigosPedidoReentregaAutomaticaPorCargaEntrega(cargaEntrega.Codigo);

                        if (codigosPedidos.Count > 0)
                            new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).AdicionarParaIntegracaoAutomaticamente(codigosPedidos, TipoRoteirizadorIntegracao.AtualizarPedido);
                    }
                }
            }
            else if (cargaEntrega.Situacao == SituacaoEntrega.Reentergue || reentregaParcial)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = reentregaParcial ? repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo) : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

                List<int> codigosPedidosReentrega = new List<int>();

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in listaCargaEntregaPedido)
                {
                    if (reentregaParcial && !cargaEntregaNotasFiscais.Any(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaEntregaPedido.CargaPedido.Codigo && o.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.AgReentrega))
                        continue;

                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedido.CargaPedido.Pedido;

                    pedido.NecessarioReentrega = true;
                    pedido.ReentregaSolicitada = true;
                    pedido.DataSolicitacaoReentrega = DateTime.Now;
                    pedido.PedidoTotalmenteCarregado = false;
                    pedido.PesoSaldoRestante += cargaEntregaPedido.CargaPedido.Peso;
                    pedido.ContagemReentrega++;
                    pedido.DisponivelParaSeparacao = true;
                    pedido.LocalExpedicao = cargaEntregaPedido.CargaPedido.Expedidor ?? pedido.Remetente;

                    repositorioPedido.Atualizar(pedido);

                    if (pedido.TipoOperacao?.ConfiguracaoPedido?.EnviarPedidoReentregaAutomaticamenteRoteirizar ?? false)
                        codigosPedidosReentrega.Add(pedido.Codigo);

                    //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                    Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. Chamado.FinalizarChamadoAnaliseDevolucao", "SaldoPedido");

                }

                if (codigosPedidosReentrega.Count > 0)
                    new Servicos.Embarcador.Integracao.IntegracaoPedidoRoterizador(unitOfWork).AdicionarParaIntegracaoAutomaticamente(codigosPedidosReentrega, TipoRoteirizadorIntegracao.AtualizarPedido);
            }
            else if (cargaEntrega.Situacao == SituacaoEntrega.Revertida)
            {
                //feito para atender a necessidade pontualmente, mas esse processo precisa ser revisto, não devemos mudar o status da nota até aprovação do atendimento, ou seja, o ajuste tem que ser na rejeição e aqui apenas confirmar a mudança de status ou não.
                List<SituacaoNotaFiscal> situacaoNotasFiscaisNaoAtualizar = new List<SituacaoNotaFiscal>();
                situacaoNotasFiscaisNaoAtualizar.Add(SituacaoNotaFiscal.Entregue);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ProcessarSituacaoEntregaXMLNotaFiscal(cargaEntrega, SituacaoNotaFiscal.AgEntrega, situacaoNotasFiscaisNaoAtualizar, unitOfWork);
            }

            AtualizarDataPrevisaoEntrega(chamado, listaCargaEntregaPedido, unitOfWork, auditado);

            if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.NaoPossuiEntregasPendentes(cargaEntrega.Carga, unitOfWork))
                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(cargaEntrega.Carga.Codigo, cargaEntrega.DataRejeitado ?? DateTime.Now, auditado, tipoServicoMultisoftware, clienteMultisoftware, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, unitOfWork))
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega.Carga, $"Fim de viagem informado automaticamente ao salvar análise de atendimento", unitOfWork);


            if (cargaEntrega != null)
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = cargaEntrega.MotivoRejeicao;

                OrigemSituacaoEntrega origem = !string.IsNullOrEmpty(chamado?.IdOcorrenciaTrizy) ? OrigemSituacaoEntrega.App : OrigemSituacaoEntrega.UsuarioMultiEmbarcador;

                if (tipoDeOcorrencia == null)
                    tipoDeOcorrencia = chamado?.MotivoChamado?.TipoOcorrencia ?? null;


                if (tipoDeOcorrencia != null)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

                    bool existeOcorrenciaEntrega = repOcorrenciaEntrega.ExisteOcorrenciaPorCargaEntregaETipoOcorrencia(cargaEntrega.Codigo, cargaEntrega.DataRejeitado ?? DateTime.Now, tipoDeOcorrencia.Codigo);

                    if (!existeOcorrenciaEntrega)
                        Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaRejeicao(cargaEntrega, cargaEntrega.DataRejeitado ?? DateTime.Now, tipoDeOcorrencia, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, "", 0m, configuracao, tipoServicoMultisoftware, clienteMultisoftware, origem, unitOfWork, auditado, null, null);
                }
            }

            if (!unitOfWork.IsActiveTransaction())
                EnviarEmailChamadoFinalizado(chamado, unitOfWork);

            new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(chamado, TipoNotificacaoApp.TratativaDoAtendimento);

            if (!string.IsNullOrEmpty(cargaEntrega.Carga.IDIdentificacaoTrizzy) && !string.IsNullOrEmpty(cargaEntrega.IdTrizy) && chamado.MotivoChamado?.BloquearParadaAppTrizy == true)
            {
                Task t = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Repositorio.UnitOfWork _unitOfWork = new Repositorio.UnitOfWork(unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AlternarBloqueioParada(false, cargaEntrega.Carga.IDIdentificacaoTrizzy, cargaEntrega.IdTrizy, _unitOfWork);
                        _unitOfWork.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }
                });
            }
        }

        public void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)//Por enquanto só embarcador vai usar
                return;

            Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);

            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
            Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento servAlertaCargaEvento = new Servicos.Embarcador.Carga.AlertaCarga.AlertaCargaEvento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> chamadoIntegracoes = repChamadoIntegracao.BuscarIntegracaoPendente(20);
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = new List<Dominio.Entidades.Embarcador.Chamados.Chamado>();

            foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao in chamadoIntegracoes)
            {
                if (chamadoIntegracao.TipoIntegracao.Tipo == TipoIntegracao.Isis)
                    new Integracao.Isis.IntegracaoIsis(unitOfWork).IntegrarChamado(chamadoIntegracao);
                else if (chamadoIntegracao.TipoIntegracao.Tipo == TipoIntegracao.Marilan)
                    new Integracao.Marilan.IntegracaoMarilan(unitOfWork).IntegrarChamadoOcorrencia(chamadoIntegracao);
                else if (chamadoIntegracao.TipoIntegracao.Tipo == TipoIntegracao.JJ)
                    new Integracao.JJ.IntegracaoJJ(unitOfWork).IntegrarChamado(chamadoIntegracao);
                else if (chamadoIntegracao.TipoIntegracao.Tipo == TipoIntegracao.CassolEventosEntrega)
                {
                    new Integracao.Cassol.IntegracaoCassolEventosEntrega(unitOfWork).IntegrarEventoEntregaChamadoOcorrencia(chamadoIntegracao);
                }

                else
                {
                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    chamadoIntegracao.ProblemaIntegracao = "Tipo de integração não implementada";
                    chamadoIntegracao.NumeroTentativas++;
                }

                repChamadoIntegracao.Atualizar(chamadoIntegracao);

                if (!chamados.Contains(chamadoIntegracao.Chamado))
                    chamados.Add(chamadoIntegracao.Chamado);
            }

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
            {
                if (chamado.Situacao == SituacaoChamado.Finalizado)//Se finalizado com integração rejeitada, não necessita efetuar esses procedimentos
                    continue;

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao> integracoes = repChamadoIntegracao.BuscarPorChamado(chamado.Codigo);

                if (integracoes.Any(obj => obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao))
                    chamado.Situacao = SituacaoChamado.FalhaIntegracao;
                else if (integracoes.Any(obj => obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao))
                    chamado.Situacao = SituacaoChamado.AgIntegracao;
                else if (integracoes.All(obj => obj.SituacaoIntegracao == SituacaoIntegracao.Integrado))
                    FinalizarChamadoAnaliseDevolucao(chamado, unitOfWork, auditado, tipoServicoMultisoftware, clienteMultisoftware);

                repChamado.Atualizar(chamado);

                if (chamado.Situacao == SituacaoChamado.Finalizado && chamado.CargaEntrega != null && chamado.CargaEntrega.Carga != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento cargaEvento = repCargaEvento.BuscarAlertaPorCargaChamado(chamado.CargaEntrega.Carga.Codigo, chamado.Codigo);
                    if (cargaEvento != null)
                    {
                        servAlertaCargaEvento.EfetuarTratativaCargaEvento(cargaEvento, "Finalizado após finalização do atendimento");
                        servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, cargaEvento);
                    }
                }
            }
        }

        public void EnviarEmailMudancaEscalationList(Dominio.Entidades.Embarcador.Chamados.NivelAtendimento nivelAtendimento, Dominio.Entidades.Embarcador.Operacional.OperadorLogistica novoOperador, string clienteURLAcesso, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Servicos.Email servicoEmail = new Servicos.Email(unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork).BuscarEmailEnviaDocumentoAtivo();
            if (configuracaoEmail == null)
                return;

            List<int> notas = new List<int>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoDoCliente = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (nivelAtendimento.Chamado.CargaEntrega != null)
            {
                cargasPedidoDoCliente = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(nivelAtendimento.Chamado.CargaEntrega.Codigo);
                notasFiscais = repCargaEntregaNotaFiscal.BuscarNotaFiscalPorCargaEntrega(nivelAtendimento.Chamado.CargaEntrega.Codigo);
            }
            else
            {
                if (nivelAtendimento.Chamado.Carga != null)
                {
                    if (nivelAtendimento.Chamado.Cliente != null)
                        cargasPedidoDoCliente = (from o in nivelAtendimento.Chamado.Carga.Pedidos where o.Pedido.Destinatario.Codigo == nivelAtendimento.Chamado.Cliente.Codigo select o).ToList();
                    else
                        cargasPedidoDoCliente = (from o in nivelAtendimento.Chamado.Carga.Pedidos select o).ToList();
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in cargasPedidoDoCliente)
            {
                if (notasFiscais == null)
                    notasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(pedido.Codigo);

                if (notasFiscais != null && notasFiscais.Count > 0)
                    notas.AddRange((from o in notasFiscais select o.XMLNotaFiscal.Numero).ToList());
            }

            notas = notas.Distinct().ToList();

            string destinatario = novoOperador.Usuario.Email;
            string assunto = $"Atendimento {nivelAtendimento.Chamado.Numero} - Este atendimento foi escalado para você";

            string mensagem = $"Olá, {novoOperador?.Usuario?.Nome}";
            mensagem += "<br/>";
            mensagem += $"<br/>O atendimento de número {nivelAtendimento.Chamado.Numero} foi escalado para você porque ultrapassou o tempo máximo ({(DateTime.Now - (nivelAtendimento.DataLimite ?? DateTime.MinValue)).TotalMinutes} minutos) para a completa resolução.";
            mensagem += "<br/>";
            mensagem += $"<br/>Acesse o link abaixo ou entre em contato com o usuário {nivelAtendimento?.Chamado?.Responsavel?.Nome} para mais detalhes.";
            mensagem += "<br/>";
            mensagem += $"<br/>https://{clienteURLAcesso}/#Chamados/ChamadoOcorrencia?Atendimento={nivelAtendimento.Chamado.Numero}";
            mensagem += "<br/>";
            mensagem += "<br/>Detalhes do atendimento:";
            mensagem += $"<br/>Número: {nivelAtendimento.Chamado.Numero}";
            mensagem += $"<br/>Carga: {nivelAtendimento.Chamado.Carga.Numero}";
            mensagem += $"<br/>Data de abertura do atendimento: {nivelAtendimento.DataCriacao.ToString("dd/MM/yyyy HH:mm")}";
            mensagem += $"<br/>Cliente: {nivelAtendimento.Chamado.Cliente.Nome}";

            if (nivelAtendimento.Chamado?.Cliente?.NomeFantasia != null)
                mensagem += $"<br/>Fantasia:  {nivelAtendimento.Chamado.Cliente?.NomeFantasia}";

            mensagem += $"<br/>Origem: {nivelAtendimento.Chamado.Carga.DadosSumarizados.Origens}";
            mensagem += $"<br/>Destino: {nivelAtendimento.Chamado.Carga.DadosSumarizados.Destinos}";
            mensagem += $"<br/>Nº pedido no embarcador: {nivelAtendimento.Chamado.Carga.DadosSumarizados?.NumeroPedidoEmbarcador}";
            mensagem += $"<br/>Notas Fiscais: {string.Join(", ", notas)}";

            if (nivelAtendimento.Chamado?.CargaEntrega?.Cliente?.Endereco != null)
                mensagem += $"<br/> Endereço : {nivelAtendimento.Chamado.CargaEntrega?.Cliente?.Endereco}";

            mensagem += $"<br/> Motivo da ocorrência: {nivelAtendimento.Chamado.MotivoChamado?.Descricao}";

            if (!string.IsNullOrWhiteSpace(nivelAtendimento.Chamado.Carga?.Veiculo?.NumeroFrota))
                mensagem += $"<br/> Frota: {nivelAtendimento.Chamado.Carga?.Veiculo?.NumeroFrota}";

            if (nivelAtendimento.Chamado?.CargaEntrega?.DataPrevista != null)
                mensagem += $"<br/> Previsão da entrega: {nivelAtendimento.Chamado?.CargaEntrega?.DataPrevista?.ToString("dd/MM/yyyy")}";

            if (!string.IsNullOrWhiteSpace(nivelAtendimento.Chamado.Observacao))
                mensagem += $"<br/> Observação: {nivelAtendimento.Chamado?.Observacao}";

            StringBuilder corpoMensagem = new StringBuilder();
            corpoMensagem.AppendLine(@"<div style=""font-family: Arial;"">");
            corpoMensagem.AppendLine($@"    <p style=""margin:0px"">{mensagem}</p>");
            corpoMensagem.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            corpoMensagem.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            corpoMensagem.AppendLine("</div>");

            List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();

            foreach (var anexo in nivelAtendimento.Chamado.Anexos)
            {
                string caminhoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), anexo.GuidArquivo + "." + anexo.ExtensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoAnexo))
                    continue;

                byte[] anexoArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoAnexo);
                anexos.Add(new System.Net.Mail.Attachment(new MemoryStream(anexoArquivo), $"Anexo: {anexo.NomeArquivo}"));
            }

            if (!Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, destinatario, new string[] { }, new string[] { }, assunto, corpoMensagem.ToString(), configuracaoEmail.Smtp, out string erro, configuracaoEmail.DisplayEmail, anexos, "", false, "", configuracaoEmail.PortaSmtp, unitOfWork))
                Log.TratarErro($"Falha ao enviar o e-mail do chamado: {erro}");
        }

        public void EnviarEmailAtendimentoDelegado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, string clienteURLAcesso, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, bool setor = false)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(unitOfWork);

            Servicos.Email servicoEmail = new Servicos.Email(unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork).BuscarEmailEnviaDocumentoAtivo();
            if (configuracaoEmail == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
            if (!configuracaoChamado.HabilitarArvoreDecisaoEscalationList)
                return;

            List<int> notas = new List<int>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoDoCliente = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (chamado.CargaEntrega != null)
            {
                cargasPedidoDoCliente = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(chamado.CargaEntrega.Codigo);
                notasFiscais = repCargaEntregaNotaFiscal.BuscarNotaFiscalPorCargaEntrega(chamado.CargaEntrega.Codigo);
            }
            else
            {
                if (chamado.Carga != null)
                {
                    if (chamado.Cliente != null)
                        cargasPedidoDoCliente = (from o in chamado.Carga.Pedidos where o.Pedido.Destinatario.Codigo == chamado.Cliente.Codigo select o).ToList();
                    else
                        cargasPedidoDoCliente = (from o in chamado.Carga.Pedidos select o).ToList();
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in cargasPedidoDoCliente)
            {
                if (notasFiscais == null)
                    notasFiscais = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(pedido.Codigo);

                if (notasFiscais != null && notasFiscais.Count > 0)
                    notas.AddRange((from o in notasFiscais select o.XMLNotaFiscal.Numero).ToList());
            }

            notas = notas.Distinct().ToList();

            List<string> destinatarios = new List<string>();

            string assunto = string.Empty;
            string mensagem = string.Empty;

            if (setor)
            {
                destinatarios.AddRange(repUsuario.BuscarEmailUsuariosPorSetor(chamado.SetorResponsavel.Codigo));
                assunto = $"Atendimento {chamado.Numero} - Este atendimento foi delegado para o seu setor";
                mensagem = $"Olá";
            }
            else
            {
                destinatarios.Add(chamado.Responsavel.Email);
                assunto = $"Atendimento {chamado.Numero} - Este atendimento foi delegado para você";
                mensagem = $"Olá, {chamado.Responsavel.Nome}";
            }

            mensagem += "<br/>";
            mensagem += $"<br/>O atendimento de número {chamado.Numero} foi delegado para você";
            mensagem += "<br/>";
            mensagem += $"<br/>Acesse o link abaixo ou entre em contato com o usuário {usuario.Nome} para mais detalhes.";
            mensagem += "<br/>";
            mensagem += $"<br/>https://{clienteURLAcesso}/#Chamados/ChamadoOcorrencia?Atendimento={chamado.Numero}";
            mensagem += "<br/>";
            mensagem += "<br/>Detalhes do atendimento:";
            mensagem += $"<br/>Número: {chamado.Numero}";
            mensagem += $"<br/>Carga: {chamado.Carga.Numero}";
            mensagem += $"<br/>Data de abertura do atendimento: {chamado.DataCriacao.ToString("dd/MM/yyyy HH:mm")}";
            mensagem += $"<br/>Cliente: {chamado.Cliente.Nome}";
            mensagem += $"<br/>Fansia:  {chamado.Cliente.NomeFantasia}";
            mensagem += $"<br/>Origem: {chamado.Carga.DadosSumarizados.Origens}";
            mensagem += $"<br/>Destino: {chamado.Carga.DadosSumarizados.Destinos}";
            mensagem += $"<br/>Nº pedido no embarcador: {chamado.Carga.DadosSumarizados.NumeroPedidoEmbarcador}";
            mensagem += $"<br/>Notas Fiscais: {string.Join(", ", notas)}";

            if (chamado.CargaEntrega?.Cliente?.Endereco != null)
                mensagem += $"<br/> Endereço : {chamado.CargaEntrega?.Cliente?.Endereco}";

            mensagem += $"<br/> Motivo da ocorrência: {chamado.MotivoChamado?.Descricao}";

            if (!string.IsNullOrWhiteSpace(chamado.Carga?.Veiculo?.NumeroFrota))
                mensagem += $"<br/> Frota: {chamado.Carga?.Veiculo?.NumeroFrota}";

            if (chamado.CargaEntrega?.DataPrevista != null)
                mensagem += $"<br/> Previsão da entrega: {chamado.CargaEntrega?.DataPrevista?.ToString("dd/MM/yyyy")}";

            if (!string.IsNullOrWhiteSpace(chamado.Observacao))
                mensagem += $"<br/> Observação: {chamado.Observacao}";

            mensagem += "<br/>";

            StringBuilder corpoMensagem = new StringBuilder();
            corpoMensagem.AppendLine(@"<div style=""font-family: Arial;"">");
            corpoMensagem.AppendLine($@"    <p style=""margin:0px"">{mensagem}</p>");
            corpoMensagem.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            corpoMensagem.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            corpoMensagem.AppendLine("</div>");

            List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();

            foreach (var anexo in chamado.Anexos)
            {
                string caminhoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), anexo.GuidArquivo + "." + anexo.ExtensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoAnexo))
                    continue;

                byte[] anexoArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoAnexo);
                anexos.Add(new System.Net.Mail.Attachment(new MemoryStream(anexoArquivo), $"Anexo: {anexo.NomeArquivo}"));
            }

            foreach (string destinatario in destinatarios)
            {
                if (!Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, destinatario, new string[] { }, new string[] { }, assunto, corpoMensagem.ToString(), configuracaoEmail.Smtp, out string erro, configuracaoEmail.DisplayEmail, anexos, "", false, "", configuracaoEmail.PortaSmtp, unitOfWork))
                    Log.TratarErro($"Falha ao enviar o e-mail do chamado: {erro}");
            }
        }

        public void GerarIntegracoes(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)//Por enquanto só embarcador vai usar
                return;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<TipoIntegracao> tiposIntegracaoGeracao = new List<TipoIntegracao>();
            tiposIntegracaoGeracao.Add(TipoIntegracao.Isis);
            tiposIntegracaoGeracao.Add(TipoIntegracao.Marilan);
            tiposIntegracaoGeracao.Add(TipoIntegracao.JJ);
            tiposIntegracaoGeracao.Add(TipoIntegracao.CassolEventosEntrega);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoGeracao);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Isis:
                        IntegracaoIsis(chamado, tipoIntegracao, unitOfWork, auditado);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marilan:
                        IntegracaoMarilan(chamado, tipoIntegracao, unitOfWork, auditado);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.JJ:
                        if (chamado?.MotivoChamado?.IntegrarComDansales ?? false)
                            IntegracaoJJ(chamado, tipoIntegracao, unitOfWork, auditado);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CassolEventosEntrega:
                        IntegracaoCassolEventosEntregaAtendimentos(chamado, tipoIntegracao, unitOfWork, auditado);
                        break;
                }
            }
        }

        public void GerarPagamentoMotorista(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, double pessoaTituloPagar, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;
            if (chamado.Situacao != SituacaoChamado.Finalizado && chamado.Situacao != SituacaoChamado.LiberadaOcorrencia)
                return;
            if (!chamado.MotivoChamado.PermiteAdicionarValorComoAdiantamentoMotorista || chamado.MotivoChamado.PagamentoMotoristaTipo == null || chamado.Valor == 0 || chamado.Carga == null || chamado.Motorista == null)
                return;

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            if (repPagamentoMotorista.ContemPorChamado(chamado.Codigo, chamado.Motorista.Codigo, chamado.Valor))
                return;

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS();

            pagamentoMotorista.Usuario = usuario;
            pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgInformacoes;
            pagamentoMotorista.Data = DateTime.Now.Date;
            pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Iniciada;
            pagamentoMotorista.Numero = repPagamentoMotorista.BuscarProximoNumero();
            pagamentoMotorista.PagamentoLiberado = true;

            pagamentoMotorista.Carga = chamado.Carga;
            pagamentoMotorista.Chamado = chamado;
            pagamentoMotorista.Motorista = chamado.Motorista;
            pagamentoMotorista.PagamentoMotoristaTipo = chamado.MotivoChamado.PagamentoMotoristaTipo;
            pagamentoMotorista.DataPagamento = DateTime.Now;
            pagamentoMotorista.DataVencimentoTituloPagar = pagamentoMotorista.DataPagamento;
            pagamentoMotorista.Valor = chamado.Valor;
            pagamentoMotorista.SaldoDescontado = chamado.SaldoDescontadoMotorista;
            pagamentoMotorista.SaldoDiariaMotorista = chamado.Motorista?.SaldoDiaria ?? 0;
            pagamentoMotorista.Observacao = $"GERADO A PARTIR DO ATENDIMENTO Nº {chamado.Numero} REFERENTE AO ADIANTAMENTO DO MOTORISTA";
            if (!string.IsNullOrWhiteSpace(chamado.Observacao))
                pagamentoMotorista.Observacao += " " + chamado.Observacao;
            pagamentoMotorista.PessoaTituloPagar = pessoaTituloPagar > 0 ? repCliente.BuscarPorCPFCNPJ(pessoaTituloPagar) : null;

            if (pagamentoMotorista.Motorista.PlanoAcertoViagem != null && usuario.PlanoConta != null)
            {
                pagamentoMotorista.PlanoDeContaDebito = usuario.PlanoConta;
                pagamentoMotorista.PlanoDeContaCredito = pagamentoMotorista.Motorista.PlanoAcertoViagem;
            }
            else if (pagamentoMotorista.PagamentoMotoristaTipo.GerarMovimentoAutomatico && pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento != null)
            {
                pagamentoMotorista.PlanoDeContaDebito = pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento.PlanoDeContaCredito;
                pagamentoMotorista.PlanoDeContaCredito = pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento.PlanoDeContaDebito;
            }

            if (pagamentoMotorista.PagamentoMotoristaTipo != null && pagamentoMotorista.PagamentoMotoristaTipo.GerarMovimentoAutomatico && (pagamentoMotorista.PlanoDeContaCredito == null || pagamentoMotorista.PlanoDeContaDebito == null))
                throw new ControllerException("Favor selecione os Planos de Contas para este Tipo de Pagamento.");

            if (repPagamentoMotorista.ContemPagamentoEmAberto(pagamentoMotorista.Motorista.Codigo))
                throw new ControllerException("Já existe um pagamento em aberto para este motorista, favor finalize o mesmo antes de iniciar um novo.");

            if (repPagamentoMotorista.ContemPagamentoIdentico(pagamentoMotorista.DataPagamento.Date, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PagamentoMotoristaTipo.Codigo, pagamentoMotorista.Valor))
                throw new ControllerException("Já existe um pagamento com a mesma Data do Pagamento, Motorista, Tipo do Pagamento e Valor.");

            Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotorista, unitOfWork, tipoServicoMultisoftware);

            repPagamentoMotorista.Inserir(pagamentoMotorista, auditado);

            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;

            if (Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, tipoServicoMultisoftware, unitOfWork, usuario, unitOfWork.StringConexao, auditado, out bool contemAprovadorIgualAoOperador))
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AutorizacaoPendente;
                pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.AgAutorizacao;
            }
            else
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgIntegracao;
                pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;
            }

            if (contemAprovadorIgualAoOperador)
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotorista.Codigo, pagamentoMotorista.Usuario.Codigo);

                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotorista.Usuario, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, configuracaoEmbarcador);

                string msgRetorno = "";
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetorno, tipoServicoMultisoftware, auditado, unitOfWork.StringConexao, configuracaoEmbarcador, pagamentoMotorista.Usuario);
                Servicos.Auditoria.Auditoria.Auditar(auditado, pagamentoMotorista, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unitOfWork);
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

            if (tipoIntegracaoPagamentoMotorista.PossuiIntegracao())
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
                pagamentoMotoristaIntegracaoEnvio.Data = DateTime.Now.Date;
                pagamentoMotoristaIntegracaoEnvio.NumeroTentativas = 0;
                pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS = pagamentoMotorista;
                pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista = tipoIntegracaoPagamentoMotorista;

                repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvio);

                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
            }
            else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
            {
                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                {
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                }
                else
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                    if (configuracaoEmbarcador.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        string msgRetorno = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, configuracaoEmbarcador.TipoMovimentoPagamentoMotorista, auditado, pagamentoMotorista.Usuario, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware);
                    }
                }
            }
        }

        public void InserirDespesaAcertoViagem(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;
            if (chamado.Situacao != SituacaoChamado.Finalizado && chamado.Situacao != SituacaoChamado.LiberadaOcorrencia)
                return;
            if (!chamado.MotivoChamado.PermiteAdicionarValorComoDespesaMotorista || chamado.MotivoChamado.Justificativa == null || chamado.Valor == 0 || chamado.Carga == null || chamado.Motorista == null)
                return;

            Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
            if (repAcertoOutraDespesa.ContemPorChamado(chamado.Codigo))
                return;

            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento repAcertoVeiculoSegmento = new Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento(unitOfWork);

            Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarAcertoAberto(chamado.Motorista.Codigo);
            if (acertoViagem == null)
                return;
            Dominio.Entidades.Veiculo veiculo = chamado.Carga.Veiculo;
            if (veiculo == null)
                return;

            if (!acertoViagem.Veiculos.Any(o => o.Veiculo.Codigo == veiculo.Codigo))
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo veiculoAcerto = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo();
                veiculoAcerto.AcertoViagem = acertoViagem;
                veiculoAcerto.Veiculo = veiculo;
                repAcertoVeiculo.Inserir(veiculoAcerto);

                Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento acertoVeiculoSegmento = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento();
                acertoVeiculoSegmento.AcertoViagem = acertoViagem;
                acertoVeiculoSegmento.Veiculo = veiculo;
                repAcertoVeiculoSegmento.Inserir(acertoVeiculoSegmento);
            }

            Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa despesa = new Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa();
            despesa.AcertoViagem = acertoViagem;
            despesa.Data = chamado.DataCriacao;
            despesa.Observacao = $"GERADO A PARTIR DO ATENDIMENTO Nº {chamado.Numero} REFERENTE A DESPESA DO MOTORISTA";
            despesa.NumeroDocumento = chamado.Numero;
            despesa.Pessoa = chamado.MotivoChamado.FornecedorDespesa ?? chamado.Cliente;
            if (despesa.Pessoa != null)
                despesa.NomeFornecedor = despesa.Pessoa.Nome.Length > 44 ? despesa.Pessoa.Nome.Substring(0, 44) : despesa.Pessoa.Nome;
            despesa.Quantidade = 1;
            despesa.Valor = chamado.Valor;
            despesa.Veiculo = veiculo;
            despesa.Justificativa = chamado.MotivoChamado.Justificativa;
            despesa.TipoPagamento = chamado.PagoPeloMotorista ? TipoPagamentoAcertoDespesa.Motorista : TipoPagamentoAcertoDespesa.Empresa;
            despesa.Chamado = chamado;

            repAcertoOutraDespesa.Inserir(despesa, auditado);

            Servicos.Auditoria.Auditoria.Auditar(auditado, despesa.AcertoViagem, null, "Despesa " + despesa.Descricao + " inserida pelo atendimento " + chamado.Numero, unitOfWork);
        }

        public void InserirDescontoAcertoViagem(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;
            if (chamado.Situacao != SituacaoChamado.Finalizado && chamado.Situacao != SituacaoChamado.LiberadaOcorrencia)
                return;
            if (!chamado.MotivoChamado.PermiteAdicionarValorComoDescontoMotorista || chamado.MotivoChamado.Justificativa == null || chamado.Valor == 0 || chamado.Carga == null || chamado.Motorista == null)
                return;

            Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
            if (repAcertoDesconto.ContemPorChamado(chamado.Codigo))
                return;

            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento repAcertoVeiculoSegmento = new Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento(unitOfWork);

            Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarAcertoAberto(chamado.Motorista.Codigo);
            if (acertoViagem == null)
                return;
            Dominio.Entidades.Veiculo veiculo = chamado.Carga.Veiculo;
            if (veiculo == null)
                return;

            if (!acertoViagem.Veiculos.Any(o => o.Veiculo.Codigo == veiculo.Codigo))
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo veiculoAcerto = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo();
                veiculoAcerto.AcertoViagem = acertoViagem;
                veiculoAcerto.Veiculo = veiculo;
                repAcertoVeiculo.Inserir(veiculoAcerto);

                Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento acertoVeiculoSegmento = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento();
                acertoVeiculoSegmento.AcertoViagem = acertoViagem;
                acertoVeiculoSegmento.Veiculo = veiculo;
                repAcertoVeiculoSegmento.Inserir(acertoVeiculoSegmento);
            }

            Dominio.Entidades.Embarcador.Acerto.AcertoDesconto desconto = new Dominio.Entidades.Embarcador.Acerto.AcertoDesconto()
            {
                AcertoViagem = acertoViagem,
                Data = chamado.DataCriacao,
                DataBaseCRT = chamado.DataCriacao,
                Justificativa = chamado.MotivoChamado.Justificativa,
                Motivo = $"GERADO A PARTIR DO ATENDIMENTO Nº {chamado.Numero} {chamado.Observacao}",
                MoedaCotacaoBancoCentral = MoedaCotacaoBancoCentral.Real,
                ValorDesconto = chamado.Valor,
                ValorMoedaCotacao = 0,
                ValorOriginalMoedaEstrangeira = 0,
                Veiculo = veiculo,
                Chamado = chamado
            };
            repAcertoDesconto.Inserir(desconto, auditado);

            Servicos.Auditoria.Auditoria.Auditar(auditado, desconto.AcertoViagem, null, "Desconto " + desconto.Descricao + " inserido pelo atendimento " + chamado.Numero, unitOfWork);
        }

        public void GerarAtendimentoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente cliente, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado()
            {
                Observacao = "Chamado gerado automaticamente através do Tipo de Ocorrência",
                MotivoChamado = tipoDeOcorrencia.MotivoChamadoGeracaoAutomatica,
                Carga = carga,
                Empresa = carga.Empresa,
                Cliente = cliente,
                Destinatario = carga.Pedidos.FirstOrDefault()?.Pedido?.Destinatario
            };

            AbrirChamado(objetoChamado, usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, unitOfWork);
        }

        public DateTime AdicionarTempoDiaUtil(DateTime dataInicial, int minutosParaAdicionar)
        {
            DateTime dataAtual = dataInicial;
            TimeSpan horarioInicio = new TimeSpan(8, 0, 0);
            TimeSpan horarioFim = new TimeSpan(18, 0, 0);

            while (minutosParaAdicionar > 0)
            {

                if (dataAtual.DayOfWeek == DayOfWeek.Saturday)
                    dataAtual = dataAtual.AddDays(2).Date.Add(horarioInicio);
                else if (dataAtual.DayOfWeek == DayOfWeek.Sunday)
                    dataAtual = dataAtual.AddDays(1).Date.Add(horarioInicio);

                TimeSpan horarioAtual = dataAtual.TimeOfDay;


                if (horarioAtual < horarioInicio)
                    dataAtual = dataAtual.Date.Add(horarioInicio);

                else if (horarioAtual >= horarioFim)
                    dataAtual = dataAtual.AddDays(1).Date.Add(horarioInicio);

                int minutosRestantesNoDia = (int)(horarioFim - dataAtual.TimeOfDay).TotalMinutes;

                if (minutosParaAdicionar <= minutosRestantesNoDia)
                {
                    dataAtual = dataAtual.AddMinutes(minutosParaAdicionar);
                    minutosParaAdicionar = 0;
                }
                else
                {
                    minutosParaAdicionar -= minutosRestantesNoDia;
                    dataAtual = dataAtual.AddDays(1).Date.Add(horarioInicio);
                }
            }

            return dataAtual;
        }

        public void FinalizarAtendimentosEmAberto(List<int> numerosNotasFiscais, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string origem, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamadosDaNota = null;
            var repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);

            if (numerosNotasFiscais != null && numerosNotasFiscais.Count > 0)
                listaChamadosDaNota = repChamado.BuscarChamadoAtendimentosEmAbertoPorNotasFiscais(numerosNotasFiscais);

            if (cargaEntrega != null)
                listaChamadosDaNota = repChamado.BuscarAtendimentosPorEntregaEmAberto(cargaEntrega.Codigo);

            if (listaChamadosDaNota != null && listaChamadosDaNota.Count > 0)
                listaChamadosDaNota = listaChamadosDaNota.DistinctBy(x => x.Codigo).ToList();

            if (listaChamadosDaNota != null && listaChamadosDaNota.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in listaChamadosDaNota)
                {
                    if (chamado != null && (chamado.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoChamado?.FinalizarAutomaticamenteAtendimentoNfeEntregue ?? false))
                    {
                        var analises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);

                        chamado.Situacao = SituacaoChamado.Finalizado;
                        chamado.DataFinalizacao = DateTime.Now;
                        chamado.ControleDuplicidade = chamado.Codigo;
                        repChamado.Atualizar(chamado, auditado);

                        if (analises != null && analises.Count > 0)
                        {
                            foreach (var analise in analises)
                            {
                                analise.Observacao += $"\nO atendimento foi encerrado automaticamente, pois a nota fiscal foi entregue. Origem: {origem}";
                                repChamadoAnalise.Atualizar(analise, auditado);
                            }
                        }

                        Servicos.Auditoria.Auditoria.Auditar(auditado, chamado, $"O atendimento foi encerrado automaticamente, pois a nota fiscal foi entregue. Origem: {origem}", unitOfWork);

                        Servicos.Embarcador.Hubs.Chamado hubChamado = new Servicos.Embarcador.Hubs.Chamado();
                        hubChamado.NotificarChamadoCancelado(chamado);

                    }
                }
            }
        }

        public static void ReverterQuantidadeDevolucao(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado repositorioCargaEntregaProdutoChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repositorioCargaEntregaProduto.BuscarPorCargaEntrega(chamado.CargaEntrega.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargaEntregaProdutoChamados = repositorioCargaEntregaProdutoChamado.BuscarPorChamado(chamado.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado cargaEntregaProdutoChamado in cargaEntregaProdutoChamados)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = cargaEntregaProdutos.Find(p => p.XMLNotaFiscal?.Codigo == cargaEntregaProdutoChamado.XMLNotaFiscal?.Codigo && p.Produto.Codigo == cargaEntregaProdutoChamado.Produto.Codigo);
                cargaEntregaProduto.QuantidadeDevolucao -= cargaEntregaProdutoChamado.QuantidadeDevolucao;
                repositorioCargaEntregaProduto.Atualizar(cargaEntregaProduto);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private List<Dominio.Entidades.Usuario> BuscarAnalistasChamado(List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaFiltrada, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Usuario> analistas = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regra in listaFiltrada)
            {
                foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                {
                    if (!analistas.Contains(aprovador))
                        analistas.Add(aprovador);
                }
            }

            try
            {
                Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

                foreach (Dominio.Entidades.Usuario analista in analistas)
                {
                    string nota = string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.UsuarioAbriuChamadoAguardaPosicionamento, usuario.Nome, chamado.Numero, chamado.MotivoChamado.Descricao);
                    servicoNotificacao.GerarNotificacao(analista, usuario, chamado.Codigo, "Chamados/ChamadoOcorrencia", nota, IconesNotificacao.agConfirmacao, TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro($"Falha ao gerar notificação para usuario: {ex.Message}");
            }

            return analistas;
        }

        private List<Dominio.Entidades.Usuario> BuscarResponsaveisChamado(List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaFiltrada)
        {
            List<Dominio.Entidades.Usuario> responsaveis = new List<Dominio.Entidades.Usuario>();

            foreach (Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regra in listaFiltrada)
            {
                foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                {
                    if (!responsaveis.Contains(aprovador))
                        responsaveis.Add(aprovador);
                }
            }

            return responsaveis;
        }

        private void EnviarEmailChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, string assunto, string mensagem, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Usuario> usuarios = null)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                if (!repositorioConfiguracao.ObterConfiguracaoPorNomePropriedade<bool>("EnviarEmailAnalistasChamado") && usuarios == null)
                    return;

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string de = configuracaoEmail.Email;
                string usuario = configuracaoEmail.Email;
                string senha = configuracaoEmail.Senha;
                string[] copiaOcultaPara = new string[] { };
                string[] copiaPara = new string[] { };
                string servidorSMTP = configuracaoEmail.Smtp;
                List<System.Net.Mail.Attachment> anexos = null;
                string assinatura = "";
                bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                string responderPara = "";
                int porta = configuracaoEmail.PortaSmtp;
                StringBuilder corpoMensagem = new StringBuilder();

                corpoMensagem.AppendLine(@"<div style=""font-family: Arial;"">");
                corpoMensagem.AppendLine($@"    <p style=""margin:0px"">{mensagem}</p>");
                corpoMensagem.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
                corpoMensagem.AppendLine("    <p></p>");
                corpoMensagem.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
                corpoMensagem.AppendLine("</div>");

                if (usuarios?.Count > 0)
                {
                    foreach (Dominio.Entidades.Usuario funcionario in usuarios)
                    {
                        if (string.IsNullOrWhiteSpace(funcionario.Email))
                            continue;

                        if (!Servicos.Email.EnviarEmail(de, usuario, senha, funcionario.Email, copiaOcultaPara, copiaPara, assunto, corpoMensagem.ToString(), servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, unitOfWork))
                            Log.TratarErro($"Falha ao enviar o e-mail do chamado: {erro}");
                    }
                }
                else
                {
                    foreach (Dominio.Entidades.Usuario analista in chamado.Analistas)
                    {
                        if (string.IsNullOrWhiteSpace(analista.Email))
                            continue;

                        if (!Servicos.Email.EnviarEmail(de, usuario, senha, analista.Email, copiaOcultaPara, copiaPara, assunto, corpoMensagem.ToString(), servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, unitOfWork))
                            Log.TratarErro($"Falha ao enviar o e-mail do chamado: {erro}");
                    }
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro("Falha ao enviar o e-mail do chamado: " + excecao);
            }
        }

        private void FinalizarChamadosSemRetorno(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);
            Servicos.Embarcador.Notificacao.Notificacao servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            DateTime horaBase = DateTime.Now;
            horaBase = horaBase.AddSeconds(-horaBase.Second);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarChamadosSemRetorno(horaBase);

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repChamadoOcorrencia.BuscarOcorrenciasPorChamado(chamado.Codigo);

                if (ocorrencias.All(obj => obj.SituacaoOcorrencia == SituacaoOcorrencia.Finalizada || obj.SituacaoOcorrencia == SituacaoOcorrencia.Cancelada))
                {
                    chamado.Situacao = SituacaoChamado.Finalizado;
                    chamado.DataFinalizacao = DateTime.Now;
                    repChamado.Atualizar(chamado);

                    servicoNotificacao.GerarNotificacao(chamado.Autor, chamado.Codigo, "Chamados/ChamadoOcorrencia", string.Format(Localization.Resources.Chamado.ChamadoOcorrencia.ChamadoEncerradoAutomaticamente, chamado.Descricao), IconesNotificacao.agConfirmacao, SmartAdminBgColor.yellow, TipoNotificacao.credito, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unitOfWork);
                    servicoChamado.EnviarEmailChamadoFinalizado(chamado, unitOfWork);
                }
            }
        }

        private async Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>> VerificarRegrasAutorizacaoOcorrenciaAsync(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Repositorio.Embarcador.Chamados.RegrasAnaliseChamados repositorioRegrasAnaliseChamados = new Repositorio.Embarcador.Chamados.RegrasAnaliseChamados(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaRegras = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>();
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaFiltrada = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>();

            //Regra pormotivo da avaria
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaRegrasMotivoChamado = await repositorioRegrasAnaliseChamados.BuscarRegraPorMotivoChamadaAsync(chamado.MotivoChamado.Codigo, chamado.DataCriacao);
            listaRegras.AddRange(listaRegrasMotivoChamado);

            //Regra por Filial
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaRegraFilial = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>();

            int codigoFilial = chamado.Carga?.Filial?.Codigo ?? 0;
            if (chamado.Carga?.FilialOrigem != null)
                codigoFilial = chamado.Carga.FilialOrigem.Codigo;

            if (codigoFilial > 0)
                listaRegraFilial = await repositorioRegrasAnaliseChamados.BuscarRegraPorFilialAsync(codigoFilial, chamado.DataCriacao);

            listaRegras.AddRange(listaRegraFilial);

            //Regra por Região Destino
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados> listaRegraRegiaoDestino = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados>();
            List<int> codigosRegiaoDestino = chamado.Carga != null ? await repositorioPedido.BuscarCodigosRegiaoDestinoPorCargaAsync(chamado.Carga.Codigo) : new List<int>();
            if (codigosRegiaoDestino.Count > 0)
            {
                listaRegraRegiaoDestino = await repositorioRegrasAnaliseChamados.BuscarRegraPorRegiaoDestinoAsync(codigosRegiaoDestino, chamado.DataCriacao);
                listaRegras.AddRange(listaRegraRegiaoDestino);
            }

            if (listaRegras.Distinct().Any())
            {
                listaFiltrada.AddRange(listaRegras.Distinct());

                foreach (Dominio.Entidades.Embarcador.Chamados.RegrasAnaliseChamados regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorMotivoChamado)
                    {
                        bool valido = false;
                        if (regra.RegrasMotivoChamado.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.MotivoChamado.Codigo == chamado.MotivoChamado.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoChamado.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.MotivoChamado.Codigo == chamado.MotivoChamado.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoChamado.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.MotivoChamado.Codigo != chamado.MotivoChamado.Codigo))
                            valido = true;
                        else if (regra.RegrasMotivoChamado.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.MotivoChamado.Codigo != chamado.MotivoChamado.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorFilial && codigoFilial > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Filial.Codigo != codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Filial.Codigo != codigoFilial))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorRegiaoDestino && codigosRegiaoDestino.Count > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasRegiaoDestino.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && codigosRegiaoDestino.Contains(o.Regiao.Codigo)))
                            valido = true;
                        else if (regra.RegrasRegiaoDestino.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && codigosRegiaoDestino.Contains(o.Regiao.Codigo)))
                            valido = true;
                        else if (regra.RegrasRegiaoDestino.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && !codigosRegiaoDestino.Contains(o.Regiao.Codigo)))
                            valido = true;
                        else if (regra.RegrasRegiaoDestino.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && !codigosRegiaoDestino.Contains(o.Regiao.Codigo)))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return listaFiltrada;
        }

        private async Task<List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>> VerificarRegrasAtendimentoChamadoAsync(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados repositorioRegrasAtendimentoChamados = new Repositorio.Embarcador.Chamados.RegrasAtendimentoChamados(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaRegras = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaFiltrada = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();

            if (chamado.Carga != null)
                pedido = await repositorioPedido.BuscarPrimeiroPorCargaAsync(chamado.Carga.Codigo);

            //Regra por Filial
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaRegraFilial = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            int codigoFilial = chamado.Carga?.Filial?.Codigo ?? 0;

            if (chamado.Carga?.Filial != null)
                codigoFilial = chamado.Carga.Filial.Codigo;

            if (codigoFilial > 0)
                listaRegraFilial = await repositorioRegrasAtendimentoChamados.BuscarRegraPorFilialAsync(codigoFilial, chamado.DataCriacao);

            listaRegras.AddRange(listaRegraFilial);

            //Regra por Tipo de Operação
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaRegraTipoOperacao = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            int codigoTipoOperacao = chamado.Carga?.TipoOperacao?.Codigo ?? 0;

            if (chamado.Carga?.TipoOperacao != null)
                codigoTipoOperacao = chamado.Carga.TipoOperacao.Codigo;

            if (codigoTipoOperacao > 0)
                listaRegraTipoOperacao = await repositorioRegrasAtendimentoChamados.BuscarRegraPorTipoOperacaoAsync(codigoTipoOperacao, chamado.DataCriacao);

            listaRegras.AddRange(listaRegraTipoOperacao);

            //Regra por Transportador
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaRegraTransportador = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            int codigoTransportador = chamado.Carga?.Empresa?.Codigo ?? 0;

            if (chamado.Carga?.Empresa != null)
                codigoTransportador = chamado.Carga.Empresa.Codigo;

            if (codigoTransportador > 0)
                listaRegraTransportador = await repositorioRegrasAtendimentoChamados.BuscarRegraPorTransportadorAsync(codigoTransportador, chamado.DataCriacao);

            listaRegras.AddRange(listaRegraTransportador);

            //Regra por Estado
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaRegraEstado = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            string siglaEstado = pedido?.Origem?.Estado?.Sigla ?? string.Empty;
            if (pedido?.Origem?.Estado != null)
                siglaEstado = pedido.Origem.Estado.Sigla;

            if (!string.IsNullOrEmpty(siglaEstado))
                listaRegraEstado = await repositorioRegrasAtendimentoChamados.BuscarRegraPorEstadoAsync(siglaEstado, chamado.DataCriacao);

            listaRegras.AddRange(listaRegraEstado);

            //Regra por Canal Venda
            List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados> listaRegraCanalVenda = new List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados>();
            int codigoCanalVenda = pedido?.CanalVenda?.Codigo ?? 0;
            if (pedido?.CanalVenda != null)
                codigoCanalVenda = pedido.CanalVenda.Codigo;

            if (codigoCanalVenda > 0)
                listaRegraCanalVenda = await repositorioRegrasAtendimentoChamados.BuscarRegraPorCanalVendaAsync(codigoCanalVenda, chamado.DataCriacao);

            listaRegras.AddRange(listaRegraCanalVenda);

            if (listaRegras.Distinct().Any())
            {
                listaFiltrada.AddRange(listaRegras.Distinct());

                foreach (Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoChamados regra in listaRegras.Distinct())
                {
                    if (regra.RegraPorCanalVenda && codigoCanalVenda > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasCanalVenda.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.CanalVenda.Codigo == codigoCanalVenda))
                            valido = true;
                        else if (regra.RegrasCanalVenda.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.CanalVenda.Codigo == codigoCanalVenda))
                            valido = true;
                        else if (regra.RegrasCanalVenda.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.CanalVenda.Codigo != codigoCanalVenda))
                            valido = true;
                        else if (regra.RegrasCanalVenda.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.CanalVenda.Codigo != codigoCanalVenda))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorEstado && !string.IsNullOrEmpty(siglaEstado))
                    {
                        bool valido = false;
                        if (regra.RegrasEstado.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Estado.Sigla == siglaEstado))
                            valido = true;
                        else if (regra.RegrasEstado.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Estado.Sigla == siglaEstado))
                            valido = true;
                        else if (regra.RegrasEstado.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Estado.Sigla != siglaEstado))
                            valido = true;
                        else if (regra.RegrasEstado.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Estado.Sigla != siglaEstado))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorFilial && codigoFilial > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Filial.Codigo == codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Filial.Codigo != codigoFilial))
                            valido = true;
                        else if (regra.RegrasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Filial.Codigo != codigoFilial))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorTipoOperacao && codigoTipoOperacao > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.TipoOperacao.Codigo == codigoTipoOperacao))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.TipoOperacao.Codigo == codigoTipoOperacao))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.TipoOperacao.Codigo != codigoTipoOperacao))
                            valido = true;
                        else if (regra.RegrasTipoOperacao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.TipoOperacao.Codigo != codigoTipoOperacao))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                    if (regra.RegraPorTransportador && codigoTransportador > 0)
                    {
                        bool valido = false;
                        if (regra.RegrasTransportador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Empresa.Codigo == codigoTransportador))
                            valido = true;
                        else if (regra.RegrasTransportador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Empresa.Codigo == codigoTransportador))
                            valido = true;
                        else if (regra.RegrasTransportador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.Ou && o.Empresa.Codigo != codigoTransportador))
                            valido = true;
                        else if (regra.RegrasTransportador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoAvaria.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoAvaria.E && o.Empresa.Codigo != codigoTransportador))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return listaFiltrada;
        }

        private dynamic ObterDiariaAutomaticaEntradasESaidas(Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica, Repositorio.UnitOfWork unitOfWork)
        {
            if (diariaAutomatica == null)
            {
                return new List<dynamic>();
            }

            var servicoDiariaAutomatica = new Servicos.Embarcador.Monitoramento.DiariaAutomatica(unitOfWork);
            var listaCargaEntrega = servicoDiariaAutomatica.ObterCargasEntregaPorLocalFreeTime(diariaAutomatica.Carga, diariaAutomatica.LocalFreeTime);

            return (from o in listaCargaEntrega
                    select new
                    {
                        Cliente = o.Cliente?.Nome ?? "",
                        Ordem = o.Ordem,
                        DataEntradaRaio = o.DataEntradaRaio?.ToString("dd/MM/yyyy HH:mm"),
                        DataSaidaRaio = o.DataSaidaRaio?.ToString("dd/MM/yyyy HH:mm"),
                    }).ToList();
        }

        private async Task SalvarNivelAtendimento(Dominio.Entidades.Embarcador.Chamados.Chamado chamado)
        {
            Repositorio.Embarcador.Chamados.NivelAtendimento repositorioNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(_unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos repositorioGatilhosMotivo = new Repositorio.Embarcador.Chamados.MotivoChamadoGatilhos(_unitOfWork);

            Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList gatilho = await repositorioGatilhosMotivo.BuscarNivelPorMotivoChamadoAsync(chamado.MotivoChamado.Codigo, EscalationList.Nivel1);

            if (gatilho == null)
                return;

            chamado.Nivel = EscalationList.Nivel1;
            Dominio.Entidades.Embarcador.Chamados.NivelAtendimento nivelAtendimento = new Dominio.Entidades.Embarcador.Chamados.NivelAtendimento()
            {
                Chamado = chamado,
                DataCriacao = DateTime.Now,
                FoiNotificado = false,
                Nivel = EscalationList.Nivel1
            };

            if (gatilho.MotivoChamado.ConsiderarHorasDiasUteis)
            {
                nivelAtendimento.DataLimite = AdicionarTempoDiaUtil(DateTime.Now, gatilho.Tempo);
            }
            else
            {
                nivelAtendimento.DataLimite = DateTime.Now.AddMinutes(gatilho.Tempo);
            }

            await repositorioNivelAtendimento.InserirAsync(nivelAtendimento);
        }

        private void IntegracaoIsis(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);

            if (chamado.CargaEntrega == null || chamado.CargaEntrega.MotivoRejeicao == null || chamado.TratativaDevolucao == SituacaoEntrega.Revertida)
                return;

            if (!repOcorrenciaTipoIntegracao.PossuiIntegracaoPorTipoOcorrenciaETipoIntegracao(chamado.CargaEntrega.MotivoRejeicao.Codigo, TipoIntegracao.Isis))
                return;

            AdicionarIntegracaoChamadoOcorrencia(chamado, tipoIntegracao, unitOfWork, auditado);
        }

        private void IntegracaoCassolEventosEntregaAtendimentos(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(unitOfWork);

            if (chamado.CargaEntrega == null)
                return;

            AdicionarIntegracaoChamadoOcorrencia(chamado, tipoIntegracao, unitOfWork, auditado);
        }

        private void IntegracaoMarilan(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (!chamado.InformarDadosChamadoFinalizadoComCusto)
                return;

            AdicionarIntegracaoChamadoOcorrencia(chamado, tipoIntegracao, unitOfWork, auditado);
        }

        private void IntegracaoJJ(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (chamado.CargaEntrega == null)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            if (chamado.CargaEntrega.DevolucaoParcial)
                cargaEntregaNotasFiscais.AddRange(repCargaEntregaNotaFiscal.BuscarNotasDevolucaoTotalOuParcialPorCargaEntrega(chamado.CargaEntrega.Codigo, chamado.Codigo));
            else
                cargaEntregaNotasFiscais.AddRange(repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(chamado.CargaEntrega.Codigo));

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
                AdicionarIntegracaoChamadoOcorrenciaComNF(chamado, tipoIntegracao, cargaEntregaNotaFiscal, unitOfWork, auditado);
        }

        private void AdicionarIntegracaoChamadoOcorrencia(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao = repChamadoIntegracao.BuscarPrimeiroPorChamado(chamado.Codigo);

            if (chamadoIntegracao != null)
                return;

            chamadoIntegracao = new Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao();
            chamadoIntegracao.TipoIntegracao = tipoIntegracao;
            chamadoIntegracao.DataIntegracao = DateTime.Now;
            chamadoIntegracao.ProblemaIntegracao = "";
            chamadoIntegracao.Chamado = chamado;
            chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            repChamadoIntegracao.Inserir(chamadoIntegracao);

            chamado.Situacao = SituacaoChamado.AgIntegracao;
            repChamado.Atualizar(chamado, auditado);
        }

        private void AdicionarIntegracaoChamadoOcorrenciaComNF(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Chamados.ChamadoIntegracao repChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao = repChamadoIntegracao.BuscarPrimeiroPorChamadoECargaEntregaNotaFiscal(chamado.Codigo, cargaEntregaNotaFiscal.Codigo);

            if (chamadoIntegracao != null)
                return;

            chamadoIntegracao = new Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao();
            chamadoIntegracao.TipoIntegracao = tipoIntegracao;
            chamadoIntegracao.DataIntegracao = DateTime.Now;
            chamadoIntegracao.ProblemaIntegracao = "";
            chamadoIntegracao.Chamado = chamado;
            chamadoIntegracao.CargaEntregaNotaFiscal = cargaEntregaNotaFiscal;
            chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            repChamadoIntegracao.Inserir(chamadoIntegracao);

            chamado.Situacao = SituacaoChamado.AgIntegracao;
            repChamado.Atualizar(chamado, auditado);
        }

        private void AlterarSituacaoAgendamentoENotaFiscalEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido pedido in cargaEntrega.Pedidos)
            {
                if (pedido.CargaPedido.Pedido != null)
                {
                    pedido.CargaPedido.Pedido.SituacaoAgendamentoEntregaPedido = SituacaoAgendamentoEntregaPedido.AguardandoReagendamento;

                    Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorioNotaFiscalSituacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao = repositorioNotaFiscalSituacao.BuscarPorGatilho(NotaFiscalSituacaoGatilho.RetificarEntrega);

                    if (notaFiscalSituacao != null)
                    {
                        foreach (var nota in pedido.CargaPedido.NotasFiscais)
                            nota.XMLNotaFiscal.NotaFiscalSituacao = notaFiscalSituacao;
                    }
                }
            }
        }

        private static bool VerificarTransportadorPodeAbrirChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Usuario usuario)
        {
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoChamado configuracaoTipoOperacaoChamado = chamado.Carga.TipoOperacao?.ConfiguracaoTipoOperacaoChamado ?? null;

            if (configuracaoTipoOperacaoChamado.MotivosChamados.Contains(chamado.MotivoChamado) && configuracaoTipoOperacaoChamado.Transportadores.Contains(usuario.Empresa))
                return true;

            return false;

        }

        public void EnviarEmailChamadoCanceladoParaTransportador(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (!chamado.MotivoChamado.EnviarEmailParaTransportadorAoCancelarChamado)
                    return;

                if (chamado.Carga?.Empresa == null || string.IsNullOrWhiteSpace(chamado.Carga.Empresa.Email))
                    return;

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string assunto = $"Atendimento {chamado.Numero} cancelado";
                System.Text.StringBuilder corpoEmail = new System.Text.StringBuilder();

                corpoEmail.Append("<span style=\"width: 100%; display: inline-block\">Olá, </span>");
                corpoEmail.Append($"<span style=\"width: 100%; display: inline-block\">Prezado(a) Sr(a) {chamado.Carga.Empresa.RazaoSocial}, </span>");
                corpoEmail.Append($"<span style=\"width: 100%; display: inline-block\">O atendimento {chamado.Numero} para a carga {chamado.Carga?.CodigoCargaEmbarcador ?? ""} foi cancelado.</span>");
                corpoEmail.Append("<div style=\"margin: 30px 0;\">");
                corpoEmail.Append("<table style=\"border: 1px solid #b9b5b5; border-collapse: collapse;\">");
                corpoEmail.Append("<thead style=\"background-color: #d9e1f2; color: black;\">");
                corpoEmail.Append("<tr>");
                corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Informação</th>");
                corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Detalhe</th>");
                corpoEmail.Append("</tr>");
                corpoEmail.Append("</thead>");
                corpoEmail.Append("<tbody>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número do Atendimento</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Numero}</td>");
                corpoEmail.Append("</tr>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Motivo do Atendimento</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.MotivoChamado?.Descricao ?? ""}</td>");
                corpoEmail.Append("</tr>");

                if (chamado.Carga != null)
                {
                    corpoEmail.Append("<tr>");
                    corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número da Carga</td>");
                    corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Carga.CodigoCargaEmbarcador}</td>");
                    corpoEmail.Append("</tr>");
                }

                if (!string.IsNullOrWhiteSpace(chamado.Observacao))
                {
                    corpoEmail.Append("<tr>");
                    corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Observação</td>");
                    corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Observacao}</td>");
                    corpoEmail.Append("</tr>");
                }

                corpoEmail.Append("</tbody>");
                corpoEmail.Append("</table>");
                corpoEmail.Append("</div>");

                corpoEmail.Append("<p style=\"font-size: 12px;\">");
                corpoEmail.Append($"Mensagem enviada em {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}<br/>");
                corpoEmail.Append("Este e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responda a esta mensagem.");
                corpoEmail.Append("</p>");

                string emailTransportador = chamado.Carga.Empresa.Email ?? string.Empty;

                Servicos.Email.EnviarEmail(
                    configuracaoEmail.Email,
                    configuracaoEmail.Email,
                    configuracaoEmail.Senha,
                    emailTransportador,
                    null,
                    null,
                    assunto,
                    corpoEmail.ToString(),
                    configuracaoEmail.Smtp,
                    out string mensagemErro,
                    configuracaoEmail.DisplayEmail,
                    null,
                    "",
                    configuracaoEmail.RequerAutenticacaoSmtp,
                    "",
                    configuracaoEmail.PortaSmtp,
                    unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    Log.TratarErro($"Falha ao enviar o e-mail de cancelamento do chamado para o transportador: {mensagemErro}");
            }
            catch (Exception excecao)
            {
                Log.TratarErro("Falha ao enviar o e-mail de cancelamento do chamado para o transportador: " + excecao);
                // Apenas logar o erro, não afetar o fluxo principal
            }
        }

        public void EnviarEmailTransportadorAlteracaoChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analise, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (!chamado.MotivoChamado.EnviarEmailParaTransportadorAoAlterarChamado)
                    return;

                if (chamado?.Empresa == null || string.IsNullOrWhiteSpace(chamado.Empresa.Email))
                    return;

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string assunto = $"Atendimento {chamado.Numero} Alterado";
                System.Text.StringBuilder corpoEmail = new System.Text.StringBuilder();

                corpoEmail.Append("<span style=\"width: 100%; display: inline-block\">Olá, </span>");
                corpoEmail.Append($"<span style=\"width: 100%; display: inline-block\">Prezado(a) Sr(a) {chamado.Empresa.RazaoSocial}, </span>");
                corpoEmail.Append($"<span style=\"width: 100%; display: inline-block\">O atendimento {chamado.Numero} para a carga {chamado.Carga?.CodigoCargaEmbarcador ?? ""} foi alterado.</span>");
                corpoEmail.Append("<div style=\"margin: 30px 0;\">");
                corpoEmail.Append("<table style=\"border: 1px solid #b9b5b5; border-collapse: collapse;\">");
                corpoEmail.Append("<thead style=\"background-color: #d9e1f2; color: black;\">");
                corpoEmail.Append("<tr>");
                corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Informação</th>");
                corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Detalhe</th>");
                corpoEmail.Append("</tr>");
                corpoEmail.Append("</thead>");
                corpoEmail.Append("<tbody>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número do Atendimento</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Numero}</td>");
                corpoEmail.Append("</tr>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Situação</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Situacao.ObterDescricao()}</td>");
                corpoEmail.Append("</tr>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Motivo do Atendimento</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.MotivoChamado?.Descricao ?? ""}</td>");
                corpoEmail.Append("</tr>");

                if (chamado.Carga != null)
                {
                    corpoEmail.Append("<tr>");
                    corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número da Carga</td>");
                    corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Carga.CodigoCargaEmbarcador}</td>");
                    corpoEmail.Append("</tr>");
                }

                if (analise != null && !string.IsNullOrWhiteSpace(analise.Observacao))
                {
                    corpoEmail.Append("<tr>");
                    corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Observação da Análise</td>");
                    corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{analise.Observacao}</td>");
                    corpoEmail.Append("</tr>");
                }

                corpoEmail.Append("</tbody>");
                corpoEmail.Append("</table>");
                corpoEmail.Append("</div>");

                corpoEmail.Append("<p style=\"font-size: 12px;\">");
                corpoEmail.Append($"Mensagem enviada em {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}<br/>");
                corpoEmail.Append("Este e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responda a esta mensagem.");
                corpoEmail.Append("</p>");

                string emailTransportador = chamado.Empresa.Email ?? string.Empty;

                Servicos.Email.EnviarEmail(
                    configuracaoEmail.Email,
                    configuracaoEmail.Email,
                    configuracaoEmail.Senha,
                    emailTransportador,
                    null,
                    null,
                    assunto,
                    corpoEmail.ToString(),
                    configuracaoEmail.Smtp,
                    out string mensagemErro,
                    configuracaoEmail.DisplayEmail,
                    null,
                    "",
                    configuracaoEmail.RequerAutenticacaoSmtp,
                    "",
                    configuracaoEmail.PortaSmtp,
                    unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    Log.TratarErro($"Falha ao enviar o e-mail de alteração do chamado para o transportador: {mensagemErro}");
            }
            catch (Exception excecao)
            {
                Log.TratarErro("Falha ao enviar o e-mail de alteração do chamado para o transportador: " + excecao);
                // Apenas logar o erro, não afetar o fluxo principal
            }
        }

        public void EnviarEmailParaTransportadorAoFinalizarChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (!chamado.MotivoChamado.EnviarEmailParaTransportadorAoFinalizarChamado)
                    return;

                if (chamado.Carga?.Empresa == null || string.IsNullOrWhiteSpace(chamado.Carga.Empresa.Email))
                    return;

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string assunto = $"Atendimento {chamado.Numero} finalizado";
                System.Text.StringBuilder corpoEmail = new System.Text.StringBuilder();

                corpoEmail.Append("<span style=\"width: 100%; display: inline-block\">Olá, </span>");
                corpoEmail.Append($"<span style=\"width: 100%; display: inline-block\">Prezado(a) Sr(a) {chamado.Carga.Empresa.RazaoSocial}, </span>");
                corpoEmail.Append($"<span style=\"width: 100%; display: inline-block\">O atendimento {chamado.Numero} para a carga {chamado.Carga?.CodigoCargaEmbarcador ?? ""} foi finalizado.</span>");
                corpoEmail.Append("<div style=\"margin: 30px 0;\">");
                corpoEmail.Append("<table style=\"border: 1px solid #b9b5b5; border-collapse: collapse;\">");
                corpoEmail.Append("<thead style=\"background-color: #d9e1f2; color: black;\">");
                corpoEmail.Append("<tr>");
                corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Informação</th>");
                corpoEmail.Append("<th style=\"border: 1px solid #b9b5b5; padding: 10px;\">Detalhe</th>");
                corpoEmail.Append("</tr>");
                corpoEmail.Append("</thead>");
                corpoEmail.Append("<tbody>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número do Atendimento</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Numero}</td>");
                corpoEmail.Append("</tr>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Motivo do Atendimento</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.MotivoChamado?.Descricao ?? ""}</td>");
                corpoEmail.Append("</tr>");

                corpoEmail.Append("<tr>");
                corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Número da Carga</td>");
                corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Carga?.CodigoCargaEmbarcador ?? ""}</td>");
                corpoEmail.Append("</tr>");

                if (!string.IsNullOrWhiteSpace(chamado.Observacao))
                {
                    corpoEmail.Append("<tr>");
                    corpoEmail.Append("<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">Observação</td>");
                    corpoEmail.Append($"<td style=\"border: 1px solid #b9b5b5; padding: 10px;\">{chamado.Observacao}</td>");
                    corpoEmail.Append("</tr>");
                }

                corpoEmail.Append("</tbody>");
                corpoEmail.Append("</table>");
                corpoEmail.Append("</div>");

                corpoEmail.Append("<p style=\"font-size: 12px;\">");
                corpoEmail.Append($"Mensagem enviada em {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}<br/>");
                corpoEmail.Append("Este e-mail foi enviado automaticamente. Por favor, não responda a esta mensagem.");
                corpoEmail.Append("</p>");

                string emailTransportador = chamado.Carga.Empresa.Email ?? string.Empty;

                Servicos.Email.EnviarEmail(
                    configuracaoEmail.Email,
                    configuracaoEmail.Email,
                    configuracaoEmail.Senha,
                    emailTransportador,
                    null,
                    null,
                    assunto,
                    corpoEmail.ToString(),
                    configuracaoEmail.Smtp,
                    out string mensagemErro,
                    configuracaoEmail.DisplayEmail,
                    null,
                    "",
                    configuracaoEmail.RequerAutenticacaoSmtp,
                    "",
                    configuracaoEmail.PortaSmtp,
                    unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                    Log.TratarErro($"Falha ao enviar o e-mail de finalização do chamado para o transportador: {mensagemErro}");
            }
            catch (Exception excecao)
            {
                Log.TratarErro("Falha ao enviar o e-mail de finalização do chamado para o transportador: " + excecao);
                // Apenas logar o erro, não afetar o fluxo principal
            }
        }

        private async Task ValidarCriticidadeDoAtendimento(Dominio.Entidades.Embarcador.Chamados.Chamado chamadoAtual)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamados = (await repositorioChamado.BuscarListaPorCargaAsync(chamadoAtual.Carga.Codigo)).Where(x => x.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Cancelada).OrderByDescending(x => x.Codigo).ToList();

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado item in listaChamados)
            {
                Dominio.Entidades.Embarcador.Chamados.Chamado chamadoAnterior = item;

                if (chamadoAnterior.MotivoChamado.NumeroCriticidadeAtendimento > chamadoAtual.MotivoChamado.NumeroCriticidadeAtendimento)
                    throw new ServicoException($"Já existe um atendimento com o motivo {item.MotivoChamado.Descricao} para esta carga. Não será possível abrir um atendimento com criticidade menor.");
            }
        }

        private async Task CancelarAtendimentoAnteriorDeAcordoComACriticidadeAsync(Dominio.Entidades.Embarcador.Chamados.Chamado chamadoAtual)
        {
            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repositorioChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Chamados.Chamado> listaChamados = await repositorioChamado.BuscarListaPorCargaECriticidadeAsync(chamadoAtual.Carga.Codigo, chamadoAtual.Codigo, chamadoAtual.MotivoChamado.NumeroCriticidadeAtendimento);

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado item in listaChamados)
            {
                //Cancela o chamado Anterior
                item.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Cancelada;
                item.Observacao = $"Atendimento {chamadoAtual.Numero} aberto com criticidade maior para esta carga.";

                await repositorioChamado.AtualizarAsync(item);

                //Vincula as ocorrencias do chamado anteriror no novo chamado
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia> listaChamadoOcorrencia = await repositorioChamadoOcorrencia.BuscarPorChamadoAsync(item.Codigo);

                foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia itemChamadoOcorrencia in listaChamadoOcorrencia)
                {
                    Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrencia = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
                    {
                        CargaOcorrencia = itemChamadoOcorrencia.CargaOcorrencia,
                        Chamado = chamadoAtual
                    };

                    await repositorioChamadoOcorrencia.InserirAsync(chamadoOcorrencia);
                }
            }
        }

        private async Task AcoesPosCriacaoChamado(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, string stringConexao, Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado)
        {
            if (cargaEntrega != null && !string.IsNullOrEmpty(cargaEntrega.Carga.IDIdentificacaoTrizzy) && !string.IsNullOrEmpty(cargaEntrega.IdTrizy) && motivoChamado.BloquearParadaAppTrizy)
            {
                Task t = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova);
                        Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AlternarBloqueioParada(true, cargaEntrega.Carga.IDIdentificacaoTrizzy, cargaEntrega.IdTrizy, unitOfWork);
                        unitOfWork.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                    }
                });
            }
        }

        private void AtualizarDataPrevisaoEntrega(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> listaCargaEntregaPedido, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = chamado.CargaEntrega;

            if (cargaEntrega.Situacao != SituacaoEntrega.Rejeitado && cargaEntrega.Situacao != SituacaoEntrega.Reentergue && cargaEntrega.Situacao != SituacaoEntrega.NaoEntregue)
                return;

            if (!chamado.DataPrevisaoEntregaPedidos.HasValue)
                return;

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in listaCargaEntregaPedido)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedido.CargaPedido.Pedido;
                pedido.Initialize();
                pedido.PrevisaoEntrega = chamado.DataPrevisaoEntregaPedidos;
                repositorioPedido.Atualizar(pedido, auditado);
            }
        }

        #endregion Métodos Privados
    }
}
