using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Imposto
{
    public sealed class OutraAliquotaInstance
    {
        #region Atributos Globais

        private static OutraAliquotaInstance _instancia;
        public List<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquota> OutrasAliquotas { get; private set; }

        #endregion

        #region Construtores

        private OutraAliquotaInstance()
        {
        }

        public static OutraAliquotaInstance GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (_instancia == null)
            {
                _instancia = new OutraAliquotaInstance();
                _instancia.CarregarTodasAliquotasAsync(unitOfWork).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return _instancia;
        }

        public static async Task<OutraAliquotaInstance> GetInstanceAsync(Repositorio.UnitOfWork unitOfWork)
        {
            if (_instancia == null)
            {
                _instancia = new OutraAliquotaInstance();
                await _instancia.CarregarTodasAliquotasAsync(unitOfWork);
            }

            return _instancia;
        }

        #endregion

        #region Métodos Públicos

        public async Task AtualizarOutraAliquotaInstanceAsync(Repositorio.UnitOfWork unitOfWork)
        {
            OutraAliquotaInstance instance = await GetInstanceAsync(unitOfWork);
            await instance.CarregarTodasAliquotasAsync(unitOfWork);
        }

        #endregion

        #region Métodos Privados

        private async Task CarregarTodasAliquotasAsync(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas repositorioOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork);
            Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto repositorioOutrasAliquotasImposto = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotasImposto(unitOfWork);

            OutrasAliquotas = (await repositorioOutrasAliquotas.BuscarTodasOutrasAliquotasAsync().ConfigureAwait(false)).ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Imposto.OutraAliquotaImposto> outraAliquotaImpostos = (await repositorioOutrasAliquotasImposto.BuscarTodasOutrasAliquotasImpostoAsync().ConfigureAwait(false)).ToList();

            for (int i = 0; i < OutrasAliquotas.Count; i++)
            {
                OutrasAliquotas[i].Impostos = outraAliquotaImpostos.FindAll(obj => obj.CodigoOutraAliquota == OutrasAliquotas[i].Codigo);
            }
        }

        #endregion
    }
}
