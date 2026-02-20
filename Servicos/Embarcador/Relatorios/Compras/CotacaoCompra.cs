using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Compras
{
    public class CotacaoCompra : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra, Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompra>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Compras.Cotacao _repositorioCotacaoCompra;

        #endregion

        #region Construtores

        public CotacaoCompra(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCotacaoCompra = new Repositorio.Embarcador.Compras.Cotacao(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompra> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCotacaoCompra.ConsultarRelatorioCotacaoCompra(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCotacaoCompra.ContarConsultaRelatorioCotacaoCompra(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Compras/CotacaoCompra";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

            if (filtrosPesquisa.CodigosProduto != null && filtrosPesquisa.CodigosProduto.Count > 0)
            {
                List<Dominio.Entidades.Produto> produtos = filtrosPesquisa.CodigosProduto.Count > 0 ? repProduto.BuscarPorCodigo(filtrosPesquisa.CodigosProduto.ToArray()) : new List<Dominio.Entidades.Produto>();
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", string.Join(", ", from obj in produtos select obj.Descricao), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

            if (filtrosPesquisa.CodigosFornecedor != null && filtrosPesquisa.CodigosFornecedor.Count > 0)
            {
                List<Dominio.Entidades.Cliente> fornecedores = filtrosPesquisa.CodigosFornecedor.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CodigosFornecedor) : new List<Dominio.Entidades.Cliente>();
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", string.Join(", ", from obj in fornecedores select obj.Nome), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}