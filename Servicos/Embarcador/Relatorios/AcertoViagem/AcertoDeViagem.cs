using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.AcertoViagem
{
    public class AcertoDeViagem : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem, Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AcertoDeViagem>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Acerto.AcertoViagem _repositorioAcertoViagem;

        #endregion

        #region Construtores

        public AcertoDeViagem(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList() metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.AcertoDeViagem> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAcertoViagem.RelatorioAcertoDeViagem(filtrosPesquisa.TipoMotorista, filtrosPesquisa.StatusMotorista, filtrosPesquisa.UltimoAcerto, filtrosPesquisa.Motorista, filtrosPesquisa.Segmento, filtrosPesquisa.VeiculoTracao, filtrosPesquisa.VeiculoReboque, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.DataInicialFechamento, filtrosPesquisa.DataFinalFechamento, filtrosPesquisa.AcertoViagem, filtrosPesquisa.Situacao, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAcertoViagem.ContarAcertoDeViagem(filtrosPesquisa.TipoMotorista, filtrosPesquisa.StatusMotorista, filtrosPesquisa.UltimoAcerto, filtrosPesquisa.Motorista, filtrosPesquisa.Segmento, filtrosPesquisa.VeiculoTracao, filtrosPesquisa.VeiculoReboque, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.DataInicialFechamento, filtrosPesquisa.DataFinalFechamento, filtrosPesquisa.AcertoViagem, filtrosPesquisa.Situacao);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/AcertoViagem/AcertoDeViagem";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Acertos.FiltroPesquisaRelatorioAcertoDeViagem filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAcerto", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAcerto", false));

            if (filtrosPesquisa.DataInicialFechamento != DateTime.MinValue || filtrosPesquisa.DataFinalFechamento != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicialFechamento != DateTime.MinValue ? filtrosPesquisa.DataInicialFechamento.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinalFechamento != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinalFechamento.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFechamento", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFechamento", false));

            if ((int)filtrosPesquisa.Situacao > 0)
            {
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Cancelado", true));
                else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Em Andamento", true));
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Fechado", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Todos", true));

            if (filtrosPesquisa.AcertoViagem > 0)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto = repAcertoViagem.BuscarPorCodigo(filtrosPesquisa.AcertoViagem);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcertoViagem", acerto.Numero.ToString(), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcertoViagem", false));

            if (filtrosPesquisa.Motorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repFuncionario.BuscarPorCodigo(filtrosPesquisa.Motorista);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", "(" + motorista.CPF_Formatado + ") " + motorista.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

            if (filtrosPesquisa.Segmento > 0)
            {
                Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmento = repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.Segmento);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", segmento.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Segmento", false));

            if (filtrosPesquisa.VeiculoTracao > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.VeiculoTracao);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoTracao", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoTracao", false));

            if (filtrosPesquisa.VeiculoReboque > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.VeiculoReboque);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoReboque", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VeiculoReboque", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            if (filtrosPesquisa.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMotorista", "Próprio", true));
            else if (filtrosPesquisa.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMotorista", "Terceiro", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoMotorista", "Todos", true));

            if (filtrosPesquisa.StatusMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusMotorista", "Ativos", true));
            else if (filtrosPesquisa.StatusMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusMotorista", "Inativos", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusMotorista", "Todos", true));

            if (filtrosPesquisa.UltimoAcerto)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UltimosAcertos", "Sim", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("UltimosAcertos", "Não", true));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}