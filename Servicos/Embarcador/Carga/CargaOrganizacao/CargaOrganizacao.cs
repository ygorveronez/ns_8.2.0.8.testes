using Dominio.Interfaces.Database;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.Carga.CargaOrganizacao
{
    public class CargaOrganizacao : ServicoBase
    {
        #region Construtores

        public CargaOrganizacao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #endregion

        #region Métodos Públicos
        public void RemoverCargaOrganizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaCargaOrganizacao repositorioCargaOrganizacao = new Repositorio.Embarcador.Cargas.CargaCargaOrganizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao> listaCargaOrganizacaoExistente = repositorioCargaOrganizacao.BuscarPorCarga(carga.Codigo);

            if (listaCargaOrganizacaoExistente.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao item in listaCargaOrganizacaoExistente)
                {
                    repositorioCargaOrganizacao.Deletar(item);
                }
            }
        }

        public void CriarCargaOrganizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrganizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaOrganizacao repositorioCargaOrganizacao = new Repositorio.Embarcador.Cargas.CargaCargaOrganizacao(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga item in cargasOrganizacao)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao cargaCargaOrganizacao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao
                {
                    Carga = carga,
                    CargaOrganizacao = item
                };
                repositorioCargaOrganizacao.Inserir(cargaCargaOrganizacao);
            }
        }
        #endregion
    }
}
