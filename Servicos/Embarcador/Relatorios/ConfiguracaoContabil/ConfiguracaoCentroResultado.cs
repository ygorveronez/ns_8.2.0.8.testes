using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.ConfiguracaoContabil
{
    public class ConfiguracaoCentroResultado : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado, Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.ConfiguracaoCentroResultado>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado _repositorioConfiguracaoCentroResultado;
        private readonly int _limitePergustasRespostas = 60;

        #endregion

        #region Construtores

        public ConfiguracaoCentroResultado(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConfiguracaoCentroResultado = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.ConfiguracaoCentroResultado> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioConfiguracaoCentroResultado.ConsultarRelatorioConfiguracaoCentroResultado(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioConfiguracaoCentroResultado.ContarConsultaRelatorioConfiguracaoCentroResultado(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/ConfiguracaoContabil/ConfiguracaoCentroResultado";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
            Repositorio.Cliente repositorioRemetente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProduto(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultadoContabilizacao = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultadoEscrituracao = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultadoICMS = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultadoPIS = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultadoCOFINS = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);

            Dominio.Entidades.Empresa transportador = filtrosPesquisa.Transportador > 0 ? repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.Transportador) : null;
            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = filtrosPesquisa.TipoOcorrencia > 0 ? repTipoOcorrencia.BuscarPorCodigo(filtrosPesquisa.TipoOcorrencia) : null;
            Dominio.Entidades.Cliente remetente = filtrosPesquisa.Remetente > 0 ? repositorioRemetente.BuscarPorCPFCNPJ(filtrosPesquisa.Remetente) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.Destinatario > 0 ? repositorioRemetente.BuscarPorCPFCNPJ(filtrosPesquisa.Destinatario) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.Tomador > 0 ? repositorioRemetente.BuscarPorCPFCNPJ(filtrosPesquisa.Tomador) : null;
            Dominio.Entidades.RotaFrete rotaFrete = filtrosPesquisa.RotaFrete > 0 ? repRotaFrete.BuscarPorCodigo(filtrosPesquisa.RotaFrete) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.TipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.TipoOperacao) : null;
            Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProduto = filtrosPesquisa.GrupoProduto > 0 ? repGrupoProduto.BuscarPorCodigo(filtrosPesquisa.GrupoProduto) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoContabilizacao = filtrosPesquisa.CentroResultadoContabilizacao > 0 ? repCentroResultadoContabilizacao.BuscarPorCodigo(filtrosPesquisa.CentroResultadoContabilizacao) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoEscrituracao  = filtrosPesquisa.CentroResultadoEscrituracao > 0 ? repCentroResultadoEscrituracao.BuscarPorCodigo(filtrosPesquisa.CentroResultadoEscrituracao) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoICMS = filtrosPesquisa.CentroResultadoICMS > 0 ? repCentroResultadoICMS.BuscarPorCodigo(filtrosPesquisa.CentroResultadoICMS) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoPIS  = filtrosPesquisa.CentroResultadoPIS > 0 ? repCentroResultadoPIS.BuscarPorCodigo(filtrosPesquisa.CentroResultadoPIS) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoCOFINS = filtrosPesquisa.CentroResultadoCOFINS > 0 ? repCentroResultadoCOFINS.BuscarPorCodigo(filtrosPesquisa.CentroResultadoCOFINS) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", tomador?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOcorrencia", tipoOcorrencia?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", grupoProduto?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RotaFrete", rotaFrete?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultadoContabilizacao", centroResultadoContabilizacao?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultadoEscrituracao", centroResultadoEscrituracao?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultadoICMS", centroResultadoICMS?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultadoPIS", centroResultadoPIS?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultadoCOFINS", centroResultadoCOFINS?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao.HasValue.ObterDescricaoAtivo()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
