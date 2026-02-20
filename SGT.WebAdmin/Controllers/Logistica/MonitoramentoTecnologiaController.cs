using System.Dynamic;
using System.Globalization;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MonitoramentoTecnologia")]
    public class MonitoramentoTecnologiaController : BaseController
    {
        #region Construtores

        public MonitoramentoTecnologiaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarRastreadores()
        {
            try
            {

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTecnologia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoTecnologia()
                    {
                        Tecnologia = Request.GetIntParam("TecnologiaRastreador"),
                        SomenteRastreadoresAtivos = Request.GetBoolParam("SomenteRastreadoresAtivos"),
                    };

                    Repositorio.Embarcador.Veiculos.TecnologiaRastreador repositoriotecnologia = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                    IList<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador> listaRastreadores = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador>();
                    listaRastreadores = repositoriotecnologia.consultaRastreadoresTecnologia(filtrosPesquisa);

                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador> listaRastreadoresFiltrados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador>();
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador> listaGerenciadoresFiltrados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador>();

                    bool utilizaAppTrizy = configuracao.UtilizaAppTrizy;

                    foreach (var ras in listaRastreadores)
                    {
                        bool removerRastreadorPosicaoMobileAntigo = ras.Descricao.Contains("(M)") &&
                            ras.Rastreador == EnumTecnologiaRastreador.Mobile &&
                            !utilizaAppTrizy;

                        if (!listaRastreadoresFiltrados.Any(x => x.Rastreador == ras.Rastreador) &&
                            !removerRastreadorPosicaoMobileAntigo &&
                            ras.Rastreador != EnumTecnologiaRastreador.NaoDefinido &&
                            ras.Gerenciadora == EnumTecnologiaGerenciadora.NaoDefinido)
                        {
                            listaRastreadoresFiltrados.Add(ras);
                        }

                        if (!listaGerenciadoresFiltrados.Any(x => x.Gerenciadora == ras.Gerenciadora) &&
                            ras.Gerenciadora != EnumTecnologiaGerenciadora.NaoDefinido)
                        {
                            listaGerenciadoresFiltrados.Add(ras);
                        }
                    }

                    List<dynamic> rastreadores = (from obj in listaRastreadoresFiltrados select DetalheRastreador(obj, configuracao.TempoSemPosicaoParaVeiculoPerderSinal, unitOfWork)).ToList();
                    List<dynamic> gerenciadores = (from obj in listaGerenciadoresFiltrados select DetalheRastreador(obj, configuracao.TempoSemPosicaoParaVeiculoPerderSinal, unitOfWork)).ToList();

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repositorioConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                    string versao = repositorioConfiguracaoMonitoramento.BuscarConfiguracaoPadrao().VersaoMonitoramento;

                    return new JsonpResult(new
                    {
                        Rastreadores = rastreadores,
                        Gerenciadores = gerenciadores,
                        Versao = versao
                    });
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar tecnologias.");
                }
                finally
                {
                    unitOfWork.Dispose();
                }

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }


        private dynamic DetalheRastreador(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoRastreador objeto, int tempoSemPosicoesParaPerderSinal, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic retorno = new ExpandoObject();

            Repositorio.Embarcador.Logistica.PosicaoAtual repositorioposicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioposicaoAtual.BuscarPorCodigo(objeto.CodigoPosicao);

            Random randNum = new Random();
            string local = posicaoAtual.Descricao.Length > 32 ? posicaoAtual.Descricao.Substring(0, 32) + "..." : posicaoAtual.Descricao;

            string tecnologia = objeto.Gerenciadora != EnumTecnologiaGerenciadora.NaoDefinido ? objeto.Gerenciadora.ObterDescricao() : objeto.Rastreador.ObterDescricao();

            string logo = "/img/rastreadores/logo_" + tecnologia.ToLower(new CultureInfo("pt-BR", false)) + ".png";

            if (objeto.Rastreador == EnumTecnologiaRastreador.AutoTrackEmbarcador)
                logo = "/img/rastreadores/logo_autotracEmbarcadorIntegrador.png";

            if (!Utilidades.IO.FileStorageService.Storage.Exists(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory + "/wwwroot" + logo)))
                logo = "/img/rastreadores/logo_nao_encontrado.png";

            retorno.CodigoPosicao = objeto.CodigoPosicao;
            retorno.Tecnologia = tecnologia;
            retorno.Logo = logo;
            retorno.DataProcessada = tempoSemPosicoesParaPerderSinal <= 120 ? objeto.DataUltimaPosicaoProcessada.ToString("dd/MM/yyyy HH:mm:ss") : DateTime.Now.AddMinutes(-randNum.Next(10)).ToString("dd/MM/yyyy HH:mm:ss");
            retorno.DataRecebida = objeto.DataUltimaPosicaoRecebida == objeto.DataUltimaPosicaoRecebida ? "-" : objeto.DataUltimaPosicaoRecebida.ToString("dd/MM/yyyy HH:mm:ss") ?? "-";
            retorno.Latitude = posicaoAtual.Latitude.ToString();
            retorno.Endereco = local;
            retorno.Longitude = posicaoAtual.Longitude.ToString();
            retorno.Placa = posicaoAtual.Veiculo.Placa;
            retorno.Situacao = VerificarStatus(objeto.DataUltimaPosicaoProcessada, tempoSemPosicoesParaPerderSinal);
            retorno.Rastreador = tempoSemPosicoesParaPerderSinal <= 120 ? objeto.DataUltimaPosicaoProcessada.AddMinutes(tempoSemPosicoesParaPerderSinal) > DateTime.Now ? true : false : true;
            return retorno;
        }


        private static string VerificarStatus(DateTime dataposicao, int tempoSemPosicoesParaPerderSinal)
        {
            // Converter as strings para objetos DateTime
            DateTime dt1 = dataposicao;
            DateTime dt2 = DateTime.Now;

            // Calcular a diferença em dias
            TimeSpan diferenca = dt2 - dt1;
            int dias = Math.Abs(diferenca.Days);

            // Verificar o status com base na diferença
            if (dias > 30)
                return "INATIVA";
            else
            {
                if (tempoSemPosicoesParaPerderSinal <= 120)
                {
                    if (dataposicao.AddMinutes(tempoSemPosicoesParaPerderSinal) > DateTime.Now)
                        return "ON-LINE";
                    else
                        return "OFF-LINE";
                }
                else
                    return "ON-LINE";
            }
        }

        #endregion
    }
}
