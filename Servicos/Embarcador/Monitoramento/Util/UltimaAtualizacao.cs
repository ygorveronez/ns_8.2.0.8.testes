using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Monitoramento
{
    public static class UltimaAtualizacao
    {
        public static Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao ObterUltimaAtualizacao(int codigo, ref List<Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao> ultimasAtualicaoes)
        {
            if (ultimasAtualicaoes == null) ultimasAtualicaoes = new List<Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao>();
            int total = ultimasAtualicaoes.Count;
            for (int i = 0; i < total; i++)
            {
                if (ultimasAtualicaoes[i].Codigo == codigo)
                {
                    return ultimasAtualicaoes[i];
                }
            }

            // Não encontrou, cria um novo, adiciona ao cache e retorna
            Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao ultima = new Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao
            {
                Codigo = codigo,
                Data = DateTime.MinValue
            };
            ultimasAtualicaoes.Add(ultima);

            return ultima;
        }

        public static bool VerrificaSeJaExpirou(Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao ultima, DateTime data, int totalMinutos)
        {
            if (totalMinutos <= 0)
            {
                return true;
            }

            if (data >= ultima.Data)
            {
                TimeSpan diferenca = data - ultima.Data;
                if (diferenca.TotalMinutes > totalMinutos)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool VerificaSeJaExpirouEAtualiza(ref Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao ultima, DateTime data, int totalMinutos)
        {
            bool jaExpirou = VerrificaSeJaExpirou(ultima, data, totalMinutos);
            if (jaExpirou) ultima.Data = data;
            return jaExpirou;
        }

    }
}