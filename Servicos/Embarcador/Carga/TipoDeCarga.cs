using System.Collections.Generic;

namespace Servicos.Embarcador.Carga
{
    public class TipoCarga
    {
        public static Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoDeCargaPorRegra(Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.RegraTipoCarga repRegraTipoCarga = new Repositorio.Embarcador.Cargas.RegraTipoCarga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga regra = null;

            double destino = destinatario?.CPF_CNPJ ?? 0d;

            string uforigem = remetente?.Localidade.Estado.Sigla ?? string.Empty;
            string ufdestino = destinatario?.Localidade.Estado.Sigla ?? string.Empty;

            int grupoOrigem = remetente?.GrupoPessoas?.Codigo ?? 0;
            int grupoDestino = destinatario?.GrupoPessoas?.Codigo ?? 0;

            // Por destino
            List<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga> regrasDestinos = repRegraTipoCarga.BuscaPorDestino(destino);
            if (regrasDestinos.Count > 0)
                regra = RetornarRegraValida(regrasDestinos, destino, uforigem, ufdestino, grupoOrigem, grupoDestino);

            if (regra == null)
            {
                // Por UFs
                List<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga> regrasEstado = repRegraTipoCarga.BuscaPorUFs(uforigem, ufdestino);
                if (regrasEstado.Count > 0)
                    regra = RetornarRegraValida(regrasEstado, destino, uforigem, ufdestino, grupoOrigem, grupoDestino);
            }

            if (regra == null)
            {
                // Por Grupo
                List<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga> regrasGrupos = repRegraTipoCarga.BuscaPorGrupo(grupoOrigem, grupoDestino);
                if (regrasGrupos.Count > 0)
                    regra = RetornarRegraValida(regrasGrupos, destino, uforigem, ufdestino, grupoOrigem, grupoDestino);
            }

            if (regra != null)
                return regra.TipoCarga;

            return null;
        }

        public static Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga RetornarRegraValida(List<Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga> regras, double destino, string uforigem, string ufdestino, int grupoOrigem, int grupoDestino)
        {
            foreach(Dominio.Entidades.Embarcador.Cargas.RegraTipoCarga regra in regras)
            {
                bool valido = true;
                if (regra.ClienteDestino != null && regra.ClienteDestino.CPF_CNPJ != destino)
                    valido = false;

                if (!string.IsNullOrWhiteSpace(regra.UFOrigem) && regra.UFOrigemDiferente == false && regra.UFOrigem != uforigem)
                    valido = false;
                if (!string.IsNullOrWhiteSpace(regra.UFOrigem) && regra.UFOrigemDiferente == true && regra.UFOrigem == uforigem)
                    valido = false;
                if (!string.IsNullOrWhiteSpace(regra.UFDestino) && regra.UFDestinoDiferente == false && regra.UFDestino != ufdestino)
                    valido = false;
                if (!string.IsNullOrWhiteSpace(regra.UFDestino) && regra.UFDestinoDiferente == true && regra.UFDestino == ufdestino)
                    valido = false;

                else if (regra.GrupoPessoasOrigem != null && regra.GrupoPessoasOrigem.Codigo != grupoOrigem)
                    valido = false;
                else if (regra.GrupoPessoasDestino != null && regra.GrupoPessoasDestino.Codigo != grupoDestino)
                    valido = false;

                if (valido)
                    return regra;
            }

            return null;
        }
    }
}
