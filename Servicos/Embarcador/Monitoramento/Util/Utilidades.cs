using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento
{
    public static class Util
    {
        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);
        private static DateTime unixEpochUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static List<double> ExtrairDouble(string conteudo, char separador = ',')
        {
            List<double> partes = new List<double>();
            if (!string.IsNullOrWhiteSpace(conteudo))
            {
                string[] codigosClientesAlvosAtuais = conteudo.Split(separador);
                int total = codigosClientesAlvosAtuais.Length;
                for (int i = 0; i < total; i++)
                {
                    if (!string.IsNullOrWhiteSpace(codigosClientesAlvosAtuais[i]))
                    {
                        double codigoClienteAlvoAtual = double.Parse(codigosClientesAlvosAtuais[i]);
                        partes.Add(codigoClienteAlvoAtual);
                    }
                }
            }
            return partes;
        }

        public static List<double> Intersecao(List<double> lista1, List<double> lista2)
        {
            List<double> intersecao = new List<double>();
            int totalLista1 = lista1?.Count ?? 0;
            int totalLista2 = lista2?.Count ?? 0;
            for (int i = 0; i < totalLista1; i++)
                for (int j = 0; j < totalLista2; j++)
                    if (lista1[i] == lista2[j])
                        intersecao.Add(lista1[i]);
            return intersecao;
        }

        public static long ObterMiliseconds(DateTime data)
        {
            long milisegundos = (long)(data - unixEpoch).TotalMilliseconds;
            return milisegundos;
        }

        public static DateTime ObterDataPelosMilisegundos(long milisegundos)
        {
            DateTime data = unixEpochUTC.AddMilliseconds(milisegundos);
            return data.ToLocalTime();
        }

    }
}