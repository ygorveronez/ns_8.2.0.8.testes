using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class LeilaoController : ApiController
    {

        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("leilao.aspx") select obj).FirstOrDefault();
        }

        #endregion


        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Repositorio.Embarcador.Cargas.LeilaoParticipante repLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante> leiloesParticipante = repLeilaoParticipante.ConsultarLeiloesParaParticipante(this.EmpresaUsuario.Codigo, inicioRegistros, 50);
                int contNumeroRegistros = repLeilaoParticipante.ContarLeiloesParaParticipante(this.EmpresaUsuario.Codigo);

                var lista = from obj in leiloesParticipante
                            select new
                            {
                                obj.Codigo,
                                OrigemDestino = obterOrigemDestino(obj.Leilao, unitOfWork),
                                TipoVeiculo = obterModeloVeicular(obj.Leilao, unitOfWork),
                                valorIncial = obj.Leilao.ValorInicial.ToString("n2"),
                                ValorOfertado = obj.ValorLance > 0 ? obj.ValorLance.ToString("n2") : "Não ouve oferta"
                            };

                return Json(lista, true, null, new string[] { "Código", "Origem e Destino|50", "Tipo de Veiculo|18", "Valor Inicial|9", "Valor Ofertado|9" }, contNumeroRegistros);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os leilões.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AtualizarOferta()
        {
            if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                return Json<bool>(false, false, "Permissão para acesso a oferta negada!");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                
                int codigoLeilaoParticipante = 0;
                int.TryParse(Request.Params["Codigo"], out codigoLeilaoParticipante);

                decimal valorOferta = 0;
                decimal.TryParse(Request.Params["ValorOferta"], out valorOferta);

                

                Repositorio.Embarcador.Cargas.LeilaoParticipante repLeilaoParticipante = new Repositorio.Embarcador.Cargas.LeilaoParticipante(unitOfWork);
                Repositorio.Embarcador.Cargas.Leilao repLeilao = new Repositorio.Embarcador.Cargas.Leilao(unitOfWork);
                Repositorio.Embarcador.Cargas.LeilaoParticipanteHistoricoLance repLeilaoParticipanteHistoricoLance = new Repositorio.Embarcador.Cargas.LeilaoParticipanteHistoricoLance(unitOfWork);
              
                Dominio.Entidades.Embarcador.Cargas.LeilaoParticipante leilaoParticipante = repLeilaoParticipante.BuscarPorCodigo(codigoLeilaoParticipante);
                Dominio.Entidades.Embarcador.Cargas.LeilaoParticipanteHistoricoLance leilaoParticipanteHistoricoLance = new Dominio.Entidades.Embarcador.Cargas.LeilaoParticipanteHistoricoLance();


                if (valorOferta <= leilaoParticipante.Leilao.ValorInicial)
                {

                    if ((leilaoParticipante.Leilao.DataParaEncerramentoLeilao >= DateTime.Now || leilaoParticipante.Leilao.DataParaEncerramentoLeilao == null) && leilaoParticipante.Leilao.SituacaoLeilao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLeilao.iniciado)
                    {

                        leilaoParticipanteHistoricoLance.DataLance = DateTime.Now;
                        leilaoParticipanteHistoricoLance.LeilaoParticipante = leilaoParticipante;
                        leilaoParticipanteHistoricoLance.ValorLance = valorOferta;
                        repLeilaoParticipanteHistoricoLance.Inserir(leilaoParticipanteHistoricoLance);

                        leilaoParticipante.ValorLance = valorOferta;
                        if (leilaoParticipante.DataLance != null)
                        {
                            leilaoParticipante.DataLance = DateTime.Now;
                            repLeilaoParticipante.Atualizar(leilaoParticipante);
                        }
                        else
                        {
                            leilaoParticipante.DataLance = DateTime.Now;
                            repLeilaoParticipante.Inserir(leilaoParticipante);
                            leilaoParticipante.Leilao.NumeroDeLances += 1;
                        }

                        if (leilaoParticipante.Leilao.MenorLance > leilaoParticipante.ValorLance || leilaoParticipante.Leilao.MenorLance == 0)
                        {
                            leilaoParticipante.Leilao.MenorLance = leilaoParticipante.ValorLance;
                        }

                        repLeilao.Atualizar(leilaoParticipante.Leilao);

                        unitOfWork.CommitChanges();
                        return Json<bool>(true, true);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return Json<bool>(false, true, "O leilão foi encerrado, por isso não é possível mais dar lances.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return Json<bool>(false, true, "O valor do lance deve ser menor que " + leilaoParticipante.Leilao.ValorInicial.ToString("n2") + ".");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao fazer a oferta.");
            }
        }


        private string obterModeloVeicular(Dominio.Entidades.Embarcador.Cargas.Leilao leilao, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorLeilao(leilao.Codigo);
            return cargaLeilao.Carga.ModeloVeicularCarga.Descricao;
        }

        private string obterOrigemDestino(Dominio.Entidades.Embarcador.Cargas.Leilao leilao, Repositorio.UnitOfWork unitOfWork)
        {
            string origem = "";
            string destino = "";
            int i = 0;

            Repositorio.Embarcador.Cargas.CargaLeilao repCargaLeilao = new Repositorio.Embarcador.Cargas.CargaLeilao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaLeilao cargaLeilao = repCargaLeilao.BuscarPorLeilao(leilao.Codigo);

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(cargaLeilao.Carga.Codigo);

            origem = (from obj in cargaPedidos select obj.Pedido.Origem.DescricaoCidadeEstado).FirstOrDefault();
            List<Dominio.Entidades.Localidade> destinos = (from obj in cargaPedidos select obj.Pedido.Destino).Distinct().ToList();
            foreach (Dominio.Entidades.Localidade locDestino in destinos)
            {
                i++;

                destino += locDestino.DescricaoCidadeEstado;

                if (destinos.Count > i)
                    destino += " / ";
            }

            string entrega = "Entrega";
            if (i > 1)
                entrega = "Entregas";

            return origem + " até " + destino + " (" + destinos.Count() + " " + entrega + ")";
        }

    }



}
