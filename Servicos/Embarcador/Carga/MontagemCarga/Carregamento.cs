using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public static class Carregamento
    {
        /// <summary>
        /// Método para validar carregamentos com situação diferente de Fechado com carga e Fechar os carregamentos ao Cancelar ou Finalizar uma sessão de roteirização.
        /// </summary>
        /// <param name="codigoSessaoRoteirizador">Código da sessão de roteirização para analisar os carregamentos.</param>
        /// <param name="situacao">Situação dos carregamentos</param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public static List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> ValidarCarregamentosSessaoRoteirizador(int codigoSessaoRoteirizador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento situacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento repositorioCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> pendentes = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento>();

            //Frimesa, possui carregamentos EM MONTAGEM porem com carga gerada, não  foi possível identificar a origem da situação ficar em MONTAGEM... vamos finalizar o carregamento.
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento> carregamentosPendentes = repositorioCarregamento.CarregamentosSessaoRoteirizador(codigoSessaoRoteirizador, situacao);
            foreach (Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento in carregamentosPendentes)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCarregamento(carregamento.Codigo);
                if (carga != null && (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.EmMontagem || situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.GerandoCargaBackground))
                {
                    carregamento.SituacaoCarregamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Fechado;
                    repositorioCarregamento.Atualizar(carregamento);
                }
                else
                    pendentes.Add(carregamento);
            }
            return pendentes;
        }

    }
}
