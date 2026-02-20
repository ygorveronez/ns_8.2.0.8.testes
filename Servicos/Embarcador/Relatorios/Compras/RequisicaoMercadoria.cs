using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Compras
{
    public class RequisicaoMercadoria : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria, Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioRequisicaoMercadoria>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Compras.RequisicaoMercadoria _repositorioRequisicaoMercadoria;

        #endregion

        #region Construtores

        public RequisicaoMercadoria(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioRequisicaoMercadoria> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioRequisicaoMercadoria.ConsultarRelatorioRequisicaoMercadoria(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioRequisicaoMercadoria.ContarConsultaRelatorioRequisicaoMercadoria(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Compras/RequisicaoMercadoria";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Embarcador.Compras.MotivoCompra repMotivoCompra = new Repositorio.Embarcador.Compras.MotivoCompra(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProdutoTMS = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Setor repositorioSetor = new Repositorio.Setor(_unitOfWork);

            Dominio.Entidades.Setor setor = filtrosPesquisa.SetorFuncionario > 0 ? repositorioSetor.BuscarPorCodigo(filtrosPesquisa.SetorFuncionario) : null;

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

            if (filtrosPesquisa.NumeroInicial > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", false));

            if (filtrosPesquisa.NumeroFinal > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", false));

            if ((int)filtrosPesquisa.Tipo > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoriaHelper.ObterDescricao(filtrosPesquisa.Tipo), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", false));

            if (filtrosPesquisa.Situacao != null && filtrosPesquisa.Situacao.Count > 0)
            {
                List<string> descricaoSituacao = new List<string>();
                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria situacaoRequisicaoMercadoria in filtrosPesquisa.Situacao)
                    descricaoSituacao.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoriaHelper.ObterDescricao(situacaoRequisicaoMercadoria));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", descricaoSituacao), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

            if (filtrosPesquisa.Colaborador > 0)
            {
                Dominio.Entidades.Usuario _usuario = repUsuario.BuscarPorCodigo(filtrosPesquisa.Colaborador);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Colaborador", _usuario.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Colaborador", false));

            if (filtrosPesquisa.FuncionarioRequisitado > 0)
            {
                Dominio.Entidades.Usuario _usuario = repUsuario.BuscarPorCodigo(filtrosPesquisa.FuncionarioRequisitado);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioRequisitado", _usuario.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioRequisitado", false));

            if (filtrosPesquisa.Motivo > 0)
            {
                Dominio.Entidades.Embarcador.Compras.MotivoCompra _motivoCompra = repMotivoCompra.BuscarPorCodigo(filtrosPesquisa.Motivo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motivo", _motivoCompra.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motivo", false));

            if (filtrosPesquisa.Produto > 0)
            {
                Dominio.Entidades.Produto _produto = repProduto.BuscarPorCodigo(filtrosPesquisa.Produto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", _produto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

            if (filtrosPesquisa.GrupoProduto > 0)
            {
                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS _grupoProdutoTMS = repGrupoProdutoTMS.BuscarPorCodigo(filtrosPesquisa.GrupoProduto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", _grupoProdutoTMS.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SetorFuncionario", setor?.Descricao ?? string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipo")
                return "Tipo";

            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}