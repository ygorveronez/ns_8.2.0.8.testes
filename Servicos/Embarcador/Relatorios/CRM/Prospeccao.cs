using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.CRM
{
    public class Prospeccao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao, Dominio.Relatorios.Embarcador.DataSource.CRM.Prospeccao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.CRM.Prospeccao _repositorioProspeccao;

        #endregion

        #region Construtores

        public Prospeccao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioProspeccao = new Repositorio.Embarcador.CRM.Prospeccao(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CRM.Prospeccao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioProspeccao.ConsultarRelatorioProspeccao(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioProspeccao.ContarConsultaRelatorioProspeccao(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CRM/Prospeccao";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioProspeccao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.CRM.ClienteProspect repClienteProspect = new Repositorio.Embarcador.CRM.ClienteProspect(_unitOfWork);
            Repositorio.Embarcador.CRM.ProdutoProspect repProdutoProspect = new Repositorio.Embarcador.CRM.ProdutoProspect(_unitOfWork);
            Repositorio.Embarcador.CRM.OrigemContatoClienteProspect repOrigemContatoClienteProspect = new Repositorio.Embarcador.CRM.OrigemContatoClienteProspect(_unitOfWork);

            Dominio.Entidades.Usuario usuarioFiltro = filtrosPesquisa.CodigoUsuario > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoUsuario) : null;
            Dominio.Entidades.Localidade localidadeFiltro = filtrosPesquisa.CodigoCidade > 0 ? repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoCidade) : null;
            Dominio.Entidades.Embarcador.CRM.ClienteProspect clienteFiltro = filtrosPesquisa.CodigoCliente > 0 ? repClienteProspect.BuscarPorCodigo(filtrosPesquisa.CodigoCliente, filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Embarcador.CRM.ProdutoProspect produtoFiltro = filtrosPesquisa.CodigoProduto > 0 ? repProdutoProspect.BuscarPorCodigo(filtrosPesquisa.CodigoProduto) : null;
            Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect origemFiltro = filtrosPesquisa.CodigoOrigemContato > 0 ? repOrigemContatoClienteProspect.BuscarPorCodigo(filtrosPesquisa.CodigoOrigemContato) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoInicial", filtrosPesquisa.DataLancamentoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoFinal", filtrosPesquisa.DataLancamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataRetornoInicial", filtrosPesquisa.DataRetornoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataRetornoFinal", filtrosPesquisa.DataRetornoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Usuario", usuarioFiltro?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Localidade", localidadeFiltro?.DescricaoCidadeEstado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cliente", clienteFiltro?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", produtoFiltro?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("OrigemContato", origemFiltro?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CNPJ", filtrosPesquisa.CNPJ));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContato", filtrosPesquisa.TipoContato?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Satisfacao", filtrosPesquisa.Satisfacao?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Faturado", filtrosPesquisa.Faturado.HasValue ? filtrosPesquisa.Faturado.Value ? "Sim" : "Não" : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
