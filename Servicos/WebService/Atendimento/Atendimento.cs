using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.WebService.Atendimento
{
    public class Atendimento : ServicoWebServiceBase
    {
        #region Propriedades Privadas

        readonly private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        readonly protected string _adminStringConexao;

        #endregion

        #region Construtores

        public Atendimento(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao) : base(unitOfWork)
        {
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _auditado = auditado;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public async Task<Retorno<int>> AdicionarAtendimento(Dominio.ObjetosDeValor.WebService.Atendimento.AdicionarAtendimento adicionarAtendimento, CancellationToken cancellationToken)
        {
            Servicos.Log.TratarErro($"AdicionarAtendimento: {Newtonsoft.Json.JsonConvert.SerializeObject(adicionarAtendimento)}");

            try
            {
                if ((adicionarAtendimento.Carga == null && adicionarAtendimento.NotaFiscal == null) || adicionarAtendimento.Motivo == null)
                    throw new ServicoException("É obrigatório enviar os objetos da requisição.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Chamados.MotivoChamado repositorioMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(_unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork, cancellationToken);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = !string.IsNullOrWhiteSpace(adicionarAtendimento.Motivo.CodigoIntegracao) ? await repositorioMotivoChamado.BuscarPorCodigoIntegracaoAsync(adicionarAtendimento.Motivo.CodigoIntegracao) : await repositorioMotivoChamado.BuscarPorDescricaoAsync(adicionarAtendimento.Motivo.Descricao);
                if (motivoChamado == null)
                    throw new ServicoException("Motivo não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXmlNotaFiscal = null;

                if (adicionarAtendimento.NotaFiscal != null && adicionarAtendimento.Carga == null)
                {
                    if (!string.IsNullOrWhiteSpace(adicionarAtendimento.NotaFiscal.Chave))
                        pedidoXmlNotaFiscal = await repositorioPedidoXMLNotaFiscal.BuscarUltimaPorChaveNFeAsync(adicionarAtendimento.NotaFiscal.Chave);

                    if (pedidoXmlNotaFiscal == null && !string.IsNullOrWhiteSpace(adicionarAtendimento.NotaFiscal.Numero) && !string.IsNullOrWhiteSpace(adicionarAtendimento.NotaFiscal.Serie) && !string.IsNullOrWhiteSpace(adicionarAtendimento.NotaFiscal.CnpjEmitente))
                        pedidoXmlNotaFiscal = await repositorioPedidoXMLNotaFiscal.BuscarUltimaPorNumeroSerieNFeTransportadorAsync(adicionarAtendimento.NotaFiscal.Numero.ToInt(), adicionarAtendimento.NotaFiscal.Serie, adicionarAtendimento.NotaFiscal.CnpjEmitente.ToDouble());

                    carga = pedidoXmlNotaFiscal?.CargaPedido?.Carga;
                }

                if (carga == null && adicionarAtendimento.Carga != null)
                {
                    if (adicionarAtendimento.Carga.Protocolo > 0)
                        carga = await repositorioCarga.BuscarPorProtocoloAsync(adicionarAtendimento.Carga.Protocolo);

                    if (carga == null && !string.IsNullOrWhiteSpace(adicionarAtendimento.Carga.Numero))
                        carga = await repositorioCarga.BuscarPorCodigoEmbarcadorAsync(adicionarAtendimento.Carga.Numero);
                }

                if (carga == null)
                {
                    string mensagemErro = adicionarAtendimento.NotaFiscal != null ? $"para a nota fiscal ({adicionarAtendimento.NotaFiscal.Chave})" : "";

                    throw new ServicoException($"Carga não encontrada {mensagemErro}");
                }

                Dominio.Entidades.Usuario usuario = await repositorioCargaMotorista.BuscarPrimeiroMotoristaPorCargaAsync(carga.Codigo);
                if (usuario == null)
                    throw new ServicoException("Carga não possui motorista! Necessário informar antes de prosseguir.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracao.BuscarConfiguracaoPadraoAsync();

                Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado()
                {
                    Observacao = adicionarAtendimento.Observacao ?? string.Empty,
                    MotivoChamado = motivoChamado,
                    Carga = carga,
                    Empresa = carga.Empresa,
                    Cliente = await ObterClienteChamadoAsync(carga, pedidoXmlNotaFiscal?.CargaPedido?.Pedido, configuracao),
                    TipoCliente = configuracao.ChamadoOcorrenciaUsaRemetente ? Dominio.Enumeradores.TipoTomador.Remetente : Dominio.Enumeradores.TipoTomador.Destinatario,
                    Motorista = adicionarAtendimento.Carga?.CpfMotorista != null ? repUsuario.BuscarPorCPF(carga.Empresa.Codigo, adicionarAtendimento.Carga.CpfMotorista, "M") : null,
                    NotaFiscal = pedidoXmlNotaFiscal?.XMLNotaFiscal,
                };

                await _unitOfWork.StartAsync();

                Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(_unitOfWork, _tipoServicoMultisoftware, _auditado);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = await servicoChamado.AbrirChamadoAsync(objetoChamado, usuario);

                await _unitOfWork.CommitChangesAsync();

                return Retorno<int>.CriarRetornoSucesso(chamado.Codigo);
            }
            catch (ServicoException excecao)
            {
                await _unitOfWork.RollbackAsync();
                return Retorno<int>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                await _unitOfWork.RollbackAsync();

                return Retorno<int>.CriarRetornoExcecao("Ocorreu uma falha ao Adicionar Atendimento.");
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private async Task<Dominio.Entidades.Cliente> ObterClienteChamadoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (pedido != null)
                return configuracao.ChamadoOcorrenciaUsaRemetente ? pedido.Remetente : pedido.Destinatario;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            return await repositorioCargaPedido.BuscarPrimeiroClientePorCargaAsync(carga.Codigo, !configuracao.ChamadoOcorrenciaUsaRemetente);
        }

        #endregion Métodos Privados
    }
}
