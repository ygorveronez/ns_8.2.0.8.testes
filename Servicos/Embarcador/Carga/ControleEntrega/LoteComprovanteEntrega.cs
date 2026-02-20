using Repositorio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class LoteComprovanteEntrega : ServicoBase
    {        
        public LoteComprovanteEntrega() : base() { }
        
        public static void CriarAutomaticamenteLoteByCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, UnitOfWork unitOfWork)
        {
            unitOfWork.Start();

            try
            {
                var repLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.LoteComprovanteEntrega(unitOfWork);
                var repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                var repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                var repCargaEntregaLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega(unitOfWork);
                var repCanhotoLoteComprovanteEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CanhotoLoteComprovanteEntrega(unitOfWork);
                var repDadosRecebedor = new Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor(unitOfWork);

                // Criar o lote
                var lote = new Dominio.Entidades.Embarcador.Cargas.LoteComprovanteEntrega();
                lote.Carga = cargaEntrega.Carga;
                lote.DataCriacao = DateTime.Now;

                // Criar um CargaEntregaLoteComprovanteEntrega
                var cargaEntregaLote = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaLoteComprovanteEntrega();
                cargaEntregaLote.CargaEntrega = cargaEntrega;
                cargaEntregaLote.LoteComprovanteEntrega = lote;
                cargaEntregaLote.Latitude = cargaEntrega.Cliente?.Latitude ?? "0";
                cargaEntregaLote.Longitude = cargaEntrega.Cliente?.Longitude ?? "0";

                if(cargaEntrega.DadosRecebedor != null)
                {
                    cargaEntregaLote.DadosRecebedor = cargaEntrega.DadosRecebedor;
                } else
                {
                    var dadosRecebedorVazio = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor();
                    dadosRecebedorVazio.Nome = "";
                    dadosRecebedorVazio.CPF = "";
                    dadosRecebedorVazio.DataEntrega = DateTime.Now;
                    repDadosRecebedor.Inserir(dadosRecebedorVazio);
                    cargaEntregaLote.DadosRecebedor = dadosRecebedorVazio;
                }

                // Para cada canhoto da cargaEntrega, preencher um CanhotoLoteComprovanteEntrega
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaCarga = repCanhoto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                var canhotos = obterCanhotosCargaEntrega(cargaEntrega.Codigo, canhotosDaCarga, cargaEntregaNotasFiscais);

                repLoteComprovanteEntrega.Inserir(lote);
                repCargaEntregaLoteComprovanteEntrega.Inserir(cargaEntregaLote);

                foreach (var canhoto in canhotos)
                {
                    var canhotoLote = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CanhotoLoteComprovanteEntrega();
                    canhotoLote.Canhoto = canhoto;
                    canhotoLote.LoteComprovanteEntrega = lote;
                    canhotoLote.CargaEntregaLoteComprovanteEntrega = cargaEntregaLote;
                    repCanhotoLoteComprovanteEntrega.Inserir(canhotoLote);
                }

                unitOfWork.CommitChanges();

            } catch (Exception e)
            {
                unitOfWork.Rollback();
            }
            
        }

        public static string ObterCaminhoImagemNotaFiscal(Dominio.Entidades.Embarcador.Cargas.XMLNotaFiscalComprovanteEntrega notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {            
            return Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoXMLNotaFiscalComprovanteEntrega;
        }

        private Bitmap ResizeImage(Image image, int newWidth)
        {
            int newHeight = (image.Height * newWidth) / image.Width;
            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private static List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> obterCanhotosCargaEntrega(int codigoCargaEntrega, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosDaCarga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais)
        {
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosEntrega = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscaisParada = (from obj in cargaEntregaNotasFiscais where obj.CargaEntrega.Codigo == codigoCargaEntrega select obj).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscaisParada)
            {

                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = (from obj in canhotosDaCarga
                                                                         where
                                                 (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)
                                                 || (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso
                                                 && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo)
                                                 )
                                                                         select obj).FirstOrDefault();

                if (canhoto != null && !canhotosEntrega.Contains(canhoto))
                    canhotosEntrega.Add(canhoto);
            }

            return canhotosEntrega;
        }
    }
}
