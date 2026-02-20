using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Veiculos
{
    public class Manutencao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao, Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS _repositorioMovimentoVeiculos;

        #endregion

        #region Construtores

        public Manutencao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoVeiculos = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork);
        }

        public Manutencao(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoVeiculos = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioMovimentoVeiculos.RelatorioManutencaoAsync(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Manutencao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMovimentoVeiculos.RelatorioManutencao(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMovimentoVeiculos.ContarManutencao(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Veiculos/Manutencao";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = filtrosPesquisa.CodigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(filtrosPesquisa.CodigoTipoMovimento) : null;
            List<Dominio.Entidades.NaturezaDaOperacao> naturezaOperacao = filtrosPesquisa.CodigosNaturezaOperacao.Count > 0 ? repNaturezaDaOperacao.BuscarPorIds(filtrosPesquisa.CodigosNaturezaOperacao.ToArray()) : new List<Dominio.Entidades.NaturezaDaOperacao>();
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> segmentos = filtrosPesquisa.CodigosSegmento.Count > 0 ? repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosSegmento.ToArray()) : new List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();
            List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> equipamentos = filtrosPesquisa.CodigosEquipamento.Count > 0 ? repEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigosEquipamento.ToArray()) : new List<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Cliente fornecedor = filtrosPesquisa.CnpjCpfFornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjCpfFornecedor) : null;
            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centrosResultado = filtrosPesquisa.CentrosResultado.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.CentrosResultado) : new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            List<string> produtos = filtrosPesquisa.Produtos.Count > 0 ? repProduto.BuscarDescricaoPorCodigo(filtrosPesquisa.Produtos) : new List<string>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));

            if (filtrosPesquisa.CodigoPlacas != null && filtrosPesquisa.CodigoPlacas.Count > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculos", "Multiplas placas", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculos", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMovimento", tipoMovimento?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NaturezaOperacao", string.Join(", ", from obj in naturezaOperacao select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", string.Join(", ", from obj in segmentos select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Equipamento", string.Join(", ", from obj in equipamentos select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", fornecedor?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", string.Join(", ", produtos)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirApenasComVeiculoOuEquipamento", filtrosPesquisa.ExibirApenasComVeiculoOuEquipamento == true ? "Sim" : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", string.Join(", ", from obj in centrosResultado select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoLancDocEntrada", filtrosPesquisa.SituacaoLancDocEntrada?.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoLancDocEntrada")
                return "SituacaoLancDocEntrada";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}