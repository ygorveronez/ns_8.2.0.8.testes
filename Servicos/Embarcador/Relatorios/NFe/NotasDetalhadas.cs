using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.NFe
{
    public class NotasDetalhadas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas, Dominio.Relatorios.Embarcador.DataSource.NFe.NotasDetalhadas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.NotaFiscal _repositorioNotasDetalhadas;

        #endregion

        #region Construtores

        public NotasDetalhadas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioNotasDetalhadas = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasDetalhadas> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioNotasDetalhadas.RelatorioNotasDetalhadas(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, true).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioNotasDetalhadas.ContarRelatorioNotasDetalhadas(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NFe/NotasDetalhadas";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasDetalhadas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoa) : null;
            List<string> grupoProdutos = filtrosPesquisa.CodigosGrupoProduto.Count > 0 ? repGrupoProduto.BuscarDescricaoPorCodigo(filtrosPesquisa.CodigosGrupoProduto) : null;

            parametros.Add(new Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial));
            parametros.Add(new Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal));
            parametros.Add(new Parametro("Serie", filtrosPesquisa.Serie));

            if (filtrosPesquisa.CodigosNaturezaOperacao?.Count > 0)
            {
                List<Dominio.Entidades.NaturezaDaOperacao> naturezas = repNaturezaDaOperacao.BuscarPorIds(filtrosPesquisa.CodigosNaturezaOperacao);
                parametros.Add(new Parametro("Natureza", string.Join(", ", naturezas.Select(o => o.Descricao)), true));
            }
            else
                parametros.Add(new Parametro("Natureza", false));

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto);
                parametros.Add(new Parametro("Produto", "(" + produto.Codigo.ToString() + ") " + produto.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Produto", false));

            if (filtrosPesquisa.CodigoServico > 0)
            {
                Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = repServico.BuscarPorCodigo(filtrosPesquisa.CodigoServico);
                parametros.Add(new Parametro("Servico", "(" + servico.Codigo.ToString() + ") " + servico.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Servico", false));

            //if (codigoModelo > 0)
            if (filtrosPesquisa.CodigosModeloDocumentoFiscal != null && filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0)
            {
                List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumentosFiscais = filtrosPesquisa.CodigosModeloDocumentoFiscal.Count > 0 ? repModelo.BuscarPorCodigo(filtrosPesquisa.CodigosModeloDocumentoFiscal.ToArray()) : new List<Dominio.Entidades.ModeloDocumentoFiscal>();
                parametros.Add(new Parametro("Modelo", string.Join(", ", from obj in modelosDocumentosFiscais select obj.Descricao), true));
            }
            else
                parametros.Add(new Parametro("Modelo", false));

            if (filtrosPesquisa.CnpjPessoa > 0)
            {
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoa);
                parametros.Add(new Parametro("Cliente", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
            }
            else
                parametros.Add(new Parametro("Cliente", false));

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("Tipo", filtrosPesquisa.TipoMovimento.ObterDescricao()));
            parametros.Add(new Parametro("Chave", filtrosPesquisa.Chave));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                parametros.Add(new Parametro("Filial", repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa).RazaoSocial, true));
            else
                parametros.Add(new Parametro("Filial", false));

            if (filtrosPesquisa.CodigoVeiculo > 0)
                parametros.Add(new Parametro("Veiculo", repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo).Placa, true));
            else
                parametros.Add(new Parametro("Veiculo", false));

            parametros.Add(new Parametro("StatusNota", filtrosPesquisa.StatusNotaEntrada.ObterDescricao()));
            parametros.Add(new Parametro("SituacaoFinanceiraNota", filtrosPesquisa.SituacaoFinanceiraNotaEntrada.ObterDescricao()));
            parametros.Add(new Parametro("GrupoProduto", grupoProdutos));

            if (!string.IsNullOrEmpty(filtrosPesquisa.EstadoEmitente) && filtrosPesquisa.EstadoEmitente != "0")
                parametros.Add(new Parametro("EstadoEmitente", filtrosPesquisa.EstadoEmitente, true));
            else
                parametros.Add(new Parametro("EstadoEmitente", false));

            if (filtrosPesquisa.CodigoSegmento > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmento = repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoSegmento);
                parametros.Add(new Parametro("Segmento", segmento.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Segmento", false));

            parametros.Add(new Parametro("DataEntrada", filtrosPesquisa.DataEntradaInicial, filtrosPesquisa.DataEntradaFinal));

            if (filtrosPesquisa.CodigosTipoMovimento != null && filtrosPesquisa.CodigosTipoMovimento.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> tiposMovimentos = filtrosPesquisa.CodigosTipoMovimento.Count > 0 ? repTipoMovimento.BuscarPorCodigo(filtrosPesquisa.CodigosTipoMovimento.ToArray()) : new List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
                parametros.Add(new Parametro("TipoMovimento", string.Join(", ", from obj in tiposMovimentos select obj.Descricao), true));
            }
            else
                parametros.Add(new Parametro("TipoMovimento", false));

            if (filtrosPesquisa.CodigoEquipamento > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = repEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoEquipamento);
                parametros.Add(new Parametro("Equipamento", equipamento.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Equipamento", false));

            parametros.Add(new Parametro("GrupoPessoa", grupoPessoas?.Descricao));
            parametros.Add(new Parametro("SomenteDiferencaValorTabelaFornecedor", filtrosPesquisa.NotasComDiferencaDeValorTabelaFornecedor ? "Sim" : ""));
            parametros.Add(new Parametro("DataFinalizacao", filtrosPesquisa.DataFinalizacaoInicial, filtrosPesquisa.DataFinalizacaoFinal));

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