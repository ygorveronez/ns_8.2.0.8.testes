using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class Cheque : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio, Dominio.Relatorios.Embarcador.DataSource.Financeiros.Cheque>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.Cheque _repositorioCheque;

        #endregion

        #region Construtores

        public Cheque(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCheque = new Repositorio.Embarcador.Financeiro.Cheque(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Cheque> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCheque.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCheque.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/Cheque";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            if (filtrosPesquisa.DataCompensacaoInicio.HasValue || filtrosPesquisa.DataCompensacaoLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataCompensacaoInicio.HasValue ? $"{filtrosPesquisa.DataCompensacaoInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataCompensacaoLimite.HasValue ? $"até {filtrosPesquisa.DataCompensacaoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoCompensacao", periodo, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoCompensacao", false));

            if (filtrosPesquisa.DataTransacaoInicio.HasValue || filtrosPesquisa.DataTransacaoLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataTransacaoInicio.HasValue ? $"{filtrosPesquisa.DataTransacaoInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataTransacaoLimite.HasValue ? $"até {filtrosPesquisa.DataTransacaoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoTransacao", periodo, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoTransacao", false));

            if (filtrosPesquisa.DataVencimentoInicio.HasValue || filtrosPesquisa.DataVencimentoLimite.HasValue)
            {
                string periodo = $"{(filtrosPesquisa.DataVencimentoInicio.HasValue ? $"{filtrosPesquisa.DataVencimentoInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataVencimentoLimite.HasValue ? $"até {filtrosPesquisa.DataVencimentoLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoVencimento", periodo, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoVencimento", false));

            if ((filtrosPesquisa.ValorInicio > 0m) || (filtrosPesquisa.ValorLimite > 0m))
            {
                string intervaloValor = $"{((filtrosPesquisa.ValorInicio > 0m) ? $"{filtrosPesquisa.ValorInicio.ToString("n2")} " : "")}{((filtrosPesquisa.ValorLimite > 0m) ? $"até {filtrosPesquisa.ValorLimite.ToString("n2")}" : "")}";

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("IntervaloValor", intervaloValor, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("IntervaloValor", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCheque))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCheque", filtrosPesquisa.NumeroCheque, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCheque", false));

            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
            {
                Repositorio.Cliente repositorioPessoa = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente pessoa = repositorioPessoa.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjPessoa);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

            if (filtrosPesquisa.Status.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", filtrosPesquisa.Status.Value.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", false));

            if (filtrosPesquisa.Tipo.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", filtrosPesquisa.Tipo.Value.ObterDescricao(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", false));

            if (filtrosPesquisa.Banco > 0)
            {
                Repositorio.Banco repBanco = new Repositorio.Banco(_unitOfWork);
                Dominio.Entidades.Banco banco = repBanco.BuscarPorCodigo(filtrosPesquisa.Banco);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Banco", banco.Descricao, true));
            }
            else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Banco", false));
            }

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataCadastroFormatada")
                return "DataCadastro";

            if (propriedadeOrdenarOuAgrupar == "DataTransacaoFormatada")
                return "DataTransacao";

            if (propriedadeOrdenarOuAgrupar == "DataVencimentoFormatada")
                return "DataVencimento";

            if (propriedadeOrdenarOuAgrupar == "DataCompensacaoFormatada")
                return "DataCompensacao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}