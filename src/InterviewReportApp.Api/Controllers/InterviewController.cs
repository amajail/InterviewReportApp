using System.Text.Json;
using InterviewReportApp.Api.Models;
using InterviewReportApp.Core.Application.Services;
using InterviewReportApp.Core.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace InterviewReportApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InterviewController : ControllerBase
    {
        private IInterviewReportService _interviewReportService;

        public InterviewController(IInterviewReportService interviewReportService)
        {
            _interviewReportService = interviewReportService;
        }

        // Endpoint to get all questions
        [HttpGet("questions")]
        public IActionResult GetQuestions()
        {
            return Ok(_interviewReportService.GetQuestions());
        }

        // Endpoint to submit interview notes
        [HttpPost("submit-notes")]
        public async Task<IActionResult> SubmitNotes([FromBody] string notes, CancellationToken cancellationToken)
        {
            notes = @"Estuvo estudiando ingles en Australia. Fue a Sídney.
            Dice Mejoro el ingles. Lo encontré trabado y por ahi estaba nervioso.

            15 años de experiencia.
            Tecnologías Microsoft

            Dynamics 365, desktop server, Mvc

            Mantenimiento, freelance. pensiones en Colombia.

            Siempre aprendió al entrar a un proyecto.

            Estuvo 12 años en una misma empresa.

            Y a lo ultimo en una aseguradora, Angular 7 y punto net. 2 años.
            Entro como TL. 22 personas inicialmente. 
            Quedaron 5 al final, había arquitecto, líder técnico, analista y QAs.

            Microservicios, tenían varias bases de datos, sql, oracle.
            Conoce ventajas, distribución, trabajo en equipo.
            Desventajas, muchos ms complejiza el proyecto.

            Duplicar código, podes traer una librería DLL, no trabajo con NuGets.

            No trabajo con api clientes.

            Trabajo con reintentos, 

            Recursos de azure, APIM, app service, blob storage. KeyVault no lo conocía bien.

            Message queue. Service bus, no tiene mucha experiencia.

            Uso swagger y postman, no para QA automation.

            No trabajo con funciones.

            Autorización.
            JWT, Header, información del usuario, no sabia de signature.
            No conocía flujo de OAuth.

            Base de datos.
            Execution plan no sabia.
            EntityFramework. SQL transaction sabia.

            EF vs Procedure, este ultimo ayuda la complejidad.

            Conoce app insights. NLog.

            Patron de diseño, Factory, no lo tenia muy claro.

            Comenta que trabajo con clean.

            No trabajo con graph QL.

            Interfaces, desacoplamiento. 
            Inyecta por IOC, no conocía mucho, ni singleton ni transient ni scoped.

            UnitTest. Trabajas con las interfases, trabajo con mocks.

            Tenian devops en los proyectos y no sabe lo que es CI CD.
            Le segui preguntando y lo supo explicar. No sabia lo que era un artifact.

            Creaban paginas. Con MicrosFrontends, Redux trabajo.";


            var report = await _interviewReportService.GenerateReportAsync(notes, cancellationToken);
            return Ok(report);
        }
    }
}
