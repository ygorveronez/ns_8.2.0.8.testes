using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ConfiguracaoDescargaCliente : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>
    {
        #region Construtores

        public ConfiguracaoDescargaCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaCliente filtrosPesquisa)
        {
            var consultaConfiguracaoDescargaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>();

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CpfCnpjCliente > 0d)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => o.Clientes.Any(c => c.CPF_CNPJ == filtrosPesquisa.CpfCnpjCliente));

            if (filtrosPesquisa.Status != SituacaoAtivoPesquisa.Todos)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => o.Ativo == (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Ativo));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.DataVigenciaInicial != DateTime.MinValue)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => o.InicioVigencia != null && filtrosPesquisa.DataVigenciaInicial <= o.InicioVigencia && (o.FimVigencia == null || o.FimVigencia >= filtrosPesquisa.DataVigenciaInicial));
            
            if (filtrosPesquisa.DataVigenciaFinal != DateTime.MinValue)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => o.FimVigencia != null && filtrosPesquisa.DataVigenciaFinal >= o.FimVigencia && (o.InicioVigencia == null || o.InicioVigencia <= filtrosPesquisa.DataVigenciaFinal));

            if (filtrosPesquisa.SomenteVigentes)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => 
                ((o.InicioVigencia != null && o.FimVigencia != null) && (o.InicioVigencia <= DateTime.Now && DateTime.Now <= o.FimVigencia)) ||
                ((o.InicioVigencia != null && o.FimVigencia == null) && o.InicioVigencia <= DateTime.Now)  ||
                ((o.FimVigencia != null && o.InicioVigencia == null) && o.InicioVigencia <= DateTime.Now)
                );

            if (filtrosPesquisa.CodigosModelosVeiculares.Count > 0)
            {
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(o => filtrosPesquisa.CodigosModelosVeiculares.Contains(o.ModeloVeicular.Codigo));
            }

            return consultaConfiguracaoDescargaCliente;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente BuscarPorCodigo(int codigo)
        {
            var consultaConfiguracaoDescargaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>()
                .Where(configuracao => configuracao.Codigo == codigo);

            return consultaConfiguracaoDescargaCliente.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> BuscarPorCodigosSemRegraAprovacao(List<int> codigos)
        {
            var consultaConfiguracaoDescargaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>()
                .Where(configuracao => codigos.Contains(configuracao.Codigo) && configuracao.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao);

            return consultaConfiguracaoDescargaCliente.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaConfiguracaoDescargaCliente = Consultar(filtrosPesquisa);

            return ObterLista(consultaConfiguracaoDescargaCliente, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaCliente filtrosPesquisa)
        {
            var consultaConfiguracaoDescargaCliente = Consultar(filtrosPesquisa);

            return consultaConfiguracaoDescargaCliente.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> BuscarConfiguracoesAtivasCompativeis(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoDescargaClienteCompativel filtrosPesquisa)
        {
            var consultaConfiguracaoDescargaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>()
                   .Where(configuracao => configuracao.Ativo == true);

            if (filtrosPesquisa.CodigoConfiguracaoDescargaClienteDesconsiderar > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.Codigo != filtrosPesquisa.CodigoConfiguracaoDescargaClienteDesconsiderar);

            if (filtrosPesquisa.SomenteComVigenciaInformada)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.InicioVigencia != null || configuracao.FimVigencia != null);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.Filial.Codigo == filtrosPesquisa.CodigoFilial);
            else
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.Filial == null);

            if (filtrosPesquisa.CodigoTipoCarga > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.TipoCarga.Codigo == filtrosPesquisa.CodigoTipoCarga);
            else
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.TipoCarga == null);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.ModeloVeicular.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);
            else
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.ModeloVeicular == null);

            if (filtrosPesquisa.CpfCnpjClientes?.Count > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.Clientes.Any(cliente => filtrosPesquisa.CpfCnpjClientes.Contains(cliente.CPF_CNPJ)));

            if (filtrosPesquisa.CodigosGruposClientes?.Count > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.GrupoPessoas.Any(grupoPessoas => filtrosPesquisa.CodigosGruposClientes.Contains(grupoPessoas.Codigo)));

            if (filtrosPesquisa.CodigosTiposOperacao?.Count > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.TiposOperacoes.Any(tipoOperacao => filtrosPesquisa.CodigosTiposOperacao.Contains(tipoOperacao.Codigo)));

            if (filtrosPesquisa.CodigosTransportadores?.Count > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.Transportadores.Any(transportador => filtrosPesquisa.CodigosTransportadores.Contains(transportador.Codigo)));

            return consultaConfiguracaoDescargaCliente.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> BuscarConfiguracoesValidas(int codigoFilial, int codigoTipoCarga, int codigoModeloVeicularCarga, DateTime? dataValidarVigencia, List<double> cpfCnpjClientes)
        {
            var consultaConfiguracaoDescargaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>()
                   .Where(configuracao =>
                       (configuracao.Filial == null || configuracao.Filial.Codigo == codigoFilial) &&
                       (configuracao.TipoCarga == null || configuracao.TipoCarga.Codigo == codigoTipoCarga) &&
                       (configuracao.ModeloVeicular == null || configuracao.ModeloVeicular.Codigo == codigoModeloVeicularCarga) &&
                       (configuracao.Ativo == true) &&
                       (configuracao.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.Aprovada || configuracao.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao)
                   );

            if (dataValidarVigencia.HasValue)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao =>
                    (configuracao.InicioVigencia == null && configuracao.FimVigencia != null && configuracao.FimVigencia >= dataValidarVigencia) ||
                    (configuracao.InicioVigencia != null && configuracao.FimVigencia == null && configuracao.InicioVigencia <= dataValidarVigencia) ||
                    (configuracao.InicioVigencia != null && configuracao.FimVigencia != null && configuracao.InicioVigencia <= dataValidarVigencia && configuracao.FimVigencia >= dataValidarVigencia)
                );

            if (cpfCnpjClientes?.Count > 0)
                consultaConfiguracaoDescargaCliente = consultaConfiguracaoDescargaCliente.Where(configuracao => configuracao.Clientes.Any(cliente => cpfCnpjClientes.Contains(cliente.CPF_CNPJ)));

            return consultaConfiguracaoDescargaCliente.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pallets.TaxasDescarga> ConsultarRelatorioTaxasDescarga(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTaxasDescarga = new Repositorio.Embarcador.Pallets.ConsultaTaxasDescarga().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaTaxasDescarga.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pallets.TaxasDescarga)));

            return consultaTaxasDescarga.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Pallets.TaxasDescarga>();
        }

        public int ContarConsultaRelatorioTaxasDescarga(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaTaxasDescarga = new Repositorio.Embarcador.Pallets.ConsultaTaxasDescarga().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaTaxasDescarga.SetTimeout(1200).UniqueResult<int>();
        }

        #endregion Métodos Públicos
    }
}
