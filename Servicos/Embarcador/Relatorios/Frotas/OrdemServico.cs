using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class OrdemServico : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico, Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Frota.OrdemServicoFrota _repositorioMovimentoFrota;

        #endregion

        #region Construtores

        public OrdemServico(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);
        }

        public OrdemServico(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMovimentoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioMovimentoFrota.ConsultarRelatorioOrdemServicoAsync(filtrosPesquisa, parametrosConsulta, propriedadesAgrupamento);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.RelatorioOrdemServico> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMovimentoFrota.ConsultarRelatorioOrdemServico(filtrosPesquisa, parametrosConsulta, propriedadesAgrupamento).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMovimentoFrota.ContarConsultaRelatorioOrdemServico(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frota/OrdemServico";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioOrdemServico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(_unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(_unitOfWork);
            Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServico = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(_unitOfWork);

            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(_unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);
            Repositorio.Embarcador.Frota.GrupoServico repGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);

            List<Dominio.Entidades.Cliente> locaisManutencao = filtrosPesquisa.LocaisManutencao?.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.LocaisManutencao) : new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Veiculo> veiculos = filtrosPesquisa.Veiculos?.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculos) : new List<Dominio.Entidades.Veiculo>();
            List<Dominio.Entidades.Usuario> motoristas = filtrosPesquisa.Motoristas?.Count > 0 ? repUsuario.BuscarMotoristaPorCodigo(filtrosPesquisa.Motoristas) : new List<Dominio.Entidades.Usuario>();
            List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota> servicos = filtrosPesquisa.Servicos?.Count > 0 ? repServico.BuscarPorCodigo(filtrosPesquisa.Servicos) : new List<Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota>();
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo> tipos = filtrosPesquisa.Tipos?.Count > 0 ? repTipoOrdemServico.BuscarPorCodigo(filtrosPesquisa.Tipos) : new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo>();
            List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> equipamentos = filtrosPesquisa.Equipamentos?.Count > 0 ? repEquipamento.BuscarPorCodigo(filtrosPesquisa.Equipamentos) : new List<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();

            Dominio.Entidades.MarcaVeiculo marcaVeiculo = filtrosPesquisa.MarcaVeiculo > 0 ? repMarcaVeiculo.BuscarPorCodigo(filtrosPesquisa.MarcaVeiculo, 0) : new Dominio.Entidades.MarcaVeiculo();
            Dominio.Entidades.ModeloVeiculo modeloVeiculo = filtrosPesquisa.ModeloVeiculo > 0 ? repModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.ModeloVeiculo, 0) : new Dominio.Entidades.ModeloVeiculo();
            List<Dominio.Entidades.Embarcador.Frota.GrupoServico> grupoServicos = filtrosPesquisa.GrupoServicos?.Count > 0 ? repGrupoServico.BuscarPorCodigos(filtrosPesquisa.GrupoServicos) : new List<Dominio.Entidades.Embarcador.Frota.GrupoServico>();
            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centroResultados = filtrosPesquisa.CentroResultados?.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.GrupoServicos) : new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo> segmentoVeiculos = filtrosPesquisa.Segmentos?.Count > 0 ? repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.Segmentos) : new List<Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo>();
            Dominio.Entidades.Usuario usuarioOperadorLancamento = filtrosPesquisa.OperadorLancamentoDocumento > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.OperadorLancamentoDocumento) : new Dominio.Entidades.Usuario();
            Dominio.Entidades.Usuario usuarioOperadorFinalizou = filtrosPesquisa.OperadorFinalizouDocumento > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.OperadorFinalizouDocumento) : new Dominio.Entidades.Usuario();
            List<Dominio.Entidades.Usuario> mecanicos = filtrosPesquisa.Mecanicos?.Count > 0 ? repUsuario.BuscarPorCodigos(filtrosPesquisa.Mecanicos) : new List<Dominio.Entidades.Usuario>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtrosPesquisa.NumeroInicial, filtrosPesquisa.NumeroFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalManutencao", string.Join(", ", locaisManutencao.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", string.Join(", ", veiculos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", string.Join(", ", motoristas.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", string.Join(", ", servicos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", string.Join(", ", tipos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoManutencao", string.Join(", ", filtrosPesquisa.TipoManutencao.Select(o => o.ObterDescricao()))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", string.Join(", ", filtrosPesquisa.Situacao.Select(o => o.ObterDescricao())))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Equipamento", string.Join(", ", equipamentos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOrdemServico", filtrosPesquisa.TipoOrdemServico?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MarcaVeiculo", marcaVeiculo.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoServico", string.Join(", ", grupoServicos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", string.Join(", ", centroResultados.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", string.Join(", ", segmentoVeiculos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("OperadorLancamento", usuarioOperadorLancamento?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("OperadorFinalizou", usuarioOperadorFinalizou?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataHoraInclusao", filtrosPesquisa.DataInicialInclusao, filtrosPesquisa.DataFinalInclusao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Mecanicos", string.Join(", ", mecanicos.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Prioridade", filtrosPesquisa.Prioridade.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLimiteExecucao", filtrosPesquisa.DataInicialLimiteExecucao, filtrosPesquisa.DataFinalLimiteExecucao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLiberacao", filtrosPesquisa.DataLiberacaoInicio, filtrosPesquisa.DataLiberacaoFim));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFechamento", filtrosPesquisa.DataFechamentoInicio, filtrosPesquisa.DataFechamentoFim));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataReabertura", filtrosPesquisa.DataReaberturaInicio, filtrosPesquisa.DataReaberturaFim));
            
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