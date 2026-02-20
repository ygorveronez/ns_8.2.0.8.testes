using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Servicos.Embarcador.Relatorios.Pallets
{
    public class TaxasDescarga : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga, Dominio.Relatorios.Embarcador.DataSource.Pallets.TaxasDescarga>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente _repositorioConfiguracaoDescargaCliente;

        #endregion

        #region Construtores

        public TaxasDescarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConfiguracaoDescargaCliente = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pallets.TaxasDescarga> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioConfiguracaoDescargaCliente.ConsultarRelatorioTaxasDescarga(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioConfiguracaoDescargaCliente.ContarConsultaRelatorioTaxasDescarga(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pallets/TaxasDescarga";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoCliente = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);



            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga  modeloVeicular = filtrosPesquisa.CodigoModeloVeicular > 0 ? repModeloVeicular.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeicular) : null;
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = filtrosPesquisa.CodigoTipoCarga > 0 ? repTipoCarga.BuscarPorCodigo(filtrosPesquisa.CodigoTipoCarga) : null;
            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CpfCnpjCliente > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjCliente) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoCliente = filtrosPesquisa.CodigoGrupoCliente > 0 ? repGrupoCliente.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoCliente) : null;
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tipoOperacao = filtrosPesquisa.CodigoTipoOperacao.Count > 0 ? repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigoTipoOperacao) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicular", modeloVeicular?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tipoCarga?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", filtrosPesquisa?.Status?.ObterDescricaoAtivo()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa?.CodigoSituacao?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicioVigencia", filtrosPesquisa?.DataInicioVigencia.ToString("dd/MM/yyyy")));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFimVigencia", filtrosPesquisa?.DataFimVigencia.ToString("dd/MM/yyyy")));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cliente", cliente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoCliente", grupoCliente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", string.Join(", ", tipoOperacao?.Select(o => o.Descricao))));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            if (propriedadeOrdenarOuAgrupar == "DataHoraInclusaoFormatada")
                return "DataHoraInclusao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}