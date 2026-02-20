using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarRota(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Dominio.Entidades.RotaFrete rota, out int idRota, out string mensagemErro)
        {
            mensagemErro = null;
            idRota = 0;
            string mensagemLog = string.Empty;
            try
            {
                bool sucesso = false;

                if (!string.IsNullOrEmpty(rota?.CodigoIntegracaoGerenciadoraRisco))
                {
                    idRota = int.Parse(rota.CodigoIntegracaoGerenciadoraRisco);
                    return true;
                }

                Dominio.Entidades.Localidade origem = null;
                Dominio.Entidades.Localidade destino = null;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaIntegracao.Carga.Pedidos.FirstOrDefault();

                if (rota != null)
                {
                    if (rota.Remetente != null)
                        origem = rota.Remetente.Localidade;
                    else if (rota.LocalidadesOrigem?.Count > 0)
                        origem = rota.LocalidadesOrigem.FirstOrDefault();

                    if (rota.Destinatarios?.Count > 0)
                        destino = rota.Destinatarios.First().Cliente.Localidade;
                    else if (rota.Localidades?.Count > 0)
                        destino = rota.Localidades.OrderByDescending(x => x.Ordem).ThenByDescending(o => o.Codigo).FirstOrDefault()?.Localidade;
                }
                else
                {
                    origem = cargaPedido.Origem;
                    destino = cargaPedido.Destino;
                }

                if (origem == null)
                {
                    mensagemErro = "Não foi encontrada uma origem na rota da carga.";
                    return false;
                }

                if (destino == null)
                {
                    mensagemErro = "Não foi encontrado um destino na rota da carga.";
                    return false;
                }

                object envioWS = ObterRota(cargaIntegracao.Carga, rota, origem, destino);

                //Transmite o arquivo
                retornoWebService retornoWS = retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "rotas", this.tokenAutenticacao);

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON A52 rota: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro da rota; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        string mensagem = null;
                        retRota retRota = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                            else if (count == 2 && retorno.statusCode == "409")
                                retRota = message.ToString().FromJson<retRota>();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                        {
                            mensagemErro = "Ocorreu uma falha ao efetuar o cadastro da rota.";
                            sucesso = false;
                        }
                        else if (retRota == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro da rota; RetornoWS {0}.", mensagem);
                            sucesso = false;
                        }
                        else
                        {
                            idRota = (int)retRota.id;
                            sucesso = true;
                        }
                    }
                }
                else if (retornoWS.erro)
                {
                    mensagemErro = "Ocorreu uma falha ao efetuar o cadastro da rota.";
                    mensagemLog = mensagemErro;
                    sucesso = false;
                }
                else
                {
                    retRota retRota = retornoWS.jsonRetorno.ToString().FromJson<retRota>();
                    idRota = (int)retRota.id;

                    if (rota != null)
                    {
                        Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
                        rota.CodigoIntegracaoGerenciadoraRisco = idRota.ToString();
                        repRotaFrete.Atualizar(rota);
                    }
                    mensagemLog = "Rotas: Rota integrada com sucesso.";
                    sucesso = true;
                }
                
                SalvarArquivosIntegracao(cargaIntegracao, retornoWS.jsonEnvio, retornoWS.jsonRetorno, mensagemLog);

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados da rota.";
                return false;
            }
        }

        private envRota ObterRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.RotaFrete rota, Dominio.Entidades.Localidade origem, Dominio.Entidades.Localidade destino)
        {
            envRota retorno = new envRota();

            retorno.nome = null;
            retorno.identificador = null;
            retorno.ativo = true;

            if (rota != null)
                retorno.nome = rota.Descricao;
            else
                retorno.nome = $"{origem.Descricao} x {destino.Descricao} - {DateTime.Now.ToString("ddMMyyyyHHmmss")}";

            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargaRotaFrete != null && cargaRotaFrete.PolilinhaRota != null)
                retorno.polilinha = cargaRotaFrete.PolilinhaRota.Replace(@"\", @"\\");
            else
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPoints = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();

                wayPoints.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint((double)(origem?.Latitude ?? 0), (double)(origem?.Longitude ?? 0)));
                wayPoints.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint((double)(destino?.Latitude ?? 0), (double)(destino?.Longitude ?? 0)));

                retorno.polilinha = Servicos.Embarcador.Logistica.Polilinha.Codificar(wayPoints).Replace(@"\", @"\\");
            }

            return retorno;
        }

        #endregion Métodos Privados

    }
}
