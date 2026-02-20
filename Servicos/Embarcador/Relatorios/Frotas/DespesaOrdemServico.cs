using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class DespesaOrdemServico : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico, Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.OrdemServicoFrota _repositorioMovimentoFrota;

        #endregion

        #region Construtores

        public DespesaOrdemServico(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);
        }

        public DespesaOrdemServico(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioMovimentoFrota.ConsultarRelatorioDespesaOrdemServicoAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMovimentoFrota.ConsultarRelatorioDespesaOrdemServico(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMovimentoFrota.ContarConsultaRelatorioDespesaOrdemServico(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/DespesaOrdemServico";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);
            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(_unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(_unitOfWork);
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServicoVeiculoFrota = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);



            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo> tipoOrdemServico = filtrosPesquisa.TipoOrdemServico.Count > 0 ? repTipoOrdemServico.BuscarPorCodigo(filtrosPesquisa.TipoOrdemServico) : null;
            Dominio.Entidades.Veiculo _veiculo = filtrosPesquisa.Veiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo) : null;
            List<Dominio.Entidades.ModeloVeiculo> _modeloVeiculo = filtrosPesquisa.ModeloVeiculo.Count > 0 ? repModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.ModeloVeiculo) : null;
            Dominio.Entidades.MarcaVeiculo _marcaVeiculo = filtrosPesquisa.MarcaVeiculo > 0 ? repMarcaVeiculo.BuscarPorCodigo(filtrosPesquisa.MarcaVeiculo, 0) : null;
            Dominio.Entidades.Cliente _localManutencao = filtrosPesquisa.LocalManutencao > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.LocalManutencao) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.Empresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.Empresa) : null;
            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> _servicoVeiculo = filtrosPesquisa.Servico.Count > 0 ? repServicoVeiculoFrota.BuscarPorCodigo(filtrosPesquisa.Servico) : null;
            List<Dominio.Entidades.Produto> _produto = filtrosPesquisa.Produto.Count > 0 ? repProduto.BuscarPorCodigo(filtrosPesquisa.Produto.ToArray()) : null;
            List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> _equipamento = filtrosPesquisa.Equipamento.Count > 0 ? repEquipamento.BuscarPorCodigo(filtrosPesquisa.Equipamento) : null;
            List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS> grupoProduto = filtrosPesquisa.GrupoProduto.Count > 0 ? repGrupoProduto.BuscarPorCodigos(filtrosPesquisa.GrupoProduto) : null;

            string data = "";
            data += filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") + " " : "";
            data += filtrosPesquisa.DataFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : "";

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtrosPesquisa.NumeroOS > 0 ? filtrosPesquisa.NumeroOS.ToString() : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", _veiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", _modeloVeiculo != null && _modeloVeiculo.Count > 0 ? string.Join(", ", _modeloVeiculo.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MarcaVeiculo", _marcaVeiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalManutencao", _localManutencao?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", tipoOrdemServico != null && tipoOrdemServico.Count > 0 ? string.Join(", ", tipoOrdemServico.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", filtrosPesquisa.Situacoes?.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", _servicoVeiculo != null && _servicoVeiculo.Count > 0 ? string.Join(", ", _servicoVeiculo.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", _produto != null && _produto.Count > 0 ? string.Join(", ", _produto.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", grupoProduto != null && grupoProduto.Count > 0 ? string.Join(", ", grupoProduto.Select(o => o.Descricao).ToList()) : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Equipamento", _equipamento != null && _equipamento.Count > 0 ? string.Join(", ", _equipamento.Select(o => o.Descricao).ToList()) : null));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}