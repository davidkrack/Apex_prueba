using Microsoft.AspNetCore.Mvc;
using InvoiceManagement.Api.Services;

namespace InvoiceManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IInvoiceDataService _invoiceDataService;
        private readonly ILogger<DataController> _logger;

        public DataController(IInvoiceDataService invoiceDataService, ILogger<DataController> logger)
        {
            _invoiceDataService = invoiceDataService;
            _logger = logger;
        }

        /// <summary>
        /// Carga las facturas desde un archivo JSON
        /// </summary>
        /// <param name="jsonFilePath">Ruta del archivo JSON</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("load-from-file")]
        public async Task<ActionResult> LoadInvoicesFromFile([FromBody] LoadFromFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FilePath))
                {
                    return BadRequest("La ruta del archivo es requerida");
                }

                var result = await _invoiceDataService.LoadInvoicesFromJsonAsync(request.FilePath);
                
                if (result)
                {
                    return Ok(new { message = "Datos cargados exitosamente", success = true });
                }
                else
                {
                    return BadRequest(new { message = "Error al cargar los datos", success = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el controlador al cargar datos");
                return StatusCode(500, new { message = "Error interno del servidor", success = false });
            }
        }

        /// <summary>
        /// Carga las facturas desde contenido JSON directo
        /// </summary>
        /// <param name="request">Contenido JSON</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("load-from-json")]
        public async Task<ActionResult> LoadInvoicesFromJson([FromBody] LoadFromJsonRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.JsonContent))
                {
                    return BadRequest("El contenido JSON es requerido");
                }

                var result = await _invoiceDataService.LoadInvoicesFromJsonStringAsync(request.JsonContent);
                
                if (result)
                {
                    return Ok(new { message = "Datos cargados exitosamente", success = true });
                }
                else
                {
                    return BadRequest(new { message = "Error al cargar los datos", success = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el controlador al cargar datos desde JSON");
                return StatusCode(500, new { message = "Error interno del servidor", success = false });
            }
        }
    }

    public class LoadFromFileRequest
    {
        public string FilePath { get; set; } = string.Empty;
    }

    public class LoadFromJsonRequest
    {
        public string JsonContent { get; set; } = string.Empty;
    }
}