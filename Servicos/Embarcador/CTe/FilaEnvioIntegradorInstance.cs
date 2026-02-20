using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Servicos.Embarcador.CTe
{
    public sealed class FilaEnvioIntegradorInstance
    {
        private static readonly Lazy<FilaEnvioIntegradorInstance> _instancia = new Lazy<FilaEnvioIntegradorInstance>(() => new FilaEnvioIntegradorInstance());
        private List<Dominio.ObjetosDeValor.Embarcador.CTe.FilaEnvioIntegrador> _filaEnvioIntegrador;
        private int _filaEnvioIntegradorAtual;
        private static readonly object _BuscarProximoLock = new object();

        private FilaEnvioIntegradorInstance() { }

        public static FilaEnvioIntegradorInstance GetInstance(Repositorio.UnitOfWork unitOfWork)
        {
            if (_instancia.Value._filaEnvioIntegrador == null)
                _instancia.Value.CarregarFilaEnvioIntegrador(unitOfWork);

            // Retornar a instância singleton
            return _instancia.Value;
        }

        public int BuscarProximaFilaEnvioIntegrador(int tipoEnvio)
        {
            lock (_BuscarProximoLock)
            {
                if (_filaEnvioIntegrador.Count > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.FilaEnvioIntegrador filaEnvioIntegrador = _filaEnvioIntegrador.Where(o => o.CodigoTipoEnvio > _filaEnvioIntegradorAtual).OrderBy(o => o.CodigoTipoEnvio).FirstOrDefault();

                    if (filaEnvioIntegrador == null)
                        filaEnvioIntegrador = _filaEnvioIntegrador.OrderBy(o => o.CodigoTipoEnvio).FirstOrDefault();

                    _filaEnvioIntegradorAtual = filaEnvioIntegrador.CodigoTipoEnvio;
                }
                else
                    _filaEnvioIntegradorAtual = tipoEnvio;
            }

            return _filaEnvioIntegradorAtual;
        }

        public void AtualizarFilaEnvioIntegrador(Repositorio.UnitOfWork unitOfWork)
        {
            CarregarFilaEnvioIntegrador(unitOfWork);
        }

        private void CarregarFilaEnvioIntegrador(Repositorio.UnitOfWork unitOfWork)
        {
            _filaEnvioIntegrador = new Repositorio.Embarcador.CTe.FilaEnvioIntegrador(unitOfWork).BuscarFilaEnvioIntegrador();
            _filaEnvioIntegradorAtual = _filaEnvioIntegrador?.Count() > 0 ? _filaEnvioIntegrador.OrderBy(o => o.CodigoTipoEnvio).FirstOrDefault().CodigoTipoEnvio : 0;
        }
    }
}