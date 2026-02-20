using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Frete.AlcadasTaxaDescarga
{
    public sealed class AprovacaoAlcadaTaxaDescarga : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga,
        Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga,
        Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente
    >
    {
        #region Construtores

        public AprovacaoAlcadaTaxaDescarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTaxaDescarga filtrosPesquisa)
        {
            var consultaTaxaDescarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>();
            var consultaAlcadaTaxaDescarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga>().Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Situacao.HasValue)
            {
                consultaTaxaDescarga = consultaTaxaDescarga.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

                if (filtrosPesquisa.Situacao.Value == SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao)
                    return consultaTaxaDescarga;
            }

            if (filtrosPesquisa.Valor > 0)
                consultaTaxaDescarga = consultaTaxaDescarga.Where(o => o.Valor == filtrosPesquisa.Valor);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                consultaTaxaDescarga = consultaTaxaDescarga.Where(o => o.InicioVigencia >= filtrosPesquisa.DataInicio.Date);

            if (filtrosPesquisa.DataLimite != DateTime.MinValue)
                consultaTaxaDescarga = consultaTaxaDescarga.Where(o => o.FimVigencia <= filtrosPesquisa.DataLimite.Date);

            if (filtrosPesquisa.CpfCnpjClientes.Count > 0)
                consultaTaxaDescarga = consultaTaxaDescarga.Where(o => o.Clientes.Any(c => filtrosPesquisa.CpfCnpjClientes.Contains(c.CPF_CNPJ)));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaTaxaDescarga = consultaTaxaDescarga.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
                consultaTaxaDescarga = consultaTaxaDescarga.Where(o => o.ModeloVeicular.Codigo == filtrosPesquisa.CodigoModeloVeicular);

            if (filtrosPesquisa.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao)
                consultaAlcadaTaxaDescarga = consultaAlcadaTaxaDescarga.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.Situacao.HasValue)
                return consultaTaxaDescarga.Where(o => consultaAlcadaTaxaDescarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaTaxaDescarga.Where(o =>
                o.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao ||
                consultaAlcadaTaxaDescarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
                );
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTaxaDescarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTaxaDescarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaTaxaDescarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTaxaDescarga filtrosPesquisa)
        {
            var consultaTaxaDescarga = Consultar(filtrosPesquisa);

            return consultaTaxaDescarga.Count();
        }

        public bool PossuiRegrasCadastradas()
        {
            var consultaAlcadaTaxaDescarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga>()
                .Where(o => o.Ativo);

            return consultaAlcadaTaxaDescarga.Any();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> ObterPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>();

            query = query.Where(o => o.Ativo && o.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga> BuscarAprovacoesPendentesPorTaxasDescarga(List<int> codigosTaxaDescarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga>();

            query = query.Where(o => !o.Bloqueada && o.Situacao == SituacaoAlcadaRegra.Pendente && codigosTaxaDescarga.Contains(o.OrigemAprovacao.Codigo));

            return query.Fetch(o => o.Usuario)
                        .Fetch(o => o.OrigemAprovacao)
                        .ToList();
        }

        #endregion
    }
}
