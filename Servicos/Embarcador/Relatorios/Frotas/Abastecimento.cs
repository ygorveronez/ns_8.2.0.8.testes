using System.Collections.Generic;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class Abastecimento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento, Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento>
    {
        #region Atributos

        private readonly Repositorio.Abastecimento _repositorioAbastecimento;

        #endregion

        #region Construtores

        public Abastecimento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
        }

        public Abastecimento(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAbastecimento = new Repositorio.Abastecimento(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioAbastecimento.RelatorioAbastecimentoAsync(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Públicos Protegidos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAbastecimento.RelatorioAbastecimento(filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAbastecimento.ContarRelatorioAbastecimento(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/Abastecimento";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Pais repPais = new Repositorio.Pais(_unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamento = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(_unitOfWork);

            List<Dominio.Entidades.Produto> produtos = filtrosPesquisa.CodigosProdutos.Count > 0 ? repProduto.BuscarPorCodigo(filtrosPesquisa.CodigosProdutos) : new List<Dominio.Entidades.Produto>();
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Cliente proprietario = filtrosPesquisa.CodigoProprietario > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoProprietario) : null;
            Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmento = filtrosPesquisa.CodigoSegmento > 0 ? repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoSegmento) : null;
            Dominio.Entidades.Usuario motorista = filtrosPesquisa.CodigoMotorista > 0 ? repMotorista.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Cliente fornecedor = filtrosPesquisa.Fornecedor > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Fornecedor) : null;
            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = filtrosPesquisa.CodigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoEquipamento) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = filtrosPesquisa.CodigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(filtrosPesquisa.CodigoCentroResultado) : null;
            Dominio.Entidades.Estado estado = filtrosPesquisa.UFFornecedor != "0" ? repEstado.BuscarPorSigla(filtrosPesquisa.UFFornecedor) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = filtrosPesquisa.ModeloVeicularCarga > 0 ? repModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.ModeloVeicularCarga) : null;
            List<Dominio.Entidades.Pais> paises = repPais.BuscarPorCodigos(filtrosPesquisa.Paises);
            Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = repLocalArmazenamento.BuscarPorCodigo(filtrosPesquisa.CodigoLocalArmazenamento);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCRT", filtrosPesquisa.DataBaseCRTInicial, filtrosPesquisa.DataBaseCRTFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", string.Join(", ", from obj in produtos select obj.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));

            if (filtrosPesquisa.StatusAbastecimento == "I")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Inconsistente", true));
            else if (filtrosPesquisa.StatusAbastecimento == "F")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Fechado", true));
            else if (filtrosPesquisa.StatusAbastecimento == "A")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Aberto", true));
            else if (filtrosPesquisa.StatusAbastecimento == "G")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Agrupado", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Todos", true));

            if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 1)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoAbastecimentoInternoExterno", "Interno", true));
            else if (filtrosPesquisa.TipoAbastecimentoInternoExterno == 2)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoAbastecimentoInternoExterno", "Externo", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoAbastecimentoInternoExterno", false));

            if (filtrosPesquisa.TipoPropriedade == "P")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPropriedade", "Próprio", true));
            else if (filtrosPesquisa.TipoPropriedade == "T")
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPropriedade", "Terceiro", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoPropriedade", false));

            if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.EmAcerto)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoAcerto", "Em Acerto", true));
            else if (filtrosPesquisa.SituacaoAcerto == Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem.NaoConstaEmAcerto)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoAcerto", "Não Consta em Acerto", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoAcerto", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Proprietario", proprietario?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", segmento?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", fornecedor?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Equipamento", equipamento?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoAbastecimento", filtrosPesquisa.TipoAbastecimento.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoRecebimento", string.Join(", ", filtrosPesquisa.TiposRecebimento.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", centroResultado?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UFFornecedor", estado?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicularCarga", modeloVeicularCarga?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pais", paises.Select(o => o.Nome)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Moeda", filtrosPesquisa.Moedas.Select(o => o.ObterDescricao())));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalArmazenamento", localArmazenamento?.Descricao));
            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "MoedaDescricao")
                return "Moeda";

            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "DescricaoTipoRecebimento")
                return "TipoRecebimento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}