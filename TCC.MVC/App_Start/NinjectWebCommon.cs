[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(TCC.MVC.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(TCC.MVC.App_Start.NinjectWebCommon), "Stop")]

namespace TCC.MVC.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using System;
    using System.Web;
    using TCC.Domain.Entities;
    using TCC.Domain.Interfaces.Repositories;
    using TCC.Domain.Interfaces.Services;
    using TCC.Domain.Services;
    using TCC.Infra.Data.Repositories;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<Chat>().To<Chat>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<IVagaService>().To<VagaService>();
            kernel.Bind<IGaragemService>().To<GaragemService>();
            kernel.Bind<IVeiculoService>().To<VeiculoService>();
            kernel.Bind<IEventoService>().To<EventoService>();
            kernel.Bind<ILocacaoService>().To<LocacaoService>();
            kernel.Bind<IChatService>().To<ChatService>();
            kernel.Bind<IAvaliacaoService>().To<AvaliacaoService>();
            kernel.Bind<IFotoService>().To<FotoService>();
            kernel.Bind<IPagamentoService>().To<PagamentoService>();
            kernel.Bind<IDadosPagamentoService>().To<DadosPagamentoService>();
            kernel.Bind<INotificacaoEmailSmsService>().To<NotificacaoEmailSmsService>();
            kernel.Bind<ITransferenciaService>().To<TransferenciaService>();
            kernel.Bind<IDadosBancarioService>().To<DadosBancarioService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<IVagaRepository>().To<VagaRepository>();
            kernel.Bind<IGaragemRepository>().To<GaragemRepository>();  
            kernel.Bind<IVeiculoRepository>().To<VeiculoRepository>();
            kernel.Bind<IEventoRepository>().To<EventoRepository>();
            kernel.Bind<ILocacaoRepository>().To<LocacaoRepository>();
            kernel.Bind<IChatRepository>().To<ChatRepository>();
            kernel.Bind<IAvaliacaoRepository>().To<AvaliacaoRepository>();
            kernel.Bind<IFotoRepository>().To<FotoRepository>();
            kernel.Bind<IPagamentoRepository>().To<PagamentoRepository>();
            kernel.Bind<IDadosPagamentoRepository>().To<DadosPagamentoRepository>();
            kernel.Bind<INotificacaoEmailSmsRepository>().To<NotificacaoEmailSmsRepository>();
            kernel.Bind<ITransferenciaRepository>().To<TransferenciaRepository>();
            kernel.Bind<IDadosBancarioRepository>().To<DadosBancarioRepository>();
        }
    }
}