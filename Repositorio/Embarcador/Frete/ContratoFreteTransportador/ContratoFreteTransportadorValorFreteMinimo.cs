using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public sealed class ContratoFreteTransportadorValorFreteMinimo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo>
    {
        #region Construtores

        public ContratoFreteTransportadorValorFreteMinimo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo BuscarPorParametros(Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosContratoFreteTransportadorValorFreteMinimo parametros)
        {
            var consultaValorFreteMinimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo>()
                .Where(o => o.ContratoFreteTransportador.Codigo == parametros.CodigoContratoFreteTransportador);

            if (parametros.CodigoModeloVeicularCarga > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiModelosVeicularesCarga || o.ModelosVeicularesCarga.Any(m => m.Codigo == parametros.CodigoModeloVeicularCarga));

            if (parametros.CodigoTipoCarga > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiTiposCarga || o.TiposCarga.Any(t => t.Codigo == parametros.CodigoTipoCarga));

            if (parametros.ListaCpfCnpjClienteDestino?.Count > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiClientesDestino || o.ClientesDestino.Any(c => parametros.ListaCpfCnpjClienteDestino.Contains(c.CPF_CNPJ)));

            if (parametros.ListaCpfCnpjClienteOrigem?.Count > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiClientesOrigem || o.ClientesOrigem.Any(c => parametros.ListaCpfCnpjClienteOrigem.Contains(c.CPF_CNPJ)));

            if (parametros.ListaCodigoLocalidadeDestino?.Count > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiLocalidadesDestino || o.LocalidadesDestino.Any(l => parametros.ListaCodigoLocalidadeDestino.Contains(l.Codigo)));

            if (parametros.ListaCodigoLocalidadeOrigem?.Count > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiLocalidadesOrigem || o.LocalidadesOrigem.Any(l => parametros.ListaCodigoLocalidadeOrigem.Contains(l.Codigo)));

            if (parametros.ListaUfDestino?.Count > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiEstadosDestino|| o.EstadosDestino.Any(e => parametros.ListaUfDestino.Contains(e.Sigla)));

            if (parametros.ListaUfOrigem?.Count > 0)
                consultaValorFreteMinimo = consultaValorFreteMinimo.Where(o => !o.PossuiEstadosOrigem || o.EstadosOrigem.Any(e => parametros.ListaUfOrigem.Contains(e.Sigla)));

            return consultaValorFreteMinimo
                .OrderByDescending(o =>
                    (Convert.ToInt32(o.PossuiClientesOrigem) * 13) +
                    (Convert.ToInt32(o.PossuiLocalidadesOrigem) * 12) +
                    (Convert.ToInt32(o.PossuiEstadosOrigem) * 11) +
                    (Convert.ToInt32(o.PossuiClientesDestino) * 13) +
                    (Convert.ToInt32(o.PossuiLocalidadesDestino) * 12) +
                    (Convert.ToInt32(o.PossuiEstadosDestino) * 11) +
                    (Convert.ToInt32(o.PossuiTiposCarga) * 10) +
                    (Convert.ToInt32(o.PossuiModelosVeicularesCarga) * 10)
                )
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo> BuscarPorContratoFreteTransportador(int codigoContratoFreteTransportador)
        {
            var consultaValorFreteMinimo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo>()
                .Where(o => o.ContratoFreteTransportador.Codigo == codigoContratoFreteTransportador);

            return consultaValorFreteMinimo.ToList();
        }

        #endregion Métodos Públicos
    }
}
