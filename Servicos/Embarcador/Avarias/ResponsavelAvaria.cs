using System.Collections.Generic;

namespace Servicos.Embarcador.Avarias
{
    public class ResponsavelAvaria
    {
        public static bool CriaResponsavelSolicitacao(Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria solicitacao, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.Embarcador.Avarias.ResponsavelAvaria repResponsavelAvaria = new Repositorio.Embarcador.Avarias.ResponsavelAvaria(unitOfWork);

            // Verifica se a ocorrencia se encaixa na regra de emissao
            List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada = AutorizacaoSolicitacaoAvaria.VerificarRegrasAutorizacaoAvaria(solicitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Lote, unitOfWork);
            List<Dominio.Entidades.Usuario> aprovadores = RetornarAprovadoresDiferentes(listaFiltrada);
            
            // Caso se encaixar, busca os aprovadores, que agora serao os responsaveis
            if (aprovadores.Count > 0)
            {
                // Cria lista de aprovadores
                foreach (Dominio.Entidades.Usuario aprovador in aprovadores)
                {
                    Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria responsavel = new Dominio.Entidades.Embarcador.Avarias.ResponsavelAvaria();
                    responsavel.Usuario = aprovador;
                    responsavel.SolicitacaoAvaria = solicitacao;
                    responsavel.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteAutorizacao.Pendente;

                    repResponsavelAvaria.Inserir(responsavel);
                }

                return true;
            }

            return false;
        }


        /* RetornarAprovadoresDiferentes
         * Itera todas regras e seus respestivos aprovadores
         * Retornar os aprovadores diferentes
         */
        private static List<Dominio.Entidades.Usuario> RetornarAprovadoresDiferentes(List<Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria> listaFiltrada)
        {
            // Busca uma lista de aprovadores unicos
            List<Dominio.Entidades.Usuario> aprovadores = new List<Dominio.Entidades.Usuario>();

            for (var i = 0; i < listaFiltrada.Count; i++)
                foreach (Dominio.Entidades.Usuario aprovador in listaFiltrada[i].Aprovadores)
                    if (!aprovadores.Contains(aprovador)) aprovadores.Add(aprovador);

            return aprovadores;
        }
    }
}
