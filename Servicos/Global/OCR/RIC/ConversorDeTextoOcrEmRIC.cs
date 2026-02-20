using Dominio.ObjetosDeValor.OCR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicos.Global
{
    public class ConversorDeTextoOcrEmRIC
    {
        private readonly ServicoOCR _servicoOCR;
        private readonly byte[] _file;

        public ConversorDeTextoOcrEmRIC(ServicoOCR servicoOCR, byte[] file)
        {
            _servicoOCR = servicoOCR;
            _file = file;
        }

        public IObjetoModeloRic ExecutarConversao()
        {
            var respostaOCR = _servicoOCR.ExecutarServico(_file, false);
            if (!respostaOCR.Sucesso())
                throw new Exception(respostaOCR.ErrorMessage.LastOrDefault());
            
            var modelo = DetectarModelo(respostaOCR);
            if (modelo == null)
                throw new Exception("Não foi possivel identificar o modelo de RIC enviado.");
            
            if (modelo is ModeloRic_4)
                respostaOCR = _servicoOCR.ExecutarServico(_file, true);

            if (modelo is ModeloRic_7)
                respostaOCR = _servicoOCR.ExecutarServico(_file, true);

            if (!respostaOCR.Sucesso())
                throw new Exception(respostaOCR.ErrorMessage.LastOrDefault());

            PreencherCampos(respostaOCR, modelo);
            return modelo;
        }

        private IObjetoModeloRic DetectarModelo(RespostaOCR respostaOCR)
        {
            var textoOcr = respostaOCR.ObterTexto();
            var tipos = new List<Type>()
            {
                typeof(ModeloRic_3),
                typeof(ModeloRic_4),
                typeof(ModeloRic_5),
                typeof(ModeloRic_6),
                typeof(ModeloRic_7),
                typeof(ModeloRic_8),
                typeof(ModeloRic_9),
                typeof(ModeloRic_10),
                typeof(ModeloRic_11),
                typeof(ModeloRic_12),
                typeof(ModeloRic_13),
            };
            foreach (var t in tipos)
            {
                IObjetoModeloRic obj = (IObjetoModeloRic)Activator.CreateInstance(t);
                foreach (var id in obj.IdentificadorDoModeloOCR)
                    if (textoOcr.Contains(id))
                        return obj;
            }
            return null;
        }

        private IObjetoModeloRic PreencherCampos(RespostaOCR respostaOCR, IObjetoModeloRic modeloRic)
        {
            if (modeloRic is ModeloRic_4)
                return PreencherCamposDoModelo_4(respostaOCR, modeloRic);

            //if (modeloRic is ModeloRic_5)
              //  return PreencherCamposDoModelo_5(respostaOCR, modeloRic);

            // if (modeloRic is ModeloRic_6)
            //     return PreencherCamposDoModelo_6(respostaOCR, modeloRic);

            // if (modeloRic is ModeloRic_7)
            //    return PreencherCamposDoModelo_7(respostaOCR, modeloRic);

            //if (modeloRic is ModeloRic_8)
            //    return PreencherCamposDoModelo_8(respostaOCR, modeloRic);

            if (modeloRic is ModeloRic_12)
                return PreencherCamposDoModelo_12(respostaOCR, modeloRic);


            var textoOCR = respostaOCR.ObterTextoEmLinhas();

            for (int i = 0; i < textoOCR.Count; i++)
            {
                string linhaTexto = textoOCR[i];

                if (string.IsNullOrWhiteSpace(modeloRic.DataDeColeta))
                {
                    modeloRic.DataDeColeta = ExtratorDeDadosRic.Data(ExtrairValor(textoOCR, i, modeloRic.DataDeColeta_ModeloOCR));
                    if (modeloRic is ModeloRic_3)
                        if (textoOCR.Count > i + 2)
                        {
                            modeloRic.Container = textoOCR[i + 1].Trim(":".ToCharArray()).Trim();
                            modeloRic.TipoContainer = textoOCR[i + 2].Trim(":".ToCharArray()).Trim();
                        }
                    if (modeloRic is ModeloRic_11)
                        if (modeloRic.DataDeColeta.Length > 16)
                            modeloRic.DataDeColeta = modeloRic.DataDeColeta.Substring(0, 16);

                    if (!(modeloRic is ModeloRic_11))
                    {
                        modeloRic.DataDeColeta += " " + ExtratorDeDadosRic.Hora(ExtrairValor(textoOCR, i, new List<string> { "HORA/TIME", "HORA", "TIME" }, false, 1, ExtratorDeDadosRic._regexHora));
                        modeloRic.DataDeColeta = modeloRic.DataDeColeta.Trim();
                    }
                }
                else if (modeloRic.DataDeColeta.Length == 10)
                {
                    modeloRic.DataDeColeta += " " + ExtratorDeDadosRic.Hora(ExtrairValor(textoOCR, i, new List<string> { "HORA/TIME", "HORA", "TIME" }, false, 1, ExtratorDeDadosRic._regexHora));
                    modeloRic.DataDeColeta = modeloRic.DataDeColeta.Trim();
                }

                if (string.IsNullOrEmpty(modeloRic.Container))
                    modeloRic.Container = ExtratorDeDadosRic.Container(ExtrairValor(textoOCR, i, modeloRic.Container_ModeloOCR, false, 1, ExtratorDeDadosRic._regexContainer));

                if (string.IsNullOrEmpty(modeloRic.TipoContainer))
                {
                    modeloRic.TipoContainer = ExtrairValor(textoOCR, i, modeloRic.TipoContainer_ModeloOCR);
                    if (!string.IsNullOrEmpty(modeloRic.TipoContainer) && modeloRic is ModeloRic_13)
                        modeloRic.TipoContainer = ExtrairNumeros(textoOCR[i - 1]) + modeloRic.TipoContainer;

                    if(modeloRic is ModeloRic_9)
                        modeloRic.TipoContainer = ExtrairValor(textoOCR, i, modeloRic.TipoContainer_ModeloOCR, false, 1, ExtratorDeDadosRic._regexTipoContainer);
                    
                }

                if (string.IsNullOrEmpty(modeloRic.TaraContainer))
                {
                    if(modeloRic is ModeloRic_4)
                        modeloRic.TaraContainer = ExtratorDeDadosRic.Tara(ExtrairValor(textoOCR, i, modeloRic.TaraContainer_ModeloOCR));
                    else
                        modeloRic.TaraContainer = ExtratorDeDadosRic.Tara(ExtrairValor(textoOCR, i, modeloRic.TaraContainer_ModeloOCR, false, 1, ExtratorDeDadosRic._regexTara));
                }

                if (string.IsNullOrEmpty(modeloRic.ArmadorBooking))
                    modeloRic.ArmadorBooking = ExtrairValor(textoOCR, i, modeloRic.ArmadorBooking_ModeloOCR);

                if (string.IsNullOrEmpty(modeloRic.Transportadora))
                {
                    modeloRic.Transportadora = ExtrairValor(textoOCR, i, modeloRic.Transportadora_ModeloOCR, true);

                    if (modeloRic is ModeloRic_10)
                        if (i + 1 < textoOCR.Count)
                            if (!textoOCR[i + 1].Contains(":"))
                                modeloRic.Transportadora += " " + textoOCR[i + 1];

                    if (modeloRic is ModeloRic_13 && !string.IsNullOrEmpty(modeloRic.Transportadora))
                        foreach(var p in modeloRic.Placa_ModeloOCR)
                            if (modeloRic.Transportadora.Contains(p))
                                modeloRic.Transportadora = string.Empty;
                    
                }
                if (string.IsNullOrEmpty(modeloRic.Motorista))
                {
                    modeloRic.Motorista = ExtrairValor(textoOCR, i, modeloRic.Motorista_ModeloOCR);
                    if (modeloRic is ModeloRic_10)
                        if (i + 1 < textoOCR.Count)
                            if (!textoOCR[i + 1].Contains(":"))
                                modeloRic.Motorista += " " + textoOCR[i + 1];

                    if(modeloRic is ModeloRic_9)
                    {
                        if (!string.IsNullOrWhiteSpace(modeloRic.Motorista))
                        {
                            var motoristaEPlaca = modeloRic.Motorista.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            if(motoristaEPlaca.Length > 1)
                                modeloRic.Motorista = motoristaEPlaca[1]?.Trim();
                        }
                    }
                }

                if (string.IsNullOrEmpty(modeloRic.Placa))
                    modeloRic.Placa = ExtratorDeDadosRic.Placa(ExtrairValor(textoOCR, i, modeloRic.Placa_ModeloOCR));
            }

            return modeloRic;
        }

        private IObjetoModeloRic PreencherCamposDoModelo_4(RespostaOCR respostaOCR, IObjetoModeloRic modeloRic)
        {
            var textoLinhas = respostaOCR.ObterTextoEmLinhas(false);
            var texto = respostaOCR.ObterTexto().Replace("\t", " ").Replace("\r", string.Empty).Replace("  ", " ").Trim();
            Regex regexContainer_1 = new Regex(@"(\w{4}\d{6}-\d{1})", RegexOptions.IgnoreCase);
            Regex regexContainer_2 = new Regex(@"(\w{4} \d{6} \d{1})", RegexOptions.IgnoreCase);
            Regex regexContainer_3 = new Regex(@"(\w{4} \d{6}-\d{1})", RegexOptions.IgnoreCase);
            Regex regexContainer_4 = new Regex(@"(\w{4} \d{3} \d{3}-\d{1})", RegexOptions.IgnoreCase);

            Regex regexPlaca = new Regex(@"(\w{3}\d{4})|(\w{3}-\d{4})|(\w{3}\d{1}\w{1}\d{2})|(\w{3}-\d{1}\w{1}\d{2})", RegexOptions.IgnoreCase);

            Match rxResult = regexContainer_1.Match(texto);

            if (!rxResult.Success)
                rxResult = regexContainer_2.Match(texto);

            if (!rxResult.Success)
                rxResult = regexContainer_3.Match(texto);

            if (!rxResult.Success)
                rxResult = regexContainer_4.Match(texto);

            if (!rxResult.Success)
                return null;

            var nContainer = rxResult.Value;

            var tipoContainerIndex = texto.IndexOf(nContainer, StringComparison.InvariantCultureIgnoreCase) + nContainer.Length + 1;
            var tipoContainerLength = texto.IndexOf(" ", tipoContainerIndex) - tipoContainerIndex;

            var containerTipo = texto.Substring(tipoContainerIndex, tipoContainerLength);

            var taraIndex = tipoContainerIndex + tipoContainerLength + 1;
            var taraLength = texto.IndexOf(" ", taraIndex) - taraIndex;
            var tara = texto.Substring(taraIndex, taraLength);

            var dataIndex = texto.IndexOf("DATE");
            var data = string.Empty;
            var hora = string.Empty;
            if (dataIndex > -1)
            {
                var idxHora = texto.IndexOf("HORA", dataIndex);
                var idxFimHora = texto.IndexOf("\n", idxHora);

                if (idxFimHora - idxHora > 0 && idxFimHora - idxHora < texto.Length)
                {
                    hora = texto.Substring(idxHora, idxFimHora - idxHora);
                    hora = ExtratorDeDadosRic.Hora(hora);
                }

                var dataLength = idxHora - dataIndex;
                if (dataLength > 0)
                {
                    data = texto.Substring(dataIndex + 4, dataLength);
                    data = data.Replace(":", string.Empty).Replace(")", string.Empty).Replace("HORA", string.Empty).Trim();
                    data = data.Replace(".", "/");
                }
            }

            modeloRic.DataDeColeta = (ExtratorDeDadosRic.Data(data) + " " + hora).Trim();

            modeloRic.Container = ExtratorDeDadosRic.Container(nContainer);
            modeloRic.TipoContainer = containerTipo;
            modeloRic.TaraContainer = ExtratorDeDadosRic.Tara(tara);

            for (int i = 0; i < textoLinhas.Count; i++)
            {
                var linha = textoLinhas[i];
                var armadorOCR = new List<string> { "ARMADOR", "SHIPPER", "LACRE AG" };
                var transpOCR = new List<string> { "TRANSPORTADOR", "CARRIER", "MOTORISTA", "DRIVER" };

                bool linhaArmador = false;
                bool linhaTransportador = false;

                armadorOCR.ForEach(x =>
                {
                    if (linha.Contains(x)) linhaArmador = true;
                });

                if (!linhaArmador)
                    transpOCR.ForEach(x =>
                    {
                        if (linha.Contains(x)) linhaTransportador = true;
                    });

                if (linhaArmador)
                {
                    modeloRic.ArmadorBooking = textoLinhas[i + 1];
                    var idxFimArmador = modeloRic.ArmadorBooking.IndexOf("\t");
                    if (idxFimArmador > -1)
                        modeloRic.ArmadorBooking = modeloRic.ArmadorBooking.Substring(0, idxFimArmador).Trim();

                }
                if (linhaTransportador)
                {
                    var transp_motorista_placa = textoLinhas[i + 1];
                    modeloRic.Transportadora = textoLinhas[i + 1];
                    var idxFimTransportadora = modeloRic.Transportadora.IndexOf("\t");
                    if (idxFimTransportadora > -1)
                        modeloRic.Transportadora = modeloRic.Transportadora.Substring(0, idxFimTransportadora).Trim();

                    transp_motorista_placa = transp_motorista_placa.Replace(modeloRic.Transportadora, string.Empty).Trim();
                    if (transp_motorista_placa.IndexOf("\t") == 0)
                        transp_motorista_placa = transp_motorista_placa.Substring(1);

                    var idxFimMotorista = transp_motorista_placa.IndexOf("\t");
                    modeloRic.Motorista = transp_motorista_placa.Substring(0, idxFimMotorista);

                    var placas = regexPlaca.Matches(transp_motorista_placa);
                    if (placas.Count > 0)
                    {
                        var p = placas[0];
                        modeloRic.Placa = p.Value?.Replace("-", string.Empty)?.Trim();
                    }
                    modeloRic.Placa = ExtratorDeDadosRic.Placa(modeloRic.Placa);
                }
            }
            return modeloRic;
        }

        private IObjetoModeloRic PreencherCamposDoModelo_5(RespostaOCR respostaOCR, IObjetoModeloRic modeloRic)
        {
            var textoLinhas = respostaOCR.ObterTextoEmLinhas();

            for (int i = 0; i < textoLinhas.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(modeloRic.DataDeColeta))
                {
                    modeloRic.DataDeColeta = ExtratorDeDadosRic.Data(ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.DataDeColeta_ModeloOCR));
                    modeloRic.DataDeColeta += " " + ExtratorDeDadosRic.Hora(ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, new List<string> { "HORA" }));
                    modeloRic.DataDeColeta = modeloRic.DataDeColeta.Trim();
                }
                else if (modeloRic.DataDeColeta.Length == 10)
                {
                    modeloRic.DataDeColeta += " " + ExtratorDeDadosRic.Hora(ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, new List<string> { "HORA" }));
                    modeloRic.DataDeColeta = modeloRic.DataDeColeta.Trim();
                }

                if (string.IsNullOrEmpty(modeloRic.Container))
                    modeloRic.Container = ExtratorDeDadosRic.Container(ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.Container_ModeloOCR));

                if (string.IsNullOrEmpty(modeloRic.TipoContainer))
                    modeloRic.TipoContainer = ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.TipoContainer_ModeloOCR);

                if (string.IsNullOrEmpty(modeloRic.TaraContainer))
                    modeloRic.TaraContainer = ExtratorDeDadosRic.Tara(ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.TaraContainer_ModeloOCR));

                if (string.IsNullOrEmpty(modeloRic.ArmadorBooking))
                    modeloRic.ArmadorBooking = ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.ArmadorBooking_ModeloOCR);
                else
                    modeloRic.ArmadorBooking = modeloRic.ArmadorBooking.Replace("CNT", string.Empty).Replace("BKG", string.Empty).Trim();

                if (string.IsNullOrEmpty(modeloRic.Transportadora))
                    modeloRic.Transportadora = ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.Transportadora_ModeloOCR, true);

                if (string.IsNullOrEmpty(modeloRic.Motorista))
                    modeloRic.Motorista = ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.Motorista_ModeloOCR);

                if (string.IsNullOrEmpty(modeloRic.Placa))
                    modeloRic.Placa = ExtratorDeDadosRic.Placa(ExtratorDeDadosRic.ExtrairValor(textoLinhas, i, modeloRic.Placa_ModeloOCR));
            }
            return modeloRic;
        }

        
        private IObjetoModeloRic PreencherCamposDoModelo_12(RespostaOCR respostaOCR, IObjetoModeloRic modeloRic)
        {
            var textoOCR = respostaOCR.ObterTextoEmLinhas();

            for (int i = 0; i < textoOCR.Count; i++)
            {
                string linhaTexto = textoOCR[i];

                if (string.IsNullOrEmpty(modeloRic.DataDeColeta))
                {
                    if (linhaTexto.StartsWith("DATA"))
                        modeloRic.DataDeColeta = textoOCR[i + 1];
                }
                else
                {
                    if (linhaTexto.StartsWith("HORA"))
                        modeloRic.DataDeColeta += " " + textoOCR[i + 1];
                }

                if (string.IsNullOrEmpty(modeloRic.Container))
                {
                    if (linhaTexto.StartsWith("CONTAINER"))
                        modeloRic.Container = textoOCR[i + 1].Replace("-", string.Empty).Replace(" ", string.Empty);
                }

                if (string.IsNullOrEmpty(modeloRic.TipoContainer))
                {
                    if (linhaTexto.StartsWith("TIPO"))
                        modeloRic.TipoContainer = textoOCR[i + 2].Replace(" ", string.Empty);
                }

                if (string.IsNullOrEmpty(modeloRic.TaraContainer))
                {
                    if (linhaTexto.StartsWith("TARA"))
                        modeloRic.TaraContainer = textoOCR[i + 1];
                }

                if (string.IsNullOrEmpty(modeloRic.ArmadorBooking))
                {
                    if (linhaTexto.StartsWith("ARMADOR"))
                        modeloRic.ArmadorBooking = textoOCR[i + 1];
                }

                if (string.IsNullOrEmpty(modeloRic.Transportadora))
                {
                    if (linhaTexto.StartsWith("TRANSPORT"))
                        modeloRic.Transportadora = textoOCR[i + 1];
                }

                if (string.IsNullOrEmpty(modeloRic.Motorista))
                {
                    if (linhaTexto.StartsWith("MOTORISTA"))
                        modeloRic.Motorista = textoOCR[i + 1];
                }

                if (string.IsNullOrEmpty(modeloRic.Placa))
                {
                    if (linhaTexto.StartsWith("PLACA"))
                        modeloRic.Placa = textoOCR[i + 1].Replace("-", string.Empty);
                }
            }
            return modeloRic;
        }


        private string ExtrairNumeros(string valor)
        {
            Regex regexNumeros = new Regex(@"(\d+)");
            var m = regexNumeros.Match(valor);
            return m.Success ? m.Value : string.Empty;
        }

        private string ExtrairValor(List<string> textoOCR, int i, List<string> modelosDeCampoOCR, bool transportadora = false, int numeroRecursao = 1, Regex filtro = null)
        {
            return ExtratorDeDadosRic.ExtrairValor(textoOCR, i, modelosDeCampoOCR, transportadora, numeroRecursao, filtro);

            /* string valor = linhaTexto;
             foreach (var str in stringsModeloOCR)
                 valor = valor.Replace(str, string.Empty);

             valor = valor.Replace("•", string.Empty).Replace("*", string.Empty);
             valor = valor.Trim().Trim("-".ToCharArray()).Trim(".".ToCharArray()).Trim(":".ToCharArray()).Trim();
             return valor;*/
        }

        public ObjetoRicRetorno ConverterEmDTO(IObjetoModeloRic modeloRic)
        {
            return new ObjetoRicRetorno
            {
                ArmadorBooking = modeloRic.ArmadorBooking,
                Container = modeloRic.Container?.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpperInvariant().Trim(),
                DataDeColeta = modeloRic.DataDeColeta,
                Motorista = modeloRic.Motorista,
                Placa = modeloRic.Placa,
                TaraContainer = (int)modeloRic.TaraContainer?.Trim().ToInt(),
                TipoContainer = modeloRic.TipoContainer,
                Transportadora = modeloRic.Transportadora
            };
        }

        /*  private string ExtrairTexto(string valorLinha, int tamanhoMaximoPalavra, string valorSeguinte, string separador = "=", List<string> lista = null)
          {
              if (lista == null)
                  lista = new List<string>();

              if (valorLinha.Length <= tamanhoMaximoPalavra)
                  return valorSeguinte.Trim();

              if (valorLinha.Contains(separador) && separador == "=")
              {
                  var valores = valorLinha.Split('=');
                  if (!string.IsNullOrEmpty(valores[1]))
                      return valores[1].Trim();
              }
              else if (separador == ":" && valorLinha.Contains(separador))
              {
                  var valores = valorLinha.Split(':');
                  if (valores.Count() > 0 && !string.IsNullOrEmpty(valores[1]))
                      return valores[1].Trim();
                  else
                      return valorSeguinte;

              }
              else
                  return ExtrairValor(valorLinha, lista);

              return string.Empty;
          }*/

        private string ExtrairPlaca(string linhaPlaca)
        {

            Regex regex = new Regex(@"([A-Z]{3}\d{4})|([A-Z]{3}-\d{4})|([A-Z]{3}-\d{1}[A-Z]\d{2})");
            var placaValida = regex.Match(linhaPlaca);
            if (!string.IsNullOrEmpty(placaValida.Groups[0].Value))
            {
                if (!linhaPlaca.Contains("PLACA"))
                {
                    var validarString = linhaPlaca.Substring(7);
                    if (!string.IsNullOrEmpty(validarString))
                        return string.Empty;
                }
                return placaValida.Groups[0].Value;
            }


            return string.Empty;
        }

        private string ExtrairData(string linha)
        {
            Regex reg = new Regex(@"([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}|([0-9](\/)\d{2}(\/)\d{4})");
            var validos = reg.Match(linha);
            if (!string.IsNullOrEmpty(validos.Groups[0].Value))
                return validos.Groups[0].Value;

            return string.Empty;
        }
        private string ExtrairContainer(string linha)
        {
            Regex reg = new Regex(@"([a-zA-Z]{4}\d{7})|([a-zA-Z]{4}-\d{7})|([a-zA-Z]{4}\s\d{7})|([a-zA-Z]{4}\s\d{6}-\d{1})|([a-zA-Z]{4}\s\d{3}-\d{3}-\d{1})");
            var numeroContainer = reg.Match(linha);
            if (!string.IsNullOrEmpty(numeroContainer.Groups[0].Value))
                if (!linha.Contains("CONT"))
                    return string.Empty;
                else
                    return numeroContainer.Groups[0].Value;

            return string.Empty;
        }

    }
}
