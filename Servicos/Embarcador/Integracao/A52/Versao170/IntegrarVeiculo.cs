using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Servicos.Embarcador.Monitoramento.Eventos;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarVeiculos(List<Dominio.Entidades.Veiculo> veiculos, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out List<int> idVeiculos, out string mensagemErro, List<LogIntegracao> logs = null)
        {
            mensagemErro = null;
            idVeiculos = new List<int>();

            foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
            {
                int idVeiculo = 0;
                if (!IntegrarVeiculo(veiculo, cargaIntegracao, out idVeiculo, out mensagemErro, logs))
                    return false;
                else
                    idVeiculos.Add(idVeiculo);
            }

            return true;
        }

        private bool IntegrarVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out int idVeiculo, out string mensagemErro, List<LogIntegracao> logs = null)
        {
            mensagemErro = null;
            idVeiculo = 0;
            string mensagemLog = string.Empty;
            try
            {
                bool sucesso = false;

                object envioWS = ObterVeiculo(veiculo, cargaIntegracao);

                //Transmite o arquivo
                retornoWebService retornoWS = retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "veiculos", this.tokenAutenticacao);

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de cadastro de veículo A52: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do veículo; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        string mensagem = null;
                        retVeiculo retVeiculo = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                            else if(count == 2 && retorno.statusCode == "409")
                                retVeiculo = message.ToString().FromJson<retVeiculo>();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                        {
                            mensagemErro = "Ocorreu uma falha ao efetuar o cadastrodo veículo.";
                            sucesso = false;
                        }
                        else if (retVeiculo == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do veículo; RetornoWS {0}.", mensagem);
                            sucesso = false;
                        }
                        else
                        {
                            idVeiculo = (int)retVeiculo.id;
                            mensagemLog = "IntegrarVeiculo: Veículo Integrado com Sucesso";
                            sucesso = true;
                        }
                    }
                    mensagemLog = mensagemErro ?? mensagemLog;
                }
                else if (retornoWS.erro)
                {
                    mensagemErro = "Ocorreu uma falha ao efetuar o cadastrodo veículo.";
                    mensagemLog = mensagemErro;
                    sucesso = false;
                }
                else
                {
                    retVeiculo retVeiculo = retornoWS.jsonRetorno.ToString().FromJson<retVeiculo>();
                    idVeiculo = (int)retVeiculo.id;
                    mensagemLog = "IntegrarVeiculo: Veículo Integrado com Sucesso";
                    sucesso = true;
                }

                logs?.Add(new LogIntegracao
                {
                    NomeEtapa = $"IntegrarVeiculo - {veiculo.Placa ?? veiculo.Codigo.ToString()}",
                    JsonEnvio = retornoWS.jsonEnvio,
                    JsonRetorno = retornoWS.jsonRetorno
                });

                SalvarArquivosIntegracao(cargaIntegracao, retornoWS.jsonEnvio, retornoWS.jsonRetorno, mensagemLog);

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do veículo.";
                return false;
            }
        }

        private envVeiculo ObterVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            envVeiculo retorno = new envVeiculo();

            retorno.placa = veiculo.Placa_Formatada;
            retorno.frota = veiculo.NumeroFrota;

            int nTipoVeiculoA52;
            if (!string.IsNullOrEmpty(veiculo.ModeloVeicularCarga?.TipoVeiculoA52) && int.TryParse(veiculo.ModeloVeicularCarga?.TipoVeiculoA52, out nTipoVeiculoA52))
                retorno.tipo = nTipoVeiculoA52;
            else
                retorno.tipo = 1;

            retorno.tipoCarreta = 0;
            retorno.vinculo = veiculo.Tipo == "T" ? 2 : 1;
            retorno.limiteVelocidade = 0;
            retorno.ativo = true;
            retorno.equipamentos = null;
            retorno.capacidadeKg = veiculo.CapacidadeKG;
            retorno.capacidadeM3 = veiculo.CapacidadeM3;
            retorno.capacidadePallet = 0;
            retorno.idTipoEmblema = null;

            Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao repTecnologiaRastreadorCodigoIntegracao = new Repositorio.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao tecnologiaRastreadorCodigoIntegracao = null;

            if (veiculo.PossuiRastreador && veiculo.TecnologiaRastreador != null)
            {
                retorno.equipamentos = new List<envVeiculoEquipamento>();

                envVeiculoEquipamento equipamento = new envVeiculoEquipamento();
                equipamento.id = tecnologiaRastreadorCodigoIntegracao.Codigo;
                equipamento.codigo = veiculo.NumeroEquipamentoRastreador;
                equipamento.recebe_posicao = true;
                equipamento.recebe_macro = true;
                equipamento.recebe_evento = true;
                equipamento.recebe_mensagem = true;

                tecnologiaRastreadorCodigoIntegracao = repTecnologiaRastreadorCodigoIntegracao.BuscarPorTecnologiaRastreadorETipoIntegracao(veiculo.TecnologiaRastreador, cargaIntegracao.TipoIntegracao);

                int idTecnologia;
                if (!string.IsNullOrEmpty(tecnologiaRastreadorCodigoIntegracao?.CodigoIntegracao) && int.TryParse(tecnologiaRastreadorCodigoIntegracao?.CodigoIntegracao, out idTecnologia))
                    equipamento.idTecnologia = idTecnologia;

                retorno.equipamentos.Add(equipamento);
            }

            return retorno;
        }

        #endregion Métodos Privados

    }
}
