using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class ConferenciaFiscal : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConferenciaFiscal, Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConferenciaFiscal>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repConferenciaFiscal;

        #endregion

        #region Construtores

        public ConferenciaFiscal(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repConferenciaFiscal = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConferenciaFiscal> ConsultarRegistros(FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repConferenciaFiscal.ConsultarRelatorioConferenciaFiscal(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repConferenciaFiscal.ContarConsultaRelatorioConferenciaFiscal(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/ConferenciaFiscal";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioConferenciaFiscal filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.Veiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = filtrosPesquisa.ModeloVeicular > 0 ? repModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.ModeloVeicular) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.TipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.TipoOperacao) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.Empresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.Empresa) : null;

            List<string> DescricaoTerceiros = new List<string>();
            string terceiros = string.Empty;

            if (filtrosPesquisa.CpfCnpjTerceiros.Count > 0)
            {
                foreach (var cpfCnpjTerceiro in filtrosPesquisa.CpfCnpjTerceiros)
                {
                    Dominio.Entidades.Cliente terceiro = cpfCnpjTerceiro > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTerceiro) : null;

                    if (terceiro != null)
                        DescricaoTerceiros.Add(terceiro.Descricao);
                }

                terceiros = string.Join(",", DescricaoTerceiros);
            }

            parametros.Add(new Parametro("Terceiro", terceiros));
            parametros.Add(new Parametro("DataEmissaoContratoInicial", filtrosPesquisa.DataEmissaoContratoInicial));
            parametros.Add(new Parametro("DataEmissaoContratoFinal", filtrosPesquisa.DataEmissaoContratoFinal));
            parametros.Add(new Parametro("NumeroContrato", filtrosPesquisa.NumeroContrato));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("NumeroCTe", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao?.Select(o => o.ObterDescricao())));
            parametros.Add(new Parametro("ModeloVeicular", modeloVeicularCarga?.Descricao));
            parametros.Add(new Parametro("DataAprovacaoInicial", filtrosPesquisa.DataAprovacaoInicial));
            parametros.Add(new Parametro("DataAprovacaoFinal", filtrosPesquisa.DataAprovacaoFinal));
            parametros.Add(new Parametro("DataEncerramentoInicial", filtrosPesquisa.DataEncerramentoInicial));
            parametros.Add(new Parametro("DataEncerramentoFinal", filtrosPesquisa.DataEncerramentoFinal));
            parametros.Add(new Parametro("DataAberturaCIOT", filtrosPesquisa.DataAberturaCIOTInicial, filtrosPesquisa.DataAberturaCIOTFinal));
            parametros.Add(new Parametro("DataEncerramentoCIOT", filtrosPesquisa.DataEncerramentoCIOTInicial, filtrosPesquisa.DataEncerramentoCIOTFinal));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("Empresa", empresa?.Descricao));

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
