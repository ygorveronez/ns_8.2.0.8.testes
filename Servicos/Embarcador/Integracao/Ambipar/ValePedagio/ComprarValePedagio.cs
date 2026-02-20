using DocumentFormat.OpenXml.Drawing.Diagrams;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem;
using Dominio.ObjetosDeValor.Embarcador.CIOT.Repom;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Ambipar.ValePedagio;
using Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.Integracao.Ambipar
{
    public partial class ValePedagio
    {
        public void ComprarValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            _integracaoAmbipar = servicoValePedagio.ObterIntegracaoAmbipar(carga, tipoServicoMultisoftware);
            if (!ValidarConfiguracaoIntegracao(cargaValePedagio))
                return;

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPrimeiroPorCarga(carga.Codigo);

            string mensagemErro = string.Empty;
            this.ObterToken(out mensagemErro);
            if (string.IsNullOrWhiteSpace(token))
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = mensagemErro;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }

            CIOT.Ambipar ambipar = new CIOT.Ambipar(_unitOfWork, this.urlWebService, this.token);

            List<Dominio.Entidades.Veiculo> Carretas = cargaValePedagio.Carga.VeiculosVinculados.ToList();
            Dominio.Entidades.Usuario motorista = cargaValePedagio.Carga.Motoristas.FirstOrDefault();
            Dominio.Entidades.Cliente transportador = cargaValePedagio.Carga.Veiculo.Proprietario;
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(transportador, _unitOfWork);

            int? idMotorista = null;
            int? idTransportador = null;
            int? idVeiculo = null;
            List<int> idsCarreta = null;
            int? roteiroID = null;
            int? idCartao = null;

            if (cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Cartao && !string.IsNullOrEmpty(motorista.NumeroCartaoValePedagio))
            {
                if (!ambipar.BuscarCartao(motorista.NumeroCartaoValePedagio, _unitOfWork, out idCartao, out mensagemErro))
                {
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.ProblemaIntegracao = mensagemErro;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }
            }

            if (!ambipar.IntegrarTransportador(cargaValePedagio.Carga.Veiculo.Proprietario, modalidadeTerceiro, _unitOfWork, out mensagemErro, out idTransportador) ||
                !ambipar.IntegrarMotorista(transportador, motorista, transportador, idTransportador, _unitOfWork, out mensagemErro, out idMotorista) ||
                !ambipar.IntegrarVeiculo(cargaValePedagio.Carga.Veiculo, false, transportador, idTransportador, _unitOfWork, out mensagemErro, out idVeiculo) ||
                !ambipar.IntegrarCarretas(Carretas, transportador, idTransportador, _unitOfWork, out mensagemErro, out idsCarreta) ||
                !ambipar.IntegrarRota(carga, carga.Rota, _unitOfWork, out mensagemErro, out roteiroID, true))
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = mensagemErro;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }

            //Compra o vale pedágio
            EmitirViagemValePedagio(cargaValePedagio, pedido, idTransportador, idMotorista, idVeiculo, idsCarreta, roteiroID, idCartao);
        }

        #region Métodos Privados

        private void EmitirViagemValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, int? idTransportador, int? idMotorista, int? idVeiculo, List<int> idsCarreta, int? roteiroID, int? idCartao)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Ambipar.ValePedagio.envViagem comprarValePedagio = this.ObterObjViagem(cargaValePedagio.Carga, pedido, cargaValePedagio, idTransportador, idMotorista, idVeiculo, idsCarreta, roteiroID, idCartao);

                string url = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/Viagem";
                HttpClient requisicao = CriarRequisicao(url);
                jsonRequisicao = JsonConvert.SerializeObject(comprarValePedagio, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    string mensagemRetorno = string.Empty;
                    if (string.IsNullOrEmpty(jsonRetorno))
                        mensagemRetorno = $"Falha ao conectar no WS Ambipar: {retornoRequisicao.StatusCode}";
                    else
                        mensagemRetorno = $"Falha ao conectar no WS Ambipar: {jsonRetorno}";

                    throw new ServicoException(mensagemRetorno);
                }

                var retorno = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                cargaValePedagio.NumeroValePedagio = "";
                cargaValePedagio.IdCompraValePedagio = retorno.id.ToString();
                cargaValePedagio.NumeroValePedagio = retorno.id.ToString();
                cargaValePedagio.ValorValePedagio = retorno.valor ?? 0m;
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a compra do vale pedágio";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Ambipar.ValePedagio.envViagem ObterObjViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, int? idTransportador, int? idMotorista, int? idVeiculo, List<int> idsCarreta, int? roteiroID, int? idCartao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Ambipar.ValePedagio.envViagem retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Ambipar.ValePedagio.envViagem();

            int idEmpresa = 0;
            if (int.TryParse(carga.Empresa.CodigoIntegracao, out idEmpresa))
                retorno.embarcadorFilialID = idEmpresa;

            retorno.cartaoID = idCartao;
            retorno.motoristaID = idMotorista;
            retorno.transportadorID = idTransportador;
            retorno.veiculoID = idVeiculo;
            retorno.roteiroID = idTransportador;

            if (cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Cartao)
                retorno.midiaCargaTipoID = enumMidiaCargaTipoID.Vale_Pedagio_Ambipar_Visa_Cargo;
            else
                retorno.midiaCargaTipoID = enumMidiaCargaTipoID.Pedagio_Eletronico_Tag_Eletrônica;

            retorno.quantidadeEixosVeiculo = this.ObterNumeroEixos(carga, carga.Veiculo);
            retorno.quantidadeEixosCarreta = this.ObterNumeroEixosCarreta(carga);
            retorno.eixoSuspensoIda = this.ObterNumeroEixosSuspensos(carga);
            retorno.eixoSuspensoVolta = 0;
            retorno.midiaIdentificador = $"PedagioAvulso{carga.Codigo}";
            retorno.ignorarPreConfiguracao = false;
            retorno.tagId = null;

            if (cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag)
            {
                retorno.tag = !string.IsNullOrEmpty(cargaValePedagio.Carga.Veiculo.TagSemParar) ? cargaValePedagio.Carga.Veiculo.TagSemParar : null;

                if (cargaValePedagio.Carga.Veiculo.ModoCompraValePedagioTarget != null && cargaValePedagio.Carga.Veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagAmbipar)
                    retorno.tipoTag = enumTipoTag.Tag_Ambipar;
                else
                    retorno.tipoTag = enumTipoTag.Tag_SemParar;
            }

            DateTime dataTermino = carga.DataPrevisaoTerminoCarga.HasValue ? carga.DataPrevisaoTerminoCarga.Value : DateTime.Now.AddDays(1);
            retorno.dataTermino = dataTermino.ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T");

            if (idsCarreta != null)
            {
                foreach (int idCarreta in idsCarreta)
                {
                    if (retorno.carretaID == null)
                    {
                        retorno.carretaID = idCarreta;
                    }
                    else
                    {
                        if (retorno.carretaAdicional == null)
                            retorno.carretaAdicional = new List<carretaAdicional>();

                        carretaAdicional carretaAdicional = new carretaAdicional();
                        carretaAdicional.carretaID = idCarreta;
                        retorno.carretaAdicional.Add(carretaAdicional);
                    }
                }
            }

            return retorno;
        }

        private int ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Veiculo veiculo)
        {
            int numeroEixos = 0;
            if (carga.Veiculo.ModeloVeicularCarga != null)
                numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;

            return numeroEixos;
        }

        private int ObterNumeroEixosCarreta(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int numeroEixos = 0;
            if (carga.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                {
                    if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;
                }
            }

            return numeroEixos;
        }

        private int ObterNumeroEixosSuspensos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            bool eixosSuspensos = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio;

            int numeroEixos = 0;
            if (carga.Veiculo.ModeloVeicularCarga != null && eixosSuspensos)
                numeroEixos -= carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;

            if (carga.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                {
                    if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                    {
                        if (eixosSuspensos)
                            numeroEixos += reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                    }
                }
            }

            return numeroEixos;
        }

        #endregion
    }
}