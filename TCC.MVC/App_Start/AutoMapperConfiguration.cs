 using AutoMapper;
using System;
using TCC.Domain.Entities;
using TCC.MVC.ViewModels;

namespace TCC.MVC.App_Start
{
    public class AutoMapperConfiguration
    {
        public static void MapsConfig()
        {
            Mapper.Initialize(cfg =>
            {
                //Domain To Application
                cfg.CreateMap<Usuario, UsuarioViewModel>()
                    .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => src.DataCadastro.ToShortDateString()))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId.ToString()));
                                
                cfg.CreateMap<Garagem, GaragemViewModel>()
                    .ForMember(dest => dest.GaragemId, opt => opt.MapFrom(src => src.GaragemId.ToString()))
                    .ForMember(dest => dest.NumeroRua, opt => opt.MapFrom(src => src.NumeroRua.ToString()))
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude.ToString()))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude.ToString()));
                                
                cfg.CreateMap<Vaga, VagaViewModel>()
                    .ForMember(dest => dest.VagaId, opt => opt.MapFrom(src => src.VagaId.ToString()))
                    .ForMember(dest => dest.PrecoAvulso, opt => opt.MapFrom(src => src.PrecoAvulso.ToString()))
                    .ForMember(dest => dest.AposPrimeiraHora, opt => opt.MapFrom(src => src.AposPrimeiraHora.ToString()))
                    .ForMember(dest => dest.PrecoMensal, opt => opt.MapFrom(src => src.PrecoMensal.ToString()))
                    .ForMember(dest => dest.PrecoDiaria, opt => opt.MapFrom(src => src.PrecoDiaria.ToString()))
                    .ForMember(dest => dest.GaragemId, opt => opt.MapFrom(src => src.GaragemId.ToString()))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId.ToString()));

                cfg.CreateMap<Veiculo, VeiculoViewModel>()
                    .ForMember(dest => dest.VeiculoId, opt => opt.MapFrom(src => src.VeiculoId.ToString()))
                    .ForMember(dest => dest.IdModeloFipe, opt => opt.MapFrom(src => src.IdModeloFipe.ToString()))
                    .ForMember(dest => dest.IdMarcaFipe, opt => opt.MapFrom(src => src.IdMarcaFipe.ToString()))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId.ToString()));

                cfg.CreateMap<Evento, EventoViewModel>()
                    .ForMember(dest => dest.EventoId, opt => opt.MapFrom(src => src.EventoId.ToString()))
                    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Numero.ToString()))
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude.ToString()))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude.ToString()))
                    .ForMember(dest => dest.DataHoraInicio, opt => opt.MapFrom(src => src.DataHoraInicio.ToString()))
                    .ForMember(dest => dest.DataHoraFim, opt => opt.MapFrom(src => src.DataHoraFim.ToString()))
                    .ForMember(dest => dest.CategoriaEvento, opt => opt.MapFrom(src => src.CategoriaEvento.ToString()));

                cfg.CreateMap<Locacao, LocacaoViewModel>()
                    .ForMember(dest => dest.LocacaoId, opt => opt.MapFrom(src => src.LocacaoId.ToString()))
                    .ForMember(dest => dest.DiaHoraInicio, opt => opt.MapFrom(src => src.DiaHoraInicio.ToString()))
                    .ForMember(dest => dest.DiaHoraFim, opt => opt.MapFrom(src => src.DiaHoraFim.ToString()))
                    .ForMember(dest => dest.DiaHoraSaida, opt => opt.MapFrom(src => src.DiaHoraSaida.ToString()))
                    .ForMember(dest => dest.Multa, opt => opt.MapFrom(src => src.Multa.ToString()))
                    .ForMember(dest => dest.ValorLocacaoLocador, opt => opt.MapFrom(src => src.ValorLocacaoLocador.ToString()))
                    .ForMember(dest => dest.ValorLocacaoLocatario, opt => opt.MapFrom(src => src.ValorLocacaoLocatario.ToString()))
                    .ForMember(dest => dest.ModalidadeLocacao, opt => opt.MapFrom(src => src.ModalidadeLocacao.ToString()))
                    .ForMember(dest => dest.QuemCancelou, opt => opt.MapFrom(src => src.QuemCancelou.ToString()))
                    .ForMember(dest => dest.VeiculoId, opt => opt.MapFrom(src => src.VeiculoId.ToString()))
                    .ForMember(dest => dest.VagaId, opt => opt.MapFrom(src => src.VagaId.ToString()))
                    .ForMember(dest => dest.TaxaLucro, opt => opt.MapFrom(src => src.TaxaLucro.ToString()));

                cfg.CreateMap<Avaliacao, AvaliacaoViewModel>()
                    .ForMember(dest => dest.Nota, opt => opt.MapFrom(src => src.Nota.ToString()))
                    .ForMember(dest => dest.AvaliacaoId, opt => opt.MapFrom(src => src.AvaliacaoId.ToString()))
                    .ForMember(dest => dest.UsuarioAvaliadorId, opt => opt.MapFrom(src => src.UsuarioAvaliadorId.ToString()))
                    .ForMember(dest => dest.LocacaoId, opt => opt.MapFrom(src => src.LocacaoId.ToString()));

                cfg.CreateMap<Foto, FotoViewModel>()
                    .ForMember(dest => dest.FotoId, opt => opt.MapFrom(src => src.FotoId.ToString()))
                    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
                    .ForMember(dest => dest.VagaId, opt => opt.MapFrom(src => src.VagaId.ToString()))
                    .ForMember(dest => dest.EventoId, opt => opt.MapFrom(src => src.EventoId.ToString()));

                cfg.CreateMap<DadosPagamento, DadosPagamentoViewModel>()
                    .ForMember(dest => dest.DadosPagamentoId, opt => opt.MapFrom(src => src.DadosPagamentoId.ToString()))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId.ToString()));

                cfg.CreateMap<Pagamento, PagamentoViewModel>()
                    .ForMember(dest => dest.PagamentoId, opt => opt.MapFrom(src => src.PagamentoId.ToString()))
                    .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor.ToString()))
                    .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => src.DataPagamento.ToString()))
                    .ForMember(dest => dest.LocacaoId, opt => opt.MapFrom(src => src.LocacaoId.ToString()))
                    .ForMember(dest => dest.DadosPagamentoId, opt => opt.MapFrom(src => src.DadosPagamentoId.ToString()));

                cfg.CreateMap<DadosBancario, DadosBancarioViewModel>()
                    .ForMember(dest => dest.DadosBancarioId, opt => opt.MapFrom(src => src.DadosBancarioId.ToString()))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId.ToString()));

                cfg.CreateMap<Transferencia, TransferenciaViewModel>()
                    .ForMember(dest => dest.TransferenciaId, opt => opt.MapFrom(src => src.TransferenciaId.ToString()))
                    .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.Valor.ToString()))
                    .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => src.DataPagamento.ToString()))
                    .ForMember(dest => dest.DataEfetivado, opt => opt.MapFrom(src => src.DataEfetivado.ToString()))
                    .ForMember(dest => dest.DadosBancarioId, opt => opt.MapFrom(src => src.DadosBancarioId.ToString()));

                //Application To Domain
                cfg.CreateMap<UsuarioViewModel, Usuario>()
                    .ForMember(dest => dest.DataCadastro, opt => opt.MapFrom(src => Convert.ToDateTime(src.DataCadastro)))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => int.Parse(src.UsuarioId)));
                    
                cfg.CreateMap<GaragemViewModel, Garagem>()
                    .ForMember(dest => dest.GaragemId, opt => opt.MapFrom(src => int.Parse(src.GaragemId)))
                    .ForMember(dest => dest.NumeroRua, opt => opt.MapFrom(src => int.Parse(src.NumeroRua)))
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => Convert.ToDecimal(src.Latitude)))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => Convert.ToDecimal(src.Longitude)));
                                
                cfg.CreateMap<VagaViewModel, Vaga>()
                    .ForMember(dest => dest.VagaId, opt => opt.MapFrom(src => int.Parse(src.VagaId)))
                    .ForMember(dest => dest.PrecoAvulso, opt => opt.MapFrom(src => Convert.ToDouble(src.PrecoAvulso)))
                    .ForMember(dest => dest.AposPrimeiraHora, opt => opt.MapFrom(src => Convert.ToDouble(src.AposPrimeiraHora)))
                    .ForMember(dest => dest.PrecoMensal, opt => opt.MapFrom(src => Convert.ToDouble(src.PrecoMensal)))
                    .ForMember(dest => dest.PrecoDiaria, opt => opt.MapFrom(src => Convert.ToDouble(src.PrecoDiaria)))
                    .ForMember(dest => dest.GaragemId, opt => opt.MapFrom(src => int.Parse(src.GaragemId)))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => int.Parse(src.UsuarioId)));
                    
                cfg.CreateMap<VeiculoViewModel, Veiculo>()
                    .ForMember(dest => dest.VeiculoId, opt => opt.MapFrom(src => int.Parse(src.VeiculoId)))
                    .ForMember(dest => dest.IdModeloFipe, opt => opt.MapFrom(src => int.Parse(src.IdModeloFipe)))
                    .ForMember(dest => dest.IdMarcaFipe, opt => opt.MapFrom(src => int.Parse(src.IdMarcaFipe)))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => int.Parse(src.UsuarioId)));

                cfg.CreateMap<EventoViewModel, Evento>()
                    .ForMember(dest => dest.EventoId, opt => opt.MapFrom(src => int.Parse(src.EventoId)))
                    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => int.Parse(src.Numero)))
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => Convert.ToDecimal(src.Latitude)))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => Convert.ToDecimal(src.Longitude)))
                    .ForMember(dest => dest.DataHoraInicio, opt => opt.MapFrom(src => DateTime.Parse(src.DataHoraInicio)))
                    .ForMember(dest => dest.DataHoraFim, opt => opt.MapFrom(src => DateTime.Parse(src.DataHoraFim)))
                    .ForMember(dest => dest.CategoriaEvento, opt => opt.MapFrom(src => int.Parse(src.CategoriaEvento)));

                cfg.CreateMap<LocacaoViewModel, Locacao>()
                    .ForMember(dest => dest.LocacaoId, opt => opt.MapFrom(src => int.Parse(src.LocacaoId)))
                    .ForMember(dest => dest.DiaHoraInicio, opt => opt.MapFrom(src => DateTime.Parse(src.DiaHoraInicio)))
                    .ForMember(dest => dest.DiaHoraFim, opt => opt.MapFrom(src => DateTime.Parse(src.DiaHoraFim)))
                    .ForMember(dest => dest.DiaHoraSaida, opt => opt.MapFrom(src => DateTime.Parse(src.DiaHoraSaida)))
                    .ForMember(dest => dest.Multa, opt => opt.MapFrom(src => Convert.ToDecimal(src.Multa.Replace(".", ","))))
                    .ForMember(dest => dest.ValorLocacaoLocador, opt => opt.MapFrom(src => Convert.ToDecimal(src.ValorLocacaoLocador.Replace(".", ","))))
                    .ForMember(dest => dest.ValorLocacaoLocatario, opt => opt.MapFrom(src => Convert.ToDecimal(src.ValorLocacaoLocatario.Replace(".", ","))))
                    .ForMember(dest => dest.ModalidadeLocacao, opt => opt.MapFrom(src => int.Parse(src.ModalidadeLocacao)))
                    .ForMember(dest => dest.QuemCancelou, opt => opt.MapFrom(src => int.Parse(src.QuemCancelou)))
                    .ForMember(dest => dest.VeiculoId, opt => opt.MapFrom(src => int.Parse(src.VeiculoId)))
                    .ForMember(dest => dest.VagaId, opt => opt.MapFrom(src => int.Parse(src.VagaId)))
                    .ForMember(dest => dest.TaxaLucro, opt => opt.MapFrom(src => int.Parse(src.TaxaLucro)));

                cfg.CreateMap<AvaliacaoViewModel, Avaliacao>()
                    .ForMember(dest => dest.Nota, opt => opt.MapFrom(src => int.Parse(src.Nota)))
                    .ForMember(dest => dest.AvaliacaoId, opt => opt.MapFrom(src => int.Parse(src.AvaliacaoId)))
                    .ForMember(dest => dest.UsuarioAvaliadorId, opt => opt.MapFrom(src => int.Parse(src.UsuarioAvaliadorId)))
                    .ForMember(dest => dest.LocacaoId, opt => opt.MapFrom(src => int.Parse(src.LocacaoId)));

                cfg.CreateMap<FotoViewModel, Foto>()
                    .ForMember(dest => dest.FotoId, opt => opt.MapFrom(src => int.Parse(src.FotoId)))
                    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => int.Parse(src.Tipo)))
                    .ForMember(dest => dest.VagaId, opt => opt.MapFrom(src => int.Parse(src.VagaId)))
                    .ForMember(dest => dest.EventoId, opt => opt.MapFrom(src => int.Parse(src.EventoId)));

                cfg.CreateMap<DadosPagamentoViewModel, DadosPagamento>()
                    .ForMember(dest => dest.DadosPagamentoId, opt => opt.MapFrom(src => int.Parse(src.DadosPagamentoId)))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => int.Parse(src.UsuarioId)));

                cfg.CreateMap<PagamentoViewModel, Pagamento>()
                    .ForMember(dest => dest.PagamentoId, opt => opt.MapFrom(src => int.Parse(src.PagamentoId)))
                    .ForMember(dest => dest.LocacaoId, opt => opt.MapFrom(src => int.Parse(src.LocacaoId)))
                    .ForMember(dest => dest.DadosPagamentoId, opt => opt.MapFrom(src => int.Parse(src.DadosPagamentoId)))
                    .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => Convert.ToDecimal(src.Valor.Replace(".", ","))))
                    .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => DateTime.Parse(src.DataPagamento)));

                cfg.CreateMap<DadosBancarioViewModel, DadosBancario>()
                    .ForMember(dest => dest.DadosBancarioId, opt => opt.MapFrom(src => int.Parse(src.DadosBancarioId)))
                    .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => int.Parse(src.UsuarioId)));

                cfg.CreateMap<TransferenciaViewModel, Transferencia>()
                    .ForMember(dest => dest.TransferenciaId, opt => opt.MapFrom(src => int.Parse(src.TransferenciaId)))
                    .ForMember(dest => dest.DadosBancarioId, opt => opt.MapFrom(src => int.Parse(src.DadosBancarioId)))
                    .ForMember(dest => dest.DataPagamento, opt => opt.MapFrom(src => DateTime.Parse(src.DataPagamento)))
                    .ForMember(dest => dest.DataEfetivado, opt => opt.MapFrom(src => DateTime.Parse(src.DataEfetivado)))
                    .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => Convert.ToDecimal(src.Valor.Replace(".", ","))));
            });
        }
    }
}