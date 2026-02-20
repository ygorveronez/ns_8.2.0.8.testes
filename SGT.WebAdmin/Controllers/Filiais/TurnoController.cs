using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Filiais
{
    [CustomAuthorize("Filiais/Turno")]
    public class TurnoController : BaseController
    {
		#region Construtores

		public TurnoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Privados

        private void AtualizarTurno(Dominio.Entidades.Embarcador.Filiais.Turno turno)
        {
            string descricao = string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? string.Empty : Request.Params("Descricao");
            string observacao = string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? string.Empty : Request.Params("Observacao");
            bool ativo = false;

            bool.TryParse(Request.Params("Status"), out ativo);

            turno.Ativo = ativo;
            turno.Descricao = descricao;
            turno.Observacao = observacao;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                var propriedadeOrdenar = (grid.header[grid.indiceColunaOrdena].data == "DescricaoAtivo") ? "Ativo" : grid.header[grid.indiceColunaOrdena].data;
                int totalRegistros = 0;
                var lista = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private dynamic Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var codigoFilial = Request.GetIntParam("Filial");
            var codigoSetor = Request.GetIntParam("Setor");
            var descricao = Request.Params("Descricao");
            var status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);
            var repositorio = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);
            var listaTurno = repositorio.Consultar(descricao, status, codigoFilial, codigoSetor, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(descricao, status, codigoFilial, codigoSetor);

            return (
                from turno in listaTurno
                select new
                {
                    turno.Codigo,
                    turno.Descricao,
                    turno.DescricaoAtivo
                }
            ).ToList();
        }

        private string ValidarTurno(Dominio.Entidades.Embarcador.Filiais.Turno turno)
        {
            if (string.IsNullOrWhiteSpace(turno.Descricao))
                return Localization.Resources.Filiais.Turno.DescricaoObrigatoria;

            if (turno.Descricao.Length > 200)
                return Localization.Resources.Filiais.Turno.DescricaoNaoPodePassarDuzentosCaracteres;

            if (turno.Observacao.Length > 2000)
                return Localization.Resources.Filiais.Turno.DescricaoNaoPodePassarDoisMilCaracteres;

            return string.Empty;
        }

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);
                var turno = new Dominio.Entidades.Embarcador.Filiais.Turno();

                AtualizarTurno(turno);

                string mensagemErro = ValidarTurno(turno);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Inserir(turno, Auditado);

                SalvarHorarioAcesso(turno, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Filiais.Turno.OcorreuFalhaAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);
                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var turno = repositorio.BuscarPorCodigo(codigo, true);

                if (turno == null)
                    return new JsonpResult(false, true, Localization.Resources.Filiais.Turno.NaoFoiPossivelEncontrarRegistro);

                AtualizarTurno(turno);

                string mensagemErro = ValidarTurno(turno);

                if (!string.IsNullOrEmpty(mensagemErro))
                    return new JsonpResult(false, true, mensagemErro);

                repositorio.Atualizar(turno, Auditado);

                SalvarHorarioAcesso(turno, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Filiais.Turno.OcorreuFalhaAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Filiais.Turno repositorio = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);
                Repositorio.Embarcador.Filiais.TurnoHorarioAcesso repositorioTurnoHorarioAcesso = new Repositorio.Embarcador.Filiais.TurnoHorarioAcesso(unitOfWork);

                int codigo = 0;

                int.TryParse(Request.Params("Codigo"), out codigo);

                var turno = repositorio.BuscarPorCodigo(codigo);

                if (turno == null)
                    return new JsonpResult(false, true, Localization.Resources.Filiais.Turno.NaoFoiPossivelEncontrarRegistro);

                List<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso> horariosAcessos = repositorioTurnoHorarioAcesso.BuscarPorTurno(codigo);

                return new JsonpResult(new
                {
                    turno.Codigo,
                    turno.Descricao,
                    Status = turno.Ativo,
                    Observacao = turno.Observacao ?? string.Empty,
                    HorariosAcessos = (
                        from obj in horariosAcessos
                        select new 
                        { 
                            obj.Codigo,
                            DiasDaSemana = Newtonsoft.Json.JsonConvert.SerializeObject(obj.DiasDaSemana.Select(o => (int)o).ToList()),
                            DiasDaSemanaDescricao = string.Join(", ", obj.DiasDaSemana.Select(o => Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemanaHelper.ObterDescricao(o)).ToList()),
                            HoraInicial = obj.HoraInicial.HasValue ? obj.HoraInicial.Value.ToString(@"hh\:mm") : "",
                            HoraFinal = obj.HoraFinal.HasValue ? obj.HoraFinal.Value.ToString(@"hh\:mm") : "",
                            HorarioDescricao = string.Format(Localization.Resources.Filiais.Turno.XAteX, (obj.HoraInicial.HasValue ? obj.HoraInicial.Value.ToString(@"hh\:mm") : ""), (obj.HoraFinal.HasValue ? obj.HoraFinal.Value.ToString(@"hh\:mm") : ""))
                        }
                    ).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Filiais.Turno.OcorreuFalhaConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                var turno = repositorio.BuscarPorCodigo(codigo);

                if (turno == null)
                    return new JsonpResult(false, true, Localization.Resources.Filiais.Turno.NaoFoiPossivelEncontrarRegistro);

                repositorio.Deletar(turno, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Filiais.Turno.OcorreuFalhaRemover);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Filiais.Turno.OcorreuFalhaGerar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Filiais.Turno.OcorreuFalhaExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        public void SalvarHorarioAcesso(Dominio.Entidades.Embarcador.Filiais.Turno turno, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.TurnoHorarioAcesso repositorioTurnoHorarioAcesso = new Repositorio.Embarcador.Filiais.TurnoHorarioAcesso(unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso> horariosAcessos = repositorioTurnoHorarioAcesso.BuscarPorTurno(turno.Codigo);
            
            dynamic dynHorariosAcessos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("HorariosAcessos"));

            if (horariosAcessos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var horarioAcesso in dynHorariosAcessos)
                    if (horarioAcesso.Codigo != null)
                        codigos.Add((int)horarioAcesso.Codigo);

                List<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso> horariosAcessosDeletar = (from obj in horariosAcessos where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso horarioAcessoDeletar in horariosAcessosDeletar)
                    repositorioTurnoHorarioAcesso.Deletar(horarioAcessoDeletar, Auditado);
            }
            else
                horariosAcessos = new List<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso>();

            foreach (var horarioAcesso in dynHorariosAcessos)
            {
                Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso turnoHorarioAcesso = horarioAcesso.Codigo != null ? repositorioTurnoHorarioAcesso.BuscarPorCodigo((int)horarioAcesso.Codigo) : null;
                if (turnoHorarioAcesso == null)
                    turnoHorarioAcesso = new Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso();

                TimeSpan.TryParse((string)horarioAcesso.HoraInicial, out TimeSpan horaInicial);
                TimeSpan.TryParse((string)horarioAcesso.HoraFinal, out TimeSpan horaFinal);
                dynamic dynDiasDaSemana = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)horarioAcesso.DiasDaSemana);

                turnoHorarioAcesso.HoraInicial = horaInicial != null ? horaInicial : TimeSpan.Zero;
                turnoHorarioAcesso.HoraFinal = horaFinal != null ? horaFinal : TimeSpan.Zero;
                turnoHorarioAcesso.DiasDaSemana = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>();

                if (dynDiasDaSemana?.Count > 0)
                {
                    foreach (dynamic codigoDiaDaSemana in dynDiasDaSemana)
                        turnoHorarioAcesso.DiasDaSemana.Add(((string)codigoDiaDaSemana).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana>());
                }

                turnoHorarioAcesso.Turno = turno;

                if (turnoHorarioAcesso.Codigo > 0)
                    repositorioTurnoHorarioAcesso.Atualizar(turnoHorarioAcesso, Auditado);
                else
                    repositorioTurnoHorarioAcesso.Inserir(turnoHorarioAcesso, Auditado);
        
            }
        }

        #endregion
    }
}
