using Kinyx.Jobs;
using Quartz;
using ServicoQuartz.Jobs;
using static Quartz.Logging.OperationName;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<ExportarImagensJob>();
        builder.Services.AddQuartzWithUI();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.UseQuartzUI();

        var schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        IScheduler scheduler = schedulerFactory.GetScheduler().Result;

        IJobDetail jobExportarImagens = JobBuilder.Create<ExportarImagensJob>()
                .WithIdentity("MEU_JOBAO")
                .PersistJobDataAfterExecution(true)
                /*.SetJobData(new JobDataMap()
                {
                    { "CONFIG_ITEM", jobData }
                })*/
                .Build();

        scheduler.AddJob(jobExportarImagens, true, true);

        // Gerar agendamento
        ITrigger trigger = TriggerBuilder.Create()
        .ForJob(jobExportarImagens)
        .WithIdentity("TRIGGAO")
                .WithSchedule(CronScheduleBuilder.CronSchedule("0/10 * * * * ?"))
        .StartNow()
        .Build();

        // Adicionar tarefa;
        scheduler.ScheduleJob(trigger);

        app.Run();
    }
}