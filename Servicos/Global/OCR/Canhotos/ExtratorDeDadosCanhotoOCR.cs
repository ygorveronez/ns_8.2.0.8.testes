using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Global.OCR.Canhotos
{
    public class ExtratorDeDadosCanhotoOCR
    {
        public List<CanhotoOCR> ObterCanhotosMinerva(RespostaOCR json, List<int> numerosNotasFiscais)
        {
            if (!json.Sucesso())
                return null;

            if (json.ParsedResults == null || !json.ParsedResults.Any())
                return null;

            var linhas = json.ParsedResults[0].TextOverlay?.Lines;

            if (linhas.IsNullOrEmpty())
                return null;

            //var culture = new CultureInfo("en-US");
            var listaRetorno = new List<CanhotoOCR>();
            int index = -1;
            bool inicioDeCanhoto = false;
            bool fimDeCanhoto = false;
            bool podeProcurarPelaNfe = false;
            //string tresPrimeirosDigitosDaNfe = string.Empty;
            //string[] prefixosNfe = new string[] { "Nº", "N°", "VA", "VO", "V°", "Vº", "Nª" };
            List<int> numerosEncontrados = new List<int>();

            foreach (var l in linhas)
            {
                inicioDeCanhoto = false;
                fimDeCanhoto = false;
                if (l.LineText.ToUpperInvariant().StartsWith("RECEBEMOS DE MINERVA"))
                {
                    inicioDeCanhoto = true;
                    podeProcurarPelaNfe = false;
                }
                else
                {
                    podeProcurarPelaNfe = true;
                }
                if (StartsWithAny(l.LineText, new string[] { "SÉRIE", "SERIE" }))
                {
                    fimDeCanhoto = true;
                }
                if (fimDeCanhoto)
                {
                    podeProcurarPelaNfe = false;
                    //deve voltar pra procurar o numero da nfe se a nfe ainda nao tiver numero
                }

                if (podeProcurarPelaNfe && index > -1)
                {
                    foreach(var numero in numerosNotasFiscais)
                        if (l.LineText.Contains(numero.ToString()))
                        {
                            if (numerosEncontrados.Contains(numero))
                                break;

                            listaRetorno[index].NumeroNFe = numero;
                            numerosEncontrados.Add(numero);
                            break;
                        }
                    
                    //if(listaRetorno[index].NumeroNFe <= 0)
                    //    if (StartsWithAny(l.LineText, prefixosNfe))
                    //        listaRetorno[index].NumeroNFe = l.LineText.ObterSomenteNumeros().ToInt();
                }

                if (inicioDeCanhoto)
                {
                    if (index > -1 && listaRetorno.Any())
                        listaRetorno[index].Height = (int)l.MinTop - listaRetorno[index].Y;
                    
                    index++;
                    listaRetorno.Add(new CanhotoOCR
                    {
                        Y = (int)l.MinTop
                    });
                }
            }
            return listaRetorno;
        }
        private bool StartsWithAny(string value, string[] values)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            foreach (var item in values)
            {
                bool yes = value.Trim().ToUpperInvariant().StartsWith(item);
                if (yes)
                    return true;
            }
            return false;
        }
    }

    

    public class CanhotoOCR
    {
        public int Y { get; set; }
        public int Height { get; set; }
        public int NumeroNFe { get; set; }
        public string Base64 { get; set; }
        public DateTime DataDeCriacao { get; set; }
    }
}
