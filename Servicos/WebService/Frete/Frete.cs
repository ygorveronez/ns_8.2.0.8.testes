using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Servicos.Embarcador.Integracao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.WebService.Frete
{
    public class Frete
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        private readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        private readonly string _adminStringConexao;

        #endregion Atributos

        #region Constructores

        public Frete(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, string stringConexaoAdmin)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteAcesso = clienteURLAcesso;
            _clienteMultisoftware = cliente;
            _adminStringConexao = stringConexaoAdmin;
        }

        #endregion Constructores

        #region Métodos Privados

        private int SalvarDadosRetornoAprovacaoTabela(Dominio.Entidades.WebService.Integradora integradora, string request, string response)
        {
            Repositorio.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono repProcessamentoAprovacaoTabelaAssincrono = new Repositorio.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono processamentoAprovacaoTabelaAssincrono = new Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono
            {
                Integradora = integradora,
                DataRecebimento = DateTime.Now,
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "json", _unitOfWork),
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoProcessamentoAprovacaoTabelaAssincrono.Pendente
            };

            repProcessamentoAprovacaoTabelaAssincrono.Inserir(processamentoAprovacaoTabelaAssincrono);
            return processamentoAprovacaoTabelaAssincrono.Codigo;
        }
        #endregion Métodos Privados

        #region Metodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoContratoTransportador(List<Dominio.ObjetosDeValor.WebService.Frete.RetornoContratoTransportador> dadosRequest)
        {
            if (dadosRequest.Count == 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Precisa enviar dados para ser processados");

            Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao repositorioContratoFreteTransportadorIntegracao = new Repositorio.Embarcador.Frete.ContratoTransporteFreteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Frete.StatusAssinaturaContrato repositorioStatus = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(_unitOfWork);
            Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            foreach (Dominio.ObjetosDeValor.WebService.Frete.RetornoContratoTransportador contrato in dadosRequest)
            {
                int.TryParse(contrato.IDContratoMulti, out int codigoContrato);

                Dominio.Entidades.Embarcador.Frete.ContratoTransporteFreteIntegracao existeContratoFreteTransportadorIntegracao = repositorioContratoFreteTransportadorIntegracao.BuscarPorContrato(codigoContrato);

                if (existeContratoFreteTransportadorIntegracao == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Contrato com o codigo {contrato.IDContratoMulti} não encontrado");

                Dominio.Entidades.Embarcador.Frete.StatusAssinaturaContrato existeStatus = repositorioStatus.BuscarPorCodigoIntegracao(contrato.Status);

                if (existeStatus == null)
                    return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao($"Status {contrato.Status} não cadastrado na multisoftware");

                int.TryParse(contrato.IDContratoJaggaer, out int codigoJaggaer);
                existeContratoFreteTransportadorIntegracao.ContratoTransporteFrete.ContratoExternoID = codigoJaggaer;
                existeContratoFreteTransportadorIntegracao.ContratoTransporteFrete.StatusAssinaturaContrato = existeStatus;
                existeContratoFreteTransportadorIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                existeContratoFreteTransportadorIntegracao.DataIntegracao = DateTime.Now;

                servicoArquivoTransacao.Adicionar(existeContratoFreteTransportadorIntegracao, Newtonsoft.Json.JsonConvert.SerializeObject(contrato), Newtonsoft.Json.JsonConvert.SerializeObject(Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true)), "json");

            }

            return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> RetornoAprovacaoTabela(List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornosTabelaFrete, Dominio.Entidades.WebService.Integradora integradora)
        {
            Dominio.ObjetosDeValor.WebService.Retorno<bool> retorno = Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            string guid = Guid.NewGuid().ToString();
            Servicos.Log.TratarErro($" {guid} - {(retornosTabelaFrete != null ? Newtonsoft.Json.JsonConvert.SerializeObject(retornosTabelaFrete) : string.Empty)}", "RetornoAprovacaoTabela");

            if (retornosTabelaFrete.Count == 0)
            {
                Servicos.Log.TratarErro($" {guid} - lista vazia", "RetornoAprovacaoTabela");
                retorno = Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Precisa enviar dados para ser processados");
            }

            //Nenhuma validaçao agora, retorna sucesso, salva dados em uma tabela com arquivo para processar depois em paralelo
            int codigo = SalvarDadosRetornoAprovacaoTabela(integradora, Newtonsoft.Json.JsonConvert.SerializeObject(retornosTabelaFrete), Newtonsoft.Json.JsonConvert.SerializeObject(retorno));

            Servicos.Log.TratarErro($" {guid} - incluído para processamento assíncrono: {codigo}", "RetornoAprovacaoTabela");
            return retorno;
        }

        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<bool>> RetornoValorFreteOperador(int ProtocoloIntegracaoCarga, decimal ValorFreteOperador)
        {
            if (ProtocoloIntegracaoCarga >= 0)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Código de carga inválido.");

            try
            {
                Dominio.ObjetosDeValor.WebService.Rest.Frete.DadosInformarValorFreteOperador dadosInformarValorFreteOperador = new Dominio.ObjetosDeValor.WebService.Rest.Frete.DadosInformarValorFreteOperador
                {
                    CodigoCarga = ProtocoloIntegracaoCarga,
                    ValorFrete = ValorFreteOperador
                };

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoComFetchAsync(dadosInformarValorFreteOperador.CodigoCarga, new List<string> { "TabelaFrete" });

                dadosInformarValorFreteOperador.AvancarCarga = carga.ObrigatorioInformarValorFreteOperador;

                await new Servicos.Embarcador.Frete.TabelaFreteCliente(_unitOfWork).InformarValorManualAsync(dadosInformarValorFreteOperador, carga, null, default, _tipoServicoMultisoftware, configuracaoTMS, null, _auditado);

                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);

            }
            catch (BaseException ex)
            {
                await _unitOfWork.RollbackAsync();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao alterar valores manualmente");
            }
        }

        #endregion Metodos Públicos
    }
}