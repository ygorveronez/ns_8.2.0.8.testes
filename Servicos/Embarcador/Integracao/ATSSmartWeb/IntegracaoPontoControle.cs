using DFe.Utils;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.ATSSmartWeb
{
    public partial class IntegracaoATSSmartWeb
    {
        #region Metodos Publicos

        public bool IntegrarPontoControle(ref Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            bool sucesso = false;

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle> pontosControle = this.ObterDadosPontosControle(cargaIntegracao.Carga);

                foreach (var pontoControle in pontosControle)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroPontoControleIntegracao/Integrar", pontoControle);

                    servicoArquivoTransacao.Adicionar(cargaIntegracao, retWS.jsonRequisicao, retWS.jsonRetorno, "json", "Integração de pontos de controle");

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        sucesso = true;
                    else
                        throw new ServicoException(retWS.ProblemaIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.ProblemaIntegracao = "Erro ao tentar integrar pontos de controle com a ATS Smart Web";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json", "Integração de pontos de controle");

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }

        public bool IntegrarPontoControle(ref Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, bool controleColeta = false)
        {

            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            bool sucesso = false;

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle> pontosControle = this.ObterDadosPontosControle(cargaDadosTransporteIntegracao.Carga, controleColeta);

                foreach (var pontoControle in pontosControle)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService retWS = transmitir("CadastroPontoControleIntegracao/Integrar", pontoControle);

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, retWS.jsonRequisicao, retWS.jsonRetorno, "json", "Integração de pontos de controle");

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                        sucesso = true;
                    else
                        throw new ServicoException(retWS.ProblemaIntegracao);
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaDadosTransporteIntegracao.ProblemaIntegracao = message;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Erro ao tentar integrar pontos de controle com a ATS Smart Web";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

            return sucesso;
        }
        #endregion

        #region Métodos Privados


        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPessoa obterPessoa(Dominio.Entidades.Cliente pessoa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPessoa retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPessoa();

            if(pessoa == null)
                throw new ServicoException(@"Pessoa não definida para integrar ponto de controle");

            retorno.Nome = pessoa.Nome;
            retorno.CPF_CNPJ = pessoa.CPF_CNPJ_SemFormato.ToString();
            retorno.CodigoExterno = pessoa.Codigo.ToString();
            retorno.Condutor = false;
            retorno.Cidade = pessoa.Localidade?.Descricao ?? "";
            retorno.UF = obterCodigoDeUF(pessoa.Localidade?.Estado?.Sigla ?? "");
            retorno.Endereco = this.obterEndereco(pessoa);
            retorno.Complemento = this.obterComplemento(pessoa);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle> ObterDadosPontosControle(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool controleColeta = false)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioTMS.BuscarPrimeiroRegistro();
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle> dadosPontosControle = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);

            if (controleColeta)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();

                if (cargaPedido != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle();

                    Dominio.Entidades.Cliente cliente = cargaPedido.Pedido?.Expedidor ?? cargaPedido.Pedido?.Remetente;

                    ponto.Pessoa = this.obterPessoa(cliente);
                    ponto.Nome = ponto.Pessoa.Nome;
                    ponto.Latitude = cliente.Localidade?.Latitude ?? 0;
                    ponto.Longitude = cliente.Localidade?.Longitude ?? 0;

                    dadosPontosControle.Add(ponto);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido pedido in carga.Pedidos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle();

                    Dominio.Entidades.Cliente cliente = pedido.Pedido?.Recebedor ?? pedido.Pedido?.Destinatario;

                    ponto.Pessoa = this.obterPessoa(cliente);
                    ponto.Nome = ponto.Pessoa.Nome;
                    ponto.Latitude = pedido.Destino?.Latitude ?? cliente.Localidade?.Latitude ?? 0;
                    ponto.Longitude = pedido.Destino?.Longitude ?? cliente.Localidade?.Longitude ?? 0;

                    dadosPontosControle.Add(ponto);
                }
            }
            else
            {
                if (cargaEntregas.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();

                    if (!cargaEntregas.Exists(ce => ce.Coleta))
                    {
                        if (cargaPedido == null)
                            throw new ServicoException("Carga sem Coleta e sem Pedido para integrar ponto de controle.");

                        Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle();

                        Dominio.Entidades.Cliente cliente = cargaPedido.Pedido?.Expedidor ?? cargaPedido.Pedido?.Remetente;

                        ponto.Pessoa = this.obterPessoa(cliente);
                        ponto.Nome = ponto.Pessoa.Nome;
                        ponto.Latitude = cliente.Localidade?.Latitude ?? 0;
                        ponto.Longitude = cliente.Localidade?.Longitude ?? 0;

                        dadosPontosControle.Add(ponto);
                    }


                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.envPontoControle();

                        Dominio.Entidades.Cliente cliente = cargaEntrega.Cliente;

                        ponto.Pessoa = this.obterPessoa(cliente);
                        ponto.Nome = ponto.Pessoa.Nome;
                        ponto.Latitude = cargaEntrega.Localidade?.Latitude ?? cliente.Localidade?.Latitude ?? 0;
                        ponto.Longitude = cargaEntrega.Localidade?.Longitude ?? cliente.Localidade?.Longitude ?? 0;

                        dadosPontosControle.Add(ponto);
                    }
                }
                else
                    throw new ServicoException(@"Carga não possui registros de entregas");
            }

            return dadosPontosControle;
        }

        #endregion
    }
}
